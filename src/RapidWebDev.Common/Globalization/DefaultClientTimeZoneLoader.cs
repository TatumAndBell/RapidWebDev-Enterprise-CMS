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
	/// The interface to get the timezone of the client.
	/// </summary>
	public class DefaultClientTimeZoneLoader : IClientTimeZoneLoader
	{
		/// <summary>
		/// Load timezone of the client.
		/// </summary>
		/// <returns></returns>
		public virtual TimeZoneSetting Load()
		{
			return new TimeZoneSetting { UtcOffset = (DateTime.Now - DateTime.UtcNow).TotalHours, Name = Resources.WebServerTimeZoneName };
		}
	}
}