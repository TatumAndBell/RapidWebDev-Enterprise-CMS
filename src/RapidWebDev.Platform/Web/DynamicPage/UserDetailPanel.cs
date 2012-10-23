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
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Common.Data;
using RapidWebDev.ExtensionModel.Web.Controls;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Platform.Properties;
using RapidWebDev.Platform.Web.Controls;
using RapidWebDev.UI.DynamicPages;
using AspNetMembership = System.Web.Security.Membership;
using PermissionTreeView = RapidWebDev.Platform.Web.Controls.PermissionTreeView;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using System.Globalization;

namespace RapidWebDev.Platform.Web.DynamicPage
{
    /// <summary>
    /// User detail panel page handler.
    /// </summary>
    public class UserDetailPanel : DetailPanelPage
    {
        private static object syncObject = new object();
        /// <summary>
        /// Protected authentication context
        /// </summary>
        protected static readonly IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
        /// <summary>
        /// Protected membership Api
        /// </summary>
        protected static readonly IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
        /// <summary>
        /// Protected platform configuration
        /// </summary>
        protected static readonly IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
        /// <summary>
        /// Protected role Api
        /// </summary>
        protected static readonly IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
        /// <summary>
        /// Protected permission Api
        /// </summary>
        protected static readonly IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();
        /// <summary>
        /// Protected organization Api
        /// </summary>
        protected static readonly IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

        /// <summary>
        /// Protected metadata Api
        /// </summary>
        protected static readonly IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();

        private List<CheckBox> dynamicGeneratedCheckBox = new List<CheckBox>();

        #region Protected Web Controls

        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected OrganizationSelector OrganizationSelector;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxUserName;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected PlaceHolder PlaceHolderPassword;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxPassword;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxConfirmPassword;

        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxDisplayName;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxEmail;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxMobile;

        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected ExtensionDataForm UserExtensionDataForm;

        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxComment;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected RadioButtonList RadioButtonListStatus;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected PlaceHolder PlaceHolderOperateContext;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxCreationDate;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxLastLoginDate;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxLastActivityDate;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxLockedOutDate;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxLastPasswordChangedDate;
        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected TextBox TextBoxLastUpdatedDate;

        /// <summary />
        [Binding("TabContainer/TabPanelProfile")]
        protected Panel PanelRoleContainer;

        /// <summary />
        [Binding("TabContainer/TabPanelPermission")]
        protected PermissionTreeView @PermissionTreeView;

        /// <summary />
        [Binding("TabContainer")]
        protected AjaxControlToolkit.TabPanel TabPanelProfile;
        /// <summary />
        [Binding("TabContainer")]
        protected AjaxControlToolkit.TabPanel TabPanelPermission;

        #endregion

        /// <summary>
        /// Gets organization domain
        /// </summary>
        protected string Domain
        {
            get { return authenticationContext.TempVariables["Domain.Value"] as string; }
        }

        /// <summary>
        /// Create a new user from detail panel and return the id.
        /// The method needs to create a new entity and set control values to its properties then persist it.
        /// </summary>
        /// <returns>returns the user id after it's created successfully.</returns>
        public override string Create()
        {
            this.ValidateDataInputForm(null);

            using (TransactionScope ts = new TransactionScope())
            {
                Guid extensionTypeId = this.ResolveUserExtensionDataTypeId();
                UserObject userObject = new UserObject { ExtensionDataTypeId = extensionTypeId };
                if (this.UserExtensionDataForm != null)
                    this.UserExtensionDataForm.SetObjectPropertiesFromControlValues(userObject);

                this.SetUserPropertiesFromControls(userObject);

                string password = null;
                if (this.TextBoxPassword != null)
                    password = this.TextBoxPassword.Text;

                membershipApi.Save(userObject, password, null);
                Guid userId = userObject.UserId;

                if (this.OrganizationSelector != null)
                {
                    OrganizationObject organizationObject = organizationApi.GetOrganization(this.OrganizationSelector.SelectedOrganization.OrganizationId);
                    IEnumerable<Guid> roleIds = this.ResolveSelectedRoleCheckBox(organizationObject);
                    roleApi.SetUserToRoles(userId, roleIds);
                }

                if (this.PermissionTreeView != null)
                    permissionApi.SetUserPermissions(userId, @PermissionTreeView.CheckedValues);

                ts.Complete();
                return userId.ToString();
            }
        }

