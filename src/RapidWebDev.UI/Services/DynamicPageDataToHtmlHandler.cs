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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using RapidWebDev.Common;
using RapidWebDev.Common.CodeDom;
using RapidWebDev.Common.Validation;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;
using Spring.Aop.Framework;
using Spring.Aop.Support;
using Spring.Objects;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// The service is to create an html document file for clients to print based on query criteria submitted.
	/// </summary>
	public class DynamicPageDataToHtmlHandler : IHttpHandler, IRequiresSessionState
	{
		/// <summary>
		/// The server encountered an unknown error when accessing the URI {0}.
		/// </summary>
		private const string UNKNOWN_ERROR_LOGGING_MSG = "The server encountered an unknown error when accessing the URI {0}.";

		private static readonly IDynamicPagePrinter dynamicPagePrinter = SpringContext.Current.GetObject<IDynamicPagePrinter>("DynamicPageHtmlPrinter");
		private static readonly IPermissionBridge permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();

		private IRequestHandler requestHandler;

		bool IHttpHandler.IsReusable { get { return true; } }

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			try
			{
				this.ProcessRequest(context);

				context.Response.StatusCode = (int)HttpStatusCode.OK;
			}
			catch (ThreadAbortException)
			{
			}
			catch (ValidationException exp)
			{
				throw new BadRequestException(exp);
			}
			catch (UnauthorizedAccessException)
			{
				throw;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw new InternalServerErrorException(exp);
			}
		}

		private void ProcessRequest(HttpContext context)
		{
			if (Kit.IsEmpty(QueryStringUtility.ObjectId))
				throw new BadRequestException("The query string parameter ObjectId is not specified.");

			string objectId = QueryStringUtility.ObjectId;
			IDynamicPage dynamicPageService = null;
			try
			{
				dynamicPageService = DynamicPageContext.Current.GetDynamicPage(objectId, false, this.GetRequestHandler());
			}
			catch (ConfigurationErrorsException)
			{
				throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "The query string parameter ObjectId \"{0}\" is invalid. Please be caution the parameter is case sensitive.", objectId));
			}

			if (!permissionBridge.HasPermission(dynamicPageService.Configuration.PermissionValue))
				throw new UnauthorizedException(string.Format(CultureInfo.InvariantCulture, @"The user doesn't have the permission to print the page with permission value ""{0}"".", dynamicPageService.Configuration.PermissionValue));

			DynamicPagePrintResult printResult = dynamicPagePrinter.Print(dynamicPageService, HttpContext.Current.Request.QueryString);
			WebUtility.RedirectToPage(printResult.Result);
		}

		private IRequestHandler GetRequestHandler()
		{
			if (this.requestHandler == null)
			{
				ProxyFactory proxyFactory = new ProxyFactory(this);
				proxyFactory.AddInterface(typeof(IRequestHandler));
				proxyFactory.AddIntroduction(new DefaultIntroductionAdvisor(new HttpRequestHandler()));
				proxyFactory.ProxyTargetType = true;
				this.requestHandler = proxyFactory.GetProxy() as IRequestHandler;
			}

			return this.requestHandler;
		}
	}
}
