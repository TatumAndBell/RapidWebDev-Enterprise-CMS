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

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Date value enumeration
	/// </summary>
	public enum DefaultDateValues
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// First day of the week
		/// </summary>
		FirstDayOfWeek = 1,

		/// <summary>
		/// First day of the month
		/// </summary>
		FirstDayOfMonth = 2,

		/// <summary>
		/// First day of the year
		/// </summary>
		FirstDayOfYeay = 4,

		/// <summary>
		/// Today
		/// </summary>
		Today = 8
	}
}
