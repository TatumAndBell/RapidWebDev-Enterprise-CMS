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
using RapidWebDev.Common.Validation;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using NUnit.Framework;

namespace RapidWebDev.Tests.Platform
{
	[TestFixture]
	public class RoleApiTests
	{
		private List<Guid> createdRoleIds = new List<Guid>();
		private List<Guid> createdUserIds = new List<Guid>();
		private List<Guid> createdOrganizationTypeIds = new List<Guid>();

		[TearDown]
		public void TearDown()
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				foreach (Guid createdUserId in createdUserIds)
				{
					ctx.UsersInRoles.Delete(uir => uir.UserId == createdUserId);
					ctx.Memberships.Delete(m => m.UserId == createdUserId);
					ctx.Users.Delete(u => u.UserId == createdUserId);
				}

				foreach (Guid createdRoleId in createdRoleIds)
				{
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

		[Test, Description("Basic role CRUD test")]
		public void BasicCRUDTest()
		{
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			OrganizationTypeObject customer = new OrganizationTypeObject { Name = "customer", Domain = "Customer", Description = "customer-desc" };
			organizationApi.Save(customer);
			createdOrganizationTypeIds.Add(customer.OrganizationTypeId);

			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department", Predefined = true };
			RoleObject business = new RoleObject { RoleName = "business", Description = "business-desc", Domain = "Department" };
			RoleObject customers = new RoleObject { RoleName = "customers", Description = "customers-desc", Domain = "Customer" };
			roleApi.Save(powerAdministrators);
			roleApi.Save(business);
			roleApi.Save(customers);

			createdRoleIds.AddRange(new Guid[] { powerAdministrators.RoleId, business.RoleId, customers.RoleId });

			powerAdministrators = roleApi.Get(powerAdministrators.RoleId);
			Assert.AreEqual("powerAdministrators", powerAdministrators.RoleName);
			Assert.AreEqual("powerAdministrators-desc", powerAdministrators.Description);
			Assert.IsTrue(powerAdministrators.Predefined);
			Assert.AreEqual("Department", powerAdministrators.Domain);

			business = roleApi.Get(business.RoleId);
			Assert.AreEqual("business", business.RoleName);
			Assert.AreEqual("business-desc", business.Description);
			Assert.IsFalse(business.Predefined);
			Assert.AreEqual("Department", business.Domain);

			powerAdministrators.RoleName = "admins";
			powerAdministrators.Description = "admins-desc";
			roleApi.Save(powerAdministrators);

			powerAdministrators = roleApi.Get(powerAdministrators.RoleId);
			Assert.AreEqual("admins", powerAdministrators.RoleName);
			Assert.AreEqual("admins-desc", powerAdministrators.Description);
			Assert.IsTrue(powerAdministrators.Predefined);

			Assert.AreEqual(3, roleApi.FindByDomain("Department").Count());
			Assert.AreEqual(1, roleApi.FindByDomain("Customer").Count());

			roleApi.HardDelete(business.RoleId);
			business = roleApi.Get(business.RoleId);
			Assert.IsNull(business);
			Assert.AreEqual(2, roleApi.FindByDomain("Department").Count());
		}

		[Test, Description("Update role to a existed role name test"), ExpectedException(typeof(ValidationException))]
		public void UpdateRoleWithDuplicateNameTest()
		{
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
			RoleObject administrators = new RoleObject { RoleName = "administrators", Description = "administrators-desc", Domain = "Department" };
			RoleObject business = new RoleObject { RoleName = "business", Description = "business-desc", Domain = "Department" };
			roleApi.Save(administrators);
			roleApi.Save(business);
			createdRoleIds.AddRange(new Guid[] { administrators.RoleId, business.RoleId });

			business.RoleName = "administrators";
			roleApi.Save(business);
		}

		[Test, Description("Roles and users relationship test")]
		public void UsersAndRolesTest()
		{
			IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
			IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();

			UserObject eunge = new UserObject
			{
				OrganizationId = platformConfiguration.Organization.OrganizationId,
				UserName = "eunge",
				DisplayName = "Eunge",
				Email = "eunge.liu@gmail.com",
				Comment = "The author of RapidWebDev.",
				IsApproved = true
			};

			membershipApi.Save(eunge, "password1", null);
			createdUserIds.Add(eunge.UserId);

			OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
			organizationApi.Save(department);
			createdOrganizationTypeIds.Add(department.OrganizationTypeId);

			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department" };
			RoleObject business = new RoleObject { RoleName = "business", Description = "business-desc", Domain = "Department" };
			roleApi.Save(powerAdministrators);
			roleApi.Save(business);
			createdRoleIds.AddRange(new Guid[] { powerAdministrators.RoleId, business.RoleId });

			roleApi.SetUserToRoles(eunge.UserId, new Guid[] { powerAdministrators.RoleId });
			Assert.AreEqual(1, roleApi.FindByUserId(eunge.UserId).Count());
			Assert.IsTrue(roleApi.IsUserInRole(eunge.UserId, powerAdministrators.RoleId));
			Assert.IsTrue(roleApi.IsUserInRole(eunge.UserId, "powerAdministrators"));

			roleApi.SetUserToRoles(eunge.UserId, new Guid[] { powerAdministrators.RoleId, business.RoleId });
			Assert.AreEqual(2, roleApi.FindByUserId(eunge.UserId).Count());
			Assert.IsTrue(roleApi.IsUserInRole(eunge.UserId, powerAdministrators.RoleId));
			Assert.IsTrue(roleApi.IsUserInRole(eunge.UserId, "powerAdministrators"));
			Assert.IsTrue(roleApi.IsUserInRole(eunge.UserId, business.RoleId));
			Assert.IsTrue(roleApi.IsUserInRole(eunge.UserId, "business"));
		}

		[Test, Description("Find roles by dynamic predicate.")]
		public void FindRolesTest()
		{
			IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
			RoleObject powerAdministrators = new RoleObject { RoleName = "powerAdministrators", Description = "powerAdministrators-desc", Domain = "Department", Predefined = true };
			RoleObject business = new RoleObject { RoleName = "business", Description = "business-desc", Domain = "Department", Predefined = true };
			RoleObject customers = new RoleObject { RoleName = "customers", Description = "customers-desc", Domain = "Customer"};

			roleApi.Save(powerAdministrators);
			roleApi.Save(business);
			roleApi.Save(customers);
			createdRoleIds.AddRange(new Guid[] { powerAdministrators.RoleId, business.RoleId, customers.RoleId });

			LinqPredicate linqPredicate = new LinqPredicate("Predefined=@0 And RoleName=@1", true, "business");
			int recordCount;
			IEnumerable<RoleObject> roles = roleApi.FindRoles(linqPredicate, null, 0, 10, out recordCount);
			Assert.AreEqual(1, recordCount);
			Assert.AreEqual("business", roles.First().RoleName);
			Assert.AreEqual("business-desc", roles.First().Description);
		}
	}
}