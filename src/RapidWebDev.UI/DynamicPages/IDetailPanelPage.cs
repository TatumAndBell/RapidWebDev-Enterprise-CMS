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

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The interface mapping to detail panel of dynamic page.
	/// </summary>
	public interface IDetailPanelPage : IDynamicComponent
	{
		/// <summary>
		/// Create a new entity from detail panel and return id of the entity.
		/// The method needs to create a new entity and set control values to its properties then persist it.
		/// </summary>
		/// <returns>returns id of the new entity.</returns>
		string Create();

		/// <summary>
		/// Update an existed entity from detail panel.
		/// The method needs to load an existed entity by specified id and set control values to overwrite its original properties then persist it.
		/// </summary>
		/// <param name="entityId"></param>
		void Update(string entityId);

		/// <summary>
		/// Reset all controls of the detail panel to initial state.
		/// The method will be invoked when enables the detail panel to support creating entities continuously.
		/// After an entity been created, the method will be invoked to reset form controls for another input.
		/// </summary>
		void Reset();

		/// <summary>
		/// The method is designed to load entity by id to editable detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		void LoadWritableEntity(string entityId);

		/// <summary>
		/// The method is designed to load entity by id to readonly detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		void LoadReadOnlyEntity(string entityId);

		/// <summary>
		/// The method will be invoked when the aggregate panel is initialized.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument.</param>
		void OnInit(IRequestHandler sender, DetailPanelPageEventArgs e);

		/// <summary>
		/// The method will be invoked when detail panel is loaded.
		/// The implementation can resolve current visiting Page from the conversation as (HttpContext.Current.Handler as Page).
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument.</param>
		void OnLoad(IRequestHandler sender, DetailPanelPageEventArgs e);

		/// <summary>
		/// The method will be invoked when the detail panel is prerendering.
		/// The implementation can resolve current visiting Page from the conversation as (HttpContext.Current.Handler as Page).
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web environment.</param>
		/// <param name="e">Callback event argument.</param>
		void OnPreRender(IRequestHandler sender, DetailPanelPageEventArgs e);
	}

	/// <summary>
	/// The event argument of aggregate panel page.
	/// </summary>
	public class DetailPanelPageEventArgs : EventArgs
	{
		/// <summary>
		/// Gets editing entity id. 
		/// If the detail panel is opened for creating a new entity, the property returns null.
		/// </summary>
		public string EntityId { get; private set; }

		/// <summary>
		/// Gets detail panel page modes.
		/// </summary>
		public DetailPanelPageRenderModes Mode { get; private set; }

		/// <summary>
		/// Construct event argument 
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="detailPanelPageMode"></param>
		public DetailPanelPageEventArgs(string entityId, DetailPanelPageRenderModes detailPanelPageMode)
		{
			this.EntityId = entityId;
			this.Mode = detailPanelPageMode;
		}

		/// <summary>
		/// Construct event argument when the detail panel is used for creating an entity.
		/// </summary>
		public DetailPanelPageEventArgs()
		{
			this.Mode = DetailPanelPageRenderModes.New;
		}
	}

	/// <summary>
	/// Detail panel page modes.
	/// </summary>
	public enum DetailPanelPageRenderModes
	{
		/// <summary>
		/// The detail panel is opened for creating a new entity.
		/// </summary>
		New,

		/// <summary>
		/// The detail panel is opened for editing an existed entity.
		/// </summary>
		Update,

		/// <summary>
		/// The detail panel is opened for viewing an existed entity.
		/// </summary>
		View
	}
}

