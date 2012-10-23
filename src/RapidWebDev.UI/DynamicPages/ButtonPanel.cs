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

namespace RapidWebDev.UI.DynamicPages
{
	using System;
	using System.Configuration;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using RapidWebDev.Common;
	using RapidWebDev.Common.Web;
	using RapidWebDev.UI.Controls;
	using RapidWebDev.UI.DynamicPages.Configurations;
	using RapidWebDev.UI.Properties;
	using Newtonsoft.Json;

	/// <summary>
	/// Button panel groups a collection of buttons which do operations against multiple selected gridview records.
	/// </summary>
	public class ButtonPanel : Panel, INamingContainer
	{
		private static readonly IPermissionBridge permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();

		/// <summary>
		/// Configuration indicates how to render the panel.
		/// </summary>
		public ButtonPanelConfiguration Configuration { get; set; }

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			if (this.Configuration == null)
				throw new ConfigurationErrorsException(Resources.Ctrl_PropertyConfigurationNotSpecified);

			base.CreateChildControls();
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			string variableName = WebUtility.GenerateVariableName(this.ClientID);
			string clientScript = string.Format(CultureInfo.InvariantCulture, "window.{0} = new ButtonPanel({1});", variableName, this.CreateButtonPanelConfigJson());
			ClientScripts.RegisterScriptBlock(clientScript);
		}

		private string CreateButtonPanelConfigJson()
		{
			IDynamicPage dynamicPageService = DynamicPageContext.Current.GetDynamicPage(QueryStringUtility.ObjectId);
			DynamicPageConfiguration dynamicPageConfiguration = dynamicPageService.Configuration;

			StringBuilder jsonOutput = new StringBuilder();
			using(StringWriter stringWriter = new StringWriter(jsonOutput))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartObject();

				if (this.Configuration.ButtonAlignment != HorizontalAlign.NotSet && this.Configuration.ButtonAlignment != HorizontalAlign.Justify)
				{
					jsonTextWriter.WritePropertyName("Align");
					jsonTextWriter.WriteValue(this.Configuration.ButtonAlignment.ToString());
				}

				jsonTextWriter.WritePropertyName("ClientID");
				jsonTextWriter.WriteValue(this.ClientID);

				jsonTextWriter.WritePropertyName("VariableName");
				jsonTextWriter.WriteValue(WebUtility.GenerateVariableName(this.ClientID));

				jsonTextWriter.WritePropertyName("Buttons");
				jsonTextWriter.WriteStartArray();
				foreach (ButtonConfiguration buttonConfiguration in this.Configuration.Buttons)
				{
					string permissionValue = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", dynamicPageConfiguration.PermissionValue, buttonConfiguration.CommandArgument);
					if (permissionBridge.HasPermission(permissionValue))
						this.CreateButtonConfigJson(jsonTextWriter, buttonConfiguration);
				}

				jsonTextWriter.WriteEndArray();

				jsonTextWriter.WriteEndObject();
			}

			return jsonOutput.ToString();
		}

		/// <summary>
		/// Create JSON string for JavaScript Button.
		/// </summary>
		/// <param name="jsonTextWriter"></param>
		/// <param name="buttonConfiguration"></param>
		/// <returns></returns>
		private void CreateButtonConfigJson(JsonTextWriter jsonTextWriter, ButtonConfiguration buttonConfiguration)
		{
			jsonTextWriter.WriteStartObject();

			jsonTextWriter.WritePropertyName("DisplayAsImage");
			jsonTextWriter.WriteValue(buttonConfiguration.ButtonRenderType != ButtonRenderTypes.Button && buttonConfiguration.ButtonRenderType != ButtonRenderTypes.Link);

			jsonTextWriter.WritePropertyName("ImageUrl");
			jsonTextWriter.WriteValue(ResolveImageUrl(buttonConfiguration.ButtonRenderType, Kit.ResolveAbsoluteUrl(buttonConfiguration.ImageUrl)));

			jsonTextWriter.WritePropertyName("Text");
			jsonTextWriter.WriteValue(buttonConfiguration.Text);

			jsonTextWriter.WritePropertyName("ToolTip");
			jsonTextWriter.WriteValue(buttonConfiguration.ToolTip);

			jsonTextWriter.WritePropertyName("Css");
			jsonTextWriter.WriteValue(buttonConfiguration.Css);

			jsonTextWriter.WritePropertyName("OnClientClick");
			jsonTextWriter.WriteValue(WebUtility.EncodeJavaScriptString(buttonConfiguration.OnClientClick));

			jsonTextWriter.WritePropertyName("CommandArgument");
			jsonTextWriter.WriteValue(buttonConfiguration.CommandArgument);

			jsonTextWriter.WritePropertyName("GridSelectionRequired");
			jsonTextWriter.WriteValue(buttonConfiguration.GridSelectionRequired.ToString().ToLowerInvariant());

			jsonTextWriter.WritePropertyName("GridSelectionRequiredWarningMessage");
			jsonTextWriter.WriteValue(buttonConfiguration.GridSelectionRequiredWarningMessage ?? "");

			jsonTextWriter.WriteEndObject();
		}

		/// <summary>
		/// Resolve image URL for specified button type. Returns null for Button, Link and CustomImage.
		/// </summary>
		/// <param name="buttonType"></param>
		/// <param name="specifiedImageUrl"></param>
		/// <returns></returns>
		private static string ResolveImageUrl(ButtonRenderTypes buttonType, string specifiedImageUrl)
		{
			switch (buttonType)
			{
				case ButtonRenderTypes.Button:
				case ButtonRenderTypes.Link:
					return null;
				case ButtonRenderTypes.CustomImage:
					return Kit.ResolveAbsoluteUrl(specifiedImageUrl);
				default:
					return Kit.ResolveAbsoluteUrl(string.Format(CultureInfo.InvariantCulture, "~/resources/images/{0}.gif", buttonType.ToString().Replace("Image", "")));
			}
		}
	}
}

