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
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
    /// Extension type's business abstract class
	/// </summary>
	[DataContract(Namespace = ServiceNamespaces.Platform)]
	[Serializable]
	[KnownType(typeof(DateTimeFieldValue))]
	[KnownType(typeof(DecimalFieldValue))]
	[KnownType(typeof(IntegerFieldValue))]
	[KnownType(typeof(StringFieldValue))]
	[KnownType(typeof(EnumerationValueCollection))]

	[KnownType(typeof(DateTime))]
	[KnownType(typeof(decimal))]
	[KnownType(typeof(int))]
	[KnownType(typeof(DateTime?))]
	[KnownType(typeof(decimal?))]
	[KnownType(typeof(int?))]
	[KnownType(typeof(string))]
	[KnownType(typeof(EnumerationValueCollection))]
	public abstract class AbstractExtensionBizObject : IExtensionBizObject
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractExtensionBizObject"/> class.
        /// </summary>
		public AbstractExtensionBizObject()
		{
			this.Properties = new Dictionary<string, object>();
		}

		/// <summary>
		/// Gets / sets extension data type id.
		/// </summary>
		[DataMember]
		public Guid ExtensionDataTypeId { get; set; }
		
		/// <summary>
		/// Extension fields
		/// </summary>
		[DataMember]
		public Dictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified property name.
        /// </summary>
        /// <value></value>
		public object this[string propertyName] 
		{
			get 
			{
				if (!this.Properties.ContainsKey(propertyName)) return null;
				return this.Properties[propertyName];
			}
			set { this.Properties[propertyName] = value; }
		}

        /// <summary>
        /// Gets the field enumerator.
        /// </summary>
        /// <returns></returns>
		public IEnumerator<KeyValuePair<string, object>> GetFieldEnumerator()
		{
            if (this.Properties == null) 
				return new List<KeyValuePair<string, object>>().GetEnumerator();

			return this.Properties.GetEnumerator();
		}

		/// <summary>
        /// Parse extension properties of extensionObject into current extension object.
		/// </summary>
		/// <param name="extensionObject"></param>
		public void ParseExtensionPropertiesFrom(IExtensionObject extensionObject)
		{
			this.ExtensionDataTypeId = extensionObject.ExtensionDataTypeId;
			IEnumerator<KeyValuePair<string, object>> properties = extensionObject.GetFieldEnumerator();
			while (properties.MoveNext())
				this[properties.Current.Key] = properties.Current.Value;
		}

		/// <summary>
		/// Clone the properties of this instance to the specified biz object.
		/// </summary>
		/// <param name="bizObject"></param>
		protected void ClonePropertiesTo(AbstractExtensionBizObject bizObject)
		{
			if (this.Properties != null)
			{
				bizObject.Properties = new Dictionary<string, object>();
				foreach (string propertyName in this.Properties.Keys)
					bizObject.Properties[propertyName] = this.Properties[propertyName];
			}
		}
	}
}
