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
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel.Properties;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
    /// The factory of extension type object instance. When you create instance object, this structure can automatically set instance's related extension type's id (ExtensionDataTypeId).
	/// </summary>
	public static class ExtensionObjectFactory 
	{
		private static IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();

		/// <summary>
		/// Create extension type's instance. 
		/// The method intends to resolve extension type from metadata by the generic type name.
		/// </summary>
		/// <typeparam name="T">Implement IExtensionObject interface</typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidProgramException">Cannot find extension type which has same name with T</exception>
		public static T Create<T>() where T : IExtensionObject, new()
		{
			string className = typeof(T).Name;
			IObjectMetadata objectMetadata = metadataApi.GetType(className);
			Guid extensionDataTypeId = objectMetadata != null ? objectMetadata.Id : Guid.Empty;
			return new T { ExtensionDataTypeId = extensionDataTypeId };
		}

		/// <summary>
		/// Create extension type's instance. 
		/// The method intends to resolve extension type from metadata by the generic type name if the specified data type id is Empty.
		/// </summary>
		/// <param name="extensionDataTypeId">Extension data type id</param>
		/// <typeparam name="T">Implement IExtensionObject interface</typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidProgramException">Cannot find extension type which has same name with T</exception>
		public static T Create<T>(Guid extensionDataTypeId) where T : IExtensionObject, new()
		{
			if (extensionDataTypeId == Guid.Empty)
				return Create<T>();

			return new T { ExtensionDataTypeId = extensionDataTypeId };
		}

		/// <summary>
        /// Create extension type's instance. 
		/// The method intends to resolve extension type from metadata by the generic type name if the property "ExtensionDataTypeId" of specified extension biz object is Empty.
		/// </summary>
		/// <param name="extensionBizObject"></param>
        /// <typeparam name="T">Implement IExtensionObject interface</typeparam>
		/// <returns></returns>
        /// <exception cref="InvalidProgramException">Cannot find extension type which has same name with T</exception>
		public static T Create<T>(IExtensionBizObject extensionBizObject) where T : IExtensionObject, new()
		{
			T instance = Create<T>(extensionBizObject.ExtensionDataTypeId);
			IEnumerator<KeyValuePair<string, object>> properties = extensionBizObject.GetFieldEnumerator();
			while(properties.MoveNext())
				instance[properties.Current.Key] = properties.Current.Value;

			return instance;
		}
	}
}
