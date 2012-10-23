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
using System.Xml.Linq;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// A config that will be used to create the grid's UI row view.
	/// </summary>
	public sealed class GridViewFieldRowViewConfiguration
	{
		/// <summary>
		/// (required) The field displayed in row view of grid.
		/// </summary>
		public string FieldName { get; private set; }

		/// <summary>
		/// (optional) Apply custom CSS classes to row view during rendering.
		/// </summary>
		public string Css { get; set; }

		/// <summary>
		/// (optional) The HTML tag wraps the field value from specified field name. The default tag name is "p".
		/// </summary>
		public string TagName { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public GridViewFieldRowViewConfiguration()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="gridViewRowViewElement"></param>
		/// <param name="xmlParser"></param>
		public GridViewFieldRowViewConfiguration(XmlElement gridViewRowViewElement, XmlParser xmlParser) 
		{
			this.FieldName = xmlParser.ParseString(gridViewRowViewElement, "@FieldName");
			this.Css = xmlParser.ParseString(gridViewRowViewElement, "@Css");
			this.TagName = xmlParser.ParseString(gridViewRowViewElement, "@TagName");
		}
	}
}
