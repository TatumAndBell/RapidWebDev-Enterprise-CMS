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
using System.Globalization;

namespace RapidWebDev.Tests.Platform
{
	[TestFixture]
	public class PermissionApiTests
	{
		private static IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();
		private static IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
		private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

		private List<Guid> createdRoleIds = new List<Guid>();
		private List<Guid> createdUserIds = new List<Guid>();
		private List<Guid> createdOrganizationTypeIds = new List<Guid>();

		[SetUp]
		public void StartUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				foreach (Guid createdUserId in createdUserIds)
				{
					ctx.Permissions.Delete(p => p.UserId == createdUserId);
					ctx.UsersInRoles.Delete(uir => uir.UserId == createdUserId);
					ctx.Memberships.Delete(m => m.UserId == createdUserId);
					ctx.Users.Delete(u => u.UserId == createdUserId);
				}

				foreach (Guid createdRoleId in createdRoleIds)
				{
					ctx.Permissions.Delete(p => p.RoleId == createdRoleId);
					ctx.Roles.Delete(r => r.RoleId == createdRoleId);
				}

				foreach (Guid createdOrganizationTypeId in createdOrganizationTypeIds)
				{
					ctx.OrganizationTypes.Delete(orgType => orgType.OrganizationTypeId == createdOrganizationTypeId);
				}

				ctx.SubmitChanges();
			}

