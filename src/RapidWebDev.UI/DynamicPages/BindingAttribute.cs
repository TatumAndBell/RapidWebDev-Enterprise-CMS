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

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Binding attribute indicates the fields of page workshop are binding to controls in skin template.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class BindingAttribute : System.Attribute
	{
		/// <summary>
		/// Get parent control path, the parent controls are separated by character "\".
		/// </summary>
		public string ParentControlPath { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public BindingAttribute() { }

		/// <summary>
		/// Constructor with parent control id.
		/// </summary>
		/// <param name="parentControlPath">Parent control path, the parent controls are separated by character "\".</param>
		public BindingAttribute(string parentControlPath)
		{
			this.ParentControlPath = parentControlPath;
		}
	}
}
