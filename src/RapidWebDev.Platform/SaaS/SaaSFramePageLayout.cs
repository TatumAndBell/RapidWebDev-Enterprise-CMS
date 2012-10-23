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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Services;

namespace RapidWebDev.Platform.SaaS
{
	/// <summary>
	/// The implementation to render layout of the frame page as (header), (navigation bar) at top, (working area) at center and (footer) at bottom.
	/// </summary>
	public class SaaSFramePageLayout : IFramePageLayout
	{
		private IFramePageLayout compositeFramePageLayout;
		private IAuthenticationContext authenticationContext;
		private IApplicationApi applicationApi;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authenticationContext"></param>
		/// <param name="applicationApi"></param>
		public SaaSFramePageLayout(IAuthenticationContext authenticationContext, IApplicationApi applicationApi)
		{
			this.authenticationContext = authenticationContext;
			this.applicationApi = applicationApi;

			ApplicationObject application = this.applicationApi.Get(authenticationContext.ApplicationId);
			string framePageLayout = application["FramePageLayout"] as string;
			if (string.Equals(framePageLayout, "LeftNavigation", StringComparison.OrdinalIgnoreCase))
				this.compositeFramePageLayout = new LeftNavigationBarFramePageLayout();
			else if (string.Equals(framePageLayout, "TopNavigation", StringComparison.OrdinalIgnoreCase))
				this.compositeFramePageLayout = new TopNavigationMenuFramePageLayout();
			else
				this.compositeFramePageLayout = new LeftNavigationBarFramePageLayout();
		}

		/// <summary>
		/// Create a control contains all elements in the frame page, e.g. header, navigation bar, working area and footer. 
		/// The method will be invoked by <see cref="RapidWebDev.UI.Services.FramePageHandler"/> in the event "OnInit".
		/// </summary>
		/// <param name="configurationSection">Frame page configuration section.</param>
		/// <param name="headerPanel">Page header panel.</param>
		/// <param name="footerPanel">Page footer panel.</param>
		/// <param name="siteMapItems">Sitemap items</param>
		/// <returns></returns>
		public Control CreateControl(FramePageConfigurationSection configurationSection, Panel headerPanel, Panel footerPanel, IEnumerable<RapidWebDev.UI.ISiteMapItem> siteMapItems)
		{
			return this.compositeFramePageLayout.CreateControl(configurationSection, headerPanel, footerPanel, siteMapItems);
		}

		/// <summary>
		/// The method will be invoked by <see cref="RapidWebDev.UI.Services.FramePageHandler"/> in the event "OnPreRender".
		/// </summary>
		public void OnPreRender()
		{
			this.compositeFramePageLayout.OnPreRender();
		}
	}
}