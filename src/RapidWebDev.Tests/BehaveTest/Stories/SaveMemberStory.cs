using System;
using NUnit.Framework;
using AspNetMembership = System.Web.Security.Membership;
using NBehave.Spec.NUnit;
using NBehave.Narrator.Framework;
using BaoJianSoft.Platform;

using BaoJianSoft.Common;

using BaoJianSoft.Platform.Initialization;



namespace BaoJianSoft.Tests.BehaveTest.Stories
{
    [TestFixture, Theme("Save Member")]
    public class SaveMemeberStory : SetupFixture
    {
        private Story _story;

        private UserObject _Uobject;

        private IMembershipApi _membershipApi;

        private string _password;

        private string _passwordAnswer;

        public SaveMemeberStory()
        {
            base.GlobalSetup();

            _story = new Story("Save Memeber");
            _story.AsA("User")
              .IWant("to be able to save member status")
              .SoThat("I can update or create member info");

            _membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
        }

        [Story, Test]
        public void SaveANewMemberWithNullUser()
        {
          
            
            
            _story.WithScenario("Save a new Member with  empty properties' value")
                .Given("the new member with nothing", () => { _Uobject = new UserObject(); })
                .When("I save this member")
                .Then("I Get an Excepton from ", () => typeof(ArgumentNullException).ShouldBeThrownBy(() => _membershipApi.Save(_Uobject, _password, _passwordAnswer)));

        }

        [Story, Test]
        public void SaveANewMemberWithValues() 
        {
            _story.WithScenario("Save a new Member with value")
                .Given("the new member with nothing", () => { _Uobject = CreateUserObject(); })
                .And("Give the Password and PasswordAnser", () => { _password = "123Hello"; _passwordAnswer = "123Hello"; })
                .When("I save this member", () => _membershipApi.Save(_Uobject, _password, _passwordAnswer))
                .Then("I Get an new User ", () => _Uobject.ApplicationId.ShouldNotBeNull());
 
        }


        [Story, Test]
        public void SaveANewMemberWithValuesAndEmptyPassword()
        {
            _story.WithScenario("Save a new Member with value")
                .Given("the new member with nothing", () => { _Uobject = CreateUserObject(); })
                .And("Give the Password and PasswordAnser", () => { _password = ""; _passwordAnswer = ""; })
                .When("I save this member")
                .Then("I Get an new User ", () => typeof(ArgumentNullException).ShouldBeThrownBy(() => _membershipApi.Save(_Uobject, _password, _passwordAnswer)));


        }


        private UserObject CreateUserObject() 
        {
            IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();

            UserObject userObject = new UserObject
            {
                OrganizationId = platformConfiguration.Organization.OrganizationId,
                Comment = "IT specialist",
                DisplayName = "Eunge Liu",
                Email = "eunge.liu@gmail.com",
                IsApproved = true,
                MobilePin = "137641855XX",
                UserName = "Eunge"
            };

            userObject["Birthday"] = new DateTime(1982, 2, 7);
            userObject["Sex"] = "Male";
            userObject["IdentityNo"] = "51010419820207XXXX";
            userObject["EmployeeNo"] = "200708200002";
            userObject["Department"] = "Simulation";
            userObject["Position"] = "Team Lead";
            userObject["PhoneNo"] = "021-647660XX";
            userObject["City"] = "ShangHai";
            userObject["Address"] = "MeiLong 2nd Cun, MingHang District";
            userObject["ZipCode"] = "210000";

            return userObject;
        }
    }
}
