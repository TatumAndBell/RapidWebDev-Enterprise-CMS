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
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using NUnit.Framework;

namespace RapidWebDev.Tests.Platform
{
    [TestFixture]
    public class OrganizationApiTests
    {
        List<Guid> createdOrganizationIds = new List<Guid>();
        List<Guid> createdOrganizationTypeIds = new List<Guid>();

        [TearDown]
        public void TearDown()
        {
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                var organizationsToDelete = ctx.Organizations.Where(org => createdOrganizationIds.ToArray().Contains(org.OrganizationId));
                var organizationTypesToDelete = ctx.OrganizationTypes.Where(orgType => createdOrganizationTypeIds.ToArray().Contains(orgType.OrganizationTypeId));

                ctx.Organizations.DeleteAllOnSubmit(organizationsToDelete);
                ctx.OrganizationTypes.DeleteAllOnSubmit(organizationTypesToDelete);
                ctx.SubmitChanges();
            }
        }

        [Test, Description("Basic organization type CRUD test.")]
        public void BasicOrganizationTypeTest()
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
            organizationApi.Save(department);
            createdOrganizationTypeIds.Add(department.OrganizationTypeId);

            OrganizationTypeObject customer = new OrganizationTypeObject { Name = "customer", Domain = "Customer", Description = "customer-desc" };
            organizationApi.Save(customer);
            createdOrganizationTypeIds.Add(customer.OrganizationTypeId);

            department = organizationApi.GetOrganizationType(department.OrganizationTypeId);
            Assert.AreEqual("department", department.Name);
            Assert.AreEqual("department-desc", department.Description);

            customer = organizationApi.GetOrganizationType("customer");
            Assert.AreEqual("customer", customer.Name);
            Assert.AreEqual("customer-desc", customer.Description);

            var organizationTypes = organizationApi.FindOrganizationTypes().ToList();
            Assert.AreEqual(1, organizationTypes.Where(ct => ct.Name == "customer").Count());
            Assert.AreEqual(1, organizationTypes.Where(ct => ct.Name == "department").Count());

            department.Name = "departmentX";
            department.Description = "departmentX-desc";
            organizationApi.Save(department);

            department = organizationApi.GetOrganizationType(department.OrganizationTypeId);
            Assert.AreEqual("departmentX", department.Name);
            Assert.AreEqual("departmentX-desc", department.Description);

            Assert.IsNotNull(organizationApi.GetOrganizationType("customer"));

            organizationTypes = organizationApi.FindOrganizationTypes().ToList();
            Assert.AreEqual(1, organizationTypes.Where(ct => ct.Name == "customer").Count());
            Assert.AreEqual(1, organizationTypes.Where(ct => ct.Name == "departmentX").Count());

            customer.DeleteStatus = DeleteStatus.Deleted;
            organizationApi.Save(customer);

            organizationTypes = organizationApi.FindOrganizationTypes()
				.Where(orgType => orgType.DeleteStatus == DeleteStatus.NotDeleted).ToList();
            Assert.AreEqual(1, organizationTypes.Where(ct => ct.Name == "departmentX").Count());

            department = organizationApi.GetOrganizationType(department.OrganizationTypeId);
            Assert.AreEqual("departmentX", department.Name);
            Assert.AreEqual("departmentX-desc", department.Description);

