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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using RapidWebDev.Common;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// The configuration is used to build detail panel for single entity create, update and view.
	/// </summary>
	public class DetailPanelConfiguration : BasePanelConfiguration
	{
		private string typeName;

		/// <summary>
		/// The C# managed type of dynamic page which implements the interface IDetailPanelPage.
		/// </summary>
		public Type @Type
		{
			get { return Kit.GetType(this.typeName); }
		}

		/// <summary>
		/// Specify an ASCX template to render panel UI. Here supports both relative(~/) and abosolute path.
		/// </summary>
		public string SkinPath { get; set; }

		/// <summary>
		/// The width of detail panel (defaults to 960).
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// The height of detail panel (defaults to 600).
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// True indicates the detail panel is resizable (defaults to false).
		/// </summary>
		public bool Resizable { get; set; }

		/// <summary>
		/// True indicates the detail panel is draggable (defaults to false).
		/// </summary>
		public bool Draggable { get; set; }

		/// <summary>
		/// True indicates show message box in UI after saved successfully (defaults to true).
		/// </summary>
		public bool ShowMessageAfterSavedSuccessfully { get; set; }

		/// <summary>
		/// True to set focus on first input control automatically when the detail panel is loaded, defaults to true.
		/// </summary>
		public bool SetFocusOnFirstInputControlAutomatically { get; set; }

		/// <summary>
		/// Sets/gets "Save and Add Another" button to add a new entity in detail panel and reset the form after the new entity is saved successfully.
		/// </summary>
		public FormPanelButtonConfiguration SaveAndAddAnotherButton { get; set; }

		/// <summary>
		/// Sets/gets "Save and Close" button to add/update an entity in detail panel and close the dialog after the entity is saved successfully.
		/// </summary>
		public FormPanelButtonConfiguration SaveAndCloseButton { get; set; }

		/// <summary>
		/// Sets/gets "Cancel" button to cancel changes to an entity in detail panel.
		/// </summary>
		public FormPanelButtonConfiguration CancelButton { get; set; }

		/// <summary>
		/// Gets panel type - detail panel.
		/// </summary>
		public override DynamicPagePanelTypes PanelType { get { return DynamicPagePanelTypes.DetailPanel; } }

		/// <summary>
		/// Construct DetailPanelConfiguration
		/// </summary>
		/// <param name="typeName">The C# managed type of dynamic page which implements the interface IDetailPanelPage.</param>
		public DetailPanelConfiguration(string typeName) : base() 
		{
			Kit.NotNull(typeName, "typeName");

			this.Width = 960;
			this.Height = 600;
			this.typeName = typeName;
			this.ShowMessageAfterSavedSuccessfully = true;
			this.SetFocusOnFirstInputControlAutomatically = true;
			ValidateTypeName(typeName);
		}

		/// <summary>
		/// Construct DetailPanelConfiguration instance from xml element.
		/// </summary>
		/// <param name="panelElement"></param>
		/// <param name="xmlParser"></param>
		public DetailPanelConfiguration(XmlElement panelElement, XmlParser xmlParser)
			: base(panelElement, xmlParser)
		{
			this.typeName = xmlParser.ParseString(panelElement, "p:Type");
			ValidateTypeName(this.typeName);

			this.SkinPath = xmlParser.ParseString(panelElement, "p:SkinPath");
			this.Width = xmlParser.ParseInt(panelElement, "@Width", 960);
			this.Height = xmlParser.ParseInt(panelElement, "@Height", 600);
			this.Resizable = xmlParser.ParseBoolean(panelElement, "@Resizable", false);
			this.Draggable = xmlParser.ParseBoolean(panelElement, "@Draggable", false);
			this.ShowMessageAfterSavedSuccessfully = xmlParser.ParseBoolean(panelElement, "@ShowMessageAfterSavedSuccessfully", true);
			this.SetFocusOnFirstInputControlAutomatically = xmlParser.ParseBoolean(panelElement, "@SetFocusOnFirstInputControlAutomatically", true);

			short defaultFormButtons = 0;
			XmlElement saveAndAddAnotherButtonElement = panelElement.SelectSingleNode("p:SaveAndAddAnotherButton", xmlParser.NamespaceManager) as XmlElement;
			if (saveAndAddAnotherButtonElement != null)
			{
				this.SaveAndAddAnotherButton = new FormPanelButtonConfiguration(saveAndAddAnotherButtonElement, xmlParser);
				this.SaveAndAddAnotherButton.Text = this.SaveAndAddAnotherButton.Text ?? Resources.DPCtrl_SaveAndAddAnotherText;
				if (this.SaveAndAddAnotherButton.IsFormDefaultButton) defaultFormButtons++;
			}

			XmlElement saveAndCloseButtonElement = panelElement.SelectSingleNode("p:SaveAndCloseButton", xmlParser.NamespaceManager) as XmlElement;
			if (saveAndCloseButtonElement != null)
			{
				this.SaveAndCloseButton = new FormPanelButtonConfiguration(saveAndCloseButtonElement, xmlParser);
				this.SaveAndCloseButton.Text = this.SaveAndCloseButton.Text ?? Resources.DPCtrl_SaveAndCloseText;
				if (this.SaveAndCloseButton.IsFormDefaultButton) defaultFormButtons++;
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

		private static void ValidateTypeName(string typeName)
		{
			Type dynamicComponentType = Kit.GetType(typeName);
			if (dynamicComponentType == null || dynamicComponentType.GetInterface(typeof(IDetailPanelPage).FullName) == null)
				throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, Resources.DP_InvalidDetailPanelType, typeName));
		}
	}
}
