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
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.ServiceModel;
using RapidWebDev.Common.Properties;
using RapidWebDev.Common;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
	/// <summary>
	/// external service for populate Application Object
	/// </summary>
	[ServiceContract(Name = "ApplicationService", Namespace = ServiceNamespaces.Platform, SessionMode = SessionMode.Allowed)]
	public interface IApplicationService
	{
		/// <summary>
		/// Save application. Empty application id of application object means that the specified application object is new created. 
		/// And after it's inserted into database, the database generated id will be set back to property id.
		/// Non-empty application id means to update an existed application. 
		/// If the non-empty application id is not available in database, it throws exception ValidationException.
		/// </summary>
		/// <param name="applicationObject"></param>
		/// <exception cref="RapidWebDev.Common.Validation.ValidationException">If the non-empty application id is not available in database, it throws exception ValidationException.</exception>
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, UriTemplate = "json/Save")]
		[Permission]
		string SaveJson(ApplicationObject applicationObject);

		/// <summary>
		/// Get application biz object by id. Returns null if the application id is invalid.
		/// </summary>
		/// <param name="applicationId"></param>
		/// <returns>returns null if the application id is invalid.</returns>
		[OperationContract]
		[WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, UriTemplate = "json/Get/{applicationId}")]
		[Permission]
		ApplicationObject GetJson(string applicationId);

		/// <summary>
		/// Get application biz object by name with ignoring character case. Returns null if the application id is invalid.
		/// </summary>
		/// <param name="applicationName"></param>
		/// <returns>Returns null if the application name is invalid.</returns>
		[OperationContract]
		[WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, UriTemplate = "json/GetByName/{applicationName}")]
		[Permission]
		ApplicationObject GetByNameJson(string applicationName);

		/// <summary>
		/// Returns true if specified application name exists with ignoring character case.
		/// </summary>
		/// <param name="applicationName"></param>
		/// <returns>Returns true if specified application name exists.</returns>
		[OperationContract]
		[WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, UriTemplate = "json/Exists/{applicationName}")]
		[Permission]
		bool ExistsJson(string applicationName);
	}
}