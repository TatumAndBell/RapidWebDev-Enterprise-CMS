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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.WebResources;
using Spring.Core.IO;

namespace RapidWebDev.Platform.SaaS
{
	/// <summary>
	/// The class renders web resources by Theme configured in SaaS application and Culture setting from the request of client.
	/// </summary>
	public class SaaSWebResourceManager : XmlConfigWebResourceManager
	{
		private IAuthenticationContext authenticationContext;
		private IApplicationApi applicationApi;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="xmlConfigDirectoryPath">The directory stores web resource xml configuration files.</param>
		/// <param name="authenticationContext">authentication context</param>
		/// <param name="applicationApi">application Api</param>
		public SaaSWebResourceManager(string xmlConfigDirectoryPath, IAuthenticationContext authenticationContext, IApplicationApi applicationApi)
			: base(xmlConfigDirectoryPath)
		{
			this.authenticationContext = authenticationContext;
			this.applicationApi = applicationApi;
		}

		/// <summary>
		/// Render the resources in the container by id with filters.
		/// </summary>
		/// <param name="resourceId"></param>
		/// <param name="filters"></param>
		public override void Flush(string resourceId, params KeyValuePair<string, string>[] filters)
		{
			List<KeyValuePair<string, string>> filterList = new List<KeyValuePair<string, string>>(filters);

			ApplicationObject app = this.applicationApi.Get(this.authenticationContext.ApplicationId);
			string theme = app["Theme"] as string;
			if (!string.IsNullOrEmpty(theme))
				filterList.Add(new KeyValuePair<string, string>("Theme", theme));

			if (CultureInfo.CurrentUICulture != null)
				filterList.Add(new KeyValuePair<string, string>("Culture", CultureInfo.CurrentUICulture.Name));

			base.Flush(resourceId, filterList.ToArray());
		}
	}
}