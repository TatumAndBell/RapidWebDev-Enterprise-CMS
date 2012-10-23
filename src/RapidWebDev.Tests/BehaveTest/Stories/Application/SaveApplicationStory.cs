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
    public class SaveApplicationStory : SetupFixture
    {

         private Story _story;

        private ApplicationObject _ApplicationObject;

        private ApplicationObject _ApplicationObject1;

         //private ApplicationObject _ApplicationObject2;

        IApplicationApi _applicationApi ;

        List<Guid> createdApplicationIds;

        [SetUp]
        public void Setup()
        {
            base.GlobalSetup();

            createdApplicationIds = new List<Guid>();

            _applicationApi = SpringContext.Current.GetObject<IApplicationApi>();
        }

        public  SaveApplicationStory()
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

        [Story,Test]
        public void SaveApplication()
        {
            _story = new Story("Save the object ");

            _story.AsA("User")
              .IWant("to be able to save the Application ")
              .SoThat("I can get  the application ");
           
            _story.WithScenario("Judge the application ")
                .Given("an unexisting application Name ", () =>
                {
                    _ApplicationObject = new ApplicationObject() { Description = "Haha", Name = "App" };
                })
                .When("I save this Application", () =>
                {
                    _applicationApi.Save(_ApplicationObject);

                    createdApplicationIds.Add(_ApplicationObject.Id);
                })
                .Then(" I can get the object and judge if it exists ", () =>
                {
                    _applicationApi.Exists("App").ShouldBeTrue();
                });

            this.CleanUp();
        }

        [Story, Test]
        public void SaveApplicationTwice()
        {
            _story = new Story("Save the object ");

            _story.AsA("User")
              .IWant("to be able to save the Application ")
              .SoThat("I can get  the application ");

            _story.WithScenario("Judge the application ")
                .Given("an unexisting application Name ", () =>
                {
                    _ApplicationObject = new ApplicationObject() { Description = "Haha", Name = "App" };
                })
                .When("I save this Application", () =>
                {
                    _applicationApi.Save(_ApplicationObject);
                    _applicationApi.Save(_ApplicationObject);

                    createdApplicationIds.Add(_ApplicationObject.Id);
                })
                .Then(" I can get the object and judge if it exists ", () =>
                {
                    _applicationApi.Exists("App").ShouldBeTrue();
                });

            this.CleanUp();
        }

        [Story, Test]
        public void UpdateApplication()
        {
            _story = new Story("Save the object ");

            _story.AsA("User")
              .IWant("to be able to save the Application ")
              .SoThat("I can get  the application ");

            _story.WithScenario("Judge the application ")
                .Given("an unexisting application Name ", () =>
                {
                    _ApplicationObject = new ApplicationObject() { Description = "Haha", Name = "App" };
                })
                .When("I save this Application", () =>
                {
                    _applicationApi.Save(_ApplicationObject);

                    _ApplicationObject1 = _applicationApi.Get(_ApplicationObject.Id);

                    _ApplicationObject.Name = " ";

                    _applicationApi.Save(_ApplicationObject);

                    createdApplicationIds.Add(_ApplicationObject.Id);

                    
                })
                .Then(" I can get the object and it's name be changed ", () =>
                {
                    _ApplicationObject.Name.ShouldEqual(" ");
                });
            this.CleanUp();
        }

    }
}
