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
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace RapidWebDev.Common
{
	/// <summary>
	/// The interface is used to isolate logic applications in RapidWebDev. 
    /// With this interface, developers can know which application is in current execution context 
    /// and access Session and Temporary Variables (By Request) without coupling to ASP.NET HttpContext.Current.Session and HttpContext.Current.Items.
    /// And developers also can get basic authentication from this interface.
	/// </summary>
	public interface IApplicationContext
	{
		/// <summary>
		/// Gets current running application id.
		/// </summary>
		Guid ApplicationId { get; }
        
        /// <summary>
        /// gets context value by name. 
        /// It likes HttpSessionState which creates context among requests from a signed-on client.
        /// </summary>
        /// <returns></returns>
        IDictionary Session { get; }

		/// <summary>
		/// Gets temporary variables likes HttpContext.Current.Items
		/// The variables stored will be disposed after a request is completed so that developers can share variables in a request in web applications.
		/// </summary>
		IDictionary TempVariables { get; }

		/// <summary>
		/// Get current authenticated identity instance.
		/// </summary>
		IIdentity Identity { get; }

		/// <summary>
		/// Get the client IP address.
		/// </summary>
		string ClientIPAddress { get; }
	}
}