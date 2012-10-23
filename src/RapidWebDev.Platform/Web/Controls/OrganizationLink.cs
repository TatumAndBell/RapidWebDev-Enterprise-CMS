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
using RapidWebDev.Platform;
using RapidWebDev.UI.Controls;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Web.Controls
{
    /// <summary>
	/// HyperLink to Organization Profile page.
    /// </summary>
	public class OrganizationLink : HyperLink
    {
		private static IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

		private const string OrganizationProfileUrlTemplate = "~/OrganizationManagement/DetailPanel.svc?entityid={0}&rendermode=View&stamp={1}&domain={2}";

		/// <summary>
		/// Sets/gets organization id.
		/// </summary>
		public string OrganizationId 
		{ 
			get { return this.ViewState["OrganizationId"] as string; }
			set { this.ViewState["OrganizationId"] = value; }
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (!string.IsNullOrEmpty(this.OrganizationId))
			{
				IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
				try
				{
					Guid organizationId = new Guid(this.OrganizationId);
					OrganizationObject organizationObject = organizationApi.GetOrganization(organizationId);
					if (organizationObject != null)
					{
						string organizationLinkUrl = this.ResolveOrganizationLinkUrl();
						string windowFrameObjectJs = ModalWindowHandler.FormatVariableDeclaration(Resources.OrganizationProfile, organizationLinkUrl, false, true, false, 920, 600, "");

						this.Text = organizationObject.ToString();
						this.NavigateUrl = HttpContext.Current.Request.Url.ToString() + "#";
						this.Attributes["onclick"] = windowFrameObjectJs;
					}
				}
				catch
				{
				}
			}
		}

		private string ResolveOrganizationLinkUrl()
		{
			Guid organizationIdValue = new Guid(this.OrganizationId);
			OrganizationObject org = organizationApi.GetOrganization(organizationIdValue);
			OrganizationTypeObject orgType = organizationApi.GetOrganizationType(org.OrganizationTypeId);

			string organizationProfileUrl = string.Format(CultureInfo.InvariantCulture, OrganizationProfileUrlTemplate, this.OrganizationId, DateTime.UtcNow.Ticks, orgType.Domain);
			return Kit.ResolveAbsoluteUrl(organizationProfileUrl);
		}

		/// <summary>
		/// Build HTML fragment which contains a link to organization profile page.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <returns></returns>
		public static string BuildOrganizationLink(string organizationId)
		{
			Guid organizationIdValue = new Guid(organizationId);
			OrganizationObject org = organizationApi.GetOrganization(organizationIdValue);
			OrganizationTypeObject orgType = organizationApi.GetOrganizationType(org.OrganizationTypeId);

			string pageUrl = string.Format(CultureInfo.InvariantCulture, OrganizationProfileUrlTemplate, organizationId, DateTime.UtcNow.Ticks, orgType.Domain);
			string invokedJs = ModalWindowHandler.FormatVariableDeclaration(Resources.OrganizationProfile, pageUrl, false, true, false, 960, 560, "");
			return string.Format(CultureInfo.InvariantCulture, @"<a href=""#"" onclick=""{1}"">{0}</a>", org.OrganizationName, invokedJs);
		}
    }
}
