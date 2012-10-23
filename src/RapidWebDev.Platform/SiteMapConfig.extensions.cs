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
using RapidWebDev.UI;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Sitemap Item Configuration Class.
	/// </summary>
	public partial class SiteMapItemConfig : ISiteMapItem
	{
		/// <summary>
		/// Gets the children of current sitemap item.
		/// </summary>
		IEnumerable<ISiteMapItem> ISiteMapItem.Children
		{
			get 
			{
				if (this.Item == null) return new List<ISiteMapItem>();
				return this.Item.Cast<ISiteMapItem>();
			}
		}

		/// <summary>
		/// Sitemap item type.
		/// </summary>
		RapidWebDev.UI.SiteMapItemTypes ISiteMapItem.Type
		{
			get 
			{
				return (RapidWebDev.UI.SiteMapItemTypes)Enum.Parse(typeof(RapidWebDev.UI.SiteMapItemTypes), this.Type.ToString());
			}
		}
	}
}

