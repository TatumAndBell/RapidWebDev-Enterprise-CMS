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
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// HierarchySelector control
	/// </summary>
	public class HierarchySelector : System.Web.UI.WebControls.WebControl, IEditableControl, IQueryFieldControl, INamingContainer, IPostBackDataHandler
	{
		private readonly static JavaScriptSerializer serializer = new JavaScriptSerializer();

		/// <summary>
		/// Sets/gets the selected hierarchy items.
		/// </summary>
		public IEnumerable<HierarchyItem> SelectedItems { get; set; }

		/// <summary>
		/// Sets/gets the title displayed on the UI, which indicates for selection. Like "Select Area" for geography hierarchy data.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Sets/gets the hierarchy service Url to pull hierarchy data collection.
		/// </summary>
		public string ServiceUrl { get; set; }

		/// <summary>
		/// Sets/gets field Name of hierarchy data text.
		/// </summary>
		public string TextField { get; set; }

		/// <summary>
		/// Sets/gets field Name of hierarchy data value.
		/// </summary>
		public string ValueField { get; set; }

		/// <summary>
		/// Sets/gets field name indicates which hierarchy node is the parent in the returned array from the service.
		/// </summary>
		public string ParentValueField { get; set; }

		/// <summary>
		/// Sets/gets width of modal dialog to select hierarchy data in pixel, defaults to 480.
		/// </summary>
		public int ModalDialogWidth { get; set; }

		/// <summary>
		/// Sets/gets height of modal dialog to select hierarchy data in pixel, defaults to 320.
		/// </summary>
		public int ModalDialogHeight { get; set; }

		/// <summary>
		/// Hierarchy tree nodes checking cascading type, defaults to Full.
		/// </summary>
		public TreeNodeCheckCascadingTypes Cascading { get; set; }

		#region IEditableControl Members

		object IEditableControl.Value
		{
			get { return this.SelectedItems; }
			set { this.SelectedItems = value as IEnumerable<HierarchyItem>; }
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

		#region IPostBackDataHandler Members

		bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			string postValue = postCollection[postDataKey];
			if (!string.IsNullOrEmpty(postValue))
				this.SelectedItems = serializer.Deserialize<List<HierarchyItem>>(postValue);
			else
				this.SelectedItems = new List<HierarchyItem>();

			return true;
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{
		}

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public HierarchySelector()
		{
			this.Width = new Unit(154, UnitType.Pixel);
			this.ModalDialogHeight = 320;
			this.ModalDialogWidth = 480;
			this.Cascading = TreeNodeCheckCascadingTypes.Full;
			this.Title = Resources.DPCtrl_Selector;
		}

		/// <summary>
		/// Set platform HierarchyType of RapidWebDev.
		/// </summary>
		/// <param name="hierarchyType"></param>
		public void SetHierarchyType(string hierarchyType)
		{
			this.ServiceUrl = Kit.ResolveAbsoluteUrl(string.Format(CultureInfo.InvariantCulture, "~/services/HierarchyService.svc/json/GetAllHierarchyData/{0}", hierarchyType));
			this.TextField = "Name";
			this.ValueField = "HierarchyDataId";
			this.ParentValueField = "ParentHierarchyDataId";
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			const string JavaScriptDeclarationTemplate = "window.{0}=new HierarchySelector('{1}', '{2}', '{1}_container', {3}, {4}, {5}, {6});";

			string dataSourceParameter = this.CreateDataSourceSchemaParameterJson();
			string globalizationParameter = this.CreateGlobalizationParameterJson();
			string optionsParameter = this.CreateOptionsParameterJson();
			string selectionParameter = this.CreateSelectionParameterJson();

			string javaScript = string.Format(CultureInfo.InvariantCulture, JavaScriptDeclarationTemplate, this.ClientVariableName, this.ClientID, this.UniqueID, dataSourceParameter, globalizationParameter, optionsParameter, selectionParameter);
			ClientScripts.RegisterScriptBlock(javaScript);
			ClientScripts.RegisterHeaderScriptInclude("~/resources/javascript/HierarchySelector.js");
		}

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The System.Web.UI.HtmlTextWriter object that receives the control content.</param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render(writer);

			writer.Write(@"<div id=""{0}_container"" class=""hierarchySelectorContainer""></div>", this.ClientID);
		}

		private string CreateDataSourceSchemaParameterJson()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				jsonWriter.WriteStartObject();

				jsonWriter.WritePropertyName("Url");
				jsonWriter.WriteValue(this.ServiceUrl);

				jsonWriter.WritePropertyName("TextField");
				jsonWriter.WriteValue(this.TextField);

				jsonWriter.WritePropertyName("ValueField");
				jsonWriter.WriteValue(this.ValueField);

				jsonWriter.WritePropertyName("ParentValueField");
				jsonWriter.WriteValue(this.ParentValueField);

				jsonWriter.WriteEndObject();
			}

			return jsonBuilder.ToString();
		}

		private string CreateGlobalizationParameterJson()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				jsonWriter.WriteStartObject();

				jsonWriter.WritePropertyName("LoadRemoteDataFailed");
				jsonWriter.WriteValue(Resources.DTCtrl_LoadRemoteDataFailed);

				jsonWriter.WritePropertyName("Loading");
				jsonWriter.WriteValue(Resources.DPCtrl_LoadingText);

				jsonWriter.WritePropertyName("Title");
				jsonWriter.WriteValue(this.Title);

				jsonWriter.WriteEndObject();
			}

			return jsonBuilder.ToString();
		}

		private string CreateOptionsParameterJson()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				jsonWriter.WriteStartObject();

				int width = 480;
				if (this.Width != Unit.Empty && this.Width.Type == UnitType.Pixel)
					width = (int)this.Width.Value;

				jsonWriter.WritePropertyName("Width");
				jsonWriter.WriteValue(width);

				jsonWriter.WritePropertyName("Enabled");
				jsonWriter.WriteValue(this.Enabled);

				jsonWriter.WritePropertyName("ModalDialogWidth");
				jsonWriter.WriteValue(this.ModalDialogWidth);

				jsonWriter.WritePropertyName("ModalDialogHeight");
				jsonWriter.WriteValue(this.ModalDialogHeight);

				jsonWriter.WritePropertyName("Cascading");
				jsonWriter.WriteValue(this.Cascading.ToString().ToLower(CultureInfo.InvariantCulture));

				jsonWriter.WriteEndObject();
			}

			return jsonBuilder.ToString();
		}

		private string CreateSelectionParameterJson()
		{
			if (this.SelectedItems == null || this.SelectedItems.Count() == 0) return "[]";
			return serializer.Serialize(this.SelectedItems.ToList());
		}
	}

	/// <summary>
	/// Hierarchy item
	/// </summary>
	[Serializable]
	public class HierarchyItem
	{
		/// <summary>
		/// Id
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public HierarchyItem()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		public HierarchyItem(Guid id, string name)
		{
			this.Id = id;
			this.Name = name;
		}
	}
}
