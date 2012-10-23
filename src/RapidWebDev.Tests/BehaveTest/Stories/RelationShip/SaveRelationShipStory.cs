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


namespace BaoJianSoft.Tests.BehaveTest.Stories.RelationShip
{

    [TestFixture, Theme]
    public class SaveRelationShipStory : SetupFixture

    {
        private IRelationshipApi _RelationshipApi;

        private RelationshipObject _RelationshipObject;

        private UserObject _User1;

        private UserObject _User2;

       // private IList<Guid> _createdRelationshipIds;

        private IList<Guid> _createUserIds;

        private BehaveMemberShipUtils _memberUtils;

        private IMembershipApi _MembershipApi;
        private Story _story;

        [SetUp]
        public void Setup()
        {
           // _createdRelationshipIds = new List<Guid>();

            _createUserIds = new List<Guid>();


            base.GlobalSetup();

            _MembershipApi = SpringContext.Current.GetObject<IMembershipApi>();
            _RelationshipApi = SpringContext.Current.GetObject<IRelationshipApi>();
        }

        public SaveRelationShipStory()
        {
            Console.WriteLine("=================Setup===================");

           // _createdRelationshipIds = new List<Guid>();

           
            _createUserIds = new List<Guid>();

            base.GlobalSetup();
            _MembershipApi = SpringContext.Current.GetObject<IMembershipApi>();

            _RelationshipApi = SpringContext.Current.GetObject<IRelationshipApi>();

            Console.WriteLine("============Ending Setup===================");
        }

        [TearDown]
        public void CleanUp()
        {
            try
            {
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
                {
                    var relationShips = ctx.Relationships.Where(x => 1 == 1);

                    ctx.Relationships.DeleteAllOnSubmit(relationShips);

                
                    var member = ctx.Memberships.Where(x => _createUserIds.ToArray().Contains(x.UserId));

                    var userToDelete = ctx.Users.Where(x => _createUserIds.ToArray().Contains(x.UserId));

                    ctx.Memberships.DeleteAllOnSubmit(member);
                    ctx.Users.DeleteAllOnSubmit(userToDelete);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception e) 
            {
                Console.Error.WriteLine(e.Message);
                throw e;
            }
        }

        [Story, Test]
       
        public void SaveRelationshipStories()
        {
            _story = new Story("Save Relationship among objects");

            _story.WithScenario("Save RelationShip between Users")
                .Given("Create an RelationShipOBject firstly", () =>
                {
                    _RelationshipObject = new RelationshipObject() { RelationshipType = "Parnter" };

                })
                .And("Create two Users", () =>
                {


                    _memberUtils = new BehaveMemberShipUtils();

                    _User1 = _memberUtils.CreateUserObject("Tim Yi", true);

                    _User2 = _memberUtils.CreateUserObject("Eunge", true);

                    _MembershipApi.Save(_User1, "123456Hello", "123456Hello");
                    _MembershipApi.Save(_User2, "123456Hello", "123456Hello");

                    _createUserIds.Add(_User1.UserId);
                    _createUserIds.Add(_User2.UserId);
                })
                .When("Save the relationshipobject with ", () =>
                {
                    _RelationshipApi.Save(_User2.UserId, _RelationshipObject);


                })
                .And("Save the relation between two Users , and the Relationship Type already have", () =>
                {
                    _RelationshipApi.Save(_User1.UserId, _User2.UserId, "Parnter");
                })
                .And("Save the relation between two Users , and the Relationship Type doesn't exist", () =>
                {
                    _RelationshipApi.Save(_User1.UserId, _User2.UserId, "Who Knows");
                })
                .And("Save the relation between one Users ", () =>
                {
                    _RelationshipApi.Save(_User1.UserId, _User1.UserId, "Parnter");
                })
                .Then("Get the relation ship ", () =>
                {
                    _RelationshipApi.GetOneToOne(_User2.UserId, _RelationshipObject.RelationshipType).RelationshipType.ShouldEqual("Parnter");
                })
                .WithScenario("Update Existing Relationship")
                .Given("An Existing RelationshipObject", () => 
                {

                })
                .And("Update the relationship type", () =>
                {
                    _RelationshipObject.RelationshipType = "NEWPARNTER";

                })
                .And("Save the relationship to User who already have this relationshipobject before", () =>
                {
                    _RelationshipApi.Save(_User2.UserId, _RelationshipObject);
                })
                .When("I get the relationship", () =>
                {
                    _RelationshipApi.GetOneToOne(_User2.UserId, _RelationshipObject.RelationshipType).RelationshipType.ShouldEqual("NEWPARNTER");
                }).
                Then("I can get the relationship")
                ;




            this.CleanUp();
        }
    }
}
