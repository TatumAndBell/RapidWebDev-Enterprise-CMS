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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using WebControls = System.Web.UI.WebControls;

namespace RapidWebDev.ExtensionModel.Web
{
	/// <summary>
	/// The implementation class to build control for string typed extension field.
	/// </summary>
	public class StringExtensionFieldControlBuilder : AbstractExtensionFieldControlBuilder
	{
		private TextBox textBox;

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
		protected TextBox TextBoxDefaultValue;
		/// <summary />
		[Binding]
		protected IntegerTextBox IntegerTextBoxMinLength;
		/// <summary />
		[Binding]
		protected IntegerTextBox IntegerTextBoxMaxLength;
		/// <summary />
		[Binding]
		protected TextBox TextBoxRegex;
		/// <summary />
		[Binding]
		protected TextBox TextBoxDescription;

		/// <summary>
		/// Sets/gets true if the data input control is readonly.
		/// </summary>
		public override bool ReadOnly
		{
			get { return this.textBox.ReadOnly; }
			set { this.textBox.ReadOnly = value; }
		}

		/// <summary>
		/// Sets/gets editor control value.
		/// </summary>
		public override object Value
		{
			get { return this.textBox.Text; }
			set { this.textBox.Text = value as string; }
		}

		/// <summary>
		/// Sets/gets field metadata in metadata management UI.
		/// </summary>
		public override IFieldMetadata Metadata
		{
			get
			{
				StringFieldMetadata metadata = new StringFieldMetadata
				{
					Name = this.TextBoxName.Text,
					FieldGroup = this.TextBoxFieldGroup.Text,
					Priviledge = (FieldPriviledges)Enum.Parse(typeof(FieldPriviledges), this.DropDownListPriviledge.SelectedValue),
					IsRequired = this.CheckBoxRequired.Checked,
					Ordinal = this.IntegerTextBoxOrdinal.Value.HasValue ? this.IntegerTextBoxOrdinal.Value.Value : (int)short.MaxValue,
					Description = this.TextBoxDescription.Text,
					Default = this.TextBoxDefaultValue.Text,
					Regex = this.TextBoxRegex.Text,
					MaxLength = this.IntegerTextBoxMaxLength.Value.HasValue ? this.IntegerTextBoxMaxLength.Value.Value : 0,
					MaxLengthSpecified = this.IntegerTextBoxMaxLength.Value.HasValue,
					MinLength = this.IntegerTextBoxMinLength.Value.HasValue ? this.IntegerTextBoxMinLength.Value.Value : 0,
					MinLengthSpecified = this.IntegerTextBoxMinLength.Value.HasValue
				};

				return metadata;
			}
			set
			{
				StringFieldMetadata metadata = value as StringFieldMetadata;
				if (metadata == null) return;

				this.TextBoxName.Text = metadata.Name;
				this.TextBoxName.ReadOnly = true;

				this.TextBoxFieldGroup.Text = metadata.FieldGroup;
				this.DropDownListPriviledge.SelectedValue = metadata.Priviledge.ToString();
				this.CheckBoxRequired.Checked = metadata.IsRequired;
				this.IntegerTextBoxOrdinal.Value = metadata.Ordinal;
				this.TextBoxDescription.Text = metadata.Description;
				this.TextBoxDefaultValue.Text = metadata.Default;
				this.TextBoxRegex.Text = metadata.Regex;

				if (metadata.MaxLengthSpecified)
					this.IntegerTextBoxMaxLength.Value = metadata.MaxLength;

				if (metadata.MinLengthSpecified)
					this.IntegerTextBoxMinLength.Value = metadata.MinLength;
			}
		}

		/// <summary>
		/// Build data input control for specified field metadata.
		/// </summary>
		/// <param name="fieldMetadata"></param>
		/// <returns></returns>
		public override ExtensionDataInputControl BuildDataInputControl(IFieldMetadata fieldMetadata)
		{
			StringFieldMetadata metadata = fieldMetadata as StringFieldMetadata;
			int controlOccupiedCells = metadata.MaxLengthSpecified && metadata.MaxLength > 256 ? int.MaxValue : 1;

			WebControls.PlaceHolder placeHolder = new WebControls.PlaceHolder();

			string textBoxId = string.Format(CultureInfo.InvariantCulture, "SFM{0}_{1}", WebUtility.ConvertControlId(metadata.Name), metadata.Ordinal > 0 ? metadata.Ordinal : int.MaxValue);
			WebControls.TextBoxMode textBoxMode = controlOccupiedCells == 1 ? WebControls.TextBoxMode.SingleLine : WebControls.TextBoxMode.MultiLine;
			string cssClass = controlOccupiedCells == 1 ? "textboxShort" : "textboxarea textboxLong";
			this.textBox = new TextBox { ID = textBoxId, TextMode = textBoxMode, CssClass = cssClass };
			if (metadata.MaxLengthSpecified)
				this.textBox.MaxLength = metadata.MaxLength;

			placeHolder.Controls.Add(this.textBox);

			if (metadata.IsRequired)
			{
				LiteralControl requiredLabel = new LiteralControl("<span class=\"required\">*</span>");
				placeHolder.Controls.Add(requiredLabel);
			}

			if (!(HttpContext.Current.Handler as Page).IsPostBack && !string.IsNullOrEmpty(metadata.Default))
				this.textBox.Text = metadata.Default;

			return new ExtensionDataInputControl { Control = placeHolder, OccupiedControlCells = controlOccupiedCells };
		}

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		public override Control BuildMetadataControl()
		{
			return base.CreateFieldMetadataTemplateControl(FieldType.String);
		}
	}
}
