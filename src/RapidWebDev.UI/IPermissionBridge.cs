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

namespace RapidWebDev.UI
{
	/// <summary>
	/// Authentication bridge which will be used by web infrastructure for authority and permission checking. 
	/// </summary>
	public interface IPermissionBridge
	{
		/// <summary>
		/// Returns true if the current user has any permissions in specified permission.
		/// </summary>
		/// <param name="permissionValue">Permission value.</param>
		/// <returns>Returns true if the current user has any permissions in specified permission.</returns>
		bool HasPermission(string permissionValue);

		/// <summary>
		/// Resolve sitemap for current user depends on the authorized permission of the user.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ISiteMapItem> ResolveSiteMapItems();
	}
}