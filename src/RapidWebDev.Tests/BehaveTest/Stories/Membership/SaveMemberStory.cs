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
using BaoJianSoft.Platform.Initialization;
using BaoJianSoft.Platform.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using AspNetMembership = System.Web.Security.Membership;



namespace BaoJianSoft.Tests.BehaveTest.Stories
{
    [TestFixture, Theme("Save Member By IMembershipApi ")]
    public class SaveMemeberStory : SetupFixture
    {
        private Story _story;
        private UserObject _Uobject;
        private UserObject _Uobject2;
        private IMembershipApi _membershipApi;
        private string _password;
        private string _passwordAnswer;
        private BehaveMemberShipUtils _utils;
        private IList<Guid> createdObjectIds ;

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

    

        [Story,Test]
        public void SaveANewMemberWithEmptyProperties()
        {
            _story = new Story("Save a Memeber By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to create a new member, but I don't assign value to it")
              .SoThat("I can get specific exception");

            _story.WithScenario("Save a new Member which have no value")
                .Given("the new member with nothing", () => { _Uobject = new UserObject(); })
                .When("I save this member", () => { })
                .Then("I Get an ArgumentNullException from IMembershipApi.Save()", () => typeof(ArgumentNullException).ShouldBeThrownBy(() => _membershipApi.Save(_Uobject, _password, _passwordAnswer)));
        }

        [Story,Test]
        
        public void SaveANewMemberWithValues() 
        {
            _story = new Story("Save a Memeber By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to create a new member with Values by IMembershipApi")
              .SoThat("I can create member info by myself");

            _story.WithScenario("Save a new Member with values")
                .Given("the new member with default value", () => { _Uobject = _utils.CreateUserObject("Eunge Liu", true); })
                .And("Give the Password and PasswordAnser", () => { _password = "123Hello"; _passwordAnswer = "123Hello"; })
                .When("I save this member", () => _membershipApi.Save(_Uobject, _password, _passwordAnswer))
                .Then("I Get a new User's GUID, and the GUID is not null ", () => _Uobject.ApplicationId.ShouldNotBeNull());

            createdObjectIds.Add(_Uobject.UserId);
            this.CleanUp();
 
        }


        [Story,Test]
        
        public void SaveANewMemberWithValuesAndEmptyPassword()
        {
            _story = new Story("Save a Memeber By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to create a new member with value, but Don't follow the password setting rule ")
              .SoThat("I can get the specific exception");

            _story.WithScenario("Save a new Member with value")
                .Given("the new member with nothing", () => { _Uobject = _utils.CreateUserObject("Eunge Liu", true); })
                .And("Give the Password and PasswordAnser", () => { _password = ""; _passwordAnswer = ""; })
                .When("I save this member", () => { })
                .Then("I Get an ArgumentNullException  from IMembershipApi.Save()", () => 
					{
						typeof(ArgumentNullException).ShouldBeThrownBy(() => _membershipApi.Save(_Uobject, _password, _passwordAnswer));
					});
        }

        [Story,Test]
        
        public void SaveMoreThanOneMembersWithSameName()
        {
            _story = new Story("Save a Memeber By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to create more than one member with value, but give the same display name to them ")
              .SoThat("I can get the specific exception");

            _story.WithScenario("Save more than Members with values")
                .Given("the new member with default value", () =>
					{
						_Uobject = _utils.CreateUserObject("Eunge Liu", true);
						_Uobject2 = _utils.CreateUserObject("Eunge Liu", true);
					})
                .And("Give the Password and PasswordAnser", () => 
					{ 
						_password = "123Hello"; 
						_passwordAnswer = "123Hello"; 
					})
                .When("I save the first members", () => 
					{
						_membershipApi.Save(_Uobject, _password, _passwordAnswer);                                
					})
				.Then("I can get ValidationException when I save the second User with the same name of the first one ", () => 
					{ 
						typeof(ValidationException).ShouldBeThrownBy(() => _membershipApi.Save(_Uobject2, _password, _passwordAnswer)); 
					});

            createdObjectIds.Add(_Uobject.UserId);
            this.CleanUp();
        }

        [Story,Test]
        
        public void UpdateOldMemberWithChangeValues()
        {
            _story = new Story("Save a Memeber By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to update an old member which existed ")
              .SoThat("I can get the specific value after update it");

            _Uobject = _utils.CreateUserObject("Eunge Liu", true);
            _membershipApi.Save(_Uobject, "123Hello", "123Hello");

            _story.WithScenario("Update an User's name")
                .Given("Get a existing User", ()=>_Uobject2 = _membershipApi.Get(_Uobject.UserId))
                .And("Change it's name",()=>_Uobject.DisplayName = "YILO")
                .When("I save the member", () => _membershipApi.Save(_Uobject, "", "123Hello"))
                .Then("I can know the display name of _user1 and _user2 are different",()=> _Uobject.DisplayName.ShouldNotEqual(_Uobject2.DisplayName));

            createdObjectIds.Add(_Uobject.UserId);
            this.CleanUp();
        }

        [Story, Test]
        
        public void UpdateOldMemberWithAddNewProperties()
        {

            _story = new Story("Save a Memeber By IMembershipApi");

            _story.AsA("User")
              .IWant("to be able to update an old member which existed, and add a new property for it ")
              .SoThat("I can get the specific value after update it");

            _Uobject = _utils.CreateUserObject("Eunge Liu", true);
            _membershipApi.Save(_Uobject, "123Hello", "123Hello");

            _story.WithScenario("Update an User with add new dynamic Field")
                .Given("Get a existing User")
                .And("Add a new dynamic Field for _Uoject", () => _Uobject = _utils.AddExtensionFields(_Uobject))
                .When("I save the user _Uobject", () => _membershipApi.Save(_Uobject, "", null))
                .Then("I can get the dynamic Field's valule", () => _Uobject["YILO"].ShouldEqual("YILO"));

            createdObjectIds.Add(_Uobject.UserId);
            this.CleanUp();
        }
    }
}
