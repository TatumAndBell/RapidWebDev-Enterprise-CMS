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
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Linq
{
	/// <summary>
	/// The implementation organization API which intents to cache all organization types and organizations and interact with database by linq-2-sql OrganizationDbProvider.
	/// </summary>
	public partial class OrganizationApi : CachableApi, IOrganizationApi
	{
		/// <summary>
		/// Save organization business object. 
		/// If organizationObject.Id equals Guid.Empty, it means to save a new organization. 
		/// Otherwise it's updating an existed organization.
		/// </summary>
		/// <param name="organizationObject"></param>
		public void Save(OrganizationObject organizationObject)
		{
			Kit.NotNull(organizationObject, "organizationObject");
			Kit.NotNull(organizationObject.OrganizationCode, "organizationObject.OrganizationCode");
			Kit.NotNull(organizationObject.OrganizationName, "organizationObject.OrganizationName");

			if (organizationObject.OrganizationTypeId == Guid.Empty)
				throw new ArgumentException(Resources.InvalidOrganizationTypeID);

			if (organizationObject.Status == OrganizationStatus.None)
				throw new ArgumentException(Resources.OrganizationStatusNotSpecified, "organizationObject.Status");

			try
			{
				using (TransactionScope transactionScope = new TransactionScope())
				{
					bool isUpdate = organizationObject.OrganizationId != Guid.Empty;
					this.SaveOrganization(organizationObject);

					// remove the cache for update
					if (isUpdate)
						base.RemoveCache(organizationObject.OrganizationId);

					transactionScope.Complete();
					UpdateTimeZone(organizationObject);
				}
			}
			catch (ValidationException)
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
		/// Find organization elements by enumerable organization ids.
		/// </summary>
		/// <param name="organizationIdList">enumerabe organization type ids, null or empty value indicates to query all organizations</param>
		/// <returns></returns>
		public IDictionary<Guid, OrganizationObject> BulkGetOrganizations(IEnumerable<Guid> organizationIdList)
		{
			Dictionary<Guid, OrganizationObject> organizationDictionary = new Dictionary<Guid, OrganizationObject>();
			foreach (Guid organizationId in organizationIdList)
			{
				OrganizationObject organizationObject = base.GetCacheObject<OrganizationObject>(organizationId);
				organizationDictionary[organizationId] = organizationObject.Clone();
			}

			IEnumerable<Guid> uncachedOrganizationIdList = organizationDictionary.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key);
			IDictionary<Guid, OrganizationObject> uncachedOrganizationDictionary = this.BulkGetOrganizationsFromDB(uncachedOrganizationIdList);
			foreach (Guid uncachedOrganizationId in uncachedOrganizationDictionary.Keys)
			{
				organizationDictionary[uncachedOrganizationId] = uncachedOrganizationDictionary[uncachedOrganizationId].Clone();
				if (uncachedOrganizationDictionary[uncachedOrganizationId] != null)
					base.AddCache(uncachedOrganizationId, uncachedOrganizationDictionary[uncachedOrganizationId]);
			}

			UpdateTimeZone(organizationDictionary.Values);
			return organizationDictionary;
		}

		/// <summary>
		/// Get organization instance by id.
		/// </summary>
		/// <param name="organizationId">organization id</param>
		/// <returns></returns>
		public OrganizationObject GetOrganization(Guid organizationId)
		{
			try
			{
				OrganizationObject organizationObject = base.GetCacheObject<OrganizationObject>(organizationId);
				if (organizationObject == null)
				{
					using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
					{
						Organization organization = (from org in ctx.Organizations
													 where org.ApplicationId == this.authenticationContext.ApplicationId && org.OrganizationId == organizationId
													 select org).FirstOrDefault();

						if (organization == null) return null;

						organizationObject = Convert2OrganizationObject(ctx, organization);
						base.AddCache(organizationObject.OrganizationId, organizationObject);
					}
				}

				// only returns an deep copy of the organization.
				organizationObject = organizationObject.Clone();
				UpdateTimeZone(organizationObject);

				return organizationObject;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Get organization instance by name.
		/// </summary>
		/// <param name="organizationName">organization name</param>
		/// <returns></returns>
		public OrganizationObject GetOrganizationByName(string organizationName)
		{
			Kit.NotNull(organizationName, "organizationName");

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					Organization organization = (from org in ctx.Organizations
												 where org.ApplicationId == this.authenticationContext.ApplicationId && org.OrganizationName == organizationName
												 select org).FirstOrDefault();

					if (organization == null) return null;

					OrganizationObject organizationObject = Convert2OrganizationObject(ctx, organization);
					UpdateTimeZone(organizationObject);
					return organizationObject;
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Get organization instance by code.
		/// </summary>
		/// <param name="organizationCode">organization code</param>
		/// <returns></returns>
		public OrganizationObject GetOrganizationByCode(string organizationCode)
		{
			Kit.NotNull(organizationCode, "organizationCode");

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					Organization organization = (from org in ctx.Organizations
												 where org.ApplicationId == this.authenticationContext.ApplicationId && org.OrganizationCode == organizationCode
												 select org).FirstOrDefault();

					if (organization == null) return null;

					OrganizationObject organizationObject = Convert2OrganizationObject(ctx, organization);
					UpdateTimeZone(organizationObject);
					return organizationObject;
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Find organization business objects by custom predicates.
		/// </summary>
		/// <param name="predicate">linq predicate. see organization properties for predicate at <see cref="RapidWebDev.Platform.Linq.Organization"/>.</param>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns>Returns enumerable organizations</returns>
		public IEnumerable<OrganizationObject> FindOrganizations(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount)
		{
			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					// set query criteria.
					var q = from org in ctx.Organizations 
							where org.ApplicationId == authenticationContext.ApplicationId 
							select org;

					if (predicate != null && !Kit.IsEmpty(predicate.Expression))
						q = q.Where(predicate.Expression, predicate.Parameters);

					if (!Kit.IsEmpty(orderby))
						q = q.OrderBy(orderby);

					// get count of matched organizations
					recordCount = q.Count();

					// collect organizations
					List<Organization> organizations = q.Skip(pageIndex * pageSize).Take(pageSize).ToList();

					// collect all OrganizationsInHierarchy entities of the organizations.
					Guid[] organizationIdArray = organizations.Select(org => org.OrganizationId).ToArray();
					var organizationsInHierarchyDictionary = (from organizationsInHierarchy in ctx.OrganizationsInHierarchies
															  where organizationsInHierarchy.ApplicationId == this.authenticationContext.ApplicationId
																	&& organizationIdArray.Contains(organizationsInHierarchy.OrganizationId)
															  group organizationsInHierarchy by organizationsInHierarchy.OrganizationId into g
															  select g).ToDictionary(g => g.Key, g => g.ToList());

					List<OrganizationObject> organizationObjects = new List<OrganizationObject>();

					// go through each organization to parse dynamic properties and hierarchies.
					foreach (Organization org in organizations)
					{
						OrganizationObject organizationObject = new OrganizationObject()
						{
							ExtensionDataTypeId = org.ExtensionDataTypeId,
							OrganizationId = org.OrganizationId,
							OrganizationCode = org.OrganizationCode,
							OrganizationName = org.OrganizationName,
							OrganizationTypeId = org.OrganizationTypeId,
							Description = org.Description,
							Status = org.Status,
							CreatedBy = org.CreatedBy,
							CreatedDate = org.CreatedDate,
							LastUpdatedBy = org.LastUpdatedBy,
							LastUpdatedDate = org.LastUpdatedDate,
							ParentOrganizationId = org.ParentOrganizationId
						};

						if (organizationsInHierarchyDictionary.ContainsKey(org.OrganizationId))
						{
							List<OrganizationsInHierarchy> organizationsInHierarchies = organizationsInHierarchyDictionary[org.OrganizationId];
							foreach (OrganizationsInHierarchy organizationsInHierarchy in organizationsInHierarchies)
								organizationObject.Hierarchies[organizationsInHierarchy.HierarchyType] = organizationsInHierarchy.HierarchyDataId;
						}

						organizationObject.ParseExtensionPropertiesFrom(org);
						base.AddCache(organizationObject.OrganizationId, organizationObject.Clone());

						UpdateTimeZone(organizationObject);
						organizationObjects.Add(organizationObject);
					}

					return organizationObjects;
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		#region Save Related

		/// <summary>
		/// Save organization business object. 
		/// If organizationObject.Id equals Guid.Empty, it means to save a new organization. 
		/// Otherwise it's updating an existed organization.
		/// </summary>
		/// <param name="organizationObject"></param>
		private void SaveOrganization(OrganizationObject organizationObject)
		{
			try
			{
				Organization organization = null;

				using (TransactionScope transactionScope = new TransactionScope())
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					using (ValidationScope validationScope = new ValidationScope(true))
					{
						// check duplicate organization name
						int existedOrganizationCount = (from org in ctx.Organizations
														where org.OrganizationName == organizationObject.OrganizationName
															 && org.ApplicationId == this.authenticationContext.ApplicationId
															 && org.OrganizationId != organizationObject.OrganizationId
														select org).Count();
						if (existedOrganizationCount > 0)
							validationScope.Error(Resources.ExistedOrganizationName, organizationObject.OrganizationName);

						// check duplicate organization code
						if (!string.IsNullOrEmpty(organizationObject.OrganizationCode))
						{
							existedOrganizationCount = (from org in ctx.Organizations
														where org.OrganizationCode == organizationObject.OrganizationCode
															 && org.ApplicationId == this.authenticationContext.ApplicationId
															 && org.OrganizationId != organizationObject.OrganizationId
														select org).Count();
							if (existedOrganizationCount > 0)
								validationScope.Error(Resources.ExistedOrganizationCode, organizationObject.OrganizationCode);
						}

						// validate organization type
						OrganizationTypeObject organizationTypeObject = this.GetOrganizationType(organizationObject.OrganizationTypeId);
						if (organizationTypeObject == null)
							validationScope.Error(Resources.InvalidOrganizationTypeID);

						// validate organization located area
						if (organizationObject.Hierarchies != null && organizationObject.Hierarchies.Count > 0)
						{
							foreach (string hierarchyType in organizationObject.Hierarchies.Keys)
							{
								HierarchyDataObject hierarchyDataObject = hierarchyApi.GetHierarchyData(organizationObject.Hierarchies[hierarchyType]);
								if (hierarchyDataObject == null || !string.Equals(hierarchyDataObject.HierarchyType, hierarchyType, StringComparison.OrdinalIgnoreCase))
									validationScope.Error(Resources.InvalidHierarchyAssociatedWithOrganization, hierarchyType, hierarchyDataObject.HierarchyDataId);
							}
						}

						// check circular reference of parent
						if (organizationObject.ParentOrganizationId.HasValue)
						{
							if (organizationObject.OrganizationId == organizationObject.ParentOrganizationId.Value)
								validationScope.Error(Resources.InvalidParentOrganizationID);

							if (organizationObject.OrganizationId != organizationObject.ParentOrganizationId.Value)
							{
								List<string> existedOrganizationNames = new List<string>();
								HashSet<Guid> existedOrganizationIds = new HashSet<Guid> { organizationObject.OrganizationId };
								OrganizationObject parentOrganizationObject = this.GetOrganization(organizationObject.ParentOrganizationId.Value);

								if (parentOrganizationObject == null)
									validationScope.Error(Resources.InvalidParentOrganizationID);
								else
								{
									this.VerifyStatusAgainstParentOrganization(parentOrganizationObject, organizationObject.Status, organizationObject.OrganizationName, validationScope);

									while (parentOrganizationObject != null)
									{
										existedOrganizationNames.Add(parentOrganizationObject.OrganizationName);
										if (existedOrganizationIds.Contains(parentOrganizationObject.OrganizationId))
										{
											validationScope.Error(Resources.ParentOrganizationCircularReference, FormatCircularOrganizationNames(existedOrganizationNames));
											break;
										}

										existedOrganizationIds.Add(parentOrganizationObject.OrganizationId);
										if (!parentOrganizationObject.ParentOrganizationId.HasValue) break;

										parentOrganizationObject = this.GetOrganization(parentOrganizationObject.ParentOrganizationId.Value);
									}
								}
							}
						}

						if (organizationObject.OrganizationId == Guid.Empty)
						{
							organization = ExtensionObjectFactory.Create<Organization>(organizationObject);
							organization.OrganizationCode = organizationObject.OrganizationCode;
							organization.CreatedDate = DateTime.UtcNow;
							organization.CreatedBy = this.authenticationContext.User.UserId;

							ctx.Organizations.InsertOnSubmit(organization);
						}
						else
						{
							organization = ctx.Organizations.FirstOrDefault(org => org.OrganizationId == organizationObject.OrganizationId);
							if (organization == null)
								validationScope.Error(Resources.InvalidOrganizationID);

							organization.ExtensionDataTypeId = organizationObject.ExtensionDataTypeId;

							if (organization.OrganizationCode != organizationObject.OrganizationCode)
								validationScope.Error(Resources.CodeCannotUpdate);

							// update status of all children only when updates status from Enabled to Disabled.
							if (organizationObject.Status == OrganizationStatus.Disabled && organization.Status == OrganizationStatus.Enabled)
								UpdateSubOrganizationsStatus(ctx, new[] { organization.OrganizationId }, organizationObject.Status);
						}
					}

					organization.ApplicationId = this.authenticationContext.ApplicationId;
					organization.OrganizationName = organizationObject.OrganizationName;
					organization.OrganizationTypeId = organizationObject.OrganizationTypeId;
					organization.ParentOrganizationId = organizationObject.ParentOrganizationId;
					organization.Status = organizationObject.Status;
					organization.Description = organizationObject.Description;
					organization.LastUpdatedDate = DateTime.UtcNow;
					organization.LastUpdatedBy = this.authenticationContext.User.UserId;

					// remove original hierarchies and added new ones
					if (organizationObject.OrganizationId != Guid.Empty && organization.Hierarchies.Count > 0)
						ctx.OrganizationsInHierarchies.DeleteAllOnSubmit(organization.Hierarchies);

					if (organizationObject.Hierarchies != null && organizationObject.Hierarchies.Count > 0)
					{
						foreach (string hierarchyType in organizationObject.Hierarchies.Keys)
						{
							HierarchyDataObject hierarchyDataObject = hierarchyApi.GetHierarchyData(organizationObject.Hierarchies[hierarchyType]);
							if (hierarchyDataObject != null)
							{
								ctx.OrganizationsInHierarchies.InsertOnSubmit(new OrganizationsInHierarchy
								{
									ApplicationId = authenticationContext.ApplicationId,
									HierarchyType = hierarchyType,
									HierarchyDataId = organizationObject.Hierarchies[hierarchyType],
									Organization = organization,
								});
							}
						}
					}

					organization.ParseExtensionPropertiesFrom(organizationObject);

					ctx.SubmitChanges();
					transactionScope.Complete();

					organizationObject.OrganizationId = organization.OrganizationId;
					organizationObject.CreatedBy = organization.CreatedBy;
					organizationObject.CreatedDate = organization.CreatedDate;
					organizationObject.LastUpdatedBy = organization.LastUpdatedBy;
					organizationObject.LastUpdatedDate = organization.LastUpdatedDate;
				}
			}
			catch (ValidationException)
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
		/// Validation on if there specifies parent organization to new created one, the status of new organization must be lower priority than its parent.
		/// Organization status priority: Enabled &gt; Pending &gt; Disabled.
		/// </summary>
		/// <param name="parentOrganizationObject"></param>
		/// <param name="status"></param>
		/// <param name="organizationName"></param>
		/// <param name="validationScope"></param>
		private void VerifyStatusAgainstParentOrganization(OrganizationObject parentOrganizationObject, OrganizationStatus status, string organizationName, ValidationScope validationScope)
		{
			OrganizationStatus parentStatus = parentOrganizationObject.Status;
			if ((int)parentStatus < (int)status)
				validationScope.Error(Resources.OrganizationStatusCannotBeHigherThanParent, parentOrganizationObject.OrganizationName, parentStatus, organizationName, status);
		}

		private void UpdateSubOrganizationsStatus(MembershipDataContext ctx, Guid[] parentOrganizationIds, OrganizationStatus status)
		{
			var q = from org in ctx.Organizations
					where org.ParentOrganizationId.HasValue
						&& parentOrganizationIds.Contains(org.ParentOrganizationId.Value)
						&& org.Status != status
					select org;

			if (q.Count() == 0) return;

			foreach (Organization organization in q.AsEnumerable())
			{
				organization.Status = status;
				base.RemoveCache(organization.OrganizationId);
			}

			UpdateSubOrganizationsStatus(ctx, q.Select(org => org.OrganizationId).ToArray(), status);
		}

		private static string FormatCircularOrganizationNames(IEnumerable<string> organizationNames)
		{
			StringBuilder output = new StringBuilder();
			foreach (string organizationName in organizationNames)
			{
				if (output.Length == 0) output.Append(organizationName);
				else output.AppendFormat(" -> {0}", organizationName);
			}

			return output.ToString();
		}

		#endregion

		private static void UpdateTimeZone(OrganizationObject orgObject)
		{
			orgObject.CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(orgObject.CreatedDate);
			orgObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(orgObject.LastUpdatedDate);
		}

		private static void UpdateTimeZone(IEnumerable<OrganizationObject> orgObjects)
		{
			foreach (OrganizationObject orgObject in orgObjects)
			{
				orgObject.CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(orgObject.CreatedDate);
				orgObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(orgObject.LastUpdatedDate);
			}
		}

		private static OrganizationObject Convert2OrganizationObject(MembershipDataContext ctx, Organization org)
		{
			OrganizationObject organizationObject = new OrganizationObject()
			{
				ExtensionDataTypeId = org.ExtensionDataTypeId,
				OrganizationId = org.OrganizationId,
				OrganizationCode = org.OrganizationCode,
				OrganizationName = org.OrganizationName,
				OrganizationTypeId = org.OrganizationTypeId,
				Description = org.Description,
				Status = org.Status,
				CreatedBy = org.CreatedBy,
				CreatedDate = org.CreatedDate,
				LastUpdatedBy = org.LastUpdatedBy,
				LastUpdatedDate = org.LastUpdatedDate,
				ParentOrganizationId = org.ParentOrganizationId
			};

			foreach (OrganizationsInHierarchy organizationsInHierarchy in org.Hierarchies)
				organizationObject.Hierarchies[organizationsInHierarchy.HierarchyType] = organizationsInHierarchy.HierarchyDataId;

			organizationObject.ParseExtensionPropertiesFrom(org);
			return organizationObject;
		}

		/// <summary>
		/// Find organization elements by enumerable organization ids.
		/// </summary>
		/// <param name="organizationIdList">enumerabe organization type ids, null or empty value indicates to query all organizations</param>
		/// <returns></returns>
		private IDictionary<Guid, OrganizationObject> BulkGetOrganizationsFromDB(IEnumerable<Guid> organizationIdList)
		{
			try
			{
				if (organizationIdList.Count() == 0)
					return new Dictionary<Guid, OrganizationObject>();

				Guid[] organizationIdArray = organizationIdList.ToArray();
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					// collect all organizations
					var q = ctx.Organizations.Where(org => organizationIdArray.Contains(org.OrganizationId) && org.ApplicationId == authenticationContext.ApplicationId);
					List<Organization> organizations = (from org in ctx.Organizations
														where organizationIdArray.Contains(org.OrganizationId)
															 && org.ApplicationId == authenticationContext.ApplicationId
														select org).Distinct().ToList();

					// collect related hierarchies for the organizations.
					var organizationsInHierarchyDictionary = (from organizationsInHierarchy in ctx.OrganizationsInHierarchies
															  where organizationsInHierarchy.ApplicationId == this.authenticationContext.ApplicationId
																	&& organizationIdArray.Contains(organizationsInHierarchy.OrganizationId)
															  group organizationsInHierarchy by organizationsInHierarchy.OrganizationId into g
															  select g).ToDictionary(g => g.Key, g => g.ToList());

					List<OrganizationObject> organizationObjects = new List<OrganizationObject>();

					// go through each organization to parse dynamic properties and hierarchies.
					foreach (Organization org in organizations)
					{
						OrganizationObject organizationObject = new OrganizationObject()
						{
							ExtensionDataTypeId = org.ExtensionDataTypeId,
							OrganizationId = org.OrganizationId,
							OrganizationCode = org.OrganizationCode,
							OrganizationName = org.OrganizationName,
							OrganizationTypeId = org.OrganizationTypeId,
							Description = org.Description,
							Status = org.Status,
							CreatedBy = org.CreatedBy,
							CreatedDate = org.CreatedDate,
							LastUpdatedBy = org.LastUpdatedBy,
							LastUpdatedDate = org.LastUpdatedDate,
							ParentOrganizationId = org.ParentOrganizationId
						};

						if (organizationsInHierarchyDictionary.ContainsKey(org.OrganizationId))
						{
							List<OrganizationsInHierarchy> organizationsInHierarchies = organizationsInHierarchyDictionary[org.OrganizationId];
							foreach (OrganizationsInHierarchy organizationsInHierarchy in organizationsInHierarchies)
								organizationObject.Hierarchies[organizationsInHierarchy.HierarchyType] = organizationsInHierarchy.HierarchyDataId;
						}

						organizationObject.ParseExtensionPropertiesFrom(org);
						base.AddCache(organizationObject.OrganizationId, organizationObject.Clone());

						UpdateTimeZone(organizationObject);
						organizationObjects.Add(organizationObject);
					}

					return organizationObjects.ToDictionary(org => org.OrganizationId, org => org);
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}
	}
}