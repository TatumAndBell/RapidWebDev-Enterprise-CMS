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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using Newtonsoft.Json;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// TreeView Control
	/// </summary>
	public class TreeView : WebControl, INamingContainer, IPostBackDataHandler, ITextControl
	{
		private Panel treeViewContainer;
		private HiddenField hiddenFieldCachedDataSource;
		private bool isDataSourceChanged;
		private IEnumerable<TreeNode> dataSource;

		private const string TREEVIEW_JS_TEMPLATE = @"
			var {0} = new TreeView('{0}', {1}, {2});
			
				if ($HasCheckedNodes$)
				{{
					{0}.uncheckAll();
					{0}.checkNodes($CheckedNodeValues$);
					{0}.saveState();
				}}

				if (!$Checkable$)
				{{
					{0}.disableCheckBox();
				}}";

		/// <summary>
		/// Tree nodes checking cascading type
		/// </summary>
		public TreeNodeCheckCascadingTypes CascadingType { get; set; }

		/// <summary>
		/// True to use vista-like style, defaults to false.
		/// </summary>
		public bool UseVistaLikeStyle { get; set; }

		/// <summary>
		/// True to fill background color, defaults to false.
		/// </summary>
		public bool FillBackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the text content of a control.
		/// </summary>
		public string Text
		{
			get 
			{
				if (this.CheckedValues != null)
					return this.CheckedValues.Concat();

				return null;
			}
			set { }
		}

		/// <summary>
		/// Checked node values of the treeview.
		/// </summary>
		public IEnumerable<string> CheckedValues { get; set; }

		/// <summary>
		/// Data source of the treeview.
		/// </summary>
		public IEnumerable<TreeNode> DataSource 
		{
			get { return this.dataSource; }
			set
			{
				this.isDataSourceChanged = true;
				this.dataSource = value;
			}
		}

		/// <summary>
		/// Whether to disable checkbox of tree node, defaults to false.
		/// </summary>
		public bool ReadOnly
		{
			get { return !this.Checkable; }
			set { this.Checkable = !value; }
		}

		/// <summary>
		/// Whether to enable checkbox of tree node, defaults to true.
		/// </summary>
		public bool Checkable
		{
			get { return this.ViewState["Checkable"] == null ? true : (bool)this.ViewState["Checkable"]; }
			set { this.ViewState["Checkable"] = value; }
		}

		/// <summary>
		/// Whether to show checkbox of tree node, defaults to true.
		/// </summary>
		public bool EnableCheckBox
		{
			get { return this.ViewState["EnableCheckBox"] == null ? true : (bool)this.ViewState["EnableCheckBox"]; }
			set { this.ViewState["EnableCheckBox"] = value; }
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.Init event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.hiddenFieldCachedDataSource = new HiddenField { ID = "hiddenFieldCachedDataSource" };
			this.Controls.Add(this.hiddenFieldCachedDataSource);

			this.treeViewContainer = new Panel { ID = "TreeViewContainer" };
			this.Controls.Add(this.treeViewContainer);
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.Load event.
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

			string dataSourceJson = this.BuildTreeViewDataSourceJson();

			if (this.isDataSourceChanged)
				this.hiddenFieldCachedDataSource.Value = this.SerializeTreeViewDataSource();

			bool hasCheckedNodes = this.CheckedValues != null && this.CheckedValues.Count() > 0;
			string javaScriptBlock = string.Format(CultureInfo.InvariantCulture, TREEVIEW_JS_TEMPLATE, this.treeViewContainer.ClientID, dataSourceJson, this.BuildTreeViewOptionsArgument());
			javaScriptBlock = javaScriptBlock.Replace("$HasCheckedNodes$", hasCheckedNodes.ToString().ToLowerInvariant())
				.Replace("$CheckedNodeValues$", this.BuildTreeViewCheckedValues())
				.Replace("$Checkable$", this.Checkable.ToString().ToLowerInvariant());

			ClientScripts.OnDocumentReady.Add2BeginOfBody(javaScriptBlock);
			ClientScripts.RegisterHeaderScriptInclude("~/resources/javascript/TreeView.js");
		}

		#region IPostBackDataHandler Members

		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			string checkedNodeValuesKey = string.Format(CultureInfo.InvariantCulture, "{0}_TreeViewContainer__hidden", postDataKey.Replace("$", "_"));
			string postDataValue = postCollection[checkedNodeValuesKey];
			string[] postDataValueArray = !string.IsNullOrEmpty(postDataValue) ? postDataValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
			this.CheckedValues = postDataValueArray.Select(value => value.Trim());

			string dataSourceValueKey = string.Format(CultureInfo.InvariantCulture, "{0}$hiddenFieldCachedDataSource", postDataKey);
			postDataValue = postCollection[dataSourceValueKey];
			this.dataSource = !string.IsNullOrEmpty(postDataValue) ? this.DeserializeTreeViewDataSource(postDataValue) : new List<TreeNode>();

			return true;
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{
			
		}

		#endregion

		private string BuildTreeViewDataSourceJson()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				this.BuildTreeViewDataSourceJson(jsonWriter, this.DataSource);
			}

			return jsonBuilder.ToString();
		}

		private void BuildTreeViewDataSourceJson(JsonTextWriter jsonWriter, IEnumerable<TreeNode> treeNodeEnumerable)
		{
			jsonWriter.WriteStartArray();

			if (treeNodeEnumerable != null)
			{
				foreach (TreeNode treeNode in treeNodeEnumerable)
				{
					jsonWriter.WriteStartObject();

					if (!string.IsNullOrEmpty(treeNode.Value))
					{
						// id
						jsonWriter.WritePropertyName("id");
						jsonWriter.WriteValue(treeNode.Value);
					}

					// text
					jsonWriter.WritePropertyName("text");
					jsonWriter.WriteValue(treeNode.Text);

					// on node click event
					if (!string.IsNullOrEmpty(treeNode.OnClientClick))
					{
						jsonWriter.WritePropertyName("listeners");
						jsonWriter.WriteStartObject();
						jsonWriter.WritePropertyName("click");
						jsonWriter.WriteRawValue(string.Format(CultureInfo.InvariantCulture, "function(node, e) {{ {0}(node, e); }}", treeNode.OnClientClick));
						jsonWriter.WriteEndObject();
					}

					//  expanded 
					jsonWriter.WritePropertyName("expanded");
					jsonWriter.WriteValue(true);

					if (this.EnableCheckBox)
					{
						// checked
						jsonWriter.WritePropertyName("checked");
						jsonWriter.WriteValue(false);
					}

					if (treeNode.Children != null && treeNode.Children.Count() > 0)
					{
						// children
						jsonWriter.WritePropertyName("children");
						this.BuildTreeViewDataSourceJson(jsonWriter, treeNode.Children);
					}
					else
					{
						// leaf
						jsonWriter.WritePropertyName("leaf");
						jsonWriter.WriteValue(true);
					}

					jsonWriter.WriteEndObject();
				}
			}

			jsonWriter.WriteEndArray();
		}

		private string BuildTreeViewOptionsArgument()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				jsonWriter.WriteStartObject();

				jsonWriter.WritePropertyName("useVistaLikeArrow");
				jsonWriter.WriteValue(this.UseVistaLikeStyle);

				jsonWriter.WritePropertyName("fillBackground");
				jsonWriter.WriteValue(this.FillBackgroundColor);

				jsonWriter.WritePropertyName("cascadingMode");
				jsonWriter.WriteValue(this.CascadingType.ToString().ToLowerInvariant());

				jsonWriter.WriteEndObject();
			}

			return jsonBuilder.ToString();
		}

		private string BuildTreeViewCheckedValues()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				jsonWriter.WriteStartArray();

				if (this.CheckedValues != null)
				{
					foreach (string checkedValue in this.CheckedValues)
						jsonWriter.WriteValue(checkedValue);
				}

				jsonWriter.WriteEndArray();
			}

			return jsonBuilder.ToString();
		}

		private string SerializeTreeViewDataSource()
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			return serializer.Serialize(this.dataSource);
		}

		private IEnumerable<TreeNode> DeserializeTreeViewDataSource(string s)
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			return serializer.Deserialize<IEnumerable<TreeNode>>(s);
		}
	}

	/// <summary>
	/// TreeView Node
	/// </summary>
	[Serializable]
	public class TreeNode
	{
		/// <summary>
		/// Tree node text
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Tree node value
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// The client event the the tree node is clicked. 
		/// The client binding function prototype should be "function (Node this, Ext.EventObject e) { }".
		/// </summary>
		public string OnClientClick { get; set; }

		/// <summary>
		/// The children of the current node.
		/// </summary>
		public IEnumerable<TreeNode> Children { get; set; }
	}

	/// <summary>
	/// TreeView node checking cascading types.
	/// </summary>
	public enum TreeNodeCheckCascadingTypes
	{
		/// <summary>
		/// No impact to other tree node when a tree node is checked/unchecked
		/// </summary>
		None = 0,

		/// <summary>
		/// Fully cascading check/uncheck the parent and children when a node is checked/unchecked
		/// </summary>
		Full = 1,

		/// <summary>
		/// The parent will be checked when a node is checked. The children will be unchecked when the parent node is unchecked.
		/// Standard means there allows the parent node been checked without any children checked, but all the ancestors have to be checked when a child is checked.
		/// </summary>
		Standard = 2,

		/// <summary>
		/// Only one node can be checked at a time.
		/// </summary>
		SingleCheck = 3
	}
}

