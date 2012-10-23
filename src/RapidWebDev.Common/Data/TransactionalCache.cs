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
using System.Threading;
using System.Web;
using System.Web.Caching;
using RapidWebDev.Common.Caching;

namespace RapidWebDev.Common.Data
{
	/// <summary>
	/// Transactional cache which decorates a physical cache passed into the constructor and make it being in the transaction scope. 
	/// </summary>
	public class TransactionalCache : ICache
	{
		private static readonly Dictionary<int, Dictionary<object, CacheCommand>> transactionalCache = new Dictionary<int, Dictionary<object, CacheCommand>>();
		private ICache physicalCache;

		/// <summary>
		/// Transactional cache which decorates a physical cache passed into the constructor and make it being in the transaction scope. 
		/// </summary>
		/// <param name="cache"></param>
		public TransactionalCache(ICache cache)
		{
			Kit.NotNull(cache, "cache");
			this.physicalCache = cache;

			TransactionScopeContext.AfterNewTransactionStarted += new Action<int>(AfterNewTransactionStarted);
			TransactionScopeContext.AfterTransactionCommitted += new Action<int>(AfterTransactionCommitted);
			TransactionScopeContext.AfterTransactionRollback += new Action<int>(AfterTransactionRollback);
		}

		private Dictionary<object, CacheCommand> CacheCommands
		{
			get
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				if (!transactionalCache.ContainsKey(threadId))
					transactionalCache[threadId] = new Dictionary<object, CacheCommand>();

				return transactionalCache[threadId];
			}
		}

		/// <summary>
		/// Gets caching object count includes not committed cache item in transaction.
		/// </summary>
		public int Count 
		{ 
			get 
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				if (!transactionalCache.ContainsKey(threadId)) return this.physicalCache.Count; 

				Dictionary<object, CacheCommand> cacheCommandDictionary = transactionalCache[Thread.CurrentThread.ManagedThreadId];
				if (cacheCommandDictionary.Count == 0) return this.physicalCache.Count; ;

				int cacheItemCount = this.physicalCache.Count;
				foreach (CacheCommand command in cacheCommandDictionary.Values)
				{
					bool existedCacheKey = this.physicalCache.Get(command.Item.Key) != null;
					if (existedCacheKey && command.Type == CacheCommandType.Remove) cacheItemCount--;
					else if (!existedCacheKey && command.Type == CacheCommandType.Add) cacheItemCount++;
				}

				return cacheItemCount;
			} 
		}

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

			if (!TransactionScopeContext.HasTransaction)
			{
				this.physicalCache.Add(key, value, slidingDuration, cachePriorityType);
				return;
			}

			this.CacheCommands[key] = new CacheCommand
			{
				Type = CacheCommandType.Add,
				Item = new CacheItem
				{
					Key = key,
					Value = value,
					SlidingDuration = slidingDuration,
					CachePriorityType = cachePriorityType
				}
			};
		}

		/// <summary>
		/// Get caching object by key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object Get(object key)
		{
			Kit.NotNull(key, "key");

			if (transactionalCache.ContainsKey(Thread.CurrentThread.ManagedThreadId) && this.CacheCommands.ContainsKey(key))
			{
				CacheCommand command = this.CacheCommands[key];
				if (command.Type == CacheCommandType.Remove) return null;
				else if (command.Type == CacheCommandType.Add) return command.Item.Value;
			}

			return this.physicalCache.Get(key);
		}

		/// <summary>
		/// Remove caching object by key.
		/// </summary>
		/// <param name="key"></param>
		public void Remove(object key)
		{
			Kit.NotNull(key, "key");

			if (!TransactionScopeContext.HasTransaction)
			{
				this.physicalCache.Remove(key);
				return;
			}

			this.CacheCommands[key] = new CacheCommand
			{
				Type = CacheCommandType.Remove,
				Item = new CacheItem
				{
					Key = key,
					Value = null,
					SlidingDuration = TimeSpan.Zero,
					CachePriorityType = CachePriorityTypes.Default
				}
			};
		}

		private void AfterNewTransactionStarted(int threadId)
		{
			if (transactionalCache.ContainsKey(threadId))
				transactionalCache.Remove(threadId);
		}

		private void AfterTransactionCommitted(int threadId)
		{
			if (transactionalCache.ContainsKey(threadId))
			{
				foreach (CacheCommand command in this.CacheCommands.Values)
				{
					if (command.Type == CacheCommandType.Remove)
						this.physicalCache.Remove(command.Item.Key);
					else if (command.Type == CacheCommandType.Add)
						this.physicalCache.Add(command.Item.Key, command.Item.Value, command.Item.SlidingDuration, command.Item.CachePriorityType);
				}

				transactionalCache.Remove(threadId);
			}
		}

		private void AfterTransactionRollback(int threadId)
		{
			if (transactionalCache.ContainsKey(threadId))
				transactionalCache.Remove(threadId);
		}

		#region Internal caching command and item

		private class CacheCommand
		{
			/// <summary>
			/// caching command type - add/remove a cache item
			/// </summary>
			public CacheCommandType Type { get; set; }

			/// <summary>
			/// cache item
			/// </summary>
			public CacheItem Item { get; set; }
		}

		private enum CacheCommandType { Add, Remove }

		private class CacheItem
		{
			/// <summary>
			/// cache key
			/// </summary>
			public object Key { get; set; }

			/// <summary>
			/// cache value
			/// </summary>
			public object Value { get; set; }

			/// <summary>
			/// caching sliding duration
			/// </summary>
			public TimeSpan SlidingDuration { get; set; }

			/// <summary>
			/// caching priority type.
			/// </summary>
			public CachePriorityTypes CachePriorityType { get; set; }
		}

		#endregion
	}
}

