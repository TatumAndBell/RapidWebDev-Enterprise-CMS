/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.long.yi@RapidWebDev.org

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
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Security;
using RapidWebDev.Common;
using RapidWebDev.Platform.Properties;
using System.Web.UI;
using System.Reflection;
using System.Globalization;
using RapidWebDev.Common.Web.Services;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// This class provide authenicate when new request which call external Api coming
    /// </summary>
    public class RapidWebServiceAuthorizationManager : ServiceAuthorizationManager
    {
        /// <summary>
		/// Authorization
        /// </summary>
        private const string securityKey = "Authorization";
		private static IServicePermissionMapApi servicePermissionMapApi = SpringContext.Current.GetObject<IServicePermissionMapApi>();
        private static IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();
		private static IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();
		private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

        /// <summary>
        /// Check if there are users logon already
        /// if not, check if the request header contains the info about authentication
        /// </summary>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override bool CheckAccess(OperationContext operationContext)
        {
			if(WebOperationContext.Current == null)
                throw new BadRequestException("The web operation context doesn't exist.");

			OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

			string statusDescription = null;

			// whether the client has been already authenicated
            if (authenticationContext.Identity == null || !authenticationContext.Identity.IsAuthenticated)
			{
				try
				{
					string authenticationToken = WebOperationContext.Current.IncomingRequest.Headers[securityKey];
					if (string.IsNullOrEmpty(authenticationToken))
						throw new UnauthorizedException(Resources.UnauthorizedAccess);

					this.LogOnFromHttpRequestHeader(authenticationToken);
				}
				catch(UnauthorizedException exp)
				{
					statusDescription = exp.Message;
				}
			}

			// check permission
			try
			{
				WCFLookupResult WCFLookupResult = WCFOperationContextUtility.ResolvePermissionAttribute(operationContext);
				if (WCFLookupResult == null) return true;

				string permissionValue = servicePermissionMapApi.GetPermissionValue(WCFLookupResult.ServiceContractName, WCFLookupResult.OperationContractName);
				if (!permissionApi.HasPermission(permissionValue))
					throw new UnauthorizedException();
			}
			catch (UnauthorizedException exp)
			{
				statusDescription = exp.Message ?? statusDescription;
				response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
				response.StatusDescription = statusDescription;
				return false;
			}

			return true;
        }

		private void LogOnFromHttpRequestHeader(string authenticationToken)
		{
			if (authenticationToken.Length < 6)
                    throw new UnauthorizedException(Resources.UnauthorizedAccess);

            if (string.Compare(authenticationToken.Substring(0, 6), "basic ", StringComparison.OrdinalIgnoreCase) != 0)
                throw new UnauthorizedException(Resources.UnauthorizedAccess);

            string credentials = authenticationToken.Substring(6); // skip "basic "
            byte[] decodedBytes = Convert.FromBase64String(credentials);
            credentials = Encoding.UTF8.GetString(decodedBytes);

            int index = credentials.IndexOf(":", StringComparison.Ordinal);
            if (index <= 0 || index > credentials.Length - 2)
                throw new UnauthorizedException();

            //Get the User and Password in request 
            string userName = credentials.Substring(0, index);
            string password = credentials.Substring(index + 1);

			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				throw new UnauthorizedException(Resources.UnauthorizedAccess);

			switch (authenticationContext.Login(userName, password))
            {
                case LoginResults.InvalidCredential:
                    throw new UnauthorizedException(Resources.InvalidUserCredential);

                case LoginResults.InvalidOrganization:
					throw new UnauthorizedException(Resources.InvalidOrganizationCredential);

				case LoginResults.LockedOut:
					throw new UnauthorizedException(Resources.UserHasBeenLockedOut);
            }
		}
    }
}