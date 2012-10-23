/***************************************************************************
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
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;

namespace RapidWebDev.Tests
{
	/// <summary>
	/// 模拟的验证关联类
	/// </summary>
	public class MockedAuthenticationContext : IAuthenticationContext
	{
		private Guid applicationId = Guid.Empty;
		private IIdentity identity = new GenericIdentity("admin");
		private UserObject userObject;
		private Dictionary<int, Hashtable> sessionsByThread = new Dictionary<int, Hashtable>();
		private Dictionary<int, Hashtable> variablesByThread = new Dictionary<int, Hashtable>();
		private IApplicationApi applicationApi;
		private IApplicationNameRouter applicationNameRouter;

		/// <summary>
		/// Gets/sets membership API
		/// </summary>
		public IMembershipApi MembershipApi { get { return SpringContext.Current.GetObject<IMembershipApi>(); } }

		/// <summary>
		/// Sets/gets organization API
		/// </summary>
		public IOrganizationApi OrganizationApi { get { return SpringContext.Current.GetObject<IOrganizationApi>(); } }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="applicationApi"></param>
		/// <param name="applicationNameRouter"></param>
		public MockedAuthenticationContext(IApplicationApi applicationApi, IApplicationNameRouter applicationNameRouter)
		{
			this.applicationApi = applicationApi;
			this.applicationNameRouter = applicationNameRouter;
		}

		#region IAuthenticationContext Members

		public Guid ApplicationId 
		{
			get
			{
				if (this.applicationId == Guid.Empty)
				{
					string loweredApplicationName = this.applicationNameRouter.GetApplicationName().ToLowerInvariant();
					using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
					{
						ApplicationObject applicationObject = this.applicationApi.Get(loweredApplicationName);
						if (applicationObject == null)
							throw new InvalidSaaSApplicationException(string.Format(CultureInfo.InvariantCulture, "The application \"{0}\" doesn't exist.", loweredApplicationName));

						this.applicationId = applicationObject.Id;
					}
				}

				return this.applicationId;
			} 
		}

		public IIdentity Identity { get { return this.identity; } }

		public OrganizationObject Organization
		{
			get
			{
				UserObject userObject = this.User;
				if (userObject != null)
					return this.OrganizationApi.GetOrganization(userObject.OrganizationId);

				return null;
			}
		}

		/// <summary>
		/// Get an Empty string. 
		/// </summary>
		public string ClientIPAddress
		{
			get { return string.Empty; }
		}

		public UserObject User
		{
			get
			{
				if (this.userObject == null || this.userObject.UserName != this.identity.Name)
					this.userObject = this.MembershipApi.Get(this.identity.Name);

				return this.userObject;
			}
		}

		public IDictionary Session
		{
			get 
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				if (!this.sessionsByThread.ContainsKey(threadId))
					sessionsByThread.Add(threadId, new Hashtable());

				return this.sessionsByThread[threadId];
			}
			
		}

		public IDictionary TempVariables
		{
			get
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				if (!this.variablesByThread.ContainsKey(threadId))
					variablesByThread.Add(threadId, new Hashtable());

				return this.variablesByThread[threadId];
			}
		}

		public LoginResults Login(string username, string password)
		{
			LoginResults loginResults = this.MembershipApi.Login(username, password);
			if (loginResults != LoginResults.Successful) return loginResults;

			this.userObject = this.MembershipApi.Get(username);
			this.identity = new GenericIdentity(username);
			return LoginResults.Successful;
		}

		/// <summary>
		/// Logout the current user.
		/// </summary>
		public void Logout()
		{
			if (this.User == null) return;
			this.MembershipApi.Logout(this.User.UserId);
		}

		/// <summary>
		/// Track the user interaction for every 30 seconds.
		/// </summary>
		public void Act()
		{
			if (this.User == null) return;
			DateTime lastActivityDate = LocalizationUtility.ConvertClientTimeToUtcTime(this.User.LastActivityDate);
			if (DateTime.UtcNow - lastActivityDate >= new TimeSpan(0, 0, 30))
				this.MembershipApi.Act(this.User.UserId);
		}

		#endregion
	}
}

