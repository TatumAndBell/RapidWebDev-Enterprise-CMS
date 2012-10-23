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
using System.Xml.Linq;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Event argument to manage visibility of controls in gridview rows of dynamic page.
	/// </summary>
	public sealed class GridRowControlBindEventArgs : EventArgs
	{
		/// <summary>
		/// Gets binding data item which used for callback to set control visibility by data.
		/// </summary>
		public object DataItem { get; private set; }

		/// <summary>
		/// Sets/gets true to display checkbox column. Default value is true.
		/// </summary>
		public bool ShowCheckBoxColumn { get; set; }

		/// <summary>
		/// Sets/gets true to display view button in buttons column. Default value is true.
		/// </summary>
		public bool ShowViewButton { get; set; }

		/// <summary>
		/// Sets/gets true to display edit button in buttons column. Default value is true.
		/// </summary>
		public bool ShowEditButton { get; set; }

		/// <summary>
		/// Sets/gets true to display delete button in buttons column. Default value is true.
		/// </summary>
		public bool ShowDeleteButton { get; set; }

		/// <summary>
		/// Construct gridview row control binding argument.
		/// </summary>
		/// <param name="dataItem"></param>
		public GridRowControlBindEventArgs(object dataItem)
		{
			this.DataItem = dataItem;
			this.ShowCheckBoxColumn = true;
			this.ShowViewButton = true;
			this.ShowEditButton = true;
			this.ShowDeleteButton = true;
		}
	}
}

