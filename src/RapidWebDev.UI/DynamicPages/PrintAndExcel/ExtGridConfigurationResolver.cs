/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Eunge, Legal Name: Jian Liu, Email: eunge.liu@RapidWebDev.org

	The GNU Library General Public License (LGPL) used in RapidWebDev is 
	intended to guarantee your freedom to share and change free software - to 
	make sure the software is free for all its users.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Library General Public License (LGPL) for more details.

	You should have received a copy of the GNU Library General Public License (LGPL)
	along with this program.  
	If not, see http://www.rapidwebdev.org/Content/ByUniqueKey/OpenSourceLicense
 ****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Web;

namespace RapidWebDev.UI.DynamicPages.PrintAndExcel
{
	/// <summary>
	/// The implementation to resolve ExtJS grid client configuration from request cookie. 
	/// </summary>
	internal sealed class ExtGridConfigurationResolver
	{
		private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

		/// <summary>
		/// Resolve ExtJS grid client configuration from request cookie. 
		/// </summary>
		/// <returns></returns>
		internal static ExtGridConfiguration Resolve()
		{
			if (HttpContext.Current == null) return null;
			if (string.IsNullOrEmpty(QueryStringUtility.ObjectId)) return null;

			string cookieValue = null;
			foreach (string cookieKey in HttpContext.Current.Request.Cookies.Keys)
			{
				HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieKey];
				if (cookie.Name.Contains(QueryStringUtility.ObjectId) && cookie.Name.Contains("GridViewPanel"))
					cookieValue = cookie.Value;
			}

			if (string.IsNullOrEmpty(cookieValue)) return null;

			string jsonString = TransformCookieValueToJson(cookieValue);
			return ResolveExtGridConfiguration(jsonString);
		}

		private static ExtGridConfiguration ResolveExtGridConfiguration(string jsonString)
		{
			ExtGridConfiguration clientGrid = new ExtGridConfiguration();
			
			IDictionary<string, object> result = serializer.DeserializeObject(jsonString) as IDictionary<string, object>;

			// parse grid columns
			List<ExtGridColumnConfiguration> clientGridColumns = new List<ExtGridColumnConfiguration>();
			clientGrid.Columns = clientGridColumns;
			object[] jsonGridColumns = result["columns"] as object[];
			foreach (IDictionary<string, object> jsonGridColumn in jsonGridColumns)
			{
				clientGridColumns.Add(new ExtGridColumnConfiguration
				{
					ColumnIndex = jsonGridColumn.ContainsKey("id") ? (int)(decimal)jsonGridColumn["id"] : -1,
					Width = jsonGridColumn.ContainsKey("width") ? (int)(decimal)jsonGridColumn["width"] : 0,
					Hidden = jsonGridColumn.ContainsKey("hidden") ? (bool)jsonGridColumn["hidden"] : false
				});
			}

			// parse sorting field/direction
			IDictionary<string, object> jsonSortingFields = result["sort"] as IDictionary<string, object>;
			if (jsonSortingFields.ContainsKey("field"))
				clientGrid.SortedBy = jsonSortingFields["field"].ToString();
			if (jsonSortingFields.ContainsKey("direction"))
				clientGrid.SortedAscendingly = !string.Equals("desc", jsonSortingFields["direction"].ToString(), StringComparison.OrdinalIgnoreCase);

			return clientGrid;
		}

		private static string TransformCookieValueToJson(string cookie)
		{
			Regex regex = new Regex(@"^(a|n|d|b|s|o)\:(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			string decodedString = Microsoft.JScript.GlobalObject.unescape(cookie);
			var match = regex.Match(decodedString);
			if (match == null || match.Groups.Count < 2) return "null"; // non state cookie

			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				var type = match.Groups[1].Value;
				var v = match.Groups[2].Value;
				string[] values;
				string jsonRawValue;
				switch (type)
				{
					case "n":
						double doubleValue;
						if (double.TryParse(v, out doubleValue))
						{
							jsonWriter.WriteValue(doubleValue);
							break;
						}

						throw new ArgumentException();

					case "d":
						DateTime dateTimeValue;
						if (DateTime.TryParse(v, out dateTimeValue))
						{
							jsonWriter.WriteValue(dateTimeValue);
							break;
						}

						throw new ArgumentException();

					case "b":
						jsonWriter.WriteValue(v == "1");
						break;

					case "a":
						jsonWriter.WriteStartArray();
						values = v.Split('^');
						for (int i = 0, len = values.Length; i < len; i++)
						{
							jsonRawValue = TransformCookieValueToJson(values[i]);
							jsonWriter.WriteRawValue(jsonRawValue);
						}

						jsonWriter.WriteEndArray();
						break;

					case "o":
						jsonWriter.WriteStartObject();
						values = v.Split('^');
						for (int i = 0, len = values.Length; i < len; i++)
						{
							var kv = values[i].Split('=');
							if (kv.Length > 1)
							{
								jsonRawValue = TransformCookieValueToJson(kv[1]);
								jsonWriter.WritePropertyName(kv[0]);
								jsonWriter.WriteRawValue(jsonRawValue);
							}
						}

						jsonWriter.WriteEndObject();
						break;

					default:
						return "\"" + v + "\"";
				}
			}

			return jsonBuilder.ToString();
		}
	}
}
