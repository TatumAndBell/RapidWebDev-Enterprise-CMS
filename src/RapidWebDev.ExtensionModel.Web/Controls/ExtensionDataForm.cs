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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.UI;

namespace RapidWebDev.ExtensionModel.Web.Controls
{
	/// <summary>
	/// Class to render a web form for users to input data for specified extension type.
	/// </summary>
	public class ExtensionDataForm : PlaceHolder, INamingContainer, ITextControl
	{
		private static IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();

		private bool readOnly;
		private IExtensionDataFormLayout extensionDataFormLayout;
		private IPermissionBridge permissionBridge;
		private Guid extensionDataTypeId;
		private Dictionary<IFieldMetadata, IExtensionFieldControlBuilder> fieldMetadataControlBuilders = new Dictionary<IFieldMetadata, IExtensionFieldControlBuilder>();

		/// <summary>
		/// Sets or gets true if the data form is readonly, defaults to false.
		/// </summary>
		public bool ReadOnly
		{
			get { return this.readOnly; }
			set
			{
				// if ReadOnly is changed by the setting
				if (this.readOnly ^ value)
				{
					foreach (IFieldMetadata fieldMetadata in this.fieldMetadataControlBuilders.Keys)
						this.fieldMetadataControlBuilders[fieldMetadata].ReadOnly = value;

					this.readOnly = value;
				}
			}
		}

		/// <summary>
		/// Sets or gets true when want to ignore field priviledge in data input form, defaults to false.
		/// </summary>
		public bool IgnoreFieldPriviledge { get; set; }

		/// <summary>
		/// Sets or gets the stragety to create the layout of extension fields, defaults to the instance configured in Spring.NET IoC.
		/// </summary>
		public IExtensionDataFormLayout ExtensionDataFormLayout
		{
			get
			{
				if (this.extensionDataFormLayout == null)
					return SpringContext.Current.GetObject<IExtensionDataFormLayout>();

				return this.extensionDataFormLayout;
			}
			set { this.extensionDataFormLayout = value; }
		}

		/// <summary>
		/// Sets or gets the stragety to check whether the current user having permission to the rendering fields, defaults to the instance configured in Spring.NET IoC.
		/// </summary>
		public IPermissionBridge PermissionBridge
		{
			get
			{
				if (this.permissionBridge == null)
					return SpringContext.Current.GetObject<IPermissionBridge>();

				return this.permissionBridge;
			}
			set { this.permissionBridge = value; }
		}

		/// <summary>
		/// Initialize field controls by metadata type id.
		/// That means the property "ExtensionDataTypeId" has to be set before the control loaded.
		/// If the property "ExtensionDataTypeId" equals to Guid.Empty, the <see cref="InvalidProgramException"/> will be thrown.
		/// </summary>
		/// <param name="extensionDataTypeId"></param>
		public void CreateDataInputForm(Guid extensionDataTypeId)
		{
			this.extensionDataTypeId = extensionDataTypeId;

			if (extensionDataTypeId == Guid.Empty) return;

			IObjectMetadata objectMetadata = metadataApi.GetType(extensionDataTypeId);
			if (objectMetadata == null)
				throw new InvalidProgramException(@"The argument ""extensionDataTypeId"" is invalid.");

			IEnumerable<KeyValuePair<IFieldMetadata, FieldVisibilityTypes>> fieldMetadataEnumerable = this.ResolveAuthorizedFieldMetadata(extensionDataTypeId);
			if (fieldMetadataEnumerable.Count() == 0) return;

			List<KeyValuePair<IFieldMetadata, IExtensionFieldControlBuilder>> extensionFieldControlBuildersByFieldMetadata = new List<KeyValuePair<IFieldMetadata, IExtensionFieldControlBuilder>>();
			foreach (KeyValuePair<IFieldMetadata, FieldVisibilityTypes> fieldMetadataPair in fieldMetadataEnumerable)
			{
				IFieldMetadata fieldMetadata = fieldMetadataPair.Key;
				string controlBuilderTypeName = string.Format(CultureInfo.InvariantCulture, "RapidWebDev.ExtensionModel.Web.{0}ExtensionFieldControlBuilder, RapidWebDev.ExtensionModel.Web", fieldMetadata.Type);
				Type type = Kit.GetType(controlBuilderTypeName);
				IExtensionFieldControlBuilder fieldControlBuilder = Activator.CreateInstance(type) as IExtensionFieldControlBuilder;
				this.fieldMetadataControlBuilders[fieldMetadata] = fieldControlBuilder;
				extensionFieldControlBuildersByFieldMetadata.Add(new KeyValuePair<IFieldMetadata, IExtensionFieldControlBuilder>(fieldMetadata, fieldControlBuilder));
			}

			Control uiControl = this.ExtensionDataFormLayout.Create(extensionFieldControlBuildersByFieldMetadata);
			this.Controls.Add(uiControl);

			foreach (KeyValuePair<IFieldMetadata, FieldVisibilityTypes> fieldMetadataPair in fieldMetadataEnumerable)
			{
				IFieldMetadata fieldMetadata = fieldMetadataPair.Key;
				if (this.fieldMetadataControlBuilders.ContainsKey(fieldMetadata))
				{
					IExtensionFieldControlBuilder extensionFieldControlBuilder = this.fieldMetadataControlBuilders[fieldMetadata];
					extensionFieldControlBuilder.ReadOnly = fieldMetadataPair.Value == FieldVisibilityTypes.Viewable;
				}
			}
		}

