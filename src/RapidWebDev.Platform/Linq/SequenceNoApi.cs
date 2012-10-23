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
using RapidWebDev.Common;
using RapidWebDev.Common.Data;

namespace RapidWebDev.Platform.Linq
{
	/// <summary>
	/// The implementation to the interface ISequenceNoApi. 
	/// An business example is to generate order number in product-order system.
	/// The class generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
	/// </summary>
	public class SequenceNoApi : ISequenceNoApi
	{
		private static readonly object syncObj = new object();
		private IAuthenticationContext authenticationContext;

		/// <summary>
		/// Construct API instance to generate sequence number
		/// </summary>
		/// <param name="authenticationContext"></param>
		public SequenceNoApi(IAuthenticationContext authenticationContext)
		{
			this.authenticationContext = authenticationContext;
		}

		/// <summary>
		/// Create sequence number on specified type for special object id.
		/// The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
		/// </summary>
		/// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
		/// <returns></returns>
		public long Create(string sequenceNoType)
		{
			return this.Create(Guid.Empty, sequenceNoType);
		}

		/// <summary>
		/// Create sequence number on specified type for special object id.
		/// The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
		/// </summary>
		/// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
		/// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
		/// <returns></returns>
		public long Create(Guid objectId, string sequenceNoType)
		{
			lock (syncObj)
			{
				try
				{
					using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress))
					using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
					{
						SequenceNo sequenceNo = ctx.SequenceNos.Where(s => s.ObjectId.Equals(objectId)
							&& s.Type.Equals(sequenceNoType)
							&& s.ApplicationId == this.authenticationContext.ApplicationId).FirstOrDefault();

						if (sequenceNo == null)
						{
							sequenceNo = new SequenceNo()
							{
								ApplicationId = this.authenticationContext.ApplicationId,
								ObjectId = objectId,
								Type = sequenceNoType,
								Number = 1
							};

							ctx.SequenceNos.InsertOnSubmit(sequenceNo);
						}
						else
						{
							sequenceNo.Number++;
						}

						ctx.SubmitChanges();
						ts.Complete();
						return sequenceNo.Number;
					}
				}
				catch (Exception exp)
				{
					Logger.Instance(this).Error(exp);
					throw;
				}
			}
		}

		/// <summary>
		/// Create sequence numbers on specified sequence number type.
		///  The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
		/// </summary>
		/// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
		/// <param name="sequenceNoCount">Indicates how many sequence number is required.</param>
		/// <returns></returns>
		public IEnumerable<long> Create(string sequenceNoType, ushort sequenceNoCount)
		{
			return this.Create(sequenceNoType, sequenceNoCount);
		}

		/// <summary>
		/// Create sequence numbers on specified type for special object id.
		///  The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
		/// </summary>
		/// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
		/// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
		/// <param name="sequenceNoCount">Indicates how many sequence number is required.</param>
		/// <returns></returns>
		public IEnumerable<long> Create(Guid objectId, string sequenceNoType, ushort sequenceNoCount)
		{
			if (sequenceNoCount <= 0)
				throw new ArgumentException("The argument \"sequenceNoCount\" should be more than zero.", "sequenceNoCount");

			lock (syncObj)
			{
				try
				{
					using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress))
					using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
					{
						SequenceNo sequenceNo = ctx.SequenceNos.Where(s => s.ObjectId.Equals(objectId)
							&& s.Type.Equals(sequenceNoType)
							&& s.ApplicationId == this.authenticationContext.ApplicationId).FirstOrDefault();

						long startSequenceNo;
						if (sequenceNo == null)
						{
							sequenceNo = new SequenceNo()
							{
								ApplicationId = this.authenticationContext.ApplicationId,
								ObjectId = objectId,
								Type = sequenceNoType,
								Number = sequenceNoCount
							};

							startSequenceNo = 1;
							ctx.SequenceNos.InsertOnSubmit(sequenceNo);
						}
						else
						{
							startSequenceNo = sequenceNo.Number + 1;
							sequenceNo.Number += sequenceNoCount;
						}

						ctx.SubmitChanges();
						ts.Complete();

						List<long> sequenceNumbers = new List<long>();
						for (long i = startSequenceNo; i < startSequenceNo + sequenceNoCount; i++)
							sequenceNumbers.Add(i);

						return sequenceNumbers;
					}
				}
				catch (Exception exp)
				{
					Logger.Instance(this).Error(exp);
					throw;
				}
			}
		}
	}
}

