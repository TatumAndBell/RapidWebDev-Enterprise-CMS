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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The interface restraints the dynamic page. 
	/// </summary>
	public interface IDynamicPage : IDynamicComponent
	{
		/// <summary>
		/// Execute query for results binding to dynamic page grid.
		/// </summary>
		/// <param name="parameter">Query parameter.</param>
		/// <returns>Returns query results.</returns>
		QueryResults Query(QueryParameter parameter);

		/// <summary>
		/// Delete record by id.
		/// </summary>
		/// <param name="entityId"></param>
		void Delete(string entityId);

		/// <summary>
		/// The method will be invoked when the data item binding to grid rows.
		/// </summary>
		/// <param name="e"></param>
		void OnGridRowControlsBind(GridRowControlBindEventArgs e);

		/// <summary>
		/// The method will be invoked when the web page is initialized.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument</param>
		void OnInit(IRequestHandler sender, EventArgs e);

		/// <summary>
		/// The method will be invoked when the web page is loaded.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument</param>
		void OnLoad(IRequestHandler sender, EventArgs e);

		/// <summary>
		/// The method will be invoked when the web page is prerendering.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument</param>
		void OnPreRender(IRequestHandler sender, EventArgs e);
	}
}