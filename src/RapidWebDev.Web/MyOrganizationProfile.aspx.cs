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
using System.Globalization;
using System.Web;
using System.Web.UI;
using RapidWebDev.Common;
using RapidWebDev.Platform;
using RapidWebDev.UI;
using RapidWebDev.Common.Web;

namespace RapidWebDev.Web
{
    public partial class MyOrganizationProfile : Page
    {
        private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
        private static IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

        protected void Page_Load(object sender, EventArgs e)
        {
			if (authenticationContext.Identity.IsAuthenticated)
			{
				OrganizationObject organization = authenticationContext.Organization;
				OrganizationTypeObject organizationType = organizationApi.GetOrganizationType(organization.OrganizationTypeId);
				if (organizationType == null)
					throw new ResourceNotFoundException(string.Format(CultureInfo.InvariantCulture, Resources.Membership.InvalidOrganizationTypeId, organization.OrganizationTypeId));

				string redirectPageUrl = string.Format(CultureInfo.InvariantCulture, "~/OrganizationManagement/DetailPanel.svc?entityid={0}&rendermode=View&stamp={1}&domain={2}&urlreferer={3}", organization.OrganizationId, DateTime.Now.Ticks, organizationType.Domain, HttpContext.Current.Request.Url);
				WebUtility.RedirectToPage(redirectPageUrl);
			}
        }
    }
}