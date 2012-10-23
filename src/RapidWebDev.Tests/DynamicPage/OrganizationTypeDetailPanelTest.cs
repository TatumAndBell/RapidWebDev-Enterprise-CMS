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
using System.Web.UI.WebControls;

using RapidWebDev.ExtensionModel.Web.DynamicPage;
using RapidWebDev.Mocks.UIMocks;
using RapidWebDev.Mocks;
using RapidWebDev.Platform.Web.DynamicPage;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;

using NUnit.Framework;

namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class OrganizationTypeDetailPanelTest
    {
        List<Guid> ids = new List<Guid>();

        [TearDown]
        public void CleanUp()
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
            foreach (var id in ids)
            {
                OrganizationTypeObject obj = organizationApi.GetOrganizationType(id);
                obj.DeleteStatus = DeleteStatus.Deleted;
                organizationApi.Save(obj);
            }
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                var organizationsToDelete = ctx.OrganizationTypes.Where(org => ids.ToArray().Contains(org.OrganizationTypeId));


                ctx.OrganizationTypes.DeleteAllOnSubmit(organizationsToDelete);

                ctx.SubmitChanges();
                ids.Clear();
            }
        }
        [Test, Description("Test the Create Method in OrganizationTypeDetailPanel")]
        public void TestCreate()
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            OrganizationTypeDetailPanel page = new OrganizationTypeDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/OrganizationTypeDetailPanel/DynamicPage.svc?Domain=Department");

                #region bind web control
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);

                RapidWebDev.UI.Controls.TextBox TextBoxName = new RapidWebDev.UI.Controls.TextBox();
                TextBoxName.Text = "OrganizationTypeTest" + surfix;
                proxy.Set("TextBoxName", TextBoxName);

                RapidWebDev.UI.Controls.TextBox TextBoxDescription = new RapidWebDev.UI.Controls.TextBox();
                TextBoxDescription.Text = "OrganizationTypeTest" + surfix;
                proxy.Set("TextBoxDescription", TextBoxDescription);

                proxy.Set("DropDownListDomain", null);
                #endregion
                string entityId = proxy.Create();

                ids.Add(new Guid(entityId));

                Assert.AreEqual(organizationApi.GetOrganizationType(new Guid(entityId)).Description, "OrganizationTypeTest" + surfix);
            }

        }

        [Test, Description("Test the Update Method in OrganizationTypeDetailPanel")]
        public void TestUpdate() 
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            OrganizationTypeDetailPanel page = new OrganizationTypeDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/OrganizationTypeDetailPanel/DynamicPage.svc?Domain=Department");

                #region Create Data
                 Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);
                OrganizationTypeObject obj = new OrganizationTypeObject() 
                {
                    Name = "OrganizationTypeTestUpdate" + surfix,
                    Description = "OrganizationTypeTest" + surfix,
                    Predefined = false,
                    Domain = "Department",
                    DeleteStatus = DeleteStatus.NotDeleted
                };

                organizationApi.Save(obj);

                ids.Add(obj.OrganizationTypeId);
                #endregion
                #region Bind Web Control

                RapidWebDev.UI.Controls.TextBox TextBoxName = new RapidWebDev.UI.Controls.TextBox();
                TextBoxName.Text = "OrganizationTypeTestUpdate" + surfix;
                proxy.Set("TextBoxName", TextBoxName);

                RapidWebDev.UI.Controls.TextBox TextBoxDescription = new RapidWebDev.UI.Controls.TextBox();
                TextBoxDescription.Text = "OrganizationTypeTestUpdate" + surfix;
                proxy.Set("TextBoxDescription", TextBoxDescription);

                DropDownList DropDownListDomain = new DropDownList();
                var typeData = new string[] { "Department" };
                DropDownListDomain.DataSource = typeData;
                DropDownListDomain.DataBind();
                DropDownListDomain.SelectedIndex = 0;
                proxy.Set("DropDownListDomain", DropDownListDomain);

                #endregion

                proxy.Update(obj.OrganizationTypeId.ToString());

                Assert.AreEqual(obj.Description, "OrganizationTypeTest" + surfix);

                obj = organizationApi.GetOrganizationType(obj.OrganizationTypeId);

                Assert.AreEqual(obj.Description, "OrganizationTypeTestUpdate" + surfix);


            }
        }
    }
}
