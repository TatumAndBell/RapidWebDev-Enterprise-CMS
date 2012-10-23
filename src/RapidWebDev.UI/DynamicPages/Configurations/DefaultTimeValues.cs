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
	/// Time value enumeration
	/// </summary>
	public enum DefaultTimeValues
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// Now
		/// </summary>
		Now = 1,

		/// <summary>
		/// 00:00AM
		/// </summary>
		H0,

		/// <summary>
		/// 01:00AM
		/// </summary>
		H1,

		/// <summary>
		/// 02:00AM
		/// </summary>
		H2,

		/// <summary>
		/// 03:00AM
		/// </summary>
		H3,

		/// <summary>
		/// 04:00AM
		/// </summary>
		H4,

		/// <summary>
		/// 05:00AM
		/// </summary>
		H5,

		/// <summary>
		/// 06:00AM
		/// </summary>
		H6,

		/// <summary>
		/// 07:00AM
		/// </summary>
		H7,

		/// <summary>
		/// 08:00AM
		/// </summary>
		H8,

		/// <summary>
		/// 09:00AM
		/// </summary>
		H9,

		/// <summary>
		/// 10:00AM
		/// </summary>
		H10,

		/// <summary>
		/// 11:00AM
		/// </summary>
		H11,

		/// <summary>
		/// 12:00AM
		/// </summary>
		H12,

		/// <summary>
		/// 01:00PM
		/// </summary>
		H13,

		/// <summary>
		/// 02:00PM
		/// </summary>
		H14,

		/// <summary>
		/// 03:00PM
		/// </summary>
		H15,

		/// <summary>
		/// 04:00PM
		/// </summary>
		H16,

		/// <summary>
		/// 05:00PM
		/// </summary>
		H17,

		/// <summary>
		/// 06:00PM
		/// </summary>
		H18,

		/// <summary>
		/// 07:00PM
		/// </summary>
		H19,

		/// <summary>
		/// 08:00PM
		/// </summary>
		H20,

		/// <summary>
		/// 09:00PM
		/// </summary>
		H21,

		/// <summary>
		/// 10:00PM
		/// </summary>
		H22,

		/// <summary>
		/// 11:00PM
		/// </summary>
		H23,
	}
}

