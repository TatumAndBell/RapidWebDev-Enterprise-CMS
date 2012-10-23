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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using RapidWebDev.Common;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
	/// Enumeration field's value
	/// </summary>
	[DataContract(Name = "EnumerationFieldValue", Namespace = ServiceNamespaces.ExtensionModel)]
	public class EnumerationFieldValue : IFieldValue
	{
		[IgnoreDataMember]
		object IFieldValue.Value
		{
			get { return this.Value; }
			set { this.Value = value as EnumerationValueCollection; }
		}

		[IgnoreDataMember]
		FieldType IFieldValue.Type { get { return FieldType.Enumeration; } }

        /// <summary>
        /// Gets or sets the value of property.
        /// </summary>
        /// <value>The value.</value>
		[DataMember(Name = "SelectionItemValues")]
		public EnumerationValueCollection Value { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
		public EnumerationFieldValue()
		{
		}

        /// <summary>
		/// Constructor.
        /// </summary>
        /// <param name="value">The value.</param>
		public EnumerationFieldValue(EnumerationValueCollection value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Convert Selection field value to Selection data
		/// </summary>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		public static implicit operator EnumerationValueCollection(EnumerationFieldValue fieldValue)
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
			if (this.Value == null) return null;
			StringBuilder output = new StringBuilder();
			foreach (string selectionItem in this.Value)
			{
				if (output.Length > 0) output.Append(", ");
				output.Append(selectionItem);
			}

			return output.ToString();
		}
	}

	/// <summary>
    /// Enumeration values collection
	/// </summary>
	[CollectionDataContract(Name = "EnumerationValueCollection", ItemName = "EnumerationValue")]
	[Serializable]
	public class EnumerationValueCollection : Collection<string>
	{
	}
}

