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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;
using RapidWebDev.UI.WebResources;
using Newtonsoft.Json;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// Frame page handler. 
	/// Frame page - when the user log onto system, the page is composed of navigation, page header, page footer and business/content page. We call this page as Frame page. It's the main entry of the system.
	/// </summary>
	public class FramePageHandler : SupportDocumentReadyPage, IRequiresSessionState
	{
		private const string NOCACHE_META = @"
				<meta http-equiv=""Expires"" content=""0"" ></meta>
				<meta http-equiv=""Cache-Control"" content=""no-cache""></meta>
				<meta http-equiv=""Pragma"" content=""no-cache""></meta>";

		private IFramePageLayout framePageLayout;
		private IApplicationContext applicationContext;
		private IPermissionBridge permissionBridge;
		private FramePageConfigurationSection configurationSection;
		private Panel panelHeader;
		private Panel panelFooter;

		/// <summary>
		/// Raises the System.Web.UI.Control.Init event to initialize the page.
		/// </summary>
		/// <param name="e">An System.EventArgs that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.framePageLayout = SpringContext.Current.GetObject<IFramePageLayout>();
			this.EnableViewState = false;

			this.Initialize();

			HtmlGenericControl htmlTag = new HtmlGenericControl("html");
			htmlTag.Attributes["xmlns"] = "http://www.w3.org/1999/xhtml";
			this.Controls.Add(htmlTag);

			this.CreateHtmlHead(htmlTag);
			this.CreateHtmlBody(htmlTag);
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			this.framePageLayout.OnPreRender();
		}

		/// <summary>
		/// Initializes the System.Web.UI.HtmlTextWriter object and calls on the child controls of the System.Web.UI.Page to render.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
			base.Render(writer);
		}

		/// <summary>
		/// The method is working on validating the permission and authentication of user to the page.
		/// </summary>
		private void Initialize()
		{
			this.configurationSection = ConfigurationManager.GetSection("framePage") as FramePageConfigurationSection;

			this.applicationContext = SpringContext.Current.GetObject<IApplicationContext>();
			this.permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();

			if (this.configurationSection == null)
				throw new InternalServerErrorException(@"The configuration section ""framePage"" is not found in application configuration file.");

			if (!this.permissionBridge.HasPermission(this.configurationSection.PermissionValue))
				throw new UnauthorizedException(string.Format(CultureInfo.InvariantCulture, @"The user doesn't have the permission to access the frame page with permission value ""{0}"".", this.configurationSection.PermissionValue));
		}

		private void CreateHtmlHead(Control htmlContainer)
		{
			htmlContainer.Controls.Add(new HtmlHead());
			this.Header.Controls.Add(new HtmlTitle { Text = this.configurationSection.FramePageTitle ?? Resources.FP_DefaultPageTitle });

			LiteralControl metaDefinition = new LiteralControl(NOCACHE_META);
			this.Header.Controls.Add(metaDefinition);

			// register style and script file references.
			IWebResourceManager resourceManager = SpringContext.Current.GetObject<IWebResourceManager>();
			resourceManager.Flush("FramePage");
		}

		private void CreateHtmlBody(Control htmlContainer)
		{
			HtmlGenericControl htmlBody = new HtmlGenericControl("body");
			htmlBody.Attributes["class"] = "framepagebody";
			htmlContainer.Controls.Add(htmlBody);

			HtmlForm htmlForm = new HtmlForm { ID = "FramePageForm" };
			htmlBody.Controls.Add(htmlForm);

			Panel pageContainer = new Panel { ID = "FramePageContainer", CssClass = "framepagecontainer" };
			htmlForm.Controls.Add(pageContainer);

			// initialize header
			this.panelHeader = new Panel { ID = "Header" };
			if (!string.IsNullOrEmpty(this.configurationSection.HeaderTemplate))
			{
				Control headerControl = this.LoadControl(this.configurationSection.HeaderTemplate);
				this.panelHeader.Controls.Add(headerControl);
			}

			// initialize footer
			this.panelFooter = new Panel { ID = "Footer" };
			if (!string.IsNullOrEmpty(this.configurationSection.FooterTemplate))
			{
				Control footerControl = this.LoadControl(this.configurationSection.FooterTemplate);
				this.panelFooter.Controls.Add(footerControl);
			}

			IEnumerable<ISiteMapItem> siteMapItems = this.permissionBridge.ResolveSiteMapItems();
			Control framePageContainer = this.framePageLayout.CreateControl(this.configurationSection, this.panelHeader, this.panelFooter, siteMapItems);
			pageContainer.Controls.Add(framePageContainer);
		}
	}
}