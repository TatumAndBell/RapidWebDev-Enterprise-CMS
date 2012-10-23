using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RapidWebDev.RestfulServices.MembershipServices;
using RapidWebDev.RestfulServices.OrganizationServices;
using System.Net;
using RapidWebDev.RestfulServices.RelationshipServices;
using System.Collections.ObjectModel;

namespace RapidWebDev.RestfulServices.Test
{
    [TestFixture]
    public class RelationshipServiceTest
    {
        private UserObject common1;
        private UserObject common2;

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


            common1 = new UserObject
            {
                UserName = "common1" + Guid.NewGuid().ToString().Substring(0, 5),
                DisplayName = "CommonUser1" + Guid.NewGuid().ToString().Substring(0, 5),
                LastActivityDate = DateTime.Now,
                LastLockoutDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                LastPasswordChangedDate = DateTime.Now,
                CreationDate = System.DateTime.Now,
                OrganizationId = organizationObject.OrganizationId,
                LastUpdatedDate = DateTime.Now,
                PasswordQuestion = pwdAns
            };
            common2 = new UserObject
            {
                UserName = "common2" + Guid.NewGuid().ToString().Substring(0, 5),
                DisplayName = "CommonUser2" + Guid.NewGuid().ToString().Substring(0, 5),
                LastActivityDate = DateTime.Now,
                LastLockoutDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                LastPasswordChangedDate = DateTime.Now,
                CreationDate = System.DateTime.Now,
                OrganizationId = organizationObject.OrganizationId,
                LastUpdatedDate = DateTime.Now,
                PasswordQuestion = pwdAns
            };

            string serviceUri = string.Format("/MembershipService.svc/json/Save");

            string content = TestServicesHelper.GenerateJsonByType(common1);
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("password", pwd);
            parameters.Add("passwordAnswer", pwdAns);

            string id = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, parameters).Replace("\"", "");
            common1.UserId = new Guid(id);
            string content2 = TestServicesHelper.GenerateJsonByType(common2);
            string id2 = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content2, parameters).Replace("\"", "");
            common2.UserId = new Guid(id2);
        }

        [TearDown]
        public void CleanUp()
        {
            string serviceUri = string.Format("/MembershipService.svc/json/Save");

            common1.IsApproved = false;
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(common1), null);

            common2.IsApproved = false;
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(common2), null);


            string serviceUri1 = @"/OrganizationService.svc/json/SaveOrganizationType";
            string serviceUri2 = @"/OrganizationService.svc/json/SaveOrganization";

            organizationTypeObject.DeleteStatus = DeleteStatus.Deleted;
            organizationObject.Status = OrganizationStatus.Disabled;
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri1, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationTypeObject), null);
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationObject), null);
      
        }

        [Test, Description("SaveJson(string objectXId,string objectYId, string RelationshipType, string Ordinal), UriTemplate = json/Save?objectXId={objectXId}&objectYId={objectYId}&RelationshipType={RelationshipType}&Ordinal={Ordinal}")]
        public void TestSaveJson()
        {
            string serviceUri = string.Format("/RelationshipService.svc/json/Save?objectXId={0}&objectYId={1}&RelationshipType={2}&Ordinal={3}",common1.UserId,common2.UserId,"parent","");

            string serviceUri2 = string.Format("/RelationshipService.svc/json/GetOneToOne/{0}/{1}", common1.UserId, "parent");

            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.GetDataByJson);

            RelationshipObject reference = TestServicesHelper.GenerateObjectByJson<RelationshipObject>(content);

            Assert.IsTrue(reference.ReferenceObjectId == common2.UserId);
        }

        [Test, Description("RemoveJson ,UriTemplate = json/Remove?objectXId={objectXId}&objectYId={objectYId}&relationshipType={relationshipType}")]
        public void TestRemoveJson()
        {
            string serviceUri = string.Format("/RelationshipService.svc/json/Save?objectXId={0}&objectYId={1}&RelationshipType={2}&Ordinal={3}", common1.UserId, common2.UserId, "parent", "");

            string serviceUri2 = string.Format("/RelationshipService.svc/json/GetOneToOne/{0}/{1}", common1.UserId, "parent");

            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.GetDataByJson);

            RelationshipObject reference = TestServicesHelper.GenerateObjectByJson<RelationshipObject>(content);

            Assert.IsTrue(reference.ReferenceObjectId == common2.UserId);

            string serviceUri3 = string.Format("/RelationshipService.svc/json/Remove?objectXId={0}&objectYId={1}&relationshipType={2}", common1.UserId, common2.UserId,"parent");

            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri3, userName, password, TestServicesHelper.GetDataByJson);

            string contentRemove = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.GetDataByJson);

            //RelationshipObject referenceRemove = TestServicesHelper.GenerateObjectByJson<RelationshipObject>(contentRemove);

            Assert.IsTrue(contentRemove.Equals(""));
        }

        [Test, Description("GetOneToMany,UriTemplate = json/GetOneToMany/{objectId}/{relationshipType}")]
        public void TestGetOneToMany()
        {
            string serviceUri = string.Format("/RelationshipService.svc/json/Save?objectXId={0}&objectYId={1}&RelationshipType={2}&Ordinal={3}", common1.UserId, common2.UserId, "parent", "");

            string serviceUri2 = string.Format("/RelationshipService.svc/json/GetOneToMany/{0}/{1}", common1.UserId, "parent");

            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.GetDataByJson);

            Collection<RelationshipObject> references = TestServicesHelper.GenerateObjectByJson<Collection<RelationshipObject>>(content);

            var result = references.Where(x => x.ReferenceObjectId == common2.UserId).Single();

            Assert.IsTrue(result != null);

            string serviceUri3 = string.Format("/RelationshipService.svc/json/Remove?objectXId={0}&objectYId={1}&relationshipType={2}", common1.UserId, common2.UserId, "parent");

            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri3, userName, password, TestServicesHelper.GetDataByJson);

            string content2 = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.GetDataByJson);

            Collection<RelationshipObject> results = TestServicesHelper.GenerateObjectByJson<Collection<RelationshipObject>>(content2);
            Assert.IsTrue(results.Count() == 0);


        }
         
    }
}
