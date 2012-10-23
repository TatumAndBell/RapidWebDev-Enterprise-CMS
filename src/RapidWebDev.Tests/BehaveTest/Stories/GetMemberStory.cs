using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaoJianSoft.Common;
using BaoJianSoft.Platform;
using NBehave.Narrator.Framework;
using NUnit.Framework;

namespace BaoJianSoft.Tests.BehaveTest.Stories
{
    [TestFixture,Theme("Get Member")]
    public class GetMemberStory
    {
        private Story _story;

        private IMembershipApi membershipApi; 

        [SetUp]
        public void Setup()
        {
            _story = new Story("Get the Members");
            membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
        }

        [Test]
        public void GetMemberById()
        {
            _story.AsA("User")
                .IWant("Get the Member ")
                .SoThat("I can set the Member's properties");

            _story.WithScenario("Get the Member By Id")
                .Given("a existing member'id")
                .When("I call the Membership API to get")
                .Then("I get the Member Account");
        }
    }
}
