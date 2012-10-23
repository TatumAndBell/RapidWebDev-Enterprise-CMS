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
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Configuration class maps to DynamicDataSourceConfiguration element.
	/// </summary>
	public sealed class ComboBoxDynamicDataSourceConfiguration
	{
		/// <summary>
		/// The URL from which to load data through an HttpProxy.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// The http access method, GET/POST, defaults to GET.
		/// </summary>
		public HttpMethods Method { get; set; }

		/// <summary>
		/// Note that if you are retrieving data from a page that is in a domain that is NOT the same as the originating domain of the running page, you must use ScriptTagProxy rather than HttpProxy.
		/// The content passed back from a server resource requested by a ScriptTagProxy must be executable JavaScript source code because it is used as the source inside a script tag.
		/// If you choose ScriptTagProxy, the http server MUST wrap the returned JSON string within "(JSON String);".
		/// Defaults to HttpProxy.
		/// </summary>
		public DataProxyTypes Proxy { get; set; }

		/// <summary>
		/// The name of the property which contains the Array of row objects in remote call result.
		/// </summary>
		public string Root { get; set; }

		/// <summary>
		/// The underlying field name of displaying text to bind to this ComboBox.
		/// </summary>
		public string TextField { get; set; }

		/// <summary>
		/// The underlying field name of selection value to bind to this ComboBox.
		/// </summary>
		public string ValueField { get; set; }

		/// <summary>
		/// Name of the query as it will be passed on the querystring (defaults to 'query')
		/// </summary>
		public string QueryParam { get; set; }

		/// <summary>
		/// The template string used to display each item in the dropdown list. Use this to create custom UI layouts for items in the list.
		/// </summary>
		public string XTemplate { get; set; }

		/// <summary>
		/// This setting is required if a custom XTemplate has been specified which assigns a class other than ".x-combo-list-item" to dropdown list items.
		/// </summary>
		public string ItemSelector { get; set; }

		/// <summary>
		/// Gets collection of http request parameters.
		/// </summary>
		public Collection<ComboBoxDynamicDataSourceParamConfiguration> Params { get; set; }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public ComboBoxDynamicDataSourceConfiguration()
		{
			this.QueryParam = "query";
			this.Params = new Collection<ComboBoxDynamicDataSourceParamConfiguration>();
		}

		/// <summary>
		/// Construct DynamicDataSourceConfiguration instance from xml element.
		/// </summary>
		/// <param name="remoteDataSourceElement"></param>
		/// <param name="xmlParser"></param>
		public ComboBoxDynamicDataSourceConfiguration(XmlElement remoteDataSourceElement, XmlParser xmlParser)
		{
			this.Url = xmlParser.ParseString(remoteDataSourceElement, "@Url");
			this.Root = xmlParser.ParseString(remoteDataSourceElement, "@Root");
			this.TextField = xmlParser.ParseString(remoteDataSourceElement, "@TextField");
			this.ValueField = xmlParser.ParseString(remoteDataSourceElement, "@ValueField");
			this.QueryParam = xmlParser.ParseString(remoteDataSourceElement, "@QueryParam");
			this.Method = xmlParser.ParseEnum<HttpMethods>(remoteDataSourceElement, "@Method");
			this.Proxy = xmlParser.ParseEnum<DataProxyTypes>(remoteDataSourceElement, "@Proxy");
			if (string.IsNullOrEmpty(this.QueryParam))
				this.QueryParam = "query";

			this.Params = new Collection<ComboBoxDynamicDataSourceParamConfiguration>();
			XmlNodeList paramNodes = remoteDataSourceElement.SelectNodes("p:Param", xmlParser.NamespaceManager);
			foreach (XmlNode paramNode in paramNodes)
				this.Params.Add(new ComboBoxDynamicDataSourceParamConfiguration(paramNode as XmlElement, xmlParser));

			XmlElement xtemplateElement = remoteDataSourceElement.SelectSingleNode("p:XTemplate", xmlParser.NamespaceManager) as XmlElement;
			if (xtemplateElement != null)
			{
				this.XTemplate = xtemplateElement.InnerXml;
				this.ItemSelector = xmlParser.ParseString(xtemplateElement, "@ItemSelector");
			}
		}
	}

	/// <summary>
	/// Configuration class maps to ComboBoxDynamicDataSourceParamConfiguration element.
	/// </summary>
	public class ComboBoxDynamicDataSourceParamConfiguration
	{
		private string value;

		/// <summary>
		/// Gets parameter name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets parameter value.
		/// </summary>
		public string Value 
		{
			get { return WebUtility.ReplaceVariables(this.value); }
			set { this.value = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public ComboBoxDynamicDataSourceParamConfiguration(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Construct ComboBoxDynamicDataSourceParamConfiguration instance from xml element.
		/// </summary>
		/// <param name="nameValueConfigurationElement"></param>
		/// <param name="xmlParser"></param>
		public ComboBoxDynamicDataSourceParamConfiguration(XmlElement nameValueConfigurationElement, XmlParser xmlParser)
		{
			this.Name = xmlParser.ParseString(nameValueConfigurationElement, "@Name");
			this.Value = xmlParser.ParseString(nameValueConfigurationElement, "@Value");
		}
	}

	/// <summary>
	/// HTTP access methods.
	/// </summary>
	public enum HttpMethods
	{
		/// <summary>
		/// GET
		/// </summary>
		GET, 

		/// <summary>
		/// POST
		/// </summary>
		POST
	}

	/// <summary>
	/// Ajax data accessing proxy types.
	/// </summary>
	public enum DataProxyTypes
	{
		/// <summary>
		/// Use http proxy to access the resources in the same domain to the web page.
		/// </summary>
		HttpProxy,

		/// <summary>
		/// Use script tag proxy to access the resources in the different domain to the web page.
		/// </summary>
		ScriptTagProxy
	}
}

