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
	/// The external services for concrete data.
	/// </summary>
	[ServiceContract(Name = "ConcreteDataService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
	public interface IConcreteDataService
	{
		/// <summary>
		/// Add/update concrete data object depends on whether identity of object is empty or not.<br />
		/// Uri Template: json/Save
		/// </summary>
		/// <param name="concreteDataObject">The name of concrete data should be unique in a concrete data type.</param>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/Save")]
		[Permission]
		string SaveJson(ConcreteDataObject concreteDataObject);

		/// <summary>
		/// Add/update concrete data object depends on whether identity of object is empty or not.<br />
		/// Uri Template: xml/Save
		/// </summary>
		/// <param name="concreteDataObject">The name of concrete data should be unique in a concrete data type.</param>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/Save")]
		[Permission]
		string SaveXml(ConcreteDataObject concreteDataObject);

		/// <summary>
		/// Get an non-deleted concrete data by id. <br />
		/// Uri Template: json/GetById/{concreteDataId}
		/// </summary>
		/// <param name="concreteDataId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetById/{concreteDataId}")]
		[Permission]
		ConcreteDataObject GetByIdJson(string concreteDataId);

		/// <summary>
		/// Get an non-deleted concrete data by id. <br />
		/// Uri Template: xml/GetById/{concreteDataId}
		/// </summary>
		/// <param name="concreteDataId"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetById/{concreteDataId}")]
		[Permission]
		ConcreteDataObject GetByIdXml(string concreteDataId);

		/// <summary>
		/// Get an non-deleted concrete data by name.<br />
		/// Uri Template: json/GetByName/{type}/{name}
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetByName/{type}/{name}")]
		[Permission]
		ConcreteDataObject GetByNameJson(string type, string name);

		/// <summary>
		/// Get an non-deleted concrete data by name.<br />
		/// Uri Template: xml/GetByName/{type}/{name}
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetByName/{type}/{name}")]
		[Permission]
		ConcreteDataObject GetByNameXml(string type, string name);

		/// <summary>
		/// Find non-deleted concrete data by a keyword which may be included in concrete data name or value.<br />
		/// Uri Template: json/FindByKeyword?concreteDataType={concreteDataType}&amp;q={query}&amp;limit={limit}
		/// </summary>
		/// <param name="concreteDataType">Concrete data type.</param>
		/// <param name="query">Keyword included in Name or Value. Null or empty value indicates to query all records in the concrete type.</param>
		/// <param name="limit">Maximum number of returned records.</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindByKeyword?concreteDataType={concreteDataType}&q={query}&limit={limit}")]
		[Permission]
		Collection<ConcreteDataObject> FindByKeywordJson(string concreteDataType, string query, int limit);

		/// <summary>
		/// Find non-deleted concrete data by a keyword which may be included in concrete data name or value.<br />
		/// Uri Template: xml/FindByKeyword?concreteDataType={concreteDataType}&amp;q={query}&amp;limit={limit}
		/// </summary>
		/// <param name="concreteDataType">Concrete data type.</param>
		/// <param name="query">Keyword included in Name or Value. Null or empty value indicates to query all records in the concrete type.</param>
		/// <param name="limit">Maximum number of returned records.</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindByKeyword?concreteDataType={concreteDataType}&q={query}&limit={limit}")]
		[Permission]
		Collection<ConcreteDataObject> FindByKeywordXml(string concreteDataType, string query, int limit);

		/// <summary>
		/// Find all available concrete data types.<br />
		/// Uri Template: json/FindConcreteDataTypes
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindConcreteDataTypes")]
		[Permission]
		Collection<string> FindConcreteDataTypesJson();

		/// <summary>
		/// Find all available concrete data types.<br />
		/// Uri Template: xml/FindConcreteDataTypes
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindConcreteDataTypes")]
		[Permission]
		Collection<string> FindConcreteDataTypesXml();

		/// <summary>
		/// Find concrete data in all types by custom predicates.<br />
		/// Uri Template: json/QueryConcreteData?pageindex={pageIndex}&amp;pagesize={pageSize}&amp;orderby={orderby}
		/// </summary>
		/// <param name="orderby"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="predicate"></param>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/QueryConcreteData?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
		[Permission]
		ConcreteDataQueryResult QueryConcreteDataJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);

		/// <summary>
		/// Find concrete data in all types by custom predicates.<br />
		/// Uri Template: xml/QueryConcreteData?pageindex={pageIndex}&amp;pagesize={pageSize}&amp;orderby={orderby}
		/// </summary>
		/// <param name="orderby"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="predicate"></param>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/QueryConcreteData?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
		[Permission]
		ConcreteDataQueryResult QueryConcreteDataXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);
	}

	/// <summary>
	/// Concrete data query result
	/// </summary>
	[CollectionDataContract(ItemName = "ConcreteDataObject", Namespace = ServiceNamespaces.Platform)]
	public class ConcreteDataQueryResult : Collection<ConcreteDataObject>
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objects"></param>
        public ConcreteDataQueryResult(IList<ConcreteDataObject> objects) : base(objects)
        {
        }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public ConcreteDataQueryResult()
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