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
using System.Drawing;
using System.Linq;
using RapidWebDev.Common;

namespace RapidWebDev.FileManagement
{
	/// <summary>
	/// Api to manage thumbnails of files.
	/// </summary>
	public interface IThumbnailApi
	{
		/// <summary>
		/// Get relative thumbnail url (starts from ~/) for the file on the web server.
		/// </summary>
		/// <param name="fileId"></param>
		/// <param name="thumbnailSize"></param>
		/// <returns>return path of thumbnail file on the server.</returns>
		string GetThumbnailUrl(Guid fileId, Size thumbnailSize);

		/// <summary>
		/// Delete all thumbnails of the file. 
		/// </summary>
		/// <param name="fileId"></param>
		void DeleteThumbnails(Guid fileId);
	}
}