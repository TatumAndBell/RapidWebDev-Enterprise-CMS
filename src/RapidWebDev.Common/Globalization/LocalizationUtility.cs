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

namespace RapidWebDev.Common.Globalization
{
	/// <summary>
	/// Globalization convert methods.
	/// </summary>
	public static class LocalizationUtility
	{
		private static readonly DefaultDateTimeLocalizer defaultDateTimeLocalizer = new DefaultDateTimeLocalizer(new DefaultClientTimeZoneLoader());

		/// <summary>
		/// Convert input date to string without time segment.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="emptyStringWhenEarlierEqualThanDate">Returns empty string when specified date is earlier/equal than this argument.</param>
		/// <returns></returns>
		public static string ToDateString(DateTime dateTime, DateTime emptyStringWhenEarlierEqualThanDate)
		{
			if (dateTime <= emptyStringWhenEarlierEqualThanDate) return "";
            return ToDateString(dateTime);
		}

		/// <summary>
		/// Convert input date to string without time segment.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static string ToDateString(DateTime dateTime)
		{
            	//IDateTimeLocalizer localizer = SpringContext.Current.GetObject<IDateTimeLocalizer>() ?? defaultDateTimeLocalizer;
            IDateTimeLocalizer localizer = defaultDateTimeLocalizer;
            if (localizer == null)
                return defaultDateTimeLocalizer.ToDateString(dateTime);
            else
                return localizer.ToDateString(dateTime);
		}

		/// <summary>
		/// Convert input date/time to string
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="emptyStringWhenEarlierEqualThanDate">Returns empty string when specified date is earlier/equal than this argument.</param>
		/// <returns></returns>
		public static string ToDateTimeString(DateTime dateTime, DateTime emptyStringWhenEarlierEqualThanDate)
		{
			if (dateTime <= emptyStringWhenEarlierEqualThanDate) return "";
            return ToDateTimeString(dateTime);
		}

		/// <summary>
		/// Convert input date/time to string
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static string ToDateTimeString(DateTime dateTime)
		{
            	//IDateTimeLocalizer localizer = SpringContext.Current.GetObject<IDateTimeLocalizer>() ?? defaultDateTimeLocalizer;
            IDateTimeLocalizer localizer = defaultDateTimeLocalizer;
            if (localizer == null)
                return defaultDateTimeLocalizer.ToDateTimeString(dateTime);
            else
                return localizer.ToDateTimeString(dateTime);
		}

		/// <summary>
		/// Convert server time to Utc time.
		/// </summary>
		/// <param name="serverDateTime"></param>
		/// <returns></returns>
		public static DateTime ConvertServerTimeToUtcTime(DateTime serverDateTime)
		{
				//IDateTimeLocalizer localizer = SpringContext.Current.GetObject<IDateTimeLocalizer>() ?? defaultDateTimeLocalizer;
            IDateTimeLocalizer localizer = defaultDateTimeLocalizer;
			return localizer.ConvertServerTimeToUtcTime(serverDateTime);
		}

		/// <summary>
		/// Convert client time to Utc time.
		/// </summary>
		/// <param name="clientDateTime"></param>
		/// <returns></returns>
		public static DateTime ConvertClientTimeToUtcTime(DateTime clientDateTime)
		{
				//IDateTimeLocalizer localizer = SpringContext.Current.GetObject<IDateTimeLocalizer>() ?? defaultDateTimeLocalizer;
            IDateTimeLocalizer localizer = defaultDateTimeLocalizer;
			return localizer.ConvertServerTimeToUtcTime(clientDateTime);
		}

		/// <summary>
		/// Convert client time to Utc time.
		/// </summary>
		/// <param name="clientDateTime"></param>
		/// <returns></returns>
		public static DateTime? ConvertClientTimeToUtcTime(DateTime? clientDateTime)
		{
			if (!clientDateTime.HasValue) return null;

				//IDateTimeLocalizer localizer = SpringContext.Current.GetObject<IDateTimeLocalizer>() ?? defaultDateTimeLocalizer;
            IDateTimeLocalizer localizer = defaultDateTimeLocalizer;
			return localizer.ConvertServerTimeToUtcTime(clientDateTime.Value);
		}

		/// <summary>
		/// Convert Utc time to the client time. 
		/// </summary>
		/// <param name="utcDateTime"></param>
		/// <returns></returns>
		public static DateTime ConvertUtcTimeToClientTime(DateTime utcDateTime)
		{
				//IDateTimeLocalizer localizer = SpringContext.Current.GetObject<IDateTimeLocalizer>() ?? defaultDateTimeLocalizer;
            IDateTimeLocalizer localizer = defaultDateTimeLocalizer;
			return localizer.ConvertUtcTimeToClientTime(utcDateTime);
		}

		/// <summary>
		/// Convert Utc time to the client time. 
		/// </summary>
		/// <param name="utcDateTime"></param>
		/// <returns></returns>
		public static DateTime? ConvertUtcTimeToClientTime(DateTime? utcDateTime)
		{
			if (!utcDateTime.HasValue) return null;

			//IDateTimeLocalizer localizer = SpringContext.Current.GetObject<IDateTimeLocalizer>() ?? defaultDateTimeLocalizer;
            IDateTimeLocalizer localizer = defaultDateTimeLocalizer;
			return localizer.ConvertUtcTimeToClientTime(utcDateTime.Value);
		}
	}
}