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
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Validation;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.Platform.Linq
{
	/// <summary>
	/// RapidWebDev Membership gives you a built-in way to validate and store user credentials which developed on ASP.NET Membership.
	/// The membership therefore helps you manage user authentication in your Web sites. 
	/// With extended from ASP.NET membership, you can configure the membership by ASP.NET Membreship Configuration as seeing the topic "Configuring an ASP.NET Application to Use Membership" in MSDN.
	/// The RapidWebDev membership model is NOT compatible to ASP.NET Membership Controls like Login Control, PasswordRecovery Control.
	/// </summary>
	public class MembershipApi : CachableApi, IMembershipApi
	{
		private IAuthenticationContext authenticationContext;
		private IOrganizationApi organizationApi;
		private MembershipDbProvider membershipDbProvider;

		/// <summary>
		/// Construct membership API instance.
		/// </summary>
		/// <param name="authenticationContext"></param>
		/// <param name="organizationApi"></param>
		public MembershipApi(IAuthenticationContext authenticationContext, IOrganizationApi organizationApi)
			: base(authenticationContext)
		{
			this.authenticationContext = authenticationContext;
			this.organizationApi = organizationApi;
			this.membershipDbProvider = new MembershipDbProvider(authenticationContext, organizationApi);
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

			try
			{
				this.membershipDbProvider.Save(userObject, password, passwordAnswer);
				base.RemoveCache(userObject.UserId);
			}
			catch (ArgumentException)
			{
				throw;
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
		public IDictionary<Guid, UserObject> BulkGet(IEnumerable<Guid> userIds)
		{
			try
			{
				CloneableDictionary<Guid, UserObject> returnedValue = new CloneableDictionary<Guid, UserObject>();
				List<Guid> userIdList = userIds.ToList();
				foreach (Guid userId in userIdList)
				{
					UserObject userObject = base.GetCacheObject<UserObject>(userId);
					returnedValue.Add(userId, userObject);
				}

				List<Guid> userIdsNeededToLoadFromDb = returnedValue.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).ToList();
				IDictionary<Guid, UserObject> usersLoadedFromDb = this.membershipDbProvider.BulkGet<UserObject>(userIdsNeededToLoadFromDb);
				foreach (Guid userIdNeededToLoadFromDb in userIdsNeededToLoadFromDb)
				{
					if (usersLoadedFromDb.ContainsKey(userIdNeededToLoadFromDb) && usersLoadedFromDb[userIdNeededToLoadFromDb] != null)
					{
						UserObject userObjectLoadedFromDb = usersLoadedFromDb[userIdNeededToLoadFromDb];
						returnedValue[userIdNeededToLoadFromDb] = userObjectLoadedFromDb;

						base.AddCache(userObjectLoadedFromDb.UserId, userObjectLoadedFromDb);
					}
				}

				returnedValue = returnedValue.Clone();
				foreach (UserObject userObject in returnedValue.Values)
				{
					userObject.CreationDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.CreationDate);
					userObject.LastActivityDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastActivityDate);
					userObject.LastLockoutDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastLockoutDate);
					userObject.LastLoginDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastLoginDate);
					userObject.LastPasswordChangedDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastPasswordChangedDate);
					userObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastUpdatedDate);
				}

				return returnedValue;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Resolve user objects from enumerable user ids.
		/// </summary>
		/// <param name="userIds"></param>
		/// <returns></returns>
		public IDictionary<Guid, TResult> BulkGet<TResult>(IEnumerable<Guid> userIds) where TResult : UserObject, new()
		{
			try
			{
				CloneableDictionary<Guid, TResult> returnedValue = new CloneableDictionary<Guid, TResult>();
				List<Guid> userIdList = userIds.ToList();
				foreach (Guid userId in userIdList)
				{
					TResult userObject = base.GetCacheObject<TResult>(userId);
					returnedValue.Add(userId, userObject);
				}

				List<Guid> userIdsNeededToLoadFromDb = returnedValue.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).ToList();
				IDictionary<Guid, TResult> usersLoadedFromDb = this.membershipDbProvider.BulkGet<TResult>(userIdsNeededToLoadFromDb);
				foreach (Guid userIdNeededToLoadFromDb in userIdsNeededToLoadFromDb)
				{
					if (usersLoadedFromDb.ContainsKey(userIdNeededToLoadFromDb) && usersLoadedFromDb[userIdNeededToLoadFromDb] != null)
					{
						TResult userObjectLoadedFromDb = usersLoadedFromDb[userIdNeededToLoadFromDb];
						returnedValue[userIdNeededToLoadFromDb] = userObjectLoadedFromDb;

						base.AddCache(userObjectLoadedFromDb.UserId, userObjectLoadedFromDb);
					}
				}

				returnedValue = returnedValue.Clone();
				foreach (UserObject userObject in returnedValue.Values)
				{
					userObject.CreationDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.CreationDate);
					userObject.LastActivityDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastActivityDate);
					userObject.LastLockoutDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastLockoutDate);
					userObject.LastLoginDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastLoginDate);
					userObject.LastPasswordChangedDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastPasswordChangedDate);
					userObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastUpdatedDate);
				}

				return returnedValue;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Get user by user name.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public UserObject Get(string userName)
		{
			return this.membershipDbProvider.Get(userName);
		}

		/// <summary>
		/// Get user object by user id.
		/// </summary>
		/// <param name="userId">user id</param>
		/// <returns></returns>
		public UserObject Get(Guid userId)
		{
			UserObject userObject = base.GetCacheObject<UserObject>(userId);
			if (userObject == null)
			{
				userObject = this.membershipDbProvider.Get(userId);
				if (userObject == null) return null;

				base.AddCache(userObject.UserId, userObject);
			}

			// convert timezone on the copy so that it doesn't impact the cached object
			userObject = userObject.Clone();
			userObject.CreationDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.CreationDate);
			userObject.LastActivityDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastActivityDate);
			userObject.LastLockoutDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastLockoutDate);
			userObject.LastLoginDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastLoginDate);
			userObject.LastPasswordChangedDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastPasswordChangedDate);
			userObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(userObject.LastUpdatedDate);

			return userObject;
		}

		/// <summary>
		/// Validate user is authenticated to application. Both invalid credential and unauthenticated organizations will be failed logon.
		/// </summary>
		/// <param name="username">user name</param>
		/// <param name="password">password</param>
		public LoginResults Login(string username, string password)
		{
			return this.membershipDbProvider.Login(username, password);
		}

		/// <summary>
		/// Logout the user by id.
		/// </summary>
		/// <param name="userId"></param>
		public void Logout(Guid userId)
		{
			this.membershipDbProvider.Logout(userId);
			base.RemoveCache(userId);
		}

		/// <summary>
		/// Track the user activities online.
		/// </summary>
		/// <param name="userId"></param>
		public void Act(Guid userId)
		{
			// don't remove the user from cache in this block of code because Act is called frequently. 
			UserObject userObject = base.GetCacheObject<UserObject>(userId);
			if (userObject != null)
			{
				userObject.LastActivityDate = DateTime.UtcNow;
				base.AddCache(userId, userObject);
			}

			this.membershipDbProvider.Act(userId);
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
			Kit.NotNull(oldPassword, "oldPassword");
			Kit.NotNull(newPassword, "newPassword");

			base.RemoveCache(userId);
			return this.membershipDbProvider.ChangePassword(userId, oldPassword, newPassword);
		}

		/// <summary>
		/// Find user business objects by custom predicates.
		/// </summary>
		/// <param name="predicate">linq predicate. see user properties for predicate at <see cref="RapidWebDev.Platform.Linq.User"/>.</param>
		/// <param name="orderby">sorting field and direction</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns>Returns enumerable user objects</returns>
		public IEnumerable<UserObject> FindUsers(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount)
		{
			IEnumerable<Guid> userIds = this.membershipDbProvider.FindUsers(predicate, orderby, pageIndex, pageSize, out recordCount);
			IDictionary<Guid, UserObject> usersById = this.BulkGet(userIds);

			List<UserObject> results = new List<UserObject>();
			foreach (Guid userId in userIds)
				if (usersById.ContainsKey(userId) && usersById[userId] != null)
					results.Add(usersById[userId]);

			return results;
		}
	}
}