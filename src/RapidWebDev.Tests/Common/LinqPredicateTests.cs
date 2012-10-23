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
using RapidWebDev.Common;
using NUnit.Framework;

namespace RapidWebDev.Tests.Common
{
	[TestFixture(Description = "Test cases for linq predicate merge in Common assembly.")]
	public class LinqPredicateTests
	{
		[Test]
		[Description("Simple test linq predicate without merge.")]
		public void LinqPredicateSimpleTest()
		{
			LinqPredicate predicate = new LinqPredicate("Name=@0 AND Height=@1", "Eunge", 170);
			Assert.AreEqual(2, predicate.Parameters.Length);
			Assert.AreEqual("Eunge", predicate.Parameters[0]);
			Assert.AreEqual(170, predicate.Parameters[1]);
			Assert.AreEqual("(Name=@0 AND Height=@1)", predicate.Expression);
		}

		[Test]
		[Description("Merge linq predicate with parameters in ascending order.")]
		public void MergeLinqPredicateWithSequentialParameters()
		{
			LinqPredicate predicate = new LinqPredicate("Name=@0 AND Height=@1", "Eunge", 170);
			predicate.Add("Degree=@0 AND City=@1", "Master", "Shanghai");
			Assert.AreEqual(4, predicate.Parameters.Length);
			Assert.AreEqual("Eunge", predicate.Parameters[0]);
			Assert.AreEqual(170, predicate.Parameters[1]);
			Assert.AreEqual("Master", predicate.Parameters[2]);
			Assert.AreEqual("Shanghai", predicate.Parameters[3]);
			Assert.AreEqual("(Name=@0 AND Height=@1) AND (Degree=@2 AND City=@3)", predicate.Expression);
		}

		[Test]
		[Description("Merge linq predicate with parameters in reversed order.")]
		public void MergeLinqPredicateWithReversedParameters()
		{
			LinqPredicate predicate = new LinqPredicate("Name=@1 AND Height=@0", 170, "Eunge");
			predicate.Add("Degree=@1 AND City=@0", "Shanghai", "Master");
			Assert.AreEqual(4, predicate.Parameters.Length);
			Assert.AreEqual("Eunge", predicate.Parameters[0]);
			Assert.AreEqual(170, predicate.Parameters[1]);
			Assert.AreEqual("Master", predicate.Parameters[2]);
			Assert.AreEqual("Shanghai", predicate.Parameters[3]);
			Assert.AreEqual("(Name=@0 AND Height=@1) AND (Degree=@2 AND City=@3)", predicate.Expression);
		}

		[Test]
		[Description("Merge linq predicate with duplicate variables defined in the expression.")]
		public void MergeLinqPredicateWithDuplicateExpressionVariables()
		{
			LinqPredicate predicate = new LinqPredicate("City=@0 AND Height>=@1", "Shanghai", 170);
			predicate.Add("Name.StartsWith(@0) AND Height>=@1", "Eunge", 170);
			Assert.AreEqual(3, predicate.Parameters.Length);
			Assert.AreEqual("Shanghai", predicate.Parameters[0]);
			Assert.AreEqual(170, predicate.Parameters[1]);
			Assert.AreEqual("Eunge", predicate.Parameters[2]);
			Assert.AreEqual("(City=@0 AND Height>=@1) AND (Name.StartsWith(@2) AND Height>=@1)", predicate.Expression);
		}

		[Test]
		[Description("Merge null or empty linq predicate.")]
		public void MergeNullOrEmptyLinqPredicate()
		{
			LinqPredicate predicate = new LinqPredicate("City=@0 AND Height>=@1", "Shanghai", 170);
			predicate.Add((LinqPredicate)null);
			predicate.Add(new LinqPredicate(null));
			predicate.Add((string)null);
			Assert.AreEqual("(City=@0 AND Height>=@1)", predicate.Expression);
			Assert.AreEqual(2, predicate.Parameters.Length);
		}
	}
}