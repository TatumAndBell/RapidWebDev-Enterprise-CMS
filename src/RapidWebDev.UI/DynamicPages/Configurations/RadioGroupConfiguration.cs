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
	/// Configuration class maps to RadioGroupConfiguration element.
	/// </summary>
	public sealed class RadioGroupConfiguration : BaseControlConfiguration
	{
		/// <summary>
		/// Gets radio group items.
		/// </summary>
		public Collection<RadioGroupItemConfiguration> Items { get; set; }

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public override ControlConfigurationTypes ControlType { get { return ControlConfigurationTypes.RadioGroup; } }

		/// <summary>
		/// Constructor
		/// </summary>
		public RadioGroupConfiguration()
		{
			this.Items = new Collection<RadioGroupItemConfiguration>();
		}

		/// <summary>
		/// Construct RadioGroupConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public RadioGroupConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser) : base(baseControlConfigurationElement, xmlParser)
		{
			this.Items = new Collection<RadioGroupItemConfiguration>();
			XmlNodeList radioConfigurationElements = baseControlConfigurationElement.SelectNodes("p:Item", xmlParser.NamespaceManager);
			foreach (XmlElement radioConfigurationElement in radioConfigurationElements.Cast<XmlElement>())
				this.Items.Add(new RadioGroupItemConfiguration(radioConfigurationElement, xmlParser));
		}
	}

	/// <summary>
	/// Configuration class maps to RadioGroupItemConfiguration element.
	/// </summary>
	public class RadioGroupItemConfiguration
	{
		private string value;

		/// <summary>
		/// Gets radio item display text.
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		/// Gets radio item value.
		/// </summary>
		public string Value
		{
			get { return this.value ?? this.Text; }
			set { this.value = value; }
		}

		/// <summary>
		/// True indicates the radio is checked as default. The default value is false.
		/// </summary>
		public bool Checked { get; set; }

		/// <summary>
		/// Construct RadioGroupItemConfiguration instance from xml element.
		/// </summary>
		/// <param name="radioGroupItemConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public RadioGroupItemConfiguration(XmlElement radioGroupItemConfigurationElement, XmlParser xmlParser)
		{
			this.Text = xmlParser.ParseString(radioGroupItemConfigurationElement, "@Text");
			this.Value = xmlParser.ParseString(radioGroupItemConfigurationElement, "@Value");
			this.Checked = xmlParser.ParseBoolean(radioGroupItemConfigurationElement, "@Checked", false);
		}
	}
}

