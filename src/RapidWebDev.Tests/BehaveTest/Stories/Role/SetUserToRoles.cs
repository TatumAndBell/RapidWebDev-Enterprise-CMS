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
using BaoJianSoft.Platform.Initialization;
using BaoJianSoft.Platform.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace BaoJianSoft.Tests.BehaveTest.Stories.Role
{
    [TestFixture, Theme]
    public class SetUserToRoles : SetupFixture
    {
        private List<Guid> createdRoleIds = new List<Guid>();
        private List<Guid> createdUserIds = new List<Guid>();
        private List<Guid> createdOrganizationTypeIds = new List<Guid>();
        private IRoleApi roleApi;
        private RoleObject powerAdministrators;
        private RoleObject business;
        //private RoleObject customers;

        private RoleObject powerAdministrators1;
        //private RoleObject business1;
        //private RoleObject customers1;
        private Story _story;

        [SetUp]
        public void Setup()
        {
            base.GlobalSetup();



            roleApi = SpringContext.Current.GetObject<IRoleApi>();
        }

        public SetUserToRoles()
        {
            Console.WriteLine("=================Setup===================");


            base.GlobalSetup();



            roleApi = SpringContext.Current.GetObject<IRoleApi>();

            Console.WriteLine("============Ending Setup===================");
        }
        [TearDown]
        public void CleanUp()
        {
            Console.WriteLine("============Clean Up====================");
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                try
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
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    throw e;
                }
            }

            createdRoleIds.Clear();
            createdUserIds.Clear();
            createdOrganizationTypeIds.Clear();
            Console.WriteLine("========Ending Clean Up====================");
        }

        [Story, Test]
       
        public void SetUserToRoleAndUpdateStory()
        {
            _story = new Story("Create a Role By IRoleApi");
            IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
            IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
            UserObject eunge = null;
            //IDictionary<Guid, RoleObject> _objects;
            _story.AsA("User")
              .IWant("to be able to set User to  Role")
              .SoThat("I can do something");

            _story.WithScenario("Set existing User to  an Existing Role By IRoleApi  ")
                .Given("Create several new role", () =>
                                                      {
                                                          OrganizationTypeObject department =
                                                              new OrganizationTypeObject
                                                                  {
                                                                      Name = "department",
                                                                      Domain = "Inc",
                                                                      Description = "department-desc"
                                                                  };
                                                          organizationApi.Save(department);
                                                          createdOrganizationTypeIds.Add(department.OrganizationTypeId);
                                                          powerAdministrators = new RoleObject
                                                                                    {
                                                                                        RoleName = "powerAdministrators",
                                                                                        Description =
                                                                                            "powerAdministrators-desc",
                                                                                        OrganizationTypeIds =
                                                                                            new Collection<Guid>
                                                                                                {
                                                                                                    department.
                                                                                                        OrganizationTypeId
                                                                                                }
                                                                                    };
                                                          business = new RoleObject
                                                                         {
                                                                             RoleName = "business",
                                                                             Description = "business-desc",
                                                                             OrganizationTypeIds =
                                                                                 new Collection<Guid> { department.OrganizationTypeId }
                                                                         };
                                                          roleApi.Save(powerAdministrators);
                                                          roleApi.Save(business);
                                                          createdRoleIds.AddRange(new Guid[]
                                                                                      {
                                                                                          powerAdministrators.RoleId,
                                                                                          business.RoleId
                                                                                      });

                                                      })
                .And("Create User", () =>
                                        {
                                            eunge = new UserObject
                                           {
                                               OrganizationId = platformConfiguration.Organization.OrganizationId,
                                               UserName = "eunge",
                                               DisplayName = "Eunge",
                                               Email = "eunge.liu@gmail.com",
                                               Comment = "The author of BaoJianSoft.",
                                               IsApproved = true
                                           };

                                            membershipApi.Save(eunge, "password1", null);
                                            createdUserIds.Add(eunge.UserId);
                                        })
                .When("Set User to the Roles", () =>
                                                   {
                                                       roleApi.SetUserToRoles(eunge.UserId, new Guid[] { powerAdministrators.RoleId });
                                                       roleApi.SetUserToRoles(eunge.UserId, new Guid[] { powerAdministrators.RoleId, business.RoleId });
                                                       typeof(ArgumentException).ShouldBeThrownBy(() => roleApi.SetUserToRoles(new Guid(), new Guid[] { powerAdministrators.RoleId }));
                                                   })
                .Then("I get the relationship among these roles and User", () =>
                             {
                                 roleApi.FindByUserId(eunge.UserId).Count().ShouldEqual(2);
                                 roleApi.IsUserInRole(eunge.UserId, powerAdministrators.RoleId).ShouldBeTrue();
                                 roleApi.IsUserInRole(eunge.UserId, business.RoleId).ShouldBeTrue();
                             });

            _story.WithScenario("Update an Existing Role By IRoleApi  ")
                .Given("an existing Role", () => { })
                .When("Update the Role's Role Name", () => { powerAdministrators.RoleName = "NotAdmin"; roleApi.Save(powerAdministrators); })
                .Then("I still can get the Role by UserId",()=>
                                                               {
                                                                   var temp = roleApi.FindByUserId(eunge.UserId).Where(x => x.RoleName == powerAdministrators.RoleName);
                                                                   if(temp is RoleObject)
                                                                       ((RoleObject)temp).RoleName.ShouldEqual("NotAdmin");
                                                               });
            _story.WithScenario("Delete an Existing Role which assoicate with an existing User By IRoleApi  ")
                .Given("an existing Role", () => { })
                .When("Delete the Role", () => { roleApi.Delete(powerAdministrators.RoleId); })
                .Then("I  can get noting by userId", () =>
                {
                    powerAdministrators1 = roleApi.FindByUserId(eunge.UserId).Where(x => x.RoleName == powerAdministrators1.RoleName) as RoleObject;
                    powerAdministrators1.ShouldBeNull();
                });


            this.CleanUp();
        }
    }
}
