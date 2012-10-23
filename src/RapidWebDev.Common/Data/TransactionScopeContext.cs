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
using System.Threading;
using System.Web;

namespace RapidWebDev.Common.Data
{
	/// <summary>
	/// Transaction scope context which can be registered callback on transaction events.
	/// </summary>
	public sealed class TransactionScopeContext
	{
		/// <summary>
		/// The callback will be invoked after a new transaction for current executing thread been started.
		/// </summary>
		public static Action<int> AfterNewTransactionStarted;

		/// <summary>
		/// The callback will be invoked after the transaction for current executing thread been committed.
		/// </summary>
		public static Action<int> AfterTransactionCommitted;

		/// <summary>
		/// The callback will be invoked after the transaction for current executing thread been rolled back.
		/// </summary>
		public static Action<int> AfterTransactionRollback;

		/// <summary>
		/// Gets whether the executing thread is in a transaction.
		/// </summary>
		/// <returns></returns>
		public static bool HasTransaction
		{
			get { return CurrentThreadTransactionRefCount > 0; }
		}

		/// <summary>
		/// Increase the transaction reference for current executing thread.
		/// </summary>
		internal static void Increase()
		{
			if (CurrentThreadTransactionRefCount == 0 && AfterNewTransactionStarted != null)
			{
				CurrentThreadTransactionRefCount = 1;
				AfterNewTransactionStarted(Thread.CurrentThread.ManagedThreadId);
			}
			else
				CurrentThreadTransactionRefCount++;
		}

		/// <summary>
		/// Decrease the transaction reference for current executing thread. 
		/// The callback "" will be invoked when the reference decreased to zero.
		/// </summary>
		/// <exception cref="InvalidProgramException">If there is no transaction available for current executing thread.</exception>
		internal static void Decrease()
		{
			CurrentThreadTransactionRefCount--;
			if (CurrentThreadTransactionRefCount > 0) return;

			if (AfterTransactionCommitted != null)
				AfterTransactionCommitted(Thread.CurrentThread.ManagedThreadId);
		}

		/// <summary>
		/// Rollback the transaction for current executing thread.
		/// </summary>
		internal static void Rollback()
		{
			CurrentThreadTransactionRefCount = 0;
			if (AfterTransactionRollback != null)
				AfterTransactionRollback(Thread.CurrentThread.ManagedThreadId);
		}

		private static readonly Dictionary<int, int> transactionReferences = new Dictionary<int, int>();
		private const string CurrentThreadTransactionRefCountKey = "CurrentThreadTransactionRefCount";
		private static int CurrentThreadTransactionRefCount
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (!HttpContext.Current.Items.Contains(CurrentThreadTransactionRefCountKey))
						return 0;

					return (int)HttpContext.Current.Items[CurrentThreadTransactionRefCountKey];
				}
				else
				{
					if (!transactionReferences.ContainsKey(Thread.CurrentThread.ManagedThreadId))
						return 0;

					return transactionReferences[Thread.CurrentThread.ManagedThreadId];
				}
			}
			set
			{
				if (HttpContext.Current != null)
					HttpContext.Current.Items[CurrentThreadTransactionRefCountKey] = value;
				else
					transactionReferences[Thread.CurrentThread.ManagedThreadId] = value;
			}
		}
	}
}
