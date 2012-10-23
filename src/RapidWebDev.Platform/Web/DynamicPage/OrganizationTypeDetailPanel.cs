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
using RapidWebDev.Common;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Platform.Properties;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using WebControls = System.Web.UI.WebControls;
using RapidWebDev.Common.Validation;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Organization type detail panel page handler.
	/// </summary>
	public class OrganizationTypeDetailPanel : DetailPanelPage
	{
		private static object syncObject = new object();

		/// <summary>
		/// Protected authentication context.
		/// </summary>
		protected static readonly IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		/// <summary>
		/// Protected organization Api.
		/// </summary>
		protected static readonly IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>(); 

		/// <summary />
		[Binding]
		protected WebControls.DropDownList DropDownListDomain;
		/// <summary />
		[Binding]
		protected TextBox TextBoxName;
		/// <summary />
		[Binding]
		protected TextBox TextBoxDescription;

		/// <summary>
		/// Create a new organization type from detail panel and return its id.
		/// The method needs to create a new entity and set control values to its properties then persist it.
		/// </summary>
		/// <returns>returns the id of new created organization type.</returns>
		public override string Create()
		{
			this.ValidateInput(Guid.Empty);

			string domainValue = this.DropDownListDomain != null ? this.DropDownListDomain.SelectedValue : authenticationContext.TempVariables["Domain.Value"] as string;
			OrganizationTypeObject organizationTypeObject = new OrganizationTypeObject
			{
				Name = this.TextBoxName.Text,
				Description = this.TextBoxDescription.Text,
				Domain = domainValue,
				Predefined = false
			};

			organizationApi.Save(organizationTypeObject);
			return organizationTypeObject.OrganizationTypeId.ToString();
		}

		/// <summary>
		/// Update an existed organization type from detail panel.
		/// The method needs to load an existed entity by specified id and set control values to overwrite its original properties then persist it.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Update(string entityId)
		{
			Guid organizationTypeId = new Guid(entityId);
			this.ValidateInput(organizationTypeId);

			string domainValue = this.DropDownListDomain != null ? this.DropDownListDomain.SelectedValue : authenticationContext.TempVariables["Domain.Value"] as string;
			OrganizationTypeObject organizationTypeObject = new OrganizationTypeObject { OrganizationTypeId = organizationTypeId };
			organizationTypeObject.Name = this.TextBoxName.Text;
			organizationTypeObject.Description = this.TextBoxDescription.Text;
			organizationTypeObject.Domain = domainValue;
			organizationApi.Save(organizationTypeObject);
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
		}

		/// <summary>
		/// The method is designed to load entity by id to editable detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		public override void LoadWritableEntity(string entityId)
		{
			var organizationType = organizationApi.GetOrganizationType(new Guid(entityId));
			if (organizationType == null) return;

			if (this.DropDownListDomain != null)
				this.DropDownListDomain.SelectedValue = organizationType.Domain;

			this.TextBoxName.Text = organizationType.Name;
			this.TextBoxDescription.Text = organizationType.Description;
		}

		/// <summary>
		/// The method will be invoked when detail panel is loaded.
		/// The implementation can resolve current visiting Page from the conversation as (HttpContext.Current.Handler as Page).
		/// </summary>
		/// <param name="sender">The web page which contains the detail panel.</param>
		/// <param name="e">Callback event argument.</param>
		public override void OnLoad(IRequestHandler sender, DetailPanelPageEventArgs e)
		{
			if (!sender.IsPostBack && this.DropDownListDomain != null)
			{
				this.DropDownListDomain.Items.Clear();
				this.DropDownListDomain.Items.Add(new WebControls.ListItem("", ""));
				IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
				foreach (OrganizationDomain domain in platformConfiguration.Domains)
					this.DropDownListDomain.Items.Add(new WebControls.ListItem(domain.Text, domain.Value));
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
			SetupContextTempVariablesUtility.SetupOrganizationDomain(sender, e, false);
		}

		/// <summary>
		/// Validate organization type name and domain.
		/// </summary>
		/// <param name="organizationTypeId"></param>
		protected virtual void ValidateInput(Guid organizationTypeId)
		{
			using (ValidationScope validationScope = new ValidationScope())
			{
				if (Kit.IsEmpty(this.TextBoxName.Text.Trim()))
					validationScope.Error(Resources.OrganizationTypeNameCannotBeEmpty);
				else
				{
					OrganizationTypeObject organizationTypeObject = organizationApi.GetOrganizationType(this.TextBoxName.Text.Trim());
					if (organizationTypeObject != null && organizationTypeObject.OrganizationTypeId != organizationTypeId)
						validationScope.Error(Resources.DuplicateOrganizationTypeName, this.TextBoxName.Text.Trim());
				}

				if (this.DropDownListDomain != null && Kit.IsEmpty(this.DropDownListDomain.SelectedValue))
					validationScope.Error(Resources.OrganizationTypeDomainCannotBeEmpty);
			}
		}
	}
}

