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
using RapidWebDev.Common.Globalization;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// Control to pick up date.
	/// </summary>
	public class DatePicker : System.Web.UI.WebControls.TextBox, IEditableControl, IQueryFieldControl
	{
		/// <summary>
		/// The JavaScript block to transform ASP.NET textbox to ExtJS style.
		/// </summary>
		private const string EXT_JAVASCRIPT_TEMPLATE = @"
				if (window.$ControlVariableName$ != undefined && window.$ControlVariableName$ != null) window.$ControlVariableName$=null;
				window.$ControlVariableName$ = new Ext.form.DateField({ applyTo: '$ControlID$', $MinValueContainer$$MaxValueContainer$format: '$DateFormatString$', disabled: $Disabled$, width: $Width$ });";

		/// <summary>
		/// The maximum allowed date.
		/// </summary>
		public DateTime? MaxValue { get; set; }

		/// <summary>
		/// The minimum allowed date.
		/// </summary>
		public DateTime? MinValue { get; set; }

		/// <summary>
		/// Selected datetime value.
		/// </summary>
		public DateTime? SelectedValue
		{
			get
			{
				DateTime returnedValue;
				if (DateTime.TryParse(this.Text, out returnedValue))
					return returnedValue;

				return null;
			}
			set
			{
				DateTime dateTime = DateTime.MinValue;
				if (value is DateTime? && value != null)
					dateTime = (value as DateTime?).Value;
				else if (value is DateTime)
					dateTime = (DateTime)value;

				DateTime clientDateTime = LocalizationUtility.ConvertUtcTimeToClientTime(dateTime);
				this.Text = clientDateTime.ToString("yyyy-MM-dd");
			}
		}

		#region IEditableControl Members

		object IEditableControl.Value
		{
			get 
			{
				DateTime returnedValue;
				if (DateTime.TryParse(this.Text, out returnedValue))
					return returnedValue;

				return null;
			}
			set 
			{
				DateTime dateTime = DateTime.MinValue;
				if (value is DateTime? && value != null)
					dateTime = (value as DateTime?).Value;
				else if (value is DateTime)
					dateTime = (DateTime)value;

				this.Text = LocalizationUtility.ToDateString(dateTime, DateTime.MinValue);
			}
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

			string maxValueContainer = "";
			if (this.MaxValue.HasValue)
				maxValueContainer = string.Format(CultureInfo.InvariantCulture, "maxValue: '{0}', ", this.MaxValue.Value.ToString("yyyy-MM-dd"));

			string minValueContainer = "";
			if (this.MinValue.HasValue)
				minValueContainer = string.Format(CultureInfo.InvariantCulture, "minValue: '{0}', ", this.MinValue.Value.ToString("yyyy-MM-dd"));

			int width = 138;
			if (this.Width != Unit.Empty && this.Width.Type == UnitType.Pixel)
				width = (int)this.Width.Value;

			string javaScriptBlock = EXT_JAVASCRIPT_TEMPLATE.Replace("$ControlID$", this.ClientID)
				.Replace("$ControlVariableName$", this.ClientVariableName)
				.Replace("$DateFormatString$", Resources.Ctrl_DatePicker_DateFormatString)
				.Replace("$MaxValueContainer$", maxValueContainer)
				.Replace("$MinValueContainer$", minValueContainer)
				.Replace("$Disabled$", (!this.Enabled || this.ReadOnly).ToString().ToLowerInvariant())
				.Replace("$Width$", width.ToString(CultureInfo.InvariantCulture));

			ClientScripts.OnDocumentReady.Add2BeginOfBody(javaScriptBlock);
		}
	}
}

