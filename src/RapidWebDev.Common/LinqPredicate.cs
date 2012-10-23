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

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using RapidWebDev.Common;

namespace System.Linq
{
	/// <summary>
	/// The class is used to merge string typed linq predicate with parameters.
	/// </summary>
	public sealed class LinqPredicate
	{
		private readonly static Regex regexForPredicateParameters = new Regex(@"\@\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private List<KeyValuePair<string, object[]>> expressionsWithParameters = new List<KeyValuePair<string, object[]>>();
		private string cachedMergedExpression;
		private object[] cachedMergedParameters;

		/// <summary>
		/// Constructor merger from an entry predicate.
		/// </summary>
		/// <param name="expression">linq predicate expression</param>
		/// <param name="parameters">linq predicate parameters</param>
		public LinqPredicate(string expression, params object[] parameters)
		{
			if (!string.IsNullOrEmpty(expression))
			{
				ValidatePredicateWithParameters(expression, parameters);
				this.expressionsWithParameters.Add(new KeyValuePair<string, object[]>(expression, parameters));
			}
		}

		/// <summary>
		/// New predicate expression after merged.
		/// </summary>
		public string Expression
		{
			get
			{
				this.DoMerge();
				return this.cachedMergedExpression;
			}
		}

		/// <summary>
		/// New predicate parameter array after merged.
		/// </summary>
		public object[] Parameters
		{
			get
			{
				this.DoMerge();
				return this.cachedMergedParameters;
			}
		}

		/// <summary>
		/// Merge other predicate expression with parameters into the current predicate.
		/// </summary>
		/// <param name="expression">linq predicate expression</param>
		/// <param name="parameters">linq predicate parameters</param>
		public LinqPredicate Add(string expression, params object[] parameters)
		{
			if (!string.IsNullOrEmpty(expression))
			{
				ValidatePredicateWithParameters(expression, parameters);

				this.cachedMergedExpression = null;
				this.cachedMergedParameters = null;
				this.expressionsWithParameters.Add(new KeyValuePair<string, object[]>(expression, parameters));
			}

			return this;
		}

		/// <summary>
		/// Merge other predicate expression with parameters into the current predicate.
		/// </summary>
		/// <param name="linqPredicate">linq predicate</param>
		public LinqPredicate Add(LinqPredicate linqPredicate)
		{
			if (linqPredicate != null && !string.IsNullOrEmpty(linqPredicate.Expression))
			{
				ValidatePredicateWithParameters(linqPredicate.Expression, linqPredicate.Parameters);
				this.cachedMergedExpression = null;
				this.cachedMergedParameters = null;
				this.expressionsWithParameters.Add(new KeyValuePair<string, object[]>(linqPredicate.Expression, linqPredicate.Parameters));
			}

			return this;
		}
        
        /// <summary>
        /// Concat two linq predicates.
        /// </summary>
        /// <param name="predicate1"></param>
        /// <param name="predicate2"></param>
        /// <returns></returns>
        public static LinqPredicate Concat(LinqPredicate predicate1, LinqPredicate predicate2)
        {
            if (predicate1 != null && predicate2 != null)
				return predicate1.Add(predicate2);
            else if (predicate1 == null && predicate2 != null)
                return predicate2;
            else if (predicate1 != null && predicate2 == null)
                return predicate1;
            else
                return null;
        }

		private void DoMerge()
		{
			if (!Kit.IsEmpty(this.cachedMergedExpression)) return;

			const string temporaryVariablePrefix = "@Var#";
			StringBuilder output = new StringBuilder();
			ArrayList outputParameters = new ArrayList();
			Dictionary<object, string> variableByNameDictionary = new Dictionary<object, string>();
			foreach (KeyValuePair<string, object[]> predicateWithParameters in this.expressionsWithParameters)
			{
				string predicate = predicateWithParameters.Key;
				object[] parameters = predicateWithParameters.Value;
				if (parameters != null && parameters.Length > 0)
				{
					MatchCollection matchCollection = regexForPredicateParameters.Matches(predicateWithParameters.Key);
					foreach (Match match in matchCollection)
					{
						int parameterIndex = int.Parse(match.Value.Substring(1), CultureInfo.InvariantCulture);
						object parameterValue = parameters[parameterIndex];

						if (variableByNameDictionary.ContainsKey(parameterValue))
						{
							string existVariableName = variableByNameDictionary[parameterValue];
							predicate = predicate.Replace(match.Value, existVariableName);
						}
						else
						{
							string newVariableName = temporaryVariablePrefix + outputParameters.Count;
							predicate = predicate.Replace(match.Value, newVariableName);
							outputParameters.Add(parameterValue);
							variableByNameDictionary.Add(parameterValue, newVariableName);
						}
					}
				}

				predicate = predicate.Replace(temporaryVariablePrefix, "@");
				if (output.Length > 0) output.Append(" AND ");
				output.AppendFormat("({0})", predicate);
			}

			this.cachedMergedExpression = output.ToString();
			this.cachedMergedParameters = outputParameters.ToArray();
		}

		private static void ValidatePredicateWithParameters(string predicate, object[] parameters)
		{
			MatchCollection matchCollection = regexForPredicateParameters.Matches(predicate);
			if (matchCollection.Count > 0)
			{
				foreach (Match match in matchCollection)
				{
					int parameterIndex = 0;
					if (int.TryParse(match.Value.Substring(1), out parameterIndex))
					{
						if (parameterIndex >= parameters.Length)
							throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The variable \"@{0}\" included in the predicate expression is out of index to parameters.", parameterIndex));
					}
				}
			}
			else
			{
				if (parameters != null && parameters.Length > 0)
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The parameters are specified but no variable included in the predicate expression \"{0}\".", predicate));
			}
		}
	}
}
