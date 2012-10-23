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
	/// Permission treeview
	/// </summary>
	public class PermissionTreeView : WebControl, ITextControl, INamingContainer
	{
		private static IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();
		private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

		private MyTreeView treeView;

		private IEnumerable<PermissionConfig> PermissionConfigEnumerable
		{
			get
			{
				Guid userId = authenticationContext.User.UserId;
				return permissionApi.FindPermissionConfig(userId);
			}
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
		/// Sets/gets true to make all checkboxes of tree node checkable.
		/// </summary>
		public bool Checkable
		{
			get { return this.treeView.Checkable; }
			set { this.treeView.Checkable = value; }
		}

		/// <summary>
		/// Tree nodes checking cascading type
		/// </summary>
		public TreeNodeCheckCascadingTypes CascadingType 
		{
			get { if (base.ViewState["CascadingType"] == null) return TreeNodeCheckCascadingTypes.Standard; return (TreeNodeCheckCascadingTypes)base.ViewState["CascadingType"]; }
			set { base.ViewState["CascadingType"] = value; }
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
		/// Raises the System.Web.UI.Control.Init event.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.treeView = new MyTreeView { ID = "TreeView", FillBackgroundColor = true, CascadingType = TreeNodeCheckCascadingTypes.Standard };
			if (!Page.IsPostBack)
				this.treeView.DataSource = BuildTreeNodeBranch(this.PermissionConfigEnumerable);

			this.Controls.Add(this.treeView);
		}

		/// <summary>
		/// Set internal tree view cascading type.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.treeView.CascadingType = this.CascadingType;
		}

		private static IEnumerable<MyTreeNode> BuildTreeNodeBranch(IEnumerable<PermissionConfig> permissionConfigEnumerable)
		{
			if (permissionConfigEnumerable == null) return null;

			List<MyTreeNode> treeNodes = new List<MyTreeNode>();
			foreach (PermissionConfig permissionConfig in permissionConfigEnumerable)
			{
				treeNodes.Add(new MyTreeNode
				{
					Text = permissionConfig.Text,
					Value = permissionConfig.Value,
					Children = BuildTreeNodeBranch(permissionConfig.Permission)
				});
			}

			return treeNodes;
		}
	}
}

