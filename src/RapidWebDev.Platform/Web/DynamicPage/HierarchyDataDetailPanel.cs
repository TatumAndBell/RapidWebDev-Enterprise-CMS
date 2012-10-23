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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel.Web.Controls;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Platform.Properties;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using WebControls = System.Web.UI.WebControls;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Hierarchy data detail panel page handler.
	/// </summary>
	public class HierarchyDataDetailPanel : DetailPanelPage
	{
		private static object syncObject = new object();
		/// <summary>
		/// Protected authentication context
		/// </summary>
		protected static readonly IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		/// <summary>
		/// Protected hierarchy Api
		/// </summary>
		protected static readonly IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>(); 

		#region Protected Web Controls

		/// <summary />
		[Binding]
		protected RapidWebDev.UI.Controls.ComboBox ComboBoxParentHierarchyData;
		/// <summary />
		[Binding]
		protected TextBox TextBoxName;
		/// <summary />
		[Binding]
		protected TextBox TextBoxDescription;

		/// <summary />
		[Binding]
		protected ExtensionDataForm ExtensionDataForm;
		/// <summary />
		[Binding]
		protected PlaceHolder PlaceHolderOperatorContext;
		/// <summary />
		[Binding]
		protected RapidWebDev.Platform.Web.Controls.UserLink UserLinkCreatedBy;
		/// <summary />
		[Binding]
		protected TextBox TextBoxCreatedDate;
		/// <summary />
		[Binding]
		protected RapidWebDev.Platform.Web.Controls.UserLink UserLinkLastUpdatedBy;
		/// <summary />
		[Binding]
		protected TextBox TextBoxLastUpdatedDate;

		#endregion

		/// <summary>
		/// Create a new hierarchy data from detail panel and return its id.
		/// The method needs to create a new entity and set control values to its properties then persist it.
		/// </summary>
		/// <returns>returns the id of new created hierarchy data.</returns>
		public override string Create()
		{
			this.ValidateInput(Guid.Empty);

			HierarchyDataObject hierarchyDataObject = new HierarchyDataObject();
			hierarchyDataObject.ExtensionDataTypeId = this.ResolveExtensionDataTypeId();
			hierarchyDataObject.HierarchyType = authenticationContext.TempVariables["HierarchyType"] as string;

			if (this.TextBoxName != null)
				hierarchyDataObject.Name = this.TextBoxName.Text;

			if (this.ComboBoxParentHierarchyData != null)
				hierarchyDataObject.ParentHierarchyDataId = string.IsNullOrEmpty(this.ComboBoxParentHierarchyData.SelectedValue) ? (Guid?)null : new Guid(this.ComboBoxParentHierarchyData.SelectedValue);

			if (this.TextBoxDescription != null)
				hierarchyDataObject.Description = this.TextBoxDescription.Text;

			if (this.ExtensionDataForm != null)
				this.ExtensionDataForm.SetObjectPropertiesFromControlValues(hierarchyDataObject);

			hierarchyApi.Save(hierarchyDataObject);
			return hierarchyDataObject.HierarchyDataId.ToString();
		}

		/// <summary>
		/// Update an existed hierarchy data from detail panel.
		/// The method needs to load an existed entity by specified id and set control values to overwrite its original properties then persist it.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Update(string entityId)
		{
			Guid hierarchyDataId = new Guid(entityId);
			this.ValidateInput(hierarchyDataId);
			HierarchyDataObject hierarchyDataObject = hierarchyApi.GetHierarchyData(hierarchyDataId);
			if (hierarchyDataObject == null)
				throw new ValidationException(Resources.InvalidHierarchyDataId);

			hierarchyDataObject = new HierarchyDataObject { HierarchyDataId = hierarchyDataId };
			hierarchyDataObject.ExtensionDataTypeId = this.ResolveExtensionDataTypeId();
			hierarchyDataObject.HierarchyType = authenticationContext.TempVariables["HierarchyType"] as string;

			if (this.TextBoxName != null)
				hierarchyDataObject.Name = this.TextBoxName.Text;

			if (this.ComboBoxParentHierarchyData != null)
				hierarchyDataObject.ParentHierarchyDataId = string.IsNullOrEmpty(this.ComboBoxParentHierarchyData.SelectedValue) ? (Guid?)null : new Guid(this.ComboBoxParentHierarchyData.SelectedValue);

			if (this.TextBoxDescription != null)
				hierarchyDataObject.Description = this.TextBoxDescription.Text;

			if (this.ExtensionDataForm != null)
				this.ExtensionDataForm.SetObjectPropertiesFromControlValues(hierarchyDataObject);

			hierarchyApi.Save(hierarchyDataObject);
		}

		/// <summary>
		/// Reset all controls of the detail panel to initial state.
		/// The method will be invoked when enables the detail panel to support creating entities continuously.
		/// After an entity been created, the method will be invoked to reset form controls for another input.
		/// </summary>
		public override void Reset()
		{
			if (this.TextBoxName != null)
			{
				this.TextBoxName.Text = "";
				ScriptManager.GetCurrent(HttpContext.Current.Handler as Page).SetFocus(this.TextBoxName);
			}

			if (this.ComboBoxParentHierarchyData != null)
				this.ComboBoxParentHierarchyData.SelectedValue = "";

			if (this.TextBoxDescription != null)
				this.TextBoxDescription.Text = "";

			if (this.ExtensionDataForm != null)
				this.ExtensionDataForm.ResetControlValuesToDefault();

			this.BindParentHierarchyData();
		}

		/// <summary>
		/// The method is designed to load entity by id to editable detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		public override void LoadWritableEntity(string entityId)
		{
			HierarchyDataObject hierarchyDataObject = hierarchyApi.GetHierarchyData(new Guid(entityId));
			if (hierarchyDataObject == null) return;

			if (this.TextBoxName != null)
				this.TextBoxName.Text = hierarchyDataObject.Name;

			if (this.ComboBoxParentHierarchyData != null)
				this.ComboBoxParentHierarchyData.SelectedValue = hierarchyDataObject.ParentHierarchyDataId.HasValue ? hierarchyDataObject.ParentHierarchyDataId.Value.ToString() : "";

			if (this.TextBoxDescription != null)
				this.TextBoxDescription.Text = hierarchyDataObject.Description;

			if (this.ExtensionDataForm != null)
				this.ExtensionDataForm.SetControlValuesFromObjectProperties(hierarchyDataObject);

			if (this.PlaceHolderOperatorContext != null)
				this.PlaceHolderOperatorContext.Visible = true;

			if (this.UserLinkCreatedBy != null)
				this.UserLinkCreatedBy.UserId = hierarchyDataObject.CreatedBy.ToString();

			if (this.UserLinkLastUpdatedBy != null)
				this.UserLinkLastUpdatedBy.UserId = hierarchyDataObject.LastUpdatedBy.ToString();

			if (this.TextBoxCreatedDate != null)
				this.TextBoxCreatedDate.Text = LocalizationUtility.ToDateTimeString(hierarchyDataObject.CreatedDate);

			if (this.TextBoxLastUpdatedDate != null && hierarchyDataObject.LastUpdatedDate.HasValue)
				this.TextBoxLastUpdatedDate.Text = LocalizationUtility.ToDateTimeString(hierarchyDataObject.LastUpdatedDate.Value);
		}

		/// <summary>
		/// The method will be invoked when detail panel is loaded.
		/// The implementation can resolve current visiting Page from the conversation as (HttpContext.Current.Handler as Page).
		/// </summary>
		/// <param name="sender">The web page which contains the detail panel.</param>
		/// <param name="e">Callback event argument.</param>
		public override void OnLoad(IRequestHandler sender, DetailPanelPageEventArgs e)
		{
			if (this.ExtensionDataForm != null)
			{
				Guid extensionDataTypeId = this.ResolveExtensionDataTypeId();
				if (extensionDataTypeId == Guid.Empty)
					this.ExtensionDataForm = null;
				else
					this.ExtensionDataForm.CreateDataInputForm(extensionDataTypeId);
			}

			if (!sender.IsPostBack)
				this.BindParentHierarchyData();
		}

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// Set domain into http context when web page is initializing.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			SetupContextTempVariablesUtility.SetupHierarchyType(sender, e, true);
		}

		/// <summary>
		/// Resolve extension data type id. Returns Guid.Empty by default.
		/// </summary>
		/// <returns></returns>
		protected virtual Guid ResolveExtensionDataTypeId()
		{
			return Guid.Empty;
		}

		/// <summary>
		/// Validate data input of hierarchy data.
		/// </summary>
		/// <param name="hierarchyDataObjectId"></param>
		protected virtual void ValidateInput(Guid hierarchyDataObjectId)
		{
			using (ValidationScope validationScope = new ValidationScope())
			{
				if (Kit.IsEmpty(this.TextBoxName.Text.Trim()))
					validationScope.Error(Resources.HierarchyDataNameCannotBeEmpty);
				else
				{
					string hierarchyType = authenticationContext.TempVariables["HierarchyType"] as string;
					HierarchyDataObject hierarchyDataObject = hierarchyApi.GetHierarchyData(hierarchyType, this.TextBoxName.Text.Trim());
					if (hierarchyDataObject != null && hierarchyDataObject.HierarchyDataId != hierarchyDataObjectId)
						validationScope.Error(Resources.DuplicateHierarchyDataName, this.TextBoxName.Text.Trim());
				}
			}
		}

		/// <summary>
		/// Bind the combobox of parent hierarchy data.
		/// </summary>
		protected void BindParentHierarchyData()
		{
			if (this.ComboBoxParentHierarchyData == null) return;
			this.ComboBoxParentHierarchyData.Items.Clear();
			this.ComboBoxParentHierarchyData.Items.Add(new ListItem("", ""));

			string hierarchyType = authenticationContext.TempVariables["HierarchyType"] as string;
			IEnumerable<HierarchyDataObject> allHierarchyDataObjects = hierarchyApi.GetAllHierarchyData(hierarchyType);
			List<ListItem> allItems = new List<ListItem> { new ListItem("") };
			LoadStandardIndustryComboBoxData(allItems, allHierarchyDataObjects, null, 0);

			this.ComboBoxParentHierarchyData.Items.Clear();
			this.ComboBoxParentHierarchyData.Items.AddRange(allItems.ToArray());
		}

		private static void LoadStandardIndustryComboBoxData(List<ListItem> allItems, IEnumerable<HierarchyDataObject> allHierarchyDataObjects, Guid? parentHierarchyDataId, int hierarchyLevel)
		{
			IEnumerable<HierarchyDataObject> childHierarchyDataObjects = allHierarchyDataObjects.Where(d => d.ParentHierarchyDataId == parentHierarchyDataId).OrderBy(d => d.Name);
			foreach (HierarchyDataObject childHierarchyDataObject in childHierarchyDataObjects)
			{
				allItems.Add(new ListItem(GetStandardIndustryItemText(childHierarchyDataObject.Name, hierarchyLevel), childHierarchyDataObject.HierarchyDataId.ToString()));
				LoadStandardIndustryComboBoxData(allItems, allHierarchyDataObjects, childHierarchyDataObject.HierarchyDataId, hierarchyLevel + 1);
			}
		}

		private static string GetStandardIndustryItemText(string standardIndustryName, int hierarchyLevel)
		{
			string result = "";
			for (int i = 0; i < hierarchyLevel; i++)
				result += "--";

			if (!string.IsNullOrEmpty(result)) result += " ";
			return result + standardIndustryName;
		}
	}
}

