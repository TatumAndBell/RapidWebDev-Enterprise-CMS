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
	/// String type field's value
	/// </summary>
	[DataContract(Name = "StringFieldValue", Namespace = ServiceNamespaces.ExtensionModel)]
	public class StringFieldValue : IFieldValue
	{
		[IgnoreDataMember]
		object IFieldValue.Value
		{
			get { return this.Value; }
			set { this.Value = value as string; }
		}

		[IgnoreDataMember]
		FieldType IFieldValue.Type { get { return FieldType.String; } }

        /// <summary>
        /// Gets or sets the value of property.
        /// </summary>
        /// <value>The value.</value>
		[DataMember(Name = "String")]
		public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringFieldValue"/> class.
        /// </summary>
		public StringFieldValue()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="StringFieldValue"/> class.
        /// </summary>
        /// <param name="s">The s.</param>
		public StringFieldValue(string s)
		{
			this.Value = s;
		}

		/// <summary>
        /// Convert string field value to string
		/// </summary>
		/// <param name="stringFieldValue"></param>
		/// <returns></returns>
		public static implicit operator string(StringFieldValue stringFieldValue)
		{
			if (stringFieldValue == null) return null;
			return stringFieldValue.Value;
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
		public override string ToString()
		{
			return this.Value;
		}
	}
}
