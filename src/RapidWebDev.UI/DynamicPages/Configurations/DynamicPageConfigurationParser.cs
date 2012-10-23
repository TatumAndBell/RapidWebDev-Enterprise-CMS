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
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
    /// <summary>
    /// Configuration parser for dynamic page controls.
    /// </summary>
    public class DynamicPageConfigurationParser : ConfigurationParser
    {
        private static object syncObject = new object();
		private XmlParser xmlParser;

		/// <summary>
		/// Get xml parser instance.
		/// </summary>
		protected override XmlParser XmlParser
		{
			get
			{
				if (this.xmlParser == null)
				{
					lock (syncObject)
					{
						if (this.xmlParser == null)
						{
							XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
							xmlNamespaceManager.AddNamespace("p", "http://www.rapidwebdev.org/schemas/dynamicpage");

							string schemaXml = Kit.GetManifestFile(typeof(DynamicPageConfigurationParser), "DynamicPage.xsd");
							using (StringReader sr = new StringReader(schemaXml))
							{
								this.xmlParser = new XmlParser(xmlNamespaceManager, XmlSchema.Read(sr, null));
							}						
						}
					}
				}

				return this.xmlParser;
			}
		}

        /// <summary>
        /// Construct ConfigurationParser instance.
        /// </summary>
        /// <param name="directories">configuration xml directories contained xml files, which will be parsed into dynamic page objects.</param>
        /// <param name="files">configuration xml files to be parsed into dynamic page objects.</param>
        public DynamicPageConfigurationParser(IEnumerable<string> directories, IEnumerable<string> files) : base(directories, files)
        {
        }

        /// <summary>
		/// Create object by specified xml doc and returns created key-value pair (object id and dynamic page instance).
        /// </summary>
		/// <param name="xmldoc"></param>
		protected override KeyValuePair<string, object> CreateConfigurationObject(XmlDocument xmldoc)
        {
			XmlElement pageElement = xmldoc.SelectSingleNode("p:Page", this.XmlParser.NamespaceManager) as XmlElement;
			DynamicPageConfiguration config = new DynamicPageConfiguration(pageElement, this.XmlParser);
			return new KeyValuePair<string, object>(config.ObjectId, config);
        }
    }
}

