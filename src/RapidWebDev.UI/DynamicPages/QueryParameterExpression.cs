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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Query parameter expression
	/// </summary>
	public class QueryParameterExpression
	{
		/// <summary>
		/// Gets query field configuration in dynamic page configuration file.
		/// </summary>
		public QueryFieldConfiguration Configuration { get; private set; }

		/// <summary>
		/// Gets query field name
		/// </summary>
		public string FieldName { get; private set; }

		/// <summary>
		/// Gets Criteria operator between field name and control value.
		/// </summary>
		public QueryFieldOperators Operator { get; private set; }

		/// <summary>
		/// Gets query value.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		/// Construct expression by a series of parameters.
		/// </summary>
		/// <param name="fieldName">Query expression field name</param>
		/// <param name="op">Query expression operator</param>
		/// <param name="value">Query expression value</param>
		public QueryParameterExpression(string fieldName, QueryFieldOperators op, object value)
		{
			this.FieldName = fieldName;
			this.Operator = op;
			this.Value = value;
		}

		/// <summary>
		/// Construct expression by a series of parameters.
		/// </summary>
		/// <param name="fieldName">Query expression field name</param>
		/// <param name="op">Query expression operator</param>
		/// <param name="value">Query expression value</param>
		/// <param name="configuration">Query field configuration</param>
		public QueryParameterExpression(string fieldName, QueryFieldOperators op, object value, QueryFieldConfiguration configuration)
		{
			this.FieldName = fieldName;
			this.Operator = op;
			this.Value = value;
			this.Configuration = configuration;
		}

		/// <summary>
		/// Returns a string that represents the current expression.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string valueString = null;
			if (this.Value != null)
			{
				if (!(this.Value is string) && this.Value is IEnumerable)
				{
					IEnumerable<object> values = (this.Value as IEnumerable).Cast<object>();
					IEnumerable<string> valueStringList = values.Select(v => v != null ? v.ToString() : "");
					valueString = "(" + valueStringList.Concat(",") + ")";
				}
				else
				{
					valueString = this.Value.ToString();
				}
			}

			string label = Kit.IsEmpty(this.Configuration.Control.Label) ? "" : this.Configuration.Control.Label.Replace(":", "").Replace("：", "").Trim();
			string operatorNatureLanguage = TranslateQueryFieldOperator(this.Operator);
			return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", label, operatorNatureLanguage, valueString);
		}

		private static string TranslateQueryFieldOperator(QueryFieldOperators queryFieldOperator)
		{
			switch (queryFieldOperator)
			{
				case QueryFieldOperators.Between:
					return Resources.DP_QueryFieldOperator_Between;

				case QueryFieldOperators.Equal:
					return Resources.DP_QueryFieldOperator_Equal;

				case QueryFieldOperators.GreaterEqualThan:
					return Resources.DP_QueryFieldOperator_GreaterEqualThan;

				case QueryFieldOperators.GreaterThan:
					return Resources.DP_QueryFieldOperator_GreaterThan;

				case QueryFieldOperators.In:
					return Resources.DP_QueryFieldOperator_In;

				case QueryFieldOperators.LessEqualThan:
					return Resources.DP_QueryFieldOperator_LessEqualThan;

				case QueryFieldOperators.LessThan:
					return Resources.DP_QueryFieldOperator_LessThan;

				case QueryFieldOperators.Like:
					return Resources.DP_QueryFieldOperator_Like;

				case QueryFieldOperators.NotIn:
					return Resources.DP_QueryFieldOperator_NotIn;

				case QueryFieldOperators.StartsWith:
					return Resources.DP_QueryFieldOperator_StartsWith;
			}

			return Resources.DP_QueryFieldOperator_Auto;
		}
	}
}

