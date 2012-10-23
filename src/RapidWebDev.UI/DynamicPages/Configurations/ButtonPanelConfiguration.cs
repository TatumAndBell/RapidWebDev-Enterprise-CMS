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
	/// Button panel groups a collection of buttons which do operations against multiple selected gridview records.
	/// </summary>
	public class ButtonPanelConfiguration : BasePanelConfiguration
	{
		/// <summary>
		/// Indicates how buttons aligned in the panel.
		/// </summary>
		public HorizontalAlign ButtonAlignment { get; set; }

		/// <summary>
		/// A collection of buttons in the panel.
		/// </summary>
		public Collection<ButtonConfiguration> Buttons { get; set; }

		/// <summary>
		/// Gets panel type - button panel. Button panel groups a collection of buttons which do operations against multiple selected gridview records.
		/// </summary>
		public override DynamicPagePanelTypes PanelType { get { return DynamicPagePanelTypes.ButtonPanel; } }

		/// <summary>
		/// Construct ButtonPanelConfiguration
		/// </summary>
		public ButtonPanelConfiguration() : base() 
		{
			this.Buttons = new Collection<ButtonConfiguration>();
		}

		/// <summary>
		/// Construct ButtonPanelConfiguration instance from xml element.
		/// </summary>
		/// <param name="panelElement"></param>
		/// <param name="xmlParser"></param>
		public ButtonPanelConfiguration(XmlElement panelElement, XmlParser xmlParser) : base(panelElement, xmlParser)
		{
			this.ButtonAlignment = xmlParser.ParseEnum<HorizontalAlign>(panelElement, "@ButtonAlignment");

			this.Buttons = new Collection<ButtonConfiguration>();
			XmlNodeList buttonNodeList = panelElement.SelectNodes("p:Button", xmlParser.NamespaceManager);
			foreach (XmlElement buttonElement in buttonNodeList.Cast<XmlElement>())
				this.Buttons.Add(new ButtonConfiguration(buttonElement, xmlParser));
		}
	}
}
