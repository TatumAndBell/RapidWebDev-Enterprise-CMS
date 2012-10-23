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
	/// DateTime field' value
	/// </summary>
	[DataContract(Name = "DateTimeFieldValue", Namespace = ServiceNamespaces.ExtensionModel)]
	public class DateTimeFieldValue : IFieldValue
	{
		[IgnoreDataMember]
		object IFieldValue.Value
		{
			get { return this.Value; }
			set { this.Value = (DateTime)value; }
		}

		[IgnoreDataMember]
		FieldType IFieldValue.Type { get { return FieldType.DateTime; } }

		/// <summary>
		/// Get current DateTime field's value
		/// </summary>
		[DataMember(Name = "DateTime")]
		public DateTime Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeFieldValue"/> class.
        /// </summary>
		public DateTimeFieldValue()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeFieldValue"/> class.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
		public DateTimeFieldValue(DateTime dateTime)
		{
			this.Value = dateTime;
		}

		/// <summary>
		///  Convert DateTime field's value to DateTime data.
		/// </summary>
		/// <param name="dateTimeFieldValue"></param>
		/// <returns></returns>
		public static implicit operator DateTime(DateTimeFieldValue dateTimeFieldValue)
		{
			return dateTimeFieldValue.Value;
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

