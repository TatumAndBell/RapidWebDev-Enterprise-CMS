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
using System.IO;
using System.Linq;
using NUnit.Framework;
using RapidWebDev.Common;
using RapidWebDev.Platform;
using RapidWebDev.FileManagement.Linq;
using RapidWebDev.FileManagement;

namespace RapidWebDev.Tests.FileManagement
{
	[TestFixture]
	public class FileManagementApiTests
	{
		private static readonly string resourcesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../TestData/");
		private IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		private FileManagementApi fileManagementApi;

		[SetUp]
		public void DoSetup()
		{
			this.fileManagementApi = new FileManagementApi(authenticationContext, new MemoryFileStorageApi(), null);
		}

		[Test]
		[Description("Basic CRUD files tests.")]
		public void CRUDFilesTest()
		{
			// add 2 files - an attachment and a thumbnail.
			Stream thumbnailFileStream = LoadFileStream("206.jpg");
			long thumbnailFileSize = thumbnailFileStream.Length;
			FileUploadObject thumbnailFileObject = new FileUploadObject
			{
				Category = "Thumbnail",
				FileName = "206.jpg",
				Stream = thumbnailFileStream
			};

			Stream attachmentFileStream = LoadFileStream("NHibernate.chm");
			long attachmentFileSize = attachmentFileStream.Length;
			FileUploadObject attachmentFileObject = new FileUploadObject
			{
				Category = "Attachment",
				FileName = "NHibernate.chm",
				Stream = LoadFileStream("NHibernate.chm")
			};

			// assert on thumbnail
			FileHeadObject thumbnailFileHeader = this.fileManagementApi.Save(thumbnailFileObject);
			Assert.AreEqual(thumbnailFileObject.Id, thumbnailFileHeader.Id);
			Assert.AreEqual("206.jpg", thumbnailFileHeader.FileName);
			Assert.AreEqual("jpg", thumbnailFileHeader.FileExtensionName);
			Assert.AreEqual(thumbnailFileSize, thumbnailFileHeader.BytesCount);
			Assert.AreEqual("Thumbnail", thumbnailFileHeader.Category);
			Assert.AreEqual(1, thumbnailFileHeader.Version);

			// save the attachment
			this.fileManagementApi.Save(attachmentFileObject);

			// delete then load the thunbmail.
			this.fileManagementApi.Delete(thumbnailFileObject.Id);
			Assert.IsNull(this.fileManagementApi.Load(thumbnailFileObject.Id));

			// load the attachment then assert.
			FileHeadObject attachmentFileHeader = this.fileManagementApi.Load(attachmentFileObject.Id);
			Assert.AreEqual(attachmentFileObject.Id, attachmentFileHeader.Id);
			Assert.AreEqual("NHibernate.chm", attachmentFileHeader.FileName);
			Assert.AreEqual("chm", attachmentFileHeader.FileExtensionName);
			Assert.AreEqual(attachmentFileSize, attachmentFileHeader.BytesCount);
			Assert.AreEqual("Attachment", attachmentFileHeader.Category);

			// bulk get files. (the thumbnail is deleted.)
			IEnumerable<FileHeadObject> files = this.fileManagementApi.BulkGet(new[] { thumbnailFileObject.Id, attachmentFileObject.Id });
			Assert.AreEqual(1, files.Count());

			// delete the attachment
			this.fileManagementApi.Delete(attachmentFileObject.Id);
			files = this.fileManagementApi.BulkGet(new[] { thumbnailFileObject.Id, attachmentFileObject.Id });
			Assert.AreEqual(0, files.Count());
		}

		private static Stream LoadFileStream(string fileName)
		{
			string filePath = Path.Combine(resourcesDirectoryPath, fileName);
			return new FileStream(filePath, FileMode.Open, FileAccess.Read); 
		}
	}
}
