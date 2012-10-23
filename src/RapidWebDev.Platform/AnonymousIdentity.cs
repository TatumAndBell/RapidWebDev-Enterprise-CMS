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
using System.Web;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Anonymous Identity implements IIdentity interface.
	/// </summary>
	internal class AnonymousIdentity : IIdentity
	{
		/// <summary>
		/// Construct AnonymousIdentity by user name
		/// </summary>
		/// <param name="userName"></param>
		internal AnonymousIdentity(string userName)
			: this(userName, "")
		{
		}

		/// <summary>
		/// Construct AnonymousIdentity by user name and authentication type.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="authenticationType"></param>
		internal AnonymousIdentity(string userName, string authenticationType)
		{
			this.Name = userName;
			this.AuthenticationType = authenticationType;
		}

		/// <summary>
		/// Gets authentication type
		/// </summary>
		public string AuthenticationType { get; private set; }

		/// <summary>
		/// Gets false always since this is anonymous identity.
		/// </summary>
		public bool IsAuthenticated { get { return false; } }

		/// <summary>
		/// Gets the authenticated user name.
		/// </summary>
		public string Name { get; private set; }
	}
}

