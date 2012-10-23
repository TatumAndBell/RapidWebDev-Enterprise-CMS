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

namespace RapidWebDev.Common.CodeDom
{
	/// <summary>
	/// Config class indicates how to generate new property from original property in dynamic created decorate class.
	/// </summary>
	public class PropertyDefinition
	{
		private string newPropertyName;

		/// <summary>
		/// Construct property definition by specified the type of new property.
		/// </summary>
		/// <param name="newPropertyType"></param>
		public PropertyDefinition(Type newPropertyType)
		{
			this.NewPropertyType = newPropertyType;
		}

		/// <summary>
		/// Construct property definition by specified the original property name.
		/// The new property type is same to original.
		/// </summary>
		/// <param name="originalPropertyName"></param>
		public PropertyDefinition(string originalPropertyName)
		{
			this.PropertyName = originalPropertyName;
		}

		/// <summary>
		/// Property name.
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// New property name.
		/// </summary>
		public string NewPropertyName 
		{
			get 
			{
				if (Kit.IsEmpty(this.newPropertyName)) 
					return this.PropertyName;

				return this.newPropertyName;
			}
			set
			{
				if (Kit.IsEmpty(value)) return;
				if (this.PropertyName == value) return;
				this.newPropertyName = value;
			}
		}

		/// <summary>
		/// New property type.
		/// </summary>
		public Type NewPropertyType { get; private set; }

		/// <summary>
		/// Callback to convert property value from original type to new type. 
		/// The platform uses Kit.ConvertType to convert property value by default if it's not specified.
		/// </summary>
		public Func<object, object> PropertyValueConvertCallback { get; set; }
	}
}