			createdRoleIds.Clear();
			createdUserIds.Clear();
			createdOrganizationTypeIds.Clear();
		}

		[Test, Description("Test the scenario that permissions are set on role only.")]
		public void PermissionOnRoleOnlyTest()
		{
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();

			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department", Predefined = true };
			roleApi.Save(powerAdministrators);
			createdRoleIds.Add(powerAdministrators.RoleId);

			permissionApi.SetRolePermissions(powerAdministrators.RoleId, new string[] { "p1", "p2", "p3" });
			var collection = permissionApi.FindRolePermissions(powerAdministrators.RoleId);
			Assert.AreEqual(3, collection.Count());

			permissionApi.SetRolePermissions(powerAdministrators.RoleId, new string[] { "p1", "p2", "p3", "p4", "p5" });
			collection = permissionApi.FindRolePermissions(powerAdministrators.RoleId);
			Assert.AreEqual(5, collection.Count());
		}

		[Test, Description("Test the scenario that permissions are set on user only.")]
		public void PermissionOnUserOnlyTest()
		{
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
			Guid userId = this.CreateUser(membershipApi);

			SetUserPermissions(userId, new string[] { "p1", "p2", "p3" });

			var collection = permissionApi.FindUserPermissions(userId, false);
			Assert.IsTrue(permissionApi.HasPermission(userId, "p1"));
			Assert.IsTrue(permissionApi.HasPermission(userId, "p2"));
			Assert.IsTrue(permissionApi.HasPermission(userId, "p3"));

			SetUserPermissions(userId, new string[] { "p2.All", "p3.All", "p4", "p5", });

			collection = permissionApi.FindUserPermissions(userId, false);
			Assert.IsFalse(permissionApi.HasPermission(userId, "p1"));
			Assert.IsTrue(permissionApi.HasPermission(userId, "p2"));
			Assert.IsTrue(permissionApi.HasPermission(userId, "p3"));
			Assert.IsTrue(permissionApi.HasPermission(userId, "p4"));
			Assert.IsTrue(permissionApi.HasPermission(userId, "p5"));
		}

		[Test, Description("Test the scenario that permissions are set on both user and role.")]
		public void PermissionOnBothUserAndRoleTest()
		{
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

			// create organization type
			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			// create role
			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department", Predefined = true };
			roleApi.Save(powerAdministrators);
			createdRoleIds.Add(powerAdministrators.RoleId);

			// set permissions on the role
			permissionApi.SetRolePermissions(powerAdministrators.RoleId, new string[] { "p1", "p2", "p3" });

			// create user
			Guid eungeId = this.CreateUser(membershipApi);

			// set permission on the user
			SetUserPermissions(eungeId, new string[] { "p3", "p4", "p5" });

			// set the users as adminstrators
			roleApi.SetUserToRoles(eungeId, new Guid[] { powerAdministrators.RoleId });
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p1"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p2"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p3"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p4"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p5"));

			// set the users without any roles
			roleApi.SetUserToRoles(eungeId, new Guid[] { });
			Assert.IsFalse(permissionApi.HasPermission(eungeId, "p1"));
			Assert.IsFalse(permissionApi.HasPermission(eungeId, "p2"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p3"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p4"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p5"));
		}

		[Test, Description("Test load permission configurations for specified user.")]
		public void LoadPermissionConfigForUserTest()
		{
			// create user
			Guid eungeId = this.CreateUser(membershipApi);

			// set permission on the user
			SetUserPermissions(eungeId, new string[] { "DepartmentManagement.All", "CustomerManagement.View" });

			IEnumerable<PermissionConfig> permissionConfigs = permissionApi.FindPermissionConfig(eungeId);

			// maintenace
			PermissionConfig permissionConfig = permissionConfigs.FirstOrDefault();
			Assert.IsNotNull(permissionConfig);

			// membership
			permissionConfig = permissionConfig.Permission.FirstOrDefault();
			Assert.IsNotNull(permissionConfig);

			// organization
			permissionConfig = permissionConfig.Permission.FirstOrDefault();
			Assert.IsNotNull(permissionConfig);

			Assert.AreEqual(1, permissionConfig.Permission.Length);
			Assert.IsNotNull(permissionConfig.Permission.FirstOrDefault(p => p.Value == "DepartmentManagement"));

			// set permission on the user
			SetUserPermissions(eungeId, new string[] { "AreaManagement", "DepartmentManagement.All", "CustomerManagement.All" });

			permissionConfigs = permissionApi.FindPermissionConfig(eungeId);

			// maintenace
			permissionConfig = permissionConfigs.FirstOrDefault();
			Assert.IsNotNull(permissionConfig);

			// membership
			permissionConfig = permissionConfig.Permission.FirstOrDefault();
			Assert.IsNotNull(permissionConfig);

			// area management
			Assert.IsNotNull(permissionConfig.Permission.FirstOrDefault(p => p.Value == "AreaManagement"));

			// organization
			permissionConfig = permissionConfig.Permission.FirstOrDefault();
			Assert.IsNotNull(permissionConfig);
			Assert.AreEqual(2, permissionConfig.Permission.Length);
		}

		[Test, Description("Test the scenario that the denied permissions are set on role only.")]
		public void DeniedPermissionOnRoleOnlyTest()
		{
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();

			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department", Predefined = true };
			roleApi.Save(powerAdministrators);
			createdRoleIds.Add(powerAdministrators.RoleId);

			permissionApi.SetRolePermissions(powerAdministrators.RoleId, new string[] { "p1", "p1.Create", "p1.Update", "p2", "p1.Create.Denied" });
			var collection = permissionApi.FindRolePermissions(powerAdministrators.RoleId);
			Assert.AreEqual(4, collection.Count());
			Assert.IsFalse(collection.Contains(new PermissionObject("p1.Create")), "The role should not have the permission p1.Create");

			permissionApi.SetRolePermissions(powerAdministrators.RoleId, new string[] { "p1", "p1.Create", "p1.Update", "p2", "p1.Denied" });
			collection = permissionApi.FindRolePermissions(powerAdministrators.RoleId);
			Assert.AreEqual(2, collection.Count());

			bool hasP1Denied = collection.Contains(new PermissionObject("p1.Denied"));
			bool hasP2 = collection.Contains(new PermissionObject("p2"));
			Assert.IsTrue(hasP1Denied && hasP2, "The role should only have the permission p1.Denied and p2.");
		}

		[Test, Description("Test the scenario that the denied permissions are set on user only")]
		public void DeniedPermissionOnUserTest()
		{
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
			Guid userId = this.CreateUser(membershipApi);

			SetUserPermissions(userId, new string[] { "p1", "p1.Create", "p1.Update", "p2", "p1.Create.Denied" });

			var collection = permissionApi.FindUserPermissions(userId, false);
			Assert.IsTrue(permissionApi.HasPermission(userId, "p1"));
			Assert.IsTrue(permissionApi.HasPermission(userId, "p1.Update"));
			Assert.IsTrue(permissionApi.HasPermission(userId, "p2"));
			Assert.IsFalse(permissionApi.HasPermission(userId, "p1.Create"), "The user should not have the permission p1.Create.");

			SetUserPermissions(userId, new string[] { "p1", "p1.Create", "p1.Update", "p2", "p1.Denied" });

			collection = permissionApi.FindUserPermissions(userId, false);
			Assert.IsFalse(permissionApi.HasPermission(userId, "p1"), "The user should not have the permission on p1.");
			Assert.IsTrue(permissionApi.HasPermission(userId, "p2"));
		}

		[Test, Description("Test the scenario that how explicit permissions of the user resolve the conflicts with the denied permissions set on roles.")]
		public void UserPositivePermissionOverrideRoleDeniedPermissionTest()
		{
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

			// create organization type
			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			// create role
			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department", Predefined = true };
			roleApi.Save(powerAdministrators);
			createdRoleIds.Add(powerAdministrators.RoleId);

			// set permissions on the role
			permissionApi.SetRolePermissions(powerAdministrators.RoleId, new string[] { "p1", "p1.Create", "p1.Update", "p1.Create.Denied", "p2" });

			// create user
			Guid eungeId = this.CreateUser(membershipApi);

			// set permission on the user
			SetUserPermissions(eungeId, new string[] { "p1.Create" });

			// set the users as adminstrators
			roleApi.SetUserToRoles(eungeId, new Guid[] { powerAdministrators.RoleId });
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p1"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p1.Create"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p1.Update"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p2"));

			// set the users without any roles
			roleApi.SetUserToRoles(eungeId, new Guid[] { });
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p1.Create"));
		}

		[Test, Description("Test the scenario that how denied permissions of the user shields the conflicts with the positive permissions set on roles.")]
		public void UserDeniedPermissionOverrideRolePositivePermissionTest()
		{
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

			// create organization type
			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			// create role
			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department", Predefined = true };
			roleApi.Save(powerAdministrators);
			createdRoleIds.Add(powerAdministrators.RoleId);

			// set permissions on the role
			permissionApi.SetRolePermissions(powerAdministrators.RoleId, new string[] { "p1", "p1.Create", "p1.Update", "p2" });

			// create user
			Guid eungeId = this.CreateUser(membershipApi);

			// set permission on the user
			SetUserPermissions(eungeId, new string[] { "p1.Create.Denied" });

			// set the users as adminstrators
			roleApi.SetUserToRoles(eungeId, new Guid[] { powerAdministrators.RoleId });

			// asserts
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p1"));
			Assert.IsFalse(permissionApi.HasPermission(eungeId, "p1.Create"), "The user should not have the permission p1.Create");
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p1.Update"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "p2"));
		}

		[Test, Description("Configure implicit permission in permission.config and test whether they can be resolved with explicit permissions.")]
		public void ImplicitPermissionTest()
		{
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

			// create organization type
			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			// create role
			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department", Predefined = true };
			roleApi.Save(powerAdministrators);
			createdRoleIds.Add(powerAdministrators.RoleId);

			// set permissions on the role
			permissionApi.SetRolePermissions(powerAdministrators.RoleId, new string[] { "PermissionWithImplicitPermissions" });
			IEnumerable<PermissionObject> rolePermissions = permissionApi.FindRolePermissions(powerAdministrators.RoleId);
			Assert.AreEqual(3, rolePermissions.Count());
			Assert.IsTrue(rolePermissions.Contains(new PermissionObject("PermissionWithImplicitPermissions")));
			Assert.IsTrue(rolePermissions.Contains(new PermissionObject("ImplicitPermission1")));
			Assert.IsTrue(rolePermissions.Contains(new PermissionObject("ImplicitPermission2")));

			// create user
			Guid eungeId = this.CreateUser(membershipApi);

			// set permission on the user
			SetUserPermissions(eungeId, new string[] { "PX" });

			// set the users as adminstrators
			roleApi.SetUserToRoles(eungeId, new Guid[] { powerAdministrators.RoleId });

			// asserts
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "PX"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "PermissionWithImplicitPermissions"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "ImplicitPermission1"));
			Assert.IsTrue(permissionApi.HasPermission(eungeId, "ImplicitPermission2"));
		}

		[Test, Description("Test permission check algorithm.")]
		public void PermissionCheckAlgorithmTest()
		{
			PermissionObject p1 = (PermissionObject)"A";
			PermissionObject p2 = (PermissionObject)"A.View";
			Assert.IsTrue(p1.Contains(p2), "Permission A should contain A.View");

			p1 = (PermissionObject)"A";
			p2 = (PermissionObject)string.Format(CultureInfo.InvariantCulture, "A.{0}.View", Guid.NewGuid());
			Assert.IsTrue(p1.Contains(p2), "Permission A should contain A.[GUID].View");

			p1 = (PermissionObject)"A";
			p2 = (PermissionObject)"A.Update";
			Assert.IsFalse(p1.Contains(p2), "Permission A should not contain A.Update");

			p1 = (PermissionObject)"A";
			p2 = (PermissionObject)"A.New";
			Assert.IsFalse(p1.Contains(p2), "Permission A should not contain A.New");

			p1 = (PermissionObject)"A.View";
			p2 = (PermissionObject)"A";
			Assert.IsFalse(p1.Contains(p2), "Permission A.View should not contain A");

			p1 = (PermissionObject)"A.Update";
			p2 = (PermissionObject)"A";
			Assert.IsFalse(p1.Contains(p2), "Permission A.Update should not contain A");

			p1 = (PermissionObject)"A.Update";
			p2 = (PermissionObject)string.Format(CultureInfo.InvariantCulture, "A.{0}.Update", Guid.NewGuid());
			Assert.IsFalse(p1.Contains(p2), "Permission A.Update should not contain A.[Guid].Update");

			p1 = (PermissionObject)"A.Update";
			p2 = (PermissionObject)string.Format(CultureInfo.InvariantCulture, "A.Update.{0}", Guid.NewGuid());
			Assert.IsFalse(p1.Contains(p2), "Permission A.Update should not contain A.Update.[Guid]");
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
			createdUserIds.Add(userObject.UserId);

			return userObject.UserId;
		}

		private static void SetUserPermissions(Guid userId, IEnumerable<string> permissions)
		{
			permissionApi.SetUserPermissions(userId, permissions);
			authenticationContext.Session["FindPermissionConfig_" + userId.ToString("N")] = null;
			authenticationContext.Session["FindSiteMapConfig_" + userId.ToString("N")] = null;
		}
	}
}