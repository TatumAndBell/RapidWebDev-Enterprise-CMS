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
using RapidWebDev.Common;
using RapidWebDev.Common.Data;

namespace RapidWebDev.FileManagement.Linq
{
	/// <summary>
	/// Api to manage relationship between files and external objects.
	/// </summary>
	public class FileBindingApi : IFileBindingApi
	{
		private IApplicationContext applicationContext;
		private IFileManagementApi fileManagementApi;

		/// <summary>
		/// Construct FileBindingApi instance.
		/// </summary>
		/// <param name="applicationContext"></param>
		/// <param name="fileManagementApi"></param>
		public FileBindingApi(IApplicationContext applicationContext, IFileManagementApi fileManagementApi)
		{
			this.applicationContext = applicationContext;
			this.fileManagementApi = fileManagementApi;
		}

		#region IFileBinderService Members


		/// <summary>
		/// Bind the file with id of the external object.
		/// </summary>
		/// <param name="relationshipType"></param>
		/// <param name="externalObjectId"></param>
		/// <param name="fileId"></param>
		public void Bind(string relationshipType, Guid externalObjectId, Guid fileId)
		{
			this.Bind(relationshipType, externalObjectId, new Guid[] { fileId });
		}

		/// <summary>
		///Bind the files with id of the external object.
		/// </summary>
		/// <param name="relationshipType"></param>
		/// <param name="externalObjectId"></param>
		/// <param name="fileIds"></param>
		public void Bind(string relationshipType, Guid externalObjectId, IEnumerable<Guid> fileIds)
		{
			try
			{
				using (TransactionScope ts = new TransactionScope())
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					List<Guid> associatedFileIds = (from f in ctx.FileBindings
												   where f.ApplicationId == applicationContext.ApplicationId
														&& f.RelationshipType == relationshipType
														&& f.ExtenalObjectId == externalObjectId
												   select f.FileId).Distinct().ToList();

					foreach (Guid fileId in fileIds)
					{
						if (associatedFileIds.Contains(fileId)) continue;
						ctx.FileBindings.InsertOnSubmit(new FileBinding
							  {
								  ApplicationId = this.applicationContext.ApplicationId,
								  RelationshipType = relationshipType,
								  ExtenalObjectId = externalObjectId,
								  FileId = fileId
							  });
					}

					ctx.SubmitChanges();
					ts.Complete();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Unbind all files associated with id of the external object.
		/// </summary>
		/// <param name="externalObjectId"></param>
		public void Unbind(Guid externalObjectId)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					IQueryable<FileBinding> q = from fileBinding in ctx.FileBindings
												where fileBinding.ApplicationId == this.applicationContext.ApplicationId
													&& fileBinding.ExtenalObjectId == externalObjectId
												select fileBinding;

					ctx.FileBindings.DeleteAllOnSubmit(q);
					ctx.SubmitChanges();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Unbind the files associated with id of the external object in special relationship type.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <param name="relationshipType"></param>
		public void Unbind(Guid externalObjectId, string relationshipType)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					IQueryable<FileBinding> q = from fileBinding in ctx.FileBindings
												where fileBinding.ApplicationId == this.applicationContext.ApplicationId
													&& fileBinding.ExtenalObjectId == externalObjectId
													&& fileBinding.RelationshipType == relationshipType
												select fileBinding;

					ctx.FileBindings.DeleteAllOnSubmit(q);
					ctx.SubmitChanges();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Unbind the file with id of the external object.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <param name="fileId"></param>
		public void Unbind(Guid externalObjectId, Guid fileId)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					IQueryable<FileBinding> q = from fileBinding in ctx.FileBindings
												where fileBinding.ApplicationId == this.applicationContext.ApplicationId
													&& fileBinding.ExtenalObjectId == externalObjectId
													&& fileBinding.FileId == fileId
												select fileBinding;

					ctx.FileBindings.DeleteAllOnSubmit(q);
					ctx.SubmitChanges();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Unbind the files with id of the external object.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <param name="fileIds"></param>
		public void Unbind(Guid externalObjectId, IEnumerable<Guid> fileIds)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					IQueryable<FileBinding> q = from fileBinding in ctx.FileBindings
												where fileBinding.ApplicationId == this.applicationContext.ApplicationId
													&& fileBinding.ExtenalObjectId == externalObjectId
													&& fileIds.ToArray().Contains(fileBinding.FileId)
												select fileBinding;

					ctx.FileBindings.DeleteAllOnSubmit(q);
					ctx.SubmitChanges();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Delete all files associated with id of the external object in any relationship types include both file entities and relationship.
		/// </summary>
		/// <param name="externalObjectId"></param>
		public void DeleteBoundFiles(Guid externalObjectId)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					List<FileBinding> fileBindings = (from fileBinding in ctx.FileBindings
													  where fileBinding.ApplicationId == this.applicationContext.ApplicationId
														  && fileBinding.ExtenalObjectId == externalObjectId
													  select fileBinding).ToList();

					this.fileManagementApi.Delete(fileBindings.Select(fileBinding => fileBinding.FileId));
					ctx.FileBindings.DeleteAllOnSubmit(fileBindings);
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Delete the files associated with id of the external object in special relationship type include both file entities and relationship.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <param name="relationshipType"></param>
		public void DeleteBoundFiles(Guid externalObjectId, string relationshipType)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					List<FileBinding> fileBindings = (from fileBinding in ctx.FileBindings
													  where fileBinding.ApplicationId == this.applicationContext.ApplicationId
														  && fileBinding.ExtenalObjectId == externalObjectId
														  && fileBinding.RelationshipType == relationshipType
													  select fileBinding).ToList();

					this.fileManagementApi.Delete(fileBindings.Select(fileBinding => fileBinding.FileId));
					ctx.FileBindings.DeleteAllOnSubmit(fileBindings);
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Get all files bound to id of the external object in any relationship types.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <returns></returns>
		public IEnumerable<FileHeadObject> FindBoundFiles(Guid externalObjectId)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					IQueryable<Guid> fileIds = from fileBinding in ctx.FileBindings
											   where fileBinding.ExtenalObjectId == externalObjectId
													&& fileBinding.ApplicationId == this.applicationContext.ApplicationId
											   select fileBinding.FileId;

					return this.fileManagementApi.BulkGet(fileIds);
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Get the files bound to id of the external object in special relationship type.
		/// </summary>
		/// <param name="externalObjectId"></param>
		/// <param name="relationshipType"></param>
		/// <returns></returns>
		public IEnumerable<FileHeadObject> FindBoundFiles(Guid externalObjectId, string relationshipType)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					List<Guid> fileIds = (from fileBinding in ctx.FileBindings
										  where fileBinding.ExtenalObjectId == externalObjectId
											   && fileBinding.ApplicationId == this.applicationContext.ApplicationId
											   && fileBinding.RelationshipType == relationshipType
										  select fileBinding.FileId).ToList();

					return this.fileManagementApi.BulkGet(fileIds);
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Get files bound to ids of the external objects.
		/// </summary>
		/// <param name="externalObjectIds"></param>
		/// <returns></returns>
		public IDictionary<Guid, IEnumerable<FileHeadObject>> FindBoundFiles(IEnumerable<Guid> externalObjectIds)
		{
			try
			{
				using (FileManagementDataContext ctx = DataContextFactory.Create<FileManagementDataContext>())
				{
					List<FileBinding> fileBindings = (from fileBinding in ctx.FileBindings
													  where externalObjectIds.ToArray().Contains(fileBinding.ExtenalObjectId)
														   && fileBinding.ApplicationId == this.applicationContext.ApplicationId
													  select fileBinding).ToList();

					Dictionary<Guid, IEnumerable<Guid>> fileIdsByExternalObjectIds = fileBindings.GroupBy(g => g.ExtenalObjectId)
						.ToDictionary(kvp => kvp.Key, kvp => kvp.Select(fileBinding => fileBinding.FileId).Distinct());

					IEnumerable<FileHeadObject> fileHeadObjects = this.fileManagementApi.BulkGet(fileBindings.Select(fileBinding => fileBinding.FileId));
					return fileIdsByExternalObjectIds.ToDictionary(kvp => kvp.Key, kvp => fileHeadObjects.Where(fileHeadObject => kvp.Value.Contains(fileHeadObject.Id)));
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		#endregion
	}
}