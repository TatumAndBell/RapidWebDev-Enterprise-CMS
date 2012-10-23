/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Eunge, Legal Name: Jian Liu, Email: eunge.liu@RapidWebDev.org

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
using System.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using NUnit.Framework;

namespace RapidWebDev.Tests.Platform
{
	[TestFixture]
	public class SequenceNoApiTests
	{
		[TearDown]
		public void TearDown()
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
				ctx.SequenceNos.Delete(s => s.ApplicationId == authenticationContext.ApplicationId);
				ctx.SubmitChanges();
			}
		}

		[Test, Description("Create sequence no test.")]
		public void CreateSequenceNoTest()
		{
			ISequenceNoApi sequenceNoApi = SpringContext.Current.GetObject<ISequenceNoApi>();

			Guid objectId = Guid.NewGuid();
			long roles1 = sequenceNoApi.Create(objectId, "Roles");
			long roles2 = sequenceNoApi.Create(objectId, "Roles");
			long users1 = sequenceNoApi.Create(objectId, "Users");

			Assert.AreEqual(1, roles1);
			Assert.AreEqual(2, roles2);
			Assert.AreEqual(1, users1);
		}
	}
}

