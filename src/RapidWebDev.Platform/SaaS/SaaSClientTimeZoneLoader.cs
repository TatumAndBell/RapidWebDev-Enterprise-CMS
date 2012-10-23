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

using RapidWebDev.Common.Globalization;
using System;
using RapidWebDev.Common;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The implementation to get the timezone of the client.
	/// </summary>
	public class SaaSClientTimeZoneLoader : DefaultClientTimeZoneLoader
	{
		private IAuthenticationContext authenticationContext;
		private IApplicationApi applicationApi;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authenticationContext"></param>
		/// <param name="applicationApi"></param>
		public SaaSClientTimeZoneLoader(IAuthenticationContext authenticationContext, IApplicationApi applicationApi)
		{
			this.authenticationContext = authenticationContext;
			this.applicationApi = applicationApi;
		}

		/// <summary>
		/// Load timezone of the client.
		/// </summary>
		/// <returns></returns>
		public override TimeZoneSetting Load()
		{
			try
			{
				if (this.authenticationContext.User != null && this.authenticationContext.User["TimeZoneUtcOffset"] != null && this.authenticationContext.User["TimeZoneName"] != null)
				{
					double timeZoneUtcOffset;
					if (double.TryParse(this.authenticationContext.User["TimeZoneUtcOffset"].ToString(), out timeZoneUtcOffset))
						return new TimeZoneSetting { UtcOffset = timeZoneUtcOffset, Name = this.authenticationContext.User["TimeZoneName"].ToString() };
				}

				Guid applicationId = this.authenticationContext.ApplicationId;
				ApplicationObject application = this.applicationApi.Get(applicationId);
				if (application != null && application["TimeZoneUtcOffset"] != null && application["TimeZoneName"] != null)
				{
					double timeZoneUtcOffset;
					if (double.TryParse(application["TimeZoneUtcOffset"].ToString(), out timeZoneUtcOffset))
						return new TimeZoneSetting { UtcOffset = timeZoneUtcOffset, Name = application["TimeZoneName"].ToString() };
				}
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidSaaSApplicationException)
			{
			}

			return base.Load();
		}
	}
}