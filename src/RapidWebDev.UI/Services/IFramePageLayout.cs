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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// The interface to render layout of the frame page.
	/// </summary>
	public interface IFramePageLayout
	{
		/// <summary>
		/// Create a control contains all elements in the frame page, e.g. header, navigation bar, working area and footer. 
		/// The method will be invoked by <see cref="RapidWebDev.UI.Services.FramePageHandler"/> in the event "OnInit".
		/// </summary>
		/// <param name="configurationSection">Frame page configuration section.</param>
		/// <param name="headerPanel">Page header panel.</param>
		/// <param name="footerPanel">Page footer panel.</param>
		/// <param name="siteMapItems">Sitemap items</param>
		/// <returns></returns>
		Control CreateControl(FramePageConfigurationSection configurationSection, Panel headerPanel, Panel footerPanel, IEnumerable<ISiteMapItem> siteMapItems);

		/// <summary>
		/// The method will be invoked by <see cref="RapidWebDev.UI.Services.FramePageHandler"/> in the event "OnPreRender".
		/// </summary>
		void OnPreRender();
	}
}