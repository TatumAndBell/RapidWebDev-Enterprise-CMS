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

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The interface is to populate all sitemap configurations for users.
	/// </summary>
	public interface ISiteMapApi
	{
		/// <summary>
		/// Returns the sitemap items visible to the user.
		/// The algorithm is to filter out sitemap items from the sitemap configuration file (or somewhere else) which the user doesn't have the permission. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>Returns the sitemap items visible to the user.</returns>
		IEnumerable<SiteMapItemConfig> FindSiteMapConfig(Guid userId);
	}
}
