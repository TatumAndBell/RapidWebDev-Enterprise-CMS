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
using System.Security.Principal;
using RapidWebDev.Common;
using System.Collections;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Authentication context interface.
	/// </summary>
	public interface IAuthenticationContext : IApplicationContext
	{
		/// <summary>
		/// Get organization the current login user belongs to.
		/// </summary>
		OrganizationObject Organization { get; }

		/// <summary>
		/// Get current login user.
		/// </summary>
		UserObject User { get; }

		/// <summary>
		/// Login with user name and password.
		/// </summary>
		/// <param name="username">user name</param>
		/// <param name="password">password</param>
		/// <returns>returns status of login</returns>
		LoginResults Login(string username, string password);

		/// <summary>
		/// Logout the current user.
		/// </summary>
		void Logout();

		/// <summary>
		/// Track the user interaction.
		/// </summary>
		void Act();
	}
}
