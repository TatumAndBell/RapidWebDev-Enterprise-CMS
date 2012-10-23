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
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The abstract implementation to interface IDetailPanelPage. 
	/// The class has declared all interface methods as virtual so that the business implementation class can implement signatures by requirement. 
	/// </summary>
	public abstract class DetailPanelPage : IDetailPanelPage
	{
		/// <summary>
		/// The delegate will be registered by dynamic page infrastructure. 
		/// It's used for dynamic page developers to show messages onto UI to users through internal infrastructure.
		/// </summary>
		public Action<MessageTypes, string> ShowMessage { get; set; }

		/// <summary>
		/// Gets/sets page dynamic page configuration to build up web page.
		/// </summary>
		public DynamicPageConfiguration Configuration { get; set; }

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		public virtual void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e) { }

		/// <summary>
		/// Create a new entity from detail panel and return id of the entity.
		/// The method needs to create a new entity and set control values to its properties then persist it.
		/// </summary>
		/// <returns>returns id of the new entity.</returns>
		public virtual string Create() { return string.Empty; }

		/// <summary>
		/// Update an existed entity from detail panel.
		/// The method needs to load an existed entity by specified id and set control values to overwrite its original properties then persist it.
		/// </summary>
		/// <param name="entityId"></param>
		public virtual void Update(string entityId) { }

		/// <summary>
		/// Reset all controls of the detail panel to initial state.
		/// The method will be invoked when enables the detail panel to support creating entities continuously.
		/// After an entity been created, the method will be invoked to reset form controls for another input.
		/// The default action of "Reset" is to redirect to current page.
		/// </summary>
		public virtual void Reset() 
		{
			ClientScripts.OnDocumentReady.Add2EndOfBody("window.location.reload();", JavaScriptPriority.Low);
		}

		/// <summary>
		/// The method is designed to load entity by id to editable detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		public virtual void LoadWritableEntity(string entityId) { }

		/// <summary>
		/// The method is designed to load entity by id to readonly detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		public virtual void LoadReadOnlyEntity(string entityId) 
		{
			this.LoadWritableEntity(entityId);
			WebUtility.MakeBindingControlsReadOnly(this);
		}

		/// <summary>
		/// The method will be invoked when the aggregate panel is initialized.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument.</param>
		public virtual void OnInit(IRequestHandler sender, DetailPanelPageEventArgs e) { }

		/// <summary>
		/// The method will be invoked when detail panel is loaded.
		/// The implementation can resolve current visiting Page from the conversation as (HttpContext.Current.Handler as Page).
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument.</param>
		public virtual void OnLoad(IRequestHandler sender, DetailPanelPageEventArgs e) { }

		/// <summary>
		/// The method will be invoked when the detail panel is prerendering.
		/// The implementation can resolve current visiting Page from the conversation as (HttpContext.Current.Handler as Page).
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument.</param>
		public virtual void OnPreRender(IRequestHandler sender, DetailPanelPageEventArgs e) { }
	}
}

