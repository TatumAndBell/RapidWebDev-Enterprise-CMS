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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Class to configure button configuration.
	/// </summary>
	[Serializable]
	public class GridViewRowButtonConfiguration
	{
		private string tooltip;

		/// <summary>
		/// Sets/gets generic button tooltip.
		/// </summary>
		public string ToolTip
		{
			set { this.tooltip = value; }
			get { return WebUtility.ReplaceVariables(this.tooltip); }
		}

		/// <summary>
		/// Sets/gets True to display button as an image icon in gridview row. 
		/// </summary>
		public bool DisplayAsImage { get; set; }

		/// <summary>
		/// Construct GridViewRowButtonConfiguration instance from xml element.
		/// </summary>
		/// <param name="buttonElement"></param>
		/// <param name="xmlParser"></param>
		public GridViewRowButtonConfiguration(XmlElement buttonElement, XmlParser xmlParser)
		{
			this.ToolTip = xmlParser.ParseString(buttonElement, "@ToolTip");
			this.DisplayAsImage = xmlParser.ParseBoolean(buttonElement, "@DisplayAsImage", true);
		}
	}
}

