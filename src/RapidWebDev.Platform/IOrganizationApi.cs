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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Security;
using System.Security.Principal;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The organization API is used for CRUD organizations and organization types.
	/// The concept "organization" can be any organizations as government, company, department, customer, team, group, vendor, etc.<br/><br/>
	/// The Organization Model of RapidWebDev has 3 concepts "Domain" &gt; "Organization Type" &gt; "Organization".
	/// "Domain" is used to categorize organizations in business. Normally the organizations in different domain map to different business workflow. 
	/// "Organization Type" belongs to a domain which is used to categorize organizations for classification purpose.<br/><br/>
	/// Take e-bussiness website as an example, there may have 3 domains, "Vendor", "Division" and "Customer".
	/// "Vendor" means where the website buys the products. The users in "Vendor" organizations can login system to query what and how many products they provided to the website.
	/// And we still need to categorize vendors into 3 "Organization Type" as "IT Products Vendor", "Clothes Vendor" and "Furniture Vendor".
	/// Then we can create a "IT Products Vendor" organization in the system.<br/>
	/// "Division" means the internal department of the website. There may have "Organization Type" like "Sales", "IT Department", "Customer Service", "Management Team", "Website Maintenance Team" etc.<br/>
	/// We may define "Individual Customer", "VIP Customer", "Enterprise Customer" in the domain "Customer".<br/><br/>
	/// Unfortunately, every user should belong to an existed organization. And every organization should belong to an existed organization type which associated with a domain.
	/// So in the business application built on RapidWebDev, there should have at least one organization with one organization type in a domain.
	/// The organization domains cannot be changed by application users but only can be configured in IPlatformConfiguration in Spring.NET IoC by developers.
	/// </summary>
	public interface IOrganizationApi
	{
		#region Organization Type

		/// <summary>
		/// Save organization type object.
		/// </summary>
		/// <param name="organizationTypeObject"></param>
		/// <exception cref="RapidWebDev.Common.Validation.ValidationException">etc organization type name does exist.</exception>
		void Save(OrganizationTypeObject organizationTypeObject);

		/// <summary>
		/// Get organization type by id.
		/// </summary>
		/// <param name="organizationTypeId">organization type id</param>
		/// <returns></returns>
		OrganizationTypeObject GetOrganizationType(Guid organizationTypeId);

		/// <summary>
		/// Get organization type by name.
		/// </summary>
		/// <param name="organizationTypeName">organization type name</param>
		/// <returns></returns>
		OrganizationTypeObject GetOrganizationType(string organizationTypeName);

		/// <summary>
		/// Find all organization types including soft deleted in current application.
		/// </summary>
		/// <returns></returns>
		IEnumerable<OrganizationTypeObject> FindOrganizationTypes();

		/// <summary>
		/// Find all organization types in specified domains in current application including soft deleted.
		/// </summary>
		/// <param name="domains"></param>
		/// <returns></returns>
		IEnumerable<OrganizationTypeObject> FindOrganizationTypes(IEnumerable<string> domains);

		#endregion

		/// <summary>
		/// Save organization business object. 
		/// If organizationObject.Id equals Guid.Empty, it means to save a new organization. 
		/// Otherwise it's updating an existed organization.
		/// </summary>
		/// <param name="organizationObject"></param>
		void Save(OrganizationObject organizationObject);

		/// <summary>
		/// Find organization objects by enumerable organization ids.
		/// </summary>
		/// <param name="organizationIdList">organization ids</param>
		/// <returns></returns>
		IDictionary<Guid, OrganizationObject> BulkGetOrganizations(IEnumerable<Guid> organizationIdList);

		/// <summary>
		/// Get organization instance by id.
		/// </summary>
		/// <param name="organizationId">organization id</param>
		/// <returns></returns>
		OrganizationObject GetOrganization(Guid organizationId);

		/// <summary>
		/// Get organization instance by name.
		/// </summary>
		/// <param name="organizationName">organization name</param>
		/// <returns></returns>
		OrganizationObject GetOrganizationByName(string organizationName);

		/// <summary>
		/// Get organization instance by code.
		/// </summary>
		/// <param name="organizationCode">organization code</param>
		/// <returns></returns>
		OrganizationObject GetOrganizationByCode(string organizationCode);

		/// <summary>
		/// Find organization business objects by custom predicates.
		/// </summary>
		/// <param name="predicate">linq predicate. see organization properties for predicate at <see cref="RapidWebDev.Platform.Linq.Organization"/>.</param>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns>Returns enumerable organizations</returns>
		IEnumerable<OrganizationObject> FindOrganizations(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount);
	}
}