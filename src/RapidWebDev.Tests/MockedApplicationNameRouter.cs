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
using System.Web;
using RapidWebDev.Platform.Initialization;

namespace RapidWebDev.Tests
{
	/// <summary>
	/// Mocked application name loader
	/// </summary>
	public class MockedApplicationNameRouter : IApplicationNameRouter
	{
		private string applicationName;

		/// <summary>
		/// Get application mapping dictionary. 
		/// The key is HttpContext.Current.Request.Url.Authority and value is application name.
		/// If application name is not configured, returns HttpContext.Current.Request.Url.Authority.
		/// </summary>
		public Dictionary<string, string> Mapping { get; set; }

		/// <summary>
		/// Get application name.
		/// </summary>
		/// <returns></returns>
		public string GetApplicationName()
		{
			if (this.applicationName == null)
			{
				this.applicationName = string.Format("UnitTest {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			}

			return this.applicationName;
		}
	}
}
