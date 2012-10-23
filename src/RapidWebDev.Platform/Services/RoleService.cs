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
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Web;
using RapidWebDev.Platform.Linq;
using System.Globalization;
using RapidWebDev.Platform.Properties;
using RapidWebDev.Platform.Initialization;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// The service implementation to search roles
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RoleService : IRoleService
    {
        private static ICache cache = SpringContext.Current.GetObject<ICache>();
        private static IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
        private static IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
        private static IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();

        #region IRoleService Members

        /// <summary>
        /// Save role business object. 
        /// It does create/update based on roleObject.Id. If id is empty, the method will create a new role object.
        /// If the specified id is invalid, the method will throw an exception.       
        /// </summary>
        /// <param name="roleObject">role object</param>
        /// <returns>returns id of the accessing role.</returns>
        public string SaveJson(RoleObject roleObject)
        {
            if (roleObject == null) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidRoleID + "{0}", "Role Object cannot be null"));
            try
            {
                roleApi.Save(roleObject);
                return roleObject.RoleId.ToString();
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
        /// Save role business object. 
        /// It does create/update based on roleObject.Id. If id is empty, the method will create a new role object.
        /// If the specified id is invalid, the method will throw an exception.
        /// </summary>
        /// <param name="roleObject">role object</param>
        /// <returns>returns id of the accessing role.</returns>
        public string SaveXml(RoleObject roleObject)
        {
            return SaveJson(roleObject);
        }

        /// <summary>
        /// Delete role by id.   
        /// </summary>
        /// <param name="roleId"></param>
        public void HardDeleteJson(string roleId)
        {
            if (string.IsNullOrEmpty(roleId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidRoleID + "{0}", "Role Id cannot be empty"));
            Guid id = Guid.Empty;
            try
            {
                id = new Guid(roleId);

                roleApi.HardDelete(id);
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
        /// Delete role by id.        
        /// </summary>
        /// <param name="roleId"></param>
        public void HardDeleteXml(string roleId)
        {
            HardDeleteJson(roleId);
        }

        /// <summary>
        /// Set user into roles which overwrites all existed roles of user.    
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        public void SetUserToRolesJson(string userId, IdCollection roleIds)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidUserID + "{0}", "User Id cannot be null"));
            if (roleIds.Count() == 0) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidRoleID + "{0}", "Role Id cannot be empty"));

            Guid userGId = Guid.Empty;
            try
            {
                userGId = new Guid(userId);
                roleApi.SetUserToRoles(userGId, ServicesHelper.ConvertStringCollectionToGuidCollection(roleIds));
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
        /// Set user into roles which overwrites all existed roles of user.       
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        public void SetUserToRolesXml(string userId, IdCollection roleIds)
        {
            SetUserToRolesJson(userId, roleIds);
        }

        /// <summary>
        /// Get role by role name.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>        
        public RoleObject GetByNameJson(string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.RoleNameCannotBeEmpty + "{0}", "Role Name cannot be empty"));
            try
            {
                return roleApi.Get(roleName);
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
        /// Get role by role name.      
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public RoleObject GetByNameXml(string roleName)
        {
            return GetByNameJson(roleName);
        }

        /// <summary>
        /// Get role by role id.        
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RoleObject GetByIdJson(string roleId)
        {
            if (string.IsNullOrEmpty(roleId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidRoleID + "{0}", "Role Id cannot be empty"));
            Guid id = Guid.Empty;
            try
            {
                id = new Guid(roleId);
                return roleApi.Get(id);
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
        /// Get role by role id.        
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RoleObject GetByIdXml(string roleId)
        {
            return GetByIdJson(roleId);
        }

        /// <summary>
        /// Bulkget role objects by a collection of role ids.        
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public Collection<RoleObject> BulkGetJson(IdCollection roleIds)
        {
            if (roleIds.Count() == 0) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidRoleID + "{0}", "Role Id cannot be empty"));

            try
            {
                IDictionary<Guid, RoleObject> results = roleApi.BulkGet(ServicesHelper.ConvertStringCollectionToGuidEnumerable(roleIds));
                if (results.Count() == 0) return new Collection<RoleObject>();
                return new Collection<RoleObject>(results.Values.ToList());
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
        /// Bulkget role objects by a collection of role ids.        
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public Collection<RoleObject> BulkGetXml(IdCollection roleIds)
        {
            return BulkGetJson(roleIds);
        }

        /// <summary>
        /// Find all available roles.       
        /// </summary>
        /// <returns></returns>
        public Collection<RoleObject> FindAllJson()
        {
            IEnumerable<RoleObject> results = roleApi.FindAll();
            if (results.Count() == 0) return new Collection<RoleObject>();
            return new Collection<RoleObject>(results.ToList());
        }

        /// <summary>
        /// Find all available roles.       
        /// </summary>
        /// <returns></returns>
        public Collection<RoleObject> FindAllXml()
        {
            return FindAllJson();
        }

        /// <summary>   
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Collection<RoleObject> FindByUserIdJson(string userId)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidUserID + "{0}", "User Id cannot be null"));
            try
            {
                IEnumerable<RoleObject> results = roleApi.FindByUserId(new Guid(userId));
                return new Collection<RoleObject>(results.ToList());
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
        /// Get all roles associated to the specified organization.        
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Collection<RoleObject> FindByUserIdXml(string userId)
        {
            return FindByUserIdJson(userId);
        }

        /// <summary>
        /// Get all roles associated to the specified organization.
        /// Uri Template: json/FindByOrganizationId/{organizationId}
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public Collection<RoleObject> FindByOrganizationIdJson(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
                throw new BadRequestException(Resources.InvalidOrganizationID);

            try
            {
                OrganizationObject org = organizationApi.GetOrganization(new Guid(organizationId));
                if (org == null)
                    throw new BadRequestException(Resources.InvalidOrganizationID);

                OrganizationTypeObject orgType = organizationApi.GetOrganizationType(org.OrganizationTypeId);
                IEnumerable<RoleObject> results = roleApi.FindByDomain(orgType.Domain);
                return new Collection<RoleObject>(results.ToList());
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
        /// Find all available roles.      
        /// </summary>
        /// <returns></returns>
        public Collection<RoleObject> FindByOrganizationIdXml(string organizationId)
        {
            return FindByOrganizationIdJson(organizationId);
        }

        /// <summary>
        /// Get all roles associated to the specified organization.     
        /// </summary>
        /// <param name="organizationTypeId"></param>
        /// <returns></returns>
        public Collection<RoleObject> FindByOrganizationTypeJson(string organizationTypeId)
        {
            if (string.IsNullOrEmpty(organizationTypeId))
                throw new BadRequestException(Resources.InvalidOrganizationTypeID);

            try
            {
                OrganizationTypeObject orgType = organizationApi.GetOrganizationType(new Guid(organizationTypeId));
                if (orgType == null)
                    throw new BadRequestException(Resources.InvalidOrganizationTypeID);

                IEnumerable<RoleObject> results = roleApi.FindByDomain(orgType.Domain);
                return new Collection<RoleObject>(results.ToList());
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
        /// Get all roles associated to the specified organization.      
        /// </summary>
        /// <param name="organizationTypeId"></param>
        /// <returns></returns>
        public Collection<RoleObject> FindByOrganizationTypeXml(string organizationTypeId)
        {
            return FindByOrganizationTypeJson(organizationTypeId);
        }

        /// <summary>
        /// Find roles by domain
        /// </summary>
        /// <param name="domain">domain</param>
        /// <returns></returns>
        public Collection<RoleObject> FindByDomainJson(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                throw new BadRequestException(Resources.InvalidOrganizationDomain);

            try
            {
                if (platformConfiguration.Domains.FirstOrDefault(d => string.Equals(d.Value, domain, StringComparison.OrdinalIgnoreCase)) == null)
                    throw new BadRequestException(Resources.InvalidOrganizationDomain);

                IEnumerable<RoleObject> results = roleApi.FindByDomain(domain);
                return new Collection<RoleObject>(results.ToList());
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
        /// Find roles by domain
        /// </summary>
        /// <param name="domain">domain</param>
        /// <returns></returns>
        public Collection<RoleObject> FindByDomainXml(string domain)
        {
            return this.FindByDomainJson(domain);
        }

        /// <summary>
        /// Gets true if specified user is in role.       
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsUserInRoleByIdJson(string userId, string roleId)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidUserID + "{0}", "User Id cannot be null"));
            if (string.IsNullOrEmpty(roleId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidRoleID + "{0}", "Role Id cannot be empty"));

            try
            {
                return roleApi.IsUserInRole(new Guid(userId), new Guid(roleId));
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
        /// Gets true if specified user is in role.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsUserInRoleByIdXml(string userId, string roleId)
        {
            return IsUserInRoleByIdJson(userId, roleId);
        }

        /// <summary>
        /// Gets true if specified user is in role.
        /// Uri Template: json/IsUserInRoleByName/{userId}/{roleName}
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool IsUserInRoleByNameJson(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidUserID + "{0}", "User Id cannot be null"));
            if (string.IsNullOrEmpty(roleName)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.RoleNameCannotBeEmpty + "{0}", "Role Name cannot be empty"));
            try
            {
                return roleApi.IsUserInRole(new Guid(userId), roleApi.Get(roleName).RoleId);
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
        /// Gets true if specified user is in role.    
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool IsUserInRoleByNameXml(string userId, string roleName)
        {
            return IsUserInRoleByNameJson(userId, roleName);
        }

        /// <summary>
        /// Query role objects by custom predicates.
        /// </summary>
        /// <param name="orderby">sorting expression</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="predicate">linq predicate. see role properties for predicate at <see cref="RapidWebDev.Platform.Linq.Role"/>.</param>
        /// <returns>Returns enumerable role objects</returns>
        public RoleQueryResult QueryRolesJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            
            try
            {
                int recordCount;
                pageSize = pageSize == 0 ? 25 : pageSize;
                IEnumerable<RoleObject> results = roleApi.FindRoles(ServicesHelper.ConvertWebPredicateToLinqPredicate(predicate), orderby, pageIndex, pageSize, out recordCount);
                if (results.Count() == 0)
                    return new RoleQueryResult() { PageIndex = pageIndex, PageSize = pageSize, TotalRecordCount = recordCount };

                RoleQueryResult result = new RoleQueryResult(results.ToList())
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecordCount = recordCount
                };

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
        /// Query role objects by custom predicates.
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public RoleQueryResult QueryRolesXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            return QueryRolesJson(orderby, pageIndex, pageSize, predicate);
        }

        #endregion
    }
}