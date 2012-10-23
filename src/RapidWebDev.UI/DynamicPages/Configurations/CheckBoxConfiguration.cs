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

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Configuration class maps to CheckBox element inner of QueryPanel.
	/// </summary>
	public sealed class CheckBoxConfiguration : BaseControlConfiguration
	{
		private string text;

		/// <summary>
		/// Gets checkbox item display text.
		/// </summary>
		public string Text
		{
			set { this.text = value; }
			get { return WebUtility.ReplaceVariables(this.text); }
		}

		/// <summary>
		/// True indicates the checkbox is checked as default. The default value is false.
		/// </summary>
		public bool Checked { get; set; }

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public override ControlConfigurationTypes ControlType { get { return ControlConfigurationTypes.CheckBox; } }

		/// <summary>
		/// Constructor
		/// </summary>
		public CheckBoxConfiguration()
		{
		}

		/// <summary>
		/// Construct CheckBoxConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public CheckBoxConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser) : base(baseControlConfigurationElement, xmlParser)
		{
			this.Text = xmlParser.ParseString(baseControlConfigurationElement, "@Text");
			this.Checked = xmlParser.ParseBoolean(baseControlConfigurationElement, "@Checked", false);
		}
	}
}

