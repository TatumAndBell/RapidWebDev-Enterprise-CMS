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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using RapidWebDev.Common;

namespace RapidWebDev.FileManagement.Web
{
	/// <summary>
	/// File object which is to be serialized to JSON for file management control.
	/// </summary>
	[Serializable]
	public class FileWebObject
	{
		/// <summary>
		/// Empty constructor used for JavaScriptSerializer only.
		/// </summary>
		public FileWebObject()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fileId"></param>
		/// <param name="fileName"></param>
		/// <param name="byteCount"></param>
		public FileWebObject(Guid fileId, string fileName, long byteCount)
		{
			this.Id = fileId;
			this.FileName = fileName;
			this.Size = byteCount / 1024;
			this.DownloadUri = this.GetDownloadUri();
			this.IconUri = this.GetIconUri();
		}

		/// <summary>
		/// File name.
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// File size.
		/// </summary>
		public long Size { get; set; }

		/// <summary>
		/// File id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// File download Uri
		/// </summary>
		public string DownloadUri { get; set; }

		/// <summary>
		/// File icon Uri.
		/// </summary>
		public string IconUri { get; set; }

		/// <summary>
		/// Get hashcode of the object
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (this.Id != Guid.Empty)
				return this.Id.GetHashCode();

			return this.GetHashCode();
		}

		/// <summary>
		/// Whether the current object equals to the argument.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			FileWebObject target = obj as FileWebObject;
			if (target == null) return false;

			return this.GetHashCode() == target.GetHashCode();
		}

		private string GetDownloadUri()
		{
			const string downloadUriTemplate = "~/FileDownloadService.svc/fileId/{0}";
			string downloadUri = string.Format(CultureInfo.InvariantCulture, downloadUriTemplate, this.Id);
			downloadUri = Kit.ResolveAbsoluteUrl(downloadUri);
			return downloadUri;
		}

		private string GetIconUri()
		{
			const string downloadUriTemplate = "~/FileIconDownloadService.svc?ext={0}";
			string fileExtensionName = Path.GetExtension(this.FileName);
			if (!string.IsNullOrEmpty(fileExtensionName))
				fileExtensionName = fileExtensionName.Substring(1);

			string downloadUri = string.Format(CultureInfo.InvariantCulture, downloadUriTemplate, fileExtensionName);
			downloadUri = Kit.ResolveAbsoluteUrl(downloadUri);
			return downloadUri;
		}
	}
}
