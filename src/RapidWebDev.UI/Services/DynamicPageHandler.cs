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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Validation;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;
using RapidWebDev.UI.WebResources;
using Newtonsoft.Json;
using RapidWebDev.UI.Controls;
using Spring.Aop.Framework;
using Spring.Aop.Support;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// HttpHandler to process request to dynamic web page.
	/// </summary>
	public class DynamicPageHandler : SupportDocumentReadyPage, IRequiresSessionState
	{
		private static readonly IDynamicPageLayout dynamicPageLayout = SpringContext.Current.GetObject<IDynamicPageLayout>();
		private static readonly IPermissionBridge permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();
		private static readonly HashSet<string> ExcludedQueryStringNames = new HashSet<string> { "objectid", "stamp" };

		private const string NOCACHE_META = @"
				<meta http-equiv=""Expires"" content=""0"" ></meta>
				<meta http-equiv=""Cache-Control"" content=""no-cache""></meta>
				<meta http-equiv=""Pragma"" content=""no-cache""></meta>";

		private IRequestHandler requestHandler;
		private IDynamicPage dynamicPage;
		private Control controlContainer;

		/// <summary>
		/// Raises the System.Web.UI.Control.Init event to initialize the page.
		/// </summary>
		/// <param name="e">An System.EventArgs that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.EnableViewState = false;
			this.Initialize();

			HtmlGenericControl htmlTag = new HtmlGenericControl("html");
			htmlTag.Attributes["xmlns"] = "http://www.w3.org/1999/xhtml";
			this.Controls.Add(htmlTag);

			this.CreateHtmlHead(htmlTag);
			this.CreateHtmlBody(htmlTag);

			try
			{
				this.dynamicPage.OnInit(this.GetRequestHandler(), new EventArgs());
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (ValidationException exp)
			{
				this.ShowMesage(MessageTypes.Warn, exp.Message);
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				this.ShowMesage(MessageTypes.Warn, Resources.DP_UnknownErrorDetail);
			}
		}

		/// <summary>
		/// Raises the System.Web.UI.Page.LoadComplete event at the end of the page load stage.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			try
			{
				this.dynamicPage.OnLoad(this.GetRequestHandler(), new EventArgs());
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (ValidationException exp)
			{
				this.ShowMesage(MessageTypes.Warn, exp.Message);
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				this.ShowMesage(MessageTypes.Warn, Resources.DP_UnknownErrorDetail);
			}
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			try
			{
				if (this.dynamicPage.Configuration.JavaScriptUrls != null && this.dynamicPage.Configuration.JavaScriptUrls.Count > 0)
				{
					foreach (string javaScriptUrl in this.dynamicPage.Configuration.JavaScriptUrls)
						ClientScripts.RegisterBodyScriptInclude(Kit.ResolveAbsoluteUrl(javaScriptUrl));
				}

				this.dynamicPage.OnPreRender(this.GetRequestHandler(), new EventArgs());
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (ValidationException exp)
			{
				this.ShowMesage(MessageTypes.Warn, exp.Message);
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				this.ShowMesage(MessageTypes.Warn, Resources.DP_UnknownErrorDetail);
			}
		}

		/// <summary>
		/// The method is trying to register gridview panel resizing functionality at the end of window document.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRenderComplete(EventArgs e)
		{
			base.OnPreRenderComplete(e);
			this.CreateGlobalDynamicPageDataServicePostVariables();
			ClientScripts.RegisterStartupScript("window.RegisteredGridViewPanelObject.ResizeGridViewPanel();");
		}

		/// <summary>
		/// Initializes the System.Web.UI.HtmlTextWriter object and calls on the child controls of the System.Web.UI.Page to render.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
			base.Render(writer);
		}

		/// <summary>
		/// The method is working on validating the permission and authentication of user to the page.
		/// </summary>
		private void Initialize()
		{
			try
			{
				this.dynamicPage = DynamicPageContext.Current.GetDynamicPage(QueryStringUtility.ObjectId, false, this.GetRequestHandler());
				this.dynamicPage.ShowMessage += new Action<MessageTypes, string>(this.ShowMesage);
			}
			catch (ConfigurationErrorsException exp)
			{
				throw new InternalServerErrorException(exp);
			}

			string permissionValue = this.dynamicPage.Configuration.PermissionValue;
			if (!permissionBridge.HasPermission(permissionValue))
				throw new UnauthorizedException(string.Format(CultureInfo.InvariantCulture, @"The user doesn't have the permission to access the frame page with permission value ""{0}"".", this.dynamicPage.Configuration.PermissionValue));
		}

		private void ShowMesage(MessageTypes messageType, string message)
		{
			const string messageBoxJsTemplate = "window.RWD.MessageBox.$MessageType$('$MessageType$', '$MessageBody$');";
			string outputMessage = WebUtility.EncodeJavaScriptString(message);
			ClientScripts.OnDocumentReady.Add2BeginOfBody(messageBoxJsTemplate.Replace("$MessageType$", messageType.ToString()).Replace("$MessageBody$", outputMessage));
		}

		private void CreateHtmlHead(Control htmlContainer)
		{
			htmlContainer.Controls.Add(new HtmlHead { Title = this.dynamicPage.Configuration.Title });
			LiteralControl metaDefinition = new LiteralControl(NOCACHE_META);
			this.Header.Controls.Add(metaDefinition);

			// add style/script references.
			IWebResourceManager resourceManager = SpringContext.Current.GetObject<IWebResourceManager>();
			resourceManager.Flush("DynamicPage");

			// initialize ExtJs cookie state and quick tips
			string javaScriptBlock = string.Format(CultureInfo.InvariantCulture, @"Ext.state.Manager.setProvider(new Ext.state.CookieProvider({{ path: '/{0}/' }})); Ext.QuickTips.init();", QueryStringUtility.ObjectId);
			ClientScripts.OnDocumentReady.Add2BeginOfBody(javaScriptBlock, JavaScriptPriority.High);
		}

		private void CreateHtmlBody(Control htmlContainer)
		{
			HtmlGenericControl htmlBody = new HtmlGenericControl("body");
			htmlBody.Attributes["class"] = "dynamicpagebody";
			htmlContainer.Controls.Add(htmlBody);

			HtmlForm htmlForm = new HtmlForm { ID = "DynamicPageForm" };
			htmlBody.Controls.Add(htmlForm);

			Panel pageContainer = new Panel { ID = "DynamicPageContainer", CssClass = "dynamicpage" };
			htmlForm.Controls.Add(pageContainer);

			this.controlContainer = new Panel { ID = "DynamicPageControlContainer", CssClass = "content" };
			this.controlContainer.Controls.Add(dynamicPageLayout.Create(this.dynamicPage.Configuration));
			pageContainer.Controls.Add(this.controlContainer);
		}

		private void CreateGlobalDynamicPageDataServicePostVariables()
		{
			StringBuilder jsonStringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonStringBuilder))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartArray();

				// write ObjectId
				jsonTextWriter.WriteStartObject();
				jsonTextWriter.WritePropertyName("Name");
				jsonTextWriter.WriteValue("ObjectId");
				jsonTextWriter.WritePropertyName("Value");
				jsonTextWriter.WriteValue(QueryStringUtility.ObjectId);
				jsonTextWriter.WriteEndObject();

				foreach (string queryStringName in this.Request.QueryString.Keys)
				{
					if (string.IsNullOrEmpty(queryStringName)) continue;

					string queryStringValue = this.Request.QueryString[queryStringName];
					string lowerCasedQueryStringName = queryStringName.ToLowerInvariant().Trim();
					if (ExcludedQueryStringNames.Contains(lowerCasedQueryStringName)) continue;

					jsonTextWriter.WriteStartObject();
					jsonTextWriter.WritePropertyName("Name");
					jsonTextWriter.WriteValue(queryStringName);
					jsonTextWriter.WritePropertyName("Value");
					jsonTextWriter.WriteValue(queryStringValue);
					jsonTextWriter.WriteEndObject();
				}

				jsonTextWriter.WriteEndArray();
			}

			string javaScriptBlock = string.Format(CultureInfo.InvariantCulture, "window.GlobalDynamicPageDataServicePostVariables = {0};", jsonStringBuilder);
			const string globalDynamicPageDataServicePostVariablesTemplate = @"
				// add a method to get an value by key.
				window.GlobalDynamicPageDataServicePostVariables.GetItem = function(name)
				{
					for (i = 0; i < this.length; i++)
						if (this[i].Name == name) return this[i].Value;

					return null;
				}

				// remove the item with specified name
				window.GlobalDynamicPageDataServicePostVariables.RemoveItem = function(name)
				{
					var item = null;
					for (i = 0; i < this.length; i++)
					{
						if (this[i].Name == name) 
						{
							item = this[i];
							break;
						}
					}

					if (item != null) this.remove(item);
				}

				// add a method to check whether the key does exist in the array.
				window.GlobalDynamicPageDataServicePostVariables.Contains = function(name)
				{
					for (i = 0; i < this.length; i++)
						if (this[i].Name == name) return true;

					return false;
				}";

			ClientScripts.RegisterScriptBlock(javaScriptBlock + globalDynamicPageDataServicePostVariablesTemplate);
		}

		private IRequestHandler GetRequestHandler()
		{
			if (this.requestHandler == null)
			{
				ProxyFactory proxyFactory = new ProxyFactory(this);
				proxyFactory.AddInterface(typeof(IRequestHandler));
				proxyFactory.AddIntroduction(new DefaultIntroductionAdvisor(new HttpRequestHandler()));
				proxyFactory.ProxyTargetType = true;
				this.requestHandler = proxyFactory.GetProxy() as IRequestHandler;
			}

			return this.requestHandler;
		}
	}
}
