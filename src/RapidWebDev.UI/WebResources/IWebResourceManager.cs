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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using RapidWebDev.Common;
using Spring.Core.IO;

namespace RapidWebDev.UI.WebResources
{
	/// <summary>
	/// The manager is to generate a Uri returned for clients which contains the configured style and JavaScript resources mapping to specified resource id. 
	/// </summary>
	public interface IWebResourceManager
	{
		/// <summary>
		/// Render the resources in the container by id with filters.
		/// </summary>
		/// <param name="resourceId"></param>
		/// <param name="filters"></param>
		void Flush(string resourceId, params KeyValuePair<string, string>[] filters);
	}
}