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

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Status of user login to system
	/// </summary>
	[Flags]
	public enum LoginResults
	{
		/// <summary>
		/// Invalid user name or password
		/// </summary>
		InvalidCredential,

		/// <summary>
		/// Successful
		/// </summary>
		Successful,

		/// <summary>
		/// Organization which the user belongs to is unavailable.
		/// </summary>
		InvalidOrganization,

		/// <summary>
		/// The user is locked out by continuous failed password attempts.
		/// </summary>
		LockedOut,
	}
}

