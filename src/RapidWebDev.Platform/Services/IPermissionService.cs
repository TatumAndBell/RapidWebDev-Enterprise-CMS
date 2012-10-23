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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// external service for permission
    /// </summary>
    [ServiceContract(Name = "PermissionService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
    public interface IPermissionService
    {
        /// <summary>
        /// Set permissions on specified role. The new permissions will overwrite the existed permissions of the role.  
        /// UriTemplate = "json/SetRolePermission/{roleId}"
        /// </summary>      
        /// <param name="roleId"></param>
        /// <param name="permissions"></param>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/SetRolePermission/{roleId}")]
		[Permission]
        void SetRolePermissionsByJson(string roleId, IdCollection permissions);
    
		/// <summary>
        /// Set permissions on specified role. The new permissions will overwrite the existed permissions of the role.
        /// UriTemplate = "xml/SetRolePermission/{roleId}"
        /// </summary>        
        /// <param name="roleId"></param>
        /// <param name="permissions"></param>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/SetRolePermission/{roleId}")]
		[Permission]
        void SetRolePermissionsByXml(string roleId, IdCollection permissions);

        /// <summary>
        /// Set permissions on specified user. The new permissions will overwrite the existed permissions of the user.
        /// UriTemplate = "json/SetUserPermission/{userId}
        /// </summary>        
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        [OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/SetUserPermission/{userId}")]
		[Permission]
        void SetUserPermissionsByJson(string userId, IdCollection permissions);

		/// <summary>
        /// Set permissions on specified user. The new permissions will overwrite the existed permissions of the user.
        /// UriTemplate = "xml/SetUserPermission/{userId}
        /// </summary>        
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        [OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/SetUserPermission/{userId}")]
		[Permission]
        void SetUserPermissionsByXml(string userId, IdCollection permissions);

		
        /// <summary>
        /// Get permissions which the user has.
        /// UriTemplate = "json/FindUserPermissions/{userId}/{explicitOnly}")
        /// </summary>        
        /// <param name="userId">specified user</param>
        /// <param name="explicitOnly">true indicates that returns the permissions only directly set on the user</param>
        /// <returns>returns user permissions' values</returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindUserPermissions/{userId}/{explicitOnly}")]
		[Permission]
        Collection<string> FindUserPermissionsByJson(string userId, string explicitOnly);
  
		/// <summary>
        /// Get permissions which the user has.
        /// UriTemplate = "xml/FindUserPermissions/{userId}/{explicitOnly}")
        /// </summary>        
        /// <param name="userId">specified user</param>
        /// <param name="explicitOnly">true indicates that returns the permissions only directly set on the user</param>
        /// <returns>returns user permissions' values</returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindUserPermissions/{userId}/{explicitOnly}")]
		[Permission]
        Collection<string> FindUserPermissionsByXml(string userId, string explicitOnly);



        /// <summary>
        /// Get permissions which the role has. 
        /// UriTemplate = "json/FindUserPermissions/{roleId}
        /// </summary>       
        /// <param name="roleId">specified role</param>
        /// <returns>returns role permissions' values</returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindRolePermissions/{roleId}")]
		[Permission]
        Collection<string> FindRolePermissionsByJson(string roleId);

		
        /// <summary>
        /// Get permissions which the role has.
        /// UriTemplate = "xml/FindUserPermissions/{roleId}
        /// </summary>        
        /// <param name="roleId">specified role</param>
        /// <returns>returns role permissions' values</returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindRolePermissions/{roleId}")]
		[Permission]
        Collection<string> FindRolePermissionsByXml(string roleId);
		

        /// <summary>
        /// Returns true if input user has the permission.
        /// The permissions owned by the user includes the ones inherited from the roles of the user implicitly.
        /// UriTemplate = "json/HasPermission/{userId}/{permissionValue}"
        /// </summary>        
        /// <param name="userId">user id</param>
        /// <param name="permissionValue">permission value</param>
        /// <returns>Returns true if input user has the permission.</returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/HasPermission/{userId}/{permissionValue}")]
		[Permission]
        bool DoesTheUserHasPermissionJson(string userId, string permissionValue);

		/// <summary>
        /// Returns true if input user has the permission.
        /// The permissions owned by the user includes the ones inherited from the roles of the user implicitly.
        /// UriTemplate = "xml/HasPermission/{userId}/{permissionValue}"
        /// </summary>        
        /// <param name="userId">user id</param>
        /// <param name="permissionValue">permission value</param>
        /// <returns>Returns true if input user has the permission.</returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/HasPermission/{userId}/{permissionValue}")]
		[Permission]
        bool DoesTheUserHasPermissionXml(string userId, string permissionValue);

		
        /// <summary>
        /// Returns true if the current authenticated user has any permissions in specified permission.
        /// UriTemplate = "json/HasPermissionCurrentUser/{permissionValue}"
        /// </summary>
        /// <param name="permissionValue">permission value</param>
        /// <returns>Returns true if the current user has permission to access permission key.</returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/HasPermission/{permissionValue}")]
		[Permission]
        bool DoesTheCurrentUserHavePermissionJson(string permissionValue);

		/// <summary>
        /// Returns true if the current authenticated user has any permissions in specified permission.
        /// UriTemplate = "xml/HasPermissionCurrentUser/{permissionValue}"
        /// </summary>
        /// <param name="permissionValue">permission value</param>
        /// <returns>Returns true if the current user has permission to access permission key.</returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/HasPermission/{permissionValue}")]
		[Permission]
        bool DoesTheCurrentUserHavePermissionXml(string permissionValue);       
    }
}
