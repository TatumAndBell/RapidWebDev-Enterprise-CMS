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
using Newtonsoft.Json;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// The implementation to render layout of the frame page as (header), (navigation bar) at top, (working area) at center and (footer) at bottom.
	/// </summary>
	public class TopNavigationMenuFramePageLayout : IFramePageLayout
	{
		private FramePageConfigurationSection configurationSection;
		private Panel headerPanel;
		private Panel menuPanel;
		private Panel footerPanel;
		private IEnumerable<ISiteMapItem> siteMapItems;
		private List<KeyValuePair<Panel, ISiteMapItem>> navigationPanels = new List<KeyValuePair<Panel, ISiteMapItem>>();

		/// <summary>
		/// Create a control contains all elements in the frame page, e.g. header, navigation bar, working area and footer. 
		/// The method will be invoked by <see cref="RapidWebDev.UI.Services.FramePageHandler"/> in the event "OnInit".
		/// </summary>
		/// <param name="configurationSection">Frame page configuration section.</param>
		/// <param name="headerPanel">Page header panel.</param>
		/// <param name="footerPanel">Page footer panel.</param>
		/// <param name="siteMapItems">Sitemap items</param>
		/// <returns></returns>
		public Control CreateControl(FramePageConfigurationSection configurationSection, Panel headerPanel, Panel footerPanel, IEnumerable<ISiteMapItem> siteMapItems)
		{
			this.configurationSection = configurationSection;
			this.headerPanel = headerPanel;
			this.footerPanel = footerPanel;
			this.siteMapItems = siteMapItems;

			PlaceHolder placeHolder = new PlaceHolder();

			if (headerPanel != null)
				placeHolder.Controls.Add(this.headerPanel);

			this.menuPanel = new Panel { ID = "TopNavigationMenuFramePageLayout", CssClass = "TopNavigationMenuFramePageLayout" };
			placeHolder.Controls.Add(this.menuPanel);

			if (footerPanel != null)
				placeHolder.Controls.Add(this.footerPanel);

			return placeHolder;
		}

		/// <summary>
		/// The method will be invoked by <see cref="RapidWebDev.UI.Services.FramePageHandler"/> in the event "OnPreRender".
		/// </summary>
		public void OnPreRender()
		{
			ClientScripts.RegisterHeaderScriptInclude(Kit.ResolveAbsoluteUrl("~/resources/javascript/TopNavigationMenuFramePageLayout.js"));
			string registeredJs = "window.FramePageObj = new FramePageClass({0}, {1});";
			registeredJs = string.Format(CultureInfo.InvariantCulture, registeredJs, this.CreateRegionJsonString(), this.CreateOptionsJsonString());
			ClientScripts.RegisterStartupScript(registeredJs);
		}

		private string CreateRegionJsonString()
		{
			StringBuilder regionJsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(regionJsonBuilder))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartObject();

				jsonTextWriter.WritePropertyName("HeaderPanelId");
				jsonTextWriter.WriteValue(this.headerPanel.ClientID);

				jsonTextWriter.WritePropertyName("MenuPanelId");
				jsonTextWriter.WriteValue(this.menuPanel.ClientID);

				jsonTextWriter.WritePropertyName("FooterPanelId");
				jsonTextWriter.WriteValue(this.footerPanel.ClientID);

				jsonTextWriter.WritePropertyName("NavigationBarTitle");
				jsonTextWriter.WriteValue(this.configurationSection.NavigationBarTitle);

				jsonTextWriter.WritePropertyName("NavigationItems");
				jsonTextWriter.WriteRawValue(CreateMenuJsonString(this.siteMapItems));

				jsonTextWriter.WriteEndObject();
			}

			return regionJsonBuilder.ToString();
		}

		private string CreateOptionsJsonString()
		{
			//configurationSection
			StringBuilder regionJsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(regionJsonBuilder))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartObject();

				jsonTextWriter.WritePropertyName("EnableMultipleTabs");
				jsonTextWriter.WriteValue(this.configurationSection.EnableMultipleTabs);

				jsonTextWriter.WritePropertyName("MaximumTabs");
				jsonTextWriter.WriteValue(this.configurationSection.MaximumTabs);

				jsonTextWriter.WritePropertyName("DefaultTab");
				jsonTextWriter.WriteStartObject();
				jsonTextWriter.WritePropertyName("Url");
				jsonTextWriter.WriteValue(this.configurationSection.DefaultPageUrl);
				jsonTextWriter.WritePropertyName("Title");
				jsonTextWriter.WriteValue(this.configurationSection.DefaultTabTitle);
				jsonTextWriter.WritePropertyName("Id");
				jsonTextWriter.WriteValue("DefaultTabAutoAssignedId");
				jsonTextWriter.WriteEndObject();

				jsonTextWriter.WriteEndObject();
			}

			return regionJsonBuilder.ToString();
		}

		private static string CreateMenuJsonString(IEnumerable<ISiteMapItem> siteMapItems)
		{
			StringBuilder jsonOutput = new StringBuilder();
			using(StringWriter stringWriter = new StringWriter(jsonOutput))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				WriteMenuJsonObject(jsonWriter, siteMapItems);
			}

			return jsonOutput.ToString();
		}

		private static void WriteMenuJsonObject(JsonTextWriter jsonWriter, IEnumerable<ISiteMapItem> siteMapItems)
		{
			jsonWriter.WriteStartArray();
			foreach (ISiteMapItem siteMapItem in siteMapItems)
			{
				if (siteMapItem.Type != SiteMapItemTypes.Separator)
					WriteMenuItemJsonObject(jsonWriter, siteMapItem);
				else // write a dash-line as separator.
					jsonWriter.WriteValue("-");
			}

			jsonWriter.WriteEndArray();
		}

		private static void WriteMenuItemJsonObject(JsonTextWriter jsonWriter, ISiteMapItem siteMapItem)
		{
			jsonWriter.WriteStartObject();

			// id
			jsonWriter.WritePropertyName("id");
			jsonWriter.WriteValue(siteMapItem.Id);

			// text
			jsonWriter.WritePropertyName("text");
			jsonWriter.WriteValue(siteMapItem.Text);

			// iconCls
			jsonWriter.WritePropertyName("iconCls");
			jsonWriter.WriteValue(siteMapItem.IconClassName);

			bool hasChildren = siteMapItem.Children != null && siteMapItem.Children.Count() > 0;
			if (siteMapItem.Type != SiteMapItemTypes.Separator && !hasChildren)
			{
				if (!string.IsNullOrEmpty(siteMapItem.ClientSideCommand))
				{
					jsonWriter.WritePropertyName("clientSideCommand");
					jsonWriter.WriteValue(WebUtility.EncodeJavaScriptString(siteMapItem.ClientSideCommand));
				}

				if (!string.IsNullOrEmpty(siteMapItem.PageUrl))
				{
					jsonWriter.WritePropertyName("url");
					jsonWriter.WriteValue(siteMapItem.PageUrl);
				}
			}
			else if (siteMapItem.Type != SiteMapItemTypes.Separator && hasChildren)
			{
				// menu
				jsonWriter.WritePropertyName("menu");
				WriteMenuJsonObject(jsonWriter, siteMapItem.Children);
			}

			jsonWriter.WriteEndObject();
		}
	}
}