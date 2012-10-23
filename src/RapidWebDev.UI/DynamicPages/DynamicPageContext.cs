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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Dynamic page context.
	/// </summary>
	public sealed class DynamicPageContext
	{
		private static DynamicPageContext singletonContext;
		private Dictionary<object, object> contextVariables = new Dictionary<object, object>();

		private IDictionary<object, object> ContextVariables
		{
			get { return this.contextVariables; }
		}

		/// <summary>
		/// Private constructor
		/// </summary>
		private DynamicPageContext()
		{
		}

		/// <summary>
		/// Get current context on processing dynamic page.
		/// </summary>
		public static DynamicPageContext Current
		{
			get
			{
				const string contextKey = "RapidWebDev.UI.DynamicPages.DynamicPageContext::Current";
				if (HttpContext.Current == null)
				{
					if (singletonContext == null)
						singletonContext = new DynamicPageContext();

					return singletonContext;
				}
				else
				{
					if (!HttpContext.Current.Items.Contains(contextKey))
						HttpContext.Current.Items.Add(contextKey, new DynamicPageContext());

					return HttpContext.Current.Items[contextKey] as DynamicPageContext;
				}
			}
		}

		/// <summary>
		/// Resolve IDynamicPage instance by specified objectId. 
		/// The method trys to resolve instance from DynamicPageConfigurationParser configured in Spring.NET.
		/// The method has mechanism to automatically clone the dynamic configuration to 
		///		avoid the changes by the IDynamicPageConfigurationProcessCallback having impact to other callers
		///		if the attribute @ProcessCallbackType of dynamic page configuration does exist.
		/// If objectId is not found in DynamicPageConfigurationParser, the exception <see cref="ConfigurationErrorsException"/> is thrown. 
		/// </summary>
		/// <param name="objectId">Requesting dynamic page object id.</param>
		/// <param name="doNotSetupContextTempVariables">True to not setup context temporary variables on the IDynamicPage instance.</param>
		/// <param name="requestHandler">Request handler.</param>
		/// <returns></returns>
		public IDynamicPage GetDynamicPage(string objectId, bool doNotSetupContextTempVariables, IRequestHandler requestHandler)
		{
			try
			{
				IConfigurationParser parser = SpringContext.Current.GetObject<DynamicPageConfigurationParser>();
				if (!parser.ContainsObject(objectId))
					throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.DP_ObjectIdNotConfigured, objectId));

				DynamicPageConfiguration dynamicPageConfiguration = parser.GetObject<DynamicPageConfiguration>(objectId);
				string dynamicPageByObjectIdKey = string.Format(CultureInfo.InvariantCulture, "IDynamicPage by ObjectId {0}", objectId);
				if (!this.ContextVariables.ContainsKey(dynamicPageByObjectIdKey))
				{
					IDynamicPage dynamicPage = CreateDynamicPageObject(dynamicPageConfiguration.DynamicPageType);
					if (!doNotSetupContextTempVariables)
						dynamicPage.SetupContextTempVariables(requestHandler, new SetupApplicationContextVariablesEventArgs());

					bool cloneConfiguration = dynamicPageConfiguration.ConfigurationProcessCallback != null;
					DynamicPageConfiguration finalVersionDynamicPageConfiguration = cloneConfiguration ? dynamicPageConfiguration.Clone() : dynamicPageConfiguration;
					if (cloneConfiguration)
						finalVersionDynamicPageConfiguration.ConfigurationProcessCallback.Process(finalVersionDynamicPageConfiguration);

					dynamicPage.Configuration = finalVersionDynamicPageConfiguration;
					this.ContextVariables.Add(dynamicPageByObjectIdKey, dynamicPage);
				}

				return this.ContextVariables[dynamicPageByObjectIdKey] as IDynamicPage;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Resolve IDynamicPage instance by specified objectId without setup temporary context variables.
		/// The method trys to resolve instance from DynamicPageConfigurationParser configured in Spring.NET.
		/// The method has mechanism to automatically clone the dynamic configuration to 
		///		avoid the changes by the IDynamicPageConfigurationProcessCallback having impact to other callers
		///		if the attribute @ProcessCallbackType of dynamic page configuration does exist.
		/// If objectId is not found in DynamicPageConfigurationParser, the exception <see cref="ConfigurationErrorsException"/> is thrown. 
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		public IDynamicPage GetDynamicPage(string objectId)
		{
			return this.GetDynamicPage(objectId, true, null);
		}

		/// <summary>
		/// Create IDynamicPage instance resolved from @Type of dynamic page configuration.
		/// </summary>
		/// <returns></returns>
		private static IDynamicPage CreateDynamicPageObject(Type dynamicPageType)
		{
			ConstructorInfo dynamicPageObjectConstructor = dynamicPageType.GetConstructor(Type.EmptyTypes);
			return dynamicPageObjectConstructor.Invoke(null) as IDynamicPage;
		}
	}
}
