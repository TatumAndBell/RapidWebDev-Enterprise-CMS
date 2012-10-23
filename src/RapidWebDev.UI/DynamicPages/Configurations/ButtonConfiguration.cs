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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Class to configure button configuration.
	/// </summary>
	[Serializable]
	public class ButtonConfiguration
	{
		private static readonly HashSet<string> protectedCommandArguments = new HashSet<string> { "UPDATE", "DELETE", "VIEW" };
		private string text;
		private string tooltip;

		/// <summary>
		/// Specify command argument so that IDynamicPage instance can catch the event. <br />
		/// The integrated concrete command arguments are listed as following,<br />
		/// 1) "New" - to create a new entity. It popups a blank detail panel when the button with command argument equals to "New" been clicked.<br />
		/// 2) "Delete", "Update", "View" are protected which cannot be configured here. They have been used by grid view.<br />
		/// 3) "Print" - to popup a window to print the found records.<br />
		/// 4) "DownloadExcel" - to download an excel document file with all found records.<br />
		/// </summary>
		public string CommandArgument { get; set; }

		/// <summary>
		/// Sets/gets generic button display text.
		/// </summary>
		public string Text
		{
			set { this.text = value; }
			get { return WebUtility.ReplaceVariables(this.text); }
		}

		/// <summary>
		/// Sets/gets generic button tooltip.
		/// </summary>
		public string ToolTip
		{
			set { this.tooltip = value; }
			get { return WebUtility.ReplaceVariables(this.tooltip); }
		}

		/// <summary>
		/// Sets/gets generic button image URL.
		/// </summary>
		public string ImageUrl { get; set; }

		/// <summary>
		/// Sets/gets javascript code fires when client click on button.
		/// The request is sent to the server only when the configured JavaScript block returns true.
		/// </summary>
		public string OnClientClick { get; set; }

		/// <summary>
		/// Sets/gets class name applied to the button.
		/// </summary>
		public string Css { get; set; }

		/// <summary>
		/// Sets/gets button rendering type. The default value is to a common Button.
		/// </summary>
		public ButtonRenderTypes ButtonRenderType { get; set; }

		/// <summary>
		/// Validate whether the user has selected any data in grid before sending request to web services. (defaults to false)
		/// </summary>
		public bool GridSelectionRequired { get; set; }

		/// <summary>
		/// Warning message popup to user if there is no grid data selected when the user clicks the button.
		/// The attribute value supports globalization variable as "$Namespace.ClassName.PropertyName, AssemblyName$" and request context variables as "$VariableName$" (included in IApplicationContext.LabelVariables["VariableName"]).
		/// </summary>
		public string GridSelectionRequiredWarningMessage { get; set; }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public ButtonConfiguration()
		{
		}

		/// <summary>
		/// Construct ButtonConfiguration instance from xml element.
		/// </summary>
		/// <param name="buttonElement"></param>
		/// <param name="xmlParser"></param>
		public ButtonConfiguration(XmlElement buttonElement, XmlParser xmlParser)
		{
			this.Css = xmlParser.ParseString(buttonElement, "@Css");
			this.Text = xmlParser.ParseString(buttonElement, "@Text");
			this.ToolTip = xmlParser.ParseString(buttonElement, "@ToolTip");
			this.ImageUrl = xmlParser.ParseString(buttonElement, "@ImageUrl");
			this.CommandArgument = xmlParser.ParseString(buttonElement, "@CommandArgument");
			if(protectedCommandArguments.Contains(this.CommandArgument.ToUpperInvariant()))
				throw new ConfigurationErrorsException(@"'Delete', 'Update' and 'View' are prohibited command arguments in button configuration.");

			this.ButtonRenderType = xmlParser.ParseEnum<ButtonRenderTypes>(buttonElement, "@Type");

			this.OnClientClick = xmlParser.ParseString(buttonElement, "p:OnClientClick");

			XmlElement gridSelectionRequiredElement = buttonElement.SelectSingleNode("p:GridSelectionRequired", xmlParser.NamespaceManager) as XmlElement;
			if (gridSelectionRequiredElement != null)
			{
				this.GridSelectionRequired = true;
				this.GridSelectionRequiredWarningMessage = xmlParser.ParseString(gridSelectionRequiredElement, "@WarningMessage");
			}
		}
	}
}

