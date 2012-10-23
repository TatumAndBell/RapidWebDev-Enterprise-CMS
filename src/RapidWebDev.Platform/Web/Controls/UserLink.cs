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
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.Platform.Properties;
using RapidWebDev.UI.Controls;

namespace RapidWebDev.Platform.Web.Controls
{
    /// <summary>
	/// HyperLink to User Profile page.
    /// </summary>
    public class UserLink : HyperLink
    {
		private static IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
		private static IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

		private const string UserProfileUrlTemplate = "~/UserManagement/DetailPanel.svc?entityid={0}&rendermode=View&stamp={1}&domain={2}";

		/// <summary>
		/// Sets/gets user id.
		/// </summary>
		public string UserId 
		{ 
			get { return this.ViewState["UserId"] as string; } 
			set { this.ViewState["UserId"] = value; } 
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (!string.IsNullOrEmpty(this.UserId))
			{
				try
				{
					Guid userId = new Guid(this.UserId);
					IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
					UserObject userObject = membershipApi.Get(userId);
					if (userObject != null)
					{
						string userLinkUrl = this.ResolveUserLinkUrl();
						string windowFrameObjectJs = ModalWindowHandler.FormatVariableDeclaration(Resources.UserProfile, userLinkUrl, false, true, false, 920, 600, "");

						this.Text = userObject.ToString();
						this.NavigateUrl = HttpContext.Current.Request.Url.ToString() + "#";
						this.Attributes["onclick"] = windowFrameObjectJs;
					}
				}
				catch
				{
				}
			}
		}

		private string ResolveUserLinkUrl()
		{
			Guid userIdValue = new Guid(this.UserId);
			UserObject userObject = membershipApi.Get(userIdValue);
			OrganizationObject orgObject = organizationApi.GetOrganization(userObject.OrganizationId);
			OrganizationTypeObject orgType = organizationApi.GetOrganizationType(orgObject.OrganizationTypeId);

			return string.Format(CultureInfo.InvariantCulture, UserProfileUrlTemplate, this.UserId, DateTime.UtcNow.Ticks, orgType.Domain);
		}

		/// <summary>
		/// Build HTML fragment which contains a link to user profile page.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static string BuildUserLink(string userId)
		{
			Guid userIdValue = new Guid(userId);
			UserObject userObject = membershipApi.Get(userIdValue);
			OrganizationObject orgObject = organizationApi.GetOrganization(userObject.OrganizationId);
			OrganizationTypeObject orgType = organizationApi.GetOrganizationType(orgObject.OrganizationTypeId);

			string pageUrl = string.Format(CultureInfo.InvariantCulture, UserProfileUrlTemplate, userId, DateTime.UtcNow.Ticks, orgType.Domain);
			string invokedJs = ModalWindowHandler.FormatVariableDeclaration(Resources.UserProfile, pageUrl, false, true, false, 960, 560, "");
			return string.Format(CultureInfo.InvariantCulture, @"<a href=""#"" onclick=""{1}"">{0}</a>", userObject.DisplayName, invokedJs);
		}
    }
}
