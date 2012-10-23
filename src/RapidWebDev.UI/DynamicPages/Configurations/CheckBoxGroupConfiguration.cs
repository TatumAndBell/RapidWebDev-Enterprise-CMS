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
	/// Configuration class maps to CheckBoxGroupConfiguration element.
	/// </summary>
	public sealed class CheckBoxGroupConfiguration : BaseControlConfiguration
	{
		/// <summary>
		/// Gets checkbox group items.
		/// </summary>
		public Collection<CheckBoxGroupItemConfiguration> Items { get; set; }

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public override ControlConfigurationTypes ControlType { get { return ControlConfigurationTypes.CheckBoxGroup; } }

		/// <summary>
		/// Constructor
		/// </summary>
		public CheckBoxGroupConfiguration()
		{
			this.Items = new Collection<CheckBoxGroupItemConfiguration>();
		}

		/// <summary>
		/// Construct CheckBoxGroupConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public CheckBoxGroupConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser) : base(baseControlConfigurationElement, xmlParser)
		{
			this.Items = new Collection<CheckBoxGroupItemConfiguration>();
			XmlNodeList checkboxConfigurationElements = baseControlConfigurationElement.SelectNodes("p:Item", xmlParser.NamespaceManager);
			foreach (XmlElement checkboxGroupItemConfigurationElement in checkboxConfigurationElements.Cast<XmlElement>())
				this.Items.Add(new CheckBoxGroupItemConfiguration(checkboxGroupItemConfigurationElement, xmlParser));
		}
	}
}

