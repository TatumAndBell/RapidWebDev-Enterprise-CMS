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
	/// Specifies the isolation level of a transaction.
	/// </summary>
	public enum IsolationLevel
	{
		/// <summary>
		/// Volatile data can be read but not modified, and no new data can be added during the transaction.
		/// </summary>
		Serializable = 0,
		
		/// <summary>
		/// Volatile data can be read but not modified during the transaction. New data can be added during the transaction.
		/// </summary>
		RepeatableRead = 1,
		
		/// <summary>
		/// Volatile data cannot be read during the transaction, but can be modified.
		/// </summary>
		ReadCommitted = 2,

		/// <summary>
		/// Volatile data can be read and modified during the transaction.
		/// </summary>
		ReadUncommitted = 3,

		/// <summary>
		/// Volatile data can be read. 
		/// Before a transaction modifies data, it verifies if another transaction has changed the data after it was initially read.
		/// If the data has been updated, an error is raised. This allows a transaction to get to the previously committed value of the data.
		/// </summary>
		Snapshot = 4,

		/// <summary>
		/// The pending changes from more highly isolated transactions cannot be overwritten.
		/// </summary>
		Chaos = 5,

		/// <summary>
		/// A different isolation level than the one specified is being used, but the level cannot be determined. 
		/// An exception is thrown if this value is set.
		/// </summary>
		Unspecified = 6,
	}
}