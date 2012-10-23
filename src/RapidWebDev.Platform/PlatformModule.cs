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
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using RapidWebDev.Common;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using AspNetMembership = System.Web.Security.Membership;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// HttpModule used to install application at first running web application based on the platform component.
	/// </summary>
	public class PlatformModule : IHttpModule
	{
		private static readonly object syncObj = new object();
		private static readonly HashSet<string> initializedApplications = new HashSet<string>();

		/// <summary>
		/// Dispose current instance
		/// </summary>
		void IHttpModule.Dispose() { }

		/// <summary>
		/// initialize http module
		/// </summary>
		/// <param name="context"></param>
		void IHttpModule.Init(HttpApplication context)
		{
			context.AuthenticateRequest += new EventHandler(OnAuthenticateRequest);
			context.PostRequestHandlerExecute += new EventHandler(OnPostRequestHandlerExecute);
		}

		private void OnAuthenticateRequest(object sender, EventArgs e)
		{
			string loweredApplicationName = AspNetMembership.ApplicationName.ToLower();
			if (!initializedApplications.Contains(loweredApplicationName))
			{
				lock (syncObj)
				{
					if (!initializedApplications.Contains(loweredApplicationName))
					{
						SpringContext.Current.GetObject<IInstallerManager>().Install(loweredApplicationName);
						initializedApplications.Add(loweredApplicationName);
					}
				}
			}
		}

		private void OnPostRequestHandlerExecute(object sender, EventArgs e)
		{
			IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
			authenticationContext.Act();
		}
	}
}