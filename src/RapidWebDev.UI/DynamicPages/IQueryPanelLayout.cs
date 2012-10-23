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
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The interface to generate layout for query panel.
	/// </summary>
	public interface IQueryPanelLayout
	{
		/// <summary>
		/// Create layout control which contains all query field controls from configurations.
		/// </summary>
		/// <param name="queryFieldControlsByConfigurations"></param>
		/// <returns></returns>
		Control Create(IEnumerable<KeyValuePair<QueryFieldConfiguration, IEnumerable<IQueryFieldControl>>> queryFieldControlsByConfigurations);
	}
}

