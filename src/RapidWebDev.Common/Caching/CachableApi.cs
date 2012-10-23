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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;

namespace RapidWebDev.Common.Caching
{
	/// <summary>
	/// Provide base methods to support cache of api.
	/// </summary>
	public abstract class CachableApi
	{
		private static readonly TimeSpan DEFAULT_CACHE_DURATION = new TimeSpan(0, 30, 0);
		private TimeSpan? cacheDuration;
		private CachePriorityTypes cachePriorityType = CachePriorityTypes.Normal;
		private IApplicationContext applicationContext;

		/// <summary>
		/// Constructor for non-SAAS architect application.
		/// </summary>
		protected CachableApi()
		{

		}

		/// <summary>
		/// Constructor for SAAS architect application.
		/// </summary>
		/// <param name="applicationContext">Sets application context to support caching for different applications in SAAS architect.</param>
		protected CachableApi(IApplicationContext applicationContext)
		{
			this.applicationContext = applicationContext;
		}

		/// <summary>
		/// Sets/gets cache instance
		/// </summary>
		public ICache Cache { private get; set; }

		/// <summary>
		/// Sets/gets cache priority.
		/// </summary>
		public CachePriorityTypes CachePriorityType 
		{
			get { return this.cachePriorityType; }
			set { this.cachePriorityType = value; } 
		}

		/// <summary>
		/// Sets/gets caching duration.
		/// </summary>
		public TimeSpan CacheDuration
		{
			get
			{
				if (this.cacheDuration.HasValue)
					return this.cacheDuration.Value;

				return DEFAULT_CACHE_DURATION;
			}
			set { this.cacheDuration = value; }
		}

		/// <summary>
		/// Add caching object. 
		/// The caching object overwrites existed one if specified key existed in cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		protected void AddCache(object key, object value)
		{
			if (this.Cache == null) return;

			string formattedKey = this.FormatCacheKey(key);
			this.Cache.Add(formattedKey, CloneObject(value), this.CacheDuration, this.CachePriorityType);
		}

		/// <summary>
		/// Remove caching object.
		/// </summary>
		/// <param name="key"></param>
		protected void RemoveCache(object key)
		{
			if (this.Cache == null) return;

			string formattedKey = this.FormatCacheKey(key);
			this.Cache.Remove(formattedKey);
		}

		/// <summary>
		/// Gets cached object. Returns null if the object is unavailable in cache.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected T GetCacheObject<T>(object key)
		{
			if (this.Cache == null) return default(T);

			string formattedKey = this.FormatCacheKey(key);
			object cachedObject = this.Cache.Get(formattedKey);
			if (cachedObject == null)
				return default(T);

			ICloneable cloneableObject = cachedObject as ICloneable;
			if (cloneableObject != null)
				return (T)cloneableObject.Clone();

			return (T)cachedObject;
		}

		/// <summary>
		/// By default, this method formats cache key by combining this.GetType().FullName and provided key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected virtual string FormatCacheKey(object key)
		{
			string cacheKey = null;
			if (this.applicationContext != null)
				cacheKey = string.Format("{0}::{1}@{2}", this.GetType().FullName, key, this.applicationContext.ApplicationId);
			else
				cacheKey = string.Format("{0}::{1}", this.GetType().FullName, key);

			if (cacheKey.Length >= 256)
				cacheKey = "hash-" + cacheKey.GetHashCode().ToString();

			return cacheKey;
		}

		private object CloneObject(object target)
		{
			ICloneable cloneable = target as ICloneable;
			if (cloneable != null) return cloneable;

			return target;
		}
	}
}

