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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RapidWebDev.Common;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.UI
{
	/// <summary>
	/// Class used to transform data binding field name which is accepted to JavaScript.
	/// </summary>
	public static class FieldNameTransformUtility
	{
		/// <summary>
		/// ___pk
		/// </summary>
		private static readonly string PrimaryKeyFieldSubfix = "___pk";
		/// <summary>
		/// ___db
		/// </summary>
		private static readonly string DataBoundFieldSubfix = "___db";
		/// <summary>
		/// vf___
		/// </summary>
		private static readonly string ViewFragmentFieldPrefix = "vf___";
		/// <summary>
		/// ___p
		/// </summary>
		private static readonly string SubPropertySeparator = "___p";
		/// <summary>
		/// ___idx
		/// </summary>
		private static readonly string PropertyIndexerNameSeparator = "___idx";
		private static readonly Regex regexToMatchIndexerProperty = new Regex(@"^\[\""[^\""\[\]]+\""]", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
		private static readonly Regex regexToMatchDictionaryProperty = new Regex(@"\[\""[^\""\[\]]+\""]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly Regex regexToMatchDataBoundFieldName = new Regex(@"^[\S\s]+__db\d+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>
		/// Transform primary key field name.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static string PrimaryKeyFieldName(string fieldName)
		{
			Kit.NotNull(fieldName, "fieldName");

			string trimmedFieldName = fieldName.Trim().Replace(".", SubPropertySeparator);
			string fieldNameWithoutIndexer = FieldNameTransformUtility.TransformIndexerToCommonPropertyName(trimmedFieldName);
			return fieldNameWithoutIndexer + FieldNameTransformUtility.PrimaryKeyFieldSubfix;
		}

		/// <summary>
		/// Get data bound name used for client to render data.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static string DataBoundFieldName(string fieldName, int index)
		{
			Kit.NotNull(fieldName, "fieldName");

			string trimmedFieldName = fieldName.Trim().Replace(".", SubPropertySeparator);
			string fieldNameWithoutIndexer = FieldNameTransformUtility.TransformIndexerToCommonPropertyName(trimmedFieldName);
			return fieldNameWithoutIndexer + FieldNameTransformUtility.DataBoundFieldSubfix + index;
		}

		/// <summary>
		/// Get field name for view fragment of grid panel.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static string ViewFragmentFieldName(string fieldName)
		{
			Kit.NotNull(fieldName, "fieldName");

			string trimmedFieldName = fieldName.Trim().Replace(".", SubPropertySeparator);
			string fieldNameWithoutIndexer = FieldNameTransformUtility.TransformIndexerToCommonPropertyName(trimmedFieldName);
			return fieldNameWithoutIndexer + FieldNameTransformUtility.ViewFragmentFieldPrefix;
		}

		/// <summary>
		/// Resolve original configured field name from the gridview panel configuration.
		/// </summary>
		/// <param name="gridViewPanelConfiguration"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static string ResolveOriginalFieldName(this GridViewPanelConfiguration gridViewPanelConfiguration, string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName)) return fieldName;

			if (!regexToMatchDataBoundFieldName.IsMatch(fieldName))
				return fieldName;

			int index = 0;
			foreach (GridViewFieldConfiguration gridViewField in gridViewPanelConfiguration.Fields)
			{
				string expectedDataBoundFieldName = FieldNameTransformUtility.DataBoundFieldName(gridViewField.FieldName, index++);
				if (expectedDataBoundFieldName == fieldName)
					return gridViewField.SortingFieldName ?? gridViewField.FieldName;
			}

			return null;
		}

		/// <summary>
		/// Transform indexer properties (e.g. 'Properties["Name"]', '["Sex"]') to common property names. 
		/// E.g. 'Properties["Name"]' -&gt; 'Properties___idxName', '["Sex"]' -&gt; 'this___idxSex'
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private static string TransformIndexerToCommonPropertyName(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName)) return "";

			string processedFieldName = propertyName.Trim();

			// The property is indexer property likes ["PropertyName"]
			Match matchIndexerProperty = regexToMatchIndexerProperty.Match(processedFieldName);
			if (matchIndexerProperty != null && matchIndexerProperty.Success)
			{
				string result = "this" + PropertyIndexerNameSeparator + matchIndexerProperty.Value.Substring(2, matchIndexerProperty.Length - 4);
				int lastIndex = matchIndexerProperty.Index + matchIndexerProperty.Length;
				if (lastIndex < processedFieldName.Length - 1)
					result += processedFieldName.Substring(lastIndex);

				processedFieldName = result;
			}

			// The property is dictionary typed likes Properties["PropertyName"]
			MatchCollection matchResults = regexToMatchDictionaryProperty.Matches(processedFieldName);
			if (matchResults.Count > 0)
			{
				int startIndex = 0;
				string result = "";
				foreach (Match match in matchResults)
				{
					result += processedFieldName.Substring(startIndex, match.Index - startIndex);
					result += PropertyIndexerNameSeparator + match.Value.Substring(2, match.Length - 4);
					startIndex = match.Index + match.Length;
				}

				if (startIndex < processedFieldName.Length - 1)
					result += processedFieldName.Substring(startIndex);

				return result;
			}

			return processedFieldName;
		}
	}
}
