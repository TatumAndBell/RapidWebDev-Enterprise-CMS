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
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.FileManagement.Properties;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.FileManagement.Linq
{
	/// <summary>
	/// File Management service
	/// </summary>
	public class FileManagementApi : CachableApi, IFileManagementApi
	{
		private IApplicationContext applicationContext;
		private IThumbnailApi thumbnailApi;
		private IFileStorageApi fileStorageApi;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileManagementApi"/> class.
		/// </summary>
		/// <param name="applicationContext">Application context.</param>
		/// <param name="fileStorageApi">File storage api.</param>
		/// <param name="thumbnailApi">Thumbnail api.</param>
		public FileManagementApi(IApplicationContext applicationContext, IFileStorageApi fileStorageApi, IThumbnailApi thumbnailApi)
		{
			this.applicationContext = applicationContext;
			this.fileStorageApi = fileStorageApi;
			this.thumbnailApi = thumbnailApi;
		}

		/// <summary>
		/// Save a file.
		/// </summary>
		/// <param name="fileUploadObject">The file uploading object.</param>
		/// <returns>returns file head of the saved file.</returns>
		public FileHeadObject Save(FileUploadObject fileUploadObject)
		{
			Kit.NotNull(fileUploadObject, "fileUploadObject");
			if (!fileUploadObject.Stream.CanRead)
				throw new ArgumentException(Resources.UnreadableFileStream, "fileUploadObject");

			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					File file;
					if (fileUploadObject.Id != Guid.Empty)
					{
						file = ctx.Files.FirstOrDefault(q => q.FileId == fileUploadObject.Id && q.ApplicationId == this.applicationContext.ApplicationId);
						if (file == null)
							throw new ArgumentException(Resources.InvalidFileId, "fileUploadObject");

						// delete related thumbnails
						this.DeleteThumbnails(fileUploadObject.Id);

						// delete the file from cache
						base.RemoveCache(fileUploadObject.Id);

						file.UpdatedOn = DateTime.UtcNow;
						file.Version++;
					}
					else
					{
						file = new File
						{
							FileId = Guid.NewGuid(),
							ApplicationId = this.applicationContext.ApplicationId,
							CreatedOn = DateTime.UtcNow,
							Version = 1
						};

						ctx.Files.InsertOnSubmit(file);
					}

					file.Description = fileUploadObject.Description;
					file.Category = fileUploadObject.Category;
					file.Name = fileUploadObject.FileName;
					file.ExtensionName = Path.GetExtension(fileUploadObject.FileName).Substring(1);
					file.BytesCount = this.fileStorageApi.Store(this.applicationContext.ApplicationId, fileUploadObject.Category, file.FileId, file.ExtensionName, fileUploadObject.Stream);

					ctx.SubmitChanges();
					fileUploadObject.Id = file.FileId;

					return new FileHeadObject
					{
						Id = file.FileId,
						BytesCount = file.BytesCount,
						Category = file.Category,
						Description = file.Description,
						FileExtensionName = file.ExtensionName,
						FileName = file.Name,
						CreatedOn = LocalizationUtility.ConvertUtcTimeToClientTime(file.CreatedOn),
						UpdatedOn = LocalizationUtility.ConvertUtcTimeToClientTime(file.UpdatedOn),
						Version = file.Version
					};
				}
			}
			catch (ArgumentException)
			{
				throw;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Delete the file by id.
		/// </summary>
		/// <param name="fileId">The file id.</param>
		public void Delete(Guid fileId)
		{
			if (fileId == Guid.Empty)
				throw new ArgumentException("fileId");

			this.Delete(new Guid[] { fileId });
		}

		/// <summary>
		/// Delete the files by ids.
		/// </summary>
		/// <param name="fileIds">The file ids.</param>
		public void Delete(IEnumerable<Guid> fileIds)
		{
			Kit.NotNull(fileIds, "fileIds");
			if (fileIds.Count() == 0) return;

			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					Guid[] fileIdsToDelete = fileIds.ToArray();
					IQueryable<File> files = ctx.Files.Where(f => fileIdsToDelete.Contains(f.FileId) && f.ApplicationId == this.applicationContext.ApplicationId);
					IQueryable<FileBinding> boundFiles = ctx.FileBindings.Where(fileBinding => fileIdsToDelete.Contains(fileBinding.FileId));

					ctx.FileBindings.DeleteAllOnSubmit(boundFiles);
					ctx.Files.DeleteAllOnSubmit(files);
					ctx.SubmitChanges();

					foreach (Guid fileId in fileIdsToDelete)
					{
						this.DeleteThumbnails(fileId);
						base.RemoveCache(fileId);
					}
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Loads the file by id.
		/// </summary>
		/// <param name="fileId">The file id.</param>
		/// <returns></returns>
		public FileHeadObject Load(Guid fileId)
		{
			if (fileId == Guid.Empty)
				throw new ArgumentException("fileId");

			FileHeadObject fileHeadObject = base.GetCacheObject<FileHeadObject>(fileId);
			if (fileHeadObject != null) return fileHeadObject;

			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					fileHeadObject = (from file in ctx.Files
									  where file.FileId == fileId && file.ApplicationId == this.applicationContext.ApplicationId
									  select new FileHeadObject
									  {
										  Id = file.FileId,
										  BytesCount = file.BytesCount,
										  FileName = file.Name,
										  Category = file.Category,
										  Description = file.Description,
										  FileExtensionName = file.ExtensionName,
										  CreatedOn = LocalizationUtility.ConvertUtcTimeToClientTime(file.CreatedOn),
										  UpdatedOn = LocalizationUtility.ConvertUtcTimeToClientTime(file.UpdatedOn),
										  Version = file.Version
									  }).FirstOrDefault();

					base.AddCache(fileId, fileHeadObject);
					return fileHeadObject;
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Bulk get the files by ids.
		/// </summary>
		/// <param name="fileIds">The file ids.</param>
		/// <returns></returns>
		public IEnumerable<FileHeadObject> BulkGet(IEnumerable<Guid> fileIds)
		{
			Kit.NotNull(fileIds, "fileIds");
			try
			{
				// try to pull from cache first.
				Dictionary<Guid, FileHeadObject> fileHeadObjectsByIds = new Dictionary<Guid, FileHeadObject>();
				List<Guid> fileIdsToPullFromDb = new List<Guid>();
				foreach (Guid fileId in fileIds)
				{
					FileHeadObject fileHeadObject = base.GetCacheObject<FileHeadObject>(fileId);
					if (fileHeadObject != null)
						fileHeadObjectsByIds.Add(fileId, fileHeadObject);
					else
						fileIdsToPullFromDb.Add(fileId);
				}

				// pull files from db, which not exist in cache.
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					List<FileHeadObject> fileHeadObjectsFromDb = (from f in ctx.Files
																  where fileIdsToPullFromDb.ToArray().Contains(f.FileId) && f.ApplicationId == this.applicationContext.ApplicationId
																  select new FileHeadObject
																  {
																	  Id = f.FileId,
																	  BytesCount = f.BytesCount,
																	  FileName = f.Name,
																	  Category = f.Category,
																	  Description = f.Description,
																	  FileExtensionName = f.ExtensionName,
																	  CreatedOn = f.CreatedOn,
																	  UpdatedOn = f.UpdatedOn,
																	  Version = f.Version
																  }).ToList();

					foreach (FileHeadObject fileHeadObjectFromDb in fileHeadObjectsFromDb)
					{
						fileHeadObjectFromDb.CreatedOn = LocalizationUtility.ConvertUtcTimeToClientTime(fileHeadObjectFromDb.CreatedOn);
						fileHeadObjectFromDb.UpdatedOn = LocalizationUtility.ConvertUtcTimeToClientTime(fileHeadObjectFromDb.UpdatedOn);
						fileHeadObjectsByIds[fileHeadObjectFromDb.Id] = fileHeadObjectFromDb;
						base.AddCache(fileHeadObjectFromDb.Id, fileHeadObjectFromDb);
					}
				}

				// assemble results to return.
				List<FileHeadObject> results = new List<FileHeadObject>();
				foreach (Guid fileId in fileIds)
				{
					if (fileHeadObjectsByIds.ContainsKey(fileId))
						results.Add(fileHeadObjectsByIds[fileId]);
				}

				return results;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		private void DeleteThumbnails(Guid fileId)
		{
			if (this.thumbnailApi != null)
				this.thumbnailApi.DeleteThumbnails(fileId);
		}
	}
}