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
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using RapidWebDev.Common;
using RapidWebDev.FileManagement.Properties;
using RapidWebDev.UI;

namespace RapidWebDev.FileManagement.Web
{
	/// <summary>
	/// Handler used to download file managed in file management.
	/// </summary>
	public class FileDownloadHttpHandler : IHttpHandler, IRequiresSessionState
	{
		private static IFileManagementApi fileManagementApi = SpringContext.Current.GetObject<IFileManagementApi>();
		private static IFileStorageApi fileStorageApi = SpringContext.Current.GetObject<IFileStorageApi>();
		private static IPermissionBridge permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();
		private static readonly string fileDownloadName = "FileDownloadService.svc";
		private static readonly UriTemplate fileDownloadUriTemplate = new UriTemplate("FileDownloadService.svc/fileid/{fileid}");
		private static Dictionary<string, string> mimeTypeContainer;

		bool IHttpHandler.IsReusable
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
			Guid fileId = Guid.Empty;
			try
			{
				fileId = this.ParseDownloadFileId(context);
			}
			catch (BadRequestException exp)
			{
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
				context.Response.StatusDescription = exp.Message;
				context.Response.End();
				return;
			}

			FileHeadObject fileHeadObject = fileManagementApi.Load(fileId);
			string fileCategory = !string.IsNullOrEmpty(fileHeadObject.Category) ? fileHeadObject.Category : "NULL";
			string permissionValue = string.Format(CultureInfo.InvariantCulture, "FileManagement.{0}.Upload", fileCategory);
			if (!permissionBridge.HasPermission(permissionValue))
			{
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
				context.Response.StatusDescription = Resources.NoPermissionToDownloadFiles;
				return;
			}

			AddDownloadHeaders(context, fileHeadObject);

			// If we have 0 byte file, flush the response otherwise IE 5.5 does not launch a "Save As" dialog.
			context.Response.Flush();

			byte[] buffer = new byte[Int16.MaxValue + 1];
			try
			{
				using (Stream stream = fileStorageApi.Load(fileHeadObject))
				{
					// this should stream the content at a rate the client can handle
					// see page 4 of the article at http://www.asp.net/Forums/ShowPost.aspx?tabindex=1&PostID=84629
					while (context.Response.IsClientConnected)
					{
						int size = stream.Read(buffer, 0, buffer.Length);
						if (size > 0)
						{
							context.Response.OutputStream.Write(buffer, 0, size);
							context.Response.Flush();
						}
						else
							break;
					}
				}
			}
			catch (FileNotFoundException)
			{
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
				context.Response.StatusDescription = Resources.FileNotFound;
				context.Response.End();
				return;
			}
			catch
			{
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
				context.Response.StatusDescription = Resources.UnknownError;
				context.Response.End();
				return;
			}
		}

		private Guid ParseDownloadFileId(HttpContext context)
		{
			string absoluteUri = context.Request.Url.AbsoluteUri;
			string absoluteUriPrefix = absoluteUri.Substring(0, absoluteUri.IndexOf(fileDownloadName, StringComparison.OrdinalIgnoreCase) - 1);

			UriTemplateMatch results = fileDownloadUriTemplate.Match(new Uri(absoluteUriPrefix), HttpContext.Current.Request.Url);
			if (results == null || results.BoundVariables.Count != 1)
			{
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
				throw new BadRequestException(Resources.InvalidFileDownloadUri);
			}

			try
			{
				return new Guid(results.BoundVariables["fileid"]);
			}
			catch
			{
				throw new BadRequestException(Resources.InvalidFileId);
			}
		}

		/// <summary>
		/// Adds the correct response headers for a file download request.
		/// </summary>
		/// <param name="context">HttpContext</param>
		/// <param name="fileHeadObject">The file object representing the file to stream.</param>
		public static void AddDownloadHeaders(HttpContext context, FileHeadObject fileHeadObject)
		{
			HttpContext httpContext = context as HttpContext;

			string requestLanguage = string.Empty;
			if (httpContext.Request.UserLanguages.Length > 0)
				requestLanguage = httpContext.Request.UserLanguages[0];

			httpContext.Response.AppendHeader("Content-Length", fileHeadObject.BytesCount.ToString(CultureInfo.InvariantCulture));
			httpContext.Response.ContentType = GetMimeType(fileHeadObject.FileName);

			string disposition = BuildContentDispositionHeader(fileHeadObject, requestLanguage,
				httpContext.Request.Browser.Type,
				httpContext.Request.Browser.MajorVersion,
				httpContext.Request.Browser.Platform);
			httpContext.Response.AppendHeader("Content-Disposition", disposition);  // gives save-as dialog and filename
		}

