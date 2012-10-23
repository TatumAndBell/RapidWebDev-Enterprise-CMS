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
    /// Extend to data type related with value of IFieldValue
	/// </summary>
	public static class PrimitiveTypeExtensions
	{
		/// <summary>
		/// Convert to StringFieldValue
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static StringFieldValue FieldValue(this string s)
		{
			return new StringFieldValue(s);
		}

		/// <summary>
        /// Convert to DateTimeFieldValue
		/// </summary>
		/// <param name="dateTime">if dateTime is null，return null</param>
		/// <returns></returns>
		public static DateTimeFieldValue FieldValue(this DateTime? dateTime)
		{
			if (dateTime.HasValue)
				return new DateTimeFieldValue(dateTime.Value);

			return null;
		}

		/// <summary>
		/// Convert to DateTimeFieldValue
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTimeFieldValue FieldValue(this DateTime dateTime)
		{
			return new DateTimeFieldValue(dateTime);
		}

		/// <summary>
		/// Convert Decimal to DecimalFieldValue
		/// </summary>
		/// <param name="value">if value is null，return null</param>
		/// <returns></returns>
		public static DecimalFieldValue FieldValue(this decimal? value)
		{
			if (value.HasValue)
				return new DecimalFieldValue(value.Value);

			return null;
		}

		/// <summary>
		/// Convert decimal value to DecimalFieldValue
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static DecimalFieldValue FieldValue(this decimal value)
		{
			return new DecimalFieldValue(value);
		}

		/// <summary>
		/// Convert Integer to IntegerFieldValue
		/// </summary>
		/// <param name="value">if value is null，return null</param>
		/// <returns></returns>
		public static IntegerFieldValue FieldValue(this int? value)
		{
			if (value.HasValue)
				return new IntegerFieldValue(value.Value);

			return null;
		}

		/// <summary>
		/// Convert Integer to IntegerFieldValue
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IntegerFieldValue FieldValue(this int value)
		{
			return new IntegerFieldValue(value);
		}

		/// <summary>
		/// Convert HierarchyNodeCollection to HierarchyFieldValue
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static HierarchyFieldValue FieldValue(this HierarchyNodeValueCollection value)
		{
			if (value == null) return null;
			return new HierarchyFieldValue(value);
		}

		/// <summary>
		/// Convert SelectionCollection to SelectionFieldValue
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static EnumerationFieldValue FieldValue(this EnumerationValueCollection value)
		{
			if (value == null) return null;
			return new EnumerationFieldValue(value);
		}
	}
}

