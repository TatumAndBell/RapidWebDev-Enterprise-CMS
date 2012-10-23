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
using System.Web;
using System.Web.UI;
using RapidWebDev.Common;

namespace RapidWebDev.UI.WebResources
{
	/// <summary>
	/// Web resources group.
	/// </summary>
	public class WebResourceGroup
	{
		/// <summary>
		/// Sets/gets enumerable web resources.
		/// </summary>
		public IEnumerable<WebResource> Resources { get; set; }
	}

	/// <summary>
	/// Web resource entity.
	/// </summary>
	public class WebResource
	{
		/// <summary>
		/// File reference type
		/// </summary>
		public WebResourceType Type { get; set; }

		/// <summary>
		/// Web resource Uri which supports local absolute/relative path, remote resource likes "http://" and assembly manifest resource likes "assembly://namespace/resource".
		/// </summary>
		public string Uri { get; set; }

		/// <summary>
		/// True indicates whether the web resource is independent that means it won't be merged and compressed to clients.
		/// </summary>
		public bool Independent { get; set; }

		/// <summary>
		/// The web resource is only rendered when the current UI culture match this property value.
		/// </summary>
		public string CultureName { get; set; }
	}

	/// <summary>
	/// Web resource type
	/// </summary>
	public enum WebResourceType 
	{ 
		/// <summary>
		/// None
		/// </summary>
		None,

		/// <summary>
		/// Style 
		/// </summary>
		Style, 
		
		/// <summary>
		/// JavaScript
		/// </summary>
		JavaScript 
	}
}

