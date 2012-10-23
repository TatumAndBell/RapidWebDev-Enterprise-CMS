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
using System.Globalization;
using System.IO;
using System.Linq;
using RapidWebDev.Common;
using RapidWebDev.FileManagement.Properties;

namespace RapidWebDev.FileManagement
{
	/// <summary>
	/// Api to store file physically.
	/// </summary>
	public class FileStorageApi : IFileStorageApi
	{
		private IApplicationContext applicationContext;

		/// <summary>
		/// Sets/gets file shared path.
		/// </summary>
		public string FileSharedPath { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="applicationContext"></param>
		public FileStorageApi(IApplicationContext applicationContext)
		{
			this.applicationContext = applicationContext;
		}

		/// <summary>
		/// Store the file stream for the category in an application and return size of the file in byte.
		/// </summary>
		/// <param name="applicationId"></param>
		/// <param name="category"></param>
		/// <param name="fileId"></param>
		/// <param name="fileExtensionName"></param>
		/// <param name="fileStream"></param>
		/// <returns>returns size of the file in byte.</returns>
		public long Store(Guid applicationId, string category, Guid fileId, string fileExtensionName, Stream fileStream)
		{
			this.CheckFileSharedPath();

			string fileStoragePath = this.ResolveFileStoragePath(applicationId, category, fileId, fileExtensionName);

			long totalBytesRead = 0;
			using (FileStream outputFileStream = new FileStream(fileStoragePath, FileMode.Create, FileAccess.Write))
			{
				byte[] buffer = new byte[16384];
				int bytesRead;
				do
				{
					bytesRead = fileStream.Read(buffer, 0, 16384);
					if (bytesRead > 0)
						outputFileStream.Write(buffer, 0, bytesRead);

					totalBytesRead += bytesRead;
				} 
				while (bytesRead != 0);
			}

			File.SetAttributes(fileStoragePath, FileAttributes.NotContentIndexed);
			return totalBytesRead;
		}

		/// <summary>
		/// Load file stream for the file head object.
		/// </summary>
		/// <param name="fileHeadObject"></param>
		/// <returns></returns>
		public Stream Load(FileHeadObject fileHeadObject)
		{
			string fileStoragePath = this.ResolveFileStoragePath(this.applicationContext.ApplicationId, fileHeadObject.Category, fileHeadObject.Id, fileHeadObject.FileExtensionName);
			if (!File.Exists(fileStoragePath))
				throw new FileNotFoundException();

			return new FileStream(fileStoragePath, FileMode.Open, FileAccess.Read, FileShare.Read, 16384);
		}

		private void CheckFileSharedPath()
		{
			if (string.IsNullOrEmpty(this.FileSharedPath))
				throw new ConfigurationErrorsException(Resources.InvalidFileSharedPath);

			if (!Directory.Exists(this.FileSharedPath))
				Directory.CreateDirectory(this.FileSharedPath);
		}

		private string ResolveFileStoragePath(Guid applicationId, string category, Guid fileId, string fileExtensionName)
		{
			string categoryFolderName = ResolveInvalidFolderName(category);
			string absoluteFileSharedPath = Kit.ToAbsolutePath(this.FileSharedPath);
			string directoryPath = Path.Combine(absoluteFileSharedPath, applicationId.ToString());
			directoryPath = Path.Combine(directoryPath, categoryFolderName);
			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);

			return Path.Combine(directoryPath, string.Format(CultureInfo.InvariantCulture, "{0}.{1}", fileId, fileExtensionName));
		}

		private static string ResolveInvalidFolderName(string category)
		{
			if (string.IsNullOrEmpty(category)) return "";

			string output = "";
			foreach (char ch in category)
				if (!Path.GetInvalidFileNameChars().Contains(ch))
					output += ch.ToString();

			return output;
		}
	}
}