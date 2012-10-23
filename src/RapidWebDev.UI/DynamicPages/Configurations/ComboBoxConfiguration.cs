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
	/// Configuration class maps to ComboBoxConfiguration element.
	/// </summary>
	public sealed class ComboBoxConfiguration : BaseControlConfiguration
	{
		/// <summary>
		/// False to prevent the user from typing text directly into the field, just like a traditional select (defaults to true).
		/// </summary>
		public bool Editable { get; set; }

		/// <summary>
		/// True to restrict the selected value to one of the values in the list, false to allow the user to set arbitrary text into the field (defaults to false)
		/// </summary>
		public bool ForceSelection { get; set; }

		/// <summary>
		/// The minimum number of characters the user must type before autocomplete and typeahead activate (defaults to 2 if remote or 0 if local, does not apply if editable = false)
		/// </summary>
		public int MinChars { get; set; }

		/// <summary>
		/// The JavaScript client event when the selected item changed. The signature of javascript callback method should be as: "void MethodName(newValue);"
		/// An example, "function onXYZSelectedIndexChanged(newValue) { ... }".
		/// </summary>
		public string OnSelectedIndexChanged { get; set; }

		/// <summary>
		/// ComboBox control static selection items. The default value is NULL.
		/// </summary>
		public Collection<ComboBoxItemConfiguration> StaticDataSource { get; set; }

		/// <summary>
		/// ComboBox control dynamic selection items populated from remote calls. The default value is NULL.
		/// </summary>
		public ComboBoxDynamicDataSourceConfiguration DynamicDataSource { get; set; }

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public override ControlConfigurationTypes ControlType { get { return ControlConfigurationTypes.ComboBox; } }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public ComboBoxConfiguration()
		{
		}

		/// <summary>
		/// Construct ComboBoxConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public ComboBoxConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser) : base(baseControlConfigurationElement, xmlParser)
		{
			XmlElement comboBoxElement = baseControlConfigurationElement;
			this.Editable = xmlParser.ParseBoolean(comboBoxElement, "@Editable", false);
			this.ForceSelection = xmlParser.ParseBoolean(comboBoxElement, "@ForceSelection", false);
			this.MinChars = xmlParser.ParseInt(comboBoxElement, "@MinChars", 2);
			this.OnSelectedIndexChanged = xmlParser.ParseString(comboBoxElement, "@OnSelectedIndexChanged");

			XmlElement staticDataSourceElement = comboBoxElement.SelectSingleNode("p:StaticDataSource", xmlParser.NamespaceManager) as XmlElement;
			if (staticDataSourceElement != null)
			{
				this.StaticDataSource = new Collection<ComboBoxItemConfiguration>();
				XmlNodeList itemNodes = staticDataSourceElement.SelectNodes("p:Item", xmlParser.NamespaceManager);
				foreach (XmlElement itemNode in itemNodes.Cast<XmlElement>())
					this.StaticDataSource.Add(new ComboBoxItemConfiguration(itemNode, xmlParser));
			}

			XmlElement dynamicDataSourceElement = comboBoxElement.SelectSingleNode("p:DynamicDataSource", xmlParser.NamespaceManager) as XmlElement;
			if (dynamicDataSourceElement != null)
				this.DynamicDataSource = new ComboBoxDynamicDataSourceConfiguration(dynamicDataSourceElement, xmlParser);
		}
	}
}

