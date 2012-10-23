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

using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using RapidWebDev.Common;
using RapidWebDev.FileManagement.Properties;
using RapidWebDev.UI;

namespace RapidWebDev.FileManagement.Web
{
	/// <summary>
	/// Handler used to upload file in file management.
	/// </summary>
	public class FileUploadHttpHandler : IHttpHandler, IRequiresSessionState
    {
		private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
		private static IFileManagementApi fileManagementApi = SpringContext.Current.GetObject<IFileManagementApi>();
		private static IPermissionBridge permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();

        /// <summary>
        /// You will need to configure this handler in the web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        #region IHttpHandler Members

		bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
			HttpPostedFile httpPostedFile = context.Request.Files["rapidwebdev.filemanagement"];
			if (httpPostedFile == null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				context.Response.StatusDescription = Resources.UploadingFileNotFound;
				return;
			}

			string category = context.Request["category"];
			if (string.IsNullOrEmpty(category))
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				context.Response.StatusDescription = Resources.FileCategoryQueryStringParameterNotFound;
				return;
			}

			string permissionValue = string.Format(CultureInfo.InvariantCulture, "FileManagement.{0}.Upload", category);
			if (!permissionBridge.HasPermission(permissionValue))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				context.Response.StatusDescription = Resources.NoPermissionToUploadFiles;
				return;
			}

			FileUploadObject fileUploadObject = new FileUploadObject
			{
				FileName = Path.GetFileName(httpPostedFile.FileName),
				Category = category, 
				Stream = httpPostedFile.InputStream
			};

			FileHeadObject fileHeadObject = fileManagementApi.Save(fileUploadObject);
			FileWebObject fileObject = new FileWebObject(fileHeadObject.Id, fileHeadObject.FileName, fileHeadObject.BytesCount);
			string fileJson = serializer.Serialize(fileObject);

			context.Response.Clear();
			context.Response.StatusCode = (int)HttpStatusCode.OK;
			context.Response.ContentType = "application/json";
			context.Response.Write(fileJson);
			context.Response.End();
        }

        #endregion
    }
}