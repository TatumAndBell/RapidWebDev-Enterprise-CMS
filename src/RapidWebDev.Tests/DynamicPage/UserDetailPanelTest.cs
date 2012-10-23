/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.long.yi@RapidWebDev.org

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

using RapidWebDev.UI.DynamicPages;
using RapidWebDev.Platform.Web.DynamicPage;
using RapidWebDev.Mocks.UIMocks;
using RapidWebDev.Mocks;
using RapidWebDev.UI.Services;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Platform;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.UI.DynamicPages.Configurations;

using NUnit.Framework;
using System.Web.UI;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Web.Controls;
using System.Web.UI.WebControls;
using RapidWebDev.ExtensionModel.Web.Controls;

namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class UserDetailPanelTest
    {
        private List<Guid> createdObjectIds = new List<Guid>();

        List<Guid> createdOrganizationIds = new List<Guid>();
        List<Guid> createdOrganizationTypeIds = new List<Guid>();

        [TearDown]
        public void TearDown()
        {
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                foreach (Guid createdObjectId in createdObjectIds)
                {
                    ctx.Memberships.Delete(m => m.UserId == createdObjectId);
                    ctx.Users.Delete(u => u.UserId == createdObjectId);
                    ctx.SubmitChanges();
                }
            }

            createdObjectIds.Clear();
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            foreach (Guid temp in createdOrganizationIds)
            {
                OrganizationObject obj = organizationApi.GetOrganization(temp);
                obj.Status = OrganizationStatus.Disabled;
                organizationApi.Save(obj);
            }
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                var organizationsToDelete = ctx.Organizations.Where(org => createdOrganizationIds.ToArray().Contains(org.OrganizationId));


                ctx.Organizations.DeleteAllOnSubmit(organizationsToDelete);

                ctx.SubmitChanges();
                createdOrganizationIds.Clear();
            }
        }

        [Test]
        public void TestCreate()
        {
            UserDetailPanel page = new UserDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/UserDetailPanel/DynamicPage.svc?Domain=Department");

                #region bind web control
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);

                OrganizationSelector OrganizationSelector = new OrganizationSelector();
                //IList<OrganizationObject> ds = new List<OrganizationObject>();
                OrganizationObject organization = new OrganizationObject()
                {
                    OrganizationCode = "123456" + surfix,
                    OrganizationName = "org" + surfix,
                    Status = OrganizationStatus.Enabled,
                    OrganizationTypeId = organizationApi.FindOrganizationTypes(new List<string>() { "Department" }).Select(x => x.OrganizationTypeId).FirstOrDefault(),
                    Description = "organ"
                };

                organizationApi.Save(organization);
                //ds.Add(organization);
                createdOrganizationIds.Add(organization.OrganizationId);

                OrganizationSelector.SelectedOrganization = organization;

                proxy.Set("OrganizationSelector", OrganizationSelector);

                TextBox TextBoxUserName = new TextBox();
                TextBoxUserName.Text = "Eunge" + surfix;
                proxy.Set("TextBoxUserName", TextBoxUserName);

                TextBox TextBoxPassword = new TextBox();
                TextBoxPassword.Text = "Password" + surfix;
                proxy.Set("TextBoxPassword", TextBoxPassword);

                TextBox TextBoxConfirmPassword = new TextBox();
                TextBoxConfirmPassword.Text = "Password" + surfix;
                proxy.Set("TextBoxConfirmPassword", TextBoxConfirmPassword);

                TextBox TextBoxDisplayName = new TextBox();
                TextBoxDisplayName.Text = "Eunge" + surfix;
                proxy.Set("TextBoxDisplayName", TextBoxConfirmPassword);

                TextBox TextBoxEmail = new TextBox();
                TextBoxEmail.Text = "Eunge@hotmail.com";
                proxy.Set("TextBoxEmail", TextBoxEmail);

                TextBox TextBoxMobile = new TextBox();
                TextBoxMobile.Text = "13456789009";
                proxy.Set("TextBoxMobile", TextBoxMobile);

                ExtensionDataForm UserExtensionDataForm = null;
                proxy.Set("UserExtensionDataForm", UserExtensionDataForm);

                TextBox TextBoxComment = new TextBox();
                TextBoxComment.Text = "13456789009" + surfix;
                proxy.Set("TextBoxComment", TextBoxComment);

                Array statusData = new string[] { "true", "false" };
                RadioButtonList RadioButtonListStatus = new RadioButtonList();
                RadioButtonListStatus.DataSource = statusData;
                RadioButtonListStatus.DataBind();
                RadioButtonListStatus.SelectedIndex = 0;
                proxy.Set("RadioButtonListStatus", RadioButtonListStatus);

                TextBox TextBoxCreationDate = new TextBox();
				TextBoxCreationDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxCreationDate", TextBoxCreationDate);

                TextBox TextBoxLastLoginDate = new TextBox();
				TextBoxLastLoginDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLastLoginDate", TextBoxLastLoginDate);


                TextBox TextBoxLastActivityDate = new TextBox();
				TextBoxLastActivityDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLastActivityDate", TextBoxLastActivityDate);

                TextBox TextBoxLockedOutDate = new TextBox();
				TextBoxLockedOutDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLockedOutDate", TextBoxLockedOutDate);

                TextBox TextBoxLastPasswordChangedDate = new TextBox();
				TextBoxLastPasswordChangedDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLastPasswordChangedDate", TextBoxLastPasswordChangedDate);

                TextBox TextBoxLastUpdatedDate = new TextBox();
				TextBoxLastUpdatedDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLastUpdatedDate", TextBoxLastUpdatedDate);

                PermissionTreeView @PermissionTreeView = null;
                proxy.Set("PermissionTreeView", @PermissionTreeView);

                #endregion
                string entity = proxy.Create();
                createdObjectIds.Add(new Guid(entity));

            }
        }

        [Test]
        public void TestUpdate()
        {
            UserDetailPanel page = new UserDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

            IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
            
            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/UserDetailPanel/DynamicPage.svc?Domain=Department");
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);

                UserObject obj = new UserObject() 
                {
                    OrganizationId = platformConfiguration.Organization.OrganizationId,
                    Comment = "IT specialist",
                    DisplayName = "Eunge Liu" + surfix,
                    Email = "eunge.liu@gmail.com",
                    IsApproved = true,
                    MobilePin = "137641855XX",
                    UserName = "Eunge" + surfix
                };

                membershipApi.Save(obj, "password1", null);
                createdObjectIds.Add(obj.UserId);

                OrganizationSelector OrganizationSelector = new OrganizationSelector();
                //IList<OrganizationObject> ds = new List<OrganizationObject>();
                OrganizationObject organization = new OrganizationObject()
                {
                    OrganizationCode = "123456" + surfix,
                    OrganizationName = "org" + surfix,
                    Status = OrganizationStatus.Enabled,
                    OrganizationTypeId = organizationApi.FindOrganizationTypes(new List<string>() { "Department" }).Select(x => x.OrganizationTypeId).FirstOrDefault(),
                    Description = "organ"
                };

                organizationApi.Save(organization);
                //ds.Add(organization);
                createdOrganizationIds.Add(organization.OrganizationId);

                OrganizationSelector.SelectedOrganization = organization;

                proxy.Set("OrganizationSelector", OrganizationSelector);

                TextBox TextBoxUserName = new TextBox();
                TextBoxUserName.Text = "Eunge" + surfix;
                proxy.Set("TextBoxUserName", TextBoxUserName);

                TextBox TextBoxPassword = new TextBox();
                TextBoxPassword.Text = "Password" + surfix;
                proxy.Set("TextBoxPassword", TextBoxPassword);

                TextBox TextBoxConfirmPassword = new TextBox();
                TextBoxConfirmPassword.Text = "Password" + surfix;
                proxy.Set("TextBoxConfirmPassword", TextBoxConfirmPassword);

                TextBox TextBoxDisplayName = new TextBox();
                TextBoxDisplayName.Text = "Eunge" + surfix;
                proxy.Set("TextBoxDisplayName", TextBoxConfirmPassword);

                TextBox TextBoxEmail = new TextBox();
                TextBoxEmail.Text = "Eunge@hotmail.com";
                proxy.Set("TextBoxEmail", TextBoxEmail);

                TextBox TextBoxMobile = new TextBox();
                TextBoxMobile.Text = "13456789009";
                proxy.Set("TextBoxMobile", TextBoxMobile);

                ExtensionDataForm UserExtensionDataForm = null;
                proxy.Set("UserExtensionDataForm", UserExtensionDataForm);

                TextBox TextBoxComment = new TextBox();
                TextBoxComment.Text = "13456789009" + surfix;
                proxy.Set("TextBoxComment", TextBoxComment);

                Array statusData = new string[] { "true", "false" };
                RadioButtonList RadioButtonListStatus = new RadioButtonList();
                RadioButtonListStatus.DataSource = statusData;
                RadioButtonListStatus.DataBind();
                RadioButtonListStatus.SelectedIndex = 0;
                proxy.Set("RadioButtonListStatus", RadioButtonListStatus);

                TextBox TextBoxCreationDate = new TextBox();
				TextBoxCreationDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxCreationDate", TextBoxCreationDate);

                TextBox TextBoxLastLoginDate = new TextBox();
				TextBoxLastLoginDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLastLoginDate", TextBoxLastLoginDate);


                TextBox TextBoxLastActivityDate = new TextBox();
				TextBoxLastActivityDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLastActivityDate", TextBoxLastActivityDate);

                TextBox TextBoxLockedOutDate = new TextBox();
				TextBoxLockedOutDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLockedOutDate", TextBoxLockedOutDate);

                TextBox TextBoxLastPasswordChangedDate = new TextBox();
				TextBoxLastPasswordChangedDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLastPasswordChangedDate", TextBoxLastPasswordChangedDate);

                TextBox TextBoxLastUpdatedDate = new TextBox();
				TextBoxLastUpdatedDate.Text = System.DateTime.UtcNow.ToShortTimeString();
                proxy.Set("TextBoxLastUpdatedDate", TextBoxLastUpdatedDate);

                PermissionTreeView @PermissionTreeView = null;
                proxy.Set("PermissionTreeView", @PermissionTreeView);


                proxy.Update(obj.UserId.ToString());
            }
        }
    }
}
