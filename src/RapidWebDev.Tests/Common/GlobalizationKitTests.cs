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
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.Tests.Common
{
	[TestFixture]
	[Description("Test GlobalizationKit.")]
	public class GlobalizationKitTests
	{
		/// <summary>
		/// Protected mocked resource name.
		/// </summary>
		protected static string ProtectedMockedResourceName { get { return "Protected"; } }

		/// <summary>
		/// Public mocked resource name.
		/// </summary>
		public static string PublicMockedResourceName { get { return "Public"; } }

		[Test]
		[Description("Using GlobalizationKit to transform input string having globalization marks.")]
		public void TransformGlobalizationMarks()
		{
			string inputString = "Test Case For $RapidWebDev.Tests.Common.GlobalizationKitTests.ProtectedMockedResourceName, RapidWebDev.Tests$.";
			string outputString = GlobalizationUtility.ReplaceGlobalizationVariables(inputString);

			string expectedString = "Test Case For Protected.";
			Assert.AreEqual(expectedString, outputString);

			inputString = "Test Case For $RapidWebDev.Tests.Common.GlobalizationKitTests.PublicMockedResourceName, RapidWebDev.Tests$ AND $RapidWebDev.Tests.Common.GlobalizationKitTests.ProtectedMockedResourceName, RapidWebDev.Tests$.";
			outputString = GlobalizationUtility.ReplaceGlobalizationVariables(inputString);

			expectedString = "Test Case For Public AND Protected.";
			Assert.AreEqual(expectedString, outputString);
		}
	}
}