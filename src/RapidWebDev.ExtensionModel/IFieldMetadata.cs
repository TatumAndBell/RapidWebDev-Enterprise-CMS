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

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
	/// Extension property metadata
	/// </summary>
	public interface IFieldMetadata : ICloneable
	{
		/// <summary>
		/// Gets or sets id.
		/// </summary>
		Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
		string Name { get; set; }

		/// <summary>
		/// Gets or sets field group.
		/// </summary>
		string FieldGroup { get; set; }

		/// <summary>
		/// Gets or sets field priviledge.
		/// </summary>
		FieldPriviledges Priviledge { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        FieldType Type { get; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this extension property is required.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
		bool IsRequired
		{
			get;
			set;
		}

        /// <summary>
        /// Order of this extension property in class
        /// </summary>
        int Ordinal
        {
            get;
            set;
        }

		/// <summary>
		/// True indicates the field is inherited from parent object metadata.
		/// </summary>
		bool Inherited { get; set; }

		/// <summary>
		/// Validate this field.
		/// </summary>
		/// <param name="value">Value of field</param>
		/// <exception cref="InvalidFieldValueException">This extension field's value is invalid</exception>
		void Validate(IFieldValue value);

		/// <summary>
		/// Get default field value
		/// </summary>
		/// <returns>if no default value, return null</returns>
		IFieldValue GetDefaultValue();
	}
}