		/// <summary>
		/// Reset control values to default by field metadata.
		/// </summary>
		public void ResetControlValuesToDefault()
		{
			foreach (IFieldMetadata fieldMetadata in this.fieldMetadataControlBuilders.Keys)
			{
				IFieldValue fieldValue = fieldMetadata.GetDefaultValue();
				this.fieldMetadataControlBuilders[fieldMetadata].Value = fieldValue != null ? fieldValue.Value : null;
			}
		}

		/// <summary>
		/// Set control value from extension business object.
		/// </summary>
		/// <param name="extensionBizObject"></param>
		public void SetControlValuesFromObjectProperties(IExtensionBizObject extensionBizObject)
		{
			Dictionary<string, IFieldMetadata> fieldMetadataDictionary = this.fieldMetadataControlBuilders.Keys.ToDictionary(kvp => kvp.Name, kvp => kvp);
			IEnumerator<KeyValuePair<string, object>> iterator = extensionBizObject.GetFieldEnumerator();
			while (iterator.MoveNext())
			{
				KeyValuePair<string, object> property = iterator.Current;
				if (!fieldMetadataDictionary.ContainsKey(property.Key)) continue;
				IFieldMetadata fieldMetadata = fieldMetadataDictionary[property.Key];
				IExtensionFieldControlBuilder fieldControlBuilder = this.fieldMetadataControlBuilders[fieldMetadata];
				fieldControlBuilder.Value = property.Value;
			}
		}

		/// <summary>
		/// Set control value from extension object.
		/// </summary>
		/// <param name="extensionObject"></param>
		public void SetControlValuesFromObjectProperties(IExtensionObject extensionObject)
		{
			Dictionary<string, IFieldMetadata> fieldMetadataDictionary = this.fieldMetadataControlBuilders.Keys.ToDictionary(kvp => kvp.Name, kvp => kvp);
			IEnumerator<KeyValuePair<string, object>> iterator = extensionObject.GetFieldEnumerator();
			while (iterator.MoveNext())
			{
				KeyValuePair<string, object> property = iterator.Current;
				if (!fieldMetadataDictionary.ContainsKey(property.Key)) continue;
				IFieldMetadata fieldMetadata = fieldMetadataDictionary[property.Key];
				IExtensionFieldControlBuilder fieldControlBuilder = this.fieldMetadataControlBuilders[fieldMetadata];
				fieldControlBuilder.Value = property.Value;
			}
		}

