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
using System.Configuration;
using System.Linq;
using System.Text;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Frame page configuration. 
	/// Frame page - when the user log onto system, the page is composed of navigation, page header, page footer and content page. We call this page as Frame page. It's the main entry of the system.
	/// </summary>
	public class FramePageConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// Page title of frame page.
		/// </summary>
		[ConfigurationProperty("FramePageTitle")]
		public string FramePageTitle
		{
			get { return WebUtility.ReplaceVariables(this["FramePageTitle"] as string); }
			set { this["FramePageTitle"] = value; }
		}

		/// <summary>
		/// Navigation bar title
		/// </summary>
		[ConfigurationProperty("NavigationBarTitle")]
		public string NavigationBarTitle
		{
			get { return WebUtility.ReplaceVariables(this["NavigationBarTitle"] as string); }
			set { this["NavigationBarTitle"] = value; }
		}

		/// <summary>
		/// Absolute/relative path of header template in frame page.
		/// </summary>
		[ConfigurationProperty("HeaderTemplate")]
		public string HeaderTemplate
		{
			get { return this["HeaderTemplate"] as string; }
			set { this["HeaderTemplate"] = value; }
		}

		/// <summary>
		/// Absolute/relative path of footer template in frame page.
		/// </summary>
		[ConfigurationProperty("FooterTemplate")]
		public string FooterTemplate
		{
			get { return this["FooterTemplate"] as string; }
			set { this["FooterTemplate"] = value; }
		}

		/// <summary>
		/// Default page url in frame page. The default page cannot be closed.
		/// </summary>
		[ConfigurationProperty("DefaultPageUrl", IsRequired = true)]
		public string DefaultPageUrl
		{
			get { return this["DefaultPageUrl"] as string; }
			set { this["DefaultPageUrl"] = value; }
		}

		/// <summary>
		/// Title of default tab when the frame page loaded.
		/// </summary>
		[ConfigurationProperty("DefaultTabTitle", IsRequired = true)]
		public string DefaultTabTitle
		{
			get { return WebUtility.ReplaceVariables(this["DefaultTabTitle"] as string); }
			set { this["DefaultTabTitle"] = value; }
		}

		/// <summary>
		/// True to enable multiple tabs for business/content pages in frame page.
		/// The multiple tabs enabled may improve user experience but will increase the web browser memory occupation obviouly.
		/// </summary>
		[ConfigurationProperty("EnableMultipleTabs", DefaultValue = false)]
		public bool EnableMultipleTabs
		{
			get { return (bool)this["EnableMultipleTabs"]; }
			set { this["EnableMultipleTabs"] = value; }
		}

		/// <summary>
		/// Maximum tabs of content page in the frame page, defaults to 5.
		/// </summary>
		[ConfigurationProperty("MaximumTabs", DefaultValue = 5)]
		public int MaximumTabs
		{
			get { return (int)this["MaximumTabs"]; }
			set { this["MaximumTabs"] = value; }
		}

		/// <summary>
		/// True to enable multiple tabs for business/content pages in frame page.
		/// The multiple tabs enabled may improve user experience but will increase the web browser memory occupation obviouly.
		/// </summary>
		[ConfigurationProperty("PermissionValue", DefaultValue = "EveryOne")]
		public string PermissionValue
		{
			get { return (string)this["PermissionValue"]; }
			set { this["PermissionValue"] = value; }
		}
	}
}
