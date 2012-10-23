using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NBehave.Spec.NUnit;
using NBehave.Narrator.Framework;

namespace BaoJianSoft.Tests.BehaveTest.Stories
{
    [TestFixture,Theme]
    public class BulkGetMembersStory
    {
        private Story _story;

        [SetUp]
        public void Setup() 
        {
            _story = new Story("");
        }

        [Test]
        public void BulkGetWithCorrectUserId() 
        {

        }
    }
}
