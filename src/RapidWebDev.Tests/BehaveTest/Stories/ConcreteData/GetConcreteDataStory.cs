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

//using NBehave.Narrator.Framework;
//using NUnit.Framework;


namespace BaoJianSoft.Tests.BehaveTest.Stories.ConcreteData
{

    [TestFixture, Theme]
    public class GetConcreteDataStory :SetupFixture



    {
        private Story _story;

        private IConcreteDataApi _ConcreteDataApi;

        private ConcreteDataObject _ConcreteDataObject;

        //private ConcreteDataObject _ConcreteDataObject1;

        //private ConcreteDataObject _ConcreteDataObject2;


        private ConcreteDataObject _ConcreteDataObject3;

        private IList<Guid> createdConcreteDataIds;

        [SetUp]
        public void Setup()
        {
            base.GlobalSetup();

            createdConcreteDataIds = new List<Guid>();

            _ConcreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();

        }

        public  GetConcreteDataStory()
        {
            Console.WriteLine("=================Setup===================");


            base.GlobalSetup();

            createdConcreteDataIds = new List<Guid>();

            _ConcreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();

            Console.WriteLine("============Ending Setup===================");
        }
        [TearDown]
        public void CleanUp()
        {
            Console.WriteLine("============Clean Up====================");

            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                var concreteDataToDelete =
                    ctx.ConcreteDatas.Where(da => createdConcreteDataIds.ToArray().Contains(da.ConcreteDataId));

                ctx.ConcreteDatas.DeleteAllOnSubmit(concreteDataToDelete);

                ctx.SubmitChanges();
            }
            createdConcreteDataIds.Clear();

            Console.WriteLine("========Ending Clean Up====================");
        }

        [Story, Test]
        public void GetConcreteDataStories()
        {
            _story = new Story("Get the ConcreteDataObject ");

            _story.AsA("User")
                .IWant("to be able to get the ConcreteDataObject ")
                .SoThat("I can get  the ConcreteDataObject ");

            string _conCreteDataName = "";
            _story.WithScenario("Get the ConcreteDataObject ")
                .Given("an unexisting ConcreteDataObject Name ", () =>
                                                                     {

                                                                     })
                .When("Call the IConcreteDataApi to get ConcreteDataObject",()=>
                                                                                {
                                                                                   
                                                                                       
                                                                                })
                .Then("I got a specific exception", ()=>typeof(ArgumentNullException).ShouldBeThrownBy(()=> _ConcreteDataApi.GetByName("",
                                                                                                                   _conCreteDataName)));
           
        }

        
        [Story, Test]
        public void GetConcreteDataStoriesByTypes()
        {
            _story = new Story("Get the ConcreteDataObject ");

            _story.AsA("User")
                .IWant("to be able to get the ConcreteDataObject ")
                .SoThat("I can get  the ConcreteDataObject ");
            IEnumerable<ConcreteDataObject> _temps;
            
            _story.WithScenario("Get the ConcreteDataObject ")
                .Given("a set of ConcreteDataObject Name ", () =>
                {
                    _ConcreteDataObject = new ConcreteDataObject()
                    {

                        Type = "Color",
                        Description = "Present",
                        Name = "Color",
                        Value = "Red;Yellow;Purple;"
                    };

                    _ConcreteDataObject3 = new ConcreteDataObject()
                    {

                        Type = "Color",
                        Description = "Present",
                        Name = "Color1",
                        Value = "Red;Yellow;Purple;"
                    };
                })
                .When("Call the IConcreteDataApi to save ConcreteDataObject", () =>
                {
                    _ConcreteDataApi.Save(_ConcreteDataObject);

                    createdConcreteDataIds.Add(_ConcreteDataObject.Id);

                    _ConcreteDataApi.Save(_ConcreteDataObject3);

                    createdConcreteDataIds.Add(_ConcreteDataObject3.Id);
                })
                .Then("I got  ConcreteDataObject", () => 
                {
                    _temps = _ConcreteDataApi.FindAllByType("Color");

                    _temps.Contains(_ConcreteDataObject).ShouldBeTrue();

                    _temps.Contains(_ConcreteDataObject3).ShouldBeTrue();
                });

            this.CleanUp();
        }

    }
}
