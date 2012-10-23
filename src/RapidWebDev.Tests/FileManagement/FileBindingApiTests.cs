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
	public class FileBindingApiTests
	{
		private static readonly string resourcesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../TestData/");
		private IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		private FileManagementApi fileManagementApi;
		private FileBindingApi fileBindingApi;

		[SetUp]
		public void DoSetup()
		{
			this.fileManagementApi = new FileManagementApi(authenticationContext, new MemoryFileStorageApi(), null);
			this.fileBindingApi = new FileBindingApi(authenticationContext, this.fileManagementApi);
		}

		[Test]
		[Description("Test cases for relationship management between files and external objects by FileBindingApi.")]
		public void CRUDFilesTest()
		{
			// add 2 files - an attachment and a thumbnail.
			FileUploadObject thumbnailFileObject1 = new FileUploadObject
			{
				Category = "Thumbnail",
				FileName = "206.jpg",
				Stream = LoadFileStream("206.jpg")
			};

			FileUploadObject thumbnailFileObject2 = new FileUploadObject
			{
				Category = "Thumbnail",
				FileName = "GenerateThumbnail4Png.png",
				Stream = LoadFileStream("GenerateThumbnail4Png.png")
			};

			FileUploadObject attachmentFileObject = new FileUploadObject
			{
				Category = "Attachment",
				FileName = "NHibernate.chm",
				Stream = LoadFileStream("NHibernate.chm")
			};

			// save the thumbnail and attachment
			this.fileManagementApi.Save(thumbnailFileObject1);
			this.fileManagementApi.Save(thumbnailFileObject2);
			this.fileManagementApi.Save(attachmentFileObject);

			// to simulate a product id
			Guid productId = Guid.NewGuid();
			this.fileBindingApi.Bind("Thumbnail", productId, thumbnailFileObject1.Id);
			this.fileBindingApi.Bind("Thumbnail", productId, thumbnailFileObject2.Id);
			this.fileBindingApi.Bind("Attachment", productId, attachmentFileObject.Id);

			// load all files associated with the product
			IEnumerable<FileHeadObject> fileHeadObjects = this.fileBindingApi.FindBoundFiles(productId);
			Assert.AreEqual(3, fileHeadObjects.Count());

			// load thumbnails associated with the product
			fileHeadObjects = this.fileBindingApi.FindBoundFiles(productId, "Thumbnail");
			Assert.AreEqual(2, fileHeadObjects.Count());

			// unbind attachments of the product but the attachments still exist.
			this.fileBindingApi.Unbind(productId, "Attachment");
			fileHeadObjects = this.fileBindingApi.FindBoundFiles(productId, "Attachment");
			Assert.AreEqual(0, fileHeadObjects.Count());
			Assert.IsNotNull(this.fileManagementApi.Load(attachmentFileObject.Id));

			// delete all thumbnails of the product
			this.fileBindingApi.DeleteBoundFiles(productId, "Thumbnail");
			fileHeadObjects = this.fileBindingApi.FindBoundFiles(productId, "Thumbnail");
			Assert.AreEqual(0, fileHeadObjects.Count());
			Assert.IsNull(this.fileManagementApi.Load(thumbnailFileObject1.Id));
			Assert.IsNull(this.fileManagementApi.Load(thumbnailFileObject2.Id));

			// finally delete the attachment to clear the temporary data.
			this.fileManagementApi.Delete(attachmentFileObject.Id);
		}

		private static Stream LoadFileStream(string fileName)
		{
			string filePath = Path.Combine(resourcesDirectoryPath, fileName);
			return new FileStream(filePath, FileMode.Open, FileAccess.Read); 
		}
	}
}
