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
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;

namespace RapidWebDev.FileManagement
{
    /// <summary>
    /// Api to manage relationship between files and external objects.
    /// </summary>
    public interface IFileBindingApi
    {
        /// <summary>
		/// Bind the file with id of the external object.
        /// </summary>
		/// <param name="relationshipType"></param>
        /// <param name="externalObjectId"></param>
        /// <param name="fileId"></param>
        void Bind(string relationshipType, Guid externalObjectId, Guid fileId);

        /// <summary>
		///Bind the files with id of the external object.
        /// </summary>
		/// <param name="relationshipType"></param>
        /// <param name="externalObjectId"></param>
        /// <param name="fileIds"></param>
		void Bind(string relationshipType, Guid externalObjectId, IEnumerable<Guid> fileIds);

        /// <summary>
		/// Unbind all files associated with id of the external object.
        /// </summary>
        /// <param name="externalObjectId"></param>
        void Unbind(Guid externalObjectId);

		/// <summary>
		/// Unbind the files associated with id of the external object in special relationship type.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <param name="relationshipType"></param>
		void Unbind(Guid externalObjectId, string relationshipType);

        /// <summary>
		/// Unbind the file with id of the external object.
        /// </summary>
        /// <param name="externalObjectId"></param>
        /// <param name="fileId"></param>
        void Unbind(Guid externalObjectId, Guid fileId);

        /// <summary>
		/// Unbind the files with id of the external object.
        /// </summary>
        /// <param name="externalObjectId"></param>
        /// <param name="fileIds"></param>
        void Unbind(Guid externalObjectId, IEnumerable<Guid> fileIds);

        /// <summary>
        /// Delete all files associated with id of the external object in any relationship types include both file entities and relationship.
        /// </summary>
        /// <param name="externalObjectId"></param>
        void DeleteBoundFiles(Guid externalObjectId);

		/// <summary>
		/// Delete the files associated with id of the external object in special relationship type include both file entities and relationship.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <param name="relationshipType"></param>
		void DeleteBoundFiles(Guid externalObjectId, string relationshipType);

        /// <summary>
        /// Get all files bound to id of the external object in any relationship types.
        /// </summary>
        /// <param name="externalObjectId"></param>
        /// <returns></returns>
        IEnumerable<FileHeadObject> FindBoundFiles(Guid externalObjectId);

		/// <summary>
		/// Get the files bound to id of the external object in special relationship type.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <param name="relationshipType"></param>
		/// <returns></returns>
		IEnumerable<FileHeadObject> FindBoundFiles(Guid externalObjectId, string relationshipType);

        /// <summary>
		/// Get files bound to ids of the external objects.
        /// </summary>
        /// <param name="externalObjectIds"></param>
        /// <returns></returns>
		IDictionary<Guid, IEnumerable<FileHeadObject>> FindBoundFiles(IEnumerable<Guid> externalObjectIds);
    }
}