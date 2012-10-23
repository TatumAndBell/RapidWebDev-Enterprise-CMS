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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using RapidWebDev.Platform.Properties;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.Platform.Linq
{
	/// <summary>
	/// The hierarchy API is used for CRUD generic data within hierarchy using linq-2-sql.
	/// The common business scenario likes Geography that each area in geography potentially has a parent and multiple children.
	/// The example likes the managed geography can be selected as shipping destination city when the user creates a order for shipping in product-order system.
	/// In this case we don't need to design the database schema and create API to CRUD areas in geography any more with Hierarchy model provided in the RapidWebDev. 
	/// We can use IHierarchyApi to CRUD any hierarchy data generally. 
	/// The hierarchy data object focused in the IHierarchyApi includes basic information likes Code, Name, Description, HierarchyType and it integrates the extension model provided in RapidWebDev. 
	/// The property HierarchyType is used to categorize the hierarchy data which provides the capacity to manage multiple hierarchy objects in a system at a time, likes Geography, Functional Zone.
	/// With extension model integrated into Hierarchy, we can define the dynamic properties for hierarchy data based on the needs of business. 
	/// </summary>
	public class HierarchyApi : CachableApi, IHierarchyApi
	{
		private IAuthenticationContext authenticationContext;

		/// <summary>
		/// Construct HierarchyApi
		/// </summary>
		/// <param name="authenticationContext"></param>
		public HierarchyApi(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
			this.authenticationContext = authenticationContext;
		}

		/// <summary>
		/// Save a hierarchy data object.
		/// </summary>
		/// <param name="hierarchyDataObject"></param>
		public void Save(HierarchyDataObject hierarchyDataObject)
		{
			Kit.NotNull(hierarchyDataObject, "hierarchyDataObject");
			Kit.NotNull(hierarchyDataObject.Name, "hierarchyDataObject.Name");
			Kit.NotNull(hierarchyDataObject.HierarchyType, "hierarchyDataObject.HierarchyType");

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				using (ValidationScope validationScope = new ValidationScope(true))
				{
					// check whether the hierarchy type of parent equals to current one's
					if (hierarchyDataObject.ParentHierarchyDataId.HasValue)
					{
						HierarchyDataObject parentHierarchyDataObject = this.GetHierarchyData(hierarchyDataObject.ParentHierarchyDataId.Value);
						if (parentHierarchyDataObject == null)
							validationScope.Error(Resources.InvalidParentHierarchyDataId);
					}

					// whether to check cycle reference on parent hierarchy data id? 
					// TODO:

					// check duplicate hierarchy data name in a hierarchy type.
					HierarchyDataObject duplicateHierarchyDataObject = this.GetHierarchyData(hierarchyDataObject.HierarchyType, hierarchyDataObject.Name);
					if (duplicateHierarchyDataObject != null && duplicateHierarchyDataObject.HierarchyDataId != hierarchyDataObject.HierarchyDataId)
						validationScope.Error(Resources.DuplicateHierarchyDataName);

					// check whether the hierarchy type has been changed.
					if (hierarchyDataObject.HierarchyDataId != Guid.Empty)
					{
						var existedHierarchyDataObject = this.GetHierarchyData(hierarchyDataObject.HierarchyDataId);
						if (existedHierarchyDataObject == null)
							validationScope.Error(Resources.InvalidHierarchyDataId);

						if (existedHierarchyDataObject.HierarchyType != hierarchyDataObject.HierarchyType)
							validationScope.Error(Resources.HierarchyTypeCannotBeChanged);
					}
				}

				HierarchyData hierarchyData = null;
				if (hierarchyDataObject.HierarchyDataId == Guid.Empty)
				{
					hierarchyData = ExtensionObjectFactory.Create<HierarchyData>(hierarchyDataObject);
					hierarchyData.ApplicationId = this.authenticationContext.ApplicationId;
					hierarchyData.CreatedBy = this.authenticationContext.User.UserId;
					hierarchyData.CreatedDate = DateTime.UtcNow;

					ctx.HierarchyDatas.InsertOnSubmit(hierarchyData);
				}
				else
				{
					hierarchyData = ctx.HierarchyDatas.FirstOrDefault(d => d.HierarchyDataId == hierarchyDataObject.HierarchyDataId);
				}

				hierarchyData.HierarchyType = hierarchyDataObject.HierarchyType;
				hierarchyData.ParentHierarchyDataId = hierarchyDataObject.ParentHierarchyDataId;
				hierarchyData.Code = hierarchyDataObject.Code;
				hierarchyData.Name = hierarchyDataObject.Name;
				hierarchyData.Description = hierarchyDataObject.Description;
				hierarchyData.LastUpdatedBy = this.authenticationContext.User.UserId;
				hierarchyData.LastUpdatedDate = DateTime.UtcNow;

				hierarchyData.ParseExtensionPropertiesFrom(hierarchyDataObject);
				ctx.SubmitChanges();

				hierarchyDataObject.HierarchyDataId = hierarchyData.HierarchyDataId;
				hierarchyDataObject.CreatedBy = hierarchyData.CreatedBy;
				hierarchyDataObject.CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(hierarchyData.CreatedDate);
				hierarchyDataObject.LastUpdatedBy = hierarchyData.LastUpdatedBy;
				hierarchyDataObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(hierarchyData.LastUpdatedDate);

				base.RemoveCache(hierarchyDataObject.HierarchyDataId);
				base.RemoveCache(hierarchyData.HierarchyType);
			}
		}

		/// <summary>
		/// Get a hierarchy data object by id.
		/// </summary>
		/// <param name="hierarchyDataId"></param>
		/// <returns></returns>
		public HierarchyDataObject GetHierarchyData(Guid hierarchyDataId)
		{
			HierarchyDataObject hierarchyDataObject = base.GetCacheObject<HierarchyDataObject>(hierarchyDataId);

			// if the object is not cached, then load it from database.
			if (hierarchyDataObject == null)
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					HierarchyData entity = (from hd in ctx.HierarchyDatas
											where hd.ApplicationId == this.authenticationContext.ApplicationId && hd.HierarchyDataId == hierarchyDataId
											select hd).FirstOrDefault();

					// nothing found.
					if (entity == null) return null;

					hierarchyDataObject = new HierarchyDataObject
					{
						HierarchyDataId = entity.HierarchyDataId,
						HierarchyType = entity.HierarchyType,
						Code = entity.Code,
						Name = entity.Name,
						Description = entity.Description,
						ParentHierarchyDataId = entity.ParentHierarchyDataId,
						CreatedBy = entity.CreatedBy,
						CreatedDate = entity.CreatedDate,
						ExtensionDataTypeId = entity.ExtensionDataTypeId,
						LastUpdatedBy = entity.LastUpdatedBy,
						LastUpdatedDate = entity.LastUpdatedDate
					};

					// parse dynamic properties.
					hierarchyDataObject.ParseExtensionPropertiesFrom(entity);

					// cache the business copy with UTC datetime.
					base.AddCache(hierarchyDataObject.HierarchyDataId, hierarchyDataObject);
				}
			}

			// only return the copy so that the datetime convertion doesn't impact the cached object.
			hierarchyDataObject = hierarchyDataObject.Clone();

			// convert UTC datetime into client timezone before return.
			hierarchyDataObject.CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(hierarchyDataObject.CreatedDate);
			hierarchyDataObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(hierarchyDataObject.LastUpdatedDate);
			return hierarchyDataObject;
		}

		/// <summary>
		/// Get a hierarchy data object by name.
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="hierarchyDataName"></param>
		/// <returns></returns>
		public HierarchyDataObject GetHierarchyData(string hierarchyType, string hierarchyDataName)
		{
			IEnumerable<HierarchyDataObject> allHierarchyData = this.GetAllHierarchyDataInternal(hierarchyType);
			return allHierarchyData.FirstOrDefault(d => d.Name == hierarchyDataName);
		}

		/// <summary>
		/// Get all children of the specified hierarchy data.
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="parentHierarchyDataId"></param>
		/// <returns></returns>
		public IEnumerable<HierarchyDataObject> GetImmediateChildren(string hierarchyType, Guid? parentHierarchyDataId)
		{
			IEnumerable<HierarchyDataObject> allHierarchyData = this.GetAllHierarchyDataInternal(hierarchyType);
			return allHierarchyData.Where(d => d.ParentHierarchyDataId == parentHierarchyDataId);
		}

		/// <summary>
		/// Get all children of the specified hierarchy data includes not immediately.
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="parentHierarchyDataId"></param>
		/// <returns></returns>
		public IEnumerable<HierarchyDataObject> GetAllChildren(string hierarchyType, Guid? parentHierarchyDataId)
		{
			IEnumerable<HierarchyDataObject> allHierarchyData = this.GetAllHierarchyDataInternal(hierarchyType);
			List<HierarchyDataObject> results = new List<HierarchyDataObject>();
			RecursivelyCollectChildren(parentHierarchyDataId, allHierarchyData, results);
			return results;
		}

		/// <summary>
		/// Get all hierarchy data in specified hierarchy type.
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <returns></returns>
		public IEnumerable<HierarchyDataObject> GetAllHierarchyData(string hierarchyType)
		{
			Kit.NotNull(hierarchyType, "hierarchyType");
			return this.GetAllHierarchyDataInternal(hierarchyType);
		}

		/// <summary>
		/// Delete a hierarchy data with all its children by id. 
		/// </summary>
		/// <param name="hierarchyDataId"></param>
		public void HardDeleteHierarchyData(Guid hierarchyDataId)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				HierarchyData hierarchyData = ctx.HierarchyDatas.FirstOrDefault(d => d.ApplicationId == this.authenticationContext.ApplicationId && d.HierarchyDataId == hierarchyDataId);
				if (hierarchyData == null) return;

				ctx.HierarchyDatas.DeleteOnSubmit(hierarchyData);
				this.RecursivelyDeleteChildHierarchyData(ctx, new[] { hierarchyDataId });

				ctx.SubmitChanges();
				base.RemoveCache(hierarchyData.HierarchyType);
			}
		}

		/// <summary>
		/// Find hierarchy data in all types by custom predicates.
		/// </summary>
		/// <param name="predicate">linq predicate which supports properties of <see cref="RapidWebDev.Platform.HierarchyDataObject"/> for query expression.</param>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns></returns>
		public IEnumerable<HierarchyDataObject> FindHierarchyData(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount)
		{
			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					IQueryable<HierarchyData> q = from hd in ctx.HierarchyDatas
												   where hd.ApplicationId == this.authenticationContext.ApplicationId
												   select hd;

					if (predicate != null && !string.IsNullOrEmpty(predicate.Expression))
						q = q.Where(predicate.Expression, predicate.Parameters);

					if (!Kit.IsEmpty(orderby))
						q = q.OrderBy(orderby);

					recordCount = q.Count();
					List<HierarchyData> hierarchyDataSets = q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
					List<HierarchyDataObject> hierarchyDataObjects = new List<HierarchyDataObject>();
					foreach (HierarchyData entity in hierarchyDataSets)
					{
						HierarchyDataObject hierarchyDataObject = new HierarchyDataObject
						{
							HierarchyDataId = entity.HierarchyDataId,
							HierarchyType = entity.HierarchyType,
							Code = entity.Code,
							Name = entity.Name,
							Description = entity.Description,
							ParentHierarchyDataId = entity.ParentHierarchyDataId,
							ExtensionDataTypeId = entity.ExtensionDataTypeId,
							CreatedBy = entity.CreatedBy,
							CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(entity.CreatedDate),
							LastUpdatedBy = entity.LastUpdatedBy,
							LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(entity.LastUpdatedDate)
						};

						hierarchyDataObject.ParseExtensionPropertiesFrom(entity);
						hierarchyDataObjects.Add(hierarchyDataObject);
					}

					return hierarchyDataObjects;
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		private IEnumerable<HierarchyDataObject> GetAllHierarchyDataInternal(string hierarchyType)
		{
			Kit.NotNull(hierarchyType, "hierarchyType");

			CloneableList<HierarchyDataObject> results = base.GetCacheObject<CloneableList<HierarchyDataObject>>(hierarchyType);
			if (results == null)
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					List<HierarchyData> entities = (from hd in ctx.HierarchyDatas
													where hd.ApplicationId == this.authenticationContext.ApplicationId && hd.HierarchyType == hierarchyType
													select hd).ToList();

					results = new CloneableList<HierarchyDataObject>();
					foreach (HierarchyData entity in entities)
					{
						HierarchyDataObject hierarchyDataObject = new HierarchyDataObject
						{
							HierarchyDataId = entity.HierarchyDataId,
							HierarchyType = entity.HierarchyType,
							Code = entity.Code,
							Name = entity.Name,
							Description = entity.Description,
							ParentHierarchyDataId = entity.ParentHierarchyDataId,
							CreatedBy = entity.CreatedBy,
							CreatedDate = entity.CreatedDate,
							ExtensionDataTypeId = entity.ExtensionDataTypeId,
							LastUpdatedBy = entity.LastUpdatedBy,
							LastUpdatedDate = entity.LastUpdatedDate
						};

						hierarchyDataObject.ParseExtensionPropertiesFrom(entity);
						results.Add(hierarchyDataObject);
					}

					// only cache the business objects with UTC datetime
					base.AddCache(hierarchyType, results);
				}
			}

			// get the deep copy of the list so that the timezone change doesn't impact the cached copy.
			results = results.Clone();

			// convert UTC datetime into client timezone.
			results.ForEach(h =>
				{
					h.CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(h.CreatedDate);
					h.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(h.LastUpdatedDate);
				});

			return results;
		}

		private void RecursivelyDeleteChildHierarchyData(MembershipDataContext ctx, Guid[] parentHierarchyDataIds)
		{
			if (parentHierarchyDataIds == null || parentHierarchyDataIds.Length == 0) return;

			foreach (Guid parentHierarchyDataId in parentHierarchyDataIds)
				base.RemoveCache(parentHierarchyDataId);

			List<HierarchyData> hierarchyDataList = ctx.HierarchyDatas.Where(d => parentHierarchyDataIds.Contains(d.ParentHierarchyDataId.Value)).ToList();
			ctx.HierarchyDatas.DeleteAllOnSubmit(hierarchyDataList);

			Guid[] hierarchyDataIdArray = hierarchyDataList.Select(d => d.HierarchyDataId).ToArray();
			RecursivelyDeleteChildHierarchyData(ctx, hierarchyDataIdArray);
		}

		private static void RecursivelyCollectChildren(Guid? parentHierarchyDataId, IEnumerable<HierarchyDataObject> allHierarchyData, List<HierarchyDataObject> results)
		{
			List<HierarchyDataObject> immediateChildren = allHierarchyData.Where(d => d.ParentHierarchyDataId == parentHierarchyDataId).ToList();
			if (immediateChildren.Count() == 0) return;

			foreach (HierarchyDataObject immediateChild in immediateChildren)
			{
				results.Add(immediateChild);
				RecursivelyCollectChildren(immediateChild.HierarchyDataId, allHierarchyData, results);
			}
		}
	}
}