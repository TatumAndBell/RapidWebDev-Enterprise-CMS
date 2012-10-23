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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.Common.CodeDom;
using RapidWebDev.Platform;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages.Configurations;
using NUnit.Framework;
using Spring.Core;
using Spring.Objects;
using RapidWebDev.Common.Validation;

namespace RapidWebDev.Tests.Common
{
	[TestFixture(Description = "Test cases to start experiences on ValidationScope.")]
	public class ValidationScopeTests
	{
		[Test]
		[ExpectedException(typeof(ValidationException))]
		public void BasicTest()
		{
			using (ValidationScope scope = new ValidationScope())
			{
				scope.Error("Name cannot be empty!");
				scope.Error("Password cannot be empty!");
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BasicTestWithSpecifiedThrownException()
		{
			using (ValidationScope scope = new ValidationScope(typeof(InvalidOperationException)))
			{
				scope.Error("Name cannot be empty!");
				scope.Error("Password cannot be empty!");
			}
		}

		[Test]
		public void NestedValidationScopeTest()
		{
			try
			{
				using (ValidationScope scope = new ValidationScope())
				{
					scope.Error("The organization is invalid!");

					Log2ErrorsIntoValidationScope();
				}
			}
			catch (ValidationException exp)
			{
				Assert.AreEqual(3, exp.Message.Split(';').Count());
			}
		}

		[Test]
		public void ThrowExceptionInNestedValidationScopeTest()
		{
			try
			{
				using (ValidationScope scope = new ValidationScope())
				{
					ThrowValidationScope();

					scope.Error("Error1.");
					scope.Error("Error2.");
				}
			}
			catch (ValidationException exp)
			{
				string[] errorMessages = exp.Message.Split(';');
				Assert.AreEqual(2, errorMessages.Length);
			}
		}

		[Test]
		[ExpectedException(typeof(ValidationException))]
		public void ThrowExceptionInNestedMethodTest()
		{
			using (ValidationScope scope = new ValidationScope())
			{
				scope.Error("Error1.");
				scope.Error("Error2.");

				ThrowInvalidProgramException();
			}
		}

		private void Log2ErrorsIntoValidationScope()
		{
			using (ValidationScope scope = new ValidationScope(typeof(InvalidOperationException)))
			{
				scope.Error("Name cannot be empty!");
				scope.Error("Password cannot be empty!");
			}
		}

		private void ThrowValidationScope()
		{
			using (ValidationScope scope = new ValidationScope(typeof(InvalidOperationException)))
			{
				scope.Error("Name cannot be empty!");
				scope.Error("Password cannot be empty!");
				scope.Throw();
			}
		}

		private void ThrowInvalidProgramException()
		{
			using (ValidationScope scope = new ValidationScope(typeof(InvalidOperationException)))
			{
				throw new InvalidProgramException();
			}
		}
	}
}

