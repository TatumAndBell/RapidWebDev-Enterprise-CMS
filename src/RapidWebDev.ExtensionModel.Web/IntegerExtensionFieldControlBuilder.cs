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
	/// The implementation class to build control for integer typed extension field.
	/// </summary>
	public class IntegerExtensionFieldControlBuilder : AbstractExtensionFieldControlBuilder
	{
		private IntegerTextBox integerTextBox;

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
		protected IntegerTextBox IntegerTextBoxDefaultValue;
		/// <summary />
		[Binding]
		protected IntegerTextBox IntegerTextBoxMinValue;
		/// <summary />
		[Binding]
		protected IntegerTextBox IntegerTextBoxMaxValue;
		/// <summary />
		[Binding]
		protected TextBox TextBoxDescription;

		/// <summary>
		/// Sets/gets true if the data input control is readonly.
		/// </summary>
		public override bool ReadOnly
		{
			get { return this.integerTextBox.ReadOnly; }
			set { this.integerTextBox.ReadOnly = value; }
		}

		/// <summary>
		/// Sets/gets editor control value.
		/// </summary>
		public override object Value
		{
			get { return this.integerTextBox.Value; }
			set { this.integerTextBox.Value = value as int?; }
		}

		/// <summary>
		/// Sets/gets field metadata in metadata management UI.
		/// </summary>
		public override IFieldMetadata Metadata
		{
			get
			{
				IntegerFieldMetadata metadata = new IntegerFieldMetadata
				{
					Name = this.TextBoxName.Text,
					FieldGroup = this.TextBoxFieldGroup.Text,
					Priviledge = (FieldPriviledges)Enum.Parse(typeof(FieldPriviledges), this.DropDownListPriviledge.SelectedValue),
					IsRequired = this.CheckBoxRequired.Checked,
					Ordinal = this.IntegerTextBoxOrdinal.Value.HasValue ? this.IntegerTextBoxOrdinal.Value.Value : (int)short.MaxValue,
					Description = this.TextBoxDescription.Text,
					Default = this.IntegerTextBoxDefaultValue.Value.HasValue ? this.IntegerTextBoxDefaultValue.Value.Value : 0,
					DefaultSpecified = this.IntegerTextBoxDefaultValue.Value.HasValue,
					MaxValue = this.IntegerTextBoxMaxValue.Value.HasValue ? this.IntegerTextBoxMaxValue.Value.Value : 0,
					MaxValueSpecified = this.IntegerTextBoxMaxValue.Value.HasValue,
					MinValue = this.IntegerTextBoxMinValue.Value.HasValue ? this.IntegerTextBoxMinValue.Value.Value : 0,
					MinValueSpecified = this.IntegerTextBoxMinValue.Value.HasValue
				};

				return metadata;
			}
			set
			{
				IntegerFieldMetadata metadata = value as IntegerFieldMetadata;
				if (metadata == null) return;

				this.TextBoxName.Text = metadata.Name;
				this.TextBoxName.ReadOnly = true;

				this.TextBoxFieldGroup.Text = metadata.FieldGroup;
				this.DropDownListPriviledge.SelectedValue = metadata.Priviledge.ToString();
				this.CheckBoxRequired.Checked = metadata.IsRequired;
				this.IntegerTextBoxOrdinal.Value = metadata.Ordinal;
				this.TextBoxDescription.Text = metadata.Description;

				if (metadata.DefaultSpecified)
					this.IntegerTextBoxDefaultValue.Value = metadata.Default;

				if (metadata.MaxValueSpecified)
					this.IntegerTextBoxMaxValue.Value = metadata.MaxValue;

				if (metadata.MinValueSpecified)
					this.IntegerTextBoxMinValue.Value = metadata.MinValue;
			}
		}

		/// <summary>
		/// Build data input control for specified field metadata.
		/// </summary>
		/// <param name="fieldMetadata"></param>
		/// <returns></returns>
		public override ExtensionDataInputControl BuildDataInputControl(IFieldMetadata fieldMetadata)
		{
			IntegerFieldMetadata metadata = fieldMetadata as IntegerFieldMetadata;

			WebControls.PlaceHolder placeHolder = new WebControls.PlaceHolder();

			string textBoxId = string.Format(CultureInfo.InvariantCulture, "IFM{0}_{1}", WebUtility.ConvertControlId(metadata.Name), metadata.Ordinal > 0 ? metadata.Ordinal : int.MaxValue);
			this.integerTextBox = new IntegerTextBox { ID = textBoxId, TextMode = WebControls.TextBoxMode.SingleLine, CssClass = "textboxShort", MaxLength = 32, AllowNegative = true };
			placeHolder.Controls.Add(this.integerTextBox);

			if (metadata.IsRequired)
			{
				LiteralControl requiredLabel = new LiteralControl("<span class=\"required\">*</span>");
				placeHolder.Controls.Add(requiredLabel);
			}

			if (!(HttpContext.Current.Handler as Page).IsPostBack && metadata.DefaultSpecified)
				this.integerTextBox.Value = metadata.Default;

			return new ExtensionDataInputControl { Control = placeHolder, OccupiedControlCells = 1 };
		}

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		public override Control BuildMetadataControl()
		{
			return base.CreateFieldMetadataTemplateControl(FieldType.Integer);
		}
	}
}
