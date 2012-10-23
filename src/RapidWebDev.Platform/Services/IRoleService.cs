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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using RapidWebDev.Common;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
	/// <summary>
	/// The service to search roles
	/// </summary>
	[ServiceContract(Name = "RoleService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
	public interface IRoleService
	{
		/// <summary>
		/// Save role business object. 
		/// It does create/update based on roleObject.Id. If id is empty, the method will create a new role object.
		/// If the specified id is invalid, the method will throw an exception.
		/// Uri Template: json/Save
		/// </summary>
		/// <param name="roleObject">role object</param>
		/// <returns>returns id of the accessing role.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/Save")]
		[Permission]
		string SaveJson(RoleObject roleObject);

		/// <summary>
		/// Save role business object. 
		/// It does create/update based on roleObject.Id. If id is empty, the method will create a new role object.
		/// If the specified id is invalid, the method will throw an exception.
		/// Uri Template: xml/Save
		/// </summary>
		/// <param name="roleObject">role object</param>
		/// <returns>returns id of the accessing role.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/Save")]
		[Permission]
		string SaveXml(RoleObject roleObject);

		/// <summary>
		/// Delete role by id.
		/// Uri Template: json/HardDelete/{roleId}
		/// </summary>
		/// <param name="roleId"></param>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/HardDelete/{roleId}")]
		[Permission]
		void HardDeleteJson(string roleId);

		/// <summary>
		/// Delete role by id.
		/// Uri Template: xml/HardDelete/{roleId}
		/// </summary>
		/// <param name="roleId"></param>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/HardDelete/{roleId}")]
		[Permission]
		void HardDeleteXml(string roleId);

		/// <summary>
		/// Set user into roles which overwrites all existed roles of user.
		/// Uri Template: json/SetUserToRoles/{userId}
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleIds"></param>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/SetUserToRoles/{userId}")]
		[Permission]
		void SetUserToRolesJson(string userId, IdCollection roleIds);

		/// <summary>
		/// Set user into roles which overwrites all existed roles of user.
		/// Uri Template: xml/SetUserToRoles/{userId}
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleIds"></param>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/SetUserToRoles/{userId}")]
		[Permission]
		void SetUserToRolesXml(string userId, IdCollection roleIds);

		/// <summary>
		/// Get role by role name.
		/// Uri Template: json/GetByName/{roleName}
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetByName/{roleName}")]
		[Permission]
		RoleObject GetByNameJson(string roleName);

		/// <summary>
		/// Get role by role name.
		/// Uri Template: xml/GetByName/{roleName}
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetByName/{roleName}")]
		[Permission]
		RoleObject GetByNameXml(string roleName);

		/// <summary>
		/// Get role by role id.
		/// Uri Template: json/GetById/{roleId}
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetById/{roleId}")]
		[Permission]
		RoleObject GetByIdJson(string roleId);

		/// <summary>
		/// Get role by role id.
		/// Uri Template: xml/GetById/{roleId}
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetById/{roleId}")]
		[Permission]
		RoleObject GetByIdXml(string roleId);

		/// <summary>
		/// Bulkget role objects by a collection of role ids.
		/// Uri Template: json/BulkGet
		/// </summary>
		/// <param name="roleIds"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/BulkGet")]
		[Permission]
		Collection<RoleObject> BulkGetJson(IdCollection roleIds);

		/// <summary>
		/// Bulkget role objects by a collection of role ids.
		/// Uri Template: xml/BulkGet
		/// </summary>
		/// <param name="roleIds"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/BulkGet")]
		[Permission]
		Collection<RoleObject> BulkGetXml(IdCollection roleIds);

		/// <summary>
		/// Find all available roles.
		/// Uri Template: json/FindAll
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindAll")]
		[Permission]
		Collection<RoleObject> FindAllJson();

		/// <summary>
		/// Find all available roles.
		/// Uri Template: xml/FindAll
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindAll")]
		[Permission]
		Collection<RoleObject> FindAllXml();

		/// <summary>
		/// Get all roles associated to the specified organization.
		/// Uri Template: json/FindByOrganizationId/{organizationId}
		/// </summary>
		/// <param name="organizationId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindByOrganizationId/{organizationId}")]
		[Permission]
		Collection<RoleObject> FindByOrganizationIdJson(string organizationId);

		/// <summary>
		/// Get all roles associated to the specified organization.
		/// Uri Template: xml/FindByOrganizationId/{organizationId}
		/// </summary>
		/// <param name="organizationId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindByOrganizationId/{organizationId}")]
		[Permission]
		Collection<RoleObject> FindByOrganizationIdXml(string organizationId);

		/// <summary>
		/// Find all roles of specified user.
		/// Uri Template: json/FindByUserId/{userId}
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindByUserId/{userId}")]
		[Permission]
		Collection<RoleObject> FindByUserIdJson(string userId);

		/// <summary>
		/// Find all roles of specified user.
		/// Uri Template: xml/FindByUserId/{userId}
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindByUserId/{userId}")]
		[Permission]
		Collection<RoleObject> FindByUserIdXml(string userId);

		/// <summary>
		/// Find roles by organization type id.
		/// Uri Template: json/FindByOrganizationType/{organizationTypeId}
		/// </summary>
		/// <param name="organizationTypeId">organization type id</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindByOrganizationType/{organizationTypeId}")]
		[Permission]
		Collection<RoleObject> FindByOrganizationTypeJson(string organizationTypeId);

		/// <summary>
		/// Find roles by organization type id.
		/// Uri Template: xml/FindByOrganizationType/{organizationTypeId}
		/// </summary>
		/// <param name="organizationTypeId">organization type id</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindByOrganizationType/{organizationTypeId}")]
		[Permission]
		Collection<RoleObject> FindByOrganizationTypeXml(string organizationTypeId);

		/// <summary>
		/// Find roles by domain
		/// Uri Template: json/FindByDomain/{domain}
		/// </summary>
		/// <param name="domain">domain</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindByDomain/{domain}")]
		[Permission]
		Collection<RoleObject> FindByDomainJson(string domain);

		/// <summary>
		/// Find roles by domain
		/// Uri Template: xml/FindByDomain/{domain}
		/// </summary>
		/// <param name="domain">domain</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindByDomain/{domain}")]
		[Permission]
		Collection<RoleObject> FindByDomainXml(string domain);

		/// <summary>
		/// Gets true if specified user is in role.
		/// Uri Template: json/IsUserInRoleById/{userId}/{roleId}
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/IsUserInRoleById/{userId}/{roleId}")]
		[Permission]
		bool IsUserInRoleByIdJson(string userId, string roleId);

		/// <summary>
		/// Gets true if specified user is in role.
		/// Uri Template: xml/IsUserInRoleById/{userId}/{roleId}
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/IsUserInRoleById/{userId}/{roleId}")]
		[Permission]
		bool IsUserInRoleByIdXml(string userId, string roleId);

		/// <summary>
		/// Gets true if specified user is in role.
		/// Uri Template: json/IsUserInRoleByName/{userId}/{roleName}
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/IsUserInRoleByName/{userId}/{roleName}")]
		[Permission]
		bool IsUserInRoleByNameJson(string userId, string roleName);

		/// <summary>
		/// Gets true if specified user is in role.
		/// Uri Template: xml/IsUserInRoleByName/{userId}/{roleName}
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/IsUserInRoleByName/{userId}/{roleName}")]
		[Permission]
		bool IsUserInRoleByNameXml(string userId, string roleName);

		/// <summary>
		/// Find role objects by custom predicates.
        /// Uri Template: json/QueryRoles?pageindex={pageIndex}&amp;pagesize={pageSize}&amp;orderby={orderby}
		/// </summary>
		/// <param name="orderby">sorting expression</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="predicate">linq predicate. see role properties for predicate at <see cref="RapidWebDev.Platform.Linq.Role"/>.</param>
		/// <returns>Returns enumerable role objects</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/QueryRoles?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
		[Permission]
		RoleQueryResult QueryRolesJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);

		/// <summary>
		/// Find role objects by custom predicates.
        /// Uri Template: xml/QueryRoles?pageindex={pageIndex}&amp;pagesize={pageSize}&amp;orderby={orderby}
		/// </summary>
		/// <param name="orderby">sorting expression</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="predicate">linq predicate. see role properties for predicate at <see cref="RapidWebDev.Platform.Linq.Role"/>.</param>
		/// <returns>Returns enumerable role objects</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/QueryRoles?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
		[Permission]
		RoleQueryResult QueryRolesXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);
	}

	/// <summary>
	/// Role data query result
	/// </summary>
	[CollectionDataContract(ItemName = "RoleObject", Namespace = ServiceNamespaces.Platform)]
	public class RoleQueryResult : Collection<RoleObject>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="objects"></param>
		public RoleQueryResult(IList<RoleObject> objects)
			: base(objects)
		{
		}

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public RoleQueryResult()
		{
		}

		/// <summary>
		/// Page index which starts from 0.
		/// </summary>
		[DataMember]
		public int PageIndex { get; set; }

		/// <summary>
		/// Page size.
		/// </summary>
		[DataMember]
		public int PageSize { get; set; }

		/// <summary>
		/// Total record count.
		/// </summary>
		[DataMember]
		public int TotalRecordCount { get; set; }
	}
}