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
namespace BaoJianSoft.Tests.BehaveTest.Stories.Role
{
    [TestFixture, Theme]
    public class GetRoleStory : SetupFixture
    {

          private List<Guid> createdRoleIds = new List<Guid>();
        private List<Guid> createdUserIds = new List<Guid>();
        private List<Guid> createdOrganizationTypeIds = new List<Guid>();
        private IRoleApi roleApi;
        private RoleObject powerAdministrators;
        private RoleObject business;
        private RoleObject customers;

        private RoleObject powerAdministrators1;
        private RoleObject business1;
        //private RoleObject customers1;
        private Story _story;

        [SetUp]
        public void Setup()
        {

            base.GlobalSetup();



            roleApi = SpringContext.Current.GetObject<IRoleApi>();

        }

        public GetRoleStory()
        {
            Console.WriteLine("=================Setup===================");

           
            base.GlobalSetup();



            roleApi = SpringContext.Current.GetObject<IRoleApi>();

            Console.WriteLine("============Ending Setup===================");
        }

        public void CleanUp()
        {
            Console.WriteLine("============Clean Up====================");
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                foreach (Guid createdUserId in createdUserIds)
                {
                    ctx.UsersInRoles.Delete(uir => uir.UserId == createdUserId);
                    ctx.Memberships.Delete(m => m.UserId == createdUserId);
                    ctx.Users.Delete(u => u.UserId == createdUserId);
                }

                foreach (Guid createdRoleId in createdRoleIds)
                {
                    ctx.RolesInOrganizationTypes.Delete(x => x.RoleId == createdRoleId);
                    ctx.Roles.Delete(r => r.RoleId == createdRoleId);
                }

                foreach (Guid createdOrganizationTypeId in createdOrganizationTypeIds)
                {
                    ctx.OrganizationTypes.Delete(orgType => orgType.OrganizationTypeId == createdOrganizationTypeId);
                }

                ctx.SubmitChanges();
            }

            createdRoleIds.Clear();
            createdUserIds.Clear();
            createdOrganizationTypeIds.Clear();
            Console.WriteLine("========Ending Clean Up====================");
        }

        [Story,Test]
        public void GetRolesStory()
        {
            _story = new Story("Get a Role By IRoleApi");

            IDictionary<Guid, RoleObject> _objects;
            _story.AsA("User")
              .IWant("to be able to get a Role")
              .SoThat("I can do something");

            _story.WithScenario("Get a Role By IRoleApi  ")
                .Given("Create several new roles", () =>
                {
                    IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
                    OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Inc", Description = "department-desc" };
                    organizationApi.Save(department);
                    createdOrganizationTypeIds.Add(department.OrganizationTypeId);

                    OrganizationTypeObject customer = new OrganizationTypeObject { Name = "customer", Domain = "Customer", Description = "customer-desc" };
                    organizationApi.Save(customer);
                    createdOrganizationTypeIds.Add(customer.OrganizationTypeId);

                    powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", OrganizationTypeIds = new Collection<Guid> { department.OrganizationTypeId }, Predefined = true };
                    business = new RoleObject { RoleName = "business", Description = "business-desc", OrganizationTypeIds = new Collection<Guid> { department.OrganizationTypeId } };
                    customers = new RoleObject { RoleName = "customers", Description = "customers-desc", OrganizationTypeIds = new Collection<Guid> { customer.OrganizationTypeId } };



                })
                .When("I save this member", () =>
                {
                    roleApi.Save(powerAdministrators);
                    roleApi.Save(business);
                    roleApi.Save(customers);

                    createdRoleIds.Add(powerAdministrators.RoleId);
                    createdRoleIds.Add(business.RoleId);
                    createdRoleIds.Add(customers.RoleId);

                })
                .Then("I get these roles", () =>
                {
                    powerAdministrators1 = roleApi.Get(powerAdministrators.RoleId);

                    powerAdministrators1.ShouldEqual(powerAdministrators);

                    business1 = roleApi.Get(business.RoleName);

                    business1.ShouldEqual(business);

                    _objects = roleApi.BulkGet(createdRoleIds);

                    _objects[powerAdministrators.RoleId].ShouldEqual(powerAdministrators1);

                    roleApi.Delete(customers.RoleId);

                    _objects = roleApi.BulkGet(createdRoleIds);

                    _objects.ContainsKey(customers.RoleId).ShouldBeFalse();

                    createdRoleIds.Add(new Guid());

                    _objects = roleApi.BulkGet(createdRoleIds);

                    createdRoleIds.Count.ShouldEqual(_objects.Count + 2);

                });

            this.CleanUp();
        }

    }
}
