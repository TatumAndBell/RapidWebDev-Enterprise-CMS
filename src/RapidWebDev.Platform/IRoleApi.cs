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
using System.Text;
using System.Web.Security;
using System.Security.Principal;
using System.Linq.Expressions;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The interface of role accessing
	/// </summary>
	public interface IRoleApi
	{
		/// <summary>
		/// Save role business object. 
		/// It does create/update based on roleObject.Id. If id is empty, the method will create a new role object.
		/// If the specified id is invalid, the method will throw an exception.
		/// </summary>
		/// <param name="roleObject">role object</param>
		void Save(RoleObject roleObject);

		/// <summary>
		/// Delete role
		/// </summary>
		/// <param name="roleId"></param>
		void HardDelete(Guid roleId);

		/// <summary>
		/// Set user into roles which overwrites all existed roles of user.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleIds"></param>
		void SetUserToRoles(Guid userId, IEnumerable<Guid> roleIds);

		/// <summary>
		/// Get role id by role name.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		RoleObject Get(string roleName);

		/// <summary>
		/// Get role id by role id.
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		RoleObject Get(Guid roleId);

		/// <summary>
		/// Bulkget role objects by a collection of role ids.
		/// </summary>
		/// <param name="roleIds"></param>
		/// <returns></returns>
		IDictionary<Guid, RoleObject> BulkGet(IEnumerable<Guid> roleIds);

		/// <summary>
		/// Find all available roles.
		/// </summary>
		/// <returns></returns>
		IEnumerable<RoleObject> FindAll();

		/// <summary>
		/// Find roles by domain. returns all if specified argument is null.
		/// </summary>
		/// <param name="domain">organization domain</param>
		/// <returns></returns>
		IEnumerable<RoleObject> FindByDomain(string domain);

		/// <summary>
		/// Find all roles of specified user.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		IEnumerable<RoleObject> FindByUserId(Guid userId);

		/// <summary>
		/// Gets true if specified user is in role.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		bool IsUserInRole(Guid userId, Guid roleId);

		/// <summary>
		/// Gets true if specified user is in role.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		bool IsUserInRole(Guid userId, string roleName);

		/// <summary>
		/// Find role objects by custom predicates.
		/// </summary>
		/// <param name="predicate">linq predicate. see role properties for predicate at <see cref="RapidWebDev.Platform.Linq.Role"/>.</param>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns>Returns enumerable role objects</returns>
		IEnumerable<RoleObject> FindRoles(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount);
	}
}

