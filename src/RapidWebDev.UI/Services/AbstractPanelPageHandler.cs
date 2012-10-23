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
using Spring.Core;


namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// HttpHandler to process request to aggregate panel of dynamic web page.
	/// </summary>
	public abstract class AbstractPanelPageHandler : SupportDocumentReadyPage, IRequiresSessionState
	{
		private delegate bool UpdateProgressTemplateInstantiateInCallback(Control container);
		private const string UPDATE_PROGRESS_IMAGE_FORMAT = @"<img src=""$ProgressIndicatorImageUrl$"" style=""float:left;vertical-align:top;margin-right:8px"" alt="""" />";
		private const string SET_DEFAULT_FORM_BUTTON = @"
			$("":text, :password, :radio"").keydown(function()
			{
				if(Ext.EventObject.getKey() == '13')
				{
					var requiredControlIDs = $(""label.required"");
					for(var i = 0; i < requiredControlIDs.length; i++)
					{
						var targetControlID = $(requiredControlIDs[i]).attr(""for"");
						var targetControl = $(""#"" + targetControlID);
						if (targetControl.length <= 0) continue;
						if (targetControl[0].tagName.toLowerCase() != ""input"") continue;
						
						var targetControlValue = targetControl.val();
						if ($.trim(targetControlValue).length <= 0)
						{
							try
							{
								$(""#"" + targetControlID).focus();
							}
							catch (ex)
							{
							}
							return;
						}
					}
					
					var command = $(""###ButtonID##"").attr(""href"");
					eval(command);
				}
			});";

		private const string SET_FOCUS_ON_FIRST_INPUT_CONTROL = @"
			var inputSearcher = $("":text, :password, :radio"");
			if (inputSearcher.length > 0)
			{
				for (var i=0; i<inputSearcher.length; i++)
				{
					var inputControl = inputSearcher[i];
					if (!inputControl.disabled)
					{
						try
						{
							inputSearcher[i].focus();
						}
						catch (ex)
						{
						}
						break;
					}
				}
			}
			
			var inputSearcher = $(""select"");
			if (inputSearcher.length > 0)
			{
				for (var i=0; i<inputSearcher.length; i++)
				{
					var inputControl = inputSearcher[i];
					if (!inputControl.disabled)
					{
						try
						{
							inputSearcher[i].focus();
						}
						catch (ex)
						{
						}
						break;
					}
				}
			}";

		/// <summary>
		/// The server encountered an unknown error when accessing the URI {0}.
		/// </summary>
		protected const string UNKNOWN_ERROR_LOGGING_MSG = "The server encountered an unknown error when accessing the URI {0}.";

		/// <summary>
		/// Permission bridge.
		/// </summary>
		protected static readonly IPermissionBridge permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();

		private MockRepository mockRepository = new MockRepository();
		private IRequestHandler requestHandler;

		/// <summary>
		/// Constructor
		/// </summary>
		public AbstractPanelPageHandler()
		{
			base.EnableEventValidation = false;
		}

		/// <summary>
		/// Show message in popup dialog.
		/// </summary>
		/// <param name="messageType"></param>
		/// <param name="message"></param>
		protected void ShowMessage(MessageTypes messageType, string message)
		{
			const string messageBoxJsTemplate = "window.RWD.MessageBox.$MessageType$('$MessageType$', '$MessageBody$');";
			string outputMessage = WebUtility.EncodeJavaScriptString(message);
			ClientScripts.OnDocumentReady.Add2BeginOfBody(messageBoxJsTemplate.Replace("$MessageType$", messageType.ToString()).Replace("$MessageBody$", outputMessage));
		}

		/// <summary>
		/// Create Update Progress Template
		/// </summary>
		/// <returns></returns>
		protected ITemplate CreateUpdateProgressTemplate()
		{
			ITemplate template = this.mockRepository.DynamicMock<ITemplate>();
			template.InstantiateIn(null);
			LastCall.IgnoreArguments().Callback(new UpdateProgressTemplateInstantiateInCallback(container =>
			{
				HtmlGenericControl loadingMaskContainer = new HtmlGenericControl("div");
				loadingMaskContainer.Attributes["id"] = "loading-mask";
				container.Controls.Add(loadingMaskContainer);

				HtmlGenericControl loadingContainer = new HtmlGenericControl("div");
				loadingContainer.Attributes["id"] = "loading";
				container.Controls.Add(loadingContainer);

				HtmlGenericControl loadingIndicatorContainer = new HtmlGenericControl("div");
				loadingIndicatorContainer.Attributes["id"] = "loading-indicator";
				loadingIndicatorContainer.InnerHtml = UPDATE_PROGRESS_IMAGE_FORMAT.Replace("$ProgressIndicatorImageUrl$", Kit.ResolveAbsoluteUrl("~/Resources/Images/progress.gif"));
				loadingContainer.Controls.Add(loadingIndicatorContainer);
				return true;
			}));

			this.mockRepository.ReplayAll();
			return template;
		}

		/// <summary>
		/// Get request handler.
		/// </summary>
		/// <returns></returns>
		protected IRequestHandler GetRequestHandler()
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

		/// <summary>
		/// Set the button being the default button of the form.
		/// </summary>
		/// <param name="buttonConfiguration"></param>
		/// <param name="buttonId"></param>
		protected void SetFormDefaultButton(FormPanelButtonConfiguration buttonConfiguration, string buttonId)
		{
			if (buttonConfiguration.IsFormDefaultButton)
			{
				string js = SET_DEFAULT_FORM_BUTTON.Replace("##ButtonID##", buttonId);
				ClientScripts.OnDocumentReady.Add2EndOfBody(js, JavaScriptPriority.Normal);
			}
		}

		/// <summary>
		/// Set focus on first input control automatically when the form is loaded.
		/// </summary>
		/// <param name="whetherToSetFocus"></param>
		protected void SetFocusOnFirstInputControlWhenPanelLoaded(bool whetherToSetFocus)
		{
			if (whetherToSetFocus)
				ClientScripts.OnDocumentReady.Add2EndOfBody(SET_FOCUS_ON_FIRST_INPUT_CONTROL, JavaScriptPriority.Normal);
		}
	}
}