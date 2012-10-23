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

namespace RapidWebDev.ExtensionModel.Web
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Web.UI;
	using RapidWebDev.ExtensionModel.Web.Controls;
	using RapidWebDev.UI;
	using RapidWebDev.UI.Controls;
	using RapidWebDev.UI.DynamicPages;
	using WebControls = System.Web.UI.WebControls;

	/// <summary>
	/// The implementation class to build control for Selection typed extension field.
	/// </summary>
	public class EnumerationExtensionFieldControlBuilder : AbstractExtensionFieldControlBuilder
	{
		private ComboBox ComboBoxFieldValue;
		private CheckBoxGroup CheckBoxGroupFieldValue;

		/// <summary />
		[Binding]
		protected TextBox TextBoxName;
		/// <summary />
		[Binding]
		protected TextBox TextBoxFieldGroup;
		/// <summary />
		[Binding]
		protected WebControls.DropDownList DropDownListPriviledge;
		/// <summary />
		[Binding]
		protected CheckBox CheckBoxRequired;
		/// <summary />
		[Binding]
		protected IntegerTextBox IntegerTextBoxOrdinal;
		/// <summary />
		[Binding]
		protected SelectionExtensionFieldControl SelectionExtensionFieldControl;
		/// <summary />
		[Binding]
		protected TextBox TextBoxDescription;
		/// <summary />
		[Binding]
		protected ComboBox ComboBoxSelectionMode;

		/// <summary>
		/// Sets/gets true if the data input control is readonly.
		/// </summary>
		public override bool ReadOnly
		{
			get 
			{
				if (this.ComboBoxFieldValue != null)
					return !this.ComboBoxFieldValue.Enabled;
				else if (this.CheckBoxGroupFieldValue != null)
					return !this.CheckBoxGroupFieldValue.Enabled;
				else
					return false;
			}
			set
			{
				if (this.ComboBoxFieldValue != null)
					this.ComboBoxFieldValue.Enabled = !value;
				else if (this.CheckBoxGroupFieldValue != null)
					this.CheckBoxGroupFieldValue.Enabled = !value;
			}
		}

		/// <summary>
		/// Sets/gets editor control value.
		/// </summary>
		public override object Value
		{
			get
			{
				if (this.ComboBoxFieldValue != null)
					return new EnumerationValueCollection { this.ComboBoxFieldValue.SelectedValue };
				else if (this.CheckBoxGroupFieldValue != null)
				{
					EnumerationValueCollection returnValue = new EnumerationValueCollection();
					foreach (WebControls.ListItem listItem in this.CheckBoxGroupFieldValue.Items)
						if (listItem.Selected)
							returnValue.Add(listItem.Value);

					return returnValue;
				}

				return new EnumerationValueCollection();
			}
			set
			{
				EnumerationValueCollection selectionCollection = value as EnumerationValueCollection;
				if (selectionCollection == null) selectionCollection = new EnumerationValueCollection();

				// single selection
				if (this.ComboBoxFieldValue != null)
					this.ComboBoxFieldValue.SelectedValue = selectionCollection != null && selectionCollection.Count > 0 ? selectionCollection[0] : null;

				// multiple selection
				else if (this.CheckBoxGroupFieldValue != null)
				{
					foreach (WebControls.ListItem listItem in this.CheckBoxGroupFieldValue.Items)
						listItem.Selected = selectionCollection.Contains(listItem.Value);
				}
			}
		}

		/// <summary>
		/// Sets/gets field metadata in metadata management UI.
		/// </summary>
		public override IFieldMetadata Metadata
		{
			get
			{
				SelectionModes selectionMode = SelectionModes.Single;
				if (!string.IsNullOrEmpty(this.ComboBoxSelectionMode.SelectedValue))
					selectionMode = (SelectionModes)Enum.Parse(typeof(SelectionModes), this.ComboBoxSelectionMode.SelectedValue);

				EnumerationFieldMetadata metadata = new EnumerationFieldMetadata
				{
					Name = this.TextBoxName.Text,
					FieldGroup = this.TextBoxFieldGroup.Text,
					Priviledge = (FieldPriviledges)Enum.Parse(typeof(FieldPriviledges), this.DropDownListPriviledge.SelectedValue),
					IsRequired = this.CheckBoxRequired.Checked,
					Ordinal = this.IntegerTextBoxOrdinal.Value.HasValue ? this.IntegerTextBoxOrdinal.Value.Value : (int)short.MaxValue,
					SelectionMode = selectionMode,
					Description = this.TextBoxDescription.Text
				};

				if (this.SelectionExtensionFieldControl.SelectionItems != null)
					metadata.Items = this.SelectionExtensionFieldControl.SelectionItems.Select(x => new ExtensionModel.SelectionItem { Name = x.Text, Value = x.Value, Selected = x.Selected }).ToArray();

				return metadata;
			}
			set
			{
				EnumerationFieldMetadata metadata = value as EnumerationFieldMetadata;
				if (metadata == null) return;

				this.TextBoxName.Text = metadata.Name;
				this.TextBoxName.ReadOnly = true;

				this.TextBoxFieldGroup.Text = metadata.FieldGroup;
				this.DropDownListPriviledge.SelectedValue = metadata.Priviledge.ToString();
				this.CheckBoxRequired.Checked = metadata.IsRequired;
				this.IntegerTextBoxOrdinal.Value = metadata.Ordinal;
				this.TextBoxDescription.Text = metadata.Description;
				this.ComboBoxSelectionMode.SelectedValue = metadata.SelectionMode.ToString();

				if (metadata.Items != null)
					this.SelectionExtensionFieldControl.SelectionItems = metadata.Items.Select(x => new SelectionItem { Text = x.Name, Value = x.Value, Selected = x.Selected });
			}
		}

		/// <summary>
		/// Build data input control for specified field metadata.
		/// </summary>
		/// <param name="fieldMetadata"></param>
		/// <returns></returns>
		public override ExtensionDataInputControl BuildDataInputControl(IFieldMetadata fieldMetadata)
		{
			EnumerationFieldMetadata metadata = fieldMetadata as EnumerationFieldMetadata;
			WebControls.PlaceHolder placeHolder = new WebControls.PlaceHolder();

			if (metadata.SelectionMode == SelectionModes.Single)
			{
				string comboBoxId = string.Format(CultureInfo.InvariantCulture, "SLTFM{0}_{1}", WebUtility.ConvertControlId(metadata.Name), metadata.Ordinal > 0 ? metadata.Ordinal : int.MaxValue);
				this.ComboBoxFieldValue = new ComboBox { ID = comboBoxId, Editable = false, ForceSelection = true, Mode = ComboBoxDataSourceModes.Local, Width = 154 };
				foreach (ExtensionModel.SelectionItem selectionItem in metadata.Items.OrderBy(item => item.Name))
					this.ComboBoxFieldValue.Items.Add(new WebControls.ListItem(selectionItem.Name, selectionItem.Value) { Selected = selectionItem.Selected });

				placeHolder.Controls.Add(this.ComboBoxFieldValue);

				if (metadata.IsRequired)
				{
					LiteralControl requiredLabel = new LiteralControl("<span class=\"required\">*</span>");
					placeHolder.Controls.Add(requiredLabel);
				}

				return new ExtensionDataInputControl { Control = placeHolder, OccupiedControlCells = 1 };
			}
			else
			{
				string checkBoxGroupId = string.Format(CultureInfo.InvariantCulture, "SLTFM{0}_{1}", WebUtility.ConvertControlId(metadata.Name), metadata.Ordinal > 0 ? metadata.Ordinal : int.MaxValue);
				this.CheckBoxGroupFieldValue = new CheckBoxGroup { ID = checkBoxGroupId, RepeatDirection = System.Web.UI.WebControls.RepeatDirection.Horizontal, RepeatLayout = System.Web.UI.WebControls.RepeatLayout.Flow };
				foreach (ExtensionModel.SelectionItem selectionItem in metadata.Items.OrderBy(item => item.Name))
					this.CheckBoxGroupFieldValue.Items.Add(new WebControls.ListItem(selectionItem.Name, selectionItem.Value) { Selected = selectionItem.Selected });

				placeHolder.Controls.Add(this.CheckBoxGroupFieldValue);

				if (metadata.IsRequired)
				{
					LiteralControl requiredLabel = new LiteralControl("<span class=\"required\">*</span>");
					placeHolder.Controls.Add(requiredLabel);
				}

				return new ExtensionDataInputControl { Control = placeHolder, OccupiedControlCells = int.MaxValue };
			}
		}

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		public override Control BuildMetadataControl()
		{
			return base.CreateFieldMetadataTemplateControl(FieldType.Enumeration);
		}
	}
}
