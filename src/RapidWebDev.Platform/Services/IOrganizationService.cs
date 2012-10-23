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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using RapidWebDev.Common;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
	/// <summary>
	/// The service to operate organizations for the authenticated user.
	/// </summary>
	[ServiceContract(Name = "OrganizationService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
	public interface IOrganizationService
	{
		/// <summary>
        /// Lists all available organization domains.
        /// UriTemplate = "json/ListDomains"
		/// </summary>		
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/ListDomains")]
		[Permission]
		Collection<OrganizationDomainObject> ListDomainsJson();

		/// <summary>
        /// Lists all available organization domains.	
        /// UriTemplate = "xml/ListDomains"
		/// </summary>	
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/ListDomains")]
		[Permission]
		Collection<OrganizationDomainObject> ListDomainsXml();

		#region Organization Type Services

		/// <summary>
        /// Add/Update organization type object.
        /// UriTemplate = "json/SaveOrganizationType"
		/// </summary>		
		/// <param name="organizationTypeObject"></param>
		/// <exception cref="RapidWebDev.Common.Validation.ValidationException">etc organization type name does exist.</exception>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/SaveOrganizationType")]
		[Permission]
		string SaveOrganizationTypeJson(OrganizationTypeObject organizationTypeObject);


		/// <summary>
		/// Add/Update organization type object.
        /// UriTemplate = "xml/SaveOrganizationType"
		/// </summary>		
		/// <param name="organizationTypeObject"></param>
		/// <exception cref="RapidWebDev.Common.Validation.ValidationException">etc organization type name does exist.</exception>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "xml/SaveOrganizationType")]
		[Permission]
		string SaveOrganizationTypeXml(OrganizationTypeObject organizationTypeObject);

		/// <summary>
		/// Get the organization type by id.
		/// UriTemplate = "json/GetOrganizationTypeById/{organizationTypeId}"
		/// </summary>
		/// <param name="organizationTypeId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetOrganizationTypeById/{organizationTypeId}")]
		[Permission]
		OrganizationTypeObject GetOrganizationTypeByIdJson(string organizationTypeId);

		/// <summary>
		/// Get the organization type by id.
		/// UriTemplate = "xml/GetOrganizationTypeById/{organizationTypeId}"
		/// </summary>
		/// <param name="organizationTypeId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetOrganizationTypeById/{organizationTypeId}")]
		[Permission]
		OrganizationTypeObject GetOrganizationTypeByIdXml(string organizationTypeId);

		/// <summary>
		/// Get the organization type by name.
		/// UriTemplate = "json/GetOrganizationTypeByName/{organizationTypeName}"
		/// </summary>
		/// <param name="organizationTypeName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetOrganizationTypeByName/{organizationTypeName}")]
		[Permission]
		OrganizationTypeObject GetOrganizationTypeByNameJson(string organizationTypeName);

		/// <summary>
		/// Get the organization type by name.
		/// UriTemplate = "xml/GetOrganizationTypeByName/{organizationTypeName}"
		/// </summary>
		/// <param name="organizationTypeName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetOrganizationTypeByName/{organizationTypeName}")]
		[Permission]
		OrganizationTypeObject GetOrganizationTypeByNameXml(string organizationTypeName);


		/// <summary>
        /// Find organization types in specified domain.
        /// UriTemplate = "json/FindOrganizationTypes/{domain}"
		/// </summary>		
		/// <param name="domain"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindOrganizationTypes/{domain}")]
		[Permission]
		Collection<OrganizationTypeObject> FindOrganizationTypesJson(string domain);

		/// <summary>
        /// Find organization types in specified domain.
        /// UriTemplate = "xml/FindOrganizationTypes/{domain}"
		/// </summary>		
		/// <param name="domain"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindOrganizationTypes/{domain}")]
		[Permission]
		Collection<OrganizationTypeObject> FindOrganizationTypesXml(string domain);

		#endregion

		#region Organization Services

		/// <summary>
        /// Add/Update organization  object.
        /// UriTemplate = "json/SaveOrganization"
		/// </summary>		
		/// <param name="organizationObject"></param>
		/// <exception cref="RapidWebDev.Common.Validation.ValidationException">etc organization type name does exist.</exception>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/SaveOrganization")]
		[Permission]
		string SaveOrganizationJson(OrganizationObject organizationObject);

		/// <summary>
        /// Add/Update organization  object.
        /// UriTemplate = "json/SaveOrganization"
		/// </summary>		
		/// <param name="organizationObject"></param>
		/// <exception cref="RapidWebDev.Common.Validation.ValidationException">etc organization type name does exist.</exception>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/SaveOrganization")]
		[Permission]
		string SaveOrganizationXml(OrganizationObject organizationObject);

		/// <summary>
        /// Get the organization by id.
        ///  UriTemplate = "json/GetOrganizationById/{organizationId}"
		/// </summary>		
		/// <param name="organizationId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetOrganizationById/{organizationId}")]
		[Permission]
		OrganizationObject GetOrganizationByIdJson(string organizationId);

		/// <summary>
        /// Get the organization by id.
        ///  UriTemplate = "xml/GetOrganizationById/{organizationId}"
		/// </summary>		
		/// <param name="organizationId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetOrganizationById/{organizationId}")]
		[Permission]
		OrganizationObject GetOrganizationByIdXml(string organizationId);

		/// <summary>
        /// Get the organization by code.
        ///  UriTemplate = "json/GetOrganizationByCode/{organizationCode}"
		/// </summary>		
		/// <param name="organizationCode"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetOrganizationByCode/{organizationCode}")]
		[Permission]
		OrganizationObject GetOrganizationByCodeJson(string organizationCode);

		/// <summary>
        /// Get the organization by code.
        ///  UriTemplate = "xml/GetOrganizationByCode/{organizationCode}"
		/// </summary>		
		/// <param name="organizationCode"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetOrganizationByCode/{organizationCode}")]
		[Permission]
		OrganizationObject GetOrganizationByCodeXml(string organizationCode);

		/// <summary>
        /// Get the organization by name.
        ///  UriTemplate = "json/GetOrganizationByName/{organizationName}"
		/// </summary>		
		/// <param name="organizationName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetOrganizationByName/{organizationName}")]
		[Permission]
		OrganizationObject GetOrganizationByNameJson(string organizationName);

		/// <summary>
        /// Get the organization by name.
        ///  UriTemplate = "xml/GetOrganizationByName/{organizationName}"
		/// </summary>		
		/// <param name="organizationName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetOrganizationByName/{organizationName}")]
		[Permission]
		OrganizationObject GetOrganizationByNameXml(string organizationName);

		/// <summary>
		/// Find organizations by organization ids.
		/// UriTemplate = "json/BulkGetOrganizationsByIds"
		/// </summary>
		/// <param name="organizationIdList">organization ids</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/BulkGetOrganizationsByIds")]
		[Permission]
		Collection<OrganizationObject> BulkGetOrganizationsByIdsJson(IdCollection organizationIdList);

		/// <summary>
		/// Find organizations by organization ids.
		/// UriTemplate = "xml/BulkGetOrganizationsByIds"
		/// </summary>
		/// <param name="organizationIdList">organization ids</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/BulkGetOrganizationsByIds")]
		[Permission]
		Collection<OrganizationObject> BulkGetOrganizationsByIdsXml(IdCollection organizationIdList);

		/// <summary>
        /// Search organizations by a collection of criterias for the authenticated user of request.
        /// UriTemplate = "json/search?domain={domain}&amp;q={q}&amp;start={start}&amp;limit={limit}&amp;sortfield={sortField}&amp;sortDirection={sortDirection}&amp;orgTypeId={orgTypeId}"
		/// </summary>        
		/// <param name="domain">Which domain of the searching organizations.</param>
		/// <param name="orgTypeId">Which organization type the searching organizations should belong to.</param>
		/// <param name="q">Keywords for searching.</param>
		/// <param name="sortField">Sorting field name, the default sorting field is LastUpdatedDate.</param>
		/// <param name="sortDirection">Sorting order, DESC or ASC, the default sorting order is DESC.</param>
		/// <param name="start">The start organization index of hit to return.</param>
		/// <param name="limit">The limit of returned organizations.</param>
		/// <returns>The query results object includes total hit count, returned records, start and limit.</returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/search?domain={domain}&q={q}&start={start}&limit={limit}&sortfield={sortField}&sortDirection={sortDirection}&orgTypeId={orgTypeId}")]
		[Permission]
        OrganizationQueryResult SearchJson(string domain, string orgTypeId, string q, string sortField, string sortDirection, int start, int limit);

		/// <summary>
        /// Search organizations by a collection of criterias for the authenticated user of request.
        /// UriTemplate = "xml/search?domain={domain}&amp;q={q}&amp;start={start}&amp;limit={limit}&amp;sortfield={sortField}&amp;sortDirection={sortDirection}&amp;orgTypeId={orgTypeId}"
		/// </summary>        
		/// <param name="domain">Which domain of the searching organizations.</param>
		/// <param name="orgTypeId">Which organization type the searching organizations should belong to.</param>
		/// <param name="q">Keywords for searching.</param>
		/// <param name="sortField">Sorting field name, the default sorting field is LastUpdatedDate.</param>
		/// <param name="sortDirection">Sorting order, DESC or ASC, the default sorting order is DESC.</param>
		/// <param name="start">The start organization index of hit to return.</param>
		/// <param name="limit">The limit of returned organizations.</param>
		/// <returns>The query results object includes total hit count, returned records, start and limit.</returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/search?domain={domain}&q={q}&start={start}&limit={limit}&sortfield={sortField}&sortDirection={sortDirection}&orgTypeId={orgTypeId}")]
		[Permission]
        OrganizationQueryResult SearchXml(string domain, string orgTypeId, string q, string sortField, string sortDirection, int start, int limit);

		/// <summary>
		/// Query organizations by custom predicates.
		/// UriTemplate = "json/QueryOrganizations?pageindex={pageIndex}&amp;pagesize={pageSize}&amp;orderby={orderby}"
		/// </summary>
		/// <param name="orderby">sorting field and direction</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>       
		/// <param name="predicate">linq predicate. see organization properties for predicate at <see cref="RapidWebDev.Platform.Linq.Organization"/>.</param>
		/// <returns>Returns organizations</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/QueryOrganizations?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
		[Permission]
		OrganizationQueryResult QueryOrganizationsJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);

		/// <summary>
		/// Query organizations by custom predicates.
		/// UriTemplate = "xml/QueryOrganizations?pageindex={pageIndex}&amp;pagesize={pageSize}&amp;orderby={orderby}"
		/// </summary>
		/// <param name="orderby">sorting field and direction</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>       
		/// <param name="predicate">linq predicate. see organization properties for predicate at <see cref="RapidWebDev.Platform.Linq.Organization"/>.</param>
		/// <returns>Returns organizations</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/QueryOrganizations?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
		[Permission]
		OrganizationQueryResult QueryOrganizationsXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);

		#endregion
	}

	/// <summary>
	/// Use Data query result
	/// </summary>
	[CollectionDataContract(ItemName = "OrganizationObject", Namespace = ServiceNamespaces.Platform)]
	public class OrganizationQueryResult : Collection<OrganizationObject>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="objects"></param>
		public OrganizationQueryResult(IList<OrganizationObject> objects)
			: base(objects)
		{
		}

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public OrganizationQueryResult()
		{
		}

		/// <summary>
		/// Page index which starts from 0.
		/// </summary>
		[DataMember]
		public int PageIndex { get; set; }

		/// <summary>
		/// Page size.
		/// </summary>
		[DataMember]
		public int PageSize { get; set; }

		/// <summary>
		/// Total record count.
		/// </summary>
		[DataMember]
		public int TotalRecordCount { get; set; }
	}
}
