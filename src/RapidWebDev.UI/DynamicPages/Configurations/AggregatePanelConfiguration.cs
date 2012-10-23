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
using RapidWebDev.UI.Properties;
using RapidWebDev.Common;
using System.Reflection;
using System.Globalization;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// The configuration is used to build aggregate panel for custom operations for multiple selected entities in grid of dynamic page.
	/// </summary>
	public class AggregatePanelConfiguration : BasePanelConfiguration
	{
		private string typeName;

		/// <summary>
		/// The C# managed type of dynamic page which implements the interface IAggregatePanelPage.
		/// </summary>
		public Type @Type
		{
			get { return Kit.GetType(this.typeName); }
		}

		/// <summary>
		/// The command argument of the custom operation, defaults to string.Empty.
		/// </summary>
		public string CommandArgument { get; set; }

		/// <summary>
		/// Specify an ASCX template to render panel UI. Here supports both relative(~/) and abosolute path.
		/// </summary>
		public string SkinPath { get; set; }

		/// <summary>
		/// The width of aggregate panel (defaults to 960).
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// The height of aggregate panel (defaults to 600).
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// True indicates the aggregate panel is resizable (defaults to false).
		/// </summary>
		public bool Resizable { get; set; }

		/// <summary>
		/// True indicates the aggregate panel is draggable (defaults to false).
		/// </summary>
		public bool Draggable { get; set; }

		/// <summary>
		/// True indicates show message box in UI after saved successfully (defaults to true).
		/// </summary>
		public bool ShowMessageAfterSavedSuccessfully { get; set; }

		/// <summary>
		/// True to set focus on first input control automatically when the aggregate panel is loaded, defaults to true.
		/// </summary>
		public bool SetFocusOnFirstInputControlAutomatically { get; set; }

		/// <summary>
		/// Sets/gets FormPanelButtonConfiguration to save button of panel. The default value is null which means not to display save button.
		/// </summary>
		public FormPanelButtonConfiguration SaveButton { get; set; }

		/// <summary>
		/// Sets/gets FormPanelButtonConfiguration to cancel button of panel. The default value is null which means not to display cancel button.
		/// </summary>
		public FormPanelButtonConfiguration CancelButton { get; set; }

		/// <summary>
		/// Gets panel type - form panel. Form panel will be rendered as a form to create/edit/view records or any operations for the records.
		/// </summary>
		public override DynamicPagePanelTypes PanelType { get { return DynamicPagePanelTypes.AggregatePanel; } }

		/// <summary>
		/// Construct AggregatePanelConfiguration instance.
		/// </summary>
		/// <param name="typeName">The C# managed type of dynamic page which implements the interface IDynamicComponent (IDetailPanelPage/IAggregatePanelPage).</param>
		public AggregatePanelConfiguration(string typeName) : base() 
		{
			Kit.NotNull(typeName, "typeName");

			this.Width = 960;
			this.Height = 600;
			this.typeName = typeName;
			this.ShowMessageAfterSavedSuccessfully = true;
			this.SetFocusOnFirstInputControlAutomatically = true;
			this.CommandArgument = string.Empty;
			ValidateTypeName("", typeName);
		}

		/// <summary>
		/// Construct AggregatePanelConfiguration instance from xml element.
		/// </summary>
		/// <param name="panelElement"></param>
		/// <param name="xmlParser"></param>
		public AggregatePanelConfiguration(XmlElement panelElement, XmlParser xmlParser) : base(panelElement, xmlParser)
		{
			this.CommandArgument = xmlParser.ParseString(panelElement, "@CommandArgument") ?? string.Empty;
			this.typeName = xmlParser.ParseString(panelElement, "p:Type");
			string objectId = panelElement.OwnerDocument.SelectSingleNode("p:Page/@ObjectId", xmlParser.NamespaceManager).Value;
			ValidateTypeName(this.CommandArgument, this.typeName);

			this.SkinPath = xmlParser.ParseString(panelElement, "p:SkinPath");
			this.Width = xmlParser.ParseInt(panelElement, "@Width", 960);
			this.Height = xmlParser.ParseInt(panelElement, "@Height", 600);
			this.Resizable = xmlParser.ParseBoolean(panelElement, "@Resizable", false);
			this.Draggable = xmlParser.ParseBoolean(panelElement, "@Draggable", false);
			this.ShowMessageAfterSavedSuccessfully = xmlParser.ParseBoolean(panelElement, "@ShowMessageAfterSavedSuccessfully", true);
			this.SetFocusOnFirstInputControlAutomatically = xmlParser.ParseBoolean(panelElement, "@SetFocusOnFirstInputControlAutomatically", true);

			short defaultFormButtons = 0;
			XmlElement saveButtonElement = panelElement.SelectSingleNode("p:SaveButton", xmlParser.NamespaceManager) as XmlElement;
			if (saveButtonElement != null)
			{
				this.SaveButton = new FormPanelButtonConfiguration(saveButtonElement, xmlParser);
				this.SaveButton.Text = this.SaveButton.Text ?? Resources.DPCtrl_SaveText;
				if (this.SaveButton.IsFormDefaultButton) defaultFormButtons++;
			}

			XmlElement cancelButtonElement = panelElement.SelectSingleNode("p:CancelButton", xmlParser.NamespaceManager) as XmlElement;
			if (cancelButtonElement != null)
			{
				this.CancelButton = new FormPanelButtonConfiguration(cancelButtonElement, xmlParser);
				this.CancelButton.Text = this.CancelButton.Text ?? Resources.DPCtrl_CancelText;
				if (this.CancelButton.IsFormDefaultButton) defaultFormButtons++;
			}

			if (defaultFormButtons > 1)
				throw new ConfigurationErrorsException(@"The buttons with attribute ""IsFormDefaultButton"" equals to true should be one at maximum.");
		}

		private static void ValidateTypeName(string commandArgument, string typeName)
		{
			Type dynamicComponentType = Kit.GetType(typeName);
			if (dynamicComponentType == null || dynamicComponentType.GetInterface(typeof(IAggregatePanelPage).FullName) == null)
				throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, Resources.DP_InvalidAggregatePanelType, typeName, commandArgument));
		}
	}
}
