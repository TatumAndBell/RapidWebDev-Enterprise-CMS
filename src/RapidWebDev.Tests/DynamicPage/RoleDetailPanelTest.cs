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
using System.Reflection;

using RapidWebDev.ExtensionModel.Web.DynamicPage;
using RapidWebDev.Mocks.UIMocks;
using RapidWebDev.Mocks;
using RapidWebDev.Platform.Web.DynamicPage;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;


using NUnit.Framework;
using RapidWebDev.Platform.Web.Controls;

namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class RoleDetailPanelTest
    {
        List<Guid> OrganizationTypeObjectIds = new List<Guid>();
        List<Guid> RoleIds = new List<Guid>();
        [TearDown]
        public void CleanUp()
        {
            IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            foreach (var id in OrganizationTypeObjectIds) 
            {
                OrganizationTypeObject obj = organizationApi.GetOrganizationType(id);
                obj.DeleteStatus = DeleteStatus.Deleted;
                organizationApi.Save(obj);
            }

            foreach (var id in RoleIds)
            {                
                roleApi.HardDelete(id);
            }

        }

        [Test]
        public void TestCreate()
        {
            IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

            IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();

            RoleDetailPanel page = new RoleDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/RoleDetailPanel/DynamicPage.svc?Domain=Department");
                UserObject current = membershipApi.Get("admin");
                httpEnv.SetSessionParaemeter("CurrentUser", current);

                #region create Data
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);
                OrganizationTypeObject obj = new OrganizationTypeObject()
                {
                    Name = "OrganizationTypeTest" + surfix,
                    Description = "OrganizationTypeTest" + surfix,
                    Predefined = false,
                    Domain = "Department",
                    DeleteStatus = DeleteStatus.NotDeleted
                };

                organizationApi.Save(obj);
                OrganizationTypeObjectIds.Add(obj.OrganizationTypeId);

                #endregion

                #region bind web control

                PermissionTreeView permissionTreeView = new PermissionTreeView();
                proxy.Set("PermissionTreeView", permissionTreeView);
                Type _type = typeof(PermissionTreeView);
                _type.GetMethod("OnInit", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(permissionTreeView, new object[] { new EventArgs() });
                IList<string> values = new List<string>();

                IList<PermissionConfig> permissions = permissionApi.FindPermissionConfig(current.UserId).ToList<PermissionConfig>();
                for (int i = 0; i < permissions.Count; i++)
                {
                    if ((permissions[i].Value == null) || (permissions[i].Value.Equals(String.Empty)))
                        values.Add("P"+i);
                    else
                        values.Add(permissions[i].Value);
                }
                permissionTreeView.CheckedValues = values;


                TextBox TextBoxName = new TextBox();
                TextBoxName.Text = "SuperRole" + surfix;
                proxy.Set("TextBoxName", TextBoxName);

                TextBox TextBoxDescription = new TextBox();
                TextBoxDescription.Text = "SuperRole" + surfix;
                proxy.Set("TextBoxDescription", TextBoxDescription);

                #endregion

                string entityId = proxy.Create();
                RoleIds.Add(new Guid(entityId));

            }
        }

        [Test]
        public void TestUpdate()
        {
            RoleDetailPanel page = new RoleDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

            IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/RoleDetailPanel/DynamicPage.svc?Domain=Department");
                UserObject current = membershipApi.Get("admin");
                httpEnv.SetSessionParaemeter("CurrentUser", current);

                #region Create Data
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);

                OrganizationTypeObject obj = new OrganizationTypeObject()
                {
                    Name = "OrganizationTypeTest" + surfix,
                    Description = "OrganizationTypeTest" + surfix,
                    Predefined = false,
                    Domain = "Department",
                    DeleteStatus = DeleteStatus.NotDeleted
                };

                organizationApi.Save(obj);
                OrganizationTypeObjectIds.Add(obj.OrganizationTypeId);


                RoleObject roleObj = new RoleObject()
                {
                    Domain = "Department",
                    RoleName = "superRole" + surfix,
                    Predefined = false
                };

                roleApi.Save(roleObj);

                RoleIds.Add(roleObj.RoleId);
                #endregion

                #region Bind Web Control
                TextBox TextBoxName = new TextBox();
                TextBoxName.Text = "SuperRole" + surfix;
                proxy.Set("TextBoxName", TextBoxName);

                TextBox TextBoxDescription = new TextBox();
                TextBoxDescription.Text = "SuperRoleUpdate" + surfix;
                proxy.Set("TextBoxDescription", TextBoxDescription);

                PermissionTreeView permissionTreeView = new PermissionTreeView();
                proxy.Set("PermissionTreeView", permissionTreeView);
                Type _type = typeof(PermissionTreeView);
                _type.GetMethod("OnInit", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(permissionTreeView, new object[] { new EventArgs() });
                IList<string> values = new List<string>();

                IList<PermissionConfig> permissions = permissionApi.FindPermissionConfig(current.UserId).ToList<PermissionConfig>();
                for (int i = 0; i < permissions.Count; i++)
                {
                    if ((permissions[i].Value == null) || (permissions[i].Value.Equals(String.Empty)))
                        values.Add("P" + i);
                    else
                        values.Add(permissions[i].Value);
                }
                permissionTreeView.CheckedValues = values;
                #endregion

                proxy.Update(roleObj.RoleId.ToString());

                Assert.AreEqual(roleApi.Get(roleObj.RoleId).Description, "SuperRoleUpdate" + surfix);


            }

        }
    }
}
