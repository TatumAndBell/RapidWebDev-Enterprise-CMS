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
using System.Xml.Serialization;
using RapidWebDev.ExtensionModel.Properties;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
	/// Data field metadata
	/// </summary>
	public partial class DateFieldMetadata : IFieldMetadata
	{
		/// <summary>
		/// Gets or sets id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		[XmlIgnore]
		public FieldType Type { get { return FieldType.Date; } }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[XmlIgnore]
		public string Description { get; set; }

		/// <summary>
		/// True indicates the field is inherited from parent object metadata.
		/// </summary>
		[XmlIgnore]
		public bool Inherited { get; set; }

		/// <summary>
		/// Validate this field.
		/// </summary>
		/// <param name="value">Value of field</param>
		/// <exception cref="InvalidFieldValueException">This extension field's value is invalid</exception>
		public void Validate(IFieldValue value)
		{
			if (value == null)
			{
				if (this.IsRequired)
					throw new InvalidFieldValueException(string.Format(Resources.FieldValueCannotBeNull, this.Name));
			}
			else
			{
				DateTime argumentDateTime = (DateTime)value.Value;
				if (this.MaxValue != null)
				{
					DateTime maxDateTime = GetDateTimeValue(this.MaxValue);
					if (argumentDateTime > maxDateTime)
						throw new InvalidFieldValueException(string.Format(Resources.FieldValueGreaterThanMaximalValue, this.Name, maxDateTime));
				}

				if (this.MinValue != null)
				{
					DateTime minDateTime = GetDateTimeValue(this.MinValue);
					if (argumentDateTime < minDateTime)
						throw new InvalidFieldValueException(string.Format(Resources.FieldValueLessThanMinimalValue, this.Name, minDateTime));
				}
			}
		}

		/// <summary>
		/// Get default field value
		/// </summary>
		/// <returns>if no default value, return null</returns>
		public IFieldValue GetDefaultValue()
		{
			if (this.DefaultValue == null) return null;
			return GetDateTimeValue(this.DefaultValue).FieldValue();
		}

		private static DateTime GetDateTimeValue(DateTimeValue dateTimeValue)
		{
			switch (dateTimeValue.DateTimeValueType)
			{
				case DateTimeValueTypes.Now:
					return LocalizationUtility.ConvertUtcTimeToClientTime(DateTime.UtcNow);
				case DateTimeValueTypes.FirstDayOfThisYear:
					return LocalizationUtility.ConvertUtcTimeToClientTime(new DateTime(DateTime.UtcNow.Year, 1, 1));
				case DateTimeValueTypes.FirstDayOfThisMonth:
					return LocalizationUtility.ConvertUtcTimeToClientTime(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1));
				case DateTimeValueTypes.FirstDayOfThisWeek:
					int dayOfWeek;
					if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday) dayOfWeek = 7;
					else dayOfWeek = (int)DateTime.UtcNow.DayOfWeek;

					return LocalizationUtility.ConvertUtcTimeToClientTime(DateTime.UtcNow.AddDays(DateTime.UtcNow.Day - dayOfWeek + 1));
				default:
					if (dateTimeValue.ValueSpecified) return dateTimeValue.Value;
					throw new ArgumentException(Resources.NotSpecifiedDate, "dateTimeValue");
			}
		}

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return new DateFieldMetadata
			{
				Id = this.Id,
				FieldGroup = this.FieldGroup,
				Priviledge = this.Priviledge,
				Description = this.Description,
				Inherited = this.Inherited,
				IsRequired = this.IsRequired,
				Name = this.Name,
				DefaultValue = this.DefaultValue,
				MaxValue = this.MaxValue,
				MinValue = this.MinValue,
				Ordinal = this.Ordinal
			};
		}

		#endregion
	}
}