		private static string GetMimeType(string fileExtensionName)
		{
			if (mimeTypeContainer == null)
				mimeTypeContainer = InitializeMimeTypes();

			string mimeType;

			// If the mimetype is not found for the extension we set the mimetype default mime type.
			bool extFound = mimeTypeContainer.ContainsKey(fileExtensionName);
			if (!extFound)
				mimeType = "application/download";
			else
				mimeType = mimeTypeContainer[fileExtensionName];

			return mimeType;
		}

		private static Dictionary<string, string> InitializeMimeTypes()
		{
			Dictionary<string, string> mimeTypes = new Dictionary<string, string>(215, StringComparer.CurrentCultureIgnoreCase);
			mimeTypes.Add(".323", "text/h323");
			mimeTypes.Add(".act", "text/xml");
			mimeTypes.Add(".actproj", "text/plain");
			mimeTypes.Add(".ai", "application/postscript");
			mimeTypes.Add(".aif", "audio/aiff");
			mimeTypes.Add(".aifc", "audio/aiff");
			mimeTypes.Add(".aiff", "audio/aiff");
			mimeTypes.Add(".aom", "application/aom");
			mimeTypes.Add(".asf", "video/x-ms-asf");
			mimeTypes.Add(".asm", "text/plain");
			mimeTypes.Add(".asx", "video/x-ms-asf");
			mimeTypes.Add(".au", "audio/basic");
			mimeTypes.Add(".avi", "video/avi");
			mimeTypes.Add(".bmp", "image/bmp");
			mimeTypes.Add(".c", "text/plain");
			mimeTypes.Add(".cab", "application/cab");
			mimeTypes.Add(".cat", "application/vnd.ms-pki.seccat");
			mimeTypes.Add(".cc", "text/plain");
			mimeTypes.Add(".cdf", "application/x-cdf");
			mimeTypes.Add(".cer", "application/x-x509-ca-cert");
			mimeTypes.Add(".cod", "text/plain");
			mimeTypes.Add(".cpp", "text/plain");
			mimeTypes.Add(".crl", "application/pkix-crl");
			mimeTypes.Add(".crt", "application/x-x509-ca-cert");
			mimeTypes.Add(".cs", "text/plain");
			mimeTypes.Add(".css", "text/css");
			mimeTypes.Add(".cxx", "text/plain");
			mimeTypes.Add(".dbs", "text/plain");
			mimeTypes.Add(".def", "text/plain");
			mimeTypes.Add(".der", "application/x-x509-ca-cert");
			mimeTypes.Add(".dib", "image/bmp");
			mimeTypes.Add(".disco", "text/xml");
			mimeTypes.Add(".dgn", "image/vnd.dgn");
			mimeTypes.Add(".dll", "application/x-msdownload");
			mimeTypes.Add(".dlm", "text/dlm");
			mimeTypes.Add(".doc", "application/msword");
			mimeTypes.Add(".dot", "application/msword");
			mimeTypes.Add(".dsp", "text/plain");
			mimeTypes.Add(".dsw", "text/plain");
			mimeTypes.Add(".dwf", "Model/vnd.dwf");
			mimeTypes.Add(".dwg", "image/vnd.dwg");
			mimeTypes.Add(".dxf", "image/vnd.dxf");
			mimeTypes.Add(".edn", "application/vnd.adobe.edn");
			mimeTypes.Add(".eml", "message/rfc822");
			mimeTypes.Add(".eps", "application/postscript");
			mimeTypes.Add(".etd", "application/x-ebx");
			mimeTypes.Add(".etp", "text/plain");
			mimeTypes.Add(".exe", "application/x-msdownload");
			mimeTypes.Add(".ext", "text/plain");
			mimeTypes.Add(".fdf", "application/vnd.fdf");
			mimeTypes.Add(".fif", "application/fractals");
			mimeTypes.Add(".fky", "text/plain");
			mimeTypes.Add(".gif", "image/gif");
			mimeTypes.Add(".gz", "application/x-gzip");
			mimeTypes.Add(".h", "text/plain");
			mimeTypes.Add(".hpp", "text/plain");
			mimeTypes.Add(".hqx", "application/mac-binhex40");
			mimeTypes.Add(".hta", "application/hta");
			mimeTypes.Add(".htc", "text/x-component");
			mimeTypes.Add(".htm", "text/html");
			mimeTypes.Add(".html", "text/html");
			mimeTypes.Add(".htt", "text/webviewhtml");
			mimeTypes.Add(".hxx", "text/plain");
			mimeTypes.Add(".i", "text/plain");
			mimeTypes.Add(".ico", "image/x-icon");
			mimeTypes.Add(".ics", "text/calendar");
			mimeTypes.Add(".idl", "text/plain");
			mimeTypes.Add(".iii", "application/x-iphone");
			mimeTypes.Add(".inc", "text/plain");
			mimeTypes.Add(".inl", "text/plain");
			mimeTypes.Add(".ins", "application/x-internet-signup");
			mimeTypes.Add(".iqy", "text/x-ms-iqy");
			mimeTypes.Add(".isp", "application/x-internet-signup");
			mimeTypes.Add(".java", "text/java");
			mimeTypes.Add(".jfif", "image/jpeg");
			mimeTypes.Add(".jpe", "image/jpeg");
			mimeTypes.Add(".jpeg", "image/jpeg");
			mimeTypes.Add(".jpg", "image/jpeg");
			mimeTypes.Add(".js", "application/x-javascript");
			mimeTypes.Add(".jsl", "text/plain");
			mimeTypes.Add(".kci", "text/plain");
			mimeTypes.Add(".latex", "application/x-latex");
			mimeTypes.Add(".lgn", "text/plain");
			mimeTypes.Add(".lst", "text/plain");
			mimeTypes.Add(".m1v", "video/mpeg");
			mimeTypes.Add(".m3u", "audio/x-mpegurl");
			mimeTypes.Add(".mak", "text/plain");
			mimeTypes.Add(".man", "application/x-troff-man");
			mimeTypes.Add(".map", "text/plain");
			mimeTypes.Add(".mdb", "application/msaccess");
			mimeTypes.Add(".mfp", "application/x-shockwave-flash");
			mimeTypes.Add(".mht", "message/rfc822");
			mimeTypes.Add(".mhtml", "message/rfc822");
			mimeTypes.Add(".mid", "audio/mid");
			mimeTypes.Add(".midi", "audio/mid");
			mimeTypes.Add(".mk", "text/plain");
			mimeTypes.Add(".mp2", "video/mpeg");
			mimeTypes.Add(".mp2v", "video/mpeg");
			mimeTypes.Add(".mp3", "audio/mpeg");
			mimeTypes.Add(".mpa", "video/mpeg");
			mimeTypes.Add(".mpe", "video/mpeg");
			mimeTypes.Add(".mpeg", "video/mpeg");
			mimeTypes.Add(".mpf", "application/vnd.ms-mediapackage");
			mimeTypes.Add(".mpg", "video/mpeg");
			mimeTypes.Add(".mpp", "application/msproject");
			mimeTypes.Add(".mpv2", "video/mpeg");
			mimeTypes.Add(".nmw", "application/nmwb");
			mimeTypes.Add(".nws", "message/rfc822");
			mimeTypes.Add(".odc", "text/x-ms-odc");
			mimeTypes.Add(".odh", "text/plain");
			mimeTypes.Add(".odl", "text/plain");
			mimeTypes.Add(".p10", "application/pkcs10");
			mimeTypes.Add(".p12", "application/x-pkcs12");
			mimeTypes.Add(".p7b", "application/x-pkcs7-certificates");
			mimeTypes.Add(".p7c", "application/pkcs7-mime");
			mimeTypes.Add(".p7m", "application/pkcs7-mime");
			mimeTypes.Add(".p7r", "application/x-pkcs7-certreqresp");
			mimeTypes.Add(".p7s", "application/pkcs7-signature");
			mimeTypes.Add(".pdf", "application/pdf");
			mimeTypes.Add(".pdx", "application/vnd.adobe.pdx");
			mimeTypes.Add(".pfx", "application/x-pkcs12");
			mimeTypes.Add(".pko", "application/vnd.ms-pki.pko");
			mimeTypes.Add(".plg", "text/html");
			mimeTypes.Add(".png", "image/png");
			mimeTypes.Add(".pot", "application/vnd.ms-powerpoint");
			mimeTypes.Add(".ppa", "application/vnd.ms-powerpoint");
			mimeTypes.Add(".pps", "application/vnd.ms-powerpoint");
			mimeTypes.Add(".ppt", "application/vnd.ms-powerpoint");
			mimeTypes.Add(".prc", "text/plain");
			mimeTypes.Add(".prf", "application/pics-rules");
			mimeTypes.Add(".prjpt5", "application/ProjectPoint5");
			mimeTypes.Add(".prjpt6", "application/ProjectPoint6");
			mimeTypes.Add(".ps", "application/postscript");
			mimeTypes.Add(".pwz", "application/vnd.ms-powerpoint");
			mimeTypes.Add(".py", "text/plain");
			mimeTypes.Add(".pyw", "text/plain");
			mimeTypes.Add(".rat", "application/rat-file");
			mimeTypes.Add(".rc", "text/plain");
			mimeTypes.Add(".rc2", "text/plain");
			mimeTypes.Add(".rct", "text/plain");
			mimeTypes.Add(".resx", "text/xml");
			mimeTypes.Add(".rgs", "text/plain");
			mimeTypes.Add(".rmf", "application/vnd.adobe.rmf");
			mimeTypes.Add(".rmi", "audio/mid");
			mimeTypes.Add(".rqy", "text/x-ms-rqy");
			mimeTypes.Add(".rtf", "application/msword");
			mimeTypes.Add(".rul", "text/plain");
			mimeTypes.Add(".s", "text/plain");
			mimeTypes.Add(".sit", "application/x-stuffit");
			mimeTypes.Add(".sln", "application/octet-stream");
			mimeTypes.Add(".snd", "audio/basic");
			mimeTypes.Add(".sol", "text/plain");
			mimeTypes.Add(".sor", "text/plain");
			mimeTypes.Add(".spc", "application/x-pkcs7-certificates");
			mimeTypes.Add(".spl", "application/futuresplash");
			mimeTypes.Add(".sql", "text/plain");
			mimeTypes.Add(".srf", "text/plain");
			mimeTypes.Add(".sst", "application/vnd.ms-pki.certstore");
			mimeTypes.Add(".stl", "application/vnd.ms-pki.stl");
			mimeTypes.Add(".swf", "application/x-shockwave-flash");
			mimeTypes.Add(".tab", "text/plain");
			mimeTypes.Add(".tar", "application/x-tar");
			mimeTypes.Add(".tdl", "text/xml");
			mimeTypes.Add(".tgz", "application/x-compressed");
			mimeTypes.Add(".tif", "image/tiff");
			mimeTypes.Add(".tiff", "image/tiff");
			mimeTypes.Add(".tlh", "text/plain");
			mimeTypes.Add(".tli", "text/plain");
			mimeTypes.Add(".trg", "text/plain");
			mimeTypes.Add(".txt", "text/plain");
			mimeTypes.Add(".udf", "text/plain");
			mimeTypes.Add(".udt", "text/plain");
			mimeTypes.Add(".uls", "text/iuls");
			mimeTypes.Add(".user", "text/plain");
			mimeTypes.Add(".usr", "text/plain");
			mimeTypes.Add(".vap", "text/plain");
			mimeTypes.Add(".vb", "text/plain");
			mimeTypes.Add(".vcf", "text/x-vcard");
			mimeTypes.Add(".vcproj", "text/plain");
			mimeTypes.Add(".vdx", "application/vnd.visio");
			mimeTypes.Add(".viw", "text/plain");
			mimeTypes.Add(".vsd", "application/vnd.visio");
			mimeTypes.Add(".vsdisco", "text/xml");
			mimeTypes.Add(".vsl", "application/vnd.visio");
			mimeTypes.Add(".vspscc", "text/plain");
			mimeTypes.Add(".vss", "application/vnd.visio");
			mimeTypes.Add(".vsscc", "text/plain");
			mimeTypes.Add(".vssscc", "text/plain");
			mimeTypes.Add(".vst", "application/vnd.visio");
			mimeTypes.Add(".vsu", "application/vnd.visio");
			mimeTypes.Add(".vsw", "application/vnd.visio");
			mimeTypes.Add(".vsx", "application/vnd.visio");
			mimeTypes.Add(".vtx", "application/vnd.visio");
			mimeTypes.Add(".wav", "audio/wav");
			mimeTypes.Add(".wax", "audio/x-ms-wax");
			mimeTypes.Add(".wiz", "application/msword");
			mimeTypes.Add(".wm", "video/x-ms-wm");
			mimeTypes.Add(".wma", "audio/x-ms-wma");
			mimeTypes.Add(".wmd", "application/x-ms-wmd");
			mimeTypes.Add(".wmv", "video/x-ms-wmv");
			mimeTypes.Add(".wmx", "video/x-ms-wmx");
			mimeTypes.Add(".wmz", "application/x-ms-wmz");
			mimeTypes.Add(".wpl", "application/vnd.ms-wpl");
			mimeTypes.Add(".wsc", "text/scriptlet");
			mimeTypes.Add(".wvx", "video/x-ms-wvx");
			mimeTypes.Add(".xbm", "image/x-xbitmap");
			mimeTypes.Add(".xdp", "application/vnd.adobe.xdp+xml");
			mimeTypes.Add(".xfd", "application/vnd.adobe.xfd+xml");
			mimeTypes.Add(".xfdf", "application/vnd.adobe.xfdf");
			mimeTypes.Add(".xls", "application/vnd.ms-excel");
			mimeTypes.Add(".xml", "text/xml");
			mimeTypes.Add(".xsl", "text/xml");
			mimeTypes.Add(".xsn", "application/octet-stream");
			mimeTypes.Add(".z", "application/x-compress");
			mimeTypes.Add(".zip", "application/x-zip-compressed");

			return mimeTypes;
		}