		/// <summary>
		/// Set properties of the extension business object from controls.
		/// </summary>
		/// <param name="extensionBizObject"></param>
		public void SetObjectPropertiesFromControlValues(IExtensionBizObject extensionBizObject)
		{
			Dictionary<string, IFieldMetadata> fieldMetadataDictionary = fieldMetadataControlBuilders.Keys.ToDictionary(kvp => kvp.Name, kvp => kvp);
			foreach (string fieldName in fieldMetadataDictionary.Keys)
			{
				IFieldMetadata fieldMetadata = fieldMetadataDictionary[fieldName];
				IExtensionFieldControlBuilder fieldControlBuilder = this.fieldMetadataControlBuilders[fieldMetadata];
				extensionBizObject[fieldName] = fieldControlBuilder.Value;
			}
		}

		/// <summary>
		/// Set properties of the extension object from controls.
		/// </summary>
		/// <param name="extensionObject"></param>
		public void SetObjectPropertiesFromControlValues(IExtensionObject extensionObject)
		{
			Dictionary<string, IFieldMetadata> fieldMetadataDictionary = fieldMetadataControlBuilders.Keys.ToDictionary(kvp => kvp.Name, kvp => kvp);
			foreach (string fieldName in fieldMetadataDictionary.Keys)
			{
				IFieldMetadata fieldMetadata = fieldMetadataDictionary[fieldName];
				IExtensionFieldControlBuilder fieldControlBuilder = this.fieldMetadataControlBuilders[fieldMetadata];
				extensionObject[fieldName] = fieldControlBuilder.Value;
			}
		}

		#region ITextControl Members

		string ITextControl.Text
		{
			get { return this.ID; }
			set { }
		}

		#endregion

		private IEnumerable<KeyValuePair<IFieldMetadata, FieldVisibilityTypes>> ResolveAuthorizedFieldMetadata(Guid extensionDataTypeId)
		{
			const string ViewPermissionTemplate = "ExtensionModel.{0}.{1}.View";
			const string EditPermissionTemplate = "ExtensionModel.{0}.{1}.Edit";

			List<KeyValuePair<IFieldMetadata, FieldVisibilityTypes>> fieldMetadataResults = new List<KeyValuePair<IFieldMetadata, FieldVisibilityTypes>>();
			IEnumerable<IFieldMetadata> fieldMetadataEnumerable = metadataApi.GetFields(extensionDataTypeId);

			foreach (IFieldMetadata fieldMetadata in fieldMetadataEnumerable)
			{
				FieldVisibilityTypes visibility = FieldVisibilityTypes.None;
				if (!this.IgnoreFieldPriviledge)
				{
					if (fieldMetadata.Priviledge == FieldPriviledges.BothEditAndViewProtected)
					{
						string permissionValue = string.Format(CultureInfo.InvariantCulture, ViewPermissionTemplate, extensionDataTypeId, fieldMetadata.Name);
						if (this.PermissionBridge.HasPermission(permissionValue))
							visibility = FieldVisibilityTypes.Viewable;

						permissionValue = string.Format(CultureInfo.InvariantCulture, EditPermissionTemplate, extensionDataTypeId, fieldMetadata.Name);
						if (this.PermissionBridge.HasPermission(permissionValue))
							visibility = FieldVisibilityTypes.Writable;
					}
					else if (fieldMetadata.Priviledge == FieldPriviledges.EditProtectedOnly)
					{
						visibility = FieldVisibilityTypes.Viewable;
						string permissionValue = string.Format(CultureInfo.InvariantCulture, EditPermissionTemplate, extensionDataTypeId, fieldMetadata.Name);
						if (this.PermissionBridge.HasPermission(permissionValue))
							visibility = FieldVisibilityTypes.Writable;
					}
					else
						visibility = FieldVisibilityTypes.Writable;
				}
				else
					visibility = FieldVisibilityTypes.Writable;

				if (visibility == FieldVisibilityTypes.None) continue;
				fieldMetadataResults.Add(new KeyValuePair<IFieldMetadata, FieldVisibilityTypes>(fieldMetadata, visibility));
			}

			return fieldMetadataResults;
		}

		private enum FieldVisibilityTypes { None, Writable, Viewable }
	}
}