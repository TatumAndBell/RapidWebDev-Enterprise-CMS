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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// The HTTP request handler which invokes the events of dynamic main, aggregate and detail page.
	/// </summary>
	public class HttpRequestHandler : IRequestHandler
	{
		#region IRequestHandler Members

		/// <summary>
		/// Whether the request is a postback.
		/// </summary>
		public bool IsPostBack
		{
			get 
			{
				if (HttpContext.Current == null)
					throw new InvalidOperationException("HttpRequestHandler doesn't work in non-web environment.");

				Page webpage = HttpContext.Current.Handler as Page;
				if (webpage == null) return false;

				return webpage.IsPostBack;
			}
		}

		/// <summary>
		/// Whether the request is in asynchronous way.
		/// </summary>
		public bool IsAsynchronous
		{
			get 
			{
				if (HttpContext.Current == null)
					throw new InvalidOperationException("HttpRequestHandler doesn't work in non-web environment.");

				Page webpage = HttpContext.Current.Handler as Page;
				if (webpage == null) return false;

				ScriptManager scriptManager = ScriptManager.GetCurrent(webpage);
				if (scriptManager == null) return false;

				return scriptManager.IsInAsyncPostBack;
			}
		}

		/// <summary>
		/// Gets the parameters to the request.
		/// </summary>
		public NameValueCollection Parameters 
		{
			get
			{
				if (HttpContext.Current == null)
					throw new InvalidOperationException("HttpRequestHandler doesn't work in non-web environment.");

				return HttpContext.Current.Request.QueryString;
			}
		}

		#endregion
	}
}