            department.DeleteStatus = DeleteStatus.Deleted;
            organizationApi.Save(department);
        }

        [Test, Description("Basic organization CRUD test.")]
        public void BasicOrganizationTest()
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
            organizationApi.Save(department);
            createdOrganizationTypeIds.Add(department.OrganizationTypeId);

            OrganizationObject shOrganization = new OrganizationObject
            {
                OrganizationCode = "sh021",
                OrganizationName = "sh-department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                Description = "sh-desc"
            };

            organizationApi.Save(shOrganization);
            createdOrganizationIds.Add(shOrganization.OrganizationId);

            OrganizationObject cdOrganization = new OrganizationObject
            {
                OrganizationCode = "cd028",
                OrganizationName = "cd-department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                Description = "cd-desc"
            };

            organizationApi.Save(cdOrganization);
            createdOrganizationIds.Add(cdOrganization.OrganizationId);

            shOrganization = organizationApi.GetOrganization(shOrganization.OrganizationId);
            Assert.AreEqual("sh021", shOrganization.OrganizationCode);
            Assert.AreEqual("sh-department", shOrganization.OrganizationName);
            Assert.IsFalse(shOrganization.ParentOrganizationId.HasValue);
            Assert.AreEqual(OrganizationStatus.Enabled, shOrganization.Status);
            Assert.AreEqual("sh-desc", shOrganization.Description);

            cdOrganization = organizationApi.GetOrganization(cdOrganization.OrganizationId);
            Assert.IsNotNull(cdOrganization);

            shOrganization.OrganizationName = "021-depart";
            shOrganization.Description = "021-desc";
            organizationApi.Save(shOrganization);

            shOrganization = organizationApi.GetOrganization(shOrganization.OrganizationId);
            Assert.AreEqual("021-depart", shOrganization.OrganizationName);
            Assert.IsFalse(shOrganization.ParentOrganizationId.HasValue);
            Assert.AreEqual(OrganizationStatus.Enabled, shOrganization.Status);
            Assert.AreEqual("021-desc", shOrganization.Description);

            shOrganization = organizationApi.GetOrganizationByName("021-depart");
            Assert.AreEqual("021-depart", shOrganization.OrganizationName);

            shOrganization = organizationApi.GetOrganizationByCode("sh021");
            Assert.AreEqual("021-depart", shOrganization.OrganizationName);
        }

        [Test, Description("Save organization test.")]
        public void SaveOrganizationTest()
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
            organizationApi.Save(department);
            createdOrganizationTypeIds.Add(department.OrganizationTypeId);

            OrganizationObject shanghai = new OrganizationObject
            {
                OrganizationCode = "sh",
                OrganizationName = "sh-department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                Description = "sh-desc"
            };

            organizationApi.Save(shanghai);
            createdOrganizationIds.Add(shanghai.OrganizationId);

            shanghai = organizationApi.GetOrganization(shanghai.OrganizationId);
            shanghai.OrganizationName = "shanghai organization";
            shanghai.Description = "shanghai organization description";
            organizationApi.Save(shanghai);

            shanghai = organizationApi.GetOrganization(shanghai.OrganizationId);
            Assert.AreEqual("shanghai organization", shanghai.OrganizationName);
            Assert.AreEqual("shanghai organization description", shanghai.Description);
        }

        [Test, Description("Setting wrong child organization status on creating test.")]
        [ExpectedException(typeof(ValidationException))]
        public void WrongChildOrganizationStatusOnCreatingTest()
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
            OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
            organizationApi.Save(department);
            createdOrganizationTypeIds.Add(department.OrganizationTypeId);

            OrganizationObject shOrganization = new OrganizationObject
            {
                OrganizationCode = "sh",
                OrganizationName = "sh-department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Disabled,
                Description = "sh-desc"
            };

            organizationApi.Save(shOrganization);
            createdOrganizationIds.Add(shOrganization.OrganizationId);

            OrganizationObject cdOrganization = new OrganizationObject
            {
                OrganizationCode = "cd",
                OrganizationName = "cd-department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                ParentOrganizationId = shOrganization.OrganizationId,
                Description = "cd-desc"
            };

            organizationApi.Save(cdOrganization);
        }

        [Test, Description("Organization status update test.")]
        public void UpdateOrganizationStatusTest()
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
            organizationApi.Save(department);
            createdOrganizationTypeIds.Add(department.OrganizationTypeId);

            OrganizationObject shOrganization = new OrganizationObject
            {
                OrganizationCode = "sh",
                OrganizationName = "sh-department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                Description = "sh-desc"
            };

            organizationApi.Save(shOrganization);
            createdOrganizationIds.Add(shOrganization.OrganizationId);

            OrganizationObject cdOrganization = new OrganizationObject
            {
                OrganizationCode = "cd",
                OrganizationName = "cd-department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                ParentOrganizationId = shOrganization.OrganizationId,
                Description = "cd-desc"
            };

            organizationApi.Save(cdOrganization);
            createdOrganizationIds.Add(cdOrganization.OrganizationId);

            // update parent's status from Enabled to Disabled will affect its children
            shOrganization.Status = OrganizationStatus.Disabled;
            organizationApi.Save(shOrganization);
            shOrganization = organizationApi.GetOrganization(shOrganization.OrganizationId);
            cdOrganization = organizationApi.GetOrganization(cdOrganization.OrganizationId);
            Assert.AreEqual(OrganizationStatus.Disabled, shOrganization.Status);
            Assert.AreEqual(OrganizationStatus.Disabled, cdOrganization.Status);

            // update parent's status from Disabled to Enabled won't affect its children
            shOrganization.Status = OrganizationStatus.Enabled;
            organizationApi.Save(shOrganization);
            shOrganization = organizationApi.GetOrganization(shOrganization.OrganizationId);
            cdOrganization = organizationApi.GetOrganization(cdOrganization.OrganizationId);
            Assert.AreEqual(OrganizationStatus.Enabled, shOrganization.Status);
            Assert.AreEqual(OrganizationStatus.Disabled, cdOrganization.Status);

            cdOrganization.Status = OrganizationStatus.Enabled;
            organizationApi.Save(cdOrganization);

            // update child's status from Enabled to Disabled won't affect its parent
            cdOrganization.Status = OrganizationStatus.Disabled;
            organizationApi.Save(cdOrganization);
            shOrganization = organizationApi.GetOrganization(shOrganization.OrganizationId);
            cdOrganization = organizationApi.GetOrganization(cdOrganization.OrganizationId);
            Assert.AreEqual(OrganizationStatus.Enabled, shOrganization.Status);
            Assert.AreEqual(OrganizationStatus.Disabled, cdOrganization.Status);
        }

        [Test, Description("Add multiple organizations first, then validate the query against them.")]
        public void FindOrganizationTest()
        {
            IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

            OrganizationTypeObject department = new OrganizationTypeObject { Name = "department", Domain = "Department", Description = "department-desc" };
            organizationApi.Save(department);
            createdOrganizationTypeIds.Add(department.OrganizationTypeId);

            OrganizationObject shanghai = new OrganizationObject
            {
                OrganizationCode = "sh",
                OrganizationName = "shanghai department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                Description = "shanghai desc"
            };

            organizationApi.Save(shanghai);
            createdOrganizationIds.Add(shanghai.OrganizationId);

            OrganizationObject chengdu = new OrganizationObject
            {
                OrganizationCode = "cd",
                OrganizationName = "chengdu department",
                OrganizationTypeId = department.OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                Description = "chengdu desc"
            };

            organizationApi.Save(chengdu);
            createdOrganizationIds.Add(chengdu.OrganizationId);

            int recordCount;
            LinqPredicate linqPredicate = new LinqPredicate("OrganizationName.EndsWith(@0) And Status=@1", "department", OrganizationStatus.Enabled);
            IEnumerable<OrganizationObject> organizations = organizationApi.FindOrganizations(linqPredicate, null, 0, 10, out recordCount);
            Assert.AreEqual(2, recordCount);
        }
    }
}