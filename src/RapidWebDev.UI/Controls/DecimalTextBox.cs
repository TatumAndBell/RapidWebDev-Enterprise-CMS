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
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// Ext Extended TextBox control to allow decimal input only by applying Ext uniform style.
	/// </summary>
	public class DecimalTextBox : System.Web.UI.WebControls.TextBox, IEditableControl, IQueryFieldControl
	{
		/// <summary>
		/// $ControlVariableName$
		/// </summary>
		private const string CONTROL_VARIABLE_NAME = "$ControlVariableName$";

		/// <summary>
		/// $ControlId$
		/// </summary>
		private const string CONTROL_ID = "$ControlId$";

		/// <summary>
		/// $AllowNegative$
		/// </summary>
		private const string ALLOW_NEGATIVE = "$AllowNegative$";

		/// <summary>
		/// $DecimalPrecision$
		/// </summary>
		private const string DECIMAL_PRECISION = "$DecimalPrecision$";

		/// <summary>
		/// The JavaScript block to transform ASP.NET textbox to ExtJS NumberField.
		/// </summary>
		private const string EXT_JAVASCRIPT_TEMPLATE = @"
			if (window.$ControlVariableName$ != undefined && window.$ControlVariableName$ != null) window.$ControlVariableName$.destroy();
			window.$ControlVariableName$ = new Ext.form.NumberField({ applyTo: Ext.DomQuery.select('#$ControlId$')[0], allowDecimals:true, allowNegative:$AllowNegative$, decimalPrecision:$DecimalPrecision$ });";

		/// <summary>
		/// Sets/gets decimal control value.
		/// </summary>
		public decimal? Value
		{
			get
			{
				if (string.IsNullOrEmpty(this.Text)) return null;
				decimal parsedValue;
				if (decimal.TryParse(this.Text, out parsedValue))
					return parsedValue;

				return null;
			}
			set { this.Text = value.HasValue ? value.ToString() : ""; }
		}

		/// <summary>
		/// False to prevent entering a negative sign (defaults to true) 
		/// </summary>
		public bool AllowNegative { get; set; }

		/// <summary>
		/// The maximum precision to display after the decimal separator (defaults to 2) 
		/// </summary>
		public int DecimalPrecision { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public DecimalTextBox()
		{
			this.AllowNegative = true;
			this.DecimalPrecision = 2;
		}

		#region IEditableControl Members

		object IEditableControl.Value
		{
			get { return this.Value; }
			set { this.Value = value as decimal?; }
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

			string javaScriptBlock = EXT_JAVASCRIPT_TEMPLATE.Replace(CONTROL_ID, this.ClientID)
				.Replace(ALLOW_NEGATIVE, this.AllowNegative.ToString().ToLowerInvariant())
				.Replace(DECIMAL_PRECISION, this.DecimalPrecision.ToString())
				.Replace(CONTROL_VARIABLE_NAME, this.ClientVariableName);

			ClientScripts.OnDocumentReady.Add2BeginOfBody(javaScriptBlock);
		}
	}
}

