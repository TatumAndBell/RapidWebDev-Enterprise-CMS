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
using System.Linq;
using System.Text;
using System.Web;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using Spring.Core.IO;

namespace RapidWebDev.UI.WebResources
{
	/// <summary>
	/// The implements creates composite resource file from not-independent resources and flush into clients. 
	/// And flush independent resources into clients directly.
	/// The flushing sequence of resources completely follows the resource configured.
	/// </summary>
	/// <example>
	/// There are 7 resource file configured as A, B, C, D, E, F, G. The C and F are independent resources.
	/// This implementation merages A and B into a new resource flushing into client as D and E. 
	/// So finally, there will be 5 resources registered to clients as (A, B), C, (D, E), F, (G).
	/// </example>
	public sealed class DefaultWebResourceManager : IWebResourceManager
	{
		private const string CssResourceUriTemplate = "{0}{1}-{2}.css";
		private const string JavaScriptResourceUriTemplate = "{0}{1}-{2}.js";
		private static object syncObject = new object();
		private static bool hasStartedUp = false;
		private static readonly long versionNumber = DateTime.Now.Ticks;

		/// <summary>
		/// Gets/Sets temporary resource file storing directory.
		/// </summary>
		public string TemporaryFileStorageDirectory { get; set; }

		/// <summary>
		/// Gets/Sets temporary resource file storing directory.
		/// </summary>
		public string TemporaryFileVirtualDirectory { get; set; }

		/// <summary>
		/// Render the resources in the container by id with filters.
		/// </summary>
		/// <param name="resourceId"></param>
		/// <param name="filters"></param>
		public void Flush(string resourceId, params KeyValuePair<string, string>[] filters)
		{
			if (string.IsNullOrEmpty(resourceId))
				throw new ArgumentNullException("The specified \"resourceId\" is invalid.", "resourceId");

			WebResourceGroup webResourceGroup = SpringContext.Current.GetObject<WebResourceGroup>(resourceId);
			if (webResourceGroup == null)
				throw new ArgumentNullException(string.Format(CultureInfo.InvariantCulture, "The WebResourceGroup with specified resourceId \"{0}\" is not found in Spring.NET IoC.", resourceId), "resourceId");
			if (webResourceGroup.Resources == null)
				throw new ArgumentNullException(string.Format(CultureInfo.InvariantCulture, "The property Resources of WebResourceGroup with specified resourceId \"{0}\" is not configured in Spring.NET IoC.", resourceId), "resourceId");

			this.Flush(resourceId, WebResourceType.Style, webResourceGroup);
			this.Flush(resourceId, WebResourceType.JavaScript, webResourceGroup);
		}

		private void Flush(string resourceId, WebResourceType resourceType, WebResourceGroup webResourceGroup)
		{
			IEnumerable<WebResource> webResources = webResourceGroup.Resources.Where(resx => resx.Type == resourceType);

			Action<string> FlushResourceToClient = null;
			if (resourceType == WebResourceType.JavaScript)
				FlushResourceToClient = new Action<string>(ClientScripts.RegisterHeaderScriptInclude);
			else if(resourceType == WebResourceType.Style)
				FlushResourceToClient = new Action<string>(ClientScripts.RegisterHeaderStyleInclude);

			IEnumerable<WebResource> resxs = webResources.Where(resx => resx.Type == resourceType);
			List<WebResource> webResourcesToMerge = new List<WebResource>();
			int resourceIndex = 0;
			string resourceUri = null;
			foreach (WebResource resx in resxs)
			{
				if (string.IsNullOrEmpty(resx.CultureName) || (CultureInfo.CurrentUICulture != null && string.Equals(CultureInfo.CurrentUICulture.Name, resx.CultureName, StringComparison.InvariantCultureIgnoreCase)))
				{
					if (resx.Independent)
					{
						if (webResourcesToMerge.Count > 0)
						{
							resourceUri = MergeResourceAndReturnUri(resourceId, resourceType, resourceIndex++, webResourcesToMerge);
							FlushResourceToClient(ResolveAbsoluteUrl(resourceUri));
							webResourcesToMerge.Clear();
						}

						FlushResourceToClient(ResolveAbsoluteUrl(resx.Uri));
					}
					else
						webResourcesToMerge.Add(resx);
				}
			}

			if (webResourcesToMerge.Count > 0)
			{
				resourceUri = MergeResourceAndReturnUri(resourceId, resourceType, resourceIndex++, webResourcesToMerge);
				FlushResourceToClient(ResolveAbsoluteUrl(resourceUri));
				webResourcesToMerge.Clear();
			}
		}

