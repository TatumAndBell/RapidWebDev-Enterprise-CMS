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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;
using RapidWebDev.UI.WebResources;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RapidWebDev.UI
{
	/// <summary>
	/// Interface of SiteMap item.
	/// </summary>
	public interface ISiteMapItem
	{
		/// <summary>
		/// Children 
		/// </summary>
		IEnumerable<ISiteMapItem> Children { get; }

		/// <summary>
		/// Id
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Text
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Value
		/// </summary>
		string Value { get; }

		/// <summary>
		/// Type
		/// </summary>
		SiteMapItemTypes Type { get; }

		/// <summary>
		/// Client side JS command
		/// </summary>
		string ClientSideCommand { get; }

		/// <summary>
		/// Navigate page Url
		/// </summary>
		string PageUrl { get; }

		/// <summary>
		/// Icon class name of menu item
		/// </summary>
		string IconClassName { get; }
	}

	/// <summary>
	/// Sitemap item type enumeration.
	/// </summary>
	public enum SiteMapItemTypes 
	{
		/// <summary>
		/// Common item
		/// </summary>
		Item,

		/// <summary>
		/// Separator
		/// </summary>
		Separator
	}
}

