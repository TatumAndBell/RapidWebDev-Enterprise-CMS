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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;
using RapidWebDev.UI.Services;
using Newtonsoft.Json;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// GridView Panel used to display query results by query panel.
	/// The class inherits from ASP.NET Panel and uses ExtJS to render a pure AJAX Grid into the panel.
	/// </summary>
	public class GridViewPanel : System.Web.UI.WebControls.Panel, INamingContainer
	{
		/// <summary>
		/// Sets/gets configuration to render the GridView panel.
		/// </summary>
		public GridViewPanelConfiguration Configuration { get; set; }

		/// <summary>
		/// Plugin configuration of grid view panel for rendering detail panel in dynamic page. 
		/// </summary>
		public GridViewPanelPluginConfiguration4DetailPanel DetailPanelPlugin { get; set; }

		/// <summary>
		/// Plugin configuration of grid view panel for rendering aggregate panel in dynamic page. 
		/// </summary>
		public GridViewPanelPluginConfiguration4AggregatePanel AggregatePanelPlugin { get; set; }

		/// <summary>
		/// Whether the grid panel is unique in the window which can be controlled by query panel or popup window for record detail information.
		/// </summary>
		public bool AsUniqueGridView { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public GridViewPanel()
		{
			this.DetailPanelPlugin = new GridViewPanelPluginConfiguration4DetailPanel();
			this.AggregatePanelPlugin = new GridViewPanelPluginConfiguration4AggregatePanel();
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			if (this.Configuration == null)
				throw new ConfigurationErrorsException(Resources.Ctrl_PropertyConfigurationNotSpecified);

			base.CreateChildControls();
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			string variableName = string.Format(CultureInfo.InvariantCulture, "window.{0}__Proxy", this.ClientID);
			string javaScriptBlock = string.Format(CultureInfo.InvariantCulture, "{0} = new GridViewPanelClass('{0}', {1}, {2}, {3}, {4}, {5});",
				variableName,
				this.CreateGridConfigJson(),
				this.CreateDetailPanelModalWindowConfigJson(),
				this.CreateAggregatePanelModalWindowConfigJson(),
				this.CreatStoreConfigJson(),
				this.CreateGlobalizationResourcesJson());

			if (this.AsUniqueGridView)
				javaScriptBlock += string.Format(CultureInfo.InvariantCulture, "window.RegisteredGridViewPanelObject = {0};", variableName);

			ClientScripts.RegisterScriptBlock(javaScriptBlock);

			if (this.Configuration.ExecuteQueryWhenLoaded)
			{
				string javaScriptToExecuteDefaultQuery = string.Format(CultureInfo.InvariantCulture, "{0}.ExecuteQuery();", variableName);
				ClientScripts.OnDocumentReady.Add2EndOfBody(javaScriptToExecuteDefaultQuery, JavaScriptPriority.Low);
			}
		}

		/// <summary>
		/// Create JSON string for argument _gridConfig of JavaScript GridViewPanelClass constructor.
		/// </summary>
		/// <returns></returns>
		private string CreateGridConfigJson()
		{
			StringBuilder jsonStringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonStringBuilder))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartObject();

				// .ClientId
				jsonTextWriter.WritePropertyName("ClientId");
				jsonTextWriter.WriteValue(this.ClientID);

				// .ViewConfig
				if (this.Configuration.RowView != null)
				{
					// create view config instance
					jsonTextWriter.WritePropertyName("ViewConfig");
					jsonTextWriter.WriteStartObject();

					string viewFragmentFieldName = FieldNameTransformUtility.ViewFragmentFieldName(this.Configuration.RowView.FieldName);
					jsonTextWriter.WritePropertyName("FieldName");
					jsonTextWriter.WriteValue(viewFragmentFieldName);

					jsonTextWriter.WritePropertyName("TagName");
					jsonTextWriter.WriteValue(this.Configuration.RowView.TagName ?? "p");

					if (!string.IsNullOrEmpty(this.Configuration.RowView.Css))
					{
						jsonTextWriter.WritePropertyName("Css");
						jsonTextWriter.WriteValue(this.Configuration.RowView.Css);
					}

					jsonTextWriter.WriteEndObject();
				}

				// .HeaderText
				jsonTextWriter.WritePropertyName("HeaderText");
				jsonTextWriter.WriteValue(this.Configuration.HeaderText);

				// .Height
				jsonTextWriter.WritePropertyName("Height");
				jsonTextWriter.WriteValue(this.Configuration.Height);

				// .ShowCheckBoxColumn
				jsonTextWriter.WritePropertyName("ShowCheckBoxColumn");
				jsonTextWriter.WriteValue(this.Configuration.EnabledCheckBoxField);

				#region Create JSON for Edit/View/Delete button

				// .ShowEditButton
				if (this.Configuration.EditButton != null)
				{
					// create edit button config instance
					jsonTextWriter.WritePropertyName("ShowEditButton");
					jsonTextWriter.WriteStartObject();

					// .DisplayAsImage
					jsonTextWriter.WritePropertyName("DisplayAsImage");
					jsonTextWriter.WriteValue(this.Configuration.EditButton.DisplayAsImage);

					// .Text
					jsonTextWriter.WritePropertyName("Text");
					jsonTextWriter.WriteValue(Resources.DPCtrl_EditText);

					// .ToolTip
					jsonTextWriter.WritePropertyName("ToolTip");
					jsonTextWriter.WriteValue(this.Configuration.EditButton.ToolTip ?? "");

					jsonTextWriter.WriteEndObject();
				}

				// .ShowViewButton
				if (this.Configuration.ViewButton != null)
				{
					// create view button config instance
					jsonTextWriter.WritePropertyName("ShowViewButton");
					jsonTextWriter.WriteStartObject();

					// .DisplayAsImage
					jsonTextWriter.WritePropertyName("DisplayAsImage");
					jsonTextWriter.WriteValue(this.Configuration.ViewButton.DisplayAsImage);

					// .Text
					jsonTextWriter.WritePropertyName("Text");
					jsonTextWriter.WriteValue(Resources.DPCtrl_ViewText);

					// .ToolTip
					jsonTextWriter.WritePropertyName("ToolTip");
					jsonTextWriter.WriteValue(this.Configuration.ViewButton.ToolTip ?? "");

					jsonTextWriter.WriteEndObject();
				}

				// .ShowDeleteButton
				if (this.Configuration.DeleteButton != null)
				{
					// create delete button config instance
					jsonTextWriter.WritePropertyName("ShowDeleteButton");
					jsonTextWriter.WriteStartObject();

					// .DisplayAsImage
					jsonTextWriter.WritePropertyName("DisplayAsImage");
					jsonTextWriter.WriteValue(this.Configuration.DeleteButton.DisplayAsImage);

					// .Text
					jsonTextWriter.WritePropertyName("Text");
					jsonTextWriter.WriteValue(Resources.DPCtrl_DeleteText);

					// .ToolTip
					jsonTextWriter.WritePropertyName("ToolTip");
					jsonTextWriter.WriteValue(this.Configuration.DeleteButton.ToolTip ?? "");

					jsonTextWriter.WriteEndObject();
				}

				#endregion

				#region Create JSON for grid fields

				if (this.Configuration.Fields != null && this.Configuration.Fields.Count > 0)
				{
					jsonTextWriter.WritePropertyName("Columns");
					jsonTextWriter.WriteStartArray();

					for (int fieldIndex = 0; fieldIndex < this.Configuration.Fields.Count; fieldIndex++)
					{
						GridViewFieldConfiguration fieldConfig = this.Configuration.Fields[fieldIndex];
						jsonTextWriter.WriteStartObject();

						if (!string.IsNullOrEmpty(fieldConfig.ExtJsRenderer))
						{
							// .Renderer
							jsonTextWriter.WritePropertyName("Renderer");
							jsonTextWriter.WriteValue(fieldConfig.ExtJsRenderer);
						}

						// .FieldName
						string clientDataBoundName = FieldNameTransformUtility.DataBoundFieldName(fieldConfig.FieldName, fieldIndex);
						jsonTextWriter.WritePropertyName("FieldName");
						jsonTextWriter.WriteValue(clientDataBoundName);

						// .HeaderText
						jsonTextWriter.WritePropertyName("HeaderText");
						jsonTextWriter.WriteValue(fieldConfig.HeaderText);

						// .Sortable
						jsonTextWriter.WritePropertyName("Sortable");
						jsonTextWriter.WriteValue(fieldConfig.Sortable);

						// .Resizable
						jsonTextWriter.WritePropertyName("Resizable");
						jsonTextWriter.WriteValue(fieldConfig.Resizable);

						if (!string.IsNullOrEmpty(fieldConfig.Css))
						{
							// .Css
							jsonTextWriter.WritePropertyName("Css");
							jsonTextWriter.WriteValue(fieldConfig.Css);
						}

						if (fieldConfig.Width.HasValue)
						{
							// .Width
							jsonTextWriter.WritePropertyName("Width");
							jsonTextWriter.WriteValue(fieldConfig.Width.Value);
						}

						if (fieldConfig.Align != HorizontalAlign.NotSet && fieldConfig.Align != HorizontalAlign.Justify)
						{
							// .Align
							jsonTextWriter.WritePropertyName("Align");
							jsonTextWriter.WriteValue(fieldConfig.Align.ToString().ToLowerInvariant());
						}

						// .Hidden
						jsonTextWriter.WritePropertyName("Hidden");
						jsonTextWriter.WriteValue(fieldConfig.Hidden);

						jsonTextWriter.WriteEndObject();
					}

					jsonTextWriter.WriteEndArray();
				}

				#endregion

				jsonTextWriter.WriteEnd();
			}

			return jsonStringBuilder.ToString();
		}

		/// <summary>
		/// Create JSON string for argument _JsonStoreConfig of JavaScript GridViewPanelClass constructor.
		/// </summary>
		/// <returns></returns>
		private string CreatStoreConfigJson()
		{
			StringBuilder jsonStringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonStringBuilder))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartObject();

				jsonTextWriter.WritePropertyName("Root");
				jsonTextWriter.WriteValue("Records");

				jsonTextWriter.WritePropertyName("TotalProperty");
				jsonTextWriter.WriteValue("TotalRecordCount");

				jsonTextWriter.WritePropertyName("IdProperty");
				jsonTextWriter.WriteValue(FieldNameTransformUtility.PrimaryKeyFieldName(this.Configuration.PrimaryKeyFieldName));

				jsonTextWriter.WritePropertyName("PageSize");
				jsonTextWriter.WriteValue(this.Configuration.PageSize);

				if (!string.IsNullOrEmpty(this.Configuration.DefaultSortField))
				{
					jsonTextWriter.WritePropertyName("DefaultSortField");
					jsonTextWriter.WriteValue(this.Configuration.DefaultSortField);

					jsonTextWriter.WritePropertyName("DefaultSortDirection");
					jsonTextWriter.WriteValue(this.Configuration.DefaultSortDirection.ToString());
				}

				if (this.Configuration.Fields != null && this.Configuration.Fields.Count > 0)
				{
					jsonTextWriter.WritePropertyName("Fields");
					jsonTextWriter.WriteStartArray();

					jsonTextWriter.WriteValue(FieldNameTransformUtility.PrimaryKeyFieldName(this.Configuration.PrimaryKeyFieldName));
					jsonTextWriter.WriteValue(DynamicPageDataServiceHandler.ShowCheckBoxColumnPropertyName);
					jsonTextWriter.WriteValue(DynamicPageDataServiceHandler.ShowEditButtonColumnPropertyName);
					jsonTextWriter.WriteValue(DynamicPageDataServiceHandler.ShowViewButtonColumnPropertyName);
					jsonTextWriter.WriteValue(DynamicPageDataServiceHandler.ShowDeleteButtonColumnPropertyName);

					int fieldIndex = 0;
					foreach (GridViewFieldConfiguration fieldConfig in this.Configuration.Fields)
						jsonTextWriter.WriteValue(FieldNameTransformUtility.DataBoundFieldName(fieldConfig.FieldName, fieldIndex++));

					if (this.Configuration.RowView != null)
						jsonTextWriter.WriteValue(FieldNameTransformUtility.ViewFragmentFieldName(this.Configuration.RowView.FieldName));

					jsonTextWriter.WriteEndArray();
				}

				jsonTextWriter.WriteEndObject();
			}

			return jsonStringBuilder.ToString();
		}

		/// <summary>
		/// Create JSON string for argument _detailPanelModalWindowConfig of JavaScript GridViewPanelClass constructor.
		/// </summary>
		/// <returns></returns>
		private string CreateDetailPanelModalWindowConfigJson()
		{
			StringBuilder jsonStringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonStringBuilder))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartObject();

				jsonTextWriter.WritePropertyName("Width");
				jsonTextWriter.WriteValue(this.DetailPanelPlugin.Width);

				jsonTextWriter.WritePropertyName("Height");
				jsonTextWriter.WriteValue(this.DetailPanelPlugin.Height);

				jsonTextWriter.WritePropertyName("Resizable");
				jsonTextWriter.WriteValue(this.DetailPanelPlugin.Resizable);

				jsonTextWriter.WritePropertyName("Draggable");
				jsonTextWriter.WriteValue(this.DetailPanelPlugin.Draggable);

				jsonTextWriter.WriteEndObject();
			}

			return jsonStringBuilder.ToString();
		}

		/// <summary>
		/// Create JSON string for argument _aggregatePanelModalWindowConfig of JavaScript GridViewPanelClass constructor.
		/// </summary>
		/// <returns></returns>
		private string CreateAggregatePanelModalWindowConfigJson()
		{
			if (this.AggregatePanelPlugin == null) return "null";

			StringBuilder jsonStringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonStringBuilder))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartObject();

				jsonTextWriter.WritePropertyName("Width");
				jsonTextWriter.WriteValue(this.AggregatePanelPlugin.Width);

				jsonTextWriter.WritePropertyName("Height");
				jsonTextWriter.WriteValue(this.AggregatePanelPlugin.Height);

				jsonTextWriter.WritePropertyName("Resizable");
				jsonTextWriter.WriteValue(this.AggregatePanelPlugin.Resizable);

				jsonTextWriter.WritePropertyName("Draggable");
				jsonTextWriter.WriteValue(this.AggregatePanelPlugin.Draggable);

				jsonTextWriter.WriteEndObject();
			}

			return jsonStringBuilder.ToString();
		}

		/// <summary>
		/// Create JSON string for argument _globalizationResources of JavaScript GridViewPanelClass constructor.
		/// </summary>
		/// <returns></returns>
		private string CreateGlobalizationResourcesJson()
		{
			StringBuilder jsonStringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonStringBuilder))
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.WriteStartObject();

				jsonTextWriter.WritePropertyName("DeleteConfirmDialogTitle");
				jsonTextWriter.WriteValue(string.Format(CultureInfo.InvariantCulture, Resources.DPCtrl_Grid_DeleteConfirmDialogTitle, this.Configuration.EntityName));

				jsonTextWriter.WritePropertyName("DeleteConfirmMessage");
				jsonTextWriter.WriteValue(string.Format(CultureInfo.InvariantCulture, Resources.DPCtrl_Grid_DeleteConfirmMessageTemplate, this.Configuration.EntityName));

				jsonTextWriter.WritePropertyName("PagingDisplayMessageTemplate");
				jsonTextWriter.WriteValue(Resources.DPCtrl_Grid_PagingDisplayMessageTemplate.Replace("{EntityName}", this.Configuration.EntityName));

				jsonTextWriter.WritePropertyName("EmptyGridPanelMessage");
				jsonTextWriter.WriteValue(string.Format(CultureInfo.InvariantCulture, Resources.DPCtrl_Grid_EmptyGridPanelMessageTemplate, this.Configuration.EntityName));

				jsonTextWriter.WritePropertyName("PreviewButtonText");
				jsonTextWriter.WriteValue(Resources.DPCtrl_Grid_PreviewButtonText);

				jsonTextWriter.WritePropertyName("GridPanelHeadCheckBoxToolTip");
				jsonTextWriter.WriteValue(Resources.DPCtrl_Grid_ColumnHeadCheckBoxToolTip);

				jsonTextWriter.WritePropertyName("EditableDetailPanelHeaderText");
				jsonTextWriter.WriteValue(this.DetailPanelPlugin.EditableHeaderText);

				jsonTextWriter.WritePropertyName("ViewableDetailPanelHeaderText");
				jsonTextWriter.WriteValue(this.DetailPanelPlugin.ViewableHeaderText);

				if (this.AggregatePanelPlugin != null)
				{
					jsonTextWriter.WritePropertyName("AggregatePanelHeaderText");
					jsonTextWriter.WriteValue(this.AggregatePanelPlugin.HeaderText);
				}

				jsonTextWriter.WriteEndObject();
			}

			return jsonStringBuilder.ToString();
		}
	}

	/// <summary>
	/// Plugin configuration of grid view panel for rendering detail panel in dynamic page.
	/// </summary>
	public sealed class GridViewPanelPluginConfiguration4DetailPanel
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public GridViewPanelPluginConfiguration4DetailPanel()
		{
			this.Width = 960;
			this.Height = 600;
			this.Resizable = false;
			this.Draggable = false;
			this.EditableHeaderText = Resources.DPCtrl_EditText;
			this.ViewableHeaderText = Resources.DPCtrl_ViewText;
		}

		/// <summary>
		/// Sets/gets width of popup modal window for edit/view the selected record (defaults to 960).
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Sets/gets height of popup modal window for edit/view the selected record (defaults to 600).
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Sets/gets true when popup modal window is resizable (defaults to false).
		/// </summary>
		public bool Resizable { get; set; }

		/// <summary>
		/// Sets/gets true when popup modal window is draggable (defaults to false).
		/// </summary>
		public bool Draggable { get; set; }

		/// <summary>
		/// Sets/gets detail panel header text in editable mode (defaults to "Update").
		/// </summary>
		public string EditableHeaderText { get; set; }

		/// <summary>
		/// Sets/gets detail panel header text in viewable mode (defaults to "View").
		/// </summary>
		public string ViewableHeaderText { get; set; }
	}

	/// <summary>
	/// Plugin configuration of grid view panel for rendering aggregate panel in dynamic page.
	/// </summary>
	public sealed class GridViewPanelPluginConfiguration4AggregatePanel
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public GridViewPanelPluginConfiguration4AggregatePanel()
		{
			this.Width = 960;
			this.Height = 600;
			this.Resizable = false;
			this.Draggable = false;
			this.HeaderText = Resources.DPCtrl_SummaryText;
		}

		/// <summary>
		/// Sets/gets width of popup modal window for edit/view the selected record (defaults to 960).
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Sets/gets height of popup modal window for edit/view the selected record (defaults to 600).
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Sets/gets true when popup modal window is resizable (defaults to false).
		/// </summary>
		public bool Resizable { get; set; }

		/// <summary>
		/// Sets/gets true when popup modal window is draggable (defaults to false).
		/// </summary>
		public bool Draggable { get; set; }

		/// <summary>
		/// Sets/gets aggregate panel header text (defaults to "Summary").
		/// </summary>
		public string HeaderText { get; set; }
	}
}

