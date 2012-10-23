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
	/// Configuration class maps to TextBoxConfiguration element.
	/// </summary>
	public sealed class TextBoxConfiguration : BaseControlConfiguration
	{
		/// <summary>
		/// Maximum input field length allowed (defaults to 2147483647).
		/// </summary>
		public int? MaxLength { get; set; }

		/// <summary>
		/// Minimum input field length required (defaults to 0).
		/// </summary>
		public int? MinLength { get; set; }

		/// <summary>
		/// A validation type to validate textbox value. No validation as default value.
		/// </summary>
		public TextBoxValidationTypes ValidationType { get; set; }

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public override ControlConfigurationTypes ControlType { get { return ControlConfigurationTypes.TextBox; } }

		/// <summary>
		/// Construct TextBoxConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public TextBoxConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser) : base(baseControlConfigurationElement, xmlParser)
		{
			XmlElement textboxElement = baseControlConfigurationElement;
			this.MaxLength = xmlParser.ParseInt(textboxElement, "@MaxLength", null);
			this.MinLength = xmlParser.ParseInt(textboxElement, "@MinLength", null);
			this.ValidationType = xmlParser.ParseEnum<TextBoxValidationTypes>(textboxElement, "@ValidationType");
		}
	}

	/// <summary>
	/// TextBox validation type.
	/// </summary>
	public enum TextBoxValidationTypes
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// Email
		/// </summary>
		Email = 1,

		/// <summary>
		/// Url
		/// </summary>
		Url = 2,

		/// <summary>
		/// Alpha
		/// </summary>
		Alpha = 4,

		/// <summary>
		/// Alpha number
		/// </summary>
		Alphanum = 8
	}
}