        /// <summary>
        /// Update an existed organization type from detail panel.
        /// The method needs to load an existed entity by specified id and set control values to overwrite its original properties then persist it.
        /// </summary>
        /// <param name="entityId"></param>
        public override void Update(string entityId)
        {
            this.ValidateDataInputForm(new Guid(entityId));

            using (TransactionScope ts = new TransactionScope())
            {
                UserObject userObject = membershipApi.Get(new Guid(entityId));
                userObject.ExtensionDataTypeId = this.ResolveUserExtensionDataTypeId();
                if (this.UserExtensionDataForm != null)
                    this.UserExtensionDataForm.SetObjectPropertiesFromControlValues(userObject);

                this.SetUserPropertiesFromControls(userObject);

                string password = null;
                if (this.TextBoxPassword != null)
                    password = this.TextBoxPassword.Text;

                membershipApi.Save(userObject, password, null);

                if (this.OrganizationSelector != null)
                {
                    OrganizationObject organizationObject = organizationApi.GetOrganization(this.OrganizationSelector.SelectedOrganization.OrganizationId);
                    IEnumerable<Guid> roleIds = this.ResolveSelectedRoleCheckBox(organizationObject);
                    roleApi.SetUserToRoles(userObject.UserId, roleIds);
                }

                if (this.PermissionTreeView != null)
                    permissionApi.SetUserPermissions(userObject.UserId, @PermissionTreeView.CheckedValues);

                ts.Complete();
            }
        }

        /// <summary>
        /// Reset all controls of the detail panel to initial state.
        /// The method will be invoked when enables the detail panel to support creating entities continuously.
        /// After an entity been created, the method will be invoked to reset form controls for another input.
        /// </summary>
        public override void Reset()
        {
            if (this.OrganizationSelector != null)
                this.OrganizationSelector.SelectedOrganization = null;

            if (this.TextBoxUserName != null)
                this.TextBoxUserName.Text = "";

            if (this.TextBoxPassword != null)
                this.TextBoxPassword.Text = "";

            if (this.TextBoxConfirmPassword != null)
                this.TextBoxConfirmPassword.Text = "";

            if (this.TextBoxDisplayName != null)
                this.TextBoxDisplayName.Text = "";

            if (this.TextBoxEmail != null)
                this.TextBoxEmail.Text = "";

            if (this.TextBoxMobile != null)
                this.TextBoxMobile.Text = "";

            this.UserExtensionDataForm.ResetControlValuesToDefault();

            if (this.TextBoxComment != null)
                this.TextBoxComment.Text = "";

            if (this.RadioButtonListStatus != null)
                this.RadioButtonListStatus.SelectedValue = true.ToString();

            if (this.PlaceHolderOperateContext != null)
                this.PlaceHolderOperateContext.Visible = false;

            if (this.TextBoxCreationDate != null)
                this.TextBoxCreationDate.Text = "";

            if (this.TextBoxLastLoginDate != null)
                this.TextBoxLastLoginDate.Text = "";

            if (this.TextBoxLastActivityDate != null)
                this.TextBoxLastActivityDate.Text = "";

            if (this.TextBoxLockedOutDate != null)
                this.TextBoxLockedOutDate.Text = "";

            if (this.TextBoxLastPasswordChangedDate != null)
                this.TextBoxLastPasswordChangedDate.Text = "";

            if (this.TextBoxLastUpdatedDate != null)
                this.TextBoxLastUpdatedDate.Text = "";

            if (this.PanelRoleContainer != null)
                this.PanelRoleContainer.Controls.Clear();

            if (this.PermissionTreeView != null)
                this.PermissionTreeView.CheckedValues = new string[0];

            if (this.TextBoxUserName != null)
                ScriptManager.GetCurrent(HttpContext.Current.Handler as Page).SetFocus(this.TextBoxUserName);
        }

