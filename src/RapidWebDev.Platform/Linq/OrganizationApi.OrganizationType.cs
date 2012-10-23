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
using System.Globalization;
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
		private const string CacheKey4AllOrganizationTypes = "CacheKey4AllOrganizationTypes";

		private IAuthenticationContext authenticationContext;
		private IPlatformConfiguration platformConfiguration;
		private IHierarchyApi hierarchyApi;

		/// <summary>
		/// Construct OrganizationApi instance.
		/// </summary>
		/// <param name="authenticationContext"></param>
		/// <param name="platformConfiguration"></param>
		/// <param name="hierarchyApi"></param>
		public OrganizationApi(IAuthenticationContext authenticationContext, IPlatformConfiguration platformConfiguration, IHierarchyApi hierarchyApi)
			: base(authenticationContext)
		{
			this.authenticationContext = authenticationContext;
			this.platformConfiguration = platformConfiguration;
			this.hierarchyApi = hierarchyApi;
		}

		/// <summary>
		/// Save organization type object.
		/// </summary>
		/// <param name="organizationTypeObject"></param>
		/// <exception cref="ValidationException">etc organization type name does exist.</exception>
		public void Save(OrganizationTypeObject organizationTypeObject)
		{
			Kit.NotNull(organizationTypeObject, "organizationTypeObject");
			Kit.NotNull(organizationTypeObject.Name, "organizationTypeObject.Name");
			if (!this.platformConfiguration.Domains.Select(d => d.Value).Contains(organizationTypeObject.Domain))
				throw new ArgumentException(string.Format(Resources.InvalidOrganizationTypeDomain, organizationTypeObject.Domain), "organizationTypeObject.Domain");

			Kit.NotNull(organizationTypeObject, "organizationTypeObject");
			Kit.NotNull(organizationTypeObject.Name, "organizationTypeObject.Name");
			if (!this.platformConfiguration.Domains.Select(d => d.Value).Contains(organizationTypeObject.Domain))
				throw new ArgumentException(string.Format(Resources.InvalidOrganizationTypeDomain, organizationTypeObject.Domain), "organizationTypeObject.Domain");

			try
			{
				using (TransactionScope ts = new TransactionScope())
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					OrganizationType organizationType = null;

					DeleteStatus originalDeleteStatus = DeleteStatus.Deleted;
					using (ValidationScope validationScope = new ValidationScope(true))
					{
						if (ctx.OrganizationTypes.Where(x => x.Name == organizationTypeObject.Name
							&& x.ApplicationId == this.authenticationContext.ApplicationId
							&& x.OrganizationTypeId != organizationTypeObject.OrganizationTypeId).Count() > 0)
						{
							validationScope.Error(Resources.ExistedOrganizationTypeName, organizationTypeObject.Name);
						}

						if (organizationTypeObject.OrganizationTypeId == Guid.Empty)
						{
							organizationType = new OrganizationType { ApplicationId = this.authenticationContext.ApplicationId };
							ctx.OrganizationTypes.InsertOnSubmit(organizationType);
						}
						else
						{
							organizationType = ctx.OrganizationTypes.FirstOrDefault(orgType => orgType.OrganizationTypeId == organizationTypeObject.OrganizationTypeId);
							if (organizationType == null)
								validationScope.Error(Resources.InvalidOrganizationTypeID);

							originalDeleteStatus = organizationType.DeleteStatus;
						}
					}

					// set organization type properties.
					organizationType.Domain = organizationTypeObject.Domain;
					organizationType.Name = organizationTypeObject.Name;
					organizationType.Description = organizationTypeObject.Description;
					organizationType.LastUpdatedDate = DateTime.UtcNow;
					organizationType.Predefined = organizationTypeObject.Predefined;
					organizationType.DeleteStatus = organizationTypeObject.DeleteStatus;

					// if disable an existed organization type
					if (organizationTypeObject.OrganizationTypeId != Guid.Empty
						&& organizationTypeObject.DeleteStatus == DeleteStatus.Deleted
						&& organizationTypeObject.DeleteStatus != originalDeleteStatus)
					{
						// remove the cache copy for disabled organizations.
						IEnumerable<Guid> disabledOrganizationIds = (from org in ctx.Organizations
																	 where org.ApplicationId == this.authenticationContext.ApplicationId
																		 && org.OrganizationTypeId == organizationTypeObject.OrganizationTypeId
																		 && org.Status != OrganizationStatus.Disabled
																	 select org.OrganizationId).ToList();

						foreach (Guid disabledOrganizationId in disabledOrganizationIds)
							base.RemoveCache(disabledOrganizationId);

						// batch disable the organizations by sql command
						string command = string.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET Status={1} WHERE ApplicationId='{2}' AND OrganizationTypeId='{3}' AND Status<>{1}",
							ctx.Mapping.GetTable(typeof(Organization)).TableName,
							(int)OrganizationStatus.Disabled,
							this.authenticationContext.ApplicationId,
							organizationTypeObject.OrganizationTypeId);

						ctx.ExecuteCommand(command);
					}

					ctx.SubmitChanges();
					ts.Complete();

					organizationTypeObject.OrganizationTypeId = organizationType.OrganizationTypeId;
					organizationTypeObject.LastUpdatedDate = organizationType.LastUpdatedDate;

					// remove cache.
					base.RemoveCache(CacheKey4AllOrganizationTypes);
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
		/// Get organization type business object by id.
		/// </summary>
		/// <param name="organizationTypeId">organization type id</param>
		/// <returns></returns>
		public OrganizationTypeObject GetOrganizationType(Guid organizationTypeId)
		{
			OrganizationTypeObject returnValue = this.FindOrganizationTypesInternal().FirstOrDefault(orgType => orgType.OrganizationTypeId == organizationTypeId);
			if (returnValue == null) return null;

			returnValue = returnValue.Clone();
			returnValue.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(returnValue.LastUpdatedDate);
			return returnValue;
		}

		/// <summary>
		/// Get organization type by name.
		/// </summary>
		/// <param name="organizationTypeName">organization type name</param>
		/// <returns></returns>
		public OrganizationTypeObject GetOrganizationType(string organizationTypeName)
		{
			OrganizationTypeObject returnValue = this.FindOrganizationTypesInternal().FirstOrDefault(orgType => orgType.Name == organizationTypeName);
			if (returnValue == null) return null;

			returnValue = returnValue.Clone();
			returnValue.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(returnValue.LastUpdatedDate);
			return returnValue;
		}

		/// <summary>
		/// Find all organization types for current application.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<OrganizationTypeObject> FindOrganizationTypes()
		{
			return this.FindOrganizationTypesInternal()
				.Select(orgType =>
				{
					OrganizationTypeObject clonedOrgType = orgType.Clone();
					clonedOrgType.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(clonedOrgType.LastUpdatedDate);
					return clonedOrgType;
				});
		}

		/// <summary>
		/// Find all organization types in specified domains in current application.
		/// </summary>
		/// <param name="domains"></param>
		/// <returns></returns>
		public IEnumerable<OrganizationTypeObject> FindOrganizationTypes(IEnumerable<string> domains)
		{
			Kit.NotNull(domains, "domains");
			return this.FindOrganizationTypesInternal()
				.Where(orgType => domains.Contains(orgType.Domain))
				.Select(orgType =>
				{
					OrganizationTypeObject clonedOrgType = orgType.Clone();
					clonedOrgType.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(clonedOrgType.LastUpdatedDate);
					return clonedOrgType;
				});
		}

		private IEnumerable<OrganizationTypeObject> FindOrganizationTypesInternal()
		{
			try
			{
				List<OrganizationTypeObject> organizationTypeObjects = base.GetCacheObject<List<OrganizationTypeObject>>(CacheKey4AllOrganizationTypes);
				if (organizationTypeObjects == null)
				{
					using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
					{
						organizationTypeObjects = (from orgType in ctx.OrganizationTypes
								where orgType.ApplicationId == this.authenticationContext.ApplicationId
								orderby orgType.Name
								select new OrganizationTypeObject()
								{
									OrganizationTypeId = orgType.OrganizationTypeId,
									Name = orgType.Name,
									Domain = orgType.Domain,
									Description = orgType.Description,
									LastUpdatedDate = orgType.LastUpdatedDate,
									Predefined = orgType.Predefined,
									DeleteStatus = orgType.DeleteStatus
								}).ToList();
					}

					base.AddCache(CacheKey4AllOrganizationTypes, organizationTypeObjects);
				}

				return organizationTypeObjects;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}
	}
}
