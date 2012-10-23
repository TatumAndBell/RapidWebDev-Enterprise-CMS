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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The interface to generate sequence number.
	/// An business example is to generate order number in product-order system.
	/// The class generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
	/// </summary>
	public interface ISequenceNoApi
	{
		/// <summary>
		/// Create sequence number on specified sequence number type.
		/// The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
		/// </summary>
		/// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
		/// <returns></returns>
		long Create(string sequenceNoType);

		/// <summary>
		/// Create sequence number on specified type for special object id.
		/// The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
		/// </summary>
		/// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
		/// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
		/// <returns></returns>
		long Create(Guid objectId, string sequenceNoType);

		/// <summary>
		/// Create sequence numbers on specified sequence number type.
		///  The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
		/// </summary>
		/// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
		/// <param name="sequenceNoCount">Indicates how many sequence number is required.</param>
		/// <returns></returns>
		IEnumerable<long> Create(string sequenceNoType, ushort sequenceNoCount);

		/// <summary>
		/// Create sequence numbers on specified type for special object id.
		///  The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
		/// </summary>
		/// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
		/// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
		/// <param name="sequenceNoCount">Indicates how many sequence number is required.</param>
		/// <returns></returns>
		IEnumerable<long> Create(Guid objectId, string sequenceNoType, ushort sequenceNoCount);
	}
}

