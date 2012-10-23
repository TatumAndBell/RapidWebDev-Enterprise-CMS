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

namespace RapidWebDev.Common.Data
{
	/// <summary>
	/// Provides additional options for creating a transaction scope.
	/// </summary>
	public enum TransactionScopeOption
	{
		/// <summary>
		/// A transaction is required by the scope. 
		/// It uses an ambient transaction if one already exists. 
		/// Otherwise, it creates a new transaction before entering the scope. 
		/// This is the default value.
		/// </summary>
		Required = 0,
		
		/// <summary>
		/// A new transaction is always created for the scope.
		/// </summary>
		RequiresNew = 1,
		
		/// <summary>
		/// The ambient transaction context is suppressed when creating the scope. 
		/// All operations within the scope are done without an ambient transaction context.
		/// </summary>
		Suppress = 2,
	}
}