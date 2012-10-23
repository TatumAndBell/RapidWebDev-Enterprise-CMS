/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.yi@RapidWebDev.org

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
using System.Collections.ObjectModel;
using System.Linq;
using BaoJianSoft.Common;
using BaoJianSoft.Common.Data;
using BaoJianSoft.Platform;
using BaoJianSoft.Platform.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace BaoJianSoft.Tests.BehaveTest.Stories.Permission
{
    [TestFixture, Theme]
    public class PermissionStory : SetupFixture
    {
        private IPermissionApi _PermissionApi;
        private Story _story;
        private List<Guid> createdRoleIds = new List<Guid>();
        private List<Guid> createdOrganizationTypeIds = new List<Guid>();
        private List<Guid> createdUserIds = new List<Guid>();
        private IRoleApi roleApi;
        private IMembershipApi _membershipApi;
        private RoleObject business;
        private UserObject _Uobject;
        private BehaveMemberShipUtils _utils;


        [SetUp]
        public void Setup()
        {
            base.GlobalSetup();



            _PermissionApi = SpringContext.Current.GetObject<IPermissionApi>();
            roleApi = SpringContext.Current.GetObject<IRoleApi>();
        }

        public PermissionStory()
        {
            Console.WriteLine("=================Setup===================");


            base.GlobalSetup();


            _PermissionApi = SpringContext.Current.GetObject<IPermissionApi>();
            roleApi = SpringContext.Current.GetObject<IRoleApi>();
            _membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
            _utils = new BehaveMemberShipUtils();

            Console.WriteLine("============Ending Setup===================");
        }


        [TearDown]
        public void CleanUp()
        {
            Console.WriteLine("============Clean Up====================");

            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                foreach (Guid createdUserId in createdUserIds)
                {
                    ctx.Permissions.Delete(p => p.UserId == createdUserId);
                    ctx.UsersInRoles.Delete(uir => uir.UserId == createdUserId);
                    ctx.Memberships.Delete(m => m.UserId == createdUserId);
                    ctx.Users.Delete(u => u.UserId == createdUserId);
                }

                foreach (Guid createdRoleId in createdRoleIds)
                {
                    ctx.Permissions.Delete(p => p.RoleId == createdRoleId);
                    ctx.RolesInOrganizationTypes.Delete(x => x.RoleId == createdRoleId);
                    ctx.Roles.Delete(r => r.RoleId == createdRoleId);
                }

                foreach (Guid createdOrganizationTypeId in createdOrganizationTypeIds)
                {
                    ctx.OrganizationTypes.Delete(orgType => orgType.OrganizationTypeId == createdOrganizationTypeId);
                }

                ctx.SubmitChanges();
            }



            Console.WriteLine("========Ending Clean Up====================");
        }

        [Story, Test]
        public void SaveRolePermissionStory()
        {
            _story = new Story("Save Role Permission");
            _story.AsA("User")
                .IWant("to be able to create a Role")
                .SoThat("I can assign permission to this Role");

            _story.WithScenario(
                "create a Role By IRoleApi including Create the Same Name;Same Domain; Same Description; Empty Name; ")
                .Given("Create several new roles", () =>
                {
                    IOrganizationApi organizationApi =
                        SpringContext.Current.GetObject<IOrganizationApi>();
                    OrganizationTypeObject department =
                        new OrganizationTypeObject
                        {
                            Name = "department",
                            Domain = "Inc",
                            Description = "department-desc"
                        };
                    organizationApi.Save(department);
                    createdOrganizationTypeIds.Add(department.OrganizationTypeId);

                    OrganizationTypeObject customer =
                        new OrganizationTypeObject
                        {
                            Name = "customer",
                            Domain = "Customer",
                            Description = "customer-desc"
                        };
                    organizationApi.Save(customer);
                    createdOrganizationTypeIds.Add(customer.OrganizationTypeId);


                    business = new RoleObject
                    {
                        RoleName = "business",
                        Description = "business-desc",
                        OrganizationTypeIds =
                            new Collection<Guid> { department.OrganizationTypeId }
                    };



                })
                .When("I save this member", () =>
                {
                    //roleApi.Save(powerAdministrators);
                    roleApi.Save(business);


                    //createdRoleIds.Add(powerAdministrators.RoleId);
                    createdRoleIds.Add(business.RoleId);


                })
                .And("Create Permission value and assign to Role", () =>
                {
                    _PermissionApi.SetRolePermissions(
                        business.RoleId,
                        new string[] { "p1", "p2" });
                })
                .Then("I get assign permission from role", () =>
                {
                    _PermissionApi.FindRolePermissions(business.RoleId).
                        Count().ShouldEqual(2);
                })
                .WithScenario("I can overwrite the permission of specific role")
                .Given("an existing role id")
                .And("Save new permission to the role", () =>
                {
                    _PermissionApi.SetRolePermissions(business.RoleId,
                                                      new string[] { "t1", "t2" });
                })
                .When("Get the permission from this role")
                .Then("I won't get P1,P2 any more", () =>
                {
                    var temp =
                        _PermissionApi.FindRolePermissions(business.RoleId);
                    temp.Contains(new PermissionObject("t1")).ShouldBeTrue();
                    temp.Contains(new PermissionObject("t2")).ShouldBeTrue();
                }


                )
                ;

            this.CleanUp();
        }

        [Story, Test]
        public void SaveToExistConfigPermission()
        {
            _story = new Story("Save Permission to config User");
            _story.AsA("User")
                .IWant("to be able to get config permission")
                .SoThat("I can assign new permission to this User");


            IEnumerable<PermissionConfig> config;
            _story.WithScenario("Assign new permission")
                .Given("Get the configured permission", () =>
                {
                    UserObject _object = _membershipApi.Get("admin");
                    config = _PermissionApi.FindPermissionConfig(_object.UserId);
                })
                .When("")
                .Then("I get can the value of the config", () => 
                {

                });
        }
        [Story, Test]
        public void SaveToUserPermissionStory()
        {
            _story = new Story("Save Permission to User");
            _story.AsA("User")
                .IWant("to be able to get config permission")
                .SoThat("I can assign new permission to this Role");

            _story.WithScenario("Create User and assign Permission")
                .Given("Create User", () => 
                {
                    _Uobject = _utils.CreateUserObject("Eunge Liu", true);
                })
                .When("Save this object", () => 
                { 
                _membershipApi.Save(_Uobject, "Hello123", "Hello123");
                createdUserIds.Add(_Uobject.UserId);
                })
                .Then("I can assign permission on it", () => 
                {
                    _PermissionApi.SetUserPermissions(_Uobject.UserId, new string[] { "p1", "p2" });

                    _PermissionApi.FindUserPermissions(_Uobject.UserId,true).Contains(new PermissionObject("p1")).ShouldBeTrue();
                    _PermissionApi.FindUserPermissions(_Uobject.UserId, true).Contains(new PermissionObject("p2")).ShouldBeTrue();

                })
                ;
        }
    }
}
