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
using System.Threading;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Platform.Linq;
using NUnit.Framework;

namespace RapidWebDev.Tests.Common
{
	[TestFixture]
	[Description("Test interception to transaction scope.")]
	public class RwdTransactionScopeTests
	{
		[Test]
		[Description(@"The cache doesn't work with database transactions originally. 
						But in service oriented API design, we will have synchronization problems in a series of API execution when we have both cache and transaction in each individual API. 
						RwdTransactionScope is designed to synchronize cache with database transactions.
						In this test case, we're testing the object saved into cache will be rollback with the transaction.")]
		public void CacheRollbackWithTransactionTest()
		{
			string applicationName = "CacheWithinTransactionScopeTest";
			string cacheKey = "CacheWithinTransactionScopeTest.CacheKey";
			ICache cache = SpringContext.Current.GetObject<ICache>();

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			using (TransactionScope transactionScope = new TransactionScope())
			{
				// create a new application to database
				Application application = new Application
				{
					ApplicationName = applicationName,
					LoweredApplicationName = applicationName.ToLowerInvariant(),
					Description = ""
				};

				ctx.Applications.InsertOnSubmit(application);
				ctx.SubmitChanges();

				// save it into cache for 1 hour
				cache.Add(cacheKey, application, new TimeSpan(1, 0, 0), CachePriorityTypes.High);

				// the application should exist in both database and cache in the transaction
				Assert.AreEqual(1, ctx.Applications.Count(app => app.ApplicationName == applicationName), "The application should exist in database.");
				Assert.IsNotNull(cache.Get(cacheKey), "The application should exist in cache.");

				// it doesn't commit the transaction by the end of the transaction scope by calling "transactionScope.Complete();"
				// that means after this transaction scope, the transaction should be rollback
			}

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				// the application should not exist in both database and cache in the transaction.
				Assert.AreEqual(0, ctx.Applications.Count(app => app.ApplicationName == applicationName), "The application should not exist in database.");
				Assert.IsNull(cache.Get(cacheKey), "The application should not exist in cache.");
			}
		}

		[Test]
		[Description(@"The cache doesn't work with database transactions originally. 
						But in service oriented API design, we will have synchronization problems in a series of API execution when we have both cache and transaction in each individual API. 
						RwdTransactionScope is designed to synchronize cache with database transactions.
						In this test case, we're testing the object saved into cache successfully with the transaction completed.")]
		public void CacheCommittedWithTransactionTest()
		{
			string applicationName = "CacheWithinTransactionScopeTest";
			string cacheKey = "CacheWithinTransactionScopeTest.CacheKey";
			ICache cache = SpringContext.Current.GetObject<ICache>();

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			using (TransactionScope transactionScope = new TransactionScope())
			{
				// create a new application to database
				Application application = new Application
				{
					ApplicationName = applicationName,
					LoweredApplicationName = applicationName.ToLowerInvariant(),
					Description = ""
				};

				ctx.Applications.InsertOnSubmit(application);
				ctx.SubmitChanges();

				// save it into cache for 1 hour
				cache.Add(cacheKey, application, new TimeSpan(1, 0, 0), CachePriorityTypes.High);

				// the application should exist in both database and cache in the transaction
				Assert.AreEqual(1, ctx.Applications.Count(app => app.ApplicationName == applicationName), "The application should exist in database.");
				Assert.IsNotNull(cache.Get(cacheKey), "The application should exist in cache.");

				// complete the transaction
				transactionScope.Complete();
			}

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				// the application should exist in both database and cache in the transaction
				Assert.AreEqual(1, ctx.Applications.Count(app => app.ApplicationName == applicationName), "The application should exist in database.");
				Assert.IsNotNull(cache.Get(cacheKey), "The application should exist in cache.");

				// clear testing data.
				ctx.Applications.Delete(app => app.ApplicationName == applicationName);
				cache.Remove(cacheKey);
			}
		}

		[Test]
		[Description("The test case is to test all callback registered to transaction scope context will be invoked.")]
		public void RegisterCallbackToTransactionStatusChangedTest()
		{
			object locker = new object();
			List<int> newTransactionThreadIds = new List<int>();
			List<int> completedTransactionThreadIds = new List<int>();
			List<int> rollbackTransactionThreadIds = new List<int>();

			TransactionScopeContext.AfterNewTransactionStarted += threadId =>
				{
					newTransactionThreadIds.Remove(threadId);
				};

			TransactionScopeContext.AfterTransactionCommitted += threadId =>
				{
					completedTransactionThreadIds.Remove(threadId);
				};

			TransactionScopeContext.AfterTransactionRollback += threadId =>
				{
					rollbackTransactionThreadIds.Remove(threadId);
				};

			WaitCallback waitTransactionCompleteCallback = state =>
				{
					Monitor.Enter(locker);
					try
					{
						newTransactionThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
						using (TransactionScope transactionScope = new TransactionScope())
						{
							completedTransactionThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
							transactionScope.Complete();
						}
					}
					finally
					{
						Monitor.Exit(locker);
					}
				};

			WaitCallback waitTransactionRollbackCallback = state =>
			{
				Monitor.Enter(locker);
				try
				{
					newTransactionThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
					using (TransactionScope transactionScope = new TransactionScope())
					{
						rollbackTransactionThreadIds.Add(Thread.CurrentThread.ManagedThreadId);

						// don't commit the transaction
					}
				}
				finally
				{
					Monitor.Exit(locker);
				}
			};

			ThreadPool.QueueUserWorkItem(waitTransactionCompleteCallback);
			ThreadPool.QueueUserWorkItem(waitTransactionRollbackCallback);

			Thread.Sleep(500);
			Monitor.Enter(locker);
			try
			{
				Assert.AreEqual(0, newTransactionThreadIds.Count, "The thread Ids should be removed in the registered callback.");
				Assert.AreEqual(0, completedTransactionThreadIds.Count, "The thread Ids should be removed in the registered callback.");
				Assert.AreEqual(0, rollbackTransactionThreadIds.Count, "The thread Ids should be removed in the registered callback.");
			}
			finally
			{
				Monitor.Exit(locker);
			}
		}
	}
}