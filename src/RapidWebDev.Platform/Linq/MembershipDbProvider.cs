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
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using RapidWebDev.Platform.Properties;
using AspNetMembership = System.Web.Security.Membership;

namespace RapidWebDev.Platform.Linq
{
	/// <summary>
	/// Membership database provider implemented in linq-2-sql. The provide interacts with datatabase only without caching.
	/// The reason to create MembershipDbProvider to separate database and cache for users is to reduce the code complexity.
	/// </summary>
	internal class MembershipDbProvider
	{
		private static RNGCryptoServiceProvider RNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		private IAuthenticationContext authenticationContext;
		private IOrganizationApi organizationApi;

		/// <summary>
		/// Construct membership API instance.
		/// </summary>
		/// <param name="authenticationContext"></param>
		/// <param name="organizationApi"></param>
		public MembershipDbProvider(IAuthenticationContext authenticationContext, IOrganizationApi organizationApi)
		{
			this.authenticationContext = authenticationContext;
			this.organizationApi = organizationApi;
		}

		/// <summary>
		/// Save user object with specified password and password retrieval answer. 
		/// If it's used to update an existed user, the password allows to be null in the case that it doesn't need to change the password of the user.
		/// The argument "passwordAnswer" is to be validated depends on the configuration on ASP.NET Membership "RequiresQuestionAndAnswer".
		/// </summary>
		/// <param name="userObject">user business object</param>
		/// <param name="password">login password. If it's used to update an existed user, the password allows to be null in the case that it doesn't need to change the password of the user.</param>
		/// <param name="passwordAnswer">password retrieve answer. The argument "passwordAnswer" is to be validated depends on the configuration on ASP.NET Membership "RequiresQuestionAndAnswer".</param>
		/// <exception cref="ValidationException">Save user failed by various reasons implied in exception message.</exception>
		/// <exception cref="ArgumentException">The property userObject.Id is specified with an invalid value.</exception>
		/// <exception cref="ArgumentNullException">The argument userObject is null.</exception>
		public void Save(UserObject userObject, string password, string passwordAnswer)
		{
			Kit.NotNull(userObject, "userObject");

			OrganizationObject organizationObject = this.organizationApi.GetOrganization(userObject.OrganizationId);
			if (organizationObject == null)
				throw new ArgumentException(Resources.InvalidOrganizationID, "userObject.OrganizationId");

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					string passwordSalt;
					int passwordFormat;
					string encodedPassword;
					string encodedPasswordAnswer;

					User userEntity = null;
					Membership membership = null;
					using (ValidationScope validationScope = new ValidationScope(true))
					{
						this.ValidateUserData(ctx, userObject);

						if (userObject.UserId == Guid.Empty)
						{
							Kit.NotNull(password, "password");
							passwordSalt = GeneratePasswordSalt();
							this.ValidatePasswordQuestionAndAnswer(passwordSalt, userObject.PasswordQuestion, passwordAnswer, out encodedPasswordAnswer);
							this.ValidatePassword(passwordSalt, password, out encodedPassword, out passwordFormat);

							userEntity = ExtensionObjectFactory.Create<User>(userObject);
							userEntity.ApplicationId = this.authenticationContext.ApplicationId;
							userEntity.UserId = Guid.NewGuid();
							userEntity.LastActivityDate = new DateTime(1930, 1, 1);
							ctx.Users.InsertOnSubmit(userEntity);

							membership = this.ConstructMembership(userEntity.UserId);
							membership.PasswordSalt = passwordSalt;
							membership.Password = encodedPassword;
							membership.PasswordFormat = passwordFormat;
							membership.PasswordQuestion = userObject.PasswordQuestion;
							membership.PasswordAnswer = encodedPasswordAnswer;
							ctx.Memberships.InsertOnSubmit(membership);

							userObject.UserId = userEntity.UserId;
							userObject.CreationDate = DateTime.UtcNow;
						}
						else
						{
							userEntity = ctx.Users.FirstOrDefault(user => user.UserId == userObject.UserId);
							if (userEntity == null)
								throw new ArgumentException(string.Format(Resources.InvalidUserID, userObject.UserId));

							userEntity.ExtensionDataTypeId = userObject.ExtensionDataTypeId;

							membership = ctx.Memberships.FirstOrDefault(m => m.UserId == userObject.UserId);
							if (membership == null)
								throw new ArgumentException(string.Format(Resources.InvalidUserID, userObject.UserId));

							passwordSalt = membership.PasswordSalt;
							if (!Kit.IsEmpty(passwordAnswer))
							{
								this.ValidatePasswordQuestionAndAnswer(passwordSalt, userObject.PasswordQuestion, passwordAnswer, out encodedPasswordAnswer);
								membership.PasswordQuestion = userObject.PasswordQuestion;
								membership.PasswordAnswer = encodedPasswordAnswer;
							}

							if (!Kit.IsEmpty(password))
							{
								this.ValidatePassword(passwordSalt, password, out encodedPassword, out passwordFormat);
								membership.Password = encodedPassword;
								membership.PasswordFormat = passwordFormat;
							}
						}
					}

					userEntity.OrganizationId = userObject.OrganizationId;
					userEntity.UserName = userObject.UserName;
					userEntity.DisplayName = userObject.DisplayName;
					userEntity.IsAnonymous = false;
					userEntity.LastUpdatedDate = DateTime.UtcNow;
					userEntity.LoweredUserName = userEntity.UserName.ToLowerInvariant();
					userEntity.MobileAlias = userObject.MobilePin;
					userEntity.ParseExtensionPropertiesFrom(userObject);

					membership.Comment = userObject.Comment;
					membership.Email = userObject.Email;
					membership.IsApproved = userObject.IsApproved;
					membership.LoweredEmail = userObject.Email != null ? userObject.Email.ToLowerInvariant() : userObject.Email;
					membership.MobilePIN = userObject.MobilePin;

					ctx.SubmitChanges();

					userObject.LastUpdatedDate = DateTime.UtcNow;
				}
			}
			catch (ValidationException)
			{
				throw;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Resolve user elements from enumerable user ids.
		/// </summary>
		/// <param name="userIds"></param>
		/// <returns></returns>
		public IDictionary<Guid, TResult> BulkGet<TResult>(IEnumerable<Guid> userIds) where TResult : UserObject, new()
		{
			Kit.NotNull(userIds, "userIds");

			Guid[] userIdArrayToQuery = userIds.ToArray();
			if (userIdArrayToQuery.Length == 0)
				return new Dictionary<Guid, TResult>();

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				var userDataList = (from user in ctx.Users
									where userIdArrayToQuery.Contains(user.UserId)
									select new { User = user, Membership = user.Membership }).ToList();

				Dictionary<Guid, TResult> returnValue = new Dictionary<Guid, TResult>();
				userDataList.ForEach(userData =>
				{
					TResult userObject = ConvertToUserObject<TResult>(userData.User, userData.Membership);

					TimeSpan span = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
					DateTime time = DateTime.UtcNow.Subtract(span);
					userObject.IsOnline = userObject.LastActivityDate.ToUniversalTime() > time;

					userObject.ParseExtensionPropertiesFrom(userData.User);
					returnValue.Add(userObject.UserId, userObject);
				});

				return returnValue;
			}
		}

