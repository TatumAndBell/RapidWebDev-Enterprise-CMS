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
using System.Linq;
using BaoJianSoft.Common;
using BaoJianSoft.Common.Data;
using BaoJianSoft.Platform;
using BaoJianSoft.Platform.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;


namespace BaoJianSoft.Tests.BehaveTest.Stories.Organization
{
    [TestFixture, Theme]
    public class SaveOrganizationStory : SetupFixture
    {
        private Story _story;

        private OrganizationObject _OrganObject1;

        private IOrganizationApi _organizationApi;

        List<Guid> createdOrganizationIds;
        List<Guid> createdOrganizationTypeIds;

        private BehaveOrganizationUtils _utils;

        [SetUp]
        public void Setup()
        {
            createdOrganizationIds = new List<Guid>();
            createdOrganizationTypeIds = new List<Guid>();

            base.GlobalSetup();

            _utils = new BehaveOrganizationUtils();

            _organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
        }

        public SaveOrganizationStory()
        {
            Console.WriteLine("=================Setup===================");

            createdOrganizationIds = new List<Guid>();
            createdOrganizationTypeIds = new List<Guid>();

            base.GlobalSetup();

            _utils = new BehaveOrganizationUtils();

            _organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            Console.WriteLine("============Ending Setup===================");
        }
        [TearDown]
        public void CleanUp()
        {
           Console.WriteLine("============Clean Up====================");
           try
           {
               using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
               {
                   var organizationsToDelete = ctx.Organizations.Where(org => createdOrganizationIds.ToArray().Contains(org.OrganizationId));
                   var organizationTypesToDelete = ctx.OrganizationTypes.Where(orgType => createdOrganizationTypeIds.ToArray().Contains(orgType.OrganizationTypeId));

                   ctx.OrganizationTypes.DeleteAllOnSubmit(organizationTypesToDelete);
                   ctx.Organizations.DeleteAllOnSubmit(organizationsToDelete);
                   
                   ctx.SubmitChanges();
                   createdOrganizationIds.Clear();
                   createdOrganizationTypeIds.Clear();
               }
           }
           catch (Exception e)
           {
               Console.WriteLine(e.Message);
               throw e;
           }
           
            Console.WriteLine("========Ending Clean Up====================");
        }

        [Story, Test]
        public void SaveANewOrganizationWithEmptyProperties()
        {
            _story = new Story("Save a Organization By IOrganizationApi");

            _story.AsA("User")
              .IWant("to be able to create a new Organization, but I don't assign value to it")
              .SoThat("I can get specific exception");

            _story.WithScenario("Save a new Organization which have no value")
                .Given("the new member with nothing", () => 
                { 
                    _OrganObject1 = new OrganizationObject(); 
                })
                .When("I save this Organization", () => { })
                .Then("I Get an ArgumentNullException from IOrganizationApi.Save()", () => typeof(ArgumentNullException).ShouldBeThrownBy(() => _organizationApi.Save(_OrganObject1)));

            this.CleanUp();
        }

        //[Story, Test]
     
        //public void SaveANewOrganizationrWithValues()

        //{
        //    _story = new Story("Save a Organization By IOrganizationApi");

        //    _story.AsA("User")
        //      .IWant("to be able to create a new Organization")
        //      .SoThat("I can get specific exception");

        //    _story.WithScenario("Save a new Organization")
        //        .Given("the new Organization ", () => 
        //        {
        //            _OrganTypeObject1 = _utils.CreateOrganizationTypeOject("Type1", "Inc");

        //            _organizationApi.Save(_OrganTypeObject1);

        //            createdOrganizationTypeIds.Add(_OrganTypeObject1.OrganizationTypeId);

        //            _OrganObject1 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "hangzhou2", "wuhan1");

        //            createdOrganizationIds.Add(_OrganObject1.OrganizationId);

        //        })
        //        .When("I save this Organization", () => 
        //        {
        //            _organizationApi.Save(_OrganObject1); 
        //        })
        //        .Then("I Get an new Organization Object from IOrganizationApi.Save()", 
        //        () => 
        //            _OrganObject1.OrganizationId.ShouldNotBeNull()
        //            );

        //    this.CleanUp();
        //}

        //[Story, Test]
        
        //public void UpdateOldOrganization()
        //{
        //    _story = new Story("Update a Organization By IOrganizationApi");

        //    _story.AsA("User")
        //      .IWant("to be able to update a old Organization")
        //      .SoThat("I can get specific exception");

        //    _story.WithScenario("Save a new Organization which have no value")
        //        .Given("the new member with nothing", () =>
        //        {
        //            _OrganTypeObject1 = _utils.CreateOrganizationTypeOject("Type3", "Inc");

        //            _organizationApi.Save(_OrganTypeObject1);

                   

        //            createdOrganizationTypeIds.Add(_OrganTypeObject1.OrganizationTypeId);

        //            _OrganObject1 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "wuhan1", "wuhan1");

        //            _organizationApi.Save(_OrganObject1);

        //            Console.Error.WriteLine("_OrganObject1's OrganizationTypeId : " + _OrganObject1.OrganizationTypeId);

        //            createdOrganizationIds.Add(_OrganObject1.OrganizationId);

        //            _OrganTypeObject2 = _utils.CreateOrganizationTypeOject("Type2", "Inc");

        //            _organizationApi.Save(_OrganTypeObject2);

        //            Console.Error.WriteLine("_OrganTypeObject2's OrganizationTypeId : " + _OrganTypeObject2.OrganizationTypeId);

        //            createdOrganizationTypeIds.Add(_OrganTypeObject2.OrganizationTypeId);

        //        })
        //        .When("I change the value of this Organization", () => 
        //        {
        //            _OrganObject1.OrganizationTypeId = _OrganTypeObject2.OrganizationTypeId; 
        //        })
        //        .Then("I Get an new Organization Object from IOrganizationApi.Save()", () => 
        //        {
        //            _organizationApi.Save(_OrganObject1);

        //            Console.Error.WriteLine("_OrganObject1's OrganizationTypeId : " + _OrganObject1.OrganizationTypeId);

        //            _OrganObject1.OrganizationTypeId.ShouldEqual(_OrganTypeObject2.OrganizationTypeId); 
        //        });
        //    this.CleanUp();
        
        //}

    }
}