		/// <summary>
		/// Note that the httpContext param can legally be null.  See callers.
		/// </summary>
		/// <remarks>
		/// The Content-Disposition HTTP response header gives us some control over how the client will react
		/// to the file download.  In particular, whether or not the client will launch a "Save As" dialog when a file
		/// is requested for dowload.  See RFC 1806 for detials.</remarks>
		/// <remarks>We tried excluding "attachment" from the contentDisposition header for IE 5.5 to avoid
		/// getting a double download prompt.  Except, this causes a problem for the case
		/// where Office2K is installed on IE5.5 -- for that case we don't get any download prompt
		/// unless we have the header.  So on IE5.5 we get a double download prompt.
		/// </remarks>
		/// <param name="fileHeadObject"></param>
		/// <param name="userLanguage"></param>
		/// <param name="capabilitiesType"></param>
		/// <param name="majorVersion"></param>
		/// <param name="platform"></param>
		/// <returns></returns>
		private static string BuildContentDispositionHeader(FileHeadObject fileHeadObject, string userLanguage, string capabilitiesType, int majorVersion, string platform)
		{
			// To build and hold the Content-Disposition header.
			StringBuilder dispositionBuilder = new StringBuilder();

			dispositionBuilder.Append("attachment;");

			// Note that we cannot determine browser details from a WebOperationContext,
			// so we simply ignore this check in that case.  See callers.
			bool requestIsFromXPIE6Japanese = false;
			if ((!string.IsNullOrEmpty(capabilitiesType)) && majorVersion > -1 && (!string.IsNullOrEmpty(platform)))
				requestIsFromXPIE6Japanese = IfRequestIsFromXPIE6Japanese(userLanguage, capabilitiesType, majorVersion, platform);

			dispositionBuilder.AppendFormat("filename=\"{0}\"", GetDownloadFilename(fileHeadObject, userLanguage, requestIsFromXPIE6Japanese));

			string disposition = dispositionBuilder.ToString();

			// Max line length is 78 characters, including header name; we'll limit the first line to less characters
			// to make sure we have space for the header and the line terminators
			if (!requestIsFromXPIE6Japanese)
				disposition = TruncateHeaderLine(fileHeadObject, disposition, 100);

			return disposition;
		}

