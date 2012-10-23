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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Query predicate expressions compiler which used to assembly QueryPredicate instance for Linq2SQL solution.
	/// </summary>
	public class Linq2SQLPredicateCompiler : IPredicateCompiler
	{
		/// <summary>
		/// Compile expression collection to query predicate.
		/// </summary>
		/// <param name="expressions"></param>
		/// <returns></returns>
		public LinqPredicate CompileQuery(QueryParameterExpressionCollection expressions)
		{
			if (expressions.Count == 0)
				return null;

			List<string> predicates = new List<string>();
			ArrayList parameters = new ArrayList();
			foreach (QueryParameterExpression exp in expressions)
			{
				QueryFieldOperators op = GetOperator(exp);
				string expressionString = AssemblyExpressionString(exp.FieldName, exp.Operator, exp.Value, parameters);
				predicates.Add(expressionString);
			}

			return new LinqPredicate(predicates.Concat(" AND "), parameters.ToArray());
		}

		/// <summary>
		/// Compile sorting expression to sort predicate.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		public string CompileSortExpression(SortExpression sortExpression)
		{
			if (sortExpression == null || string.IsNullOrEmpty(sortExpression.FieldName)) return null;
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", sortExpression.FieldName, sortExpression.Ascending ? "ASC" : "DESC");
		}

		/// <summary>
		/// Return default operator for specified control type if QueryParameterExpression.Configuration.Control.ControlType is Auto.
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		private static QueryFieldOperators GetOperator(QueryParameterExpression exp)
		{
			if (exp.Operator != QueryFieldOperators.Auto)
				return exp.Operator;

			switch (exp.Configuration.Control.ControlType)
			{
				case ControlConfigurationTypes.CheckBox:
				case ControlConfigurationTypes.ComboBox:
				case ControlConfigurationTypes.RadioGroup:
				case ControlConfigurationTypes.Custom:
					return QueryFieldOperators.Equal;
				case ControlConfigurationTypes.TextBox:
					return QueryFieldOperators.Like;
				case ControlConfigurationTypes.Date:
				case ControlConfigurationTypes.DateTime:
				case ControlConfigurationTypes.Integer:
				case ControlConfigurationTypes.Decimal:
					return QueryFieldOperators.Between;
				case ControlConfigurationTypes.CheckBoxGroup:
					return QueryFieldOperators.In;
			}

			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.DP_ControlTypeNotSupported, exp.Configuration.Control.ControlType));
		}

		/// <summary>
		/// Assembly query field name, operator and value to a predicate expression.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="op"></param>
		/// <param name="expressionValue"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static string AssemblyExpressionString(string fieldName, QueryFieldOperators op, object expressionValue, ArrayList parameters)
		{
			if (expressionValue == null) return null;

			int startParameterIndex, parameterCount;
			List<string> subExpressions;
			string expressionString = "";
			switch (op)
			{
				case QueryFieldOperators.Equal:
					expressionString = string.Format(CultureInfo.InvariantCulture, "{0}==@{1}", fieldName, parameters.Count);
					AddParameter(parameters, expressionValue);
					break;

				case QueryFieldOperators.GreaterEqualThan:
					expressionString = string.Format(CultureInfo.InvariantCulture, "{0}>=@{1}", fieldName, parameters.Count);
					AddParameter(parameters, expressionValue);
					break;

				case QueryFieldOperators.GreaterThan:
					expressionString = string.Format(CultureInfo.InvariantCulture, "{0}>@{1}", fieldName, parameters.Count);
					AddParameter(parameters, expressionValue);
					break;

				case QueryFieldOperators.LessEqualThan:
					expressionString = string.Format(CultureInfo.InvariantCulture, "{0}<=@{1}", fieldName, parameters.Count);
					AddParameter(parameters, expressionValue);
					break;

				case QueryFieldOperators.LessThan:
					expressionString = string.Format(CultureInfo.InvariantCulture, "{0}<@{1}", fieldName, parameters.Count);
					AddParameter(parameters, expressionValue);
					break;

				case QueryFieldOperators.Like:
					expressionString = string.Format(CultureInfo.InvariantCulture, "{0}.Contains(@{1})", fieldName, parameters.Count);
					AddParameter(parameters, expressionValue.ToString());
					break;

				case QueryFieldOperators.StartsWith:
					expressionString = string.Format(CultureInfo.InvariantCulture, "{0}.StartsWith(@{1})", fieldName, parameters.Count);
					AddParameter(parameters, expressionValue.ToString());
					break;

				case QueryFieldOperators.In:
					startParameterIndex = parameters.Count;
					parameterCount = AddParameter(parameters, expressionValue);
					subExpressions = new List<string>();
					for (int parameterIndex = startParameterIndex; parameterIndex < startParameterIndex + parameterCount; parameterIndex++)
						subExpressions.Add(string.Format(CultureInfo.InvariantCulture, "{0}==@{1}", fieldName, parameterIndex));

					expressionString = string.Format(CultureInfo.InvariantCulture, "({0})", subExpressions.Concat(" OR "));
					break;

				case QueryFieldOperators.NotIn:
					startParameterIndex = parameters.Count;
					parameterCount = AddParameter(parameters, expressionValue);
					subExpressions = new List<string>();
					for (int parameterIndex = startParameterIndex; parameterIndex < startParameterIndex + parameterCount; parameterIndex++)
						subExpressions.Add(string.Format(CultureInfo.InvariantCulture, "{0}!=@{1}", fieldName, parameterIndex));

					expressionString = string.Format(CultureInfo.InvariantCulture, "({0})", subExpressions.Concat(" AND "));
					break;

				case QueryFieldOperators.Between:
					IEnumerable enumerableExpressionValues = expressionValue as IEnumerable;
					if (enumerableExpressionValues == null)
						throw new ArgumentException(Resources.DP_BetweenValueMustBeEnumerableWithTwoItems);

					IEnumerable<object> expressionValues = enumerableExpressionValues.Cast<object>();
					if (expressionValues.Count() != 2)
						throw new ArgumentException(Resources.DP_BetweenValueMustBeEnumerableWithTwoItems);

					expressionString = string.Format(CultureInfo.InvariantCulture, "({0}>=@{1} AND {0}<@{2})", fieldName, parameters.Count, parameters.Count + 1);
					AddParameter(parameters, expressionValue);
					break;
			}

			return expressionString;
		}

		private static int AddParameter(ArrayList parameters, object expressionValue)
		{
			if (expressionValue is IEnumerable && !(expressionValue is string))
			{
				int parameterCount = 0;
				IEnumerable<object> enumerableObjects = (expressionValue as IEnumerable).Cast<object>();
				foreach (object parameter in enumerableObjects)
				{
					parameters.Add(parameter);
					parameterCount++;
				}

				return parameterCount;
			}
			else
			{
				parameters.Add(expressionValue);
				return 1;
			}
		}
	}
}
