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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common.Web;
using Newtonsoft.Json;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// Tab container based on ExtJs which integrates the existed html markup into TAB view.
	/// </summary>
	[ParseChildren(true, "TabPanelCollection")]
	public class TabContainer : WebControl
	{
		/// <summary>
		/// Gets/sets collection of tab panels which need to be integrated together into TAB view.
		/// </summary>
		public Collection<TabPanel> TabPanelCollection { get; set; }

		/// <summary>
		/// Render ExtJs to integrate configured panels to TAB view.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (this.TabPanelCollection != null && this.TabPanelCollection.Count > 0)
			{
				string json = this.BuildTabPanelJson();
				string javaScriptBlock = string.Format(CultureInfo.InvariantCulture, "if (window.{0}) window.{0}.destroy(); window.{0} = new Ext.TabPanel({1});", WebUtility.GenerateVariableName(this.ClientID), json);
				ClientScripts.OnDocumentReady.Add2BeginOfBody(javaScriptBlock, JavaScriptPriority.High);
			}
		}

		private string BuildTabPanelJson()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using(StringWriter writer = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
			{
				jsonWriter.WriteStartObject();

				// renderTo
				jsonWriter.WritePropertyName("renderTo");
				jsonWriter.WriteValue(this.ClientID);

				// frame
				jsonWriter.WritePropertyName("frame");
				jsonWriter.WriteValue(true);

				// activeTab
				jsonWriter.WritePropertyName("activeTab");
				jsonWriter.WriteValue(0);

				// width
				if (this.Width != Unit.Empty && this.Width.Type == UnitType.Pixel)
				{
					jsonWriter.WritePropertyName("width");
					jsonWriter.WriteValue((int)this.Width.Value);
				}

				// height
				if (this.Height != Unit.Empty && this.Height.Type == UnitType.Pixel)
				{
					jsonWriter.WritePropertyName("height");
					jsonWriter.WriteValue((int)this.Height.Value);
				}
				else
				{
					jsonWriter.WritePropertyName("defaults");
					jsonWriter.WriteStartObject();
					jsonWriter.WritePropertyName("autoHeight");
					jsonWriter.WriteValue(true);
					jsonWriter.WriteEndObject();
				}

				// tab panel items
				jsonWriter.WritePropertyName("items");
				jsonWriter.WriteStartArray();
				foreach (TabPanel tabPanel in this.TabPanelCollection)
				{
					jsonWriter.WriteStartObject();
					jsonWriter.WritePropertyName("contentEl");

					// try to find tab panel in the parent control and page.
					Control contentControl = this.Parent.FindControl(tabPanel.PanelId);
					if(contentControl == null)
						contentControl = this.Page.FindControl(tabPanel.PanelId);
					if (contentControl == null)
						contentControl = WebUtility.FindControl(this.Parent, tabPanel.PanelId);
					if (contentControl == null)
						throw new InvalidProgramException(string.Format(CultureInfo.InvariantCulture, "The programmed tab panel with id \"{0}\" in the control TabContainer is not found.", tabPanel.PanelId));
					jsonWriter.WriteValue(contentControl.ClientID);

					jsonWriter.WritePropertyName("title");
					jsonWriter.WriteValue(tabPanel.TabText);
					jsonWriter.WriteEndObject();
				}

				jsonWriter.WriteEndArray();

				jsonWriter.WriteEndObject();
			}

			return jsonBuilder.ToString();
		}
	}

	/// <summary>
	/// Tab panel indicates which panel needs to be integrated into tab container.
	/// </summary>
	public class TabPanel
	{
		/// <summary>
		/// Id of server control
		/// </summary>
		public string PanelId { get; set; }

		/// <summary>
		/// Tab text
		/// </summary>
		public string TabText { get; set; }
	}
}

