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
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// ConfigurableLinkbutton applied special style.
	/// </summary>
	public class Button : System.Web.UI.WebControls.LinkButton
	{
		/// <summary>
		/// Ext javascript block to format the rendered button to Ext style.
		/// </summary>
		private const string EXT_FORMATTER_TEMPLATE = @"
			if (window.$ControlVariableName$ != undefined && window.$ControlVariableName$ != null) 
				window.$ControlVariableName$.destroy();
			
			var linkbutton = Ext.DomQuery.select(""#$ControlID$"")[0];
			window.$ControlVariableName$ = new Ext.Button
			 ({
	 			id: linkbutton.id + '_extbutton',
	 			name: linkbutton.name,
	 			text: linkbutton.innerHTML,
	 			type: 'button',
	 			style: 'display:inline',
				tooltip: '$ToolTip$',
	 			listeners:
 				{
 					click: function(button, e)
 					{
						var senderSearcher = $('#$ControlID$');
						var sender = senderSearcher[0];
						var js = senderSearcher.attr('href');
						if (sender.onclick != null)
						{
 							if (sender.onclick())
 								eval(js);
						}
						else 
							eval(js);
 					}
 				}
			 });

			window.$ControlVariableName$.render(linkbutton.parentNode, linkbutton);
			linkbutton.style.display = 'none';";

		/// <summary>
		/// Sets/gets button configuration
		/// </summary>
		public ButtonConfiguration Configuration { get; set; }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public Button()
		{
		}

		/// <summary>
		/// Construct button from a configuration instance.
		/// </summary>
		/// <param name="configuration"></param>
		public Button(ButtonConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (this.Configuration == null)
			{
				this.Configuration = new ButtonConfiguration
				{
					ButtonRenderType = ButtonRenderTypes.Button,
					CommandArgument = this.CommandArgument,
					OnClientClick = this.OnClientClick,
					Text = this.Text
				};
			}

			this.CommandArgument = this.Configuration.CommandArgument ?? this.CommandArgument;
			this.OnClientClick = this.Configuration.OnClientClick ?? this.OnClientClick;

			switch (this.Configuration.ButtonRenderType)
			{
				case ButtonRenderTypes.Link:
					this.Text = this.Configuration.Text ?? this.Text;
					break;
				case ButtonRenderTypes.Button:
					this.Text = this.Configuration.Text ?? this.Text;
					string toolTip = WebUtility.EncodeJavaScriptString(this.ToolTip);
					ClientScripts.OnDocumentReady.Add2BeginOfBody(EXT_FORMATTER_TEMPLATE
						.Replace("$ControlID$", this.ClientID)
						.Replace("$ToolTip$", this.ToolTip)
						.Replace("$ControlVariableName$", WebUtility.GenerateVariableName(this.ClientID)), JavaScriptPriority.Low);
					break;
				default:
					this.Text = "";
					Image image = new Image();
					image.ImageUrl = GetImageUrl(this.Configuration.ButtonRenderType);
					if (Kit.IsEmpty(image.ImageUrl))
					{
						image.ImageUrl = this.Configuration.ImageUrl;
						this.Controls.Add(image);
					}
					break;
			}

			if (this.Configuration.ButtonRenderType == ButtonRenderTypes.DeleteImage && Kit.IsEmpty(this.OnClientClick))
				this.OnClientClick = string.Format("return window.confirm('{0}');", Resources.DPCtrl_DeleteButtonClientConfirmMessage);
		}

		/// <summary>
		/// Get image URL for specified button type. Returns null for Button, Link and CustomImage.
		/// </summary>
		/// <param name="buttonType"></param>
		/// <returns></returns>
		protected static string GetImageUrl(ButtonRenderTypes buttonType)
		{
			switch (buttonType)
			{
				case ButtonRenderTypes.Button:
				case ButtonRenderTypes.Link:
				case ButtonRenderTypes.CustomImage:
					return null;
				default:
					return string.Format("{0}/Resources/Images/{1}.gif", Kit.WebSiteBaseUrl, buttonType.ToString().Replace("Image", ""));
			}
		}
	}
}

