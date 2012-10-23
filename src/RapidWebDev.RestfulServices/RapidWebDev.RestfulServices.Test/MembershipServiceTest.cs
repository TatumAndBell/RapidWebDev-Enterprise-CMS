using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RapidWebDev.RestfulServices.MembershipServices;
using System.Net;
using RapidWebDev.RestfulServices.OrganizationServices;

namespace RapidWebDev.RestfulServices.Test
{
    [TestFixture]
    public class MembershipServiceTest
    {
        private UserObject common;
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
            string organizationTypeId = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri1, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationTypeObject),null).Replace("\"", "");

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

            string organizationId = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationObject),null).Replace("\"", "");
            organizationObject.OrganizationId = new Guid(organizationId);


            common = new UserObject { UserName = "common" + Guid.NewGuid().ToString().Substring(0, 5), DisplayName = "CommonUser" + Guid.NewGuid().ToString().Substring(0,5),
                LastActivityDate = DateTime.Now,LastLockoutDate = DateTime.Now, LastLoginDate = DateTime.Now,LastPasswordChangedDate = DateTime.Now, CreationDate = System.DateTime.Now,OrganizationId = organizationObject.OrganizationId,LastUpdatedDate = DateTime.Now,PasswordQuestion = pwdAns };
            string serviceUri = string.Format("/MembershipService.svc/json/Save");
          
            string content = TestServicesHelper.GenerateJsonByType(common);
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("password", pwd);
            parameters.Add("passwordAnswer", pwdAns);

            string id = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, parameters).Replace("\"", "");
            common.UserId = new Guid(id);
        }

        [TearDown]
        public void CleanUp()
        {
            string serviceUri = string.Format("/MembershipService.svc/json/Save");
          
            common.IsApproved = false;
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(common),null);

            string serviceUri1 = @"/OrganizationService.svc/json/SaveOrganizationType";
            string serviceUri2 = @"/OrganizationService.svc/json/SaveOrganization";
           
            organizationTypeObject.DeleteStatus = DeleteStatus.Deleted;
            organizationObject.Status = OrganizationStatus.Disabled;
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri1, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationTypeObject),null);
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationObject),null);
      

        }

        [Test, Description("test: GetByNameJson(string userName);UriTemplate = json/GetByName/{userName}")]
        public void TestGetByNameJsonJson()
        {
            string serviceUri = string.Format("/MembershipService.svc/json/GetByName/{0}", common.UserName);
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            UserObject user = TestServicesHelper.GenerateObjectByJson<UserObject>(content);

            Assert.IsTrue(user.UserId == common.UserId);
        }

        [Test, Description("test: GetByIdJson(string userId);UriTemplate = json/GetById/{userName}")]
        public void TestGetByIdJson()
        {
            string serviceUri = string.Format("/MembershipService.svc/json/GetById/{0}", common.UserId);
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            UserObject user = TestServicesHelper.GenerateObjectByJson<UserObject>(content);

            Assert.IsTrue(user.UserId == common.UserId);
        }

        [Test, Description("test: ChangePasswordJson(string userId);UriTemplate = json/ChangePassword/{userId}")]
        public void TestChangePasswordJson()
        {
            string serviceUri = string.Format("/MembershipService.svc/json/ChangePassword/{0}", common.UserId);
                      

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("oldPassword", pwd);
            parameters.Add("newPassword", pwdAns);

            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent,TestServicesHelper.GenerateJsonByType(common),parameters);

            bool user = TestServicesHelper.GenerateObjectByJson<bool>(content);

            Assert.IsTrue(user);
        }


        [Test, Description("test: QueryUsersJson(WebServiceQueryPredicate predicate, string orderby, int pageIndex, int pageSize);UriTemplate = json/QueryUsers?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
        public void TestQueryUsersJson()
        {
            string serviceUri = string.Format("/MembershipService.svc/json/QueryUsers?pageindex={0}&pagesize={1}&orderby={2}",0,10,"UserName");

            RapidWebDev.RestfulServices.MembershipServices.WebServiceQueryPredicate predicate = new RapidWebDev.RestfulServices.MembershipServices.WebServiceQueryPredicate() 
            {
                Expression = "UserName=@0",
                Parameters = new RapidWebDev.RestfulServices.MembershipServices.WebServiceQueryPredicateParameter[]{new RapidWebDev.RestfulServices.MembershipServices.WebServiceQueryPredicateParameter(){Name = "UserName",Type = RapidWebDev.RestfulServices.MembershipServices.WebServiceQueryPredicateParameterTypes.String,Value = common.UserName}}
            };
            
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent,TestServicesHelper.GenerateJsonByType(predicate),null);

            UserQueryResult user = TestServicesHelper.GenerateObjectByJson<UserQueryResult>(content);

            var result = user.Where(x => x.UserId == common.UserId);

            Assert.IsTrue(result.Count() > 0);
        }
    }
}
