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
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.UI.WebControls;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Configuration class maps to DateTimeConfiguration element.
	/// </summary>
	public sealed class DateTimeConfiguration : BaseControlConfiguration
	{
		/// <summary>
		/// The maximum allowed date.
		/// </summary>
		public DateTime? MaxValue { get; set; }

		/// <summary>
		/// The minimum allowed date.
		/// </summary>
		public DateTime? MinValue { get; set; }

		/// <summary>
		/// The default date of the field when loaded.
		/// </summary>
		public DefaultDateValues DefaultDate { get; set; }

		/// <summary>
		/// The default time of the field when loaded.
		/// </summary>
		public DefaultTimeValues DefaultTime { get; set; }

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public override ControlConfigurationTypes ControlType { get { return ControlConfigurationTypes.DateTime; } }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public DateTimeConfiguration()
		{
		}

		/// <summary>
		/// Construct DateTimeConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public DateTimeConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser) : base(baseControlConfigurationElement, xmlParser)
		{
			this.MaxValue = xmlParser.ParseDateTime(baseControlConfigurationElement, "@MaxLength", null);
			this.MinValue = xmlParser.ParseDateTime(baseControlConfigurationElement, "@MinLength", null);
			this.DefaultDate = xmlParser.ParseEnum<DefaultDateValues>(baseControlConfigurationElement, "@DefaultDate");
			this.DefaultTime = xmlParser.ParseEnum<DefaultTimeValues>(baseControlConfigurationElement, "@DefaultTime");
		}
	}
}

