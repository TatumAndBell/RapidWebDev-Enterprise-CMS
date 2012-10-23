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
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using RapidWebDev.Common.Properties;
using System.Globalization;

namespace RapidWebDev.Common.Globalization
{
	/// <summary>
	/// Globalization toolkit
	/// </summary>
	public static class GlobalizationUtility
	{
		private static readonly Regex regexToPickResources = new Regex(@"\$\x20*[^\$]+\x20*,\x20*[^\$]+\x20*\$", RegexOptions.Compiled);

		/// <summary>
		/// Replace globalization variables in input string.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ReplaceGlobalizationVariables(string input)
		{
			if (Kit.IsEmpty(input)) return input;

			MatchCollection matchCollection = regexToPickResources.Matches(input);
			if (matchCollection.Count == 0) return input;

			return FormatResources(input, matchCollection);
		}

		private static string FormatResources(string input, MatchCollection matchCollection)
		{
			StringBuilder returnValue = new StringBuilder();
			int lastMatchEndIndex = 0;
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Match match = matchCollection[i];
				if (match.Index > lastMatchEndIndex)
				{
					string constValue = input.Substring(lastMatchEndIndex, match.Index - lastMatchEndIndex);
					returnValue.Append(constValue);
				}

				string[] matchedTokens = match.Value.Substring(1, match.Value.Length - 2).Split(',');
				if (matchedTokens.Length != 2)
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidFormatGlobalizationVariable, match.Value));

				string namespaceAndClassAndPropertyName = matchedTokens[0];
				string assemblyName = matchedTokens[1].Trim();

				string[] namespaceAndClassAndPropertyNameArray = namespaceAndClassAndPropertyName.Split('.');
				string namespaceAndClass = namespaceAndClassAndPropertyName.Substring(0, namespaceAndClassAndPropertyName.LastIndexOf('.'));
				string propertyName = namespaceAndClassAndPropertyNameArray[namespaceAndClassAndPropertyNameArray.Length - 1];


				string fullTypeName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", namespaceAndClass, assemblyName);
				Type resourceType = Kit.GetType(fullTypeName);

				// if the resource is located in the executing assembly, the property should be Public
				PropertyInfo property = resourceType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

				// if the resource is not located in the executing assembly, the property should be NonPublic
				if (property == null)
					property = resourceType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

				// Not found the property whatever public or non-public, ignores it.
				if (property == null) continue;

				object resourceValue = property.GetValue(null, null);

				returnValue.Append(resourceValue.ToString());
				lastMatchEndIndex = match.Index + match.Length;

				if (i == matchCollection.Count - 1 && lastMatchEndIndex < input.Length)
					returnValue.Append(input.Substring(lastMatchEndIndex));
			}

			return returnValue.ToString();
		}
	}
}

