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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Web;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// The service implementation to search accessible organizations for current authenticated user.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class OrganizationService : IOrganizationService, IRequiresSessionState
    {
        private static IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
        private static IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
        private static IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
        private static IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();
        private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

        /// <summary>
        /// Lists all available organization domains.
        /// </summary>
        /// <returns></returns>
        public Collection<OrganizationDomainObject> ListDomainsJson()
        {
            IEnumerable<OrganizationDomainObject> results = platformConfiguration.Domains.Select(d => new OrganizationDomainObject { Text = d.Text, Value = d.Value });
            return new Collection<OrganizationDomainObject>(results.ToList());
        }
        /// <summary>
        /// Lists all available organization domains.
        /// </summary>
        /// <returns></returns>
        public Collection<OrganizationDomainObject> ListDomainsXml()
        {
            return ListDomainsJson();
        }

        #region Organization Type Services

        /// <summary>
        /// Save organization type object.
        /// </summary>
        /// <param name="organizationTypeObject"></param>
        /// <returns></returns>
        public string SaveOrganizationTypeJson(OrganizationTypeObject organizationTypeObject)
        {
            if (organizationTypeObject == null)
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.OrganizationCannotBeEmpty));
            try
            {
                organizationApi.Save(organizationTypeObject);
                return organizationTypeObject.OrganizationTypeId.ToString();
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Save organization type object.
        /// </summary>
        /// <param name="organizationTypeObject"></param>
        /// <returns></returns>
        public string SaveOrganizationTypeXml(OrganizationTypeObject organizationTypeObject)
        {
            return SaveOrganizationTypeJson(organizationTypeObject);
        }

        /// <summary>
        /// Get the organization type by id.
        /// </summary>
        /// <param name="organizationTypeId"></param>
        /// <returns></returns>
        public OrganizationTypeObject GetOrganizationTypeByIdJson(string organizationTypeId)
        {
            if (string.IsNullOrEmpty(organizationTypeId))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidOrganizationTypeID));
            try
            {
                return organizationApi.GetOrganizationType(new Guid(organizationTypeId));
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Get the organization type by id.
        /// </summary>
        /// <param name="organizationTypeId"></param>
        /// <returns></returns>
        public OrganizationTypeObject GetOrganizationTypeByIdXml(string organizationTypeId)
        {
            return GetOrganizationTypeByIdJson(organizationTypeId);
        }

        /// <summary>
        /// Get the organization type by name.
        /// </summary>
        /// <param name="organizationTypeName"></param>
        /// <returns></returns>
        public OrganizationTypeObject GetOrganizationTypeByNameJson(string organizationTypeName)
        {
            if (string.IsNullOrEmpty(organizationTypeName))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.OrganizationNameCannotBeEmpty));
            try
            {
                return organizationApi.GetOrganizationType(organizationTypeName);
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Get the organization type by name.
        /// </summary>
        /// <param name="organizationTypeName"></param>
        /// <returns></returns>
        public OrganizationTypeObject GetOrganizationTypeByNameXml(string organizationTypeName)
        {
            return GetOrganizationTypeByNameJson(organizationTypeName);
        }

        /// <summary>
        /// Find organization types in specified domain.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public Collection<OrganizationTypeObject> FindOrganizationTypesJson(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidDomain));

            IEnumerable<OrganizationTypeObject> organizationTypes = organizationApi.FindOrganizationTypes(new[] { domain });
			IEnumerable<OrganizationTypeObject> notDeletedOrgTypes = organizationTypes.Where(orgType => orgType.DeleteStatus == DeleteStatus.NotDeleted);
			return new Collection<OrganizationTypeObject>(notDeletedOrgTypes.ToList());
        }

        /// <summary>
        /// Find organization types in specified domain.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public Collection<OrganizationTypeObject> FindOrganizationTypesXml(string domain)
        {
            return FindOrganizationTypesJson(domain);
        }

        #endregion

        #region Organization Method
        /// <summary>
        /// Search organizations by a collection of criterias for the authenticated user of request.
        /// </summary>
        /// <param name="domain">Which domain of the searching organizations.</param>
        /// <param name="orgTypeId">Which organization type the searching organizations should belong to.</param>
        /// <param name="q">Keywords for searching.</param>
        /// <param name="sortDirection">Sorting field name, the default sorting field is LastUpdatedDate.</param>
        /// <param name="sortOrder">Sorting order, DESC or ASC, the default sorting order is DESC.</param>
        /// <param name="start">The start organization index of hit to return.</param>
        /// <param name="limit">The limit of returned organizations.</param>
        /// <returns>The query results object includes total hit count, returned records, start and limit.</returns>
        public OrganizationQueryResult SearchJson(string domain, string orgTypeId, string q, string sortDirection, string sortOrder, int start, int limit)
        {
            #region Arguments Validation

            //if (!authenticationContext.Identity.IsAuthenticated)
            //    throw new BadRequestException( "The access is not authenticated.");

            Guid userId = authenticationContext.User.UserId;

            if (string.IsNullOrEmpty(domain))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidDomain ));

            if (!platformConfiguration.Domains.Select(d => d.Value).Contains(domain))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidDomain ));

            string sortOrderValue = "DESC";
            if (!Kit.IsEmpty(sortOrder))
            {
                sortOrderValue = sortOrder.ToUpperInvariant();
                if (sortOrderValue != "ASC" && sortOrderValue != "DESC")
                    throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "The value of parameter \"sortOrder\" is invalid. The candidate value are ASC and DESC."));
            }

            string sortFieldValue = "LastUpdatedDate";
            if (!Kit.IsEmpty(sortDirection))
                sortFieldValue = sortDirection;

            string orderby = sortFieldValue + " " + sortOrderValue;
            int pageIndex = start / limit;
            int pageSize = limit;

            Guid orgTypeIdValue = Guid.Empty;
            try
            {
                orgTypeIdValue = new Guid(orgTypeId);
            }
            catch
            {
            }

            #endregion

            int recordCount;
            IEnumerable<OrganizationObject> organizations;
            if (Kit.IsEmpty(q))
            {
                LinqPredicate linqPredicate = new LinqPredicate("OrganizationType.Domain=@0 AND Status=@1", domain, OrganizationStatus.Enabled);
                if (orgTypeIdValue != Guid.Empty)
                    linqPredicate.Add("OrganizationTypeId=@0", orgTypeIdValue);

                organizations = organizationApi.FindOrganizations(linqPredicate, orderby, pageIndex, pageSize, out recordCount);
            }
            else
            {
                LinqPredicate linqPredicate = new LinqPredicate("OrganizationType.Domain=@0 AND Status=@1 AND (OrganizationCode.StartsWith(@2) || OrganizationName.Contains(@3))", domain, OrganizationStatus.Enabled, q, q);

                if (orgTypeIdValue != Guid.Empty)
                    linqPredicate.Add("OrganizationTypeId=@0", orgTypeIdValue);

                organizations = organizationApi.FindOrganizations(linqPredicate, orderby, pageIndex, pageSize, out recordCount);
            }

            return new OrganizationQueryResult(organizations.ToList()) { PageIndex = pageIndex, PageSize = pageSize, TotalRecordCount = recordCount };
        }

		/// <summary>
		/// Search organizations by a collection of criterias for the authenticated user of request.
		/// </summary>
		/// <param name="domain">Which domain of the searching organizations.</param>
		/// <param name="orgTypeId">Which organization type the searching organizations should belong to.</param>
		/// <param name="q">Keywords for searching.</param>
		/// <param name="sortField">Sorting field name, the default sorting field is LastUpdatedDate.</param>
		/// <param name="sortDirection">Sorting order, DESC or ASC, the default sorting order is DESC.</param>
		/// <param name="start">The start organization index of hit to return.</param>
		/// <param name="limit">The limit of returned organizations.</param>
		/// <returns>The query results object includes total hit count, returned records, start and limit.</returns>
		public OrganizationQueryResult SearchXml(string domain, string orgTypeId, string q, string sortField, string sortDirection, int start, int limit)
		{
			return SearchJson(domain, orgTypeId, q, sortField, sortDirection, start, limit);
		}

        /// <summary>
        /// Get first organization matching the specified query. 
        /// The matching algorithm is to try to search organizations in following order. Once an organization is found, it's returned as the result.
        /// 1) completely match organization code;
        /// 2) completely match organization name;
        /// 3) match whether there has organizations with code starts with specified query;
        /// 4) match whether there has organizations with name starts with specified query;
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public OrganizationObject GetOrganizationJson(string domain, string q)
        {
            Guid userId = authenticationContext.User.UserId;

            if (string.IsNullOrEmpty(domain))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidDomain ));

            if (!platformConfiguration.Domains.Select(d => d.Value).Contains(domain))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidDomain ));

            int recordCount;
            IEnumerable<OrganizationTypeObject> organizationTypes = organizationApi.FindOrganizationTypes(new[] { domain });
            IEnumerable<Guid> organizationTypeIds = organizationTypes.Select(ct => ct.OrganizationTypeId);

            LinqPredicate linqPredicate = new LinqPredicate("Status=@0 AND OrganizationType.Domain=@1 AND OrganizationCode=@2", OrganizationStatus.Enabled, domain, q);
            IEnumerable<OrganizationObject> organizations = organizationApi.FindOrganizations(linqPredicate, "OrganizationCode", 0, 1, out recordCount);
            if (recordCount > 0)
                return organizations.FirstOrDefault();

            linqPredicate = new LinqPredicate("Status=@0 AND OrganizationType.Domain=@1 AND OrganizationName=@2", OrganizationStatus.Enabled, domain, q);
            organizations = organizationApi.FindOrganizations(linqPredicate, "OrganizationName", 0, 1, out recordCount);
            if (recordCount > 0)
                return organizations.FirstOrDefault();

            linqPredicate = new LinqPredicate("Status=@0 AND OrganizationType.Domain=@1 AND OrganizationCode.StartWith(@2)", OrganizationStatus.Enabled, domain, q);
            organizations = organizationApi.FindOrganizations(linqPredicate, "OrganizationCode", 0, 1, out recordCount);
            if (recordCount > 0)
                return organizations.FirstOrDefault();

            linqPredicate = new LinqPredicate("Status=@0 AND OrganizationType.Domain=@1 AND OrganizationName.Contains(@2)", OrganizationStatus.Enabled, domain, q);
            organizations = organizationApi.FindOrganizations(linqPredicate, "OrganizationName", 0, 1, out recordCount);
            if (recordCount > 0)
                return organizations.FirstOrDefault();

            return null;
        }

		/// <summary>
		/// Get first organization matching the specified query. 
		/// The matching algorithm is to try to search organizations in following order. Once an organization is found, it's returned as the result.
		/// 1) completely match organization code;
		/// 2) completely match organization name;
		/// 3) match whether there has organizations with code starts with specified query;
		/// 4) match whether there has organizations with name starts with specified query;
		/// </summary>
		/// <param name="domain"></param>
		/// <param name="q"></param>
		/// <returns></returns>
		public OrganizationObject GetOrganizationXml(string domain, string q)
		{
			return GetOrganizationJson(domain, q);
		}

        /// <summary>
        /// Save organization  object
        /// </summary>
        /// <param name="organizationObject"></param>
        /// <returns></returns>
        public string SaveOrganizationJson(OrganizationObject organizationObject)
        {
            if (organizationObject == null) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.OrganizationCannotBeEmpty ));
            try
            {
                organizationApi.Save(organizationObject);
                return organizationObject.OrganizationId.ToString();
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Save organization  object
        /// </summary>
        /// <param name="organizationObject"></param>
        /// <returns></returns>
        public string SaveOrganizationXml(OrganizationObject organizationObject)
        {
            return SaveOrganizationJson(organizationObject);

        }

        /// <summary>
        /// Get the organization by id.      
        /// </summary>		
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public OrganizationObject GetOrganizationByIdJson(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidOrganizationID));
            try
            {
                return organizationApi.GetOrganization(new Guid(organizationId));
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Get the organization by id.      
        /// </summary>		
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public OrganizationObject GetOrganizationByIdXml(string organizationId)
        {
            return GetOrganizationByIdJson(organizationId);
        }

        /// <summary>
        /// Get the organization by code.
        /// </summary>		
        /// <param name="organizationCode"></param>
        /// <returns></returns>
        public OrganizationObject GetOrganizationByCodeJson(string organizationCode)
        {
            if (string.IsNullOrEmpty(organizationCode)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.OrganizationCodeCannotBeEmpty));
            try
            {
                return organizationApi.GetOrganizationByCode(organizationCode);
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Get the organization by code.
        /// </summary>		
        /// <param name="organizationCode"></param>
        /// <returns></returns>
        public OrganizationObject GetOrganizationByCodeXml(string organizationCode)
        {
            return GetOrganizationByCodeJson(organizationCode);
        }

        /// <summary>
        /// Get the organization by name.
        /// </summary>		
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public OrganizationObject GetOrganizationByNameJson(string organizationName)
        {
            if (string.IsNullOrEmpty(organizationName)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.OrganizationNameCannotBeEmpty));
            try
            {
                return organizationApi.GetOrganizationByName(organizationName);
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Get the organization by name.
        /// </summary>		
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public OrganizationObject GetOrganizationByNameXml(string organizationName)
        {
            return GetOrganizationByNameJson(organizationName);
        }

        /// <summary>
        /// Find organizations by organization ids.    
        /// </summary>
        /// <param name="organizationIdList">organization ids</param>
        /// <returns></returns>
        public Collection<OrganizationObject> BulkGetOrganizationsByIdsJson(IdCollection organizationIdList)
        {
            if (organizationIdList.Count() == 0) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidOrganizationID));
            try
            {
                IDictionary<Guid,OrganizationObject> results = organizationApi.BulkGetOrganizations(ServicesHelper.ConvertStringCollectionToGuidEnumerable(organizationIdList));
                Collection<OrganizationObject> result = new Collection<OrganizationObject>();
                if(results.Count() == 0)
                    return result;

                foreach(string temp in organizationIdList)
                {
                    result.Add(results[new Guid(temp)]);
                }
                return result;
            }
             catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Find organizations by organization ids.    
        /// </summary>
        /// <param name="organizationIdList">organization ids</param>
        /// <returns></returns>
        public Collection<OrganizationObject> BulkGetOrganizationsByIdsXml(IdCollection organizationIdList)
        {
            return BulkGetOrganizationsByIdsJson(organizationIdList);
        }

        /// <summary>
        /// Query organizations by custom predicates.
        /// </summary>
        /// <param name="orderby">sorting field and direction</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>       
        /// <param name="predicate">linq predicate. see organization properties for predicate at <see cref="RapidWebDev.Platform.Linq.Organization"/>.</param>
        /// <returns>Returns organizations</returns>
        public OrganizationQueryResult QueryOrganizationsJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            int recordCount;
            try
            {
                pageSize = (pageSize == 0) ? 25 : pageSize;

                LinqPredicate linqPredicate = ServicesHelper.ConvertWebPredicateToLinqPredicate(predicate);
                IEnumerable<OrganizationObject> rets = organizationApi.FindOrganizations(linqPredicate, orderby, pageIndex, pageSize, out recordCount);

                OrganizationQueryResult results = new OrganizationQueryResult(rets.ToList())
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecordCount = recordCount
                };
                return results;
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }


        }

        /// <summary>
        /// Query organizations by custom predicates.
        /// </summary>
        /// <param name="orderby">sorting field and direction</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>       
        /// <param name="predicate">linq predicate. see organization properties for predicate at <see cref="RapidWebDev.Platform.Linq.Organization"/>.</param>
        /// <returns>Returns organizations</returns>
        public OrganizationQueryResult QueryOrganizationsXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            return QueryOrganizationsJson(orderby, pageIndex, pageSize, predicate);
        }

        #endregion
    }
}
