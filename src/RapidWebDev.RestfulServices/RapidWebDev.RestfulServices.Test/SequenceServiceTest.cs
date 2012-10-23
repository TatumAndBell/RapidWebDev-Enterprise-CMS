using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RapidWebDev.RestfulServices.OrganizationServices;
using System.Net;

namespace RapidWebDev.RestfulServices.Test
{
    [TestFixture]
    public class SequenceServiceTest
    {
        private OrganizationObject organizationObject;
        private OrganizationTypeObject organizationTypeObject;

        private string pwd = "TimYi123";
        private string pwdAns = "YiTim123";

        private string userName = TestServicesHelper.userName;
        private string password = TestServicesHelper.password;

        [SetUp]
        public void Setup()
        {
            string serviceUri1 = @"/OrganizationService.svc/json/SaveOrganizationType";
            string serviceUri2 = @"/OrganizationService.svc/json/SaveOrganization";

            organizationTypeObject = new OrganizationTypeObject() { Name = "Manager" + Guid.NewGuid().ToString().Substring(0, 5), Domain = "Department", Description = "department-desc", LastUpdatedDate = System.DateTime.Now };
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

        [Test, Description("CreateSingleJson(string objectId, string sequenceNoType)UriTemplate = json/CreateSingle?objectId={objectId}&sequenceNoType={sequenceNoType}")]
        public void TestCreateSingleJson()
        {
            string serviceUri = string.Format("/SequenceService.svc/json/CreateSingle?objectId={0}&sequenceNoType={1}",organizationObject.OrganizationId,"basic");
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);
        }

        [Test, Description("TestCreateMultipleJson(string objectId, string sequenceNoType, string sequenceNoCount)UriTemplate = json/CreateMultiple?objectId={objectId}&sequenceNoType={sequenceNoType}&sequenceNoCount={sequenceNoCount}")]
        public void TestCreateMultipleJson()
        {
            string serviceUri = string.Format("/SequenceService.svc/json/CreateMultiple?objectId={0}&sequenceNoType={1}&sequenceNoCount={2}", organizationObject.OrganizationId, "basic", 10);
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);
        }
    }
}
