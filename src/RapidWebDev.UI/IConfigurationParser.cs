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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;

namespace RapidWebDev.UI
{
	/// <summary>
	/// Interface of configuration parser for workshop controls.
	/// </summary>
	public interface IConfigurationParser
	{
		/// <summary>
		/// Get parsing object by id in specified generic type.
		/// </summary>
		/// <typeparam name="T">The generic type of return object</typeparam>
		/// <param name="objectId">object id</param>
		/// <returns></returns>
		T GetObject<T>(string objectId);

		/// <summary>
		/// Returns true if ConfigurationParser contains specified object id.
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		bool ContainsObject(string objectId);
	}
}

