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
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Common;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Permission API interface.
	/// </summary>
	public interface IPermissionApi
	{
		/// <summary>
		/// Set permissions on specified role. The new permissions will overwrite the existed permissions of the role.
		/// </summary>
		/// <param name="roleId"></param>
		/// <param name="permissions"></param>
		void SetRolePermissions(Guid roleId, IEnumerable<string> permissions);

		/// <summary>
		/// Set permissions on specified user. The new permissions will overwrite the existed permissions of the user.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="permissions"></param>
		void SetUserPermissions(Guid userId, IEnumerable<string> permissions);

		/// <summary>
		/// Get permissions which the user has.
		/// </summary>
		/// <param name="userId">specified user</param>
		/// <param name="explicitOnly">true indicates that returns the permissions only directly set on the user</param>
		/// <returns>returns user permissions</returns>
		IEnumerable<PermissionObject> FindUserPermissions(Guid userId, bool explicitOnly);

		/// <summary>
		/// Get permissions which the role has.
		/// </summary>
		/// <param name="roleId">specified role</param>
		/// <returns>returns role permissions</returns>
		IEnumerable<PermissionObject> FindRolePermissions(Guid roleId);

		/// <summary>
		/// Returns true if input user has the permission.
		/// The permissions owned by the user includes the ones inherited from the roles of the user implicitly.
		/// </summary>
		/// <param name="userId">user id</param>
		/// <param name="permissionValue">permission value</param>
		/// <returns>Returns true if input user has the permission.</returns>
		bool HasPermission(Guid userId, string permissionValue);

		/// <summary>
		/// Returns true if the current authenticated user has any permissions in specified permission.
		/// </summary>
		/// <param name="permissionValue">permission value</param>
		/// <returns>Returns true if the current user has permission to access permission key.</returns>
		bool HasPermission(string permissionValue);

		/// <summary>
		/// Returns all permission configurations owned by specified user.
		/// </summary>
		/// <param name="operateUserId">operate user id</param>
		/// <returns>Returns all permission configurations owned by specified user.</returns>
		IEnumerable<PermissionConfig> FindPermissionConfig(Guid operateUserId);
	}
}
