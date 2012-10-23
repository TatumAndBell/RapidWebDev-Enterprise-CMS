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
using System.IO;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI;
using System.Web.Configuration;

namespace RapidWebDev.Web
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{
			//Logger.Instance(this).Error(HttpContext.Current.Request.Url, HttpContext.Current.Error);

			CustomErrorsSection customErrorsSection = ConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
			if (customErrorsSection.Mode == CustomErrorsMode.Off) return;

			if (customErrorsSection.Mode == CustomErrorsMode.On || !HttpContext.Current.Request.IsLocal)
			{
				Exception unhandledException = HttpContext.Current.Error;
				if (unhandledException is HttpUnhandledException)
					unhandledException = unhandledException.InnerException ?? unhandledException;

				if (unhandledException is ResourceNotFoundException)
					WebUtility.RedirectToPage(WebUtility.PageNotFoundUrl);
				else if (unhandledException is BadRequestException)
					WebUtility.RedirectToPage(WebUtility.PageNotFoundUrl);
				else if (unhandledException is UnauthorizedException)
					WebUtility.RedirectToPage(WebUtility.NotAuthorizedUrl);
				else
					WebUtility.RedirectToPage(WebUtility.InternalServerError);
			}
		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}
