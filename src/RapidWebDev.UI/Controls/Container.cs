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
using System.Globalization;
using System.Web.UI.WebControls;
using RapidWebDev.Common.Web;

namespace RapidWebDev.UI.Controls
{
    /// <summary>
    /// Content Container
    /// </summary>
    public class Container : Panel
	{
		#region JAVASCRIPT Block

		/// <summary>
		/// Ext javascript block to format the rendered button to Ext style.
		/// </summary>
		private const string EXT_FORMATTER_TEMPLATE = @"
						if (window.$ControlID$Accessor != undefined && window.$ControlID$Accessor != null) 
							window.$ControlID$Accessor.destroy();

						window.$ControlID$Accessor = new Ext.Panel({
							id: '$ControlExtJsID$',
							title: '$HeaderText$',
							defaultType: 'box',
							collapsed: $Collapsed$,
							collapsible: $Collapsible$,
							frame: $Frame$,
							autoHeight: true,
							titleCollapse: true,
							items: new Ext.BoxComponent({ el: '$ControlID$' }),
							$Width.Container$
							listeners:
							{
								statesave: function(component, state) 
								{ 
									if (window.cookieProvider != undefined && window.cookieProvider != null) 
										window.cookieProvider.set(component.id, component.collapsed); 
								}
							}
						});
	
						var container = Ext.DomQuery.select('#$ControlID$')[0];
						window.$ControlID$Accessor.render(container.parentNode, container);
						if (window.cookieProvider != undefined && window.cookieProvider != null) 
						{
							var isCollapsed = window.cookieProvider.get(window.$ControlID$Accessor.id, window.$ControlID$Accessor.collapsed);
							if (isCollapsed ^ window.$ControlID$Accessor.collapsed)
								isCollapsed ? window.$ControlID$Accessor.collapse() : window.$ControlID$Accessor.expand();
						}";

		#endregion

		/// <summary>
		/// True to render the panel with custom rounded borders, false to render with plain 1px square borders (defaults to true).
        /// </summary>
        public bool Frame { get; set; }

		/// <summary>
		/// True to make the panel collapsible and have the expand/collapse toggle button automatically rendered into the header tool button area, false to keep the panel statically sized with no button (defaults to false).
		/// </summary>
		public bool Collapsible { get; set; }

		/// <summary>
		/// True to render the panel collapsed, false to render it expanded (defaults to false). 
		/// </summary>
		public bool Collapsed { get; set; }

		/// <summary>
		/// The title text to display in the panel header (defaults to '').
		/// </summary>
		public string HeaderText { get; set; }

		/// <summary>
		/// A flag which causes the Container to attempt to restore the collapse state from a saved state in cookie on startup.
		/// </summary>
		public bool Stateful { get; set; }

        /// <summary>
        /// Get html tag name of this container.
        /// </summary>
        protected override string TagName { get { return "div"; } }

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			string width = "";
			if (this.Width != Unit.Empty && this.Width.Type == UnitType.Pixel)
				width = string.Format(CultureInfo.InvariantCulture, "width: {0},", (int)this.Width.Value);

			string renderedJavaScript = EXT_FORMATTER_TEMPLATE.Replace("$ControlID$", this.ClientID)
				.Replace("$ControlExtJsID$", this.ClientID.Replace("_", ""))
				.Replace("$Collapsible$", this.Collapsible.ToString().ToLowerInvariant())
				.Replace("$Collapsed$", this.Collapsed.ToString().ToLowerInvariant())
				.Replace("$Frame$", this.Frame.ToString().ToLowerInvariant())
				.Replace("$HeaderText$", WebUtility.EncodeJavaScriptString(this.HeaderText))
				.Replace("$Width.Container$", width);

			ClientScripts.OnDocumentReady.Add2EndOfBody(renderedJavaScript);
		}
    }
}
