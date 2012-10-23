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

namespace BaoJianSoft.Tests.BehaveTest.Stories
{
    [TestFixture, Theme]
    public class BulkGetMembersStory : SetupFixture
    {
        private Story _story;

        private UserObject _Uobject;

        private UserObject _Uobject2;

        private IMembershipApi _membershipApi;

        //private string _password;

        //private string _passwordAnswer;

        private IList<Guid> createdObjectIds;

        private BehaveMemberShipUtils _utils;

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("=================Setup===================");

            createdObjectIds = new List<Guid>();

            base.GlobalSetup();

            _utils = new BehaveMemberShipUtils();

            _membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

            Console.WriteLine("============Ending Setup===================");
        }

        

        [TearDown]
        public void CleanUp()
        {
            Console.WriteLine("============Clean Up====================");
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                foreach (Guid createdObjectId in createdObjectIds)
                {
                    ctx.Memberships.Delete(m => m.UserId == createdObjectId);
                    ctx.Users.Delete(u => u.UserId == createdObjectId);
                    ctx.SubmitChanges();
                }
            }

            createdObjectIds.Clear();
            Console.WriteLine("========Ending Clean Up====================");
        }

        [Story,Test]
        
        public void BulkGetWithCorrectUserId()
        {
            _story = new Story("Get Memebers By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to get members, and give the correct users' GUID")
              .SoThat("I can do somethings for members");


            IDictionary<Guid, UserObject> _Users = new Dictionary<Guid, UserObject>();

            _story.WithScenario("Get members by ImembershipApi with the correct Ids")
                .Given("More than one correct GUID of users", () =>
                {
                    _Uobject = _utils.CreateUserObject("Eunge Liu",true);
                    _Uobject2 = _utils.CreateUserObject("Tim Yi", true);

                    

                    _membershipApi.Save(_Uobject, "123Hello", "123Hello");
                    _membershipApi.Save(_Uobject2, "123Hello", "123Hello");

                    createdObjectIds.Add(_Uobject.UserId);
                    createdObjectIds.Add(_Uobject2.UserId);

                })
                .When("I use the BulkGet, I can get users ", () => _Users = _membershipApi.BulkGet(createdObjectIds))
                .Then("The return a bunch of Users have contain the User1 and User2",
                        () =>
                        {
                            _Users.ContainsKey(_Uobject.UserId).ShouldBeTrue();
                            _Users.ContainsKey(_Uobject2.UserId).ShouldBeTrue();
                        });

            this.CleanUp();
        }

        [Story, Test]
        
        public void BulkGetWithOneCorrectUserId()
        {
            _story = new Story("Get Memebers By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to get members, and give only one correct users' GUID")
              .SoThat("I can do somethings for members");


            IDictionary<Guid, UserObject> _Users = new Dictionary<Guid, UserObject>();

            Guid _temp = new Guid();

            _story.WithScenario("Get members by ImembershipApi with the only one correct Ids")
                .Given("only one correct GUID of user and fake one", () =>
                {
                    _Uobject = _utils.CreateUserObject("Eunge Liu", true);
                   
                    _membershipApi.Save(_Uobject, "123Hello", "123Hello");

                    createdObjectIds.Add(_Uobject.UserId);
                    createdObjectIds.Add(_temp);
                })
                .When("I use the BulkGet, I can get users ", () => _Users = _membershipApi.BulkGet(createdObjectIds))
                .Then("The return a bunch of Users have contain only the User1 ",
                        () =>
                        {
                            _Users[_Uobject.UserId].ShouldEqual(_Uobject);

                            _Users[_temp].ShouldBeNull();
                        });

            //createdObjectIds.Remove(_temp);

            this.CleanUp();

        }

        [Story, Test]
        public void BulkGetUsersWithWrongGUID()
        {
            _story = new Story("Get Memebers By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to get members, and give wrong users' GUID")
              .SoThat("I can get nothing");


            IDictionary<Guid, UserObject> _Users = new Dictionary<Guid, UserObject>();

            Guid _temp = new Guid();

            _story.WithScenario("Get members by ImembershipApi with the only one correct Ids")
                .Given("only one correct GUID of user and fake one", () =>
                {   
                    createdObjectIds.Add(_temp);
                })
                .When("I use the BulkGet, I can get users ", () => _Users = _membershipApi.BulkGet(createdObjectIds))
                .Then("The return a bunch of Users only contain null User ",
                        () =>
                        {
                            _Users[_temp].ShouldBeNull();
                        });

            //createdObjectIds.Remove(_temp);

            this.CleanUp();
        }

    }
}
