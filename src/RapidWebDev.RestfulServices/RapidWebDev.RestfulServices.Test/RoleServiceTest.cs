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
using RapidWebDev.RestfulServices.RoleServices;
using System.Net;
using System.Collections.ObjectModel;
using RapidWebDev.RestfulServices.MembershipServices;
using RapidWebDev.RestfulServices.OrganizationServices;

namespace RapidWebDev.RestfulServices.Test
{
    [TestFixture]
    public class RoleServiceTest
    {
        private const string GEOGRAPHY = "Geography";
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
            string serviceUriUser = string.Format("/MembershipService.svc/json/Save");

            string contentUser = TestServicesHelper.GenerateJsonByType(common);

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("password", pwd);
            parameters.Add("passwordAnswer", pwdAns);

            string id = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUriUser, userName, password, TestServicesHelper.PostDataByJsonWithContent, contentUser, parameters).Replace("\"", "");
            common.UserId = new Guid(id);

            super = new RoleObject { RoleName = "super", Domain = "Department", Description = "super role" };
            string serviceUri = @"/RoleService.svc/json/Save";

            string content = TestServicesHelper.GenerateJsonByType(super);
            string Roleid = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, content,null).Replace("\"", "");
            super.RoleId = new Guid(Roleid);
        }

        [TearDown]
        public void CleanUp()
        {
            string serviceUri1 = @"/OrganizationService.svc/json/SaveOrganizationType";
            organizationTypeObject.DeleteStatus = DeleteStatus.Deleted;
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri1, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(organizationTypeObject),null);

            string serviceUri = string.Format("/RoleService.svc/json/HardDelete/{0}", super.RoleId.ToString());
            TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

        }

        [Test, Description("test:QueryRolesJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)")]
        public void TestQueryRolesJson()
        {
            string serviceUri = string.Format("/RoleService.svc/json/QueryRoles?pageindex={0}&pagesize={1}&orderby={2}", "0", "10", "RoleName");
            
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJson);

            RoleQueryResult result = TestServicesHelper.GenerateObjectByJson<RoleQueryResult>(content);
            var obj = result.Where(x => x.RoleName == super.RoleName).FirstOrDefault();
            Assert.IsTrue(result.Contains(obj));
        }

        [Test, Description("Collection<RoleObject> FindByOrganizationTypeJson(string organizationTypeId)")]
        public void TestFindByOrganizationTypeJson()
        {
            string serviceUri = string.Format("/RoleService.svc/json/QueryRoles?pageindex={0}&pagesize={1}&orderby={2}", "0", "10", "RoleName");
           
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJson);

            Collection<RoleObject> result = TestServicesHelper.GenerateObjectByJson<Collection<RoleObject>>(content);
            var obj = result.Where(x => x.RoleName == super.RoleName).FirstOrDefault();
            Assert.IsTrue(result.Contains(obj));
        }

        [Test, Description("test:void SetUserToRolesJson(string userId, IdCollection roleIds)")]
        public void TestSetUserToRolesJson()
        {
           
            string serviceUri8 = string.Format("/RoleService.svc/json/SetUserToRoles/{0}",common.UserId);

            string content8 = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri8, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(new RapidWebDev.RestfulServices.RoleServices.IdCollection() { super.RoleId.ToString() }), null);

            string serviceUri9 = string.Format("/RoleService.svc/json/FindByUserId/{0}", common.UserId.ToString());

            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri9, userName, password, TestServicesHelper.GetDataByJson);

            Collection<RoleObject> results = TestServicesHelper.GenerateObjectByJson<Collection<RoleObject>>(content);

            var result = results.Where(x => x.RoleId == super.RoleId);

            Assert.IsTrue(result.Count()>0);

        }

        [Test, Description("test:RoleObject GetByIdJson(string roleId)")]
        public void TestGetByIdJson()
        {
            string serviceUri = string.Format("/RoleService.svc/json/GetById/{0}", super.RoleId.ToString());
           
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.GetDataByJson);

            RoleObject result = TestServicesHelper.GenerateObjectByJson<RoleObject>(content);

            Assert.IsTrue(result.RoleName == super.RoleName);
        }

        [Test, Description("test:BulkGetJson")]
        public void TestBulkGetJson()
        {
            string serviceUri = string.Format("/RoleService.svc/json/BulkGet");
            
            RapidWebDev.RestfulServices.RoleServices.IdCollection ids = new RapidWebDev.RestfulServices.RoleServices.IdCollection() { super.RoleId.ToString() };
            string content = TestServicesHelper.GetResponse<HttpWebRequest>(serviceUri, userName, password, TestServicesHelper.PostDataByJsonWithContent, TestServicesHelper.GenerateJsonByType(ids),null);

            Collection<RoleObject> results = TestServicesHelper.GenerateObjectByJson<Collection<RoleObject>>(content);


        }
    }
}
