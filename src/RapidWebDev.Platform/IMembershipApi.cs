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
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Security.Principal;
using System.Linq.Expressions;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// RapidWebDev Membership gives you a built-in way to validate and store user credentials which developed on ASP.NET Membership.
	/// The membership therefore helps you manage user authentication in your Web sites. 
	/// With extended from ASP.NET membership, you can configure the membership by ASP.NET Membreship Configuration as seeing the topic "Configuring an ASP.NET Application to Use Membership" in MSDN.
	/// The RapidWebDev membership model is NOT compatible to ASP.NET Membership Controls like Login Control, PasswordRecovery Control.
	/// </summary>
	public interface IMembershipApi
	{
		/// <summary>
		/// Save user object with specified password and password retrieval answer. 
		/// If it's used to update an existed user, the password allows to be null in the case that it doesn't need to change the password of the user.
		/// The argument "passwordAnswer" is to be validated depends on the configuration on ASP.NET Membership "RequiresQuestionAndAnswer".
		/// </summary>
		/// <param name="userObject">user business object</param>
		/// <param name="password">login password. If it's used to update an existed user, the password allows to be null in the case that it doesn't need to change the password of the user.</param>
		/// <param name="passwordAnswer">password retrieve answer. The argument "passwordAnswer" is to be validated depends on the configuration on ASP.NET Membership "RequiresQuestionAndAnswer".</param>
		/// <exception cref="RapidWebDev.Common.Validation.ValidationException">Save user failed by various reasons implied in exception message.</exception>
		/// <exception cref="ArgumentException">The property userObject.Id is specified with an invalid value.</exception>
		/// <exception cref="ArgumentNullException">The argument userObject is null.</exception>
		void Save(UserObject userObject, string password, string passwordAnswer);

		/// <summary>
		/// Resolve user objects from enumerable user ids.
		/// </summary>
		/// <param name="userIds"></param>
		/// <returns></returns>
		IDictionary<Guid, UserObject> BulkGet(IEnumerable<Guid> userIds);

		/// <summary>
		/// Resolve user objects from enumerable user ids.
		/// </summary>
		/// <param name="userIds"></param>
		/// <returns></returns>
		IDictionary<Guid, TResult> BulkGet<TResult>(IEnumerable<Guid> userIds) where TResult : UserObject, new();

		/// <summary>
		/// Get user by user name.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		UserObject Get(string userName);

		/// <summary>
		/// Get user object by user id.
		/// </summary>
		/// <param name="userId">user id</param>
		/// <returns></returns>
		UserObject Get(Guid userId);

		/// <summary>
		/// Validate user is authenticated to application. 
		/// Either invalid credential or unauthenticated company will lead to login failed.
		/// </summary>
		/// <param name="username">user name</param>
		/// <param name="password">password</param>
		LoginResults Login(string username, string password);

		/// <summary>
		/// Logout the user by id.
		/// </summary>
		/// <param name="userId"></param>
		void Logout(Guid userId);

		/// <summary>
		/// Track the user activities online.
		/// </summary>
		/// <param name="userId"></param>
		void Act(Guid userId);

		/// <summary>
		/// Change password of specified user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="oldPassword"></param>
		/// <param name="newPassword"></param>
		/// <returns>returns true if operation successfully.</returns>
		bool ChangePassword(Guid userId, string oldPassword, string newPassword);

		/// <summary>
		/// Find user business objects by custom predicates.
		/// </summary>
		/// <param name="predicate">linq predicate. see user properties for predicate at <see cref="RapidWebDev.Platform.Linq.User"/>.</param>
		/// <param name="orderby">sorting field and direction</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns>Returns enumerable user objects</returns>
		IEnumerable<UserObject> FindUsers(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount);
	}
}