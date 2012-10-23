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
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RapidWebDev.FileManagement
{
	/// <summary>
    /// Api to manage file information.
	/// </summary>
	public interface IFileManagementApi
	{
        /// <summary>
        /// Save a file.
        /// </summary>
		/// <param name="fileUploadObject">The file uploading object which should include a readable file stream.</param>
		/// <returns>returns file head of the saved file.</returns>
		FileHeadObject Save(FileUploadObject fileUploadObject);

        /// <summary>
        /// Delete the file by id.
        /// </summary>
        /// <param name="fileId">The file id.</param>
		void Delete(Guid fileId);

        /// <summary>
		/// Delete the files by ids.
        /// </summary>
        /// <param name="fileIds">The file ids.</param>
		void Delete(IEnumerable<Guid> fileIds);

        /// <summary>
        /// Load the file by id.
        /// </summary>
        /// <param name="fileId">The file id.</param>
        /// <returns></returns>
		FileHeadObject Load(Guid fileId);

        /// <summary>
		/// Bulk get the files by ids.
        /// </summary>
        /// <param name="fileIds">The file ids.</param>
        /// <returns></returns>
		IEnumerable<FileHeadObject> BulkGet(IEnumerable<Guid> fileIds);
	}
}