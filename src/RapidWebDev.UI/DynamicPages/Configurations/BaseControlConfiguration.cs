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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Configuration class maps to BaseControlConfiguration element.
	/// </summary>
	public abstract class BaseControlConfiguration
	{
		/// <summary>
		/// "RapidWebDev.UI.DynamicPages.Configurations.{0}Configuration, [AssemblyName]"
		/// </summary>
		private static string CONTROL_TYPE_TEMPLATE = "RapidWebDev.UI.DynamicPages.Configurations.{0}Configuration, " + typeof(BaseControlConfiguration).Assembly.GetName().Name;
		private string label;

		/// <summary>
		/// Gets display label of the control.
		/// </summary>
		public string Label
		{
			get { return WebUtility.ReplaceVariables(this.label); }
			set { this.label = value; }
		}

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public abstract ControlConfigurationTypes ControlType { get; }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		protected BaseControlConfiguration()
		{
		}

		/// <summary>
		/// Construct BaseControlConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		protected BaseControlConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser)
		{
			this.Label = xmlParser.ParseString(baseControlConfigurationElement, "@Label");
		}

		/// <summary>
		/// Create form control configuration from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		/// <returns></returns>
		public static BaseControlConfiguration Create(XmlElement baseControlConfigurationElement, XmlParser xmlParser)
		{
			ControlConfigurationTypes formControlType;
			string childElementName = baseControlConfigurationElement.LocalName;
			try
			{
				formControlType = (ControlConfigurationTypes)Enum.Parse(typeof(ControlConfigurationTypes), baseControlConfigurationElement.LocalName, true);
			}
			catch
			{
				throw new ConfigurationErrorsException(string.Format(Resources.DP_ControlElementNotSupported, childElementName));
			}				

			string controlTypeName = string.Format(CONTROL_TYPE_TEMPLATE, formControlType);
			Type controlManagedType = Kit.GetType(controlTypeName);
			return Activator.CreateInstance(controlManagedType, baseControlConfigurationElement, xmlParser) as BaseControlConfiguration;
		}
	}
}

