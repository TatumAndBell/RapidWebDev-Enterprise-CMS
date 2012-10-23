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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Platform.Properties;
using AspNetMembership = System.Web.Security.Membership;
using RapidWebDev.ExtensionModel.Linq;
using Rhino.Mocks;
using System.Globalization;

namespace RapidWebDev.Platform.Initialization
{
	/// <summary>
	/// Installer for default credential of current application.
	/// </summary>
	public class CredentialInstaller : IInstaller
	{
		private IApplicationApi applicationApi;
		private IPlatformConfiguration platformConfiguration;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="applicationApi"></param>
		/// <param name="platformConfiguration"></param>
		public CredentialInstaller(IApplicationApi applicationApi, IPlatformConfiguration platformConfiguration)
		{
			this.applicationApi = applicationApi;
			this.platformConfiguration = platformConfiguration;
		}

		/// <summary>
		/// Install default credential of current application.
		/// Do nothing if the default credential have existed.
		/// </summary>
		/// <param name="applicationName">application name to install</param>
		/// <returns>returns true when the installing successfully.</returns>
		public virtual void Install(string applicationName)
		{
			ApplicationObject applicationObject = this.applicationApi.Get(applicationName);
			Guid applicationId = applicationObject.Id;

			this.SetupMetadataTypeByDomain(applicationId);

			// check whether the initial default user has existed or not.
			if (!this.NeedToSetupEnvironment(applicationId))
			{
				this.ResolvePlatformConfiguration(applicationId);
				return;
			}

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				// create default organization type
				CreateOrganizationType(applicationId, this.platformConfiguration.OrganizationType);

				// create default user
				SaveUser(applicationId, this.platformConfiguration.User, this.platformConfiguration.Password, this.platformConfiguration.PasswordAnswer);

				// create default organization
				this.platformConfiguration.Organization.OrganizationTypeId = this.platformConfiguration.OrganizationType.OrganizationTypeId;
				CreateOrganization(applicationId, this.platformConfiguration.Organization, this.platformConfiguration.User.UserId);

				// create anonymous user
				UserObject anonymousUser = new UserObject
				{
					OrganizationId = this.platformConfiguration.Organization.OrganizationId,
					UserName = PermissionConst.ANONYMOUS,
					Comment = "",
					DisplayName = Resources.AnonymousUser,
					IsApproved = false
				};

				SaveUser(applicationId, anonymousUser, this.platformConfiguration.Password, null);

				// create default role
				CreateRole(applicationId, this.platformConfiguration.Role);
				SetUserToRoles(this.platformConfiguration.User.UserId, new[] { this.platformConfiguration.Role.RoleId });

				// update organization of default user to default organization
				User defaultUserEntity = ctx.Users.FirstOrDefault(u => u.UserId == this.platformConfiguration.User.UserId);
				defaultUserEntity.OrganizationId = this.platformConfiguration.Organization.OrganizationId;
				ctx.SubmitChanges();
			}
		}

		/// <summary>
		/// Uninstall application
		/// </summary>
		/// <param name="applicationName">application name to uninstall</param>
		/// <returns>returns true when the uninstalling successfully.</returns>
		public virtual void Uninstall(string applicationName)
		{
		}

