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
using System.Globalization;

namespace RapidWebDev.UI.DynamicPages.PrintAndExcel
{
	/// <summary>
	/// ExtJS grid column client configuration submitted through cookie.
	/// </summary>
	public class ExtGridColumnConfiguration
	{
		/// <summary>
		/// ExtJS grid column index in client.
		/// </summary>
		public int ColumnIndex { get; set; }

		/// <summary>
		/// ExtJS grid column width.
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// True when the grid column is hidden.
		/// </summary>
		public bool Hidden { get; set; }

		/// <summary>
		/// Convert the instance to string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Column: {0}, Width: {1}, Hidden: {2}", this.ColumnIndex, this.Width, this.Hidden);
		}
	}
}

