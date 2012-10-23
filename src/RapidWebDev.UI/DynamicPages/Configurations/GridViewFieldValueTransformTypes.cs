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
	/// GridView field value transform types.
	/// </summary>
	public enum GridViewFieldValueTransformTypes
	{
		/// <summary>
		/// No transform needed.
		/// </summary>
		None = 0,

		/// <summary>
		/// Variable output string in the rendered cell. 
		/// It supports variables defined like "http://www.rapidwebdev.org/article.aspx?id=$Id: GridViewFieldValueTransformTypes.cs 486 2009-12-11 17:00:53Z eungeliu $".
		/// The variable should be an accessible property of the object in data source, or existed variable in IApplicationContext.LabelVariables, or an existed globalization resources path.
		/// </summary>
		VariableString = 1,

		/// <summary>
		/// Use callback implementation to process output field value.
		/// The implementation must implement the interface IGridViewFieldValueTransformCallback.
		/// </summary>
		Callback = 2,

		/// <summary>
		/// Like a switch...case... syntax in C# language. 
		/// If field value matches a Case, attribute Output of the Case will be rendered.
		/// If no Case matched, the field value will be output directly as default.
		/// </summary>
		Switch = 4,

		/// <summary>
		/// The format parameter for converting the field value to string. E.g, DateTime.Now.ToString(string format).
		/// </summary>
		ToStringParameter = 8
	}
}

