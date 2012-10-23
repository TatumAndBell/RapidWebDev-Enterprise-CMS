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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Platform;
using RapidWebDev.Common;

namespace RapidWebDev.Web.Templates
{
	public partial class Header : System.Web.UI.UserControl
	{
		private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (authenticationContext.Identity.IsAuthenticated)
			{
				this.CurrentLogOnUser.UserId = authenticationContext.User.UserId.ToString();
			}
		}
	}
}
