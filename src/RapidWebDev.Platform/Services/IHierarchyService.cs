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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using RapidWebDev.Common;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
	/// <summary>
	/// The service to operate areas
	/// </summary>
	[ServiceContract(Name = "HierarchyService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
	public interface IHierarchyService
	{
		/// <summary>
		/// Save a hierarchy data object. <br/>
		/// Uri Template: json/Save
		/// </summary>
		/// <param name="hierarchyDataObject"></param>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/Save")]
		[Permission]
		string SaveJson(HierarchyDataObject hierarchyDataObject);

		/// <summary>
		/// Save a hierarchy data object. <br/>
		/// Uri Template: xml/Save
		/// </summary>
		/// <param name="hierarchyDataObject"></param>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/Save")]
		[Permission]
        string SaveXml(HierarchyDataObject hierarchyDataObject);

		/// <summary>
		/// Get a hierarchy data object by id. <br/>
        /// Uri Template: json/GetHierarchyDataById/{hierarchyDataId}
		/// </summary>
		/// <param name="hierarchyDataId"></param>
		/// <returns></returns>
		[OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetHierarchyDataById/{hierarchyDataId}")]
		[Permission]
		HierarchyDataObject GetHierarchyDataByIdJson(string hierarchyDataId);

		/// <summary>
		/// Get a hierarchy data object by id.<br/>
        /// Uri Template: xml/GetHierarchyDataById/{hierarchyDataId}
		/// </summary>
		/// <param name="hierarchyDataId"></param>
		/// <returns></returns>
		[OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetHierarchyDataById/{hierarchyDataId}")]
		[Permission]
		HierarchyDataObject GetHierarchyDataByIdXml(string hierarchyDataId);

		/// <summary>
		/// Get a hierarchy data object by name.<br/>
		/// Uri Template: json/GetHierarchyDataByName/{hierarchyType}/{hierarchyDataName}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="hierarchyDataName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetHierarchyDataByName/{hierarchyType}/{hierarchyDataName}")]
		[Permission]
		HierarchyDataObject GetHierarchyDataByNameJson(string hierarchyType, string hierarchyDataName);

		/// <summary>
		/// Get a hierarchy data object by name.<br/>
		/// Uri Template: xml/GetHierarchyDataByName/{hierarchyType}/{hierarchyDataName}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="hierarchyDataName"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetHierarchyDataByName/{hierarchyType}/{hierarchyDataName}")]
		[Permission]
		HierarchyDataObject GetHierarchyDataByNameXml(string hierarchyType, string hierarchyDataName);

		/// <summary>
		/// Get all children of the specified hierarchy data by id.<br/>
		/// Uri Template: json/GetImmediateChildrenJson/{hierarchyType}/{parentHierarchyDataId}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="parentHierarchyDataId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetImmediateChildren/{hierarchyType}/{parentHierarchyDataId}")]
		[Permission]
		Collection<HierarchyDataObject> GetImmediateChildrenJson(string hierarchyType, string parentHierarchyDataId);

		/// <summary>
		/// Get all children of the specified hierarchy data by id.<br/>
		/// Uri Template: xml/GetImmediateChildrenJson/{hierarchyType}/{parentHierarchyDataId}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="parentHierarchyDataId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetImmediateChildren/{hierarchyType}/{parentHierarchyDataId}")]
		[Permission]
		Collection<HierarchyDataObject> GetImmediateChildrenXml(string hierarchyType, string parentHierarchyDataId);

		/// <summary>
		/// Get all children of the specified hierarchy data includes not immediately.<br/>
		/// Uri Template: json/GetAllChildrenJson/{hierarchyType}/{parentHierarchyDataId}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="parentHierarchyDataId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetAllChildren/{hierarchyType}/{parentHierarchyDataId}")]
		[Permission]
		Collection<HierarchyDataObject> GetAllChildrenJson(string hierarchyType, string parentHierarchyDataId);

		/// <summary>
		/// Get all children of the specified hierarchy data includes not immediately.<br/>
		/// Uri Template: xml/GetAllChildrenJson/{hierarchyType}/{parentHierarchyDataId}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="parentHierarchyDataId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetAllChildren/{hierarchyType}/{parentHierarchyDataId}")]
		[Permission]
		Collection<HierarchyDataObject> GetAllChildrenXml(string hierarchyType, string parentHierarchyDataId);

		/// <summary>
		/// Get all hierarchy data in specified hierarchy type.<br/>
		/// Uri Template: json/GetAllHierarchyDataJson/{hierarchyType}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetAllHierarchyData/{hierarchyType}")]
		[Permission]
		Collection<HierarchyDataObject> GetAllHierarchyDataJson(string hierarchyType);

		/// <summary>
		/// Get all hierarchy data in specified hierarchy type.<br/>
		/// Uri Template: xml/GetAllHierarchyDataJson/{hierarchyType}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetAllHierarchyData/{hierarchyType}")]
		[Permission]
		Collection<HierarchyDataObject> GetAllHierarchyDataXml(string hierarchyType);

		/// <summary>
		/// Hard delete a hierarchy data with all its children by id. <br/>
		/// Uri Template: json/HardDeleteHierarchyData/{hierarchyDataId}
		/// </summary>
		/// <param name="hierarchyDataId"></param>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/HardDeleteHierarchyData/{hierarchyDataId}")]
		[Permission]
		void HardDeleteHierarchyDataJson(string hierarchyDataId);

		/// <summary>
		/// Hard delete a hierarchy data with all its children by id. <br/>
		/// Uri Template: xml/HardDeleteHierarchyData/{hierarchyDataId}
		/// </summary>
		/// <param name="hierarchyDataId"></param>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/HardDeleteHierarchyData/{hierarchyDataId}")]
		[Permission]
		void HardDeleteHierarchyDataXml(string hierarchyDataId);

		/// <summary>
		/// Find all hierarchies which include the query keyword in code or name.<br/>
		/// Uri Template: json/FindByKeyword?hierarchyType={hierarchyType}&amp;q={query}&amp;limit={maxReturnedCount}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="query"></param>
		/// <param name="maxReturnedCount"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindByKeyword?hierarchyType={hierarchyType}&q={query}&limit={maxReturnedCount}")]
		[Permission]
		Collection<HierarchyDataObject> FindByKeywordJson(string hierarchyType, string query, int maxReturnedCount);

		/// <summary>
		/// Find all hierarchies which include the query keyword in code or name.<br/>
		/// Uri Template: xml/FindByKeyword?hierarchyType={hierarchyType}&amp;q={query}&amp;limit={maxReturnedCount}
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="query"></param>
		/// <param name="maxReturnedCount"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindByKeyword?hierarchyType={hierarchyType}&q={query}&limit={maxReturnedCount}")]
		[Permission]
		Collection<HierarchyDataObject> FindByKeywordXml(string hierarchyType, string query, int maxReturnedCount);

		/// <summary>
		/// Query hierarchy data in all types by custom predicates.<br/>
		/// Uri Template: json/QueryHierarchyData?pageIndex={pageIndex}&amp;pageSize={pageSize}&amp;orderby={orderby}
		/// </summary>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="predicate">linq predicate which supports properties of <see cref="RapidWebDev.Platform.HierarchyDataObject"/> for query expression.</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/QueryHierarchyData?pageIndex={pageIndex}&pageSize={pageSize}&orderby={orderby}")]
		[Permission]
		HierarchyDataQueryResult QueryHierarchyDataJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);

		/// <summary>
		/// Query hierarchy data in all types by custom predicates.<br/>
		/// Uri Template: xml/QueryHierarchyData?pageIndex={pageIndex}&amp;pageSize={pageSize}&amp;orderby={orderby}
		/// </summary>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="predicate">linq predicate which supports properties of <see cref="RapidWebDev.Platform.HierarchyDataObject"/> for query expression.</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/QueryHierarchyData?pageIndex={pageIndex}&pageSize={pageSize}&orderby={orderby}")]
		[Permission]
		HierarchyDataQueryResult QueryHierarchyDataXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);
	}

	/// <summary>
	/// Hierarchy data query result
	/// </summary>
	[CollectionDataContract(ItemName = "HierarchyDataObject", Namespace = ServiceNamespaces.Platform)]
	public class HierarchyDataQueryResult : Collection<HierarchyDataObject>
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objects"></param>
        public HierarchyDataQueryResult(IList<HierarchyDataObject> objects): base(objects)
        {
        }
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public HierarchyDataQueryResult()
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
