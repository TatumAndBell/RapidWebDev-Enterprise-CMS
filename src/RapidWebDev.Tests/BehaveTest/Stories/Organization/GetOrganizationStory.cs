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
    public class GetOrganizationStory : SetupFixture
    {
        private Story _story;

        private OrganizationObject _OrganObject1;

        private OrganizationObject _OrganObject2;

        private OrganizationObject _OrganObject3;

        private OrganizationObject _OrganObject4;

        private OrganizationTypeObject _OrganTypeObject1;

        //private OrganizationTypeObject _OrganTypeObject2;

        private OrganizationTypeObject _OrganTypeObject3;

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

        public GetOrganizationStory()
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
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                var organizationsToDelete = ctx.Organizations.Where(org => createdOrganizationIds.ToArray().Contains(org.OrganizationId));
                var organizationTypesToDelete = ctx.OrganizationTypes.Where(orgType => createdOrganizationTypeIds.ToArray().Contains(orgType.OrganizationTypeId));

                ctx.Organizations.DeleteAllOnSubmit(organizationsToDelete);
                ctx.OrganizationTypes.DeleteAllOnSubmit(organizationTypesToDelete);
                ctx.SubmitChanges();
            }
            createdOrganizationIds.Clear();
            createdOrganizationTypeIds.Clear();
            Console.WriteLine("========Ending Clean Up====================");
        }

        [Story, Test]
        public void GetOrganizationWithCorrectGuidAndCorrectName()
        {
            _story = new Story("Find a Organization By IOrganizationApi");

            _story.AsA("User")
              .IWant("to be able to find an existing Organization")
              .SoThat("I can modify the object");

            _story.WithScenario("find an existing Organization ")
                .Given("the GUID and Name of the organization", () =>
                {
                    _OrganTypeObject1 = _utils.CreateOrganizationTypeOject("Depart","Inc");

                    _organizationApi.Save(_OrganTypeObject1);

                    createdOrganizationTypeIds.Add(_OrganTypeObject1.OrganizationTypeId);

                    _OrganObject1 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "hangzhou1", "hangzhou1");

                    _organizationApi.Save(_OrganObject1);

                    createdOrganizationIds.Add(_OrganObject1.OrganizationId);

                })
                .When("I get the organization", () =>
                {
                    _OrganObject2 = _organizationApi.GetOrganization(_OrganObject1.OrganizationId);

                    _OrganObject3 = _organizationApi.GetOrganizationByName(_OrganObject1.OrganizationName);

                    _OrganObject4 = _organizationApi.GetOrganizationByCode(_OrganObject1.OrganizationCode);

                })
                .Then("I Get two same Organization", () =>
                {
                    _OrganObject2.ShouldEqual(_OrganObject1);
                    _OrganObject2.ShouldEqual(_OrganObject3);
                    _OrganObject2.ShouldEqual(_OrganObject4);

                });

            this.CleanUp();
        }

        [Story, Test]
        public void BulkGetOrganizationTypes()
        {
            _story = new Story("Find More than one  OrganizationTypes By IOrganizationApi");

            _story.AsA("User")
              .IWant("to be able to find more than one  existing  OrganizationTypes")
              .SoThat("I can get the  OrganizationTypes");

            IList<string> domains = new List<string>();

            domains.Add("Inc");
            domains.Add("Inc2");
            domains.Add("Customer");

            IEnumerable<OrganizationTypeObject> _temp = null;

            _story.WithScenario("find  more than  OrganizationTypes ")
                .Given("the domain of the  organizationtype", () =>
                {
                    _OrganTypeObject1 = _utils.CreateOrganizationTypeOject("Depart1","Inc");

                    _organizationApi.Save(_OrganTypeObject1);

                    createdOrganizationTypeIds.Add(_OrganTypeObject1.OrganizationTypeId);

                    //_OrganTypeObject2 = _utils.CreateOrganizationTypeOject("Depart2", "Inc");

                    //_organizationApi.Save(_OrganTypeObject2);

                    //createdOrganizationTypeIds.Add(_OrganTypeObject2.OrganizationTypeId);


                    _OrganTypeObject3 = _utils.CreateOrganizationTypeOject("Depart3", "Customer");

                    _organizationApi.Save(_OrganTypeObject3);

                    createdOrganizationTypeIds.Add(_OrganTypeObject3.OrganizationTypeId);


                })
                .When("I bulkget organizations", () =>
                {
                    _temp = _organizationApi.FindOrganizationTypes(domains);
                })
                .Then("I can get all the organizations in this List", () =>
                {
                    _temp.Contains(_OrganTypeObject1).ShouldBeTrue();

                    _temp.Contains(_OrganTypeObject3).ShouldBeTrue();
                });

            this.CleanUp();
        }

        [Story, Test]
        public void BulkGetOrganizations()
        {
            _story = new Story("Find More than one  Organizations By IOrganizationApi");

            _story.AsA("User")
              .IWant("to be able to find more than one  existing  Organization")
              .SoThat("I can get the  organizations");

            IDictionary<Guid, OrganizationObject> _temp = null;

            _story.WithScenario("find  more than  Organization ")
                .Given("the a bunch of GUID of the  organizations", () =>
                {
                    _OrganTypeObject1 = _utils.CreateOrganizationTypeOject("Depart","Inc");

                    _organizationApi.Save(_OrganTypeObject1);

                    createdOrganizationTypeIds.Add(_OrganTypeObject1.OrganizationTypeId);

                    _OrganObject1 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "sh1", "sh1");

                    _organizationApi.Save(_OrganObject1);

                    createdOrganizationIds.Add(_OrganObject1.OrganizationId);

                    _OrganObject2 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "sh2", "sh2");

                    _OrganObject2.ParentOrganizationId = _OrganObject1.OrganizationId;

                    _organizationApi.Save(_OrganObject2);

                    createdOrganizationIds.Add(_OrganObject2.OrganizationId);


                    _OrganObject3 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "sh3", "sh3");

                    _OrganObject3.ParentOrganizationId = _OrganObject1.OrganizationId;

                    _organizationApi.Save(_OrganObject3);

                    createdOrganizationIds.Add(_OrganObject3.OrganizationId);



                })
                .When("I call bulkget organizations", () =>
                {
                    _temp = _organizationApi.BulkGetOrganizations(createdOrganizationIds);
                })
                .Then("I can get all the organizations in this List", () =>
                {
                    _temp[_OrganObject2.OrganizationId].ShouldEqual(_OrganObject2);
                    _temp[_OrganObject3.OrganizationId].ShouldEqual(_OrganObject3);
                    _temp[_OrganObject1.OrganizationId].ShouldEqual(_OrganObject1);

                });

            this.CleanUp();
        }

        [Story, Test]
        public void FindChildOrganization()
        {
            _story = new Story("Find More than one Child Organizations of Parent Organization By IOrganizationApi");

            _story.AsA("User")
              .IWant("to be able to find more than one  existing Child Organization")
              .SoThat("I can get the child organization");

            IEnumerable<Guid> _temp = null;

            _story.WithScenario("find  more than one  existing Child Organization ")
                .Given("the GUID of the parent organization", () =>
                {
                    _OrganTypeObject1 = _utils.CreateOrganizationTypeOject("Depart","Inc");

                    _organizationApi.Save(_OrganTypeObject1);

                    createdOrganizationTypeIds.Add(_OrganTypeObject1.OrganizationTypeId);

                    _OrganObject1 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "wuhan2", "wuhan2");

                    _organizationApi.Save(_OrganObject1);

                    createdOrganizationIds.Add(_OrganObject1.OrganizationId);

                    _OrganObject2 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "wuhan3", "wuhan3");

                    _OrganObject2.ParentOrganizationId = _OrganObject1.OrganizationId;

                    _organizationApi.Save(_OrganObject2);

                    createdOrganizationIds.Add(_OrganObject2.OrganizationId);


                    _OrganObject3 = _utils.CreateOrganizationObject(_OrganTypeObject1.OrganizationTypeId, "tianjiang", "tianjiang");

                    _OrganObject3.ParentOrganizationId = _OrganObject1.OrganizationId;

                    _organizationApi.Save(_OrganObject3);

                    createdOrganizationIds.Add(_OrganObject3.OrganizationId);



                })
                .When("I get the GUID of children organization", () =>
                {
                    _temp = _organizationApi.FindAllEnabledChildOrganizationIds(_OrganObject1.OrganizationId);
                })
                .Then("I can confirm  GUID of Children in this List", () =>
                {
                    _temp.Contains(_OrganObject2.OrganizationId);
                    _temp.Contains(_OrganObject3.OrganizationId);
                   

                });

            this.CleanUp();
        }
    }
}
