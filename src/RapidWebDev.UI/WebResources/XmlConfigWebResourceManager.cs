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
using System.Web.UI;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using Spring.Core.IO;

namespace RapidWebDev.UI.WebResources
{
	/// <summary>
	/// The manager is to generate a Uri returned for clients which contains the configured style and JavaScript resources mapping to specified resource id. 
	/// </summary>
	public class XmlConfigWebResourceManager : IWebResourceManager
	{
		private string xmlConfigDirectoryPath;
		private XmlNamespaceManager xmlNamespaceManager;
		private Dictionary<string, XmlDocument> xmlDocsByResourceId = new Dictionary<string, XmlDocument>();
		private Dictionary<string, IEnumerable<XmlElement>> templateElementsById = new Dictionary<string, IEnumerable<XmlElement>>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="xmlConfigDirectoryPath">The directory stores web resource xml configuration files.</param>
		public XmlConfigWebResourceManager(string xmlConfigDirectoryPath)
		{
			this.xmlConfigDirectoryPath = Kit.ToAbsolutePath(xmlConfigDirectoryPath);

			this.ValidateConfigurationFiles();
			this.ResolveTemplateElements();
		}

		private XmlNamespaceManager NamespaceManager
		{
			get
			{
				if (this.xmlNamespaceManager == null)
				{
					this.xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
					this.xmlNamespaceManager.AddNamespace("p", "http://www.rapidwebdev.org/schemas/ui/resources");
				}

				return this.xmlNamespaceManager;
			}
		}

		/// <summary>
		/// Render the resources in the container by id with filters.
		/// </summary>
		/// <param name="resourceId"></param>
		/// <param name="filters"></param>
		public virtual void Flush(string resourceId, params KeyValuePair<string, string>[] filters)
		{
			if (!this.xmlDocsByResourceId.ContainsKey(resourceId)) return;

			Dictionary<string, string> filtersDictionary = null;
			if (filters == null || filters.Length == 0)
				filtersDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			else
				filtersDictionary = filters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

			XmlDocument xmldoc = this.xmlDocsByResourceId[resourceId];
			XmlElement styleElement = xmldoc.SelectSingleNode("p:Resources/p:Style", this.NamespaceManager) as XmlElement;
			this.FlushInternal(styleElement, filtersDictionary);

			XmlElement scriptElement = xmldoc.SelectSingleNode("p:Resources/p:Script", this.NamespaceManager) as XmlElement;
			this.FlushInternal(scriptElement, filtersDictionary);
		}

		private void FlushInternal(XmlElement containerElement, IDictionary<string, string> filters)
		{
			if (containerElement == null) return;

			Action<string> flushResourceToClientDelegate = null;
			if (containerElement.LocalName == "Script")
				flushResourceToClientDelegate = new Action<string>(ClientScripts.RegisterHeaderScriptInclude);
			else if (containerElement.LocalName == "Style")
				flushResourceToClientDelegate = new Action<string>(ClientScripts.RegisterHeaderStyleInclude);

			foreach (XmlNode childNode in containerElement.ChildNodes)
			{
				XmlElement resourceItemElement = childNode as XmlElement;
				if (resourceItemElement == null) continue;
				FlushResourceToClient(resourceItemElement, filters, flushResourceToClientDelegate);
			}
		}

		private void ValidateConfigurationFiles()
		{
			if (!Directory.Exists(this.xmlConfigDirectoryPath)) return;

			string[] xmlConfigFilePathArray = Directory.GetFiles(this.xmlConfigDirectoryPath, "*.xml", SearchOption.TopDirectoryOnly);
			string schemaXml = Kit.GetManifestFile(typeof(XmlConfigWebResourceManager), "WebResourceConfiguration.xsd");
			using (StringReader stringReader = new StringReader(schemaXml))
			{
				XmlSchema schema = XmlSchema.Read(stringReader, null);
				foreach (string xmlConfigFilePath in xmlConfigFilePathArray)
				{
					try
					{
						XmlDocument xmldoc = new XmlDocument();
						xmldoc.Load(xmlConfigFilePath);
						Kit.ValidateXml(schema, xmldoc);

						string resourceId = Path.GetFileNameWithoutExtension(xmlConfigFilePath);
						this.xmlDocsByResourceId[resourceId] = xmldoc;
					}
					catch
					{
						continue;
					}
				}
			}
		}

		private void ResolveTemplateElements()
		{
			foreach (XmlDocument xmldoc in this.xmlDocsByResourceId.Values)
			{
				XmlNodeList templateNodeList = xmldoc.SelectNodes("//p:Template", this.NamespaceManager);
				foreach (XmlElement templateElement in templateNodeList)
				{
					string templateId = templateElement.Attributes["Id"].Value;
					List<XmlElement> resourceElements = new List<XmlElement>();
					this.templateElementsById[templateId] = resourceElements;

					XmlNodeList resourceNodeList = templateElement.SelectNodes("p:Resource", this.NamespaceManager);
					foreach (XmlElement resourceElement in resourceNodeList)
						resourceElements.Add(resourceElement);
				}
			}
		}

		private bool ShouldResourceBeRendered(XmlElement resourceItemElement, IDictionary<string, string> filters)
		{
			bool isImportTemplateElement = string.Equals(resourceItemElement.LocalName, "ImportTemplate");
			if (isImportTemplateElement) return true;

			bool isResourceElement = string.Equals(resourceItemElement.LocalName, "Resource");
			foreach (XmlAttribute attribute in resourceItemElement.Attributes)
			{
				if (isResourceElement && string.Equals(attribute.LocalName, "Path", StringComparison.Ordinal)) continue;
				if (!filters.ContainsKey(attribute.LocalName)) return false;
				if (!string.Equals(filters[attribute.LocalName], attribute.Value, StringComparison.OrdinalIgnoreCase)) return false;
			}

			return true;
		}

		private void FlushResourceToClient(XmlElement resourceItemElement, IDictionary<string, string> filters, Action<string> flushResourceToClientDelegate)
		{
			if (!ShouldResourceBeRendered(resourceItemElement, filters)) return;

			if (string.Equals(resourceItemElement.LocalName, "Resource"))
			{
				string path = resourceItemElement.Attributes["Path"].Value;
				flushResourceToClientDelegate(Kit.ResolveAbsoluteUrl(path));
			}
			else if (string.Equals(resourceItemElement.LocalName, "ResourceGroup"))
			{
				XmlNodeList resourceNodeList = resourceItemElement.SelectNodes("p:Resource", this.NamespaceManager);
				foreach (XmlNode resourceNode in resourceNodeList)
				{
					XmlElement resourceElement = resourceNode as XmlElement;
					FlushResourceToClient(resourceElement, filters, flushResourceToClientDelegate);
				}
			}
			else if (string.Equals(resourceItemElement.LocalName, "ImportTemplate"))
			{
				string templateId = resourceItemElement.Attributes["TemplateId"].Value;
				if (!this.templateElementsById.ContainsKey(templateId)) return;

				IEnumerable<XmlElement> includedResourceElements = this.templateElementsById[templateId];
				foreach(XmlElement includedResourceElement in includedResourceElements)
					FlushResourceToClient(includedResourceElement, filters, flushResourceToClientDelegate);
			}
		}
	}
}