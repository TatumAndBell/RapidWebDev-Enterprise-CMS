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
using System.Configuration;
using System.Globalization;
using System.Xml;
using RapidWebDev.Common;
using RapidWebDev.UI.DynamicPages.Resolvers;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Configuration class maps to CustomConfiguration element.
	/// </summary>
	public sealed class CustomConfiguration : BaseControlConfiguration
	{
		/// <summary>
		/// The custom control type.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// The resolver used to resolve server-side control value from http post parameters for the query field control.
		/// </summary>
		public string ResolverName { get; set; }

		/// <summary>
		/// Gets control type for the configuration.
		/// </summary>
		public override ControlConfigurationTypes ControlType { get { return ControlConfigurationTypes.Custom; } }

		/// <summary>
		/// Constructor
		/// </summary>
		public CustomConfiguration()
		{
		}

		/// <summary>
		/// Construct CustomConfiguration instance from xml element.
		/// </summary>
		/// <param name="baseControlConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public CustomConfiguration(XmlElement baseControlConfigurationElement, XmlParser xmlParser)  : base(baseControlConfigurationElement, xmlParser)
		{
			string typeName = xmlParser.ParseString(baseControlConfigurationElement, "@Type");
			try
			{
				this.Type = Kit.GetType(typeName);
			}
			catch (NotImplementedException exp)
			{
				throw new ConfigurationErrorsException(exp.Message, exp);
			}

			this.ResolverName = xmlParser.ParseString(baseControlConfigurationElement, "@ResolverName");
			IControlValueResolverFactory controlValueResolverFactory = SpringContext.Current.GetObject<IControlValueResolverFactory>();
			if(!controlValueResolverFactory.Contains(this.ResolverName))
				throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, Resources.DP_ControlValueResolverUnavailable, this.ResolverName));
		}
	}
}

