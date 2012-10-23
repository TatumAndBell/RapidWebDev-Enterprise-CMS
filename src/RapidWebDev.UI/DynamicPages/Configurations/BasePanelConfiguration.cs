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
using System.Globalization;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Class to configure base panel form.
	/// </summary>
	public abstract class BasePanelConfiguration
	{
		private string id;
		private string headerText;

		/// <summary>
		/// Unique panel ID in this dynamic page.
		/// </summary>
		public string Id 
		{
			get { return WebUtility.ReplaceVariables(this.id); }
			set { this.id = value; }
		}

		/// <summary>
		/// Set/get form header text.
		/// </summary>
		public string HeaderText
		{
			set { this.headerText = value; }
			get { return WebUtility.ReplaceVariables(this.headerText); }
		}

		/// <summary>
		/// Gets panel type
		/// </summary>
		public abstract DynamicPagePanelTypes PanelType { get; }

		/// <summary>
		/// Construct panel Configuration
		/// </summary>
		protected BasePanelConfiguration() { }

		/// <summary>
		/// Construct BasePanelConfiguration instance from xml element.
		/// </summary>
		/// <param name="panelElement"></param>
		/// <param name="xmlParser"></param>
		protected BasePanelConfiguration(XmlElement panelElement, XmlParser xmlParser)
		{
			this.Id = xmlParser.ParseString(panelElement, "@Id");
			this.HeaderText = xmlParser.ParseString(panelElement, "@HeaderText");
		}
	}

	/// <summary>
	/// The supported modules of dynamic page.
	/// </summary>
	[Flags]
	public enum DynamicPagePanelTypes 
	{ 
		/// <summary>
		/// Query panel is used for users to input query criteria. There allows only maximum one query panel in a dynamic page.
		/// </summary>
		QueryPanel, 

		/// <summary>
		/// GridView panel to display the query result items. There allows only maximum one gridview panel in a dynamic page.
		/// </summary>
		GridViewPanel, 

		/// <summary>
		/// Detail panel will be rendered as a form to create/edit/view a single record.
		/// </summary>
		DetailPanel,

		/// <summary>
		/// Aggregate panel will be rendered as a form for any custom operations to the multiple selected records.
		/// </summary>
		AggregatePanel,

		/// <summary>
		/// Panel contains a collection of buttons.
		/// </summary>
		ButtonPanel
	}
}
