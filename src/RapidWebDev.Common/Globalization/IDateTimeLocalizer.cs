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
	/// The interface to localize date time.
	/// </summary>
	public interface IDateTimeLocalizer
	{
		/// <summary>
		/// Localize input date to string without time segment.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
        string ToDateString(DateTime dateTime);

		/// <summary>
        /// Localize input date/time to string
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
        string ToDateTimeString(DateTime dateTime);

		/// <summary>
		/// Convert server time to Utc time.
		/// </summary>
		/// <param name="serverDateTime"></param>
		/// <returns></returns>
		DateTime ConvertServerTimeToUtcTime(DateTime serverDateTime);

		/// <summary>
		/// Convert client time to Utc time.
		/// </summary>
		/// <param name="clientDateTime"></param>
		/// <returns></returns>
		DateTime ConvertClientTimeToUtcTime(DateTime clientDateTime);

		/// <summary>
		/// Convert Utc time to the client time. 
		/// </summary>
		/// <param name="utcDateTime"></param>
		/// <returns></returns>
		DateTime ConvertUtcTimeToClientTime(DateTime utcDateTime);
	}
}