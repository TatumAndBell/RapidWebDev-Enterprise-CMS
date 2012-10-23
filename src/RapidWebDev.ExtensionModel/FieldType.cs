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

namespace RapidWebDev.ExtensionModel
{
    /// <summary>
    /// Extension field type
    /// </summary>
	public enum FieldType
	{
		/// <summary>
		/// Text
		/// </summary>
		String = 0,

		/// <summary>
		/// DateTime
		/// </summary>
		DateTime = 1,

		/// <summary>
		/// Decimal
		/// </summary>
		Decimal = 2,

		/// <summary>
		/// Integer
		/// </summary>
		Integer = 3,

		/// <summary>
		/// Hierarchy
		/// </summary>
		Hierarchy = 4,

		/// <summary>
		/// Enumeration
		/// </summary>
		Enumeration = 5,

		/// <summary>
		/// Date
		/// </summary>
		Date = 6,
	}
}
