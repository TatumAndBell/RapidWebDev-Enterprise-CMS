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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using RapidWebDev.Common;

namespace RapidWebDev.ExtensionModel
{

    /// <summary>
    /// Extendible Object
    /// </summary>
    public abstract class AbstractExtensionObject : IExtensionObject
    {
        private object syncObject = new object();
        private bool hasChanged;
        private IDictionary<string, object> extensionProperties;

        /// <summary>
        /// Gets or sets the extension data type id.
        /// </summary>
        /// <value>The extension data type id.</value>
        public abstract Guid ExtensionDataTypeId { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        /// <value>The extension data.</value>
        public abstract string ExtensionData { get; set; }

        /// <summary>
        /// Gets a value indicating whether this extension properties has changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has changed; otherwise, <c>false</c>.
        /// </value>
        bool IExtensionObject.HasChanged
        {
            get { return this.hasChanged; }
        }

        /// <summary>
        /// Gets or sets the property's value with the specified name. if property is not exist, return null.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get
            {
                this.ResolveExtensionProperties();

                if (!this.extensionProperties.ContainsKey(name)) return null;

                return this.extensionProperties[name];
            }
            set
            {
                this.ResolveExtensionProperties();
                this.hasChanged = true;
                this.extensionProperties[name] = value;
            }
        }

        /// <summary>
        /// Gets the dynamic field enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetFieldEnumerator()
        {
            this.ResolveExtensionProperties();
            return this.extensionProperties.GetEnumerator();
        }

		/// <summary>
		/// Clear all existed extension fields.
		/// </summary>
		public void RemoveAllExtensionFields()
		{
			this.ResolveExtensionProperties();
			if (this.extensionProperties != null)
				this.extensionProperties.Clear();
		}

        /// <summary>
        /// Get property's value, if it is null or this property is not exist, return default(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public T FieldValue<T>(string name) where T : IFieldValue
        {
            object fieldValue = this[name];
            if (fieldValue == null) return default(T);

            try
            {
                return (T)fieldValue;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Parses the extension properties  to current Object.
        /// </summary>
        /// <param name="extensionBizObject">The extension biz object.</param>
        public void ParseExtensionPropertiesFrom(IExtensionBizObject extensionBizObject)
        {
            IEnumerator<KeyValuePair<string, object>> properties = extensionBizObject.GetFieldEnumerator();
            while (properties.MoveNext())
                this[properties.Current.Key] = properties.Current.Value;
        }

        private void ResolveExtensionProperties()
        {
            if (this.extensionProperties == null)
            {
                lock (this.syncObject)
                {
                    if (this.extensionProperties == null)
                    {
                        IExtensionObjectSerializer serializer = SpringContext.Current.GetObject<IExtensionObjectSerializer>();
                        this.extensionProperties = serializer.Deserialize(this);
                    }
                }
            }
        }
    }
}

