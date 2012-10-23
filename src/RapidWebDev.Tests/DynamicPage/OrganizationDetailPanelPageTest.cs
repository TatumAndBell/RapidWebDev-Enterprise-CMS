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
    public class OrganizationDetailPanelPageTest
    {
        List<Guid> createdOrganizationIds = new List<Guid>();
        List<Guid> createdOrganizationTypeIds = new List<Guid>();

        [TearDown]
        public void CleanUp()
        {

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
        [Test, Description("Test the Create Method in OrganizationDetailPanel")]
        public void TestCreate()
        {
            
            OrganizationDetailPanel testpage = new OrganizationDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(testpage);

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);

                #region Setup the pre-required data
                //Setup the right URL
                httpEnv.SetRequestUrl("/OrganizationDetailPanel/DynamicPage.svc?Domain=Department");

                //Binding the required web controls
                TextBox organizationCode = new TextBox();
                organizationCode.Text = "123456" + surfix;
                proxy.Set("TextBoxOrganizationCode", organizationCode);

                TextBox organizationName = new TextBox();
                organizationName.Text = "testOrganization" + surfix;
                proxy.Set("TextBoxOrganizationName", organizationName);

                DropDownList DropDownListOrganizationType = new DropDownList();
                var typeData = organizationApi.FindOrganizationTypes(new List<string>() { "Department" }).Select(x => x.OrganizationTypeId);


                DropDownListOrganizationType.Items.Clear();
                DropDownListOrganizationType.DataSource = typeData;
                DropDownListOrganizationType.DataBind();
                DropDownListOrganizationType.SelectedIndex = 1;

                proxy.Set("DropDownListOrganizationType", DropDownListOrganizationType);

                Array statusData = new string[] { "Enabled", "Disabled", "Pending" };
                RadioButtonList RadioButtonListOrganizationStatus = new RadioButtonList();
                RadioButtonListOrganizationStatus.DataSource = statusData;
                RadioButtonListOrganizationStatus.DataBind();
                RadioButtonListOrganizationStatus.SelectedIndex = 0;
                proxy.Set("RadioButtonListOrganizationStatus", RadioButtonListOrganizationStatus);
                #endregion

                //call the Create Method
                string entityId = proxy.Create();
                //Get the created object
                OrganizationObject organization = organizationApi.GetOrganization(new Guid(entityId));

                Assert.AreEqual(organization.OrganizationName, "testOrganization"+surfix);

                createdOrganizationIds.Add(new Guid(entityId));
            }
        }

        [Test, Description("Test the Update Method in OrganizationDetailPanel")]
        public void TestUpdate()
        {
            OrganizationDetailPanel testpage = new OrganizationDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(testpage);

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            Guid guid = Guid.NewGuid();

            string surfix = guid.ToString().Substring(0, 5);

            using (var httpEnv = new HttpEnvironment())
            {
                #region Setup the pre-required data
                //Setup the right URL
                httpEnv.SetRequestUrl("/OrganizationDetailPanel/DynamicPage.svc?Domain=Department");

                OrganizationObject testOrganization = new OrganizationObject()
                {
                    OrganizationCode = "78903"+surfix,
                    OrganizationName = "testOrganizationUpdate" + surfix,
                    OrganizationTypeId = organizationApi.FindOrganizationTypes(new List<string>() { "Department" }).Select(x => x.OrganizationTypeId).FirstOrDefault(),
                    Status = OrganizationStatus.Enabled
                };

                organizationApi.Save(testOrganization);
                createdOrganizationIds.Add(testOrganization.OrganizationId);

                #endregion

                OrganizationObject organization = organizationApi.GetOrganizationByName("testOrganizationUpdate"+surfix);

                #region Setup the Updated Code
                //Binding the required web controls
                TextBox organizationCode = new TextBox();
                organizationCode.Text = "78903" + surfix;
                proxy.Set("TextBoxOrganizationCode", organizationCode);

                TextBox organizationName = new TextBox();
                organizationName.Text = "OrganziationTest" + surfix;
                proxy.Set("TextBoxOrganizationName", organizationName);

                DropDownList DropDownListOrganizationType = new DropDownList();
                var typeData = organizationApi.FindOrganizationTypes(new List<string>() { "Department" }).Select(x => x.OrganizationTypeId);


                DropDownListOrganizationType.Items.Clear();
                DropDownListOrganizationType.DataSource = typeData;
                DropDownListOrganizationType.DataBind();
                DropDownListOrganizationType.SelectedIndex = 1;

                proxy.Set("DropDownListOrganizationType", DropDownListOrganizationType);

                Array statusData = new string[] { "Enabled", "Disabled", "Pending" };
                RadioButtonList RadioButtonListOrganizationStatus = new RadioButtonList();
                RadioButtonListOrganizationStatus.DataSource = statusData;
                RadioButtonListOrganizationStatus.DataBind();
                RadioButtonListOrganizationStatus.SelectedIndex = 0;
                proxy.Set("RadioButtonListOrganizationStatus", RadioButtonListOrganizationStatus);
                #endregion
                proxy.Update(organization.OrganizationId.ToString());


                OrganizationObject organizationUpdated = organizationApi.GetOrganization(organization.OrganizationId);

                Assert.AreNotEqual(organizationUpdated.OrganizationName, "testOrganization" + surfix);
                Assert.AreEqual(organizationUpdated.OrganizationName, "OrganziationTest" + surfix);
            }
        }

        [Test, Description("Test the LoadWritableEntity Method in OrganizationDetailPanel")]
        public void TestLoadWritableEntity()
        {
            OrganizationDetailPanel testpage = new OrganizationDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(testpage);

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            Guid guid = Guid.NewGuid();

            string surfix = guid.ToString().Substring(0, 5);

            using (var httpEnv = new HttpEnvironment())
            {
                #region Setup the pre-required data
                //Setup the right URL
                httpEnv.SetRequestUrl("/OrganizationDetailPanel/DynamicPage.svc?Domain=Department");

                OrganizationObject testOrganization = new OrganizationObject()
                {
                    OrganizationCode = "78901" + surfix,
                    OrganizationName = "testOrganization1" + surfix,
                    OrganizationTypeId = organizationApi.FindOrganizationTypes(new List<string>() { "Department" }).Select(x => x.OrganizationTypeId).FirstOrDefault(),
                    Status = OrganizationStatus.Enabled
                };
                organizationApi.Save(testOrganization);

                #endregion

                OrganizationObject organization = organizationApi.GetOrganizationByName("testOrganization1" + surfix);

                proxy.LoadWritableEntity(organization.OrganizationId.ToString());

                Assert.AreEqual(organization.OrganizationName, ((TextBox)proxy.Get("TextBoxOrganizationName")).Text);

                createdOrganizationIds.Add(testOrganization.OrganizationId);
            }
        }

        [Test, Description("Test the LoadReadOnlyEntity Method in OrganizationDetailPanel")]
        public void TestLoadReadOnlyEntity()
        {
            OrganizationDetailPanel testpage = new OrganizationDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(testpage);

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            Guid guid = Guid.NewGuid();

            string surfix = guid.ToString().Substring(0, 5);

            using (var httpEnv = new HttpEnvironment())
            {
                #region Setup the pre-required data
                //Setup the right URL
                httpEnv.SetRequestUrl("/OrganizationDetailPanel/DynamicPage.svc?Domain=Department");

                OrganizationObject testOrganization = new OrganizationObject()
                {
                    OrganizationCode = "78902" + surfix,
                    OrganizationName = "testOrganization2" + surfix,
                    OrganizationTypeId = organizationApi.FindOrganizationTypes(new List<string>() { "Department" }).Select(x => x.OrganizationTypeId).FirstOrDefault(),
                    Status = OrganizationStatus.Enabled
                };
                organizationApi.Save(testOrganization);

                #endregion

                OrganizationObject organization = organizationApi.GetOrganizationByName("testOrganization2" + surfix);

                proxy.LoadReadOnlyEntity(organization.OrganizationId.ToString());

                Assert.AreEqual(organization.OrganizationName, ((TextBox)proxy.Get("TextBoxOrganizationName")).Text);

                createdOrganizationIds.Add(testOrganization.OrganizationId);
            }
        }

    }
}