		private string MergeResourceAndReturnUri(string resourceId, WebResourceType resourceType, int resourceIndex, IEnumerable<WebResource> webResources)
		{
			string temporaryResourceDirectoryPath = HttpContext.Current.Server.MapPath(this.TemporaryFileStorageDirectory);
			DeleteTemporaryResourceFilesWhenStartUp(temporaryResourceDirectoryPath);

			string resourceFileName = ResolveTemporaryResourceFileName(resourceId, resourceType, resourceIndex);
			string resourceFilePath = Path.Combine(temporaryResourceDirectoryPath, resourceFileName);
			if (!File.Exists(resourceFilePath))
			{
				using (FileStream fileStream = File.Create(resourceFilePath, 4096))
				using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8, 4096))
				{
					string resourceBody = this.GetResourceBody(webResources);
					streamWriter.Write(resourceBody);
				}
			}

			return VirtualPathUtility.Combine(this.TemporaryFileVirtualDirectory, resourceFileName);
		}

		private string GetResourceBody(IEnumerable<WebResource> webResources)
		{
			IEnumerable<string> resourcePaths = webResources.Select(resx => resx.Uri);
			StringBuilder resultBuilder = new StringBuilder();
			foreach (string resourcePath in resourcePaths)
			{
				IResource resource = SpringContext.Current.GetResource(resourcePath);
				if (resource.Exists)
				{
					Stream inputStream = null;
					if (!resource.IsOpen)
						inputStream = resource.File.Open(FileMode.Open, FileAccess.Read);

					using (StreamReader streamReader = new StreamReader(inputStream, true))
					{
						resultBuilder.Append("\r\n");
						resultBuilder.Append(streamReader.ReadToEnd());
					}
				}
			}

			return resultBuilder.ToString();
		}

		private static void DeleteTemporaryResourceFilesWhenStartUp(string temporaryResourceDirectory)
		{
			if (!hasStartedUp)
			{
				lock (syncObject)
				{
					if (!hasStartedUp)
					{
						try
						{
							if (Directory.Exists(temporaryResourceDirectory))
							{
								string[] temporaryFiles = Directory.GetFiles(temporaryResourceDirectory);
								foreach (string temporaryFile in temporaryFiles)
									File.Delete(temporaryFile);
							}
						}
						catch (UnauthorizedAccessException)
						{
							throw;
						}
						catch
						{
						}

						if (!Directory.Exists(temporaryResourceDirectory))
							Directory.CreateDirectory(temporaryResourceDirectory);

						hasStartedUp = true;
					}
				}
			}
		}

		private static string ResolveTemporaryResourceFileName(string resourceId, WebResourceType resourceType, int resourceIndex)
		{
			switch (resourceType)
			{
				case WebResourceType.JavaScript:
					return string.Format(CultureInfo.InvariantCulture, JavaScriptResourceUriTemplate, resourceId, versionNumber, resourceIndex);

				case WebResourceType.Style:
					return string.Format(CultureInfo.InvariantCulture, CssResourceUriTemplate, resourceId, versionNumber, resourceIndex);
			}

			return "";
		}

		private static string ResolveAbsoluteUrl(string resourceUrl)
		{
			if (VirtualPathUtility.IsAbsolute(resourceUrl)) return resourceUrl;
			if (VirtualPathUtility.IsAppRelative(resourceUrl)) return VirtualPathUtility.ToAbsolute(resourceUrl);

			return resourceUrl;
		}
	}
}
