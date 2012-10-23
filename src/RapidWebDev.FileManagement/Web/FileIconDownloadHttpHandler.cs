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
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using RapidWebDev.Common;
using Image=System.Drawing.Image;
using RapidWebDev.FileManagement.Properties;

namespace RapidWebDev.FileManagement.Web
{
	/// <summary>
	/// Handler used to download file icon in file management.
	/// </summary>
	public class FileIconDownloadHttpHandler : IHttpHandler
    {
		private static IFileIconApi fileIconApi = SpringContext.Current.GetObject<IFileIconApi>();

        /// <summary>
        /// You will need to configure this handler in the web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
			string fileExtensionName = context.Request["ext"];
			if (string.IsNullOrEmpty(fileExtensionName))
			{
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
				context.Response.StatusDescription = Resources.InvalidQueryStringParameterExt;
				context.Response.End();
				return;
			}

			IconSize iconSize = IconSize.Pixel16x16;
            try
            {
				string sizeText = context.Request["size"];
				if (!string.IsNullOrEmpty(sizeText))
					iconSize = (IconSize)Enum.Parse(typeof(IconSize), sizeText);
            }
            catch (FormatException)
            {
            }

			using (Stream iconStream = fileIconApi.ResolveIcon(fileExtensionName, iconSize))
			{
				Image image = Image.FromStream(iconStream);
				context.Response.ContentType = "image/gif";
				context.Response.Cache.VaryByParams["ext"] = true;
				context.Response.Cache.VaryByParams["size"] = true;
				image.Save(context.Response.OutputStream, ImageFormat.Gif);
			}
        }

        #endregion
    }
}

