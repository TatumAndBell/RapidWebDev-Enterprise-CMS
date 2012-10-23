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
using System.Collections.Generic;
using System.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.ExtensionModel;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using NUnit.Framework;

namespace RapidWebDev.Tests.Platform
{
	[TestFixture]
	[Description(@"We have geographic areas using hierarchy Api. 
							When we create an user, we want to set a geographic area in where the user born. 
							But the user data model and APIs don't have such support so we intend to use relationship Api.")]
	public class RelationshipApiTests
	{
		private static IMembershipApi membershipApi;
		private static IRelationshipApi relationshipApi = SpringContext.Current.GetObject<IRelationshipApi>();
		private static IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

		private const string RELATIONSHIP_TYPE = "GeographicAreaWhereUserBorn";
		private const string GEOGRAPHY = "Geography";
		private HierarchyDataObject shanghai;
		private HierarchyDataObject beijing;
		private HierarchyDataObject sichuan;
		private HierarchyDataObject chengdu;
		private HierarchyDataObject mianyang;

		private List<Guid> createdUserIds = new List<Guid>();

		[SetUp]
		public void SetupHierarchy()
		{
			IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			membershipApi = new MembershipApi(authenticationContext, organizationApi);

			this.shanghai = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Shang Hai" };
			hierarchyApi.Save(this.shanghai);

			this.beijing = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Bei Jing" };
			hierarchyApi.Save(this.beijing);

			this.sichuan = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Si Chuan" };
			hierarchyApi.Save(this.sichuan);

			this.chengdu = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Cheng Du", ParentHierarchyDataId = this.sichuan.HierarchyDataId };
			hierarchyApi.Save(this.chengdu);

			this.mianyang = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Mian Yang", ParentHierarchyDataId = this.sichuan.HierarchyDataId };
			hierarchyApi.Save(this.mianyang);
		}

		[TearDown]
		public void TearDown()
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				foreach (Guid createdUserId in this.createdUserIds)
				{
					ctx.Memberships.Delete(m => m.UserId == createdUserId);
					ctx.Users.Delete(u => u.UserId == createdUserId);
					ctx.SubmitChanges();
				}

				hierarchyApi.HardDeleteHierarchyData(this.shanghai.HierarchyDataId);
				hierarchyApi.HardDeleteHierarchyData(this.beijing.HierarchyDataId);
				hierarchyApi.HardDeleteHierarchyData(this.sichuan.HierarchyDataId);
				hierarchyApi.HardDeleteHierarchyData(this.chengdu.HierarchyDataId);
				hierarchyApi.HardDeleteHierarchyData(this.mianyang.HierarchyDataId);
			}

			this.createdUserIds.Clear();
		}

		[Test]
		[Description("The test case is set relationship b/w users and geographic areas then get users by geographic areas.")]
		public void UsersInGeographicAreasTest()
		{
			UserObject eunge = this.CreateUser("eunge", "Eunge Liu");
			relationshipApi.Save(eunge.UserId, this.chengdu.HierarchyDataId, RELATIONSHIP_TYPE);

			UserObject alex = this.CreateUser("alex", "Alex He");
			relationshipApi.Save(alex.UserId, this.chengdu.HierarchyDataId, RELATIONSHIP_TYPE);

			UserObject thomas = this.CreateUser("thomas", "Thomas Wang");
			relationshipApi.Save(thomas.UserId, this.shanghai.HierarchyDataId, RELATIONSHIP_TYPE);

			this.createdUserIds.AddRange(new[] { eunge.UserId, alex.UserId, thomas.UserId });

			IEnumerable<RelationshipObject> relatedUserIds = relationshipApi.GetOneToMany(this.chengdu.HierarchyDataId, RELATIONSHIP_TYPE);
			Assert.AreEqual(2, relatedUserIds.Count());
			Assert.AreEqual(1, relatedUserIds.Count(relatedUserId => relatedUserId.ReferenceObjectId == eunge.UserId));
			Assert.AreEqual(1, relatedUserIds.Count(relatedUserId => relatedUserId.ReferenceObjectId == alex.UserId));
		}

		[Test]
		[Description("Relationship saves the relationship on input two objects without order contraint.")]
		public void SaveRelationshipWithoutOrderConstraintTest()
		{
			UserObject eunge = this.CreateUser("eunge", "Eunge Liu");
			relationshipApi.Save(eunge.UserId, this.chengdu.HierarchyDataId, RELATIONSHIP_TYPE);

			UserObject alex = this.CreateUser("alex", "Alex He");
			relationshipApi.Save(this.chengdu.HierarchyDataId, alex.UserId, RELATIONSHIP_TYPE);

			this.createdUserIds.AddRange(new[] { eunge.UserId, alex.UserId });

			IEnumerable<RelationshipObject> relatedUserIds = relationshipApi.GetOneToMany(this.chengdu.HierarchyDataId, RELATIONSHIP_TYPE);
			Assert.AreEqual(2, relatedUserIds.Count());
		}

		private UserObject CreateUser(string userName, string displayName)
		{
			IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
			UserObject userObject = new UserObject
			{
				UserName = userName,
				DisplayName = displayName,
				OrganizationId = platformConfiguration.Organization.OrganizationId,
				IsApproved = true,
			};

			membershipApi.Save(userObject, "password1", null);
			this.createdUserIds.Add(userObject.UserId);

			return userObject;
		}
	}
}