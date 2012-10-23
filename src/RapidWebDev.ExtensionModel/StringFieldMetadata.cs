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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel.Properties;

namespace RapidWebDev.ExtensionModel
{
    /// <summary>
    /// String type's Metadata
    /// </summary>
    public partial class StringFieldMetadata : IFieldMetadata
    {
        private Regex regexInstance;

		/// <summary>
		/// Gets or sets id.
		/// </summary>
		public Guid Id { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlIgnore]
		public FieldType Type { get { return FieldType.String; } }

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
            if (value == null || Kit.IsEmpty(value.Value))
            {
                if (this.IsRequired)
                    throw new InvalidFieldValueException(string.Format(Resources.FieldValueCannotBeNull, this.Name));
            }
            else
            {
                string s = value.Value as string;
                if (this.MaxLengthSpecified && s.Length > this.MaxLength)
					throw new InvalidFieldValueException(string.Format(Resources.FieldValueGreaterThanMaximalLength, this.Name, this.MaxLength));

                if (this.MinLengthSpecified && s.Length < this.MinLength)
					throw new InvalidFieldValueException(string.Format(Resources.FieldValueLessThanMinimumLength, this.Name, this.MinLength));

                Regex regex = this.GetRegexInstance(this.Regex);
                if (regex != null)
                {
                    if (!regex.IsMatch(s))
                        throw new InvalidFieldValueException(string.Format(Resources.FieldValueNotAnswerForPreconcertedFormat, this.Name, this.Regex));
                }
            }
        }

        /// <summary>
        /// Get default field value
        /// </summary>
        /// <returns>if no default value, return null</returns>
        public IFieldValue GetDefaultValue()
        {
            if (this.Default == null) return null;
            return this.Default.FieldValue();
        }

        private Regex GetRegexInstance(string regexExpression)
        {
            if (Kit.IsEmpty(this.Regex)) return null;

            if (this.regexInstance == null)
            {
                this.regexInstance = new Regex(regexExpression, RegexOptions.Compiled);
            }

            return this.regexInstance;
        }

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return new StringFieldMetadata
			{
				Id = this.Id,
				Priviledge = this.Priviledge,
				FieldGroup = this.FieldGroup,
				Description = this.Description,
				Inherited = this.Inherited,
				IsRequired = this.IsRequired,
				Name = this.Name,
				Default = this.Default,
				MaxLength = this.MaxLength,
				MaxLengthSpecified = this.MaxLengthSpecified,
				MinLength = this.MinLength,
				MinLengthSpecified = this.MinLengthSpecified,
				Regex = this.Regex,
				Ordinal = this.Ordinal
			};
		}

		#endregion
    }
}
