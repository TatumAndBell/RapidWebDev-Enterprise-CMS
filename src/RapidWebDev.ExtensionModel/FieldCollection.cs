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
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using RapidWebDev.Common;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
	/// Field collection
	/// </summary>
	[CollectionDataContract(Name = "FieldCollection", Namespace = ServiceNamespaces.ExtensionModel)]
	public class FieldCollection : Collection<FieldNameValuePair> { }

	/// <summary>
	/// Field name and value pair
	/// </summary>
	[DataContract(Name = "FieldNameValuePair", Namespace = ServiceNamespaces.ExtensionModel)]
	[KnownType(typeof(StringFieldValue))]
	[KnownType(typeof(IntegerFieldValue))]
	[KnownType(typeof(DecimalFieldValue))]
	[KnownType(typeof(DateTimeFieldValue))]
	[KnownType(typeof(HierarchyFieldValue))]
	[KnownType(typeof(EnumerationFieldValue))]
	public class FieldNameValuePair
	{

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldNameValuePair"/> class.
        /// </summary>
		public FieldNameValuePair() { }


        /// <summary>
        /// Initializes a new instance of the <see cref="FieldNameValuePair"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
		public FieldNameValuePair(string name, IFieldValue value)
		{
			this.Name = name;
			this.Value = value;
		}


        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
		[DataMember(Name = "FieldName")]
		public string Name { get; set; }


        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
		[DataMember(Name = "FieldValue")]
		public IFieldValue Value { get; set; }


        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
		public override string ToString()
		{
			return string.Format("[{0}, {1}]", this.Name, this.Value);
		}
	}
}

