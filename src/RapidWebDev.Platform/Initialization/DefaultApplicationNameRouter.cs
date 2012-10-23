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

using System.Collections.Generic;
using System.ServiceModel;
using System.Web;
using RapidWebDev.Common;

namespace RapidWebDev.Platform.Initialization
{
	/// <summary>
	/// Default application name loader
	/// </summary>
	public class DefaultApplicationNameRouter : IApplicationNameRouter
	{
		/// <summary>
		/// Get application mapping dictionary. 
		/// The key is HttpContext.Current.Request.Url.Authority and value is application name.
		/// If application name is not configured, returns HttpContext.Current.Request.Url.Authority.
		/// </summary>
		public Dictionary<string, string> Mapping { get; set; }

		/// <summary>
		/// True to allow request on Uri authority which is not configured in the property Mapping.
		/// </summary>
		public bool AllowsAnonymousUriAuthority { get; set; }

		/// <summary>
		/// Get application name.
		/// </summary>
		/// <returns></returns>
		public string GetApplicationName()
		{
			string authority = null;
			if (HttpContext.Current != null)
				authority = HttpContext.Current.Request.Url.Authority;
			else if (OperationContext.Current != null && OperationContext.Current.Channel != null && OperationContext.Current.Channel.LocalAddress != null)
				authority = OperationContext.Current.Channel.LocalAddress.Uri.Authority;

			string returnValue;
			if (this.Mapping != null && this.Mapping.ContainsKey(authority))
				returnValue = this.Mapping[authority];
			else
			{
				if (!this.AllowsAnonymousUriAuthority)
					throw new BadRequestException();
				else
					returnValue = "localhost";
			}

			return returnValue;
		}
	}
}
