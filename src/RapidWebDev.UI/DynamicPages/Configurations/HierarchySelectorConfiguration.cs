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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using RapidWebDev.UI.Controls;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Configuration class maps to HierarchySelector element inner of QueryPanel.
	/// </summary>
	public sealed class HierarchySelectorConfiguration : BaseControlConfiguration
	{
		private string title;
		private string serviceUrl;

		/// <summary>
		/// Sets/gets the title displayed on the UI, which indicates for selection. Like "Select Area" for geography hierarchy data.
		/// </summary>
		public string Title
		{
			set { this.title = value; }
			get { return WebUtility.ReplaceVariables(this.title); }
		}

		/// <summary>
		/// Hierarchy tree nodes checking cascading type, defaults to Full.
		/// </summary>
		public TreeNodeCheckCascadingTypes Cascading { get; set; }

		/// <summary>
		/// Sets/gets the hierarchy service Url to pull hierarchy data collection.
		/// </summary>
		public string ServiceUrl
		{
			set { this.serviceUrl = value; }
			get { return WebUtility.ReplaceVariables(this.serviceUrl); }
		}

		/// <summary>
		/// Sets/gets field Name of hierarchy data text.
		/// </summary>
		public string TextField { get; set; }

		/// <summary>
		/// Sets/gets field Name of hierarchy data value.
		/// </summary>
		public string ValueField { get; set; }

		/// <summary>
		/// Sets/gets field name indicates which hierarchy node is the parent in the returned array from the service.
		/// </summary>
		public string ParentValueField { get; set; }

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public override ControlConfigurationTypes ControlType { get { return ControlConfigurationTypes.HierarchySelector; } }

		/// <summary>
		/// Constructor
		/// </summary>
		public HierarchySelectorConfiguration()
		{
			this.Cascading = TreeNodeCheckCascadingTypes.Full;
		}

		/// <summary>
		/// Construct HierarchySelectorConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public HierarchySelectorConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser)
			: base(baseControlConfigurationElement, xmlParser)
		{
			this.Title = xmlParser.ParseString(baseControlConfigurationElement, "@Title");
			this.Cascading = xmlParser.ParseEnum<TreeNodeCheckCascadingTypes>(baseControlConfigurationElement, "@Cascading");

			XmlElement hierarchyServiceElement = baseControlConfigurationElement.SelectSingleNode("p:HierarchyService", xmlParser.NamespaceManager) as XmlElement;
			if (hierarchyServiceElement != null)
			{
				string hierarchyType = xmlParser.ParseString(hierarchyServiceElement, "@HierarchyType");
				this.ServiceUrl = string.Format(CultureInfo.InvariantCulture, "~/services/HierarchyService.svc/json/GetAllHierarchyData/{0}", hierarchyType);
				this.TextField = "Name";
				this.ValueField = "HierarchyDataId";
				this.ParentValueField = "ParentHierarchyDataId";
			}
			else
			{
				this.ServiceUrl = xmlParser.ParseString(baseControlConfigurationElement, "ExternalService/@ServiceUrl");
				this.TextField = xmlParser.ParseString(baseControlConfigurationElement, "ExternalService/@TextField");
				this.ValueField = xmlParser.ParseString(baseControlConfigurationElement, "ExternalService/@ValueField");
				this.ParentValueField = xmlParser.ParseString(baseControlConfigurationElement, "ExternalService/@ParentValueField");
			}
		}
	}
}