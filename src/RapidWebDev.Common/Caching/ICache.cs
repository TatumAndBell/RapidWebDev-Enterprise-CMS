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

namespace RapidWebDev.Common.Caching
{
	/// <summary>
	/// Interface of generic cache.
	/// </summary>
	public interface ICache
	{
		/// <summary>
		/// Gets caching object count.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Add object to cache container by specified expire sliding duration and callback.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="slidingDuration"></param>
		/// <param name="cachePriorityType"></param>
		void Add(object key, object value, TimeSpan slidingDuration, CachePriorityTypes cachePriorityType);

		/// <summary>
		/// Get caching object by key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object Get(object key);

		/// <summary>
		/// Remove caching object by key.
		/// </summary>
		/// <param name="key"></param>
		void Remove(object key);
	}
}

