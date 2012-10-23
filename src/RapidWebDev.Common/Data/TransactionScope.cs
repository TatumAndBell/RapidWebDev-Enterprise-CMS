/****************************************************************************************************
    Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
    Author: Eunge, Legal Name: Jian Liu, Email: eunge.liu@RapidWebDev.org

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ****************************************************************************************************/

namespace RapidWebDev.Common.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Transactions;
	using System.Globalization;

	/// <summary>
	/// Rapid web development provided transaction scope which integrates .NET TransactionScope to support transactional caching. 
	/// The facing problem before is that the cache for an individual API cannot be controlled by transaction scope.
	/// When we developed APIs with transaction which also need to interact with cache. 
	/// In each individual API, it interacts with cache after its own transaction committed. 
	/// But in a business operation it needs an unique transaction wraps a series of APIs'. 
	/// In this case, the business operation's transaction becomes the real transaction which decides a series of API calls being successful or failed. 
	/// We can imagine that there should not have any data been changed if the whole transaction rollback although an individual API's transaction committed.
	/// The problem is that an individual API interacts with cache after commited its smaller transaction. And the cache cannot be rollback after the outside transaction rollback. 
	/// Finally somewhere depends on the cache will be inconsistent to the database. 
	/// </summary>
	public sealed class TransactionScope : IDisposable
	{
		private System.Transactions.TransactionScope transactionScope;
		private bool isCommitted;

		/// <summary>
		/// Constructor
		/// </summary>
		public TransactionScope()
			: this(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, new TimeSpan(0, 0, 30))
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isolationLevel">the isolation level of the transaction.</param>
		public TransactionScope(IsolationLevel isolationLevel)
			: this(TransactionScopeOption.Required, isolationLevel, new TimeSpan(0, 0, 30))
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isolationLevel">the isolation level of the transaction.</param>
		/// <param name="timeout">the timeout period for the transaction.</param>
		public TransactionScope(IsolationLevel isolationLevel, TimeSpan timeout)
			: this(TransactionScopeOption.Required, isolationLevel, timeout)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="transactionScopeOption">An instance of the System.Transactions.TransactionScopeOption enumeration that describes the transaction requirements associated with this transaction scope.</param>
		public TransactionScope(TransactionScopeOption transactionScopeOption)
			: this(transactionScopeOption, IsolationLevel.ReadCommitted, new TimeSpan(0, 0, 30))
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="transactionScopeOption">An instance of the System.Transactions.TransactionScopeOption enumeration that describes the transaction requirements associated with this transaction scope.</param>
		/// <param name="isolationLevel">the isolation level of the transaction.</param>
		public TransactionScope(TransactionScopeOption transactionScopeOption, IsolationLevel isolationLevel)
			: this(transactionScopeOption, isolationLevel, new TimeSpan(0, 0, 30))
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="transactionScopeOption">An instance of the System.Transactions.TransactionScopeOption enumeration that describes the transaction requirements associated with this transaction scope.</param>
		/// <param name="isolationLevel">the isolation level of the transaction.</param>
		/// <param name="timeout">the timeout period for the transaction.</param>
		public TransactionScope(TransactionScopeOption transactionScopeOption, IsolationLevel isolationLevel, TimeSpan timeout)
		{
			TransactionOptions options = new TransactionOptions();
			options.IsolationLevel = (System.Transactions.IsolationLevel)Enum.Parse(typeof(System.Transactions.IsolationLevel), isolationLevel.ToString());
			options.Timeout = timeout;

			System.Transactions.TransactionScopeOption transactionScopeOptionValue = (System.Transactions.TransactionScopeOption)Enum.Parse(typeof(System.Transactions.TransactionScopeOption), transactionScopeOption.ToString());
			this.transactionScope = new System.Transactions.TransactionScope(transactionScopeOptionValue, options);
			TransactionScopeContext.Increase();
		}

		/// <summary>
		///  Indicates that all operations within the scope are completed successfully.
		/// </summary>
		public void Complete()
		{
			this.transactionScope.Complete();
			this.isCommitted = true;
			TransactionScopeContext.Decrease();
		}

		#region IDisposable Members

		/// <summary>
		/// The dispose method
		/// </summary>
		/// <param name="disposing">called from Dispose?</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.transactionScope.Dispose();
				if (!this.isCommitted)
					TransactionScopeContext.Rollback();
			}
		}

		/// <summary>
		/// The dispose method
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}

