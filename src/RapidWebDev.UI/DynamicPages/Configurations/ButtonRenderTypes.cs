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

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Button type.
	/// </summary>
	public enum ButtonRenderTypes
	{
		/// <summary>
		/// Rendered as an Ext button.
		/// </summary>
		Button = 0,

		/// <summary>
		/// Rendered as a common hyper link
		/// </summary>
		Link = 1,

		/// <summary>
		/// Rendered as a "New" link image button.
		/// </summary>
		NewImage = 2,

		/// <summary>
		/// Rendered as a "Edit" link image button.
		/// </summary>
		EditImage = 4,

		/// <summary>
		/// Rendered as a "Delete" link image button.
		/// </summary>
		DeleteImage = 8,

		/// <summary>
		/// Rendered as a "View" link image button.
		/// </summary>
		ViewImage = 16,

		/// <summary>
		/// Rendered as a custom link image button by specified image URL.
		/// </summary>
		CustomImage = 32,

		/// <summary>
		/// Rendered as a "Print" link image button.
		/// </summary>
		PrintImage = 64,

		/// <summary>
		/// Rendered as a "Download Excel" link image button.
		/// </summary>
		DownloadExcelImage = 128,
	}
}

