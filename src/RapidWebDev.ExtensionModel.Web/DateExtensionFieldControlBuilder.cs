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
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using WebControls = System.Web.UI.WebControls;

namespace RapidWebDev.ExtensionModel.Web
{
	/// <summary>
	/// The implementation class to build control for Date typed extension field.
	/// </summary>
	public class DateExtensionFieldControlBuilder : AbstractExtensionFieldControlBuilder
	{
		private DatePicker datePicker;

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
		protected WebControls.DropDownList DropDownListDefaultValue;
		/// <summary />
		[Binding]
		protected TextBox TextBoxDescription;

		/// <summary>
		/// Sets/gets true if the data input control is readonly.
		/// </summary>
		public override bool ReadOnly
		{
			get { return this.datePicker.ReadOnly; }
			set { this.datePicker.ReadOnly = value; }
		}

		/// <summary>
		/// Sets/gets editor control value.
		/// </summary>
		public override object Value 
		{
			get { return this.datePicker.SelectedValue; }
			set { this.datePicker.SelectedValue = value as DateTime?; }
		}

		/// <summary>
		/// Sets/gets field metadata in metadata management UI.
		/// </summary>
		public override IFieldMetadata Metadata 
		{
			get
			{
				DateFieldMetadata metadata = new DateFieldMetadata
				{
					Name = this.TextBoxName.Text,
					FieldGroup = this.TextBoxFieldGroup.Text,
					Priviledge = (FieldPriviledges)Enum.Parse(typeof(FieldPriviledges), this.DropDownListPriviledge.SelectedValue),
					IsRequired = this.CheckBoxRequired.Checked,
					Ordinal = this.IntegerTextBoxOrdinal.Value.HasValue ? this.IntegerTextBoxOrdinal.Value.Value : (int)short.MaxValue,
					Description = this.TextBoxDescription.Text
				};

				if(!string.IsNullOrEmpty(this.DropDownListDefaultValue.SelectedValue))
				{
					metadata.DefaultValue = new DateTimeValue()
					{
						 DateTimeValueType = (DateTimeValueTypes)Enum.Parse(typeof(DateTimeValueTypes), this.DropDownListDefaultValue.SelectedValue)
					};
				}

				return metadata;
			}
			set
			{
				DateFieldMetadata metadata = value as DateFieldMetadata;
				if (metadata == null) return;

				this.TextBoxName.Text = metadata.Name;
				this.TextBoxName.ReadOnly = true;

				this.TextBoxFieldGroup.Text = metadata.FieldGroup;
				this.DropDownListPriviledge.SelectedValue = metadata.Priviledge.ToString();
				this.CheckBoxRequired.Checked = metadata.IsRequired;
				this.IntegerTextBoxOrdinal.Value = metadata.Ordinal;
				this.TextBoxDescription.Text = metadata.Description;

				if (metadata.DefaultValue != null)
					this.DropDownListDefaultValue.SelectedValue = metadata.DefaultValue.DateTimeValueType.ToString();
			}
		}

		/// <summary>
		/// Build data input control for specified field metadata.
		/// </summary>
		/// <param name="fieldMetadata"></param>
		/// <returns></returns>
		public override ExtensionDataInputControl BuildDataInputControl(IFieldMetadata fieldMetadata)
		{
			DateFieldMetadata metadata = fieldMetadata as DateFieldMetadata;

			WebControls.PlaceHolder placeHolder = new WebControls.PlaceHolder();
			string textBoxId = string.Format(CultureInfo.InvariantCulture, "DFM{0}_{1}", WebUtility.ConvertControlId(metadata.Name), metadata.Ordinal > 0 ? metadata.Ordinal : int.MaxValue);
			this.datePicker = new DatePicker { ID = textBoxId, TextMode = WebControls.TextBoxMode.SingleLine, CssClass = "textboxShort", MaxLength = 32 };
			placeHolder.Controls.Add(this.datePicker);

			if (metadata.IsRequired)
			{
				LiteralControl requiredLabel = new LiteralControl("<span class=\"required\">*</span>");
				placeHolder.Controls.Add(requiredLabel);
			}

			if (!(HttpContext.Current.Handler as Page).IsPostBack && metadata.DefaultValue != null)
				this.datePicker.SelectedValue = metadata.GetDefaultValue().Value as DateTime?;

			return new ExtensionDataInputControl { Control = placeHolder, OccupiedControlCells = 1 };
		}

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		public override Control BuildMetadataControl()
		{
			// reuse the ASCX template of DateTime
			return base.CreateFieldMetadataTemplateControl(FieldType.DateTime);
		}
	}
}
