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

namespace RapidWebDev.UI.DynamicPages.PrintAndExcel
{
	/// <summary>
	/// ExtJS grid client configuration submitted through cookie.
	/// </summary>
	public class ExtGridConfiguration
	{
		/// <summary>
		/// ExtJS grid column client configuration submitted through cookie.
		/// </summary>
		public IEnumerable<ExtGridColumnConfiguration> Columns { get; set; }

		/// <summary>
		/// The sorted column of grid.
		/// </summary>
		public string SortedBy { get; set; }

		/// <summary>
		/// True when the grid is sorting ascendingly.
		/// </summary>
		public bool SortedAscendingly { get; set; }
	}
}
