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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Class to configure gridview field value transform switch strategy.
	/// </summary>
	public class GridViewFieldValueTransformSwitchCase
	{
		private string output;

		/// <summary>
		/// The matching field value.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Case sensitive while matching the field value. The default value is False.
		/// </summary>
		public bool CaseSensitive { get; set; }

		/// <summary>
		/// The output string when matched the field value.
		/// </summary>
		public string Output 
		{
			get { return WebUtility.ReplaceVariables(this.output); }
			set { this.output = value; }
		}

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public GridViewFieldValueTransformSwitchCase()
		{
		}

		/// <summary>
		/// Construct GridViewFieldValueTransformSwitchCase instance from xml element.
		/// </summary>
		/// <param name="switchCaseElement"></param>
		/// <param name="xmlParser"></param>
		public GridViewFieldValueTransformSwitchCase(XmlElement switchCaseElement, XmlParser xmlParser)
		{
			this.Value = xmlParser.ParseString(switchCaseElement, "@Value");
			this.CaseSensitive = xmlParser.ParseBoolean(switchCaseElement, "@CaseSensitive", false);
			this.Output = switchCaseElement.InnerText;
		}
	}
}

