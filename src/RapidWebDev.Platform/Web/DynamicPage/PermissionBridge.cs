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
using System.Linq;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.Platform.Properties;
using RapidWebDev.UI;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Permission bridge to connect platform authority infrastructure and UI permission facade.
	/// </summary>
	public class PermissionBridge : IPermissionBridge
	{
		private IPermissionApi permissionApi;
		private ISiteMapApi siteMapApi;
		private IAuthenticationContext authenticationContext;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authenticationContext"></param>
		/// <param name="permissionApi"></param>
		/// <param name="siteMapApi"></param>
		public PermissionBridge(IAuthenticationContext authenticationContext, IPermissionApi permissionApi, ISiteMapApi siteMapApi)
		{
			this.authenticationContext = authenticationContext;
			this.permissionApi = permissionApi;
			this.siteMapApi = siteMapApi;
		}

		#region IPermissionBridge Members

		bool IPermissionBridge.HasPermission(string permissionValue)
		{
			return this.permissionApi.HasPermission(permissionValue);
		}

		IEnumerable<ISiteMapItem> IPermissionBridge.ResolveSiteMapItems()
		{
			if (!this.authenticationContext.Identity.IsAuthenticated)
				throw new UnauthorizedAccessException(Resources.InvalidAuthentication);

			return this.siteMapApi.FindSiteMapConfig(this.authenticationContext.User.UserId).Cast<ISiteMapItem>();
		}

		#endregion
	}
}