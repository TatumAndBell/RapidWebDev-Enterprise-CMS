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
	/// Concrete data detail panel page handler.
	/// </summary>
	public class ConcreteDataDetailPanel : DetailPanelPage
	{
		private static object syncObject = new object();

		/// <summary>
		/// Protected authentication context.
		/// </summary>
		protected static readonly IAuthenticationContext authenticationContext  = SpringContext.Current.GetObject<IAuthenticationContext>();
		/// <summary>
		/// Protected concrete data Api.
		/// </summary>
		protected static readonly IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();

		#region Protected Web Controls

		/// <summary />
		[Binding]
		protected TextBox TextBoxName;
		/// <summary />
		[Binding]
		protected TextBox TextBoxValue;
		/// <summary />
		[Binding]
		protected RadioButtonList RadioButtonListStatus;
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
		/// Create a new concrete data from detail panel and return its id.
		/// The method needs to create a new entity and set control values to its properties then persist it.
		/// </summary>
		/// <returns>returns the id of new created concrete data.</returns>
		public override string Create()
		{
			this.ValidateInput(Guid.Empty);

			ConcreteDataObject concreteDataObject = new ConcreteDataObject();
			concreteDataObject.ExtensionDataTypeId = this.ResolveExtensionDataTypeId();
			concreteDataObject.Type = authenticationContext.TempVariables["ConcreteDataType"] as string;

			if (this.TextBoxName != null)
				concreteDataObject.Name = this.TextBoxName.Text;

			if (this.TextBoxValue != null)
				concreteDataObject.Value = this.TextBoxValue.Text;

			if (this.TextBoxDescription != null)
				concreteDataObject.Description = this.TextBoxDescription.Text;

			if (this.RadioButtonListStatus != null)
				concreteDataObject.DeleteStatus = (DeleteStatus)Enum.Parse(typeof(DeleteStatus), this.RadioButtonListStatus.SelectedValue);

			if (this.ExtensionDataForm != null)
				this.ExtensionDataForm.SetObjectPropertiesFromControlValues(concreteDataObject);

			concreteDataApi.Save(concreteDataObject);
			return concreteDataObject.ConcreteDataId.ToString();
		}

		/// <summary>
		/// Update an existed concrete data from detail panel.
		/// The method needs to load an existed entity by specified id and set control values to overwrite its original properties then persist it.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Update(string entityId)
		{
			Guid concreteDataId = new Guid(entityId);
			this.ValidateInput(concreteDataId);
			ConcreteDataObject concreteDataObject = concreteDataApi.GetById(concreteDataId);
			if (concreteDataObject == null)
				throw new ValidationException(Resources.InvalidConcreteDataID);

			concreteDataObject = new ConcreteDataObject { ConcreteDataId = concreteDataId };
			concreteDataObject.ExtensionDataTypeId = this.ResolveExtensionDataTypeId();
			concreteDataObject.Type = authenticationContext.TempVariables["ConcreteDataType"] as string;

			if (this.TextBoxName != null)
				concreteDataObject.Name = this.TextBoxName.Text;

			if (this.TextBoxValue != null)
				concreteDataObject.Value = this.TextBoxValue.Text;

			if (this.RadioButtonListStatus != null)
				concreteDataObject.DeleteStatus = (DeleteStatus)Enum.Parse(typeof(DeleteStatus), this.RadioButtonListStatus.SelectedValue);

			if (this.TextBoxDescription != null)
				concreteDataObject.Description = this.TextBoxDescription.Text;

			if (this.ExtensionDataForm != null)
				this.ExtensionDataForm.SetObjectPropertiesFromControlValues(concreteDataObject);

			concreteDataApi.Save(concreteDataObject);
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

			if (this.TextBoxValue != null)
				this.TextBoxValue.Text = "";

			if (this.TextBoxDescription != null)
				this.TextBoxDescription.Text = "";

			if (this.ExtensionDataForm != null)
				this.ExtensionDataForm.ResetControlValuesToDefault();
		}

		/// <summary>
		/// The method is designed to load entity by id to editable detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		public override void LoadWritableEntity(string entityId)
		{
			ConcreteDataObject concreteDataObject = concreteDataApi.GetById(new Guid(entityId));
			if (concreteDataObject == null) return;

			if (this.TextBoxName != null)
				this.TextBoxName.Text = concreteDataObject.Name;

			if (this.TextBoxValue != null)
				this.TextBoxValue.Text = concreteDataObject.Value;

			if (this.RadioButtonListStatus != null)
				this.RadioButtonListStatus.SelectedValue = concreteDataObject.DeleteStatus.ToString();

			if (this.TextBoxDescription != null)
				this.TextBoxDescription.Text = concreteDataObject.Description;

			if (this.ExtensionDataForm != null)
				this.ExtensionDataForm.SetControlValuesFromObjectProperties(concreteDataObject);

			if (this.PlaceHolderOperatorContext != null)
				this.PlaceHolderOperatorContext.Visible = true;

			if (this.UserLinkCreatedBy != null)
				this.UserLinkCreatedBy.UserId = concreteDataObject.CreatedBy.ToString();

			if (this.UserLinkLastUpdatedBy != null)
				this.UserLinkLastUpdatedBy.UserId = concreteDataObject.LastUpdatedBy.ToString();

			if (this.TextBoxCreatedDate != null)
				this.TextBoxCreatedDate.Text = LocalizationUtility.ToDateTimeString(concreteDataObject.CreatedDate);

			if (this.TextBoxLastUpdatedDate != null && concreteDataObject.LastUpdatedDate.HasValue)
				this.TextBoxLastUpdatedDate.Text = LocalizationUtility.ToDateTimeString(concreteDataObject.LastUpdatedDate.Value);
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
		}

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// Set domain into http context when web page is initializing.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			SetupContextTempVariablesUtility.SetupConcreteDataType(sender, e, true);
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
		/// Validate data input
		/// </summary>
		/// <param name="concreteDataObjectId"></param>
		protected virtual void ValidateInput(Guid concreteDataObjectId)
		{
			using (ValidationScope validationScope = new ValidationScope())
			{
				if (Kit.IsEmpty(this.TextBoxName.Text.Trim()))
					validationScope.Error(Resources.ConcreteDataNameCannotBeEmpty);
				else
				{
					ConcreteDataObject concreteDataObject = concreteDataApi.GetByName(authenticationContext.TempVariables["ConcreteDataType"] as string, this.TextBoxName.Text.Trim());
					if (concreteDataObject != null && concreteDataObject.ConcreteDataId != concreteDataObjectId)
						validationScope.Error(Resources.DuplicateConcreteDataName, this.TextBoxName.Text.Trim());
				}
			}
		}
	}
}

