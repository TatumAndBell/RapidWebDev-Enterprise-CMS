/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi , Email: tim.long.yi@RapidWebDev.org

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
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// external services for populate the membership object
    /// </summary>
    [ServiceContract(Name = "MembershipService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
    public interface IMembershipService
    {
        /// <summary>
        /// Add/update user object depends on whether identity of object is empty or not.<br />
        /// Uri Template: json/Save
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/Save")]
		[Permission]
        string SaveJson(UserObject user);

		/// <summary>
        /// Add/update user object depends on whether identity of object is empty or not.<br />
        /// Uri Template: xml/Save
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "xml/Save")]
		[Permission]
        string SaveXml(UserObject user);
		
        /// <summary>
        /// Resolve user objects from enumerable user ids.
        /// UriTemplate="json/BulkGet"
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/BulkGet")]
		[Permission]
        Collection<UserObject> BulkGetJson(IdCollection userIds);
		
		/// <summary>
        /// Resolve user objects from enumerable user ids.
        /// UriTemplate="xml/BulkGet"
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/BulkGet")]
		[Permission]
        Collection<UserObject> BulkGetXml(IdCollection userIds);

        /// <summary>
        /// Get user by user name.
        /// UriTemplate = "json/GetByName/{userName}"
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetByName/{userName}")]
		[Permission]
        UserObject GetByNameJson(string userName);
		
        /// <summary>
        /// Get user by user name.
        /// UriTemplate = "xml/GetByName/{userName}"
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetByName/{userName}")]
		[Permission]
        UserObject GetByNameXml(string userName);

        /// <summary>
        /// Get user object by user id.
        /// UriTemplate = "json/GetById/{userId}
        /// </summary>        
        /// <param name="userId">user id</param>
        /// <returns></returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetById/{userId}")]
		[Permission]
        UserObject GetByIdJson(string userId);

		/// <summary>
        /// Get user object by user id.
        /// UriTemplate = "xml/GetById/{userId}
        /// </summary>        
        /// <param name="userId">user id</param>
        /// <returns></returns>
        [OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetById/{userId}")]
		[Permission]
        UserObject GetByIdXml(string userId);
		
        /// <summary>
        /// Change password of specified user. 
        /// UriTemplate = "json/ChangePassword/{userId}"
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>returns true if operation successfully.</returns>
        [OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/ChangePassword/{userId}")]
		[Permission]
        bool ChangePasswordJson(string userId);
		
		/// <summary>
        /// Change password of specified user. 
        /// UriTemplate = "xml/ChangePassword/{userId}"
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>returns true if operation successfully.</returns>
        [OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/ChangePassword/{userId}")]
		[Permission]
        bool ChangePasswordXml(string userId);

        /// <summary>
        /// Find user business objects by custom predicates.
        /// UriTemplate = "json/QueryUsers?pageindex={pageIndex}&amp;pagesize={pageSize}&amp;orderby={orderby}"
        /// </summary>        
        /// <param name="orderby">sorting field and direction</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>       
        /// <param name="predicate">linq predicate. see user properties for predicate at <see cref="RapidWebDev.Platform.Linq.User"/>.</param>
        /// <returns>Returns enumerable user objects</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/QueryUsers?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
		[Permission]
        UserQueryResult QueryUsersJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);

        /// <summary>
        /// Find user business objects by custom predicates.
        /// UriTemplate = "xml/QueryUsers?pageindex={pageIndex}&amp;pagesize={pageSize}&amp;orderby={orderby}"
        /// </summary>        
        /// <param name="orderby">sorting field and direction</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>       
        /// <param name="predicate">linq predicate. see user properties for predicate at <see cref="RapidWebDev.Platform.Linq.User"/>.</param>
        /// <returns>Returns enumerable user objects</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/QueryUsers?pageindex={pageIndex}&pagesize={pageSize}&orderby={orderby}")]
		[Permission]
        UserQueryResult QueryUsersXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate);
    }

    /// <summary>
    /// Use Data query result
    /// </summary>
    [CollectionDataContract(ItemName = "UserObject", Namespace = ServiceNamespaces.Platform)]
    public class UserQueryResult : Collection<UserObject>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objects"></param>
        public UserQueryResult(IList<UserObject> objects)
            : base(objects)
        {
        }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public UserQueryResult()
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
