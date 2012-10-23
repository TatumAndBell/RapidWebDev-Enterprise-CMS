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
using W = System.Web.UI.WebControls;

using RapidWebDev.ExtensionModel.Web.DynamicPage;
using RapidWebDev.Mocks.UIMocks;
using RapidWebDev.Mocks;
using RapidWebDev.Platform.Web.DynamicPage;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;

using NUnit.Framework;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using System.Web.UI;

namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class HierarchyDataManagementTest
    {
        List<Guid> ids = new List<Guid>();
        [Test, Description("")]
        public void TestQuery()
        {
            HierarchyDataManagement page = new HierarchyDataManagement();

            DynamicPageProxy proxy = new DynamicPageProxy(page);

            IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);

                HierarchyDataObject obj = new HierarchyDataObject() 
                {
                    
                    HierarchyType = "Department",
                    Name = "HierarchyType" + surfix,
                    Description = "HierarchyType" + surfix
                    
                };

                hierarchyApi.Save(obj);
                httpEnv.SetRequestUrl(@"/HierarchyDataDetailPanel/DynamicPage.svc?HierarchyType=Department");

                QueryParameterExpression expression = new QueryParameterExpression("Name", QueryFieldOperators.Equal, "HierarchyType" + surfix);

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
                    Assert.AreEqual(DataBinder.Eval(result, "Name"), "HierarchyType" + surfix);
                }
            }

        }
    }
}
