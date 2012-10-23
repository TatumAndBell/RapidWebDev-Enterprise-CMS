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
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.Common.Validation;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;
using RapidWebDev.UI.WebResources;
using Rhino.Mocks;
using RapidWebDev.UI.Controls;
using Spring.Aop.Framework;
using Spring.Aop.Support;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// HttpHandler to process request to aggregate panel of dynamic web page.
	/// </summary>
	public class AggregatePanelPageHandler : AbstractPanelPageHandler, IRequiresSessionState
	{
		private Panel controlContainer;
		private IDynamicPage dynamicPage;
		private IAggregatePanelPage aggregatePanelPage;
		private AggregatePanelConfiguration aggregatePanelConfiguration;
		private RapidWebDev.UI.Controls.Button ButtonSave;
		private RapidWebDev.UI.Controls.Button ButtonCancel;
		private UpdatePanel updatePanelAggregatePanelWrapper;
		private HttpCookie selectedEntityIdByQueryCookie;
		private HttpCookie selectedEntityIdCollectionCookie;

		/// <summary>
		/// Gets command argument for aggregate panel which used to do special command onto a bulk of entities.
		/// </summary>
		private string CommandArgument
		{
			get { return QueryStringUtility.CommandArgument; }
		}

		/// <summary>
		/// Gets a bulk of entities 
		/// </summary>
		private IEnumerable<string> EntityIdEnumerable
		{
			get
			{
				string contextKey = string.Format(CultureInfo.InvariantCulture, "RapidWebDev.UI.Services.AggregatePanelPageHandler::EntityIdEnumerable({0})", QueryStringUtility.ObjectId);
				if (!HttpContext.Current.Items.Contains(contextKey))
				{
					lock (this)
					{
						if (!HttpContext.Current.Items.Contains(contextKey))
						{
							IEnumerable<string> entityIdEnumerable = this.ResolveEntityIdsFromCookie();
							HttpContext.Current.Items.Add(contextKey, entityIdEnumerable);
						}
					}
				}

				return HttpContext.Current.Items[contextKey] as IEnumerable<string>;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public AggregatePanelPageHandler() : base()
		{
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.Init event to initialize the page.
		/// </summary>
		/// <param name="e">An System.EventArgs that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.Initialize();

			// create various controls in aggregate panel page.
			ConstructorInfo constructor = this.aggregatePanelConfiguration.Type.GetConstructor(Type.EmptyTypes);
			this.aggregatePanelPage = constructor.Invoke(null) as IAggregatePanelPage;
			this.aggregatePanelPage.Configuration = this.dynamicPage.Configuration;
			this.aggregatePanelPage.ShowMessage += new Action<MessageTypes, string>(base.ShowMessage);
			this.aggregatePanelPage.SetupContextTempVariables(base.GetRequestHandler(), new SetupApplicationContextVariablesEventArgs());

			this.PermissionCheck();

			HtmlGenericControl htmlTag = new HtmlGenericControl("html");
			htmlTag.Attributes["xmlns"] = "http://www.w3.org/1999/xhtml";
			this.Controls.Add(htmlTag);

			this.CreateHtmlHead(htmlTag);
			this.CreateHtmlBody(htmlTag);

			try
			{
				this.aggregatePanelPage.OnInit(base.GetRequestHandler(), new AggregatePanelPageEventArgs(this.CommandArgument, this.EntityIdEnumerable));
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (ValidationException exp)
			{
				base.ShowMessage(MessageTypes.Warn, exp.Message);
			}
			catch (Exception exp)
			{
				string errorMessage = string.Format(CultureInfo.InvariantCulture, UNKNOWN_ERROR_LOGGING_MSG, HttpContext.Current.Request.Url);
				Logger.Instance(this).Error(errorMessage, exp);
				base.ShowMessage(MessageTypes.Error, Resources.DP_UnknownErrorDetail);
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
				this.aggregatePanelPage.OnLoad(base.GetRequestHandler(), new AggregatePanelPageEventArgs(this.CommandArgument, this.EntityIdEnumerable));
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (ValidationException exp)
			{
				base.ShowMessage(MessageTypes.Warn, exp.Message);
			}
			catch (Exception exp)
			{
				string errorMessage = string.Format(CultureInfo.InvariantCulture, UNKNOWN_ERROR_LOGGING_MSG, HttpContext.Current.Request.Url);
				Logger.Instance(this).Error(errorMessage, exp);
				base.ShowMessage(MessageTypes.Error, Resources.DP_UnknownErrorDetail);
			}
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
				this.aggregatePanelPage.OnPreRender(base.GetRequestHandler(), new AggregatePanelPageEventArgs(this.CommandArgument, this.EntityIdEnumerable));
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (ValidationException exp)
			{
				base.ShowMessage(MessageTypes.Warn, exp.Message);
			}
			catch (Exception exp)
			{
				string errorMessage = string.Format(CultureInfo.InvariantCulture, UNKNOWN_ERROR_LOGGING_MSG, HttpContext.Current.Request.Url);
				Logger.Instance(this).Error(errorMessage, exp);
				base.ShowMessage(MessageTypes.Error, Resources.DP_UnknownErrorDetail);
			}
		}

		/// <summary>
		/// Initializes the System.Web.UI.HtmlTextWriter object and calls on the child controls of the System.Web.UI.Page to render.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			ScriptManager scriptManager = ScriptManager.GetCurrent(this);
			if (scriptManager == null || !scriptManager.IsInAsyncPostBack)
				writer.Write(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
			
			base.Render(writer);
		}

		private void ButtonSave_Click(object sender, EventArgs e)
		{
			try
			{
				this.aggregatePanelPage.Save(this.CommandArgument, this.EntityIdEnumerable);

				const string savedSuccessfullyJsTemplate = @"
					if (!window.parent.RegisteredGridViewPanelObject) return;
					window.parent.RegisteredGridViewPanelObject.ExecuteQuery(true);

					if ($ShowMessageAfterSavedSuccessfully$) window.parent.RWD.MessageBox.Info('$InformationText$', '$SavedSuccessfullyMessage$');
					window.parent.RegisteredGridViewPanelObject.HideAggregatePanelWindow();
					return false;";

				string savedSuccessfullyJs = savedSuccessfullyJsTemplate.Replace("$ShowMessageAfterSavedSuccessfully$", this.aggregatePanelConfiguration.ShowMessageAfterSavedSuccessfully.ToString().ToLowerInvariant())
					.Replace("$InformationText$", Resources.DP_InformationText)
					.Replace("$SavedSuccessfullyMessage$", Resources.DP_SavedSuccessfullyMessage);
				ClientScripts.OnDocumentReady.Add2BeginOfBody(savedSuccessfullyJs);

				this.ExpireEntityIdSelectionCookie();
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (ValidationException exp)
			{
				base.ShowMessage(MessageTypes.Warn, exp.Message);
			}
			catch (Exception exp)
			{
				string errorMessage = string.Format(CultureInfo.InvariantCulture, UNKNOWN_ERROR_LOGGING_MSG, HttpContext.Current.Request.Url);
				Logger.Instance(this).Error(errorMessage, exp);
				base.ShowMessage(MessageTypes.Error, Resources.DP_UnknownErrorDetail);
			}
		}

		/// <summary>
		/// The method is working on 
		/// </summary>
		private void Initialize()
		{
			if (string.IsNullOrEmpty(this.CommandArgument))
				throw new BadRequestException(@"The query string parameter ""CommandArgument"" is not specified.");

			try
			{
				this.dynamicPage = DynamicPageContext.Current.GetDynamicPage(QueryStringUtility.ObjectId);
			}
			catch (ConfigurationErrorsException exp)
			{
				Logger.Instance(this).Warn(exp);
				throw new InternalServerErrorException(exp);
			}

			IEnumerable<AggregatePanelConfiguration> aggregatePanelConfigurations = this.dynamicPage.Configuration.Panels
				.Where(p => p.PanelType == DynamicPagePanelTypes.AggregatePanel)
				.Cast<AggregatePanelConfiguration>();

			aggregatePanelConfiguration = aggregatePanelConfigurations.FirstOrDefault(p => string.Equals(p.CommandArgument, this.CommandArgument, StringComparison.OrdinalIgnoreCase));
			if (aggregatePanelConfiguration == null)
				aggregatePanelConfiguration = aggregatePanelConfigurations.FirstOrDefault(p => string.Equals(p.CommandArgument, "", StringComparison.OrdinalIgnoreCase));
			if (this.aggregatePanelConfiguration == null)
				throw new BadRequestException(@"Aggregate panel of the dynamic page is not configured.");
		}

		private void PermissionCheck()
		{
			string permissionValue = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", this.dynamicPage.Configuration.PermissionValue, this.CommandArgument);

			// check whether the user has permission to use aggregate page with command argument.
			if (!permissionBridge.HasPermission(permissionValue))
				throw new UnauthorizedException(string.Format(CultureInfo.InvariantCulture, @"The user doesn't have the permission to access the aggregate page with permission value ""{0}"".", permissionValue));
		}

		private void CreateHtmlHead(Control htmlContainer)
		{
			htmlContainer.Controls.Add(new HtmlHead { Title = this.aggregatePanelConfiguration.HeaderText });
			LiteralControl metaDefinition = new LiteralControl(
				@"<meta http-equiv=""Expires"" content=""0"" ></meta>
				<meta http-equiv=""Cache-Control"" content=""no-cache""></meta>
				<meta http-equiv=""Pragma"" content=""no-cache""></meta>");
			this.Header.Controls.Add(metaDefinition);

			// register style and script file references.
			IWebResourceManager resourceManager = SpringContext.Current.GetObject<IWebResourceManager>();
			resourceManager.Flush("AggregatePanelPage");

			// initialize ExtJs cookie state and quick tips
			ClientScripts.OnDocumentReady.Add2BeginOfBody("Ext.QuickTips.init();", JavaScriptPriority.High);
		}

		private void CreateHtmlBody(Control htmlContainer)
		{
			HtmlGenericControl htmlBody = new HtmlGenericControl("body");
			htmlBody.Attributes["class"] = "aggregatepanelpagebody";
			htmlContainer.Controls.Add(htmlBody);

			HtmlForm htmlForm = new HtmlForm { ID = "AggregatePanelForm" };
			htmlBody.Controls.Add(htmlForm);

			ScriptManager scriptManager = new ScriptManager { ID = "ScriptManagerObj", EnablePartialRendering = true, EnableScriptGlobalization = true };
			htmlForm.Controls.Add(scriptManager);

			UpdateProgress updateProgress = new UpdateProgress { ID = "UpdateProgressObj", DisplayAfter = 100, ProgressTemplate = base.CreateUpdateProgressTemplate() };
			htmlForm.Controls.Add(updateProgress);

			Panel pageContainer = new Panel { ID = "AggregatePanelContainer", CssClass = "aggregatepanel" };
			htmlForm.Controls.Add(pageContainer);

			HtmlGenericControl h4Subject = new HtmlGenericControl("h4") { InnerText = this.aggregatePanelConfiguration.HeaderText };
			pageContainer.Controls.Add(h4Subject);

			this.controlContainer = new Panel { ID = "AggregatePanelTemplateControlContainer", CssClass = "content" };
			pageContainer.Controls.Add(this.controlContainer);
			this.controlContainer.Controls.Add(this.CreateTemplateControl());

			Control buttonContainerControl = this.CreateButtonContainerControl();
			pageContainer.Controls.Add(buttonContainerControl);

			base.SetFocusOnFirstInputControlWhenPanelLoaded(this.aggregatePanelConfiguration.SetFocusOnFirstInputControlAutomatically);
		}

		private Control CreateTemplateControl()
		{
			Control skinTemplate = this.LoadControl(this.aggregatePanelConfiguration.SkinPath);
			skinTemplate.ID = "AggregatePanelTemplateControl";
			WebUtility.SetControlsByBindingAttribute(skinTemplate, this.aggregatePanelPage);

			this.updatePanelAggregatePanelWrapper = new UpdatePanel { ID = "UpdatePanelAggregatePanelWrapper", UpdateMode = UpdatePanelUpdateMode.Conditional };
			this.updatePanelAggregatePanelWrapper.ContentTemplateContainer.Controls.Add(skinTemplate);
			return this.updatePanelAggregatePanelWrapper;
		}

		private Control CreateButtonContainerControl()
		{
			Panel buttonContainer = new Panel { ID = "AggregatePanelButtonContainer" };
			buttonContainer.Style["padding-top"] = "6px";
			buttonContainer.Style["text-align"] = "center";

			HtmlTable buttonLayoutTable = new HtmlTable { CellPadding = 0, CellSpacing = 0 };
			buttonLayoutTable.Style["margin"] = "auto";
			buttonContainer.Controls.Add(buttonLayoutTable);

			HtmlTableRow buttonLayoutRow = new HtmlTableRow();
			buttonLayoutTable.Rows.Add(buttonLayoutRow);

			HtmlTableCell buttonLayoutCell = new HtmlTableCell("td");
			buttonLayoutRow.Cells.Add(buttonLayoutCell);

			if (this.aggregatePanelConfiguration.SaveButton != null)
			{
				this.ButtonSave = new RapidWebDev.UI.Controls.Button { ID = "ButtonSave", Text = this.aggregatePanelConfiguration.SaveButton.Text ?? Resources.DPCtrl_SaveText, ToolTip = this.aggregatePanelConfiguration.SaveButton.ToolTip };
				this.ButtonSave.Click += new EventHandler(this.ButtonSave_Click);
				buttonLayoutCell.Controls.Add(this.ButtonSave);
				this.updatePanelAggregatePanelWrapper.Triggers.Add(new AsyncPostBackTrigger { ControlID = this.ButtonSave.ID, EventName = "Click" });
				base.SetFormDefaultButton(this.aggregatePanelConfiguration.SaveButton, "ButtonSave");
			}

			if (this.aggregatePanelConfiguration.CancelButton != null)
			{
				buttonLayoutCell.Controls.Add(new HtmlGenericControl("span") { InnerText = " " });

				const string javaScriptBlock = @"if (window.parent.RegisteredGridViewPanelObject) window.parent.RegisteredGridViewPanelObject.HideAggregatePanelWindow(); return false;";
				this.ButtonCancel = new RapidWebDev.UI.Controls.Button() { ID = "ButtonCancel", Text = this.aggregatePanelConfiguration.CancelButton.Text ?? Resources.DPCtrl_CancelText, ToolTip = this.aggregatePanelConfiguration.CancelButton.ToolTip };
				this.ButtonCancel.OnClientClick = javaScriptBlock;
				buttonLayoutCell.Controls.Add(this.ButtonCancel);
				this.updatePanelAggregatePanelWrapper.Triggers.Add(new AsyncPostBackTrigger { ControlID = this.ButtonCancel.ID, EventName = "Click" });
				base.SetFormDefaultButton(this.aggregatePanelConfiguration.CancelButton, "ButtonCancel");
			}

			return buttonContainer;
		}

		/// <summary>
		/// Resolve enumerabe entity ids from cookie whatever a query or directly selected entity ids stored there. 
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> ResolveEntityIdsFromCookie()
		{
			IEnumerable<string> entityIdResults = null;
			string selectedEntityIdByQueryKey = QueryStringUtility.ObjectId + "_SelectedEntityIdByQuery";
			string selectedEntityIdByQueryCookieName = HttpContext.Current.Request.Cookies.AllKeys.FirstOrDefault(cookieName => cookieName.Contains(selectedEntityIdByQueryKey));
			if (!string.IsNullOrEmpty(selectedEntityIdByQueryCookieName))
			{
				this.selectedEntityIdByQueryCookie = HttpContext.Current.Request.Cookies[selectedEntityIdByQueryCookieName];
				if (!string.IsNullOrEmpty(this.selectedEntityIdByQueryCookie.Value) && !string.Equals(this.selectedEntityIdByQueryCookie.Value, "null", StringComparison.InvariantCultureIgnoreCase))
				{
					NameValueCollection nameValueCollection = this.ResolveQueryStringFromCookie(this.selectedEntityIdByQueryCookie);
					QueryParameter queryParameter = QueryParameter.CreateQueryParameter(this.dynamicPage, nameValueCollection, 0, int.MaxValue, null, "ASC");
					QueryResults queryResults = this.dynamicPage.Query(queryParameter);
					entityIdResults = this.ResolveEntityIdsFromQueryResults(queryResults);
				}
			}

			if (entityIdResults == null)
			{
				string selectedEntityIdCollectionKey = QueryStringUtility.ObjectId + "_SelectedEntityIdCollection";
				string selectedEntityIdCollectionCookieName = HttpContext.Current.Request.Cookies.AllKeys.FirstOrDefault(cookieName => cookieName.Contains(selectedEntityIdCollectionKey));
				if (!string.IsNullOrEmpty(selectedEntityIdCollectionCookieName))
				{
					this.selectedEntityIdCollectionCookie = HttpContext.Current.Request.Cookies[selectedEntityIdCollectionCookieName];
					entityIdResults = this.ResolveEntityIdsFromEntityIdCollectionCookie(this.selectedEntityIdCollectionCookie);
				}
			}

			IEnumerable<BasePanelConfiguration> baseButtonPanelConfigurations = this.aggregatePanelPage.Configuration.Panels.Where(p => p.PanelType == DynamicPagePanelTypes.ButtonPanel);
			IEnumerable<ButtonPanelConfiguration> buttonPanelConfigurations = baseButtonPanelConfigurations.Cast<ButtonPanelConfiguration>();
			IEnumerable<ButtonConfiguration> buttonConfigurations = buttonPanelConfigurations.SelectMany(p => p.Buttons.Where(b => b.CommandArgument == this.CommandArgument));
			if (buttonConfigurations.Count() == 0)
				throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, @"The command argument ""{0}"" is not configured in the aggregate page.", this.CommandArgument));

			bool allowSubmitWithoutSelectedEntities = buttonConfigurations.Count(b => !b.GridSelectionRequired) > 0;
			if (entityIdResults == null)
			{
				if (allowSubmitWithoutSelectedEntities)
					entityIdResults = new List<string>();
				else
					throw new BadRequestException(@"There is no selected entity in the submit which is not allowed for the aggregate panel.");
			}

			return entityIdResults;
		}

		/// <summary>
		/// If the cookie stores a query for selected entity ids, there needs a way to restore cookie values to database query.
		/// The method is used to restore cookie to the name-value collection which QueryParameter can parse it to database query. 
		/// </summary>
		/// <param name="selectedEntityIdByQueryCookie"></param>
		/// <returns></returns>
		private NameValueCollection ResolveQueryStringFromCookie(HttpCookie selectedEntityIdByQueryCookie)
		{
			string dataQueryString = Kit.UrlDecode(selectedEntityIdByQueryCookie.Value);
			if (dataQueryString.StartsWith("s:", StringComparison.InvariantCultureIgnoreCase))
				dataQueryString = dataQueryString.Substring(2);
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			var o = serializer.DeserializeObject(dataQueryString);

			NameValueCollection nameValueCollection = new NameValueCollection();
			IEnumerable queryParameters = o as IEnumerable;
			if (queryParameters != null)
			{
				foreach (IDictionary<string, object> queryParameter in queryParameters)
				{
					string name = queryParameter["Name"] != null ? queryParameter["Name"].ToString() : null;
					string value = queryParameter["Value"] != null ? queryParameter["Value"].ToString() : null;
					nameValueCollection.Add(name, value);
				}
			}

			return nameValueCollection;
		}

		/// <summary>
		/// Collect entity ids from query results gotten from IDynamicPage instance.
		/// The method uses PrimaryKeyFieldName configured in GridViewPanel to parse entity ids from objects in query results.
		/// </summary>
		/// <param name="queryResults"></param>
		/// <returns></returns>
		private IEnumerable<string> ResolveEntityIdsFromQueryResults(QueryResults queryResults)
		{
			Collection<string> entityIds = new Collection<string>();
			string primaryKeyFieldName = null;
			foreach (object queryResult in queryResults.Results)
			{
				if (string.IsNullOrEmpty(primaryKeyFieldName))
				{
					GridViewPanelConfiguration gridViewPanelConfiguration = this.dynamicPage.Configuration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.GridViewPanel) as GridViewPanelConfiguration;
					primaryKeyFieldName = gridViewPanelConfiguration.PrimaryKeyFieldName;
				}

				object primaryKeyFieldValue = DataBinder.Eval(queryResult, primaryKeyFieldName);
				if (primaryKeyFieldValue != null)
					entityIds.Add(primaryKeyFieldValue.ToString());
			}

			return entityIds;
		}

		/// <summary>
		/// The method is to restore selected entity ids directly from cookie.
		/// </summary>
		/// <param name="selectedEntityIdCollectionCookie"></param>
		/// <returns></returns>
		private IEnumerable<string> ResolveEntityIdsFromEntityIdCollectionCookie(HttpCookie selectedEntityIdCollectionCookie)
		{
			string dataQueryString = Kit.UrlDecode(selectedEntityIdCollectionCookie.Value);
			if (dataQueryString.StartsWith("s:", StringComparison.InvariantCultureIgnoreCase))
				dataQueryString = dataQueryString.Substring(2);

			JavaScriptSerializer serializer = new JavaScriptSerializer();
			IEnumerable entityIdEnumerable = serializer.DeserializeObject(dataQueryString) as IEnumerable;
			return entityIdEnumerable.Cast<string>();
		}
		
		/// <summary>
		/// Once save/cancel in aggregate page, the cookie stored multiple selected entity ids will be cleared.
		/// The method is used to expire that cookie.
		/// </summary>
		private void ExpireEntityIdSelectionCookie()
		{
			if (this.selectedEntityIdByQueryCookie != null)
			{
				this.selectedEntityIdByQueryCookie.Expires = DateTime.Now.AddSeconds(-1);
				HttpContext.Current.Response.Cookies.Set(this.selectedEntityIdByQueryCookie);
			}

			if (this.selectedEntityIdCollectionCookie != null)
			{
				this.selectedEntityIdCollectionCookie.Expires = DateTime.Now.AddSeconds(-1);
				HttpContext.Current.Response.Cookies.Set(this.selectedEntityIdCollectionCookie);
			}
		}
	}
}
