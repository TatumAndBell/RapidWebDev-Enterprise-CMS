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
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// Ext Extended TextBox control which applies Ext uniform style.
	/// </summary>
	public class TextBox : System.Web.UI.WebControls.TextBox, IEditableControl, IQueryFieldControl
	{
		/// <summary>
		/// The JavaScript block to transform ASP.NET textbox to ExtJS style.
		/// </summary>
		private const string EXT_JAVASCRIPT_TEMPLATE = @"
			if (window.$ControlVariableName$ != undefined && window.$ControlVariableName$ != null) { window.$ControlVariableName$.destroy(); }
			var textboxSearcher = $('#$ControlId$');
			var textboxStyleAttributeValue = textboxSearcher.attr('style');
			window.$ControlVariableName$ = new Ext.form.TextField({ applyTo: textboxSearcher[0], style: textboxStyleAttributeValue, cls: '$CssClass$' });";

		#region IEditableControl Members

		object IEditableControl.Value
		{
			get { return this.Text; }
			set { this.Text = value as string; }
		}

		#endregion

		#region IQueryFieldControl Members

		/// <summary>
		/// Auto assigned control index at runtime by infrastructure. 
		/// </summary>
		public int ControlIndex { get; set; }

		/// <summary>
		/// The client (JavaScript) variable names mapping to the query field which have the method "getValue" to get the client query field value. 
		/// The get values will be posted to server with control index for query ansynchronously and bind returned results to gridview control.
		/// </summary>
		public string ClientVariableName
		{
			get { return WebUtility.GenerateVariableName(this.ClientID); }
		}

		#endregion

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			string cssClass = !Kit.IsEmpty(this.CssClass) ? this.CssClass : "textboxShort";
			string javaScriptBlock = EXT_JAVASCRIPT_TEMPLATE.Replace("$ControlId$", this.ClientID)
				.Replace("$ControlVariableName$", this.ClientVariableName)
				.Replace("$CssClass$", cssClass);

			ClientScripts.OnDocumentReady.Add2BeginOfBody(javaScriptBlock);
		}
	}
}
