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
using System.Linq;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// HierarchyData aggregate panel page handler.
	/// </summary>
	public class HierarchyDataAggregatePanel : AggregatePanelPage
	{
		/// <summary>
		/// Protected authentication context.
		/// </summary>
		protected static readonly IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

		/// <summary>
		/// Protected hierarchy Api.
		/// </summary>
		protected static readonly IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

		#region Binding Web Controls

		/// <summary />
		[Binding]
		protected TreeView TreeViewHierarchyData;

		#endregion

		/// <summary>
		/// Load hierarchy data into the treeview
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnLoad(IRequestHandler sender, AggregatePanelPageEventArgs e)
		{
			base.OnLoad(sender, e);

			string hierarchyType = authenticationContext.TempVariables["HierarchyType"] as string;
			IEnumerable<HierarchyDataObject> hierarchyDataObjects = hierarchyApi.GetAllHierarchyData(hierarchyType);

			List<TreeNode> treeNodes = new List<TreeNode>();
			foreach (HierarchyDataObject hierarchyDataObject in hierarchyDataObjects.Where(d => !d.ParentHierarchyDataId.HasValue))
			{
				TreeNode rootTreeNode = new TreeNode { Text = hierarchyDataObject.Name, Value = hierarchyDataObject.HierarchyDataId.ToString() };
				treeNodes.Add(rootTreeNode);
				SetupChildTreeNodes(rootTreeNode, hierarchyDataObject.HierarchyDataId, hierarchyDataObjects);
			}

			this.TreeViewHierarchyData.DataSource = treeNodes;
			this.TreeViewHierarchyData.DataBind();
		}

		/// <summary>
		/// Setup hierarchy type information.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			SetupContextTempVariablesUtility.SetupHierarchyType(sender, e, true);
		}

		private static void SetupChildTreeNodes(TreeNode parentTreeNode, Guid parentHierarchyDataId, IEnumerable<HierarchyDataObject> source)
		{
			IEnumerable<HierarchyDataObject> hierarchyDataObjects = source.Where(d => d.ParentHierarchyDataId.HasValue && d.ParentHierarchyDataId.Value == parentHierarchyDataId);
			if (hierarchyDataObjects.Count() == 0) return;

			List<TreeNode> childTreeNodes = new List<TreeNode>();
			parentTreeNode.Children = childTreeNodes;
			foreach (HierarchyDataObject hierarchyDataObject in hierarchyDataObjects)
			{
				TreeNode childTreeNode = new TreeNode { Text = hierarchyDataObject.Name, Value = hierarchyDataObject.HierarchyDataId.ToString() };
				childTreeNodes.Add(childTreeNode);
				SetupChildTreeNodes(childTreeNode, hierarchyDataObject.HierarchyDataId, source);
			}
		}
	}
}

