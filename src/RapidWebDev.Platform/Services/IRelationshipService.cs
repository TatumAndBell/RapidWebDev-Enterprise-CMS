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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// external service for CRUD relationships
    /// </summary>
    [ServiceContract(Name = "RelationshipService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
    public interface IRelationshipService
    {
        /// <summary>
        /// Save relationship b/w 2 entities on special relationship type.
        /// UriTemplate = "json/Save?objectXId={objectXId}&amp;objectYId={objectYId}&amp;RelationshipType={RelationshipType}&amp;Ordinal={Ordinal}"
        /// </summary>        
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>
        /// <param name="RelationshipType"></param>
        /// <param name="Ordinal"></param>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/Save?objectXId={objectXId}&objectYId={objectYId}&RelationshipType={RelationshipType}&Ordinal={Ordinal}")]
		[Permission]
        void SaveJson(string objectXId,string objectYId, string RelationshipType, string Ordinal);
 
		/// <summary>
        /// Save relationship b/w 2 entities on special relationship type.
        /// UriTemplate = "xml/Save?objectXId={objectXId}&amp;objectYId={objectYId}&amp;RelationshipType={RelationshipType}&amp;Ordinal={Ordinal}"
        /// </summary>        
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>
        /// <param name="RelationshipType"></param>
        /// <param name="Ordinal"></param>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/Save?objectXId={objectXId}&objectYId={objectYId}&RelationshipType={RelationshipType}&Ordinal={Ordinal}")]
		[Permission]
        void SaveXml(string objectXId,string objectYId, string RelationshipType, string Ordinal);

      
        /// <summary>
        /// Remove the relationship b/w X and Y in the special relationship type.
        /// UriTemplate = "json/Remove?objectXId={objectXId}&amp;objectYId={objectYId}&amp;relationshipType={relationshipType}"
        /// </summary>        
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>
        /// <param name="relationshipType"></param>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/Remove?objectXId={objectXId}&objectYId={objectYId}&relationshipType={relationshipType}")]
		[Permission]
        void RemoveJson(string objectXId, string objectYId, string relationshipType);

		/// <summary>
        /// Remove the relationship b/w X and Y in the special relationship type.
        /// UriTemplate = "xml/Remove?objectXId={objectXId}&amp;objectYId={objectYId}&amp;relationshipType={relationshipType}"
        /// </summary>        
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>
        /// <param name="relationshipType"></param>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/Remove?objectXId={objectXId}&objectYId={objectYId}&relationshipType={relationshipType}")]
		[Permission]
        void RemoveXml(string objectXId, string objectYId, string relationshipType);

		
        /// <summary>
        /// Get the object related to the target object in special type. 
        /// UriTemplate = "json/GetOneToOneJson/{objectId}/{relationshipType}"
        /// </summary>       
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetOneToOne/{objectId}/{relationshipType}")]
		[Permission]
        RelationshipObject GetOneToOneJson(string objectId, string relationshipType);

		/// <summary>
        /// Get the object related to the target object in special type.
        /// UriTemplate = "xml/GetOneToOneJson/{objectId}/{relationshipType}"
        /// </summary>        
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetOneToOne/{objectId}/{relationshipType}")]
		[Permission]
        RelationshipObject GetOneToOneXml(string objectId, string relationshipType);

		
        /// <summary>
        /// Get the object related to the target object in special type.
        /// UriTemplate = "json/GetManyToOne/{objectId}/{relationshipType}"
        /// </summary>        
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetManyToOne/{objectId}/{relationshipType}")]
		[Permission]
        RelationshipObject GetManyToOneJson(string objectId, string relationshipType);

		 /// <summary>
        /// Get the object related to the target object in special type.
        /// UriTemplate = "xml/GetManyToOne/{objectId}/{relationshipType}"
        /// </summary>        
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetManyToOne/{objectId}/{relationshipType}")]
		[Permission]
        RelationshipObject GetManyToOneXml(string objectId, string relationshipType);
		
        /// <summary>
        /// Get the object related to the target object in special type.
        /// UriTemplate = "json/GetOneToMany/{objectId}/{relationshipType}"
        /// </summary>        
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/GetOneToMany/{objectId}/{relationshipType}")]
		[Permission]
        Collection<RelationshipObject> GetOneToManyJson(string objectId, string relationshipType);

		/// <summary>
        /// Get the object related to the target object in special type.
        /// UriTemplate = "xml/GetOneToMany/{objectId}/{relationshipType}"
        /// </summary>        
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/GetOneToMany/{objectId}/{relationshipType}")]
		[Permission]
        Collection<RelationshipObject> GetOneToManyXml(string objectId, string relationshipType);

        /// <summary>
        /// Find all objects related to the target object in any special relationship type.
        /// UriTemplate = "json/FindAllRelationship/{objectId}"
        /// </summary>        
        /// <param name="objectId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/FindAllRelationship/{objectId}")]
		[Permission]
        Collection<RelationshipObject> FindAllRelationshipJson(string objectId);
		
		  /// <summary>
        /// Find all objects related to the target object in any special relationship type. 
        /// UriTemplate = "xml/FindAllRelationship/{objectId}"
        /// </summary>       
        /// <param name="objectId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/FindAllRelationship/{objectId}")]
		[Permission]
        Collection<RelationshipObject> FindAllRelationshipXml(string objectId);
    }
}
