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
using System.Text;

namespace RapidWebDev.ExtensionModel
{
    /// <summary>
    /// Extension model metadata's Api Interface.
    /// </summary>
	public interface IMetadataApi
	{
		/// <summary>
		/// Create current application's extension type metadata.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="category"></param>
		/// <param name="description"></param>
		/// <param name="objectMetadataType"></param>
		/// <param name="isGlobal"></param>
		/// <param name="parentObjectMetadataId"></param>
		/// <returns>created extension type id</returns>
		Guid AddType(string name, string category, string description, ObjectMetadataTypes objectMetadataType, bool isGlobal, Guid? parentObjectMetadataId);


        /// <summary>
        /// Create current application's extension type metadata.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <param name="description">The description.</param>
        /// <param name="objectMetadataType">Type of the object metadata.</param>
        /// <param name="isGlobal">if set to <c>true</c> [is global].</param>
        /// <param name="parentObjectMetadataId">The parent object metadata id.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
		Guid AddType(string name, string category, string description, ObjectMetadataTypes objectMetadataType, bool isGlobal, Guid? parentObjectMetadataId, int version);

		/// <summary>
		/// Update extension type metadata's description.
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <param name="name"></param>
		/// <param name="category"></param>
		/// <param name="description"></param>
		/// <param name="parentObjectMetadataId"></param>
		void UpdateType(Guid objectMetadataId, string name, string category, string description, Guid? parentObjectMetadataId);

        /// <summary>
        /// Delete extension type metadata
        /// </summary>
        /// <param name="objectMetadataId"></param>
		/// <exception cref="RapidWebDev.Common.Validation.ValidationException">Derived type cannot be deleted</exception>
		void DeleteType(Guid objectMetadataId);

        /// <summary>
        /// Get extension type metadata
        /// </summary>
        /// <param name="objectMetadataId"></param>
        /// <returns></returns>
        IObjectMetadata GetType(Guid objectMetadataId);

		/// <summary>
        /// Get extension type metadata,first get from current domain, then get from global, if both cannot find, return null.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		IObjectMetadata GetType(string name);

        /// <summary>
        /// Saves the extension type metadata
        /// </summary>
        /// <param name="objectMetadataId">The object metadata id.</param>
        /// <param name="fieldMetadata">The field metadata.</param>
		void SaveField(Guid objectMetadataId, IFieldMetadata fieldMetadata);

        /// <summary>
        /// delete specified attribute's metadata
        /// </summary>
        /// <param name="objectMetadataId"></param>
        /// <param name="fieldName"></param>
		void DeleteField(Guid objectMetadataId, string fieldName);

        /// <summary>
        /// Get extension type's specified attribute's metadata.
        /// </summary>
        /// <param name="objectMetadataId"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
		IFieldMetadata GetField(Guid objectMetadataId, string fieldName);

		/// <summary>
		/// Get extension type's specified attribute's metadata.
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <param name="fieldId"></param>
		/// <returns></returns>
		IFieldMetadata GetField(Guid objectMetadataId, Guid fieldId);

        /// <summary>
        /// Get all attribute of extension type, ordered by Ordinal property.
        /// </summary>
        /// <param name="objectMetadataId"></param>
        /// <returns></returns>
        IEnumerable<IFieldMetadata> GetFields(Guid objectMetadataId);
	}
}

