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
using System.Web.Caching;

namespace RapidWebDev.Common.Caching
{
	/// <summary>
	/// Event argument when cached object expired.
	/// </summary>
	public class CacheExpiredEventArgs : EventArgs
	{
		/// <summary>
		/// Gets caching key.
		/// </summary>
		public object Key { get; private set; }

		/// <summary>
		/// Gets caching value.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		/// Gets the reason of caching object been removed.
		/// </summary>
		public CacheRemovedReason Reason { get; private set; }

		/// <summary>
		/// Construct cache expire event argument.
		/// </summary>
		/// <param name="cacheKey">caching key</param>
		/// <param name="cacheValue">caching value</param>
		/// <param name="reason">cached object removed reason</param>
		internal CacheExpiredEventArgs(object cacheKey, object cacheValue, CacheItemRemovedReason reason)
		{
			this.Key = cacheKey;
			this.Value = cacheKey;
			this.Reason = (CacheRemovedReason)Enum.Parse(typeof(CacheRemovedReason), reason.ToString());
		}
	}

	/// <summary>
	/// Specifies the reason an item was removed from the cache.
	/// </summary>
	public enum CacheRemovedReason
	{
		/// <summary>
		/// The item is removed from the cache by a System.Web.Caching.Cache.Remove(System.String)
		/// method call or by an System.Web.Caching.Cache.Insert(System.String,System.Object)
		/// method call that specified the same key.
		/// </summary>
		Removed = 1,
		
		/// <summary>
		/// The item is removed from the cache because it expired.
		/// </summary>
		Expired = 2,

		/// <summary>
		/// The item is removed from the cache because the system removed it to free memory.
		/// </summary>
		Underused = 3,
		
		/// <summary>
		/// The item is removed from the cache because the cache dependency associated with it changed.
		/// </summary>
		DependencyChanged = 4,
	}
}

