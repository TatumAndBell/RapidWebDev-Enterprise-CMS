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
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using RapidWebDev.Common;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Platform.Properties;
using MLApplication = RapidWebDev.Platform.Linq.Application;

namespace RapidWebDev.Platform.Initialization
{
    /// <summary>
    /// Application installer - automatically installing application for first accessing.
    /// </summary>
    public class ApplicationInstaller : IInstaller
    {
        private IApplicationApi applicationApi;
        private ApplicationObject applicationObjectTemplate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationApi">Application API</param>
        public ApplicationInstaller(IApplicationApi applicationApi)
        {
            this.applicationApi = applicationApi;
            this.applicationObjectTemplate = new ApplicationObject();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInstaller"/> class.
        /// </summary>
        /// <param name="applicationApi">The application API.</param>
        /// <param name="applicationObjectTemplate">The application object template.</param>
        public ApplicationInstaller(IApplicationApi applicationApi, ApplicationObject applicationObjectTemplate)
        {
            this.applicationApi = applicationApi;
            this.applicationObjectTemplate = applicationObjectTemplate;
        }

        /// <summary>
        /// Install application by name.
        /// Do nothing if the application have existed.
        /// </summary>
        /// <param name="applicationName">application name to install</param>
		public void Install(string applicationName)
        {
			if (this.applicationApi.Exists(applicationName)) return;

			ApplicationObject applicationObject = this.applicationObjectTemplate;
			applicationObject.Id = Guid.Empty;
			applicationObject.Name = applicationName;

			string requestUri = HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : "Local";
			applicationObject.Description = string.Format(@"Auto initialization on URI ""{0}"" at {1}", requestUri, DateTime.UtcNow);
			applicationObject["Theme"] = "Gray";
			this.applicationApi.Save(applicationObject);
        }

        /// <summary>
        /// Uninstall application
        /// </summary>
        /// <param name="applicationName">application name to uninstall</param>
        public void Uninstall(string applicationName)
        {
        }
    }
}