        /// <summary>
        /// The method is designed to load entity by id to editable detail panel controls.
        /// </summary>
        /// <param name="entityId"></param>
        public override void LoadWritableEntity(string entityId)
        {
            Guid userId = new Guid(entityId);

            UserObject userObject = membershipApi.Get(userId);
            Guid organizationId = userObject.OrganizationId;
            OrganizationObject organizationObject = organizationId != Guid.Empty ? organizationApi.GetOrganization(organizationId) : null;

            if (this.OrganizationSelector != null)
                this.OrganizationSelector.SelectedOrganization = organizationObject;

            this.CreateRoleCheckBoxes(organizationObject);

            if (this.TextBoxUserName != null)
                this.TextBoxUserName.Text = userObject.UserName;

            if (this.TextBoxPassword != null)
                this.TextBoxPassword.Text = "";

            if (this.TextBoxConfirmPassword != null)
                this.TextBoxConfirmPassword.Text = "";

            IEnumerable<Guid> roleIds = roleApi.FindByUserId(userObject.UserId).Select(roleObject => roleObject.RoleId);
            this.SetRoleCheckBoxSelection(roleIds);

            if (this.TextBoxDisplayName != null)
                this.TextBoxDisplayName.Text = userObject.DisplayName;

            if (this.TextBoxEmail != null)
                this.TextBoxEmail.Text = userObject.Email;

            if (this.TextBoxMobile != null)
                this.TextBoxMobile.Text = userObject.MobilePin;

            if (this.TextBoxComment != null)
                this.TextBoxComment.Text = userObject.Comment;

            if (this.RadioButtonListStatus != null)
                this.RadioButtonListStatus.SelectedValue = userObject.IsApproved.ToString();

            // extension properties
            if (this.UserExtensionDataForm != null)
                this.UserExtensionDataForm.SetControlValuesFromObjectProperties(userObject);

            if (this.PlaceHolderOperateContext != null)
                this.PlaceHolderOperateContext.Visible = true;
            DateTime emptyStringWhenEarlierEqualThanDate = new DateTime(1970, 1, 1);
            if (this.TextBoxCreationDate != null)
                this.TextBoxCreationDate.Text = LocalizationUtility.ToDateTimeString(userObject.CreationDate, emptyStringWhenEarlierEqualThanDate);
            if (this.TextBoxLastLoginDate != null)
                this.TextBoxLastLoginDate.Text = LocalizationUtility.ToDateTimeString(userObject.LastLoginDate, emptyStringWhenEarlierEqualThanDate);
            if (this.TextBoxLastActivityDate != null)
                this.TextBoxLastActivityDate.Text = LocalizationUtility.ToDateTimeString(userObject.LastActivityDate, emptyStringWhenEarlierEqualThanDate);
            if (this.TextBoxLockedOutDate != null)
                this.TextBoxLockedOutDate.Text = LocalizationUtility.ToDateTimeString(userObject.LastLockoutDate, emptyStringWhenEarlierEqualThanDate);
            if (this.TextBoxLastPasswordChangedDate != null)
                this.TextBoxLastPasswordChangedDate.Text = LocalizationUtility.ToDateTimeString(userObject.LastPasswordChangedDate, emptyStringWhenEarlierEqualThanDate);
            if (this.TextBoxLastUpdatedDate != null)
                this.TextBoxLastUpdatedDate.Text = LocalizationUtility.ToDateTimeString(userObject.LastUpdatedDate, emptyStringWhenEarlierEqualThanDate);

            if (this.PermissionTreeView != null)
            {
                IEnumerable<PermissionObject> permissions = permissionApi.FindUserPermissions(userObject.UserId, true);
                this.PermissionTreeView.CheckedValues = permissions.Select(p => p.PermissionValue);
            }
        }

        /// <summary>
        /// The method is designed to load entity by id to readonly detail panel controls.
        /// </summary>
        /// <param name="entityId"></param>
        public override void LoadReadOnlyEntity(string entityId)
        {
            base.LoadReadOnlyEntity(entityId);

            if (this.PanelRoleContainer != null)
            {
                // if there has roles not associated with organization type but associated with the user by business requirements,
                // here should load roles explicitly associated with the user and merge them with roles binding to the organization type.
                IEnumerable<RoleObject> rolesAssociatedWithUser = roleApi.FindByUserId(new Guid(entityId));
                foreach (RoleObject roleAssociatedWithUser in rolesAssociatedWithUser)
                {
                    string checkBoxId = string.Format("ID{0}", roleAssociatedWithUser.RoleId);
                    if (this.dynamicGeneratedCheckBox.Count(cb => cb.ID == checkBoxId) == 0)
                    {
                        CheckBox checkBox4TheRoleAssociatedWithUser = new CheckBox
                        {
                            ID = checkBoxId,
                            Text = roleAssociatedWithUser.RoleName,
                            Checked = true
                        };

                        this.dynamicGeneratedCheckBox.Add(checkBox4TheRoleAssociatedWithUser);
                        this.PanelRoleContainer.Controls.Add(checkBox4TheRoleAssociatedWithUser);
                    }
                }

                // remove unchecked role checkbox in view UI.
                this.dynamicGeneratedCheckBox.ForEach(checkbox => checkbox.Enabled = false);
                foreach (CheckBox uncheckedCheckBox in this.dynamicGeneratedCheckBox.Where(cb => !cb.Checked))
                    this.PanelRoleContainer.Controls.Remove(uncheckedCheckBox);
            }

            if (this.TabPanelPermission != null)
                this.TabPanelPermission.Visible = false;

            if (this.PlaceHolderPassword != null)
                this.PlaceHolderPassword.Visible = false;
        }

