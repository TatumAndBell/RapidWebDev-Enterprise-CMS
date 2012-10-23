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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// Extended ComboBox control
	/// </summary>
	public class ComboBox : System.Web.UI.WebControls.DropDownList, IEditableControl, IQueryFieldControl
	{
		// The JavaScript block to transform ASP.NET textbox to ExtJS style.
		private const string EXT_STATIC_DATASOURCE = @"
				if (window.$ControlVariableName$ != undefined && window.$ControlVariableName$ != null) window.$ControlVariableName$.destroy();

				window.$ControlVariableName$ = new Ext.form.ComboBox(
				{
					editable: $Editable$,
					typeAhead: true,
					triggerAction: 'all',
					transform: '$ControlId$',
					forceSelection: $ForceSelection$,
					minChars: $MinChars$,
					width: $Width$,
					disabled: $Disabled$,
					listeners:
					{
						change: function(field, newValue, oldValue)
						{
							if ($Editable$) { $SelectedItemChangedCallback.Container$ }
						},
						select: function(combobox, record, index)
						{
							if (!$Editable$) { var newValue = record.data.value; $SelectedItemChangedCallback.Container$ }
						}
					}
				});";

		private const string EXT_DYNAMIC_DATASOURCE = @"
				if (window.$ControlVariableName$ != undefined && window.$ControlVariableName$ != null) 
				{
					window.$ControlVariableName$.destroy();
					window.$ControlVariableName$ = null;
				}

				var localStore = new Ext.data.Store({
					$BaseParams.Container$
					proxy: $Proxy$,
					reader: new Ext.data.JsonReader({ root: '$Root$' }, [{ name: '$TextField$', mapping: '$TextField$' }, { name: '$ValueField$', mapping: '$ValueField$' }$ExtraFields.Container$])
				});

				if ($HasSelection$)
				{
					var defaultSelectedRecordClass = Ext.data.Record.create([{ name: '$TextField$', mapping: '$TextField$' }, { name: '$ValueField$', mapping: '$ValueField$' }]);
					var defaultSelectedRecord = new defaultSelectedRecordClass({ $TextField$: '$SelectedText$', $ValueField$: '$SelectedValue$' });
					localStore.add(defaultSelectedRecord);
				}

				window.$ControlVariableName$ = new Ext.form.ComboBox(
				{
					editable: $Editable$,
					typeAhead: true,
					triggerAction: 'all',
					transform: '$ControlId$',
					forceSelection: $ForceSelection$,
					store: localStore,
					$XTemplate.Container$
					displayField:'$TextField$',
					valueField: '$ValueField$',
					loadingText: '$LoadingText$',
					queryParam: '$QueryParam$',
					minChars: $MinChars$,
					width: $Width$,
					listWidth: $Width$,
					disabled: $Disabled$,
					listeners:
					{
						change: function(field, newValue, oldValue)
						{
							if ($Editable$) { $SelectedItemChangedCallback.Container$ }
						},
						select: function(combobox, record, index)
						{
							if (!$Editable$) { var newValue = record.data.$ValueField$; $SelectedItemChangedCallback.Container$ }
						}
					}
				});

				if ($HasSelection$)
					window.$ControlVariableName$.setValue('$SelectedValue$');";

		#region IEditableControl Members

		object IEditableControl.Value
		{
			get { return this.SelectedValue; }
			set { this.SelectedValue = value != null ? value.ToString() : null; }
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
		/// False to prevent the user from typing text directly into the field, just like a traditional select (defaults to true)
		/// </summary>
		public bool Editable { get; set; }

		/// <summary>
		/// True to restrict the selected value to one of the values in the list, false to allow the user to set arbitrary text into the field (defaults to true)
		/// </summary>
		public bool ForceSelection { get; set; }

		private int? minChars;
		/// <summary>
		/// The minimum number of characters the user must type before autocomplete and typeahead activate (defaults to 2 if remote or 0 if local, does not apply if editable = false)
		/// </summary>
		public int MinChars 
		{
			get 
			{
				if (this.minChars.HasValue) return this.minChars.Value;
				return this.Mode == ComboBoxDataSourceModes.Local ? 0 : 2;
			}
			set
			{
				this.minChars = value;
			}
		}

		/// <summary>
		/// Set to 'Local' if the ComboBox loads local data ('Remote' which loads from the server). The default value is Local.
		/// </summary>
		public ComboBoxDataSourceModes Mode { get; set; }

		/// <summary>
		/// The URL from which to load data through an HttpProxy. The response data should be in JSON format.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// The http access method, GET/POST, defaults to GET.
		/// </summary>
		public HttpMethods Method { get; set; }

		/// <summary>
		/// Note that if you are retrieving data from a page that is in a domain that is NOT the same as the originating domain of the running page, you must use ScriptTagProxy rather than HttpProxy.
		/// The content passed back from a server resource requested by a ScriptTagProxy must be executable JavaScript source code because it is used as the source inside a script tag.
		/// If you choose ScriptTagProxy, the http server MUST wrap the returned JSON string within "(JSON String);".
		/// Defaults to HttpProxy.
		/// </summary>
		public DataProxyTypes Proxy { get; set; }

		/// <summary>
		/// The name of the property which contains the Array of row objects in remote call result. 
		/// </summary>
		public string Root { get; set; }

		/// <summary>
		/// The underlying field name of displaying text to bind to this ComboBox.
		/// </summary>
		public string TextField { get; set; }

		/// <summary>
		/// The underlying field name of selection value to bind to this ComboBox.
		/// </summary>
		public string ValueField { get; set; }

		/// <summary>
		/// The extra fields can be used in XTemplate.
		/// </summary>
		public IEnumerable<string> ExtraFields { get; set; }

		/// <summary>
		/// Name of the query as it will be passed on the querystring (defaults to 'query').
		/// </summary>
		public string QueryParam { get; set; }

		/// <summary>
		/// The template string used to display each item in the dropdown list. Use this to create custom UI layouts for items in the list.
		/// The XTemplate is only applied when the mode is remote. 
		/// </summary>
		public string XTemplate { get; set; }

		/// <summary>
		/// This setting is required if a custom XTemplate has been specified which assigns a class other than ".x-combo-list-item" to dropdown list items.
		/// </summary>
		public string ItemSelector { get; set; }

		private string remoteSelectedValue;
		/// <summary>
		/// Gets the value of the selected item in the list control, or selects the item in the list control that contains the specified value.
		/// </summary>
		public override string SelectedValue
		{
			get
			{
				if (this.Mode == ComboBoxDataSourceModes.Local)
					return base.SelectedValue;
				else
					return this.remoteSelectedValue;
			}
			set
			{
				if (this.Mode == ComboBoxDataSourceModes.Local)
					base.SelectedValue = value;
				else
					this.remoteSelectedValue = value;
			}
		}

		private string remoteSelectedText;
		/// <summary>
		/// Gets the value of the selected item in the list control, or selects the item in the list control that contains the specified value.
		/// </summary>
		public string SelectedText
		{
			get
			{
				if (this.Mode == ComboBoxDataSourceModes.Local)
					return base.SelectedItem != null ? base.SelectedItem.Text : null;
				else
					return this.remoteSelectedText;
			}
			set
			{
				if (this.Mode == ComboBoxDataSourceModes.Remote)
					this.remoteSelectedText = value;
			}
		}

		/// <summary>
		/// Sets/gets client event when the selected item changed. 
		/// The signature of javascript callback method should be as: "void MethodName(newValue);"
		/// </summary>
		public string SelectedItemChangedCallback
		{
			get
			{
				if (Kit.IsEmpty(base.ViewState["SelectedItemChangedCallback"]))
					return null;

				return base.ViewState["SelectedItemChangedCallback"] as string;
			}
			set { base.ViewState["SelectedItemChangedCallback"] = value; }
		}

		/// <summary>
		/// An name-value pair which are to be sent as parameters on any HTTP request.
		/// </summary>
		public Collection<ComboBoxDynamicDataSourceParamConfiguration> Params { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public ComboBox()
		{
			this.Editable = true;
			this.ForceSelection = true;
			this.Mode = ComboBoxDataSourceModes.Local;
			this.QueryParam = "query";
			this.Params = new Collection<ComboBoxDynamicDataSourceParamConfiguration>();
		}

		/// <summary>
		/// Processes postback data for the System.Web.UI.WebControls.DropDownList control.
		/// </summary>
		/// <param name="postDataKey"></param>
		/// <param name="postCollection"></param>
		/// <returns></returns>
		protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			if (this.Mode == ComboBoxDataSourceModes.Local)
				return base.LoadPostData(postDataKey, postCollection);
			else
			{
				string[] values = postCollection.GetValues(postDataKey);
				this.EnsureDataBound();
				if (values != null)
					this.remoteSelectedValue = values[0];

				return false;
			}
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			bool disabled = !this.Enabled;
			int width = this.Width.IsEmpty ? 154 : (int)this.Width.Value;
			string javaScriptBlock = null;
			string selectedItemChangedCallbackContainer = null;
			if (!string.IsNullOrEmpty(this.SelectedItemChangedCallback))
				selectedItemChangedCallbackContainer = string.Format(@"if ({0}) {{ {0}(newValue); }}", this.SelectedItemChangedCallback);

			switch(this.Mode)
			{
				case ComboBoxDataSourceModes.Local:
					javaScriptBlock = EXT_STATIC_DATASOURCE.Replace("$ControlVariableName$", WebUtility.GenerateVariableName(this.ClientID))
						.Replace("$ControlId$", this.ClientID)
						.Replace("$Editable$", this.Editable.ToString().ToLowerInvariant())
						.Replace("$ForceSelection$", this.ForceSelection.ToString().ToLowerInvariant())
						.Replace("$MinChars$", this.MinChars.ToString())
						.Replace("$Width$", width.ToString())
						.Replace("$Disabled$", disabled.ToString().ToLowerInvariant())
						.Replace("$SelectedItemChangedCallback.Container$", selectedItemChangedCallbackContainer);
					break;

				case ComboBoxDataSourceModes.Remote:
					string baseParams = "";
					if (this.Params != null && this.Params.Count > 0)
					{
						baseParams = "baseParams: {";
						for (int i = 0; i < this.Params.Count; i++)
						{
							ComboBoxDynamicDataSourceParamConfiguration param = this.Params[i];
							if (i > 0) baseParams += ", ";
							if (param.Value != null)
							{
								string paramValue = WebUtility.ReplaceVariables(param.Value);
								baseParams += param.Name + ": '" + WebUtility.EncodeJavaScriptString(paramValue) + "'";
							}
						}

						baseParams += " },";
					}

					string xtemplateBlock = "";
					if (!string.IsNullOrEmpty(this.XTemplate))
						xtemplateBlock = string.Format(CultureInfo.InvariantCulture, "tpl: new Ext.XTemplate('{0}'), itemSelector: '{1}',", WebUtility.EncodeJavaScriptString(this.XTemplate), this.ItemSelector ?? "x-combo-list-item");

					string extraFieldsBlock = null;
					if (this.ExtraFields != null && this.ExtraFields.Count() > 0)
					{
						foreach (string extraField in this.ExtraFields)
							extraFieldsBlock += string.Format(CultureInfo.InvariantCulture, @", {{ name: '{0}', mapping: '{0}' }}", WebUtility.EncodeJavaScriptString(extraField));
					}

					bool hasSelection = !string.IsNullOrEmpty(this.remoteSelectedValue);
					string controlVariableName = WebUtility.GenerateVariableName(this.ClientID);
					javaScriptBlock = EXT_DYNAMIC_DATASOURCE.Replace("$ControlVariableName$", controlVariableName)
						.Replace("$ControlId$", this.ClientID)
						.Replace("$Editable$", this.Editable.ToString().ToLowerInvariant())
						.Replace("$ForceSelection$", this.ForceSelection.ToString().ToLowerInvariant())
						.Replace("$Proxy$", this.ResolveProxyString())
						.Replace("$Root$", WebUtility.EncodeJavaScriptString(this.Root))
						.Replace("$TextField$", WebUtility.EncodeJavaScriptString(this.TextField))
						.Replace("$ValueField$", WebUtility.EncodeJavaScriptString(this.ValueField))
						.Replace("$QueryParam$", WebUtility.EncodeJavaScriptString(this.QueryParam))
						.Replace("$LoadingText$", WebUtility.EncodeJavaScriptString(Resources.Ctrl_ComboBoxLoadingText))
						.Replace("$BaseParams.Container$", baseParams)
						.Replace("$MinChars$", this.MinChars.ToString())
						.Replace("$XTemplate.Container$", xtemplateBlock)
						.Replace("$ExtraFields.Container$", extraFieldsBlock)
						.Replace("$Width$", width.ToString())
						.Replace("$Disabled$", disabled.ToString().ToLowerInvariant())
						.Replace("$HasSelection$", hasSelection.ToString().ToLowerInvariant())
						.Replace("$SelectedValue$", this.remoteSelectedValue)
						.Replace("$SelectedText$", this.remoteSelectedText)
						.Replace("$SelectedItemChangedCallback.Container$", selectedItemChangedCallbackContainer);

					break;
			}

			ClientScripts.OnDocumentReady.Add2BeginOfBody(javaScriptBlock);
		}

		private string ResolveProxyString()
		{
			string dataSourceUrl = WebUtility.ReplaceVariables(this.Url);
			switch (this.Proxy)
			{
				case DataProxyTypes.HttpProxy:
					return "new Ext.data.HttpProxy({ url: '$Url$', method: '$Method$' })"
						.Replace("$Url$", WebUtility.EncodeJavaScriptString(dataSourceUrl))
						.Replace("$Method$", this.Method.ToString());

				case DataProxyTypes.ScriptTagProxy:
					return "new Ext.data.ScriptTagProxy({ url: '$Url$' })"
						.Replace("$Url$", WebUtility.EncodeJavaScriptString(dataSourceUrl));

				default:
					throw new NotSupportedException();
			}
		}
	}

	/// <summary>
	/// ComboBox datasource loading mode.
	/// </summary>
	public enum ComboBoxDataSourceModes
	{
		/// <summary>
		/// 'local' if the ComboBox loads local data
		/// </summary>
		Local,

		/// <summary>
		/// 'remote' which loads data from the server
		/// </summary>
		Remote
	}
}
