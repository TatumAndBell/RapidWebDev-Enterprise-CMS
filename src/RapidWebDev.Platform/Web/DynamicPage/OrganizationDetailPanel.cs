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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Platform.Properties;
using RapidWebDev.Platform.Web.Controls;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using MyControls = RapidWebDev.UI.Controls;
using UserLink = RapidWebDev.Platform.Web.Controls.UserLink;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Detail panel class for organization.
	/// </summary>
	public class OrganizationDetailPanel : DetailPanelPage
	{
		/// <summary>
		/// Protected hierarchy Api.
		/// </summary>
		protected static readonly IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();
		/// <summary>
		/// Protected organization Api.
		/// </summary>
		protected static readonly IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
		/// <summary>
		/// Protected authentication context.
		/// </summary>
		protected static readonly IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		/// <summary>
		/// Protected platform configuration.
		/// </summary>
		protected static readonly IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();

		/// <summary>
		/// Protected metadata Api
		/// </summary>
		protected static readonly IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();

		#region Web Controls

		/// <summary />
		[Binding]
		protected MyControls.HierarchySelector AssociatedAreaSelector;
		/// <summary />
		[Binding]
		protected TextBox TextBoxOrganizationCode;
		/// <summary />
		[Binding]
		protected TextBox TextBoxOrganizationName;
		/// <summary />
		[Binding]
		protected OrganizationSelector ParentOrganizationSelector;
		/// <summary />
		[Binding]
		protected DropDownList DropDownListOrganizationType;
		/// <summary />
		[Binding]
		protected RadioButtonList RadioButtonListOrganizationStatus;

		/// <summary />
		[Binding]
		protected ExtensionModel.Web.Controls.ExtensionDataForm OrganizationExtensionDataForm;

		/// <summary />
		[Binding]
		protected TextBox TextBoxDescription;

		/// <summary />
		[Binding]
		protected PlaceHolder PlaceHolderOperatorContext;
		/// <summary />
		[Binding]
		protected UserLink UserLinkCreatedBy;
		/// <summary />
		[Binding]
		protected TextBox TextBoxCreatedOn;
		/// <summary />
		[Binding]
		protected UserLink UserLinkLastModifiedBy;
		/// <summary />
		[Binding]
		protected TextBox TextBoxLastModifiedOn;

		#endregion

		/// <summary>
		/// Gets organization domain
		/// </summary>
		protected string Domain
		{
			get { return authenticationContext.TempVariables["Domain.Value"] as string; }
		}

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// Set domain into http context when web page is initializing.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			SetupContextTempVariablesUtility.SetupOrganizationDomain(sender, e);
		}

		/// <summary>
		/// Load organization types when the web page loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnLoad(IRequestHandler sender, DetailPanelPageEventArgs e)
		{
			if (this.AssociatedAreaSelector != null)
				this.AssociatedAreaSelector.SetHierarchyType(platformConfiguration.AreaHierarchyTypeValue);

			if (!sender.IsPostBack && this.DropDownListOrganizationType != null)
			{
				IEnumerable<OrganizationTypeObject> organizationTypes = organizationApi.FindOrganizationTypes(new[] { authenticationContext.TempVariables["Domain.Value"] as string });
				this.DropDownListOrganizationType.Items.Clear();
				this.DropDownListOrganizationType.Items.Add("");

				foreach (OrganizationTypeObject organizationType in organizationTypes)
					this.DropDownListOrganizationType.Items.Add(new ListItem(organizationType.Name, organizationType.OrganizationTypeId.ToString()));
			}

			if (this.ParentOrganizationSelector != null)
				this.ParentOrganizationSelector.Domain = authenticationContext.TempVariables["Domain.Value"] as string;

			if (this.OrganizationExtensionDataForm != null)
			{
				Guid extensionDataTypeId = this.ResolveOrganizationExtensionDataTypeId();
				this.OrganizationExtensionDataForm.CreateDataInputForm(extensionDataTypeId);
			}
		}

		/// <summary>
		/// Create a new organization and return the id.
		/// </summary>
		/// <returns>returns the id of new created organization.</returns>
		public override string Create()
		{
			this.ValidateDataInputForm(null);

			OrganizationObject org = new OrganizationObject
			{
				ExtensionDataTypeId = this.ResolveOrganizationExtensionDataTypeId()
			};

			SetOrgPropertiesFromControls(org);
			organizationApi.Save(org);

			return org.OrganizationId.ToString();
		}

		/// <summary>
		/// Update an existed organization.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Update(string entityId)
		{
			Guid organizationId = new Guid(entityId);
			OrganizationObject orgToUpdate = organizationApi.GetOrganization(organizationId);
			if (orgToUpdate == null)
				throw new ValidationException(Resources.InvalidOrganizationID);

			orgToUpdate.ExtensionDataTypeId = this.ResolveOrganizationExtensionDataTypeId();
			this.ValidateDataInputForm(organizationId);

			OrganizationObject org = new OrganizationObject { OrganizationId = organizationId, ExtensionDataTypeId = orgToUpdate.ExtensionDataTypeId };
			SetOrgPropertiesFromControls(org);
			organizationApi.Save(org);
		}

		/// <summary>
		/// Reset organization form for continuously adding.
		/// </summary>
		public override void Reset()
		{
			if (this.AssociatedAreaSelector != null)
				this.AssociatedAreaSelector.SelectedItems = null;

			if (this.TextBoxOrganizationCode != null)
				this.TextBoxOrganizationCode.Text = "";

			if (this.TextBoxOrganizationName != null)
				this.TextBoxOrganizationName.Text = "";

			if (this.ParentOrganizationSelector != null)
				this.ParentOrganizationSelector.SelectedOrganization = null;

			if (this.DropDownListOrganizationType != null)
				this.DropDownListOrganizationType.SelectedValue = "";

			if (this.RadioButtonListOrganizationStatus != null)
				this.RadioButtonListOrganizationStatus.SelectedValue = OrganizationStatus.Enabled.ToString();

			this.OrganizationExtensionDataForm.ResetControlValuesToDefault();

			if (this.TextBoxOrganizationCode != null)
				ScriptManager.GetCurrent(HttpContext.Current.Handler as Page).SetFocus(this.TextBoxOrganizationCode);
			else if (this.TextBoxOrganizationName != null)
				ScriptManager.GetCurrent(HttpContext.Current.Handler as Page).SetFocus(this.TextBoxOrganizationName);
		}

		/// <summary>
		/// Load organization into the form in writable mode.
		/// </summary>
		/// <param name="entityId"></param>
		public override void LoadWritableEntity(string entityId)
		{
			Guid organizationId = new Guid(entityId);
			OrganizationObject org = organizationApi.GetOrganization(organizationId);
			if (org == null)
				throw new ValidationException(Resources.InvalidOrganizationID);

			this.SetControlsFromOrgProperties(org);
		}

		/// <summary>
		/// Load organization into the form in readonly mode.
		/// </summary>
		/// <param name="entityId"></param>
		public override void LoadReadOnlyEntity(string entityId)
		{
			this.LoadWritableEntity(entityId);
			WebUtility.MakeBindingControlsReadOnly(this);
		}

		/// <summary>
		/// Resolve organization extension data type id by default xml configured metadata type.
		/// </summary>
		/// <returns></returns>
		protected virtual Guid ResolveOrganizationExtensionDataTypeId()
		{
			string extensionTypeName = this.Domain;
			IObjectMetadata objectMetadata = metadataApi.GetType(extensionTypeName);
			return objectMetadata != null ? objectMetadata.Id : Guid.Empty;
		}

		/// <summary>
		/// Set organization properties from controls for saving the organization.
		/// </summary>
		/// <param name="org"></param>
		protected virtual void SetOrgPropertiesFromControls(OrganizationObject org)
		{
			if (this.AssociatedAreaSelector != null && this.AssociatedAreaSelector.SelectedItems != null && this.AssociatedAreaSelector.SelectedItems.Count() > 0)
				org.Hierarchies[platformConfiguration.AreaHierarchyTypeValue] = this.AssociatedAreaSelector.SelectedItems.FirstOrDefault().Id;
			else
				org.Hierarchies.Remove(platformConfiguration.AreaHierarchyTypeValue);

			if (this.TextBoxOrganizationCode != null)
				org.OrganizationCode = this.TextBoxOrganizationCode.Text;

			if (this.TextBoxOrganizationName != null)
				org.OrganizationName = this.TextBoxOrganizationName.Text;

			if (this.ParentOrganizationSelector != null)
				org.ParentOrganizationId = this.ParentOrganizationSelector.SelectedOrganization != null ? this.ParentOrganizationSelector.SelectedOrganization.OrganizationId : (Guid?)null;

			if (this.DropDownListOrganizationType != null)
				org.OrganizationTypeId = new Guid(this.DropDownListOrganizationType.SelectedValue);

			if (this.RadioButtonListOrganizationStatus != null)
				org.Status = (OrganizationStatus)Enum.Parse(typeof(OrganizationStatus), this.RadioButtonListOrganizationStatus.SelectedValue);

			if (this.TextBoxDescription != null)
				org.Description = this.TextBoxDescription.Text;

			this.OrganizationExtensionDataForm.SetObjectPropertiesFromControlValues(org);
		}

		/// <summary>
		/// Set controls from organization properties for UI displaying.
		/// </summary>
		/// <param name="org"></param>
		protected virtual void SetControlsFromOrgProperties(OrganizationObject org)
		{
			if (org.ParentOrganizationId.HasValue && this.ParentOrganizationSelector != null)
				this.ParentOrganizationSelector.SelectedOrganization = organizationApi.GetOrganization(org.ParentOrganizationId.Value);

			if (org.Hierarchies.ContainsKey(platformConfiguration.AreaHierarchyTypeValue) && org.Hierarchies[platformConfiguration.AreaHierarchyTypeValue] != null && this.AssociatedAreaSelector != null)
			{
				HierarchyDataObject hierarchyDataObject = hierarchyApi.GetHierarchyData(org.Hierarchies[platformConfiguration.AreaHierarchyTypeValue]);
				this.AssociatedAreaSelector.SelectedItems = new MyControls.HierarchyItem[] { new MyControls.HierarchyItem { Id = hierarchyDataObject.HierarchyDataId, Name = hierarchyDataObject.Name } };
			}

			if (this.TextBoxOrganizationCode != null)
				this.TextBoxOrganizationCode.Text = org.OrganizationCode;

			if (this.TextBoxOrganizationName != null)
				this.TextBoxOrganizationName.Text = org.OrganizationName;

			if (this.DropDownListOrganizationType != null)
				this.DropDownListOrganizationType.SelectedValue = org.OrganizationTypeId.ToString();

			if (this.RadioButtonListOrganizationStatus != null)
				this.RadioButtonListOrganizationStatus.SelectedValue = org.Status.ToString();

			if (this.TextBoxDescription != null)
				this.TextBoxDescription.Text = org.Description;

			this.OrganizationExtensionDataForm.SetControlValuesFromObjectProperties(org);

			if (this.PlaceHolderOperatorContext != null)
				this.PlaceHolderOperatorContext.Visible = true;

			if (this.UserLinkCreatedBy != null)
				this.UserLinkCreatedBy.UserId = org.CreatedBy.ToString();

			if (this.UserLinkLastModifiedBy != null)
				this.UserLinkLastModifiedBy.UserId = org.LastUpdatedBy.ToString();

			if (this.TextBoxCreatedOn != null)
				this.TextBoxCreatedOn.Text = LocalizationUtility.ToDateTimeString(org.CreatedDate);

			if (this.TextBoxLastModifiedOn != null)
				this.TextBoxLastModifiedOn.Text = LocalizationUtility.ToDateTimeString(org.LastUpdatedDate);
		}

		/// <summary>
		/// Validate whether the organization code, name and type are filled by the user correctly.
		/// Both the organization code and name should be unique in the application.
		/// </summary>
		/// <param name="organizationId"></param>
		protected virtual void ValidateDataInputForm(Guid? organizationId)
		{
			using (ValidationScope validationScope = new ValidationScope())
			{
				OrganizationObject org = null;
				if (this.TextBoxOrganizationCode != null && Kit.IsEmpty(this.TextBoxOrganizationCode.Text))
					validationScope.Error(Resources.OrganizationCodeCannotBeEmpty);
				else
				{
					org = organizationApi.GetOrganizationByCode(this.TextBoxOrganizationCode.Text);
					if ((!organizationId.HasValue && org != null)
						|| (organizationId.HasValue && org != null && org.OrganizationId != organizationId.Value))
						validationScope.Error(Resources.DuplicateOrganizationCode, this.TextBoxOrganizationCode.Text);
				}

				if (this.TextBoxOrganizationName != null && Kit.IsEmpty(this.TextBoxOrganizationName.Text))
					validationScope.Error(Resources.OrganizationNameCannotBeEmpty);
				else
				{
					org = organizationApi.GetOrganizationByCode(this.TextBoxOrganizationName.Text);
					if ((!organizationId.HasValue && org != null)
						|| (organizationId.HasValue && org != null && org.OrganizationId != organizationId.Value))
						validationScope.Error(Resources.DuplicateOrganizationName, this.TextBoxOrganizationName.Text);
				}

				if (this.DropDownListOrganizationType != null && Kit.IsEmpty(this.DropDownListOrganizationType.SelectedValue))
					validationScope.Error(Resources.OrganizationTypeCannotBeEmpty);
			}
		}
	}
}
