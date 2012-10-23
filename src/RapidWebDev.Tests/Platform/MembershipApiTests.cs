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
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Validation;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace RapidWebDev.Tests.Platform
{
	[TestFixture]
	public class MembershipApiTests
	{
		private List<Guid> createdObjectIds = new List<Guid>();

		[TearDown]
		public void TearDown()
		{
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
		}

		[Test, Description("Basic membership CRUD test")]
		public void BasicTest()
		{
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

			Guid userId = CreateUserWithExtensionProperties(membershipApi);
			createdObjectIds.Add(userId);

			Assert.IsNotNull(membershipApi.Get("Eunge"));

			UserObject resolvedUserObject = membershipApi.Get(userId);
			Assert.AreEqual("Eunge", resolvedUserObject.UserName);
			Assert.AreEqual("eunge.liu@gmail.com", resolvedUserObject.Email);
			Assert.AreEqual("Eunge Liu", resolvedUserObject.DisplayName);
			Assert.AreEqual("IT specialist", resolvedUserObject.Comment);
			Assert.IsTrue(resolvedUserObject.IsApproved);

			Assert.AreEqual(new DateTime(1982, 2, 7), resolvedUserObject["Birthday"]);
			Assert.AreEqual("Male", resolvedUserObject["Sex"]);
			Assert.AreEqual("51010419820207XXXX", resolvedUserObject["IdentityNo"]);
			Assert.AreEqual("200708200002", resolvedUserObject["EmployeeNo"]);
			Assert.AreEqual("Simulation", resolvedUserObject["Department"]);
			Assert.AreEqual("Team Lead", resolvedUserObject["Position"]);
			Assert.AreEqual("021-647660XX", resolvedUserObject["PhoneNo"]);
			Assert.AreEqual("ShangHai", resolvedUserObject["City"]);
			Assert.AreEqual("MeiLong 2nd Cun, MingHang District", resolvedUserObject["Address"]);
			Assert.AreEqual("210000", resolvedUserObject["ZipCode"]);

			resolvedUserObject = membershipApi.Get("Eunge");
			Assert.AreEqual(userId, resolvedUserObject.UserId);

			resolvedUserObject["PhoneNo"] = "1376418AAAA";
			membershipApi.Save(resolvedUserObject, null, null);
			resolvedUserObject = membershipApi.Get(userId);
			Assert.AreEqual("Eunge", resolvedUserObject.UserName);
			Assert.AreEqual("eunge.liu@gmail.com", resolvedUserObject.Email);
			Assert.AreEqual("1376418AAAA", resolvedUserObject["PhoneNo"]);

			Assert.AreEqual(LoginResults.Successful, membershipApi.Login("eunge", "password1"));

			membershipApi.ChangePassword(userId, "password1", "password2");
			Assert.AreEqual(LoginResults.Successful, membershipApi.Login("eunge", "password2"));
		}

		[Test, Description("Update body of a user from nonnull to null test")]
		public void SaveUserWithOutExtensionProperties()
		{
			MockRepository mockRepository = new MockRepository();
			IOrganizationApi organizationApi = mockRepository.StrictMock<IOrganizationApi>();
			SetupResult.For(organizationApi.GetOrganization(Guid.Empty)).IgnoreArguments().Return(new OrganizationObject());
			mockRepository.ReplayAll();

			UserObject userObject = new UserObject
			{
				Comment = "IT specialist",
				DisplayName = "Eunge Liu",
				Email = "eunge.liu@gmail.com",
				IsApproved = true,
				MobilePin = "137641855XX",
				UserName = "Eunge"
			};

			IMembershipApi membershipApi = new MembershipApi(SpringContext.Current.GetObject<IAuthenticationContext>(), organizationApi);
			membershipApi.Save(userObject, "password1", null);
			createdObjectIds.Add(userObject.UserId);

			userObject = membershipApi.Get(userObject.UserId);
			userObject.DisplayName = "Eunge";
			membershipApi.Save(userObject, null, null);

			userObject = membershipApi.Get(userObject.UserId);
			Assert.AreEqual("Eunge", userObject.DisplayName);
		}

		[Test, Description("Save a user with existed display name test - duplicate display name is allowed.")]
		[ExpectedException(typeof(ValidationException))]
		public void SaveUserWithExistedDisplayNameTest()
		{
			MockRepository mockRepository = new MockRepository();
			IOrganizationApi organizationApi = mockRepository.StrictMock<IOrganizationApi>();
			SetupResult.For(organizationApi.GetOrganization(Guid.Empty)).IgnoreArguments().Return(new OrganizationObject());
			mockRepository.ReplayAll();

			UserObject userObject = new UserObject
			{
				Comment = "IT specialist",
				DisplayName = "Eunge Liu",
				Email = "eunge.liu@gmail.com",
				IsApproved = true,
				MobilePin = "137641855XX",
				UserName = "Eunge"
			};

			IMembershipApi membershipApi = new MembershipApi(SpringContext.Current.GetObject<IAuthenticationContext>(), organizationApi);
			membershipApi.Save(userObject, "password1", null);
			createdObjectIds.Add(userObject.UserId);
			Assert.IsNotNull(membershipApi.Get(userObject.UserName));

			UserObject duplicateUserObject = new UserObject
			{
				Comment = "IT specialist",
				DisplayName = "Eunge Liu",
				Email = "lucy.liu@gmail.com",
				IsApproved = true,
				MobilePin = "137641855XX",
				UserName = "Lucy"
			};

			membershipApi.Save(duplicateUserObject, "password1", null);
		}

		[Test, Description("Add multiple users first, then validate the query against them.")]
		public void FindUserTest()
		{
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
			this.CreateUser(membershipApi);
			this.CreateUser(membershipApi);
			this.CreateUser(membershipApi);
			this.CreateUser(membershipApi);

			int recordCount;
			LinqPredicate predicate = new LinqPredicate("UserName!=@0 AND UserName!=@1", "Admin", "Anonymous");
			IEnumerable<UserObject> userObjects = membershipApi.FindUsers(predicate, "UserName", 0, 10, out recordCount);
			Assert.AreEqual(4, recordCount);
			Assert.AreEqual(4, userObjects.Count());

			userObjects = membershipApi.FindUsers(predicate, "DisplayName", 0, 3, out recordCount);
			Assert.AreEqual(4, recordCount);
			Assert.AreEqual(3, userObjects.Count());
		}

		[Test, Description("Test ICache usage while adding an user by membership APIs.")]
		public void CacheCallingWhenAddUserTest()
		{
			IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

			MockRepository mockRepository = new MockRepository();
			IOrganizationApi organizationApi = mockRepository.StrictMock<IOrganizationApi>();
			ICache cacheInstance = mockRepository.StrictMock<ICache>();
			MembershipApi membershipApi = new MembershipApi(authenticationContext, organizationApi) { Cache = cacheInstance };

			using (mockRepository.Record())
			using (mockRepository.Ordered())
			{
				organizationApi.Expect(api => api.GetOrganization(Guid.Empty)).IgnoreArguments().Return(new OrganizationObject());

				// remove the cache.
				cacheInstance.Remove(null);
				LastCall.IgnoreArguments();
			}

			using (mockRepository.Playback())
			{
				this.CreateUser(membershipApi);
			}
		}

		[Test, Description("Test ICache usage while updating an existed user by membership APIs.")]
		public void CacheCallingWhenUpdateUserTest()
		{
			IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
			MockRepository mockRepository = new MockRepository();
			IOrganizationApi organizationApi = mockRepository.StrictMock<IOrganizationApi>();
			SetupResult.For(organizationApi.GetOrganization(Guid.Empty)).IgnoreArguments().Return(new OrganizationObject());
			mockRepository.ReplayAll();

			MembershipApi membershipApi = new MembershipApi(authenticationContext, organizationApi);
			Guid userId = this.CreateUser(membershipApi);
			UserObject userObject = membershipApi.Get(userId);

			organizationApi = mockRepository.StrictMock<IOrganizationApi>();
			ICache cacheInstance = mockRepository.StrictMock<ICache>();
			membershipApi = new MembershipApi(authenticationContext, organizationApi);
			membershipApi.Cache = cacheInstance;

			using (mockRepository.Record())
			using (mockRepository.Ordered())
			{
				organizationApi.Expect(api => api.GetOrganization(Guid.Empty)).IgnoreArguments().Return(new OrganizationObject());

				// remove the cache for original user id
				cacheInstance.Expect(cache => cache.Remove(null)).IgnoreArguments();
			}

			using (mockRepository.Playback())
			{
				userObject.UserName = "Liue";
				membershipApi.Save(userObject, "password1", null);
			}
		}

		[Test, Description("Test ICache usage while getting an existed user by id through membership APIs.")]
		public void CacheCallingWhenGetUserByIdTest()
		{
			IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

			MockRepository mockRepository = new MockRepository();
			ICache cacheInstance = mockRepository.StrictMock<ICache>();
			IOrganizationApi organizationApi = mockRepository.DynamicMock<IOrganizationApi>();
			SetupResult.For(organizationApi.GetOrganization(Guid.Empty)).IgnoreArguments().Return(new OrganizationObject());
			mockRepository.ReplayAll();

			MembershipApi membershipApi = new MembershipApi(authenticationContext, organizationApi);
			Guid userId = this.CreateUser(membershipApi);

			membershipApi.Cache = cacheInstance;

			using (mockRepository.Record())
			using (mockRepository.Ordered())
			{
				Action<MethodInvocation> methodInvocationCallback = methodInvocation =>
				{
					Assert.AreEqual(1, methodInvocation.Arguments.Length);
					Assert.AreEqual(typeof(string), methodInvocation.Arguments[0].GetType());
				};

				// get UserObject by UserId, returns null from the cache
				cacheInstance.Expect(cache => cache.Get(null)).IgnoreArguments().WhenCalled(methodInvocationCallback).Return(null);

				methodInvocationCallback = methodInvocation =>
				{
					Assert.AreEqual(4, methodInvocation.Arguments.Length);
					Assert.AreEqual(typeof(string), methodInvocation.Arguments[0].GetType());
					Assert.AreEqual(typeof(UserObject), methodInvocation.Arguments[1].GetType());
				};

				// Add pair of (UserId, UserObject) to cache
				cacheInstance.Expect(cache => cache.Add(null, null, TimeSpan.Zero, CachePriorityTypes.Normal)).IgnoreArguments().WhenCalled(methodInvocationCallback);
			}

			using (mockRepository.Playback())
			{
				membershipApi.Get(userId);
			}
		}

		[Test, Description("Test ICache usage while getting an existed user by name through membership APIs.")]
		public void CacheCallingWhenGetUserByNameTest()
		{
			IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

			MockRepository mockRepository = new MockRepository();
			ICache cacheInstance = mockRepository.StrictMock<ICache>();
			IOrganizationApi organizationApi = mockRepository.DynamicMock<IOrganizationApi>();
			SetupResult.For(organizationApi.GetOrganization(Guid.Empty)).IgnoreArguments().Return(new OrganizationObject());
			mockRepository.ReplayAll();

			MembershipApi membershipApi = new MembershipApi(authenticationContext, organizationApi);
			Guid userId = this.CreateUser(membershipApi);
			UserObject userObject = membershipApi.Get(userId);
			string userName = userObject.UserName;

			membershipApi.Cache = cacheInstance;
			using (mockRepository.Record())
			using (mockRepository.Ordered())
			{
				// Nothing on cache.
			}

			using (mockRepository.Playback())
			{
				membershipApi.Get(userName);
			}
		}

		private static Guid CreateUserWithExtensionProperties(IMembershipApi membershipApi)
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

			membershipApi.Save(userObject, "password1", null);
			return userObject.UserId;
		}

		private Guid CreateUser(IMembershipApi membershipApi)
		{
			IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
			UserObject userObject = new UserObject
			{
				OrganizationId = platformConfiguration.Organization.OrganizationId,
				Comment = "IT specialist",
				DisplayName = string.Format("DisplayName {0}", Guid.NewGuid()),
				Email = "eunge.liu@gmail.com",
				IsApproved = true,
				MobilePin = "137641855XX",
				UserName = string.Format("UserName {0}", Guid.NewGuid())
			};

			membershipApi.Save(userObject, "password1", null);
			createdObjectIds.Add(userObject.UserId);

			return userObject.UserId;
		}
	}
}