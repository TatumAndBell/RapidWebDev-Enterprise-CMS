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
using System.Web.UI;
using NUnit.Framework;
using RapidWebDev.Common;
using RapidWebDev.Mocks;
using RapidWebDev.Mocks.UIMocks;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Web.DynamicPage;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class RoleManagementTest
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
        public void TestQuery()
        {
            RoleManagement page = new RoleManagement();

            DynamicPageProxy proxy = new DynamicPageProxy(page);

            IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

            IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/RoleManagement/DynamicPage.svc?Domain=Department");
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

                #region bind query varibles
                QueryParameterExpression expression = new QueryParameterExpression("RoleName", QueryFieldOperators.Equal, "superRole" + surfix);

                SortExpression sort = new SortExpression("RoleName");

                QueryParameter parameters = new QueryParameter()
                {
                    //Expressions = express,
                    PageIndex = 0,
                    PageSize = 10,
                    SortExpression = sort
                };

                parameters.Expressions.Add(expression);

                QueryResults results = proxy.Query(parameters);

                foreach (var result in results.Results)
                {
                    Assert.AreEqual(DataBinder.Eval(result, "RoleName"), "superRole" + surfix);
                }
                #endregion
            }
        }
    }
}