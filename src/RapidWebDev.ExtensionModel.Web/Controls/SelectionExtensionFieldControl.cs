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
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common.Web;
using RapidWebDev.ExtensionModel.Web.Properties;
using Newtonsoft.Json;

namespace RapidWebDev.ExtensionModel.Web.Controls
{
	/// <summary>
	/// Control to manage selection field metadata.
	/// </summary>
	public class SelectionExtensionFieldControl : WebControl, INamingContainer, IPostBackDataHandler, ITextControl
	{
		private static JavaScriptSerializer serializer = new JavaScriptSerializer();
		private const string JAVASCRIPT_TEMPLATE = @"
				if (window.{0}Variable) window.{0}Variable.destroy();
				window.{0}Variable = new SelectionFieldMetadataControl('{0}', '{1}', {2}, {3});";

		/// <summary>
		/// Defined selection items.
		/// </summary>
		public IEnumerable<SelectionItem> SelectionItems { get; set; }

		/// <summary>
		/// Register the control to the page for requiring postback.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.Page.RegisterRequiresPostBack(this);
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			string javaScriptBlock = string.Format(CultureInfo.InvariantCulture, JAVASCRIPT_TEMPLATE,
				this.ClientID,
				this.UniqueID,
				this.CreateDataSourceJson(),
				this.CreateOptionsJson());

			ClientScripts.RegisterScriptBlock(javaScriptBlock);
			ClientScripts.RegisterHeaderScriptInclude("~/resources/javascript/SelectionFieldMetadataControl.js");
		}

		private string CreateDataSourceJson()
		{
			if (this.SelectionItems == null) return "[]";

			return serializer.Serialize(this.SelectionItems);
		}

		private string CreateOptionsJson()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				jsonWriter.WriteStartObject();

				jsonWriter.WritePropertyName("RowEditorSaveText");
				jsonWriter.WriteValue(Resources.Save);

				jsonWriter.WritePropertyName("RowEditorCancelText");
				jsonWriter.WriteValue(Resources.Cancel);

				jsonWriter.WritePropertyName("SelectionItemTextColumnHeader");
				jsonWriter.WriteValue(Resources.Text);

				jsonWriter.WritePropertyName("SelectionItemValueColumnHeader");
				jsonWriter.WriteValue(Resources.Value);

				jsonWriter.WritePropertyName("SelectionItemSelectedColumnHeader");
				jsonWriter.WriteValue(Resources.Selected);

				jsonWriter.WritePropertyName("AddItem");
				jsonWriter.WriteValue(Resources.AddItem);

				jsonWriter.WritePropertyName("RemoveItemText");
				jsonWriter.WriteValue(Resources.RemoveItem);

				jsonWriter.WritePropertyName("YesText");
				jsonWriter.WriteValue(Resources.Yes);

				jsonWriter.WritePropertyName("NoText");
				jsonWriter.WriteValue(Resources.No);

				int width = 480;
				if (this.Width != Unit.Empty && this.Width.Type == UnitType.Pixel)
					width = (int)this.Width.Value;

				jsonWriter.WritePropertyName("Width");
				jsonWriter.WriteValue(width);

				int height = 240;
				if (this.Height != Unit.Empty && this.Height.Type == UnitType.Pixel)
					height = (int)this.Height.Value;

				jsonWriter.WritePropertyName("Height");
				jsonWriter.WriteValue(height);

				jsonWriter.WritePropertyName("Disabled");
				jsonWriter.WriteValue(!this.Enabled);

				jsonWriter.WriteEndObject();
			}

			return jsonBuilder.ToString();
		}

		#region IPostBackDataHandler Members

		bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			string postValue = postCollection[postDataKey];
			if (!string.IsNullOrEmpty(postValue))
				this.SelectionItems = serializer.Deserialize<IEnumerable<SelectionItem>>(postValue);

			return true;
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{
		}

		#endregion

		#region ITextControl Members

		string ITextControl.Text { get; set; }

		#endregion
	}

	/// <summary>
	/// Select item for SelectionExtensionFieldControl.
	/// </summary>
	public class SelectionItem
	{
		/// <summary>
		/// Select item text.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Select item value.
		/// </summary>
		public string Value { get; set; }
		
		/// <summary>
		/// Default selection status.
		/// </summary>
		public bool Selected { get; set; }
	}
}
