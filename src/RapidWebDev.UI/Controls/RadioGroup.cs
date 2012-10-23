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
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// Extended RadioGroup control
	/// </summary>
	public class RadioGroup : System.Web.UI.WebControls.RadioButtonList, IEditableControl, IQueryFieldControl
	{
		private const string CONTROL_JAVASCRIPT_CLASS = @"
			function RadioGroupClass(radioGroupId, defaultValue)
			{
				this.getValue = function()
				{
					var selectedIndex = -1;
					var index = 0;
					while (true)
					{
						var radioId = '#' + radioGroupId + '_' + index.toString();
						var radioSearcher = $(radioId);
						if (radioSearcher.length != 1) break;

						var isChecked = radioSearcher.attr('checked');
						if (isChecked)
						{
							selectedIndex = index;
							return selectedIndex;
						}

						index++;
					}

					return selectedIndex;
				}

				this.setValue = function(value)
				{
					var selectedRadioIndex = value != null ? value.toString() : '';
					var index = 0;
					while (true)
					{
						var radioId = '#' + radioGroupId + '_' + index.toString();
						var radioSearcher = $(radioId);
						if (radioSearcher.length != 1) break;

						radioSearcher.attr('checked', index.toString() == selectedRadioIndex);
						index++;
					}
				}

				this.reset = function()
				{
					this.setValue(defaultValue);
				}
			}";

		#region IEditableControl Members

		object IEditableControl.Value
		{
			get { return this.SelectedValue; }
			set { this.SelectedValue = value as string; }
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
		/// Handles the System.Web.UI.Control.Init event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.CssClass = "radiogroup";
			this.RepeatDirection = RepeatDirection.Horizontal;
			this.RepeatLayout = RepeatLayout.Table;
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			ClientScripts.RegisterScriptBlock(CONTROL_JAVASCRIPT_CLASS);

			string controlJavaScriptInstance = string.Format(CultureInfo.InvariantCulture, "window.{0} = new RadioGroupClass('{1}', {2});", this.ClientVariableName, this.ClientID, this.SelectedIndex);
			ClientScripts.RegisterScriptBlock(controlJavaScriptInstance);
		}
	}
}

