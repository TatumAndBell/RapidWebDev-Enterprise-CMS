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
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using RapidWebDev.Common;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
	/// Integer field's value
	/// </summary>
	[DataContract(Name = "IntegerFieldValue", Namespace = ServiceNamespaces.ExtensionModel)]
	public class IntegerFieldValue : IFieldValue
	{
		[IgnoreDataMember]
		object IFieldValue.Value
		{
			get { return this.Value; }
			set { this.Value = (int)value; }
		}

		[IgnoreDataMember]
		FieldType IFieldValue.Type { get { return FieldType.Integer; } }

        /// <summary>
        /// Gets or sets the value of property.
        /// </summary>
        /// <value>The value.</value>
		[DataMember(Name = "Integer")]
		public int Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerFieldValue"/> class.
        /// </summary>
		public IntegerFieldValue()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerFieldValue"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
		public IntegerFieldValue(int value)
		{
			this.Value = value;
		}

		/// <summary>
        /// Convert Decimal field's value to Decimal data
		/// </summary>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		public static implicit operator int(IntegerFieldValue fieldValue)
		{
			return fieldValue.Value;
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}

