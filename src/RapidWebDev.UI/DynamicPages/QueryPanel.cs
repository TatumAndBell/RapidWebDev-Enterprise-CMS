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
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Query panel control.
	/// </summary>
	[ParseChildren(true), PersistChildren(false)]
	public sealed class QueryPanel : System.Web.UI.WebControls.Panel, INamingContainer
	{
		private const string QUERY_PANEL_EXT_FORMATTER = @"
			window.QueryFieldControlVariablesAccessorId = '$HiddenFieldQueryFieldControlVariables$';
			if (window.$ControlVariableName$ != undefined && window.$ControlVariableName$ != null) window.$ControlVariableName$.destroy();
			window.$ControlVariableName$ = new Ext.Panel(
			{ 
				id: '$ControlId$_UniqueId', 
				contentEl: '$ControlId$', 
				title: '$Title$',  
				collapsed: $Collapsed$, 
				collapsible: $Collapsible$, 
				frame: true, 
				titleCollapse:true 
			});

			window.$ControlVariableName$.render(Ext.query('#$ControlId$')[0].parentNode, '$ControlId$');";

		private const string BUTTONS_EVENT_HANDLER_REGISTRATION = @"			
			window.$ResetButtonVariableName$.purgeListeners();
			window.$ResetButtonVariableName$.on('click', function()
			{
				var queryFieldControlString = Ext.query('#' + window.QueryFieldControlVariablesAccessorId)[0].value;
				var queryFieldControls = queryFieldControlString.split(';');
				for (var i = 0; i < queryFieldControls.length; i++)
				{
					var queryFieldControl = queryFieldControls[i];
					var queryFieldControlKeyValuePair = queryFieldControl.split(':');
					var resetControlValueJs = 'window.' + queryFieldControlKeyValuePair[1] + '.reset();';
					eval(resetControlValueJs);
				}

				if (window.RegisteredGridViewPanelObject)
					window.RegisteredGridViewPanelObject.ExecuteQuery();
			});

			window.$QueryButtonVariableName$.purgeListeners();
			window.$QueryButtonVariableName$.on('click', function()
			{
				if (window.RegisteredGridViewPanelObject)
					window.RegisteredGridViewPanelObject.ExecuteQuery();
			});";

		private Button buttonQuery;
		private Button buttonReset;
		private System.Web.UI.WebControls.HiddenField hiddenFieldQueryFieldControlVariables;
		private List<KeyValuePair<IQueryFieldControl, string>> queryFieldControlsByFieldName = new List<KeyValuePair<IQueryFieldControl, string>>();

		/// <summary>
		/// Sets/gets QueryPanel configuration.
		/// </summary>
		public QueryPanelConfiguration Configuration { get; set; }

		/// <summary>
		/// Construct QueryPanel instance.
		/// </summary>
		public QueryPanel()
		{
			
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			if (this.Configuration == null)
				throw new ConfigurationErrorsException(Resources.Ctrl_PropertyConfigurationNotSpecified);

			base.CreateChildControls();

			// create layout panels
			List<KeyValuePair<QueryFieldConfiguration, IEnumerable<IQueryFieldControl>>> queryFieldControlsByConfigurations = new List<KeyValuePair<QueryFieldConfiguration, IEnumerable<IQueryFieldControl>>>();
			foreach (QueryFieldConfiguration queryFieldConfiguration in this.Configuration.Controls)
			{
				IEnumerable<IQueryFieldControl> queryFieldControlsByConfiguration = QueryFieldControlFactory.CreateQueryControls(queryFieldConfiguration);
				queryFieldControlsByConfigurations.Add(new KeyValuePair<QueryFieldConfiguration, IEnumerable<IQueryFieldControl>>(queryFieldConfiguration, queryFieldControlsByConfiguration));
				foreach (IQueryFieldControl queryFieldControl in queryFieldControlsByConfiguration)
					this.queryFieldControlsByFieldName.Add(new KeyValuePair<IQueryFieldControl, string>(queryFieldControl, queryFieldConfiguration.FieldName));
			}

			IQueryPanelLayout queryPanelLayoutGenerator = SpringContext.Current.GetObject<IQueryPanelLayout>();
			Control queryPanelLayout = queryPanelLayoutGenerator.Create(queryFieldControlsByConfigurations);
			this.Controls.Add(queryPanelLayout);

			IEnumerable<IQueryFieldControl> queryFieldControls = this.queryFieldControlsByFieldName.Select(kvp => kvp.Key);
			QueryFieldControlFactory.AssignQueryFieldControlIDAndIndex(queryFieldControls);

			// create hiddenfield to store all client variables for query field controls.
			StringBuilder queryFieldControlVariablesBuilder = new StringBuilder();
			foreach (IQueryFieldControl queryFieldControl in queryFieldControls)
			{
				if (queryFieldControlVariablesBuilder.Length > 0)
					queryFieldControlVariablesBuilder.Append(";");

				queryFieldControlVariablesBuilder.AppendFormat("{0}{1}:{2}", WebUtility.QUERY_FIELD_CONTROL_POST_PREFRIX_NAME, queryFieldControl.ControlIndex, queryFieldControl.ClientVariableName);
			}

			this.hiddenFieldQueryFieldControlVariables = new System.Web.UI.WebControls.HiddenField { ID = "HiddenFieldQueryFieldControlVariables" };
			this.hiddenFieldQueryFieldControlVariables.Value = queryFieldControlVariablesBuilder.ToString();
			this.Controls.Add(this.hiddenFieldQueryFieldControlVariables);

			HtmlTable tableButtons = new HtmlTable { ID="ButtonPanel", CellPadding = 0, CellSpacing = 0, Width = "100%" };
			tableButtons.Attributes["class"] = "querypanel-buttons";
			this.Controls.Add(tableButtons);

			HtmlTableRow rowButton = new HtmlTableRow();
			tableButtons.Rows.Add(rowButton);

			HtmlTableCell cellButton = new HtmlTableCell { Align = "center" };
			cellButton.Style["padding-top"] = "6px";
			rowButton.Cells.Add(cellButton);

			this.buttonQuery = new Button { ID = "ButtonQuery", Configuration = new ButtonConfiguration { ButtonRenderType = ButtonRenderTypes.Button, Text = Resources.Ctrl_QueryButtonText } };
			cellButton.Controls.Add(this.buttonQuery);

			HtmlGenericControl buttonSeparator = new HtmlGenericControl("span") { InnerHtml = "    " };
			buttonSeparator.Style["padding-left"] = "2px";
			buttonSeparator.Style["padding-right"] = "2px";
			cellButton.Controls.Add(buttonSeparator);

			this.buttonReset = new Button { ID = "ButtonReset", Configuration = new ButtonConfiguration { ButtonRenderType = ButtonRenderTypes.Button, Text = Resources.Ctrl_ResetButtonText } };
			cellButton.Controls.Add(this.buttonReset);
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			string javaScriptBlock = QUERY_PANEL_EXT_FORMATTER
				.Replace("$ControlId$", this.ClientID)
				.Replace("$ControlVariableName$", WebUtility.GenerateVariableName(this.ClientID))
				.Replace("$Title$", WebUtility.EncodeJavaScriptString(this.Configuration.HeaderText))
				.Replace("$Collapsed$", "false")
				.Replace("$Collapsible$", "false")
				.Replace("$HiddenFieldQueryFieldControlVariables$", this.hiddenFieldQueryFieldControlVariables.ClientID);

			ClientScripts.OnDocumentReady.Add2EndOfBody(javaScriptBlock);

			javaScriptBlock = BUTTONS_EVENT_HANDLER_REGISTRATION
				.Replace("$QueryButtonVariableName$", WebUtility.GenerateVariableName(this.buttonQuery.ClientID))
				.Replace("$ResetButtonVariableName$", WebUtility.GenerateVariableName(this.buttonReset.ClientID));

			ClientScripts.OnDocumentReady.Add2EndOfBody(javaScriptBlock);

			// There should have a container registered into dynamic page which we can get query field variable by field name.
			this.RegisterQueryFieldControlVariables();
		}

		private void RegisterQueryFieldControlVariables()
		{
			StringBuilder queryFieldControlVariablesBuilder = new StringBuilder("window.QueryPanel = new Object(); window.QueryPanel.Fields = new Ext.util.MixedCollection();");
			var queryFieldControlsByFieldNameDictionary = this.queryFieldControlsByFieldName.GroupBy(g => g.Value).ToDictionary(g => g.Key, g => g.Select(kvp => kvp.Key).ToList());
			foreach (string fieldName in queryFieldControlsByFieldNameDictionary.Keys)
			{
				List<IQueryFieldControl> queryFieldControls = queryFieldControlsByFieldNameDictionary[fieldName];
				if (queryFieldControls.Count == 1)
				{
					queryFieldControlVariablesBuilder.AppendFormat("window.QueryPanel.Fields.add('{0}', {1});", fieldName, queryFieldControls.First().ClientVariableName);
				}
				else if (queryFieldControls.Count > 1)
				{
					string queryFieldControlsArrayVariableName = string.Format(CultureInfo.InvariantCulture, "queryFieldControls{0}", Guid.NewGuid().ToString("N"));

					queryFieldControlVariablesBuilder.AppendFormat("window.{0} = new Array();", queryFieldControlsArrayVariableName);
					foreach (IQueryFieldControl queryFieldControl in queryFieldControls)
						queryFieldControlVariablesBuilder.AppendFormat("window.{0}.push({1});", queryFieldControlsArrayVariableName, queryFieldControl.ClientVariableName);

					queryFieldControlVariablesBuilder.AppendFormat("window.QueryPanel.Fields.add('{0}', {1});", fieldName, queryFieldControlsArrayVariableName);
				}
				else
				{
					continue;
				}
			}

			ClientScripts.OnDocumentReady.Add2EndOfBody(queryFieldControlVariablesBuilder.ToString(), JavaScriptPriority.Low);
		}
	}
}
