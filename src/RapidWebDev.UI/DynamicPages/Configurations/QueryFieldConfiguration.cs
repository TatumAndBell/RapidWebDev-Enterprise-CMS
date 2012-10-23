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
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using System.Configuration;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Query field configuration.
	/// </summary>
	public class QueryFieldConfiguration : ICloneable
	{
		/// <summary>
		/// Field name and control value will be assemblied together as a database query criteria expression.
		/// </summary>
		public string FieldName { get; set; }

		/// <summary>
		/// Criteria operator between field name and control value. The default value is Auto.
		/// </summary>
		public QueryFieldOperators Operator { get; set; }

		/// <summary>
		/// If FieldValueType specified, the control value will be implicitly converted into the specified type before assembly it into database query criteria expression.
		/// </summary>
		public Type FieldValueType { get; set; }

		/// <summary>
		/// Rendering control configuration for the query field criteria.
		/// </summary>
		public BaseControlConfiguration Control { get; set; }

		/// <summary>
		/// The attribute decide how many cells the query control occupied in rendered panel. 
		/// The default value is -1 which means the occupation is decided by Operator.
		/// If Occupation is auto decided, it is 1 always except 2 for Between, whole line in the panel for In/NotIn.
		/// </summary>
		public int Occupation { get; set; }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public QueryFieldConfiguration()
		{
			this.Operator = QueryFieldOperators.Equal;
		}

		/// <summary>
		/// Construct QueryFieldConfiguration instance from xml element.
		/// </summary>
		/// <param name="queryFieldControlElement"></param>
		/// <param name="xmlParser"></param>
		public QueryFieldConfiguration(XmlElement queryFieldControlElement, XmlParser xmlParser)
		{
			this.FieldName = xmlParser.ParseString(queryFieldControlElement, "@FieldName");
			this.Operator = xmlParser.ParseEnum<QueryFieldOperators>(queryFieldControlElement, "@Operator");
			this.Occupation = xmlParser.ParseInt(queryFieldControlElement, "@Occupation", -1);
			string fieldValueTypeName = xmlParser.ParseString(queryFieldControlElement, "@FieldValueType");
			if (!Kit.IsEmpty(fieldValueTypeName))
			{
				try
				{
					this.FieldValueType = Kit.GetType(fieldValueTypeName);
				}
				catch (NotImplementedException exp)
				{
					throw new ConfigurationErrorsException(exp.Message, exp);
				}
			}

			this.Control = BaseControlConfiguration.Create(queryFieldControlElement, xmlParser);
			this.Occupation = QueryFieldControlFactory.CalculateQueryControlOccupation(this);
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns></returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Creates a copy of current instance but converts Operator to explicit value if it's Auto.
		/// </summary>
		/// <returns></returns>
		public QueryFieldConfiguration Clone()
		{
			return new QueryFieldConfiguration
			{
				FieldName = this.FieldName,
				Operator = this.GetOperator(),
				FieldValueType = this.FieldValueType,
				Control = this.Control,
				Occupation = this.Occupation
			};
		}

		/// <summary>
		/// Return default operator for specified control type if Control.ControlType is Auto.
		/// </summary>
		/// <returns></returns>
		private QueryFieldOperators GetOperator()
		{
			if (this.Operator != QueryFieldOperators.Auto)
				return this.Operator;

			switch (this.Control.ControlType)
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

			return QueryFieldOperators.Equal;
		}

		#endregion
	}
}

