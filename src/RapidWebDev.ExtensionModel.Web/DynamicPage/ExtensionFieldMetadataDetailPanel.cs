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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Validation;
using RapidWebDev.Common.Web;
using RapidWebDev.ExtensionModel.Web.Properties;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.ExtensionModel.Web.DynamicPage
{
	/// <summary>
	/// Extension field metadata detail panel page handler.
	/// </summary>
	public class ExtensionFieldMetadataDetailPanel : DetailPanelPage
	{
		private static IApplicationContext applicationContext = SpringContext.Current.GetObject<IApplicationContext>();
		private static IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
		private IExtensionFieldControlBuilder extensionFieldControlBuilder;

		/// <summary />
		[Binding]
		protected DropDownList DropDownListFieldMetadataType;
		/// <summary />
		[Binding]
		protected Panel PanelFieldMetadata;

		/// <summary>
		/// Create a new field metadata from detail panel and return the field name after created successfully.
		/// The method needs to create a new entity and set control values to its properties then persist it.
		/// </summary>
		/// <returns>returns the field name after created successfully.</returns>
		public override string Create()
		{
			if (this.extensionFieldControlBuilder == null)
				throw new ValidationException(Resources.SelectFieldTypeBeforeSave);

			IFieldMetadata fieldMetadata = this.extensionFieldControlBuilder.Metadata;
			if (string.IsNullOrEmpty(fieldMetadata.Name))
				throw new ValidationException(Resources.FieldNameCannotBeEmpty);

			if (metadataApi.GetField((Guid)applicationContext.TempVariables["MetadataDataTypeId"], fieldMetadata.Name) != null)
				throw new ValidationException(Resources.FieldNameDoesExist);

			try
			{
				metadataApi.SaveField((Guid)applicationContext.TempVariables["MetadataDataTypeId"], fieldMetadata);
				return fieldMetadata.Name;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Update an existed field metadata from detail panel.
		/// The method needs to load an existed entity by specified id and set control values to overwrite its original properties then persist it.
		/// </summary>
		/// <param name="entityId">this should be field name instead of id.</param>
		public override void Update(string entityId)
		{
			if (this.extensionFieldControlBuilder == null)
			{
				this.ShowMessage(MessageTypes.Warn, Resources.SelectFieldTypeBeforeSave);
				return;
			}

			IFieldMetadata fieldMetadata = this.extensionFieldControlBuilder.Metadata;
			if (string.IsNullOrEmpty(fieldMetadata.Name))
			{
				this.ShowMessage(MessageTypes.Warn, Resources.FieldNameCannotBeEmpty);
				return;
			}

			try
			{
				if (!string.Equals(fieldMetadata.Name, entityId, StringComparison.InvariantCultureIgnoreCase))
					metadataApi.DeleteField((Guid)applicationContext.TempVariables["MetadataDataTypeId"], entityId);

				metadataApi.SaveField((Guid)applicationContext.TempVariables["MetadataDataTypeId"], fieldMetadata);
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				this.ShowMessage(MessageTypes.Error, Resources.UnknownError);
			}
		}

		/// <summary>
		/// Reset all controls of the detail panel to initial state.
		/// The method will be invoked when enables the detail panel to support creating entities continuously.
		/// After an entity been created, the method will be invoked to reset form controls for another input.
		/// </summary>
		public override void Reset()
		{
			this.DropDownListFieldMetadataType.SelectedValue = "";
			this.PanelFieldMetadata.Controls.Clear();
			this.extensionFieldControlBuilder = null;
		}

		/// <summary>
		/// The method is designed to load entity by id to editable detail panel controls.
		/// </summary>
		/// <param name="entityId"></param>
		public override void LoadWritableEntity(string entityId)
		{
			string fieldName = entityId;
			IFieldMetadata fieldMetadata = metadataApi.GetField((Guid)applicationContext.TempVariables["MetadataDataTypeId"], fieldName);
			if (fieldMetadata == null)
			{
				this.ShowMessage(MessageTypes.Error, Resources.FieldHasBeenDeletedByOtherUsers);
				return;
			}

			this.DropDownListFieldMetadataType.SelectedValue = fieldMetadata.Type.ToString();
			this.BuildFieldMetadataControl(fieldMetadata.Type);
			this.extensionFieldControlBuilder.Metadata = fieldMetadata;

			this.DropDownListFieldMetadataType.Enabled = false;
		}

		/// <summary>
		/// Make all editabel controls to be readonly.
		/// </summary>
		/// <param name="entityId"></param>
		public override void LoadReadOnlyEntity(string entityId)
		{
			base.LoadReadOnlyEntity(entityId);
			WebUtility.MakeBindingControlsReadOnly(this.extensionFieldControlBuilder);
		}

		/// <summary>
		/// The method will be invoked when detail panel is loaded.
		/// The implementation can resolve current visiting Page from the conversation as (HttpContext.Current.Handler as Page).
		/// </summary>
		/// <param name="sender">The web page which contains the detail panel.</param>
		/// <param name="e">Callback event argument.</param>
		public override void OnLoad(IRequestHandler sender, DetailPanelPageEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.DropDownListFieldMetadataType.SelectedValue))
			{
				FieldType fieldType = (FieldType)Enum.Parse(typeof(FieldType), this.DropDownListFieldMetadataType.SelectedValue);
				this.BuildFieldMetadataControl(fieldType);
			}

			ClientScripts.RegisterHeaderScriptInclude("~/resources/javascript/SelectionFieldMetadataControl.js");
		}

		/// <summary>
		/// Validate query string "MetadataDataTypeName" and setup metadata data type name to IAuthenticationContext.TempVariables.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			string metadataDataTypeName = QueryStringUtility.MetadataDataTypeName(sender);
			IObjectMetadata metadata = metadataApi.GetType(metadataDataTypeName);
			if (metadata == null)
				throw new ResourceNotFoundException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidMetadataTypeName, metadataDataTypeName));

			e.TempVariables["MetadataDataTypeId"] = metadata.Id;
			e.TempVariables["MetadataDataTypeName"] = metadata.Name;
		}

		private void BuildFieldMetadataControl(FieldType fieldType)
		{
			string metadataFieldControlTypeName = string.Format(CultureInfo.InvariantCulture, "RapidWebDev.ExtensionModel.Web.{0}ExtensionFieldControlBuilder, RapidWebDev.ExtensionModel.Web", fieldType);
			Type metadataFieldControlType = Kit.GetType(metadataFieldControlTypeName);
			this.extensionFieldControlBuilder = Activator.CreateInstance(metadataFieldControlType) as IExtensionFieldControlBuilder;
			Control metadataFieldControl = this.extensionFieldControlBuilder.BuildMetadataControl();
			this.PanelFieldMetadata.Controls.Add(metadataFieldControl);
		}
	}
}