		/// <summary>
		/// Returns true if the request from WINXP IE6 set to Japanese language.
		/// </summary>
		/// <returns></returns>
		private static bool IfRequestIsFromXPIE6Japanese(string userLanguage, string capabilitiesType, int majorVersion, string platform)
		{
			bool isXpIe6Japanese = false;

			if (userLanguage.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
			{
				if (BrowserIsIe(capabilitiesType) && majorVersion >= 6)
				{
					if (ClientIsWinXp(platform))
					{
						isXpIe6Japanese = true;
					}
				}
			}

			return isXpIe6Japanese;
		}

		private static bool BrowserIsIe(string browserString)
		{
			return browserString.StartsWith("IE", StringComparison.OrdinalIgnoreCase);
		}

		private static bool ClientIsWinXp(string platform)
		{
			return (string.Compare(platform, "WinXP", StringComparison.OrdinalIgnoreCase) == 0);
		}

		private static string GetDownloadFilename(FileHeadObject fileHeadObject, string userLanguage, bool requestIsFromXPIE6Japanese)
		{
			bool isUsePlaceholder = false;
			string origFileName = fileHeadObject.FileName;
			string fileName;
			// if the request from WINXP IE6 and Japanese
			if (requestIsFromXPIE6Japanese)
			{
				fileName = BuildDownloadNameForXPIE6Japanese(fileHeadObject);
			}
			else
			{
				if (StringContainsHighAscii(origFileName))
				{
					isUsePlaceholder = true;
					if (userLanguage.StartsWith("en", StringComparison.OrdinalIgnoreCase))
						isUsePlaceholder = false;
				}

				string nameToUse = origFileName;
				if (isUsePlaceholder)
				{
					string extension = fileHeadObject.FileExtensionName;
					nameToUse = GetPlaceholderFileName(fileHeadObject) + extension;
				}

				fileName = HttpUtility.UrlPathEncode(nameToUse);
				fileName = fileName.Replace("#", "%23");
			}

			return fileName;
		}

		/// <summary>
		/// Builds the optimum file name that can be displayed without exceeeding
		/// header limit.
		/// </summary>
		/// <param name="fileHeadObject">original file head</param>
		/// <returns></returns>
		private static string BuildDownloadNameForXPIE6Japanese(FileHeadObject fileHeadObject)
		{
			//To DO Krishna: We can change this logic to use ability of UTF8 encoded byte  to determine the truncation point.
			string nameToUse = fileHeadObject.FileName;
			string ext = fileHeadObject.FileExtensionName;
			if (ext.Length > 0)
				nameToUse = nameToUse.Substring(0, nameToUse.Length - ext.Length);

			char[] charArr = nameToUse.ToCharArray();
			string holderStr;
			StringBuilder fileName = new StringBuilder();
			int nCurrentLength = 0;

			foreach (char c in charArr)
			{

				holderStr = HttpUtility.UrlEncode(c.ToString());
				if ((holderStr.Length + nCurrentLength) > 155)
					break;

				nCurrentLength += holderStr.Length;
				fileName.Append(holderStr);
				holderStr = null;
			}

			if (ext.Length > 0)
				fileName.Append(ext);

			return fileName.ToString();
		}

		private static bool StringContainsHighAscii(string inputValue)
		{
			for (int i = 0; i < inputValue.Length; ++i)
			{
				if (inputValue[i] > 0x7f)
					return true;
			}
			return false;
		}

		/// <summary>
		/// If the filename contains high ASCII and client is non-English, the filename to use (w/o extension).
		/// </summary>
		/// <param name="fileHeadObject"></param>
		private static string GetPlaceholderFileName(FileHeadObject fileHeadObject)
		{
			return string.Format(CultureInfo.InvariantCulture, "Download_{0}", fileHeadObject.Id.ToString());
		}

		private static string TruncateHeaderLine(FileHeadObject fileHeadObject, string text, int firstLineLen)
		{
			int maxLen = firstLineLen;
			int pos = 0;
			if (pos + maxLen < text.Length)
			{
				int lastByteCount = 0;
				while (pos < maxLen)
				{
					if (text[pos] == '%')  // we don't want a filename to end with '%'.
						lastByteCount = 3;
					else
						lastByteCount = 1;
					pos += lastByteCount;
				}

				pos = pos - lastByteCount;
				string ext = fileHeadObject.FileExtensionName;
				text = text.Substring(0, pos);
				int dotPos = text.LastIndexOf(".", StringComparison.Ordinal);
				if (dotPos > 0)
					text = text.Substring(0, dotPos);

				text += ext;
			}

			return text;
		}
	}
}