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
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Query field control interface.
	/// </summary>
	public interface IQueryFieldControl
	{
		/// <summary>
		/// Gets/sets the programmatic identifier assigned to the server control.
		/// </summary>
		string ID { get; set; }

		/// <summary>
		/// Auto assigned control index at runtime by infrastructure. 
		/// </summary>
		int ControlIndex { get; set; }

		/// <summary>
		/// The client (JavaScript) variable names mapping to the query field.
		/// The variable has to have the methods "getValue" to get the control value, "setValue" to set control value and "reset" to reset the control value as loaded.
		/// </summary>
		string ClientVariableName { get; }
	}
}

