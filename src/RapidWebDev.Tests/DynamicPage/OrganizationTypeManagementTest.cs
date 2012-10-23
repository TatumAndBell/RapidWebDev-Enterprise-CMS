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
namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class OrganizationTypeManagementTest
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

        [Test, Description("Test the Query Method")]
        public void TestQuery()
        {
            OrganizationTypeManagement page = new OrganizationTypeManagement();

            DynamicPageProxy proxy = new DynamicPageProxy(page);

            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/OrganizationTypeManagement/DynamicPage.svc?Domain=Department");
                #region Create Data
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);
                OrganizationTypeObject testOrganizationType = new OrganizationTypeObject()
                {
                    Name = "78903" + surfix,
                    Description = "testOrganizationTypeUpdate" + surfix,
                    Domain = "Department",
                    Predefined = false,
                    DeleteStatus = DeleteStatus.NotDeleted
                };

                organizationApi.Save(testOrganizationType);
                ids.Add(testOrganizationType.OrganizationTypeId);
                #endregion


                QueryParameterExpression expression = new QueryParameterExpression("Name", QueryFieldOperators.Equal, "78903" + surfix);

                SortExpression sort = new SortExpression("Name");

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
                    Assert.AreEqual(DataBinder.Eval(result, "Name"), "78903" + surfix);
                }
            }
 
        }
    }
}
