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

using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using RapidWebDev.Common;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// external service for populate Sequence Number
    /// </summary>
    [ServiceContract(Name = "SequenceService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
    public interface ISequenceService
    {
        /// <summary>
        /// Create sequence number on specified type for special object id.
        /// The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
        /// UriTemplate = "json/CreateSingle?objectId={objectId}&amp;sequenceNoType={sequenceNoType}"
        /// </summary>        
        /// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
        /// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/CreateSingle?objectId={objectId}&sequenceNoType={sequenceNoType}")]
		[Permission]
        long CreateSingleJson(string objectId, string sequenceNoType);
		
		/// <summary>
        /// Create sequence number on specified type for special object id.
        /// The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
        /// UriTemplate = "json/CreateSingle?objectId={objectId}&amp;sequenceNoType={sequenceNoType}"
        /// </summary>        
        /// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
        /// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/CreateSingle?objectId={objectId}&sequenceNoType={sequenceNoType}")]
		[Permission]
        long CreateSingleXml(string objectId, string sequenceNoType);

      
        /// <summary>
        /// Create sequence numbers on specified sequence number type.
        ///  The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
        /// UriTemplate = "json/CreateMultiple?objectId={objectId}&amp;sequenceNoType={sequenceNoType}&amp;sequenceNoCount={sequenceNoCount}"
        /// </summary>        
        /// <param name="objectId"></param>
        /// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
        /// <param name="sequenceNoCount">Indicates how many sequence number is required.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "json/CreateMultiple?objectId={objectId}&sequenceNoType={sequenceNoType}&sequenceNoCount={sequenceNoCount}")]
		[Permission]
        Collection<long> CreateMultipleJson(string objectId,string sequenceNoType, string sequenceNoCount);

        /// <summary>
        /// Create sequence numbers on specified sequence number type.
        ///  The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
        /// UriTemplate = "xml/CreateMultiple?objectId={objectId}&amp;sequenceNoType={sequenceNoType}&amp;sequenceNoCount={sequenceNoCount}"
        /// </summary>        
        /// <param name="objectId"></param>
        /// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
        /// <param name="sequenceNoCount">Indicates how many sequence number is required.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, UriTemplate = "xml/CreateMultiple?objectId={objectId}&sequenceNoType={sequenceNoType}&sequenceNoCount={sequenceNoCount}")]
		[Permission]
        Collection<long> CreateMultipleXml(string objectId,string sequenceNoType, string sequenceNoCount);
    }
}
