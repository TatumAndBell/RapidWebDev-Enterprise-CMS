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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Rhino.Mocks;
using Spring.Aop.Framework;
using Spring.Aop.Support;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// HttpHandler to process request to detail panel of dynamic web page.
	/// </summary>
	public class DetailPanelPageHandler : AbstractPanelPageHandler, IRequiresSessionState
	{
		private Control skinTemplate;
		private string currentEntityId;
		private IDynamicPage dynamicPage;
		private DetailPanelPageRenderModes renderMode;
		private IDetailPanelPage detailPanelPage;
		private DetailPanelConfiguration detailPanelConfiguration;
		private RapidWebDev.UI.Controls.Button ButtonSaveAndClose;
		private RapidWebDev.UI.Controls.Button ButtonSaveAndAddAnother;
		private RapidWebDev.UI.Controls.Button ButtonCancel;
		private UpdatePanel updatePanelDetailPanelWrapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public DetailPanelPageHandler()
		{
			base.EnableEventValidation = false;
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.Init event to initialize the page.
		/// </summary>
		/// <param name="e">An System.EventArgs that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.Initialize();

			ConstructorInfo constructor = this.detailPanelConfiguration.Type.GetConstructor(Type.EmptyTypes);
			this.detailPanelPage = constructor.Invoke(null) as IDetailPanelPage;
			this.detailPanelPage.Configuration = this.dynamicPage.Configuration;
			this.detailPanelPage.ShowMessage += new Action<MessageTypes, string>(base.ShowMessage);
			this.detailPanelPage.SetupContextTempVariables(base.GetRequestHandler(), new SetupApplicationContextVariablesEventArgs());

			this.PermissionCheck();

			HtmlGenericControl htmlTag = new HtmlGenericControl("html");
			htmlTag.Attributes["xmlns"] = "http://www.w3.org/1999/xhtml";
			this.Controls.Add(htmlTag);

			this.CreateHtmlHead(htmlTag);
			this.CreateHtmlBody(htmlTag);

			try
			{
				this.detailPanelPage.OnInit(base.GetRequestHandler(), new DetailPanelPageEventArgs(this.currentEntityId, this.renderMode));
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
		/// Raises the System.Web.UI.Control.Load event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// bind control of template file to class fields marked with BindingAttribute.
			WebUtility.SetControlsByBindingAttribute(this.skinTemplate, this.detailPanelPage);

			try
			{
				this.detailPanelPage.OnLoad(base.GetRequestHandler(), new DetailPanelPageEventArgs(this.currentEntityId, this.renderMode));
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
				if (!Page.IsPostBack)
				{
					switch (this.renderMode)
					{
						case DetailPanelPageRenderModes.Update:
							this.detailPanelPage.LoadWritableEntity(this.currentEntityId);
							break;

						case DetailPanelPageRenderModes.View:
							this.detailPanelPage.LoadReadOnlyEntity(this.currentEntityId);
							break;
					}
				}
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
				if (this.detailPanelPage != null)
					this.detailPanelPage.OnPreRender(base.GetRequestHandler(), new DetailPanelPageEventArgs(QueryStringUtility.EntityId, QueryStringUtility.DetailPanelPageRenderMode));

				if (this.renderMode == DetailPanelPageRenderModes.View)
				{
					const string hideRedStarBlocks = "$('.required').css('display', 'none');";
					ClientScripts.OnDocumentReady.Add2BeginOfBody(hideRedStarBlocks, JavaScriptPriority.High);
				}
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
				switch (this.renderMode)
				{
					case DetailPanelPageRenderModes.New:
						this.detailPanelPage.Create();
						break;

					case DetailPanelPageRenderModes.Update:
						this.detailPanelPage.Update(this.currentEntityId);
						break;

					default:
						return;
				}

				const string savedSuccessfullyJsTemplate = @"
					if (!window.parent.RegisteredGridViewPanelObject) return;
					window.parent.RegisteredGridViewPanelObject.ExecuteQuery(true);
					if ($CloseDialogAfterSave$)
					{
						if ($ShowMessageAfterSavedSuccessfully$) window.parent.RWD.MessageBox.Info('$InformationText$', '$SavedSuccessfullyMessage$');
						window.parent.RegisteredGridViewPanelObject.HideDetailPanelWindow();
					}
					else if ($ShowMessageAfterSavedSuccessfully$)
						window.RWD.MessageBox.Info('$InformationText$', '$SavedSuccessfullyMessage$');

					return false;";

				bool closeDialogAfterSave = sender == this.ButtonSaveAndClose;
				string savedSuccessfullyJs = savedSuccessfullyJsTemplate.Replace("$CloseDialogAfterSave$", closeDialogAfterSave.ToString().ToLowerInvariant())
					.Replace("$ShowMessageAfterSavedSuccessfully$", this.detailPanelConfiguration.ShowMessageAfterSavedSuccessfully.ToString().ToLowerInvariant())
					.Replace("$InformationText$", Resources.DP_InformationText)
					.Replace("$SavedSuccessfullyMessage$", Resources.DP_SavedSuccessfullyMessage);
				ClientScripts.OnDocumentReady.Add2BeginOfBody(savedSuccessfullyJs);

				if (!closeDialogAfterSave)
					this.detailPanelPage.Reset();
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
			try
			{
				this.dynamicPage = DynamicPageContext.Current.GetDynamicPage(QueryStringUtility.ObjectId);
			}
			catch (ConfigurationErrorsException exp)
			{
				Logger.Instance(this).Warn(exp);
				throw new InternalServerErrorException(exp);
			}

			this.detailPanelConfiguration = this.dynamicPage.Configuration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.DetailPanel) as DetailPanelConfiguration;
			if (this.detailPanelConfiguration == null)
			{
				Logger.Instance(this).WarnFormat("The dynamic page {0} doesn't include detail panel configuration.", QueryStringUtility.ObjectId);
				throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, @"There is no detail panel configured in the dynamic page ""{0}"".", QueryStringUtility.ObjectId));
			}

			this.currentEntityId = QueryStringUtility.EntityId;
			this.renderMode = QueryStringUtility.DetailPanelPageRenderMode;
		}

		private void PermissionCheck()
		{
			string permissionValue = null;
			string urlRefererLocalPath = ResolveUrlRefererLocalPath();
			if (this.renderMode == DetailPanelPageRenderModes.View)
			{
				string permissionValueTemplate = null;
				if (urlRefererLocalPath.Equals("MyOrganizationProfile.aspx", StringComparison.InvariantCultureIgnoreCase))
					permissionValueTemplate = string.Format(CultureInfo.InvariantCulture, "MyOrganizationProfile.{0}", QueryStringUtility.EntityId);
				else if (urlRefererLocalPath.Equals("MyProfile.aspx", StringComparison.InvariantCultureIgnoreCase))
					permissionValueTemplate = string.Format(CultureInfo.InvariantCulture, "MyProfile.{0}", QueryStringUtility.EntityId);
				else
					permissionValueTemplate = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", this.dynamicPage.Configuration.PermissionValue, QueryStringUtility.EntityId);

				permissionValue = string.Format(CultureInfo.InvariantCulture, "{0}.View", permissionValueTemplate);

				// check whether the user has permission to access detail panel in rendering mode.
				if (!permissionBridge.HasPermission(permissionValue))
					throw new UnauthorizedException(string.Format(CultureInfo.InvariantCulture, @"The user doesn't have the permission to access the detail panel page with permission value ""{0}"".", permissionValue));
			}
			else if (this.renderMode == DetailPanelPageRenderModes.New)
			{
				permissionValue = string.Format(CultureInfo.InvariantCulture, "{0}.New", this.dynamicPage.Configuration.PermissionValue);

				// check whether the user has permission to create a new entity in detail panel
				if (!permissionBridge.HasPermission(permissionValue))
					throw new UnauthorizedException(string.Format(CultureInfo.InvariantCulture, @"The user doesn't have the permission to access the detail panel page with permission value ""{0}"".", permissionValue));
			}
			else if (this.renderMode == DetailPanelPageRenderModes.Update)
			{
				permissionValue = string.Format(CultureInfo.InvariantCulture, "{0}.Update", this.dynamicPage.Configuration.PermissionValue);

				// check whether the user has permission to update an entity in detail panel
				if (!permissionBridge.HasPermission(permissionValue))
					throw new UnauthorizedException(string.Format(CultureInfo.InvariantCulture, @"The user doesn't have the permission to access the detail panel page with permission value ""{0}"".", permissionValue));
			}
		}

		private static string ResolveUrlRefererLocalPath()
		{
			string urlReferer = HttpContext.Current.Request.QueryString["UrlReferer"];
			if (!string.IsNullOrEmpty(urlReferer))
			{
				try
				{
					urlReferer = Path.GetFileName(new Uri(urlReferer).LocalPath);
				}
				catch
				{
					urlReferer = null;
				}
			}

			if (string.IsNullOrEmpty(urlReferer) && HttpContext.Current.Request.UrlReferrer != null)
				urlReferer = Path.GetFileName(HttpContext.Current.Request.UrlReferrer.LocalPath);

			return urlReferer ?? string.Empty;
		}

		private void CreateHtmlHead(Control htmlContainer)
		{
			htmlContainer.Controls.Add(new HtmlHead { Title = this.detailPanelConfiguration.HeaderText });
			LiteralControl metaDefinition = new LiteralControl(
				@"<meta http-equiv=""Expires"" content=""0"" ></meta>
				<meta http-equiv=""Cache-Control"" content=""no-cache""></meta>
				<meta http-equiv=""Pragma"" content=""no-cache""></meta>");
			this.Header.Controls.Add(metaDefinition);

			// register style and script file references.
			IWebResourceManager resourceManager = SpringContext.Current.GetObject<IWebResourceManager>();
			resourceManager.Flush("DetailPanelPage");

			// initialize ExtJs cookie state and quick tips
			ClientScripts.OnDocumentReady.Add2BeginOfBody("Ext.QuickTips.init();", JavaScriptPriority.High);
		}

		private void CreateHtmlBody(Control htmlContainer)
		{
			HtmlGenericControl htmlBody = new HtmlGenericControl("body");
			htmlBody.Attributes["class"] = "detailpanelpagebody";
			htmlContainer.Controls.Add(htmlBody);

			HtmlForm htmlForm = new HtmlForm { ID = "DetailPanelForm" };
			htmlBody.Controls.Add(htmlForm);

			ScriptManager scriptManager = new ScriptManager { ID = "ScriptManagerObj", EnablePartialRendering = true, EnableScriptGlobalization = true };
			htmlForm.Controls.Add(scriptManager);

			UpdateProgress updateProgress = new UpdateProgress { ID = "UpdateProgressObj", DisplayAfter = 100, ProgressTemplate = base.CreateUpdateProgressTemplate() };
			htmlForm.Controls.Add(updateProgress);

			Panel pageContainer = new Panel { ID = "DetailPanelContainer", CssClass = "detailpanel" };
			htmlForm.Controls.Add(pageContainer);

			HtmlGenericControl h4Subject = new HtmlGenericControl("h4") { InnerText = this.detailPanelConfiguration.HeaderText };
			pageContainer.Controls.Add(h4Subject);

			Panel controlContainer = new Panel { ID = "DetailPanelTemplateControlContainer", CssClass = "content" };
			controlContainer.Controls.Add(this.CreateTemplateControl());
			pageContainer.Controls.Add(controlContainer);

			if (this.renderMode != DetailPanelPageRenderModes.View)
			{
				Control buttonContainerControl = this.CreateButtonContainerControl();
				pageContainer.Controls.Add(buttonContainerControl);
			}

			base.SetFocusOnFirstInputControlWhenPanelLoaded(this.detailPanelConfiguration.SetFocusOnFirstInputControlAutomatically);
		}

		private Control CreateTemplateControl()
		{
			this.skinTemplate = this.LoadControl(this.detailPanelConfiguration.SkinPath);
			this.skinTemplate.ID = "DetailPanelTemplateControl";

			this.updatePanelDetailPanelWrapper = new UpdatePanel { ID = "UpdatePanelDetailPanelWrapper", UpdateMode = UpdatePanelUpdateMode.Conditional };
			this.updatePanelDetailPanelWrapper.ContentTemplateContainer.Controls.Add(this.skinTemplate);
			return this.updatePanelDetailPanelWrapper;
		}

		private Control CreateButtonContainerControl()
		{
			Panel buttonContainer = new Panel { ID = "DetailPanelButtonContainer" };
			buttonContainer.Style["padding-top"] = "6px";
			buttonContainer.Style["text-align"] = "center";

			HtmlTable buttonLayoutTable = new HtmlTable { CellPadding = 0, CellSpacing = 0 };
			buttonLayoutTable.Style["margin"] = "auto";
			buttonContainer.Controls.Add(buttonLayoutTable);

			HtmlTableRow buttonLayoutRow = new HtmlTableRow();
			buttonLayoutTable.Rows.Add(buttonLayoutRow);

			HtmlTableCell buttonLayoutCell = new HtmlTableCell("td");
			buttonLayoutRow.Cells.Add(buttonLayoutCell);

			if (this.detailPanelConfiguration.SaveAndCloseButton != null)
			{
				this.ButtonSaveAndClose = new RapidWebDev.UI.Controls.Button { ID = "ButtonSaveAndClose", Text = this.detailPanelConfiguration.SaveAndCloseButton.Text, ToolTip = this.detailPanelConfiguration.SaveAndCloseButton.ToolTip };
				this.ButtonSaveAndClose.Click += new EventHandler(this.ButtonSave_Click);
				buttonLayoutCell.Controls.Add(this.ButtonSaveAndClose);
				this.updatePanelDetailPanelWrapper.Triggers.Add(new AsyncPostBackTrigger { ControlID = this.ButtonSaveAndClose.ID, EventName = "Click" });
				base.SetFormDefaultButton(this.detailPanelConfiguration.SaveAndCloseButton, "ButtonSaveAndClose");
			}

			if (this.detailPanelConfiguration.SaveAndAddAnotherButton != null && this.renderMode == DetailPanelPageRenderModes.New)
			{
				buttonLayoutCell.Controls.Add(new HtmlGenericControl("span") { InnerText = " " });
				this.ButtonSaveAndAddAnother = new RapidWebDev.UI.Controls.Button { ID = "ButtonSaveAndAddAnother", Text = this.detailPanelConfiguration.SaveAndAddAnotherButton.Text, ToolTip = this.detailPanelConfiguration.SaveAndAddAnotherButton.ToolTip };
				this.ButtonSaveAndAddAnother.Click += new EventHandler(this.ButtonSave_Click);
				buttonLayoutCell.Controls.Add(this.ButtonSaveAndAddAnother);
				this.updatePanelDetailPanelWrapper.Triggers.Add(new AsyncPostBackTrigger { ControlID = this.ButtonSaveAndAddAnother.ID, EventName = "Click" });
				base.SetFormDefaultButton(this.detailPanelConfiguration.SaveAndAddAnotherButton, "ButtonSaveAndAddAnother");
			}

			if (this.detailPanelConfiguration.CancelButton != null)
			{
				buttonLayoutCell.Controls.Add(new HtmlGenericControl("span") { InnerText = " " });
				const string javaScriptBlock = @"if (window.parent.RegisteredGridViewPanelObject) window.parent.RegisteredGridViewPanelObject.HideDetailPanelWindow(); return false;";

				this.ButtonCancel = new RapidWebDev.UI.Controls.Button() { ID = "ButtonCancel", Text = this.detailPanelConfiguration.CancelButton.Text ?? Resources.DPCtrl_CancelText, ToolTip = this.detailPanelConfiguration.CancelButton.ToolTip };
				this.ButtonCancel.OnClientClick = javaScriptBlock;
				buttonLayoutCell.Controls.Add(this.ButtonCancel);
				this.updatePanelDetailPanelWrapper.Triggers.Add(new AsyncPostBackTrigger { ControlID = this.ButtonCancel.ID, EventName = "Click" });
				base.SetFormDefaultButton(this.detailPanelConfiguration.CancelButton, "ButtonCancel");
			}

			return buttonContainer;
		}
	}
}
