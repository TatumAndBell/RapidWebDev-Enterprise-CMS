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
using System.Web;
using System.Web.Caching;

namespace RapidWebDev.Common.Caching
{
	/// <summary>
	/// Cache implementation bases on http runtime cache.
	/// </summary>
	public class HttpRuntimeCache : ICache
	{
		/// <summary>
		/// Gets caching object count.
		/// </summary>
		public int Count { get { return HttpRuntime.Cache.Count; } }

		/// <summary>
		/// Add object to cache container by specified expire sliding duration and priority type.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="slidingDuration"></param>
		/// <param name="cachePriorityType"></param>
		public void Add(object key, object value, TimeSpan slidingDuration, CachePriorityTypes cachePriorityType)
		{
			Kit.NotNull(key, "key");

			object cachedObject = HttpRuntime.Cache.Get(key.ToString());
			if (cachedObject != null) 
				HttpRuntime.Cache.Remove(key.ToString());

			HttpRuntime.Cache.Add(key.ToString(), value, null, Cache.NoAbsoluteExpiration, slidingDuration, (CacheItemPriority)Enum.Parse(typeof(CacheItemPriority), cachePriorityType.ToString()), null);
		}

		/// <summary>
		/// Get caching object by key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object Get(object key)
		{
			Kit.NotNull(key, "key");
			return HttpRuntime.Cache.Get(key.ToString());
		}

		/// <summary>
		/// Remove caching object by key.
		/// </summary>
		/// <param name="key"></param>
		public void Remove(object key)
		{
			Kit.NotNull(key, "key");
			HttpRuntime.Cache.Remove(key.ToString());
		}
	}
}

