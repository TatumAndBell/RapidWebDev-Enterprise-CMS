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
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using RapidWebDev.Common;

namespace RapidWebDev.Common
{
	/// <summary>
	/// Web service query predicate which is used to execute query from web service client.
	/// </summary>
	[Serializable]
	[DataContract(Namespace = ServiceNamespaces.Common)]
	public class WebServiceQueryPredicate
	{
		/// <summary>
		/// Construct predicate without parameters
		/// </summary>
		/// <param name="expression">Web service query predicate expression</param>
		public WebServiceQueryPredicate(string expression)
		{
			this.Expression = expression;
		}

		/// <summary>
		/// Construct predicate without parameters
		/// </summary>
		/// <param name="expression">Web service query predicate expression</param>
		/// <param name="parameters">Web service query predicate expression parameters</param>
		public WebServiceQueryPredicate(string expression, Collection<WebServiceQueryPredicateParameter> parameters)
		{
			this.Expression = expression;
			this.Parameters = parameters;
		}

		/// <summary>
		/// Query predicate expression.
		/// </summary>
		[DataMember]
		public string Expression { get; set; }

		/// <summary>
		/// New predicate parameter array after merged.
		/// </summary>
		[DataMember]
		public Collection<WebServiceQueryPredicateParameter> Parameters { get; set; }
	}

	/// <summary>
	/// Web service query predicate parameter.
	/// </summary>
	[Serializable]
	[DataContract(Namespace = ServiceNamespaces.Common)]
	public class WebServiceQueryPredicateParameter
	{
		/// <summary>
		/// Parameter name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Parameter type
		/// </summary>
		[DataMember]
		public WebServiceQueryPredicateParameterTypes Type { get; set; }

		/// <summary>
		/// Parameter value
		/// </summary>
		[DataMember]
		public string Value { get; set; }

		/// <summary>
		/// Construct web service query predicate parameter.
		/// </summary>
		/// <param name="name"></param>
		public WebServiceQueryPredicateParameter(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Construct web service query predicate parameter.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parameterType"></param>
		public WebServiceQueryPredicateParameter(string name, WebServiceQueryPredicateParameterTypes parameterType)
		{
			this.Name = name;
			this.Type = parameterType;
		}
	}

	/// <summary>
	/// Web service query predicate parameter types
	/// </summary>
	[DataContract(Namespace = ServiceNamespaces.Common)]
	public enum WebServiceQueryPredicateParameterTypes
	{
		/// <summary>
		/// String
		/// </summary>
		[EnumMember]
		String = 0,

		/// <summary>
		/// DateTime
		/// </summary>
		[EnumMember]
		DateTime = 1,

		/// <summary>
		/// Guid
		/// </summary>
		[EnumMember]
		Guid = 2,

		/// <summary>
		/// Integer
		/// </summary>
		[EnumMember]
		Integer = 3,

		/// <summary>
		/// Decimal
		/// </summary>
		[EnumMember]
		Decimal = 4,

		/// <summary>
		/// Double
		/// </summary>
		[EnumMember]
		Double = 5
	}
}
