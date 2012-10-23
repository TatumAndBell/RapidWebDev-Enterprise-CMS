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
using System.IO;

namespace RapidWebDev.FileManagement
{
	/// <summary>
	/// Api to store file physically.
	/// </summary>
	public interface IFileStorageApi
	{
		/// <summary>
		/// Store the file stream for the category in an application and return size of the file in byte.
		/// </summary>
		/// <param name="applicationId"></param>
		/// <param name="category"></param>
		/// <param name="fileId"></param>
		/// <param name="fileExtensionName"></param>
		/// <param name="fileStream"></param>
		/// <returns>returns size of the file in byte.</returns>
		long Store(Guid applicationId, string category, Guid fileId, string fileExtensionName, Stream fileStream);

		/// <summary>
		/// Load file stream for the file head object.
		/// </summary>
		/// <param name="fileHeadObject"></param>
		/// <returns></returns>
		Stream Load(FileHeadObject fileHeadObject);
	}
}