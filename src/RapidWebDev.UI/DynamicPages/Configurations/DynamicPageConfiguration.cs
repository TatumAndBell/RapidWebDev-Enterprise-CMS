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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Dynamic page configuration class.
	/// </summary>
	public sealed class DynamicPageConfiguration : ICloneable
	{
		private XmlElement pageElement;
		private XmlParser xmlParser;

		private string title;
		private string permissionValue;

		/// <summary>
		/// Sets/gets dynamic web page title.
		/// The title supports globalization variable as "$Namespace.ClassName.PropertyName, AssemblyName$" or http context variables as "$VariableName$" (included in IApplicationContext.LabelVariables["VariableName"]).
		/// </summary>
		public string Title 
		{
			set { this.title = value; }
			get { return WebUtility.ReplaceVariables(this.title); } 
		}

		/// <summary>
		/// Sets/gets permission value of dynamic web page.
		/// </summary>
		public string PermissionValue
		{
			set { this.permissionValue = value; }
			get { return WebUtility.ReplaceVariables(this.permissionValue); }
		}

		/// <summary>
		/// Configure object identifier of the web page. It will be looked up by query string parameter ObjectId.
		/// </summary>
		public string ObjectId { get; set; }

		/// <summary>
		/// The C# managed type of dynamic page which implements IDynamicPage.
		/// </summary>
		public Type DynamicPageType { get; set; }

		/// <summary>
		/// The callback is used to dynamic process the panel configuration at runtime. 
		/// </summary>
		public IDynamicPageConfigurationProcessCallback ConfigurationProcessCallback { get; set; }

		/// <summary>
		/// Gets collection of dynamic page panels.
		/// </summary>
		public Collection<BasePanelConfiguration> Panels { get; private set; }

		/// <summary>
		/// The external JavaScript references needed to be registered into web page when the page is loading.
		/// </summary>
		public Collection<string> JavaScriptUrls { get; private set; }

		/// <summary>
		/// Construct page configuration
		/// </summary>
		public DynamicPageConfiguration() 
		{
			this.Panels = new Collection<BasePanelConfiguration>();
			this.JavaScriptUrls = new Collection<string>();
		}

		/// <summary>
		/// Construct dynamic page instance by xml element.
		/// </summary>
		/// <param name="pageElement"></param>
		/// <param name="xmlParser"></param>
		public DynamicPageConfiguration(XmlElement pageElement, XmlParser xmlParser) : this()
		{
			this.pageElement = pageElement;
			this.xmlParser = xmlParser;

			this.Title = xmlParser.ParseString(pageElement, "p:Title");
			this.PermissionValue = xmlParser.ParseString(pageElement, "p:PermissionValue");
			this.ObjectId = xmlParser.ParseString(pageElement, "@ObjectId");

			string dynamicPageTypeName = xmlParser.ParseString(pageElement, "@Type");
			Type dynamicPageType = Kit.GetType(dynamicPageTypeName);
			if (dynamicPageType != null)
			{
				ConstructorInfo dynamicPageObjectConstructor = dynamicPageType.GetConstructor(Type.EmptyTypes);
				if (!(dynamicPageObjectConstructor.Invoke(null) is IDynamicPage))
					throw new ConfigurationErrorsException(string.Format(Resources.DP_ConfiguredDynamicPageTypeNotImplementIDynamicPage, dynamicPageTypeName, this.ObjectId));

				this.DynamicPageType = dynamicPageType;
			}
			else
				throw new ConfigurationErrorsException(string.Format(Resources.DP_ConfiguredDynamicPageTypeNotExists, dynamicPageTypeName, this.ObjectId));

			string callbackTypeName = xmlParser.ParseString(pageElement, "@ProcessCallbackType");
			if (!string.IsNullOrEmpty(callbackTypeName))
			{
				Type callbackType = Kit.GetType(callbackTypeName);
				if (callbackType == null)
					throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, "The callback type @ProcessCallbackType:{0} is not found.", callbackTypeName));

				this.ConfigurationProcessCallback = Activator.CreateInstance(callbackType) as IDynamicPageConfigurationProcessCallback;
				if (this.ConfigurationProcessCallback == null)
					throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, "The callback type @ProcessCallbackType:{0} doesn't implement the interface IDynamicPageConfigurationProcessCallback.", callbackTypeName));
			}

			this.JavaScriptUrls = new Collection<string>();
			XmlNodeList javaScriptUrlNodes = this.pageElement.SelectNodes("p:JavaScriptUrls/p:JavaScriptUrl", xmlParser.NamespaceManager);
			foreach (XmlNode javaScriptUrlNode in javaScriptUrlNodes)
			{
				XmlElement javaScriptUrlElement = javaScriptUrlNode as XmlElement;
				if (javaScriptUrlElement != null)
					this.JavaScriptUrls.Add(javaScriptUrlElement.InnerText);
			}

			bool hasQueryPanel = false, hasGridViewPanel = false, hasDetailPanel = false;
			HashSet<string> aggregatePanelCommandArguments = new HashSet<string>();

			// create panel configurations
			XmlElement panelElement = pageElement.SelectSingleNode("p:Panels", xmlParser.NamespaceManager) as XmlElement;
			foreach (XmlNode childNode in panelElement.ChildNodes)
			{
				XmlElement childElement = childNode as XmlElement;
				if (childElement != null)
				{
					DynamicPagePanelTypes panelType = (DynamicPagePanelTypes)Enum.Parse(typeof(DynamicPagePanelTypes), childElement.LocalName);
					BasePanelConfiguration basePanelConfiguration = null;
					switch (panelType)
					{
						case DynamicPagePanelTypes.QueryPanel:
							if (hasQueryPanel)
								throw new ConfigurationErrorsException(Resources.DP_MultipleQueryPanelConfigured);

							hasQueryPanel = true;
							basePanelConfiguration = new QueryPanelConfiguration(childElement, xmlParser);
							break;

						case DynamicPagePanelTypes.GridViewPanel:
							if (hasGridViewPanel)
								throw new ConfigurationErrorsException(Resources.DP_MultipleGridViewPanelConfigured);

							hasGridViewPanel = true;
							basePanelConfiguration = new GridViewPanelConfiguration(childElement, xmlParser);
							break;

						case DynamicPagePanelTypes.DetailPanel:
							if (hasDetailPanel)
								throw new ConfigurationErrorsException(Resources.DP_MultipleDetailPanelConfigured);

							hasDetailPanel = true;
							DetailPanelConfiguration detailPanel = new DetailPanelConfiguration(childElement, xmlParser);
							basePanelConfiguration = detailPanel;
							break;

						case DynamicPagePanelTypes.AggregatePanel:
							AggregatePanelConfiguration aggregatePanel = new AggregatePanelConfiguration(childElement, xmlParser);
							string lowerCasedCommandArgument = aggregatePanel.CommandArgument.ToLower(CultureInfo.InvariantCulture);
							if (aggregatePanelCommandArguments.Contains(lowerCasedCommandArgument))
								throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, Resources.DP_DuplicateAggregatePanelCommandArgumentConfigured, aggregatePanel.CommandArgument));

							aggregatePanelCommandArguments.Add(lowerCasedCommandArgument);
							basePanelConfiguration = aggregatePanel;
							break;

						case DynamicPagePanelTypes.ButtonPanel:
							basePanelConfiguration = new ButtonPanelConfiguration(childElement, xmlParser);
							break;
					}

					this.Panels.Add(basePanelConfiguration);
				}
			}
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a shallow copy of the current instance.
		/// </summary>
		/// <returns></returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Creates a new object that is a shallow copy of the current instance.
		/// The method is pre-designed for deep copy in the future.
		/// </summary>
		/// <returns></returns>
		public DynamicPageConfiguration Clone()
		{
			if (this.pageElement != null && this.xmlParser != null)
				return new DynamicPageConfiguration(this.pageElement, this.xmlParser);

			return this;
		}

		#endregion
	}
}
