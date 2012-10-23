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
	/// Decimal field's value
	/// </summary>
	[DataContract(Name = "DecimalFieldValue", Namespace = ServiceNamespaces.ExtensionModel)]
	public class DecimalFieldValue : IFieldValue
	{
		[IgnoreDataMember]
		object IFieldValue.Value
		{
			get { return this.Value; }
			set { this.Value = (decimal)value; }
		}

		[IgnoreDataMember]
		FieldType IFieldValue.Type { get { return FieldType.Decimal; } }

		/// <summary>
		/// Get Decimal field's value
		/// </summary>
		[DataMember(Name = "Decimal")]
		public decimal Value { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalFieldValue"/> class.
        /// </summary>
		public DecimalFieldValue()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalFieldValue"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
		public DecimalFieldValue(decimal value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Convert Decimal field's value to Decimal data
		/// </summary>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		public static implicit operator decimal(DecimalFieldValue fieldValue)
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

