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
using System.Collections.Specialized;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using RapidWebDev.Common;
using System.Text;
using System.Security.Cryptography;

namespace RapidWebDev.Platform.Initialization
{
	/// <summary>
	/// Manages storage of membership information which extends SqlMembershipProvider to support SAAS infrastructure.
	/// </summary>
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class RwdSqlMembershipProvider : SqlMembershipProvider
	{
		/// <summary>
		/// Gets or sets the name of the application to store and retrieve membership information for.
		/// </summary>
		public override string ApplicationName
		{
			get { return SpringContext.Current.GetObject<IApplicationNameRouter>().GetApplicationName(); }
			set { }
		}
        public string EncodePassword(string pass, string saltBase64)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Convert.FromBase64String(saltBase64);
            byte[] dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }
	}
}

