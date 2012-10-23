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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Platform;
using RapidWebDev.UI.Controls;
using MyTreeNode = RapidWebDev.UI.Controls.TreeNode;
using MyTreeView = RapidWebDev.UI.Controls.TreeView;

namespace RapidWebDev.Platform.Web.Controls
{
    /// <summary>
    /// Hierarchy TreeView web control.
    /// </summary>
	public class HierarchyTreeView : WebControl, INamingContainer, ITextControl
    {
		private static IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();
		private MyTreeView treeView;
		private bool checkable;

		/// <summary>
		/// Sets/gets true to make all checkboxes of tree node checkable.
		/// </summary>
		public bool Checkable
		{
			get { return this.checkable; }
			set { this.checkable = value; }
		}

		/// <summary>
		/// Checked node values of the treeview.
		/// </summary>
		public IEnumerable<string> CheckedValues
		{
			get { return this.treeView.CheckedValues; }
			set { this.treeView.CheckedValues = value; }
		}

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
		/// Hierarchy type
		/// </summary>
		public string HierarchyType { get; set; }

		/// <summary>
		/// Raises the System.Web.UI.Control.Init event.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.treeView = new MyTreeView { ID = "TreeView", FillBackgroundColor = true, CascadingType = TreeNodeCheckCascadingTypes.None };
			this.Controls.Add(this.treeView);
			base.EnsureChildControls();
		}

		/// <summary>
		/// Load hierarchy data objects by hierarchy type and bind to treeview.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack)
			{
				IEnumerable<HierarchyDataObject> hierarchyObjects = hierarchyApi.GetAllHierarchyData(this.HierarchyType);
				this.treeView.DataSource = BuildTreeNodeBranch(hierarchyObjects, null);
			}
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.treeView.Checkable = this.checkable;
		}

		private static IEnumerable<MyTreeNode> BuildTreeNodeBranch(IEnumerable<HierarchyDataObject> hierarchyObjects, Guid? parentAreaId)
		{
			if (hierarchyObjects == null) return null;

			List<MyTreeNode> treeNodes = new List<MyTreeNode>();
			foreach (HierarchyDataObject hierarchyObject in hierarchyObjects.Where(a => a.ParentHierarchyDataId == parentAreaId))
			{
				treeNodes.Add(new MyTreeNode
				{
					Text = hierarchyObject.ToString(),
					Value = hierarchyObject.HierarchyDataId.ToString(),
					Children = BuildTreeNodeBranch(hierarchyObjects, hierarchyObject.HierarchyDataId)
				});
			}

			return treeNodes;
		}
    }
}

