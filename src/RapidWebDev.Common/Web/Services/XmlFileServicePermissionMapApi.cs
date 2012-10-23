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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace RapidWebDev.Common.Web.Services
{
	/// <summary>
	/// Api to resolve the permission value for a WCF operation contract.
	/// </summary>
	public class XmlFileServicePermissionMapApi : IServicePermissionMapApi
	{
		private string docLevelPermissionValue;
		private Dictionary<string, string> serviceContractLevelPermissionValues = new Dictionary<string, string>();
		private Dictionary<string, Dictionary<string, string>> operationContractPermissionValues = new Dictionary<string, Dictionary<string, string>>();

		/// <summary>
		/// Gets xml file path.
		/// </summary>
		public string XmlFilePath { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="xmlFilePath"></param>
		public XmlFileServicePermissionMapApi(string xmlFilePath)
		{
			this.XmlFilePath = xmlFilePath;
			this.LoadRestfulServicePermissionConfiguration();
		}

		/// <summary>
		/// Get permission value for the operation contract of service contract.
		/// </summary>
		/// <returns></returns>
		public string GetPermissionValue(string serviceContractName, string operationContractName)
		{
			if (this.operationContractPermissionValues.ContainsKey(serviceContractName))
			{
				if (this.operationContractPermissionValues[serviceContractName].ContainsKey(operationContractName))
					return this.operationContractPermissionValues[serviceContractName][operationContractName];
			}

			if (this.serviceContractLevelPermissionValues.ContainsKey(serviceContractName))
				return this.serviceContractLevelPermissionValues[serviceContractName];

			return docLevelPermissionValue;
		}

		private void LoadRestfulServicePermissionConfiguration()
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			xmlNamespaceManager.AddNamespace("p", "http://www.rapidwebdev.org/schemas/restfulservicepermissions");

			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load(Kit.ToAbsolutePath(this.XmlFilePath));

			string schemaXml = Kit.GetManifestFile(typeof(XmlFileServicePermissionMapApi), "RestfulServicePermissionSchema.xsd");
			using (StringReader reader = new StringReader(schemaXml))
			{
				XmlSchema schema = XmlSchema.Read(reader, null);
				Kit.ValidateXml(schema, xmldoc);
			}

			XmlAttribute docLevelPermissionValueAttribute = xmldoc.DocumentElement.Attributes["DefaultPermissionValue"];
			this.docLevelPermissionValue = docLevelPermissionValueAttribute.Value;

			XmlNodeList serviceContractNodeList = xmldoc.SelectNodes("//p:ServiceContract", xmlNamespaceManager);
			foreach (XmlNode serviceContractNode in serviceContractNodeList)
			{
				string serviceContractName = serviceContractNode.Attributes["Name"].Value;
				XmlAttribute serviceLevelPermissionValueAttribute = serviceContractNode.Attributes["DefaultPermissionValue"];
				string serviceLevelPermissionValue = this.docLevelPermissionValue;
				if (serviceLevelPermissionValueAttribute != null)
					serviceLevelPermissionValue = serviceLevelPermissionValueAttribute.Value;

				this.serviceContractLevelPermissionValues[serviceContractName] = serviceLevelPermissionValue;
				XmlNodeList operationContractNodeList = serviceContractNode.SelectNodes("//p:OperationContract", xmlNamespaceManager);
				foreach (XmlNode operationContractNode in operationContractNodeList)
				{
					string operationContractName = operationContractNode.Attributes["Name"].Value;
					string operationContractPermission = operationContractNode.Attributes["PermissionValue"].Value;
					if (!this.operationContractPermissionValues.ContainsKey(serviceContractName))
						this.operationContractPermissionValues[serviceContractName] = new Dictionary<string, string>();

					this.operationContractPermissionValues[serviceContractName][operationContractName] = operationContractPermission;
				}
			}
		}
	}
}