		/// <summary>
		/// Get user by user name.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public UserObject Get(string userName)
		{
			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					var userAndMembership = ctx.Users.Where(user => user.UserName == userName && user.ApplicationId == this.authenticationContext.ApplicationId)
						.Select(user => new { User = user, Membership = user.Membership }).FirstOrDefault();

					if (userAndMembership == null) return null;

					return ConvertToUserObject<UserObject>(userAndMembership.User, userAndMembership.Membership);
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Get user element by user id.
		/// </summary>
		/// <param name="userId">user id</param>
		/// <returns></returns>
		public UserObject Get(Guid userId)
		{
			IDictionary<Guid, UserObject> userObjectDictionary = this.BulkGet<UserObject>(new[] { userId });
			if (!userObjectDictionary.ContainsKey(userId)) return null;

			return userObjectDictionary[userId];
		}

		/// <summary>
		/// Validate user is authenticated to application. Both invalid credential and unauthenticated organizations will be failed logon.
		/// </summary>
		/// <param name="username">user name</param>
		/// <param name="password">password</param>
		public LoginResults Login(string username, string password)
		{
			try
			{
				Guid applicationId = this.authenticationContext.ApplicationId;
                string inputPassword = String.Empty;

				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					Membership membership = ctx.Memberships.FirstOrDefault(m => m.ApplicationId == applicationId && m.User.UserName == username && m.IsApproved);
					if (membership == null)
						return LoginResults.InvalidCredential;

					// verify the password.
                    if (AspNetMembership.Provider.PasswordFormat == System.Web.Security.MembershipPasswordFormat.Clear)
                    {
                        inputPassword = password;
                    }
                    else
                    {
                        inputPassword = EncodePassword(password, membership.PasswordSalt, membership.PasswordFormat);
                    }
                        
                        if (!string.Equals(inputPassword, membership.Password, StringComparison.Ordinal))
					{
						membership.FailedPasswordAttemptCount++;
						membership.FailedPasswordAttemptWindowStart = DateTime.UtcNow;

						// lock the user if the continuous failed password attempts exceeds the configured times.
						if (membership.FailedPasswordAttemptCount > AspNetMembership.MaxInvalidPasswordAttempts)
							membership.IsLockedOut = true;

						ctx.SubmitChanges();
						return LoginResults.InvalidCredential;
					}

					// check whether the user is locked.
					if (membership.IsLockedOut && (DateTime.UtcNow - membership.FailedPasswordAttemptWindowStart).TotalMinutes < AspNetMembership.PasswordAttemptWindow)
						return LoginResults.LockedOut;

					// reset the failed password attempts if user/password is verified and the user is unlocked.
					membership.FailedPasswordAttemptCount = 0;
					membership.FailedPasswordAttemptWindowStart = new DateTime(1930, 1, 1);

					Organization organization = membership.User.Organization;
					if (organization.Status != OrganizationStatus.Enabled || organization.OrganizationType.DeleteStatus == DeleteStatus.Deleted)
					{
						// submit the database updates
						ctx.SubmitChanges();
						return LoginResults.InvalidOrganization;
					}

					membership.LastLoginDate = DateTime.UtcNow;
					membership.User.LastActivityDate = DateTime.UtcNow;
					ctx.SubmitChanges();

					return LoginResults.Successful;
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Logout the user by id.
		/// </summary>
		/// <param name="userId"></param>
		public void Logout(Guid userId)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				Guid applicationId = this.authenticationContext.ApplicationId;
				Membership membership = ctx.Memberships.FirstOrDefault(m => m.ApplicationId == applicationId && m.UserId == userId && m.IsApproved);
				if (membership != null)
				{
					membership.LastLockoutDate = DateTime.UtcNow;
					membership.User.LastActivityDate = DateTime.UtcNow;
					ctx.SubmitChanges();
				}
			}
		}

		/// <summary>
		/// Track the user activities online.
		/// </summary>
		/// <param name="userId"></param>
		public void Act(Guid userId)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				Guid applicationId = this.authenticationContext.ApplicationId;
				User user = ctx.Users.FirstOrDefault(u => u.ApplicationId == applicationId && u.UserId == userId && u.Membership.IsApproved);
				if (user != null)
				{
					user.LastActivityDate = DateTime.UtcNow;
					ctx.SubmitChanges();
				}
			}
		}

		/// <summary>
		/// Change password of specified user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="oldPassword"></param>
		/// <param name="newPassword"></param>
		/// <returns>returns true if operation successfully.</returns>
		public bool ChangePassword(Guid userId, string oldPassword, string newPassword)
		{
			Kit.NotNull(newPassword, "newPassword");

			// Min Required Password Length
			if (newPassword.Length < AspNetMembership.MinRequiredPasswordLength)
				throw new ValidationException(string.Format(Resources.PasswordLessThanMinLength, AspNetMembership.MinRequiredPasswordLength));

			// Password Strength Regular Expression
			if (!Kit.IsEmpty(AspNetMembership.PasswordStrengthRegularExpression))
			{
				Regex regex = new Regex(AspNetMembership.PasswordStrengthRegularExpression, RegexOptions.Compiled);
				if (!regex.IsMatch(newPassword))
					throw new ValidationException(Resources.InvalidPasswordFormat);
			}

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				Guid applicationId = this.authenticationContext.ApplicationId;
				Membership membership = ctx.Memberships.FirstOrDefault(m => m.ApplicationId == applicationId && m.UserId == userId);
				if (membership == null)
					throw new ArgumentException(Resources.InvalidUserID, "userId");

				string inputOldEncodedPassword = EncodePassword(oldPassword, membership.PasswordSalt, membership.PasswordFormat);
				if (!string.Equals(inputOldEncodedPassword, membership.Password, StringComparison.Ordinal))
					throw new ArgumentException(Resources.InvalidOldPasswordWhenChangePassword, "oldPassword");

				string inputNewEncodedPassword;
				int passwordFormat;
				ValidatePassword(membership.PasswordSalt, newPassword, out inputNewEncodedPassword, out passwordFormat);

				membership.Password = inputNewEncodedPassword;
				membership.PasswordFormat = passwordFormat;
				membership.LastPasswordChangedDate = DateTime.UtcNow;
				membership.User.LastActivityDate = DateTime.UtcNow;
				ctx.SubmitChanges();

				return true;
			}
		}

		/// <summary>
		/// Find user ids by predicates.
		/// </summary>
		/// <param name="predicate">linq predicate</param>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns>Returns enumerable user ids</returns>
		public IEnumerable<Guid> FindUsers(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount)
		{
			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
					var q = from user in ctx.Users where user.ApplicationId == authenticationContext.ApplicationId select user;

					if (predicate != null && !string.IsNullOrEmpty(predicate.Expression))
						q = q.Where(predicate.Expression, predicate.Parameters);

					if (!Kit.IsEmpty(orderby))
						q = q.OrderBy(orderby);

					recordCount = q.Count();
					return q.Skip(pageIndex * pageSize).Take(pageSize).Select(user => user.UserId).ToList();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		private static TResult ConvertToUserObject<TResult>(User user, Membership membership) where TResult : UserObject, new()
		{
			TResult userObject = new TResult
			{
				OrganizationId = user.OrganizationId,
				UserId = user.UserId,
				UserName = user.UserName,
				DisplayName = user.DisplayName,
				Email = membership.Email,
				ApplicationId = user.ApplicationId,
				PasswordQuestion = membership.PasswordQuestion,
				Comment = membership.Comment,
				IsApproved = membership.IsApproved,
				CreationDate = membership.CreateDate,
				IsLockedOut = membership.IsLockedOut,
				LastActivityDate = user.LastActivityDate,
				LastLockoutDate = membership.LastLockoutDate,
				LastLoginDate = membership.LastLoginDate,
				LastPasswordChangedDate = membership.LastPasswordChangedDate,
				MobilePin = membership.MobilePIN,
				LastUpdatedDate = user.LastUpdatedDate
			};

			userObject.ParseExtensionPropertiesFrom(user);

			TimeSpan span = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
			DateTime time = DateTime.UtcNow.Subtract(span);
			userObject.IsOnline = userObject.LastActivityDate.ToUniversalTime() > time;

			return userObject;
		}

		internal static string EncodePassword(string password, string passwordSalt, int passwordFormat)
		{
			Type MembershipProviderType = AspNetMembership.Provider.GetType();
			MethodInfo EncodePasswordMethodInfo = MembershipProviderType.GetMethod("EncodePassword", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(int), typeof(string) }, null);
			return EncodePasswordMethodInfo.Invoke(AspNetMembership.Provider, new object[] { password, passwordFormat, passwordSalt }) as string;
		}

		private Membership ConstructMembership(Guid userId)
		{
			Membership membership = new Membership();
			membership.ApplicationId = this.authenticationContext.ApplicationId;
			membership.UserId = userId;
			membership.CreateDate = DateTime.UtcNow;
			membership.FailedPasswordAnswerAttemptCount = 0;
			membership.FailedPasswordAnswerAttemptWindowStart = new DateTime(1930, 1, 1);
			membership.FailedPasswordAttemptCount = 0;
			membership.FailedPasswordAttemptWindowStart = new DateTime(1930, 1, 1);
			membership.IsLockedOut = false;
			membership.LastLockoutDate = new DateTime(1930, 1, 1);
			membership.LastLoginDate = new DateTime(1930, 1, 1);
			membership.LastPasswordChangedDate = new DateTime(1930, 1, 1);

			return membership;
		}

		private void ValidateUserData(MembershipDataContext ctx, UserObject userObject)
		{
			using (ValidationScope validationScope = new ValidationScope())
			{
				// check user name
				if (Kit.IsEmpty(userObject.UserName) || userObject.UserName.Length > 256)
					validationScope.Error(Resources.InvalidUserName);

				if (!Kit.IsEmpty(userObject.UserName) && ctx.Users.Where(user => user.UserName == userObject.UserName && user.ApplicationId == this.authenticationContext.ApplicationId && user.UserId != userObject.UserId).Count() > 0)
					validationScope.Error(Resources.ExistedUserName);

				// check display name
				if (Kit.IsEmpty(userObject.DisplayName) || userObject.DisplayName.Length > 256)
					validationScope.Error(Resources.InvalidDisplayName);

				if (!Kit.IsEmpty(userObject.DisplayName) && ctx.Users.Where(user => user.DisplayName == userObject.DisplayName && user.ApplicationId == this.authenticationContext.ApplicationId && user.UserId != userObject.UserId).Count() > 0)
					validationScope.Error(Resources.ExistedDisplayName);

				// check email
				if (Kit.IsEmpty(userObject.Email) && AspNetMembership.Provider.RequiresUniqueEmail)
					validationScope.Error(Resources.UserEmailCannotBeEmpty);

				if (!Kit.IsEmpty(userObject.Email) && userObject.Email.Length > 256)
					validationScope.Error(Resources.UserEmailCannotBeGreaterThan256Characters);

				if (!Kit.IsEmpty(userObject.Email) && AspNetMembership.Provider.RequiresUniqueEmail && ctx.Users.Where(user => user.Membership.LoweredEmail == userObject.Email.ToLowerInvariant() && user.ApplicationId == this.authenticationContext.ApplicationId && user.UserId != userObject.UserId).Count() > 0)
					validationScope.Error(Resources.ExistedUserEmail);
			}
		}

		private void ValidatePasswordQuestionAndAnswer(string passwordSalt, string passwordQuestion, string passwordAnswer, out string encodedPasswordAnswer)
		{
			encodedPasswordAnswer = null;

			using (ValidationScope validationScope = new ValidationScope())
			{
				// check password question
				if (Kit.IsEmpty(passwordQuestion) && AspNetMembership.Provider.RequiresQuestionAndAnswer)
					validationScope.Error(Resources.PasswordQuestionCannotBeEmpty);

				if (!Kit.IsEmpty(passwordQuestion) && passwordQuestion.Length > 256)
					validationScope.Error(Resources.PasswordQuestionInvalidLength);

				// check password answer
				if (Kit.IsEmpty(passwordAnswer) && AspNetMembership.Provider.RequiresQuestionAndAnswer)
					validationScope.Error(Resources.PasswordAnswerCannotBeEmpty);

				if (!string.IsNullOrEmpty(passwordAnswer))
				{
					if (passwordAnswer.Length > 128)
						validationScope.Error(Resources.PasswordAnswerInvalidLength);

					encodedPasswordAnswer = EncodePassword(passwordAnswer, passwordSalt, (int)AspNetMembership.Provider.PasswordFormat);
					if (encodedPasswordAnswer.Length > 128)
						validationScope.Error(Resources.PasswordAnswerTooLong);
				}
			}
		}

		private void ValidatePassword(string passwordSalt, string password, out string encodedPassword, out int passwordFormat)
		{
			encodedPassword = null;
			passwordFormat = (int)AspNetMembership.Provider.PasswordFormat;

			using (ValidationScope validationScope = new ValidationScope())
			{
				// check password
				if (Kit.IsEmpty(password) || password.Length > 128)
					validationScope.Error(Resources.PasswordInvalid);

				if (password.Length < AspNetMembership.Provider.MinRequiredPasswordLength)
					validationScope.Error(Resources.PasswordLessThanMinLength, AspNetMembership.Provider.MinRequiredPasswordLength);

				int nonAlphanumericCharactersNumber = 0;
				for (int i = 0; i < password.Length; i++)
				{
					if (!char.IsLetterOrDigit(password, i))
						nonAlphanumericCharactersNumber++;
				}

				if (nonAlphanumericCharactersNumber < AspNetMembership.Provider.MinRequiredNonAlphanumericCharacters)
					validationScope.Error(Resources.PasswordLessThanMinRequiredNonAlphanumericCharacters, AspNetMembership.Provider.MinRequiredNonAlphanumericCharacters);

				if ((AspNetMembership.Provider.PasswordStrengthRegularExpression.Length > 0) && !Regex.IsMatch(password, AspNetMembership.Provider.PasswordStrengthRegularExpression))
					validationScope.Error(Resources.InvalidPasswordFormat);

				encodedPassword = EncodePassword(password, passwordSalt, passwordFormat);
				if (encodedPassword.Length > 128)
					validationScope.Error(Resources.PasswordTooLong);
			}
		}

		private static string GeneratePasswordSalt()
		{
			byte[] data = new byte[16];
			RNGCryptoServiceProvider.GetBytes(data);
			return Convert.ToBase64String(data);
		}
	}
}