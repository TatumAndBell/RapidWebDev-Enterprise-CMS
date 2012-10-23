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
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using NUnit.Framework;
using Rhino.Mocks;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.Tests.Platform
{
	[TestFixture]
	public class SiteMapApiTests
	{
		private MockRepository mockRepository;
		private static IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
		private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		private static IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();

		private List<Guid> createdRoleIds = new List<Guid>();
		private List<Guid> createdUserIds = new List<Guid>();
		private List<Guid> createdOrganizationTypeIds = new List<Guid>();

		[SetUp]
		public void StartUp()
		{
			this.mockRepository = new MockRepository();
		}

		[TearDown]
		public void TearDown()
		{
			this.mockRepository.VerifyAll();

			string sessionKey = "FindSiteMapConfig_" + authenticationContext.User.UserId.ToString("N");
			authenticationContext.Session[sessionKey] = null;
		}

		[Test, Description("Test cases that get site map for administrators.")]
		public void SiteMapForAdministrators()
		{
			string siteMapFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../TestData/siteMap.config");
			IRoleApi roleApi = this.mockRepository.StrictMock<IRoleApi>();
			Expect.Call(roleApi.IsUserInRole(Guid.Empty, Guid.Empty)).IgnoreArguments().Return(true);
			SiteMapApi siteMapApi = new SiteMapApi(authenticationContext, roleApi, organizationApi, null, platformConfiguration, siteMapFilePath);

			this.mockRepository.ReplayAll();

			IEnumerable<SiteMapItemConfig> siteMapItems = siteMapApi.FindSiteMapConfig(authenticationContext.User.UserId);
			Assert.AreEqual(2, siteMapItems.Count());

			SiteMapItemConfig accountSiteMapItem = siteMapItems.First();
			string accountText = GlobalizationUtility.ReplaceGlobalizationVariables("$Resources.SiteMap.Account, RapidWebDev.Web$");
			Assert.AreEqual(accountText, accountSiteMapItem.Text);
		}

		[Test, Description("Test cases that get site map for non-administrators users having all permissions.")]
		public void SiteMapForNonAdministratorsWithAllPermissions()
		{
			string siteMapFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../TestData/siteMap.config");
			IRoleApi roleApi = this.mockRepository.StrictMock<IRoleApi>();
			Expect.Call(roleApi.IsUserInRole(Guid.Empty, Guid.Empty)).IgnoreArguments().Return(false);

			IPermissionApi permissionApi = this.mockRepository.StrictMock<IPermissionApi>();

			// there are 12 sitemap elements which have the attribute "Value".
			Expect.Call(permissionApi.HasPermission(Guid.Empty, null))
				.IgnoreArguments()
				.Return(true)
				.Repeat.Times(12); 

			SiteMapApi siteMapApi = new SiteMapApi(authenticationContext, roleApi, organizationApi, permissionApi, platformConfiguration, siteMapFilePath);
			this.mockRepository.ReplayAll();

			IEnumerable<SiteMapItemConfig> siteMapItems = siteMapApi.FindSiteMapConfig(authenticationContext.User.UserId);
			Assert.AreEqual(2, siteMapItems.Count());
		}

		[Test, Description("Test cases that get site map for non-administrators users having partial permissions.")]
		public void SiteMapForNonAdministratorsWithPartialPermissions()
		{
			string siteMapFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../TestData/siteMap.config");
			IRoleApi roleApi = this.mockRepository.StrictMock<IRoleApi>();
			Expect.Call(roleApi.IsUserInRole(Guid.Empty, Guid.Empty)).IgnoreArguments().Return(false);

			IPermissionApi permissionApi = this.mockRepository.StrictMock<IPermissionApi>();

			Guid userId = authenticationContext.User.UserId;
			// there are 4 sitemap elements which have the attribute "Value" equals to "EveryOne".
			Expect.Call(permissionApi.HasPermission(userId, "EveryOne"))
				.Return(true)
				.Repeat.Times(4);

			// there are 8 sitemap elements which have the attribute "Value" not equals to "EveryOne".
			Expect.Call(permissionApi.HasPermission(userId, "OrganizationTypeManagement")).Return(false);
			Expect.Call(permissionApi.HasPermission(userId, "DepartmentManagement")).Return(false);
			Expect.Call(permissionApi.HasPermission(userId, "CustomerManagement")).Return(false);
			Expect.Call(permissionApi.HasPermission(userId, "Department.RoleManagement")).Return(false);
			Expect.Call(permissionApi.HasPermission(userId, "Customer.RoleManagement")).Return(false);
			Expect.Call(permissionApi.HasPermission(userId, "Department.UserManagement")).Return(false);
			Expect.Call(permissionApi.HasPermission(userId, "Customer.UserManagement")).Return(false);
			Expect.Call(permissionApi.HasPermission(userId, "AreaManagement")).Return(false);

			SiteMapApi siteMapApi = new SiteMapApi(authenticationContext, roleApi, organizationApi, permissionApi, platformConfiguration, siteMapFilePath);
			this.mockRepository.ReplayAll();

			IEnumerable<SiteMapItemConfig> siteMapItems = siteMapApi.FindSiteMapConfig(userId);
			Assert.AreEqual(1, siteMapItems.Count());
		}
	}
}