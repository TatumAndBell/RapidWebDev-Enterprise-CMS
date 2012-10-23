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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.Platform.Initialization;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Api to read permission configuration.
	/// </summary>
	public class XmlFilePermissionConfigurationReader : IPermissionConfigurationReader
	{
		private string permissionFilePath;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="permissionFilePath"></param>
		public XmlFilePermissionConfigurationReader(string permissionFilePath)
		{
			this.permissionFilePath = permissionFilePath;
		}

		/// <summary>
		/// Read permission configuration document.
		/// </summary>
		/// <returns></returns>
		public XmlDocument Read()
		{
			string permissionFilePath = Kit.ToAbsolutePath(this.permissionFilePath);
			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load(permissionFilePath);
			string schemaXml = Kit.GetManifestFile(typeof(IPermissionConfigurationReader), "PermissionConfig.xsd");
			XmlSchema schema = XmlSchema.Read(new StringReader(schemaXml), null);
			Kit.ValidateXml(schema, xmldoc);

			return xmldoc;
		}
	}
}
