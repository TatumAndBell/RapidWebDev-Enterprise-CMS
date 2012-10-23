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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using RapidWebDev.Common;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
	/// Hierarchy field's value
	/// </summary>
	[DataContract(Name = "HierarchyFieldValue", Namespace = ServiceNamespaces.ExtensionModel)]
	public class HierarchyFieldValue : IFieldValue
	{
		[IgnoreDataMember]
		object IFieldValue.Value
		{
			get { return this.Value; }
			set { this.Value = value as HierarchyNodeValueCollection; }
		}

		[IgnoreDataMember]
		FieldType IFieldValue.Type { get { return FieldType.Hierarchy; } }

        /// <summary>
        /// Gets or sets the value of property.
        /// </summary>
        /// <value>The value.</value>
		[DataMember(Name = "HierarchyNodeValueCollection")]
		public HierarchyNodeValueCollection Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyFieldValue"/> class.
        /// </summary>
		public HierarchyFieldValue()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyFieldValue"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
		public HierarchyFieldValue(HierarchyNodeValueCollection value)
		{
			this.Value = value;
		}

		/// <summary>
        /// Convert Hierarchy field 's value to Hierarchy data.
		/// </summary>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		public static implicit operator HierarchyNodeValueCollection(HierarchyFieldValue fieldValue)
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
			foreach (string hierarchyNodeValue in this.Value)
			{
				if (output.Length > 0) output.Append(", ");
				output.Append(hierarchyNodeValue);
			}

			return output.ToString();
		}
	}

	/// <summary>
    /// Hierarchy node's Value collection
	/// </summary>
	[CollectionDataContract(Name = "HierarchyNodeValueCollection", ItemName = "HierarchyNodeValue")]
	[Serializable]
	public class HierarchyNodeValueCollection : Collection<string>
	{
	}
}

