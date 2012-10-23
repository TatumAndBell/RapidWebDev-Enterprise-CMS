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
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;

using NUnit.Framework;
using System.Web.UI;

namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class ConcreteDataManagementTest
    {
        List<Guid> ids = new List<Guid>();
        [TearDown]
        public void CleanUp()
        {
            IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();
            foreach (var id in ids)
            {
                var obj = concreteDataApi.GetById(id);
                obj.DeleteStatus = DeleteStatus.Deleted;
                concreteDataApi.Save(obj);
            }
        }

        [Test]
        public void TestQuery()
        {
            ConcreteDataManagement page = new ConcreteDataManagement();

            DynamicPageProxy proxy = new DynamicPageProxy(page);

            IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                #region Create Data
                Guid guid = Guid.NewGuid();
                string surfix = guid.ToString().Substring(0, 5);
                ConcreteDataObject testConcreteData = new ConcreteDataObject()
                {
                    Name = "concrete" + surfix,
                    Value = "concrete" + surfix,
                    Type = "Department",
                    DeleteStatus = DeleteStatus.NotDeleted
                };

                concreteDataApi.Save(testConcreteData);
                ids.Add(testConcreteData.ConcreteDataId);
                #endregion


                httpEnv.SetRequestUrl(@"/ConcreteDataDetailPanel/DynamicPage.svc?ConcreteDataType=Department");

                QueryParameterExpression expression = new QueryParameterExpression("Name", QueryFieldOperators.Equal, "concrete" + surfix);

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
                    Assert.AreEqual(DataBinder.Eval(result, "Name"), "concrete" + surfix);
                }
            }
        
        }
    }
}
