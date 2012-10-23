using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RapidWebDev.RestfulServices.OrganizationServices;
using System.Net;
using System.Collections.ObjectModel;

namespace RapidWebDev.RestfulServices.Test
{
    [TestFixture]
    public class OrganizationServiceTest
    {
        private OrganizationObject organizationObject;
        private OrganizationTypeObject organizationTypeObject;

        private string userName = TestServicesHelper.userName;
        private string password = TestServicesHelper.password;

        [SetUp]
        public void Setup()
        {

            string serviceUri1 = @"/OrganizationService.svc/json/SaveOrganizationType";
            string serviceUri2 = @"/OrganizationService.svc/json/SaveOrganization";
         
            organizationTypeObject = new OrganizationTypeObject() { Name = "Manager" + Guid.NewGuid().ToString().Substring(0,5), Domain = "Department", Description = "department-desc",LastUpdatedDate = System.DateTime.Now };
            string organizationTypeId = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri1, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationTypeObject), null).Replace("\"", "");

            organizationTypeObject.OrganizationTypeId = new Guid(organizationTypeId);

            organizationObject = new OrganizationObject()
            {
                OrganizationCode = "sh021" + Guid.NewGuid().ToString().Substring(0, 5),
                OrganizationName = "sh-department" + Guid.NewGuid().ToString().Substring(0, 5),
                OrganizationTypeId = organizationTypeObject.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                Description = "sh-desc",
                CreatedDate = System.DateTime.Now,
                LastUpdatedDate = System.DateTime.Now
                
            };

            string organizationId = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationObject), null).Replace("\"", "");
            organizationObject.OrganizationId = new Guid(organizationId);

        }

        [TearDown]
        public void CleanUp()
        {
            string serviceUri1 = @"/OrganizationService.svc/json/SaveOrganizationType";
            string serviceUri2 = @"/OrganizationService.svc/json/SaveOrganization";
            organizationTypeObject.DeleteStatus = DeleteStatus.Deleted;
            organizationObject.Status = OrganizationStatus.Disabled;
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri1, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationTypeObject), null);
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationObject), null);
        }

        [Test, Description("OrganizationObject GetOrganizationByIdJson(string domain, string q), UriTemplate = json/GetOrganizationById/{organizationId}")]
        public void TestGetOrganizationByIdJson()
        {
            string serviceUri = string.Format("/OrganizationService.svc/json/GetOrganizationById/{0}",  organizationObject.OrganizationId);
         
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);
            Assert.IsTrue(organizationObject.OrganizationId == TestServicesHelper.GenerateObjectByJson<OrganizationObject>(content).OrganizationId);
        }

        [Test, Description("Collection<OrganizationTypeObject> FindOrganizationTypesJson(string domain), UriTemplate = json/FindOrganizationTypes/{domain}")]
        public void TestFindOrganizationTypesJson()
        {
            string serviceUri = string.Format("/OrganizationService.svc/json/FindOrganizationTypes/{0}", "Department");
       
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            Collection<OrganizationTypeObject> results = TestServicesHelper.GenerateObjectByJson<Collection<OrganizationTypeObject>>(content);

            var obj = results.Where(x=>x.OrganizationTypeId == organizationTypeObject.OrganizationTypeId);

            Assert.IsTrue(obj.Count() > 0);
        }

        [Test, Description("SearchJson(string domain, string orgTypeId, string q, string sortField, string sortDirection, int start, int limit),UriTemplate = json/search?domain={domain}&q={q}&start={start}&limit={limit}&sortfield={sortField}&sortDirection={sortDirection}&orgTypeId={orgTypeId}")]
        public void TestSearchJson()
        {
            string serviceUri = string.Format("/OrganizationService.svc/json/search?domain={0}&q={1}&start={2}&limit={3}&sortfield={4}&sortDirection={5}&orgTypeId={6}", "Department", organizationObject.OrganizationCode, 0, 10, "CreatedDate","asc",organizationTypeObject.OrganizationTypeId);
         
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);
            OrganizationQueryResult result = TestServicesHelper.GenerateObjectByJson<OrganizationQueryResult>(content);

            var obj = result.Where(x => x.OrganizationId == organizationObject.OrganizationId);

            Assert.IsTrue(obj != null);
        }

        [Test, Description("QueryOrganizationsJson:json/QueryOrganizations?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
        public void TestQueryOrganizationsJson()
        {
            string serviceUri = string.Format("/OrganizationService.svc/json/QueryOrganizations?pageindex={0}&pagesize={1}&orderby={2}", 0,10,"");

            WebServiceQueryPredicate predicate = new WebServiceQueryPredicate()
            {
                Expression = "OrganizationId=@0",
                Parameters = new WebServiceQueryPredicateParameter[] { new WebServiceQueryPredicateParameter() { Name = "SearchById", Type = WebServiceQueryPredicateParameterTypes.Guid, Value = organizationObject.OrganizationId.ToString() } }

            };

            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent,TestServicesHelper.GenerateJsonByType(predicate),null);
            OrganizationQueryResult result = TestServicesHelper.GenerateObjectByJson<OrganizationQueryResult>(content);

            var obj = result.Where(x => x.OrganizationId == organizationObject.OrganizationId);

            Assert.IsTrue(obj != null);
        }

        [Test, Description("GetOrganizationTypeByIdJson: json/GetOrganizationTypeById/{organizationTypeId}")]
        public void TestGetOrganizationTypeByIdJson() 
        {
            string serviceUri = string.Format("/OrganizationService.svc/json/GetOrganizationTypeById/{0}", organizationTypeObject.OrganizationTypeId);

            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);
            Assert.IsTrue(organizationTypeObject.OrganizationTypeId == TestServicesHelper.GenerateObjectByJson<OrganizationTypeObject>(content).OrganizationTypeId);
        }

       
    }
}
