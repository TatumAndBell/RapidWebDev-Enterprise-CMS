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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using Spring.Objects;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Configuration class maps to BaseLayoutConfiguration element.
	/// </summary>
	public abstract class BaseLayoutConfiguration
	{
		private string headerText;

		/// <summary>
		/// Set/get panel header text.
		/// </summary>
		public string HeaderText
		{
			set { this.headerText = value; }
			get { return WebUtility.ReplaceVariables(this.headerText); }
		}

		/// <summary>
		/// Indicates the panel is collapsible. The default value is False.
		/// </summary>
		public bool Collapsible { get; set; }

		/// <summary>
		/// Indicates the panel is collapsed when loaded initially. This only works while Collapsible is True. The default value is False.
		/// </summary>
		public bool Collapsed { get; set; }

		/// <summary>
		/// True to render the panel with custom rounded borders, false to render with plain 1px square borders (defaults to false).
		/// </summary>
		public bool EnableFrame { get; set; }

		/// <summary>
		/// True to display the borders of the panel's body element, false to hide them (defaults to false).
		/// </summary>
		public bool EnableBorder { get; set; }

		/// <summary>
		/// Empty constructor
		/// </summary>
		protected BaseLayoutConfiguration()
		{
		}

		/// <summary>
		/// Construct BaseLayoutConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseLayoutConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		protected BaseLayoutConfiguration(XmlElement baseLayoutConfigurationElement, XmlParser xmlParser)
		{
			this.HeaderText = xmlParser.ParseString(baseLayoutConfigurationElement, "@HeaderText");
			this.Collapsible = xmlParser.ParseBoolean(baseLayoutConfigurationElement, "@Collapsible", false);
			this.Collapsed = xmlParser.ParseBoolean(baseLayoutConfigurationElement, "@Collapsed", false);
			this.EnableBorder = xmlParser.ParseBoolean(baseLayoutConfigurationElement, "@EnableBorder", false);
			this.EnableFrame = xmlParser.ParseBoolean(baseLayoutConfigurationElement, "@EnableFrame", false);
		}
	}
}

