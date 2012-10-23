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
using NUnit.Framework;
using System.Net;
using RapidWebDev.RestfulServices.HierarchyServices;
using System.Collections.ObjectModel;


namespace RapidWebDev.RestfulServices.Test
{
    [TestFixture]
    public class HierarchyServiceTest
    {
        
        private const string GEOGRAPHY = "Geography";
        private HierarchyDataObject china;

        private string userName = TestServicesHelper.userName;
        private string password = TestServicesHelper.password;

        [SetUp]
        public void Setup()
        {
            china = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "China",CreatedDate = System.DateTime.Now };
            string serviceUri = @"/HierarchyService.svc/json/Save";
            string content = TestServicesHelper.GenerateJsonByType(china);
            string id = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content,null).Replace("\"", "");
            china.Id = new Guid(id);
        }

        [TearDown]
        public void CleanUp()
        {
            string serviceUri = string.Format("/HierarchyService.svc/json/HardDeleteHierarchyData/{0}",china.Id.ToString());
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

           

        }

        

        [Test, Description("test:GetHierarchyDataByIdJson(string hierarchyDataId),UriTemplate = json/GetHierarchyDataById/{hierarchyDataId}")]
        public void TestGetHierarchyDataByIdJson()
        {
            string serviceUri = @"/HierarchyService.svc/json/GetHierarchyDataById/" + china.Id;
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            Assert.AreEqual(china.Id, (TestServicesHelper.GenerateObjectByJson<HierarchyDataObject>(response) as HierarchyDataObject).Id);
        }

        [Test, Description("test:GetHierarchyDataByNameJson(string hierarchyType, string hierarchyDataName),UriTemplate = json/GetHierarchyDataByName/{hierarchyType}/{hierarchyDataName}")]
        public void TestGetHierarchyDataByNameJson()
        {
            string serviceUri = string.Format("/HierarchyService.svc/json/GetHierarchyDataByName/{0}/{1}", GEOGRAPHY, "China" );
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            Assert.AreEqual(china.Id, (TestServicesHelper.GenerateObjectByJson<HierarchyDataObject>(response) as HierarchyDataObject).Id);
        }

        [Test,Description("test: GetImmediateChildrenJson(string hierarchyType, string parentHierarchyDataId),UriTemplate = json/GetImmediateChildren/{hierarchyType}/{parentHierarchyDataId}")]
        public void TestGetImmediateChildrenJson()
        {
            string serviceUri = string.Format("/HierarchyService.svc/json/GetImmediateChildren/{0}/{1}", GEOGRAPHY, china.Id.ToString() );
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

        }

        [Test, Description("test: QueryHierarchyDataJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate), UriTemplate = json/QueryHierarchyData?pageIndex={pageIndex}&pageSize={pageSize}&orderby={orderby}")]
        public void TestQueryHierarchyDataJson()
        {
            string serviceUri = string.Format("/HierarchyService.svc/json/QueryHierarchyData?pageIndex={0}&pageSize={1}&orderby={2}", "0","10", "Name" );
        
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJson);

            HierarchyDataQueryResult result = TestServicesHelper.GenerateObjectByJson<HierarchyDataQueryResult>(response);

            var obj = result.Where(x => x.Name == china.Name).FirstOrDefault();
            Assert.IsTrue(result.Contains(obj));
        }

        [Test,Description("FindByKeywordJson(string hierarchyType, string query, int maxReturnedCount),UriTemplate = json/FindByKeyword?hierarchyType={hierarchyType}&q={query}&limit={maxReturnedCount}")]
        public void TestFindByKeywordJson()
        {
            string serviceUri = string.Format("/HierarchyService.svc/json/FindByKeyword?hierarchyType={0}&q={1}&limit={2}", GEOGRAPHY, china.Name, 10);
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            Collection<HierarchyDataObject> results = (TestServicesHelper.GenerateObjectByJson<Collection<HierarchyDataObject>>(response) as Collection<HierarchyDataObject>);

            var obj = results.Where(x => x.Name == china.Name).FirstOrDefault();
            Assert.IsTrue(obj != null);
        }
    }
}
