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

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Criteria operator between query field name and value.
	/// </summary>
	public enum QueryFieldOperators
	{
		/// <summary>
		/// There will generate operator for special typed fields. E.g,
		/// "Equal" for CheckBox, ComboBox, Radio, RadioGroup and Custom.
		/// "Like" for TextBox.
		/// "Between" for Date, DateTime, Numeric, Time.
		/// "In" for CheckBoxGroup.
		/// </summary>
		Auto = 0,

		/// <summary>
		/// Equal
		/// </summary>
		Equal = 1,

		/// <summary>
		/// StartsWith only works on string typed field. 
		/// </summary>
		StartsWith = 2,

		/// <summary>
		/// Like only works on string typed field.
		/// </summary>
		Like = 4,

		/// <summary>
		/// In only works for CheckBoxGroup query field control.
		/// </summary>
		In = 8,

		/// <summary>
		/// NotIn only works for CheckBoxGroup query field control. 
		/// </summary>
		NotIn = 16,

		/// <summary>
		/// Greater than
		/// </summary>
		GreaterThan = 32,

		/// <summary>
		/// Greater or equal than
		/// </summary>
		GreaterEqualThan = 64,

		/// <summary>
		/// Less than
		/// </summary>
		LessThan = 128,

		/// <summary>
		/// Less or equal than
		/// </summary>
		LessEqualThan = 256,

		/// <summary>
		/// Between
		/// </summary>
		Between = 512
	}
}
