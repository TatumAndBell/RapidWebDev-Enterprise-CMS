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

namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class UserManagementTest
    {
        private List<Guid> createdObjectIds = new List<Guid>();
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
        }


        [Test]
        public void TestQuery()
        {
            UserManagement page = new UserManagement();

            DynamicPageProxy proxy = new DynamicPageProxy(page);

            IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
            IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/UserManagement/DynamicPage.svc?Domain=Department");

                #region Create Data
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
                    UserName = "Eunge"+surfix
                };
                membershipApi.Save(obj, "password1", null);
                createdObjectIds.Add(obj.UserId);

                #endregion

                #region query
                QueryParameterExpression expression = new QueryParameterExpression("UserName", QueryFieldOperators.Equal, "Eunge" + surfix);

                SortExpression sort = new SortExpression("UserName");

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
                    Assert.AreEqual(DataBinder.Eval(result, "UserName"), "Eunge" + surfix);
                }
                #endregion
            }

        }
    }
}
