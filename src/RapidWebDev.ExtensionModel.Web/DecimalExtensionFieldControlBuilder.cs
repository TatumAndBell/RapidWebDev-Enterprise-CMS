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
	/// The implementation class to build control for decimal typed extension field.
	/// </summary>
	public class DecimalExtensionFieldControlBuilder : AbstractExtensionFieldControlBuilder
	{
		private DecimalTextBox decimalTextBox;

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
		protected DecimalTextBox DecimalTextBoxDefaultValue;
		/// <summary />
		[Binding]
		protected DecimalTextBox DecimalTextBoxMinValue;
		/// <summary />
		[Binding]
		protected DecimalTextBox DecimalTextBoxMaxValue;
		/// <summary />
		[Binding]
		protected TextBox TextBoxDescription;

		/// <summary>
		/// Sets/gets true if the data input control is readonly.
		/// </summary>
		public override bool ReadOnly
		{
			get { return this.decimalTextBox.ReadOnly; }
			set { this.decimalTextBox.ReadOnly = value; }
		}

		/// <summary>
		/// Sets/gets editor control value.
		/// </summary>
		public override object Value
		{
			get { return this.decimalTextBox.Value; }
			set { this.decimalTextBox.Value = value as decimal?; }
		}

		/// <summary>
		/// Sets/gets field metadata in metadata management UI.
		/// </summary>
		public override IFieldMetadata Metadata
		{
			get
			{
				DecimalFieldMetadata metadata = new DecimalFieldMetadata
				{
					Name = this.TextBoxName.Text,
					FieldGroup = this.TextBoxFieldGroup.Text,
					Priviledge = (FieldPriviledges)Enum.Parse(typeof(FieldPriviledges), this.DropDownListPriviledge.SelectedValue),
					IsRequired = this.CheckBoxRequired.Checked,
					Ordinal = this.IntegerTextBoxOrdinal.Value.HasValue ? this.IntegerTextBoxOrdinal.Value.Value : (int)short.MaxValue,
					Description = this.TextBoxDescription.Text,
					Default = this.DecimalTextBoxDefaultValue.Value.HasValue ? this.DecimalTextBoxDefaultValue.Value.Value : 0,
					DefaultSpecified = this.DecimalTextBoxDefaultValue.Value.HasValue,
					MaxValue = this.DecimalTextBoxMaxValue.Value.HasValue ? this.DecimalTextBoxMaxValue.Value.Value : 0,
					MaxValueSpecified = this.DecimalTextBoxMaxValue.Value.HasValue,
					MinValue = this.DecimalTextBoxMinValue.Value.HasValue ? this.DecimalTextBoxMinValue.Value.Value : 0,
					MinValueSpecified = this.DecimalTextBoxMinValue.Value.HasValue
				};

				return metadata;
			}
			set
			{
				DecimalFieldMetadata metadata = value as DecimalFieldMetadata;
				if (metadata == null) return;

				this.TextBoxName.Text = metadata.Name;
				this.TextBoxName.ReadOnly = true;

				this.TextBoxFieldGroup.Text = metadata.FieldGroup;
				this.DropDownListPriviledge.SelectedValue = metadata.Priviledge.ToString();
				this.CheckBoxRequired.Checked = metadata.IsRequired;
				this.IntegerTextBoxOrdinal.Value = metadata.Ordinal;
				this.TextBoxDescription.Text = metadata.Description;

				if (metadata.DefaultSpecified)
					this.DecimalTextBoxDefaultValue.Value = metadata.Default;

				if (metadata.MaxValueSpecified)
					this.DecimalTextBoxMaxValue.Value = metadata.MaxValue;

				if (metadata.MinValueSpecified)
					this.DecimalTextBoxMinValue.Value = metadata.MinValue;
			}
		}

		/// <summary>
		/// Build data input control for specified field metadata.
		/// </summary>
		/// <param name="fieldMetadata"></param>
		/// <returns></returns>
		public override ExtensionDataInputControl BuildDataInputControl(IFieldMetadata fieldMetadata)
		{
			DecimalFieldMetadata metadata = fieldMetadata as DecimalFieldMetadata;

			WebControls.PlaceHolder placeHolder = new WebControls.PlaceHolder();

			string textBoxId = string.Format(CultureInfo.InvariantCulture, "DFM{0}_{1}", WebUtility.ConvertControlId(metadata.Name), metadata.Ordinal > 0 ? metadata.Ordinal : int.MaxValue);
			this.decimalTextBox = new DecimalTextBox { ID = textBoxId, TextMode = WebControls.TextBoxMode.SingleLine, CssClass = "textboxShort", MaxLength = 32, AllowNegative = true };
			placeHolder.Controls.Add(this.decimalTextBox);

			if (metadata.IsRequired)
			{
				LiteralControl requiredLabel = new LiteralControl("<span class=\"required\">*</span>");
				placeHolder.Controls.Add(requiredLabel);
			}

			if (!(HttpContext.Current.Handler as Page).IsPostBack && metadata.DefaultSpecified)
				this.decimalTextBox.Value = metadata.Default;

			return new ExtensionDataInputControl { Control = placeHolder, OccupiedControlCells = 1 };
		}

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		public override Control BuildMetadataControl()
		{
			return base.CreateFieldMetadataTemplateControl(FieldType.Decimal);
		}
	}
}
