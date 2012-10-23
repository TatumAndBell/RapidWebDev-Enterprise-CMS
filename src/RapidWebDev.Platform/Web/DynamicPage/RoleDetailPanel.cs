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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.Platform.Properties;
using RapidWebDev.UI.DynamicPages;
using PermissionTreeView = RapidWebDev.Platform.Web.Controls.PermissionTreeView;
using RapidWebDev.Common.Validation;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Role detail panel page handler.
	/// </summary>
	public class RoleDetailPanel : DetailPanelPage
	{
		private static object syncObject = new object();
		private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		private static IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();
		private static IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();
		private static IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

		/// <summary />
		[Binding("TabContainer/TabPanelProfile")]
		protected TextBox TextBoxName;
		/// <summary />
		[Binding("TabContainer/TabPanelProfile")]
		protected TextBox TextBoxDescription;
		/// <summary />
		[Binding("TabContainer/TabPanelPermission")]
		protected PermissionTreeView PermissionTreeView;

		/// <summary>
		/// Create a new role from detail panel and return the id.
		/// The method needs to create a new entity and set control values to its properties then persist it.
		/// </summary>
		/// <returns>returns the id of new created role.</returns>
		public override string Create()
		{
			this.ValidateInput(Guid.Empty);

			RoleObject roleObject = new RoleObject
			{
				RoleName = this.TextBoxName.Text,
				Description = this.TextBoxDescription.Text,
				Domain = authenticationContext.TempVariables["Domain.Value"] as string,
				Predefined = false
			};

			using (TransactionScope ts = new TransactionScope())
			{
				roleApi.Save(roleObject);
				permissionApi.SetRolePermissions(roleObject.RoleId, this.PermissionTreeView.CheckedValues);
				ts.Complete();
			}

			return roleObject.RoleId.ToString();
		}

		/// <summary>
		/// Update an existed organization type from detail panel.
		/// The method needs to load an existed entity by specified id and set control values to overwrite its original properties then persist it.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Update(string entityId)
		{
			Guid roleId = new Guid(entityId);
			this.ValidateInput(roleId);

			RoleObject roleObject = new RoleObject { RoleId = roleId };
			roleObject.RoleName = this.TextBoxName.Text;
			roleObject.Description = this.TextBoxDescription.Text;
			roleObject.Domain = authenticationContext.TempVariables["Domain.Value"] as string;

			using (TransactionScope ts = new TransactionScope())
			{
				roleApi.Save(roleObject);
				permissionApi.SetRolePermissions(roleObject.RoleId, this.PermissionTreeView.CheckedValues);
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
			this.TextBoxName.Text = "";
			this.TextBoxDescription.Text = "";
			ScriptManager.GetCurrent(HttpContext.Current.Handler as Page).SetFocus(this.TextBoxName);
			this.PermissionTreeView.CheckedValues = new List<string>();
		}

		/// <summary>
		/// The method is designed to load entity by id to editable detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		public override void LoadWritableEntity(string entityId)
		{
			var roleObject = roleApi.Get(new Guid(entityId));
			if (roleObject == null) return;

			this.TextBoxName.Text = roleObject.RoleName;
			this.TextBoxDescription.Text = roleObject.Description;

			IEnumerable<PermissionObject> permissions = permissionApi.FindRolePermissions(roleObject.RoleId);
			this.PermissionTreeView.CheckedValues = permissions.Select(p => p.PermissionValue);
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

		private void ValidateInput(Guid roleId)
		{
			using (ValidationScope validationScope = new ValidationScope())
			{
				if (Kit.IsEmpty(this.TextBoxName.Text.Trim()))
					validationScope.Error(Resources.RoleNameCannotBeEmpty);
				else
				{
					var roleObject = roleApi.Get(this.TextBoxName.Text.Trim());
					if (roleObject != null && roleObject.RoleId != roleId)
						validationScope.Error(Resources.DuplicateRoleName, this.TextBoxName.Text.Trim());
				}
			}
		}
	}
}

