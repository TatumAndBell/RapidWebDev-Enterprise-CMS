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
using System.Reflection;
using System.Globalization;

namespace RapidWebDev.Common.Validation
{
	/// <summary>
	/// ValidationScope is used to setup error messages in validation and feedback users with all error messages at a time. <br/>
	/// When developers write validation code for submitted forms or other posted data from client, the best user experience is to feedback users with all errors at a time. 
	/// With ValidationScope, the developers only need to declare the using-block as "using (ValidationScope scope = new ValidationScope()) { ... }" and set the errors into the variable scope. 
	/// Then ValidationScope throws an exception in disposing intelligently if there are errors logged. <br/>
	/// ValidationScope supports nesting. A nested ValidationScope doesn't throw an exception in disposing although there are errors logged unless the developers specify "ForceToThrowExceptionIfHasErrorsWhenDisposing=true" in constructor.
	/// But the errors logged in the nested ValidationScope are used in its outside ValidationScope.
	/// </summary>
	public sealed class ValidationScope : IDisposable
	{
		private List<string> errorMessages = new List<string>();

		/// <summary>
		/// Gets the exception type which be thrown in disposing if there are errors logged in the scope.
		/// Defaults to ValidationException.
		/// </summary>
		public Type ThrownExceptionType { get; private set; }

		/// <summary>
		/// True to force to throw exception if there are errors in current validation scope in disposing however there still exists validation scope reference in current thread.
		/// Defaults to false.
		/// </summary>
		public bool ForceToThrowExceptionIfHasErrorsWhenDisposing { get; private set; }

		/// <summary>
		/// Gets all error messages logged.
		/// </summary>
		public IEnumerable<string> ErrorMessages
		{
			get { return this.errorMessages; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ValidationScope()
		{
			ValidationManager.Instance.Push(this);
			this.ThrownExceptionType = typeof(ValidationException);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="thrownExceptionType">Gets the exception type which be thrown in disposing if there are errors logged in the scope.</param>
		public ValidationScope(Type thrownExceptionType) : this(false, thrownExceptionType)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="forceToThrowExceptionIfHasErrorsWhenDisposing">
		/// Force to throw exception if there are errors in current validation scope in disposing however there still exists validation scope reference in current thread.
		/// </param>
		public ValidationScope(bool forceToThrowExceptionIfHasErrorsWhenDisposing)
		{
			this.ForceToThrowExceptionIfHasErrorsWhenDisposing = forceToThrowExceptionIfHasErrorsWhenDisposing;
			ValidationManager.Instance.Push(this);
			this.ThrownExceptionType = typeof(ValidationException);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="forceToThrowExceptionIfHasErrorsWhenDisposing">
		/// Force to throw exception if there are errors in current validation scope in disposing however there still exists validation scope reference in current thread.
		/// </param>
		/// <param name="thrownExceptionType">
		/// Gets the exception type which be thrown in disposing if there are errors logged in the scope.
		/// </param>
		public ValidationScope(bool forceToThrowExceptionIfHasErrorsWhenDisposing, Type thrownExceptionType)
		{
			Kit.NotNull(thrownExceptionType, "thrownExceptionType");
			if (!thrownExceptionType.IsSubclassOf(typeof(Exception)))
				throw new ArgumentException("The argument is not a type of Exception.", "thrownExceptionType");

			ConstructorInfo constructor = thrownExceptionType.GetConstructor(new Type[] { typeof(string) });
			if (constructor == null)
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The exception type {0} doesn't include an constructor in prototype \"ctor (string)\".", thrownExceptionType), "thrownExceptionType");

			this.ForceToThrowExceptionIfHasErrorsWhenDisposing = forceToThrowExceptionIfHasErrorsWhenDisposing;
			ValidationManager.Instance.Push(this);
			this.ThrownExceptionType = thrownExceptionType;
		}

		/// <summary>
		/// Log an error message in current validation scope.
		/// </summary>
		/// <param name="message"></param>
		public void Error(string message)
		{
			this.errorMessages.Add(message);
		}

		/// <summary>
		/// Log an error message in current validation scope.
		/// </summary>
		/// <param name="messageFomat"></param>
		/// <param name="parameters"></param>
		public void Error(string messageFomat, params object[] parameters)
		{
			this.errorMessages.Add(string.Format(CultureInfo.InvariantCulture, messageFomat, parameters));
		}

		/// <summary>
		/// Force to throw an exception if there are errors logged in current validation scope or its nested scopes however it is also nested in another.
		/// </summary>
		public void Throw()
		{
			this.Throw(true);
		}

		#region IDisposable Members

		/// <summary>
		/// Dispose current validation scope.
		/// </summary>
		public void Dispose()
		{
			ValidationManager.Instance.Pop();

			this.Throw(this.ForceToThrowExceptionIfHasErrorsWhenDisposing);
		}

		private readonly static HashSet<string> Interpunctions = new HashSet<string> { ",", ";", ".", "!", "?", "~", "。", "，", "！", "；", "……", "…", "？" };

		private static string RemoveInterpunctionThenMerge(IEnumerable<string> errorMessages)
		{
			if (errorMessages == null) return "";

			StringBuilder output = new StringBuilder();
			foreach (string errorMessage in errorMessages)
			{
				string trimmedErrorMessage = errorMessage.Trim();
				string lastChar = trimmedErrorMessage[trimmedErrorMessage.Length - 1].ToString();
				if (Interpunctions.Contains(lastChar))
					trimmedErrorMessage = trimmedErrorMessage.Substring(0, trimmedErrorMessage.Length - 1);

				if (output.Length > 0) output.Append("; ");
				output.Append(trimmedErrorMessage);
			}

			output.Append(". ");
			return output.ToString();
		}

		private void Throw(bool forceToThrowExceptionIfHasErrorsWhenDisposing)
		{
			if (this.errorMessages.Count == 0) return;

			ValidationScope outsideValidationScope = ValidationManager.Instance.Peek();
			if (forceToThrowExceptionIfHasErrorsWhenDisposing || outsideValidationScope == null)
			{
				ConstructorInfo constructor = this.ThrownExceptionType.GetConstructor(new Type[] { typeof(string) });
				if (constructor != null)
					throw (Exception)constructor.Invoke(new object[] { RemoveInterpunctionThenMerge(this.errorMessages) });

				throw new NotSupportedException();
			}

			outsideValidationScope.errorMessages.AddRange(this.errorMessages);
		}

		#endregion
	}
}
