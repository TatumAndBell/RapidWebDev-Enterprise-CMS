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
using System.Text;
using BaoJianSoft.Common;
using BaoJianSoft.Common.Data;
using BaoJianSoft.Platform;
using BaoJianSoft.Platform.Initialization;
using BaoJianSoft.Platform.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace BaoJianSoft.Tests.BehaveTest.Stories
{
    [TestFixture, Theme]
    public class MemberLoginStory : SetupFixture
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

        private LoginResults _ret;

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
        public void MemberLoginWithCorrectUserNameAndPwd()
        {
            _story = new Story("Login with UserName and Password");

            _story.AsA("User")
              .IWant("to be able to login ")
              .SoThat("I can use features");

            _story.WithScenario("login with a correct username and password")
                .Given("Create a new with old password", () =>
                {
                    _Uobject = _utils.CreateUserObject("Eunge Liu",true);

                    _oldpassword = "123Hello";

                    _newpassword = "Hello123456";

                    _membershipApi.Save(_Uobject, _oldpassword, "123Hello");

                    createdObjectIds.Add(_Uobject.UserId);

                })
                .When("I login", () =>
                {
                    _ret = _membershipApi.Login(_Uobject.UserName, _oldpassword);
                })
                .Then("The User can get Successful Logon",
                        () =>
                        {
                            _ret.ShouldEqual(LoginResults.Successful);
                        });

            this.CleanUp();
        
        
        }

        [Story, Test]
        public void MemberLoginWithCorrectUserNameAndWrongPwd()
        {
            _story = new Story("Login with UserName and Password");

            _story.AsA("User")
              .IWant("to be able to login ")
              .SoThat("I can use features");

            _story.WithScenario("login with a correct username and wrong password")
                .Given("Create a new with password", () =>
                {
                    _Uobject = _utils.CreateUserObject("Eunge Liu",true);

                    _oldpassword = "123Hello";

                    _newpassword = "Hello123456";

                    _membershipApi.Save(_Uobject, _oldpassword, "123Hello");

                    createdObjectIds.Add(_Uobject.UserId);

                })
                .When("I login", () =>
                {
                    _ret = _membershipApi.Login(_Uobject.UserName, _newpassword);
                })
                .Then("The User can get InvalidCredential Logon",
                        () =>
                        {
                            _ret.ShouldEqual(LoginResults.InvalidCredential);
                        });

            this.CleanUp();
        
        }

        [Story, Test]
        public void MemberLoginWithWrongUserNameAndPwd()
        {
        
        }

        [Story, Test]
        public void MemberLoginWithCorrectUserNameAndPwdAndUserFalseApproved()
        {

            _story = new Story("Login with UserName and Password");

            _story.AsA("User")
              .IWant("to be able to login ")
              .SoThat("I can use features");

            _story.WithScenario("login with a correct username and wrong password")
                .Given("Create a new with password", () =>
                {
                    _Uobject = _utils.CreateUserObject("Eunge Liu", false);

                    _oldpassword = "123Hello";

                    _newpassword = "Hello123456";

                    _membershipApi.Save(_Uobject, _oldpassword, "123Hello");

                    createdObjectIds.Add(_Uobject.UserId);

                })
                .When("I login", () =>
                {
                    _ret = _membershipApi.Login(_Uobject.UserName, _newpassword);
                })
                .Then("The User can get InvalidCredential Logon",
                        () =>
                        {
                            _ret.ShouldEqual(LoginResults.InvalidCredential);
                        });

            this.CleanUp();
        }

        [Story, Test]
        public void MemberLoginWithCorrectUserNameAndPwdAndDisableOrganzation()
        { }


    }
}
