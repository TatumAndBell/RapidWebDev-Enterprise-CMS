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
using RapidWebDev.Common.Properties;
using System.Globalization;

namespace RapidWebDev.Common.Globalization
{
	/// <summary>
	/// The simple implementation to convert date time without considering localization.
	/// </summary>
	public sealed class DefaultDateTimeLocalizer : IDateTimeLocalizer
	{
		private IClientTimeZoneLoader clientTimeZoneLoader;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="clientTimeZoneLoader"></param>
		public DefaultDateTimeLocalizer(IClientTimeZoneLoader clientTimeZoneLoader)
		{
			this.clientTimeZoneLoader = clientTimeZoneLoader;
		}

		/// <summary>
		/// Localize input date to string without time segment.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
        public string ToDateString(DateTime dateTime)
        {
			CultureInfo culture = CultureInfo.CurrentUICulture ?? CultureInfo.GetCultureInfo("en-US");
            return dateTime.ToString(Resources.DateFormatString, culture);
        }

		/// <summary>
        /// Localize input date/time to string
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
        public string ToDateTimeString(DateTime dateTime)
        {
			CultureInfo culture = CultureInfo.CurrentUICulture ?? CultureInfo.GetCultureInfo("en-US");
            return dateTime.ToString(Resources.DateTimeFormatString, culture);
        }

		/// <summary>
		/// Convert server time to Utc time.
		/// </summary>
		/// <param name="serverDateTime"></param>
		/// <returns></returns>
		public DateTime ConvertServerTimeToUtcTime(DateTime serverDateTime)
		{
			return serverDateTime.ToUniversalTime();
		}

		/// <summary>
		/// Convert client time to Utc time.
		/// </summary>
		/// <param name="clientDateTime"></param>
		/// <returns></returns>
		public DateTime ConvertClientTimeToUtcTime(DateTime clientDateTime)
		{
			TimeZoneSetting timeZoneSetting = this.clientTimeZoneLoader.Load();
			double utcOffset = timeZoneSetting != null ? timeZoneSetting.UtcOffset : 0;
			return clientDateTime.AddHours(-utcOffset);
		}

		/// <summary>
		/// Convert Utc time to the client time. 
		/// </summary>
		/// <param name="utcDateTime"></param>
		/// <returns></returns>
		public DateTime ConvertUtcTimeToClientTime(DateTime utcDateTime)
		{
			TimeZoneSetting timeZoneSetting = this.clientTimeZoneLoader.Load();
			double utcOffset = timeZoneSetting != null ? timeZoneSetting.UtcOffset : 0;
			return utcDateTime.AddHours(utcOffset);
		}
	}
}

