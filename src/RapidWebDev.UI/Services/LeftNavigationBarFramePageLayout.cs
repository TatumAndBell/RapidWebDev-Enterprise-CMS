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
	/// The implementation to render layout of the frame page as (header), (navigation bar) at left, (working area) at righ and (footer).
	/// </summary>
	public class LeftNavigationBarFramePageLayout : IFramePageLayout
	{
		private FramePageConfigurationSection configurationSection;
		private Panel headerPanel;
		private Panel footerPanel;
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

			PlaceHolder placeHolder = new PlaceHolder();

			if (headerPanel != null)
				placeHolder.Controls.Add(this.headerPanel);

			this.CreateNavigationPanels(placeHolder, siteMapItems);

			if (footerPanel != null)
				placeHolder.Controls.Add(this.footerPanel);

			return placeHolder;
		}

		/// <summary>
		/// The method will be invoked by <see cref="RapidWebDev.UI.Services.FramePageHandler"/> in the event "OnPreRender".
		/// </summary>
		public void OnPreRender()
		{
			ClientScripts.RegisterHeaderScriptInclude(Kit.ResolveAbsoluteUrl("~/resources/javascript/LeftNavigationBarFramePageLayout.js"));
			string registeredJs = "window.FramePageObj = new FramePageClass({0}, {1});";
			registeredJs = string.Format(CultureInfo.InvariantCulture, registeredJs, this.CreateRegionJsonString(), this.CreateOptionsJsonString());
			ClientScripts.RegisterStartupScript(registeredJs);
		}

		private void CreateNavigationPanels(PlaceHolder placeHolder, IEnumerable<ISiteMapItem> siteMapItems)
		{
			foreach (ISiteMapItem siteMapItem in siteMapItems)
			{
				Panel panelNavigationSegment = new Panel { ID = siteMapItem.Id, CssClass = "navigation" };
				placeHolder.Controls.Add(panelNavigationSegment);
				this.navigationPanels.Add(new KeyValuePair<Panel, ISiteMapItem>(panelNavigationSegment, siteMapItem));
				CreateNavigationSubPanels(panelNavigationSegment, siteMapItem.Children, 1);
			}
		}

		private static void CreateNavigationSubPanels(Panel navigationSegment, IEnumerable<ISiteMapItem> siteMapItems, int hierarchyLevel)
		{
			const string parentNavigationLiteralTemplate = "<div class='nav$HierarchyLevel$'><span class='parent'>$NavigationItemTitle$</span></div>";
			const string leafNavigationLiteralTemplate = @"<div class='nav$HierarchyLevel$'><a href=""javascript:$JavaScriptBlock$"">$NavigationItemTitle$</a></div>";
			const string separatorLiteralTemplate = @"<div class=""separator""></div>";

			if (siteMapItems == null) return;

			foreach (ISiteMapItem siteMapItem in siteMapItems)
			{
				string html = null;
				bool hasChildren = siteMapItem.Children != null && siteMapItem.Children.Count() > 0;
				if (siteMapItem.Type != SiteMapItemTypes.Separator && !hasChildren)
				{
					string js = null;
					if (string.IsNullOrEmpty(siteMapItem.ClientSideCommand))
						js = "window.FramePageObj.AddTab({ Id: '$NavigationItemId$', Url: '$NavigationItemUrl$', Title: '$NavigationItemTitle$' })"
							.Replace("$NavigationItemId$", siteMapItem.Id)
							.Replace("$NavigationItemUrl$", Kit.ResolveAbsoluteUrl(WebUtility.EncodeJavaScriptString(siteMapItem.PageUrl)))
							.Replace("$NavigationItemTitle$", WebUtility.EncodeJavaScriptString(siteMapItem.Text));
					else
						js = siteMapItem.ClientSideCommand;

					html = leafNavigationLiteralTemplate.Replace("$HierarchyLevel$", hierarchyLevel.ToString())
						.Replace("$JavaScriptBlock$", js)
						.Replace("$NavigationItemTitle$", WebUtility.EncodeJavaScriptString(siteMapItem.Text));
				}
				else if (siteMapItem.Type != SiteMapItemTypes.Separator && hasChildren)
				{
					html = parentNavigationLiteralTemplate.Replace("$HierarchyLevel$", hierarchyLevel.ToString())
						.Replace("$NavigationItemTitle$", WebUtility.EncodeJavaScriptString(siteMapItem.Text));
				}
				else if (siteMapItem.Type == SiteMapItemTypes.Separator)
					html = separatorLiteralTemplate;

				LiteralControl literalControl = new LiteralControl(html);
				navigationSegment.Controls.Add(literalControl);
				if (hasChildren)
					CreateNavigationSubPanels(navigationSegment, siteMapItem.Children, hierarchyLevel + 1);
			}
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

				jsonTextWriter.WritePropertyName("FooterPanelId");
				jsonTextWriter.WriteValue(this.footerPanel.ClientID);

				jsonTextWriter.WritePropertyName("NavigationBarTitle");
				jsonTextWriter.WriteValue(this.configurationSection.NavigationBarTitle);

				jsonTextWriter.WritePropertyName("NavigationItems");
				jsonTextWriter.WriteStartArray();
				foreach (KeyValuePair<Panel, ISiteMapItem> kvp in this.navigationPanels)
				{
					jsonTextWriter.WriteStartObject();
					jsonTextWriter.WritePropertyName("Id");
					jsonTextWriter.WriteValue(kvp.Key.ClientID);

					jsonTextWriter.WritePropertyName("Title");
					jsonTextWriter.WriteValue(kvp.Value.Text);

					jsonTextWriter.WritePropertyName("IconClass");
					jsonTextWriter.WriteValue(kvp.Value.IconClassName);

					jsonTextWriter.WriteEndObject();
				}

				jsonTextWriter.WriteEndArray();
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
	}
}