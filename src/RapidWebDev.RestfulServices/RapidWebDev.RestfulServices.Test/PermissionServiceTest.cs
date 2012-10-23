using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Net;
using RapidWebDev.RestfulServices.RoleServices;
using RapidWebDev.RestfulServices.OrganizationServices;
using RapidWebDev.RestfulServices.MembershipServices;
using System.Collections.ObjectModel;

namespace RapidWebDev.RestfulServices.Test
{
    [TestFixture]
    public class PermissionServiceTest
    {
      
        private RoleObject super;


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
            string serviceUriorganization = @"/OrganizationService.svc/json/SaveOrganization";

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

            string organizationId = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUriorganization, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationObject), null).Replace("\"", "");
            organizationObject.OrganizationId = new Guid(organizationId);

            common = new UserObject
            {
                UserName = "common" + Guid.NewGuid().ToString().Substring(0, 5),
                DisplayName = "CommonUser" + Guid.NewGuid().ToString().Substring(0, 5),
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

            string content = TestServicesHelper.GenerateJsonByType(common);
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("password", pwd);
            parameters.Add("passwordAnswer", pwdAns);

            string id = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, parameters).Replace("\"", "");
            common.UserId = new Guid(id);

            super = new RoleObject { RoleName = "super", Domain = "Department", Description = "super role" };
            string serviceUriRole = @"/RoleService.svc/json/Save";

            string contentRole = TestServicesHelper.GenerateJsonByType(super);
            string idRole = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUriRole, userName, password, TestServicesHelper.PostDataByJsonWithContent, contentRole, null).Replace("\"", "");
            super.RoleId = new Guid(idRole);
        }

        [TearDown]
        public void CleanUp()
        {
            string serviceUri1 = @"/OrganizationService.svc/json/SaveOrganizationType";
            organizationTypeObject.DeleteStatus = DeleteStatus.Deleted;
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri1, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationTypeObject), null);

            string serviceUri = string.Format("/RoleService.svc/json/HardDelete/{0}", super.RoleId.ToString());
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

        }

        [Test, Description("UriTemplate = json/SetRolePermission/{roleId} , UriTemplate = json/FindRolePermissions/{roleId}")]
        public void TestSetRolePermissionsByJson()
        {
            string serviceUri = @"/PermissionService.svc/json/SetRolePermission/" + super.RoleId;
            string content = TestServicesHelper.GenerateJsonByType(new RapidWebDev.RestfulServices.PermissionServices.IdCollection(){"p1","p2","p3"});
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, null);

            string serviceUri2 = string.Format("/PermissionService.svc/json/FindRolePermissions/{0}", super.RoleId);
            string response2 = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.GetDataByJson);

            Collection<string> results = TestServicesHelper.GenerateObjectByJson<Collection<string>>(response2);

            Assert.IsTrue(results.Contains("p1"));
        }

        [Test, Description("UriTemplate = json/SetUserPermission/{userId} , UriTemplate = json/FindUserPermissions/{userId}/{explicitOnly}")]
        public void TestSetUserPermissionsByJson()
        {
            string serviceUri = @"/PermissionService.svc/json/SetUserPermission/" + common.UserId;
            string content = TestServicesHelper.GenerateJsonByType(new RapidWebDev.RestfulServices.PermissionServices.IdCollection() { "p1", "p2", "p3" });
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, null);

            string serviceUri2 = string.Format("/PermissionService.svc/json/FindUserPermissions/{0}/{1}", common.UserId,true);
            string response2 = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.GetDataByJson);

            Collection<string> results = TestServicesHelper.GenerateObjectByJson<Collection<string>>(response2);

            Assert.IsTrue(results.Contains("p1"));
        }

        [Test, Description("DoesTheUserHasPermissionJson(),UriTemplate = json/HasPermission/{userId}/{permissionValue}")]
        public void TestDoesTheUserHasPermission()
        {
            string serviceUri = @"/PermissionService.svc/json/SetUserPermission/" + common.UserId;
            string content = TestServicesHelper.GenerateJsonByType(new RapidWebDev.RestfulServices.PermissionServices.IdCollection() { "p1", "p2", "p3" });
            string response = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content, null);

            string serviceUri2 = string.Format("/PermissionService.svc/json/HasPermission/{0}/{1}", common.UserId, "p1");
            string response2 = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri2, userName, password, TestServicesHelper.GetDataByJson);

            Assert.IsTrue(TestServicesHelper.GenerateObjectByJson<bool>(response2));
        }
    }
}
