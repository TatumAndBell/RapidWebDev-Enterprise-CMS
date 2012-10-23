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
using System.IO;

using System.Globalization;
using System.Collections.ObjectModel;
using RapidWebDev.RestfulServices.ConcreteDataServices;


namespace RapidWebDev.RestfulServices.Test
{
    [TestFixture]
    public class ConcreteDataServiceTest
    {


        private const string DEGREE = "Degree";
        private ConcreteDataObject master;

        private string userName = TestServicesHelper.userName;
        private string password = TestServicesHelper.password;

        [SetUp]
        public void Setup()
        {
            //Create Testing Data

            master = new ConcreteDataObject { Type = DEGREE, Name = "Master" + Guid.NewGuid().ToString().Substring(0, 5), CreatedDate = System.DateTime.Now };


            string serviceUri = @"/ConcreteDataService.svc/json/Save";

            string content = TestServicesHelper.GenerateJsonByType(master);
            try
            {
                string id = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, null);
                master.Id = new Guid(id.Replace("\"", ""));
            }
            catch (Exception) { throw; }

        }
        [TearDown]
        public void CleanUp()
        {

            string serviceUri = @"/ConcreteDataService.svc/json/Save";
            master.DeleteStatus = DeleteStatus.Deleted;
            string content = TestServicesHelper.GenerateJsonByType(master);
            try
            {
                string id = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, null);
                master.Id = new Guid(id.Replace("\"", ""));
            }
            catch (Exception) { throw; }

        }

        [Test, Description("test: ConcreteDataObject GetByIdJson(string concreteDataId); UriTemplate = json/GetById/{concreteDataId}")]
        public void TestJsonGetById()
        {
            string serviceUri = @"/ConcreteDataService.svc/json/GetById/" + master.Id;
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            Assert.AreEqual(master.Id, (TestServicesHelper.GenerateObjectByJson<ConcreteDataObject>(response) as ConcreteDataObject).Id);


        }

        [Test, Description("test:ConcreteDataObject GetByNameJson(string type, string name),UriTemplate = json/GetByName/{type}/{name}")]
        public void TestJsonByName()
        {
            string serviceUri = string.Format(CultureInfo.InvariantCulture, "/ConcreteDataService.svc/json/GetByName/{0}/{1}", DEGREE, master.Name);

            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            Assert.AreEqual(master.Name, (TestServicesHelper.GenerateObjectByJson<ConcreteDataObject>(response) as ConcreteDataObject).Name);
        }

        [Test, Description("test:void SaveJson(ConcreteDataObject concreteDataObject),UriTemplate = json/Save")]
        public void TestJsonSave()
        {
            this.CleanUp();
            master = new ConcreteDataObject { Type = DEGREE, Name = "Master" + Guid.NewGuid().ToString().Substring(0, 5), CreatedDate = System.DateTime.Now };


            string serviceUri = @"/ConcreteDataService.svc/json/Save";
            string content = TestServicesHelper.GenerateJsonByType(master);
            string id = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, null).Replace("\"", "");

            master.Id = new Guid(id);
        }

        [Test, Description("test:Collection<ConcreteDataObject> FindByKeywordJson(string concreteDataType, string query, int limit), UriTemplate = json/FindByKeyword?concreteDataType={concreteDataType}&q={query}&limit={limit}")]
        public void TestJsonFindByKeyword()
        {
            string serviceUri = string.Format(@"/ConcreteDataService.svc/json/FindByKeyword?concreteDataType={0}&q={1}&limit={2}", DEGREE, master.Name, "10");
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);
            Collection<ConcreteDataObject> results = (TestServicesHelper.GenerateObjectByJson<Collection<ConcreteDataObject>>(response) as Collection<ConcreteDataObject>);
            Assert.IsTrue(results.FirstOrDefault().Name == master.Name);
        }

        [Test, Description("test:FindConcreteDataTypesJson(),UriTemplate = json/FindConcreteDataTypes")]
        public void TestJsonFindDataType()
        {
            string serviceUri = @"/ConcreteDataService.svc/json/FindConcreteDataTypes";
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);
            Collection<string> results = TestServicesHelper.GenerateObjectByJson<Collection<string>>(response);
            Assert.IsTrue(results.Contains(DEGREE));
        }

        [Test, Description("QueryConcreteDataJson, UriTemplate = json/QueryConcreteData?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
        public void TestQueryConcreteDataJson() 
        {
            string serviceUri = string.Format("/ConcreteDataService.svc/json/QueryConcreteData?pageindex={0}&pagesize={1}&orderby={2}", 0, 10, "CreatedDate");

            WebServiceQueryPredicateParameter parameter = new WebServiceQueryPredicateParameter() { Name = "Guid",Type = WebServiceQueryPredicateParameterTypes.String,Value = master.Name };
            
            WebServiceQueryPredicate predicate = new WebServiceQueryPredicate()
            {
                Expression = "Name=@0",
                Parameters = new WebServiceQueryPredicateParameter[] {parameter}
            };

            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent,TestServicesHelper.GenerateJsonByType(predicate),null);

            ConcreteDataQueryResult results = TestServicesHelper.GenerateObjectByJson<ConcreteDataQueryResult>(response);

            var result = results.Where(x => x.Id == master.Id).Select(x => x.Id);

            Assert.IsTrue(result.Contains(master.Id));
        }
    }
}
