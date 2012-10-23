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

namespace BaoJianSoft.Tests.BehaveTest.Stories.Application
{
    [TestFixture, Theme]
    public class GetAndExistApplicationStory : SetupFixture
    {
        private Story _story;

        private ApplicationObject _ApplicationObject;

        private ApplicationObject _ApplicationObject1;

        private ApplicationObject _ApplicationObject2;

        IApplicationApi _applicationApi;

        List<Guid> createdApplicationIds;

        [SetUp]
        public void Setup()
        {
            base.GlobalSetup();

            createdApplicationIds = new List<Guid>();

            _applicationApi = SpringContext.Current.GetObject<IApplicationApi>();
        }

        public  GetAndExistApplicationStory()
        {
            Console.WriteLine("=================Setup===================");


            base.GlobalSetup();

            createdApplicationIds = new List<Guid>();

            _applicationApi = SpringContext.Current.GetObject<IApplicationApi>();

            Console.WriteLine("============Ending Setup===================");
        }

        [TearDown]
        public void CleanUp()
        {
            Console.WriteLine("============Clean Up====================");

            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                var applicationsToDelete = ctx.Applications.Where(app => createdApplicationIds.ToArray().Contains(app.ApplicationId));

                ctx.Applications.DeleteAllOnSubmit(applicationsToDelete);

                ctx.SubmitChanges();
            }
            createdApplicationIds.Clear();


            Console.WriteLine("========Ending Clean Up====================");
        }

        [Story, Test]
        public void GetApplication()
        {
            _story = new Story("Get the Application");

            _story.AsA("User")
              .IWant("to be able to get an existing Application")
              .SoThat("I can modify the object");

            _story.WithScenario("Get the Application By correct application Name or application Id")
                .Given("an existing application Name or application Id", 
                    () =>
                     {
                         _ApplicationObject = new ApplicationObject() { Description = "Application", Name = "App" };
                         _applicationApi.Save(_ApplicationObject);

                         createdApplicationIds.Add(_ApplicationObject.Id);
                     })
                .When("I get this Application", 
                    () =>
                        {
                            _ApplicationObject1 = _applicationApi.Get(_ApplicationObject.Id);

                            _ApplicationObject2 = _applicationApi.Get(_ApplicationObject.Name);
                        })
                .Then("The Objects I get should be equal", () =>
                                                              {
                                                                  _ApplicationObject.Id.ShouldEqual(_ApplicationObject1.Id);
                                                                  //_ApplicationObject.Id.ShouldEqual(_ApplicationObject2.Id);

                                                              });

            this.CleanUp();

        }

        [Story, Test]
        public void GetApplicationWithError()
        {

            _story = new Story("Get the Application");

            _story.AsA("User")
                .IWant("to be able to get an existing Application")
                .SoThat("I can modify the object");
            Guid _temp = new Guid();
            _story.WithScenario("Get the Application By correct application Name or application Id")
                .Given("an existing application Name or application Id", () =>
                                                                             {

                                                                             })
                .When("I get this Application", () =>
                                                    {
                                                        _ApplicationObject1 = _applicationApi.Get(_temp);

                                                        _ApplicationObject2 = _applicationApi.Get("");
                                                    })
                .Then("The Objects I get should be equal", () =>
                                                               {
                                                                   _ApplicationObject1.ShouldBeNull();
                                                                   _ApplicationObject2.ShouldBeNull();
                                                                   ;

                                                               });

            this.CleanUp();


        }

        [Story, Test]
        public void ExistApplication()
        {
            _story = new Story("Judge if the Application exists");

            _story.AsA("User")
              .IWant("to be able to Judge if the Application exists")
              .SoThat("I can decide to create the application or not");

            _story.WithScenario("Judge the application ")
                .Given("an unexisting application Name ", () =>
                {
                    _ApplicationObject = new ApplicationObject() { Description = "Haha", Name = "App" };
                })
                .When("I get this Application", () =>
                {
                    _applicationApi.Exists("App").ShouldBeFalse();
                })
                .Then(" I can create the object and judge if it exists ", () =>
                {
                    _applicationApi.Save(_ApplicationObject);

                    createdApplicationIds.Add(_ApplicationObject.Id);

                    _applicationApi.Exists("App").ShouldBeTrue();
                });
            this.CleanUp();
        }
    }
}