        /// <summary>
        /// The method will be invoked when detail panel is loaded.
        /// The implementation can resolve current visiting Page from the conversation as (HttpContext.Current.Handler as Page).
        /// </summary>
        /// <param name="sender">The web page which contains the detail panel.</param>
        /// <param name="e">Callback event argument.</param>
        public override void OnLoad(IRequestHandler sender, DetailPanelPageEventArgs e)
        {
            if (this.UserExtensionDataForm != null)
            {
                Guid extensionDataTypeId = this.ResolveUserExtensionDataTypeId();
                this.UserExtensionDataForm.CreateDataInputForm(extensionDataTypeId);
            }

            if (this.OrganizationSelector != null)
            {
                this.RegisterClientAssociationBetweenOrganizationAndRole();

                this.OrganizationSelector.Domain = this.Domain;
                if (this.OrganizationSelector.SelectedOrganization != null)
                {
                    OrganizationObject organizationObject = this.OrganizationSelector.SelectedOrganization;
                    this.CreateRoleCheckBoxes(organizationObject);

                    IEnumerable<Guid> selectedRoleIds = this.ResolveSelectedRoleCheckBox(organizationObject);
                    this.SetRoleCheckBoxSelection(selectedRoleIds);
                }
            }
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
        /// Resolve user extension data type id by default xml configured metadata type.
        /// </summary>
        /// <returns></returns>
        protected virtual Guid ResolveUserExtensionDataTypeId()
        {
            string extensionTypeName = string.Format(CultureInfo.InvariantCulture, "{0}User", this.Domain);
            IObjectMetadata objectMetadata = metadataApi.GetType(extensionTypeName);
            return objectMetadata != null ? objectMetadata.Id : Guid.Empty;
        }

        /// <summary>
        /// Set controls from user fixed properties, e.g. UserName, DisplayName, Email, Comment, IsApproved, Mobile and OrganizationId.
        /// </summary>
        /// <param name="userObject"></param>
        protected virtual void SetUserPropertiesFromControls(UserObject userObject)
        {
            if (this.TextBoxUserName != null)
                userObject.UserName = this.TextBoxUserName.Text;
            if (this.TextBoxDisplayName != null)
                userObject.DisplayName = this.TextBoxDisplayName.Text;
            if (this.TextBoxEmail != null)
                userObject.Email = this.TextBoxEmail.Text;
            if (this.TextBoxComment != null)
                userObject.Comment = this.TextBoxComment.Text;
            if (this.RadioButtonListStatus != null)
                userObject.IsApproved = bool.Parse(RadioButtonListStatus.SelectedValue);
            if (this.TextBoxMobile != null)
                userObject.MobilePin = this.TextBoxMobile.Text;
            if (this.OrganizationSelector != null)
                userObject.OrganizationId = this.OrganizationSelector.SelectedOrganization.OrganizationId;
        }

        /// <summary>
        /// Validate data input from client.
        /// </summary>
        /// <param name="userId"></param>
        protected virtual void ValidateDataInputForm(Guid? userId)
        {
            using (ValidationScope validationScope = new ValidationScope())
            {
                int recordCount;

                if (this.OrganizationSelector != null && this.OrganizationSelector.SelectedOrganization == null)
                    validationScope.Error(Resources.OrganizationCannotBeEmpty);

                if (this.TextBoxUserName != null && this.TextBoxUserName.Text.Trim().Length == 0)
                    validationScope.Error(Resources.UserNameCannotBeEmpty);
                else if (this.TextBoxUserName != null)
                {
                    LinqPredicate linqPredicate = new LinqPredicate("UserName=@0 AND UserId!=@1", this.TextBoxUserName.Text, userId.HasValue ? userId.Value : Guid.NewGuid());
                    membershipApi.FindUsers(linqPredicate, null, 0, 1, out recordCount);
                    if (recordCount > 0)
                        validationScope.Error(Resources.DuplicateUserName, this.TextBoxUserName.Text);
                }

                if (this.TextBoxPassword != null && !userId.HasValue && this.TextBoxPassword.Text.Trim().Length == 0)
                    validationScope.Error(Resources.PasswordCannotBeEmpty);

                if (this.TextBoxPassword != null && this.TextBoxPassword.Text.Trim().Length > 0)
                {
                    if (this.TextBoxPassword.Text.Trim() != this.TextBoxConfirmPassword.Text.Trim())
                        validationScope.Error(Resources.PasswordNotEqualToConfirmPassword);
                    else
                    {
                        if (this.TextBoxPassword.Text.Trim().Length < AspNetMembership.MinRequiredPasswordLength)
                            validationScope.Error(Resources.PasswordLengthLessThanRequired, AspNetMembership.MinRequiredPasswordLength);

                        if (!Kit.IsEmpty(AspNetMembership.PasswordStrengthRegularExpression))
                        {
                            Regex regex = new Regex(AspNetMembership.PasswordStrengthRegularExpression, RegexOptions.Compiled);
                            if (!regex.IsMatch(TextBoxPassword.Text.Trim()))
                                validationScope.Error(Resources.PasswordFormatIsInvalid, AspNetMembership.PasswordStrengthRegularExpression);
                        }
                    }
                }

                if (this.TextBoxDisplayName != null && this.TextBoxDisplayName.Text.Trim().Length == 0)
                    validationScope.Error(Resources.DisplayNameCannotBeEmpty);
                else if (this.TextBoxDisplayName != null)
                {
                    LinqPredicate linqPredicate = new LinqPredicate("DisplayName=@0 AND UserId!=@1", this.TextBoxDisplayName.Text, userId.HasValue ? userId.Value : Guid.NewGuid());
                    membershipApi.FindUsers(linqPredicate, null, 0, 1, out recordCount);
                    if (recordCount > 0)
                        validationScope.Error(Resources.DuplicateDisplayName, this.TextBoxDisplayName.Text);
                }
            }
        }

        #region Role CheckBox Populated by Organization Selection

        private void CreateRoleCheckBoxes(OrganizationObject organization)
        {
            if (this.PanelRoleContainer == null) return;

            this.PanelRoleContainer.Controls.Clear();
            if (organization == null) return;

            IEnumerable<RoleObject> roleObjects = this.FindRolesByOrganization(organization);
            foreach (RoleObject roleObject in roleObjects)
            {
                CheckBox checkbox = new CheckBox
                {
                    ID = string.Format("ID{0}", roleObject.RoleId),
                    Text = roleObject.RoleName
                };

                this.PanelRoleContainer.Controls.Add(checkbox);
                this.dynamicGeneratedCheckBox.Add(checkbox);
            }
        }

        private void SetRoleCheckBoxSelection(IEnumerable<Guid> roleIds)
        {
            if (this.PanelRoleContainer == null) return;

            foreach (CheckBox checkBox in this.PanelRoleContainer.Controls)
            {
                if (checkBox != null) checkBox.Checked = false;
            }

            foreach (Guid roleId in roleIds)
            {
                string checkBoxId = string.Format("ID{0}", roleId);
                CheckBox checkBox = this.PanelRoleContainer.FindControl(checkBoxId) as CheckBox;
                if (checkBox != null)
                    checkBox.Checked = true;
            }
        }

        private IEnumerable<Guid> ResolveSelectedRoleCheckBox(OrganizationObject organization)
        {
            List<Guid> roleIds = new List<Guid>();
            if (organization == null) return roleIds;

            IEnumerable<RoleObject> roleObjects = this.FindRolesByOrganization(organization);
            foreach (RoleObject roleObject in roleObjects)
            {
                string name = string.Format("{0}$ID{1}", TabPanelProfile.UniqueID, roleObject.RoleId);
                string checkBoxValue = HttpContext.Current.Request.Params[name];
                if (string.Equals(checkBoxValue, "on", StringComparison.InvariantCultureIgnoreCase))
                    roleIds.Add(roleObject.RoleId);
            }

            return roleIds;
        }

        private IEnumerable<RoleObject> FindRolesByOrganization(OrganizationObject organizationObject)
        {
            int recordCount;
			OrganizationTypeObject orgType = organizationApi.GetOrganizationType(organizationObject.OrganizationTypeId);
			LinqPredicate linqPredicate = new LinqPredicate("Domain=@0", orgType.Domain);
            IEnumerable<RoleObject> results = roleApi.FindRoles(linqPredicate, "RoleName ASC", 0, int.MaxValue, out recordCount);
			return results.ToList();
        }

        private void RegisterClientAssociationBetweenOrganizationAndRole()
        {
            if (this.OrganizationSelector != null && this.PanelRoleContainer != null)
            {
                string javascript = "window." + this.PanelRoleContainer.ClientID + " = new RolesByOrganization('" + this.PanelRoleContainer.ClientID + "', '" + this.TabPanelProfile.ClientID + "');";
                ScriptManager.RegisterStartupScript(this.OrganizationSelector, this.OrganizationSelector.GetType(), javascript.GetHashCode().ToString(), javascript, true);

                this.OrganizationSelector.SelectedOrganizationChangedCallback = string.Format("window.{0}.selectedOrganizationChangedCallback", this.PanelRoleContainer.ClientID);
            }
        }

        #endregion
    }
}
