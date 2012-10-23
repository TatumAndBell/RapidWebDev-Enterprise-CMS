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
using BaoJianSoft.Common.Validation;
using BaoJianSoft.Platform;
using BaoJianSoft.Platform.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace BaoJianSoft.Tests.BehaveTest.Stories
{
    [TestFixture,Theme]
    public class MemberChangePwdStory : SetupFixture
    {
         private Story _story;

        private UserObject _Uobject;

        //private UserObject _Uobject2;

        private IMembershipApi _membershipApi;

        private string _oldpassword;

        private string _newpassword;

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


        [Story, Test]
        public void ChangeUserPasswordWithCorrectGuid() 
        {
            _story = new Story("Change Memebers' password By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to change the password")
              .SoThat("I can use new password to login");

            _story.WithScenario("Change the password with a correct GUID")
                .Given("Create a new with old password", () =>
                {
                    _Uobject = _utils.CreateUserObject("Eunge Liu", true);

                    _oldpassword = "123Hello";

                    _newpassword = "Hello123456";

                    _membershipApi.Save(_Uobject, _oldpassword, "123Hello");

                    createdObjectIds.Add(_Uobject.UserId);

                })
                .When("I change the password ", () => 
                        { 
                    _membershipApi.ChangePassword(_Uobject.UserId, _oldpassword, _newpassword); 
                        })
                .Then("The User can use new password to login",
                        () =>
                        {
                            _membershipApi.Login(_Uobject.UserName, _newpassword).ShouldEqual(LoginResults.Successful);
                        });

            this.CleanUp();
        }

        [Story, Test]
        public void ChangeUserPasswordWithWrongGuid()
        {

            _story = new Story("Change Memebers' password By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to change the password")
              .SoThat("I can use new password to login");

            Guid _temp = new Guid();

            _story.WithScenario("Change the password with a Incorrect GUID")
                .Given("Create a new with old password", () =>
                {

                    _oldpassword = "123Hello";

                    _newpassword = "Hello123456";

                    createdObjectIds.Add(_temp);

                })
                .When("I change the password ", () =>
                {
                    
                })
                .Then("I can get Error Message from ChangePassword method",
                        () =>
                        {
                            typeof(NullReferenceException).ShouldBeThrownBy(()=>_membershipApi.ChangePassword(_temp, _oldpassword, _newpassword));
                        });

            this.CleanUp();
        }


        [Story, Test]
        public void ChangeUserPasswordObeyPasswordRule() 
        {
            _story = new Story("Change Memebers' password By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to change the password")
              .SoThat("I can use new password to login");

            _story.WithScenario("Change the password with a correct GUID")
                .Given("Create a new User with old password, but the new password doesn't follow the password rule", () =>
                {
                    _Uobject = _utils.CreateUserObject("Eunge Liu", true);

                    _oldpassword = "123Hello";

                    _newpassword = "0";

                    _membershipApi.Save(_Uobject, _oldpassword, "123Hello");

                    createdObjectIds.Add(_Uobject.UserId);

                })
                .When("I change the password ", () =>
                {
                   
                })
                .Then("I can get Error Message from ChangePassword method",
                        () =>
                        {
							typeof(ValidationException).ShouldBeThrownBy(() => _membershipApi.ChangePassword(_Uobject.UserId, _oldpassword, _newpassword));
                        });

            this.CleanUp();
        }
    }
}
