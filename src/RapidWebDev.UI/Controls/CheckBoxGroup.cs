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
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// Extended CheckBox control
	/// </summary>
	public class CheckBoxGroup : System.Web.UI.WebControls.CheckBoxList, IEditableControl, IQueryFieldControl
	{
		private const string CONTROL_JAVASCRIPT_CLASS = @"
			function CheckBoxGroupClass(checkBoxGroupId, defaultValue)
			{
				this.getValue = function()
				{
					var selectedIndexString = '';
					var index = 0;
					while (true)
					{
						var checkBoxId = '#' + checkBoxGroupId + '_' + index.toString();
						var checkBoxSearcher = $(checkBoxId);
						if (checkBoxSearcher.length != 1) break;

						var isChecked = checkBoxSearcher.attr('checked');
						if (isChecked)
						{
							if (selectedIndexString.length > 0) selectedIndexString += '$ARRAY_ITEM_SEPARATOR$';
							selectedIndexString += index;
						}

						index++;
					}

					return selectedIndexString;
				}

				this.setValue = function(value)
				{
					var checkBoxValues = new Array();
					if (value != null) 
					{
						if (value instanceof Array) checkBoxValues = value;
						else checkBoxValues = value.split('$ARRAY_ITEM_SEPARATOR$');
					}

					var index = 0;
					while (true)
					{
						var checkBoxId = '#' + checkBoxGroupId + '_' + index.toString();
						var checkBoxSearcher = $(checkBoxId);
						if (checkBoxSearcher.length != 1) break;

						checkBoxSearcher.attr('checked', checkBoxValues.indexOf(index.toString()) > -1);
						index++;
					}
				}

				this.reset = function()
				{
					this.setValue(defaultValue);
				}
			}";

		/// <summary>
		/// Sets/gets enumerable checked item values.
		/// </summary>
		public IEnumerable<string> CheckedValues
		{
			get
			{
				List<string> checkedValues = new List<string>();
				foreach (ListItem listItem in this.Items)
					if (listItem.Selected)
						checkedValues.Add(listItem.Value);

				return checkedValues;
			}
			set
			{
				if (this.Items != null)
				{
					foreach (ListItem listItem in this.Items)
						listItem.Selected = value != null && value.Contains(listItem.Value);
				}
			}
		}

		#region IEditableControl Members

		object IEditableControl.Value
		{
			get { return this.CheckedValues; }
			set { this.CheckedValues = value as IEnumerable<string>; }
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

			this.CssClass = "checkboxgroup";
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

			ClientScripts.RegisterScriptBlock(CONTROL_JAVASCRIPT_CLASS.Replace("$ARRAY_ITEM_SEPARATOR$", WebUtility.ARRAY_ITEM_SEPARATOR));

			List<string> selectedIndexes = new List<string>();
			for (int i = 0; i < this.Items.Count; i++)
				if (this.Items[i].Selected) selectedIndexes.Add(i.ToString());

			string defaultValue = selectedIndexes.Concat(WebUtility.ARRAY_ITEM_SEPARATOR);
			string controlJavaScriptInstance = string.Format(CultureInfo.InvariantCulture, "window.{0} = new CheckBoxGroupClass('{1}', '{2}');", this.ClientVariableName, this.ClientID, defaultValue);
			ClientScripts.RegisterScriptBlock(controlJavaScriptInstance);
		}
	}
}

