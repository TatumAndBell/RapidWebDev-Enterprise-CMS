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
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using AspNetMembership = System.Web.Security.Membership;
using RapidWebDev.Common;
using RapidWebDev.Platform.Linq;
using System.Globalization;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Authentication context HTTP implementation class.
	/// </summary>
	public class HttpAuthenticationContext : IAuthenticationContext
	{
		internal const string CurrentUserSessionKey = "CurrentUser";

		private static readonly object syncObj = new object();
		private static readonly Dictionary<string, Guid> applicationContainers = new Dictionary<string, Guid>();
		private IApplicationApi applicationApi;

		/// <summary>
		/// Gets/sets membership API
		/// </summary>
		public IMembershipApi MembershipApi { get { return SpringContext.Current.GetObject<IMembershipApi>(); } }

		/// <summary>
		/// Sets/gets organization API
		/// </summary>
		public IOrganizationApi OrganizationApi { get { return SpringContext.Current.GetObject<IOrganizationApi>(); } }

		/// <summary>
		/// Http authentication context instance
		/// </summary>
		/// <param name="applicationApi"></param>
		public HttpAuthenticationContext(IApplicationApi applicationApi)
		{
			this.applicationApi = applicationApi;
		}

		/// <summary>
		/// Get current running application id.
		/// </summary>
		public Guid ApplicationId
		{
			get
			{
				string loweredApplicationName = AspNetMembership.ApplicationName.ToLower();
				if (!applicationContainers.ContainsKey(loweredApplicationName))
				{
					lock (syncObj)
					{
						if (!applicationContainers.ContainsKey(loweredApplicationName))
						{
							ApplicationObject applicationObject = this.applicationApi.Get(loweredApplicationName);
							if (applicationObject == null)
								throw new InvalidSaaSApplicationException(string.Format(CultureInfo.InvariantCulture, "The application \"{0}\" doesn't exist.", loweredApplicationName));

							applicationContainers.Add(loweredApplicationName, applicationObject.Id);
						}
					}
				}

				return applicationContainers[loweredApplicationName];
			}
		}

		/// <summary>
		/// Get current authenticated identity instance.
		/// </summary>
		public IIdentity Identity
		{
			get
			{
				if (HttpContext.Current.User == null || HttpContext.Current.User.Identity == null || !HttpContext.Current.User.Identity.IsAuthenticated)
					HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new AnonymousIdentity(PermissionConst.ANONYMOUS), new string[] { });

				return HttpContext.Current.User.Identity;
			}
		}

		/// <summary>
		/// Get the client IP address.
		/// </summary>
		public string ClientIPAddress
		{
			get { return HttpContext.Current.Request.UserHostAddress; }
		}

		/// <summary>
		/// Get organization the current login user belongs to.
		/// </summary>
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
		/// Get current login user.
		/// </summary>
		public UserObject User
		{
			get
			{
				UserObject userObject = this.Session[CurrentUserSessionKey] as UserObject;

				if (userObject != null && userObject.ApplicationId != this.ApplicationId)
				{
					this.Session[CurrentUserSessionKey] = null;
					userObject =  null;
				}

				if (userObject == null || userObject.UserName != HttpContext.Current.User.Identity.Name)
				{
					userObject = this.MembershipApi.Get(this.Identity.Name);
					this.Session[CurrentUserSessionKey] = userObject;
				}

				return userObject;
			}
		}

		/// <summary>
		/// Set/get context value by name.
		/// </summary>
		/// <returns></returns>
		public IDictionary Session
		{
			get 
            {
                const string contextKey = "RapidWebDev.Platform.HttpAuthenticationContext::Session";
				if (HttpContext.Current.Session[contextKey] == null)
					HttpContext.Current.Session.Add(contextKey, new Hashtable());

                return HttpContext.Current.Session[contextKey] as Hashtable; 
            }
		}

		/// <summary>
		/// Gets variables for current request context which can also be used to format variable markers in display labels like $VariableName$.
		/// It likes HttpContext.Current.Items, the variables stored will be disposed after a request is completed so that developers can share variables in a request.
		/// </summary>
		public IDictionary TempVariables 
		{
			get
			{
				const string contextKey = "RapidWebDev.Platform.HttpAuthenticationContext::TempVariables";
				if (!HttpContext.Current.Items.Contains(contextKey))
					HttpContext.Current.Items.Add(contextKey, new Hashtable());

				return HttpContext.Current.Items[contextKey] as Hashtable;
			}
		}

		/// <summary>
		/// Login with user name and password.
		/// </summary>
		/// <param name="username">user name</param>
		/// <param name="password">password</param>
		/// <returns>returns status of login</returns>
		public LoginResults Login(string username, string password)
		{
			LoginResults loginResults = this.MembershipApi.Login(username, password);
			if (loginResults != LoginResults.Successful)
				return loginResults;

			this.Session[CurrentUserSessionKey] = this.MembershipApi.Get(username);
			HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(username), new string[0]);
			FormsAuthentication.SetAuthCookie(username, true);
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
			if (HttpContext.Current.Session == null) return;
			if (this.User == null) return;
			DateTime lastActivityDate = LocalizationUtility.ConvertClientTimeToUtcTime(this.User.LastActivityDate);
			if (DateTime.UtcNow - lastActivityDate >= new TimeSpan(0, 0, 30))
				this.MembershipApi.Act(this.User.UserId);
		}
	}
}