		/// <summary>
		/// Returns true indicates the installer needs to setup environment.
		/// </summary>
		/// <param name="applicationId"></param>
		/// <returns></returns>
		protected virtual bool NeedToSetupEnvironment(Guid applicationId)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				return ctx.Users.Where(user => user.UserName == this.platformConfiguration.User.UserName && user.ApplicationId == applicationId).Count() == 0;
			}
		}

		/// <summary>
		/// Setup metadata for users and organizations by configured domains.
		/// </summary>
		/// <param name="applicationId"></param>
		protected virtual void SetupMetadataTypeByDomain(Guid applicationId)
		{
			MockRepository mockRepository = new MockRepository();
			IApplicationContext applicationContext = mockRepository.Stub<IApplicationContext>();
			SetupResult.On(applicationContext).Call(applicationContext.ApplicationId).Return(applicationId);
			mockRepository.ReplayAll();

			DLinqMetadataApi metadataApi = new DLinqMetadataApi(applicationContext);
			foreach (OrganizationDomain domain in this.platformConfiguration.Domains)
			{
				string organizationExtensionDataTypeName = domain.Value;
				string userExtensionDataTypeName = string.Format(CultureInfo.InvariantCulture, "{0}User", domain.Value);

				if (metadataApi.GetType(organizationExtensionDataTypeName) == null)
					metadataApi.AddType(organizationExtensionDataTypeName, "Platform", "", ObjectMetadataTypes.Custom, false, null);

				if (metadataApi.GetType(userExtensionDataTypeName) == null)
					metadataApi.AddType(userExtensionDataTypeName, "Platform", "", ObjectMetadataTypes.Custom, false, null);
			}

			mockRepository.VerifyAll();
		}

		/// <summary>
		/// Resolve platform configuration.
		/// </summary>
		/// <param name="applicationId"></param>
		protected virtual void ResolvePlatformConfiguration(Guid applicationId)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				User user = ctx.Users.FirstOrDefault(u => u.UserName == this.platformConfiguration.User.UserName && u.ApplicationId == applicationId);
				if (user != null)
				{
					this.platformConfiguration.User.UserId = user.UserId;
					this.platformConfiguration.User.OrganizationId = user.OrganizationId;
				}

				Organization organization = ctx.Organizations.FirstOrDefault(org => org.OrganizationCode == this.platformConfiguration.Organization.OrganizationCode && org.ApplicationId == applicationId);
				if (organization != null)
				{
					this.platformConfiguration.Organization.OrganizationId = organization.OrganizationId;
					this.platformConfiguration.Organization.OrganizationTypeId = organization.OrganizationTypeId;
				}

				Role role = ctx.Roles.FirstOrDefault(r => r.RoleName == this.platformConfiguration.Role.RoleName && r.ApplicationId == applicationId);
				if (role != null)
				{
					this.platformConfiguration.Role.RoleId = role.RoleId;
				}

				OrganizationType organizationType = ctx.OrganizationTypes.FirstOrDefault(org => org.Name == this.platformConfiguration.OrganizationType.Name && org.ApplicationId == applicationId);
				if (organizationType != null) this.platformConfiguration.OrganizationType.OrganizationTypeId = organizationType.OrganizationTypeId;
			}
		}

		/// <summary>
		/// Create organization type into specified application.
		/// </summary>
		/// <param name="applicationId"></param>
		/// <param name="orgType"></param>
		protected static void CreateOrganizationType(Guid applicationId, OrganizationTypeObject orgType)
		{
			Kit.NotNull(orgType, "orgType");

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				OrganizationType organizationType = new OrganizationType
				{
					ApplicationId = applicationId,
					Domain = orgType.Domain,
					Name = orgType.Name,
					Description = orgType.Description,
					Predefined = true,
					LastUpdatedDate = DateTime.UtcNow
				};

				ctx.OrganizationTypes.InsertOnSubmit(organizationType);
				ctx.SubmitChanges();

				orgType.OrganizationTypeId = organizationType.OrganizationTypeId;
			}
		}

		/// <summary>
		/// Get organization instance by name.
		/// </summary>
		/// <param name="applicationId">application id</param>
		/// <param name="organizationTypeName">organization type name</param>
		/// <returns></returns>
		protected static OrganizationTypeObject GetOrganizationType(Guid applicationId, string organizationTypeName)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				var organizationTypeObject = (from orgType in ctx.OrganizationTypes
											  where orgType.Name == organizationTypeName && orgType.ApplicationId == applicationId
											  select new OrganizationTypeObject()
											  {
												  OrganizationTypeId = orgType.OrganizationTypeId,
												  Name = orgType.Name,
												  Domain = orgType.Domain,
												  Description = orgType.Description,
												  Predefined = orgType.Predefined,
												  LastUpdatedDate = orgType.LastUpdatedDate
											  }).FirstOrDefault();

				return organizationTypeObject;
			}
		}

		/// <summary>
		/// Create a role into specified application.
		/// </summary>
		/// <param name="applicationId"></param>
		/// <param name="roleObject"></param>
		protected static void CreateRole(Guid applicationId, RoleObject roleObject)
		{
			Kit.NotNull(roleObject, "roleObject");

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					Role role = new Role()
					{
						ApplicationId = applicationId,
						RoleName = roleObject.RoleName,
						LoweredRoleName = roleObject.RoleName.ToLowerInvariant(),
						Domain = roleObject.Domain,
						Description = roleObject.Description,
						LastUpdatedDate = DateTime.UtcNow,
						Predefined = true
					};

					ctx.Roles.InsertOnSubmit(role);
					ctx.SubmitChanges();

					roleObject.RoleId = role.RoleId;
				}
			}
			catch (ValidationException)
			{
				throw;
			}
			catch (Exception exp)
			{
				Logger.Instance(typeof(CredentialInstaller)).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Set the user into the roles.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleIds"></param>
		protected static void SetUserToRoles(Guid userId, IEnumerable<Guid> roleIds)
		{
			using (TransactionScope ts = new TransactionScope())
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				ctx.UsersInRoles.Delete(uir => uir.UserId == userId);
				foreach (Guid roleId in roleIds)
					ctx.UsersInRoles.InsertOnSubmit(new UsersInRole { UserId = userId, RoleId = roleId });

				ctx.SubmitChanges();
				ts.Complete();
			}
		}

		/// <summary>
		/// Set permissions onto the user.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="permissions"></param>
		protected static void SetUserPermissions(Guid userId, IEnumerable<string> permissions)
		{
			Kit.NotNull(permissions, "permissions");

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					ctx.Permissions.Delete(p => p.UserId == userId);
					foreach (string permission in permissions.Distinct())
						ctx.Permissions.InsertOnSubmit(new Permission() { UserId = userId, PermissionKey = permission });

					ctx.SubmitChanges();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(typeof(CredentialInstaller)).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Set permissions onto the role.
		/// </summary>
		/// <param name="roleId"></param>
		/// <param name="permissions"></param>
		protected static void SetRolePermissions(Guid roleId, IEnumerable<string> permissions)
		{
			Kit.NotNull(permissions, "permissions");

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					ctx.Permissions.Delete(p => p.RoleId == roleId);
					foreach (string permission in permissions.Distinct())
						ctx.Permissions.InsertOnSubmit(new Permission() { RoleId = roleId, PermissionKey = permission });

					ctx.SubmitChanges();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(typeof(CredentialInstaller)).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Create an organization into specified application.
		/// </summary>
		/// <param name="applicationId"></param>
		/// <param name="organizationObject"></param>
		/// <param name="operatedUserId"></param>
		protected static void CreateOrganization(Guid applicationId, OrganizationObject organizationObject, Guid operatedUserId)
		{
			Kit.NotNull(organizationObject, "organizationObject");

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					Organization organization = ExtensionObjectFactory.Create<Organization>();
					organization.ApplicationId = applicationId;
					organization.OrganizationCode = organizationObject.OrganizationCode;
					organization.OrganizationName = organizationObject.OrganizationName;
					organization.OrganizationTypeId = organizationObject.OrganizationTypeId;
					organization.Status = organizationObject.Status;
					organization.Description = organizationObject.Description;
					organization.CreatedDate = DateTime.UtcNow;
					organization.CreatedBy = operatedUserId;
					organization.LastUpdatedDate = DateTime.UtcNow;
					organization.LastUpdatedBy = operatedUserId;
					organization.ParseExtensionPropertiesFrom(organizationObject);

					ctx.Organizations.InsertOnSubmit(organization);
					ctx.SubmitChanges();

					organizationObject.OrganizationId = organization.OrganizationId;
				}
			}
			catch (ValidationException)
			{
				throw;
			}
			catch (Exception exp)
			{
				Logger.Instance(typeof(CredentialInstaller)).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Save user object.
		/// </summary>
		/// <param name="applicationId">Application id</param>
		/// <param name="userObject">user business object</param>
		/// <param name="password">login password</param>
		/// <param name="passwordAnswer">password retrieve answer</param>
		/// <exception cref="ValidationException">Save user failed by various reasons implied in exception message.</exception>
		/// <exception cref="ArgumentException">The property userObject.Id is specified with an invalid value.</exception>
		protected static void SaveUser(Guid applicationId, UserObject userObject, string password, string passwordAnswer)
		{
			Kit.NotNull(userObject, "userObject");

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					string passwordSalt;
					string encodedPassword;
					string encodedPasswordAnswer;

					ValidateUserData(ctx, applicationId, userObject);

					User userEntity = null;
					Linq.Membership membership = null;
					if (userObject.UserId == Guid.Empty)
					{
						Kit.NotNull(password, "password");
						passwordSalt = GeneratePasswordSalt();
						ValidatePasswordQuestionAndAnswer(passwordSalt, userObject.PasswordQuestion, passwordAnswer, out encodedPasswordAnswer);
						ValidatePassword(passwordSalt, password, out encodedPassword);

						userEntity = ExtensionObjectFactory.Create<User>();
						userEntity.ApplicationId = applicationId;
						userEntity.UserId = Guid.NewGuid();
						userEntity.LastActivityDate = new DateTime(1930, 1, 1);
						ctx.Users.InsertOnSubmit(userEntity);

						membership = ConstructMembership(applicationId, userEntity.UserId);
						membership.PasswordSalt = passwordSalt;
						membership.Password = encodedPassword;
						membership.PasswordFormat = (int)AspNetMembership.Provider.PasswordFormat;
						membership.PasswordQuestion = userObject.PasswordQuestion;
						membership.PasswordAnswer = encodedPasswordAnswer;
						ctx.Memberships.InsertOnSubmit(membership);

						userObject.UserId = userEntity.UserId;
					}
					else
					{
						userEntity = ctx.Users.FirstOrDefault(user => user.UserId == userObject.UserId);
						if (userEntity == null)
							throw new ArgumentException(string.Format(Resources.InvalidUserID, userObject.UserId));

						membership = ctx.Memberships.FirstOrDefault(m => m.UserId == userObject.UserId);
						if (membership == null)
							throw new ArgumentException(string.Format(Resources.InvalidUserID, userObject.UserId));

						passwordSalt = membership.PasswordSalt;
						if (!Kit.IsEmpty(passwordAnswer))
						{
							ValidatePasswordQuestionAndAnswer(passwordSalt, userObject.PasswordQuestion, passwordAnswer, out encodedPasswordAnswer);
							membership.PasswordQuestion = userObject.PasswordQuestion;
							membership.PasswordAnswer = encodedPasswordAnswer;
						}

						if (!Kit.IsEmpty(password))
						{
							ValidatePassword(passwordSalt, password, out encodedPassword);
							membership.Password = encodedPassword;
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
				}
			}
			catch (ValidationException)
			{
				throw;
			}
			catch (Exception exp)
			{
				Logger.Instance(typeof(CredentialInstaller)).Error(exp);
				throw;
			}
		}

		private static void ValidateUserData(MembershipDataContext ctx, Guid applicationId, UserObject userObject)
		{
			// check user name
			if (Kit.IsEmpty(userObject.UserName) || userObject.UserName.Length > 256)
				throw new ValidationException(Resources.InvalidUserName);

			if (ctx.Users.Where(user => user.UserName == userObject.UserName && user.ApplicationId == applicationId && user.UserId != userObject.UserId).Count() > 0)
				throw new ValidationException(Resources.ExistedUserName);

			// check display name
			if (Kit.IsEmpty(userObject.DisplayName) || userObject.DisplayName.Length > 256)
				throw new ValidationException(Resources.InvalidDisplayName);

			if (ctx.Users.Where(user => user.DisplayName == userObject.DisplayName && user.ApplicationId == applicationId && user.UserId != userObject.UserId).Count() > 0)
				throw new ValidationException(Resources.ExistedDisplayName);

			// check email
			if (Kit.IsEmpty(userObject.Email) && AspNetMembership.Provider.RequiresUniqueEmail)
				throw new ValidationException(Resources.UserEmailCannotBeEmpty);

			if (!Kit.IsEmpty(userObject.Email) && userObject.Email.Length > 256)
				throw new ValidationException(Resources.InvalidUserEmail);

			if (AspNetMembership.Provider.RequiresUniqueEmail && ctx.Users.Where(user => user.Membership.LoweredEmail == userObject.Email.ToLowerInvariant() && user.ApplicationId == applicationId && user.UserId != userObject.UserId).Count() > 0)
				throw new ValidationException(Resources.ExistedUserEmail);
		}

		private static void ValidatePasswordQuestionAndAnswer(string passwordSalt, string passwordQuestion, string passwordAnswer, out string encodedPasswordAnswer)
		{
			encodedPasswordAnswer = null;

			// check password question
			if (Kit.IsEmpty(passwordQuestion) && AspNetMembership.Provider.RequiresQuestionAndAnswer)
				throw new ValidationException(Resources.PasswordQuestionCannotBeEmpty);

			if (!Kit.IsEmpty(passwordQuestion) && passwordQuestion.Length > 256)
				throw new ValidationException(Resources.PasswordQuestionInvalidLength);

			// check password answer
			if (Kit.IsEmpty(passwordAnswer) && AspNetMembership.Provider.RequiresQuestionAndAnswer)
				throw new ValidationException(Resources.PasswordAnswerCannotBeEmpty);

			if (!string.IsNullOrEmpty(passwordAnswer))
			{
				if (passwordAnswer.Length > 128)
					throw new ValidationException(Resources.PasswordAnswerInvalidLength);

				encodedPasswordAnswer = EncodePassword(passwordAnswer, passwordSalt);
				if (encodedPasswordAnswer.Length > 128)
					throw new ValidationException(Resources.PasswordAnswerTooLong);
			}
		}

		private static void ValidatePassword(string passwordSalt, string password, out string encodedPassword)
		{
			encodedPassword = null;

			// check password
			if (Kit.IsEmpty(password) || password.Length > 128)
				throw new ValidationException(Resources.PasswordInvalid);

			if (password.Length < AspNetMembership.Provider.MinRequiredPasswordLength)
				throw new ValidationException(string.Format(Resources.PasswordLessThanMinLength, AspNetMembership.Provider.MinRequiredPasswordLength));

			int nonAlphanumericCharactersNumber = 0;
			for (int i = 0; i < password.Length; i++)
			{
				if (!char.IsLetterOrDigit(password, i))
					nonAlphanumericCharactersNumber++;
			}

			if (nonAlphanumericCharactersNumber < AspNetMembership.Provider.MinRequiredNonAlphanumericCharacters)
				throw new ValidationException(string.Format(Resources.PasswordLessThanMinRequiredNonAlphanumericCharacters, AspNetMembership.Provider.MinRequiredNonAlphanumericCharacters));

			if ((AspNetMembership.Provider.PasswordStrengthRegularExpression.Length > 0) && !Regex.IsMatch(password, AspNetMembership.Provider.PasswordStrengthRegularExpression))
				throw new ValidationException(Resources.InvalidPasswordFormat);

            if (AspNetMembership.Provider.PasswordFormat == System.Web.Security.MembershipPasswordFormat.Clear)
            {
                encodedPassword = password;
            }
            else
            {

                encodedPassword = EncodePassword(password, passwordSalt);
            }

			if (encodedPassword.Length > 128)
				throw new ValidationException(Resources.PasswordTooLong);
		}

		private static string EncodePassword(string password, string passwordSalt)
		{
			Type MembershipProviderType = AspNetMembership.Provider.GetType();
			MethodInfo EncodePasswordMethodInfo = MembershipProviderType.GetMethod("EncodePassword", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(int), typeof(string) }, null);
			return EncodePasswordMethodInfo.Invoke(AspNetMembership.Provider, new object[] { password, (int)AspNetMembership.Provider.PasswordFormat, passwordSalt }) as string;
		}

		private static string GeneratePasswordSalt()
		{
			byte[] data = new byte[16];
			new RNGCryptoServiceProvider().GetBytes(data);
			return Convert.ToBase64String(data);
		}

		private static Linq.Membership ConstructMembership(Guid applicationId, Guid userId)
		{
			Linq.Membership membership = new Linq.Membership();
			membership.ApplicationId = applicationId;
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
	}
}
