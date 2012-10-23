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
using System.Security.Principal;
using System.Text;
using System.Web;
using RapidWebDev.Common;

namespace RapidWebDev.Platform.Initialization
{
	/// <summary>
	/// Interface to initialize application.
	/// </summary>
	public interface IInstaller
	{
		/// <summary>
		/// Install application
		/// </summary>
		/// <param name="applicationName">application name to install</param>
		/// <returns>returns false to skip following installers' execution</returns>
		void Install(string applicationName);

		/// <summary>
		/// Uninstall application
		/// </summary>
		/// <param name="applicationName">application name to uninstall</param>
		/// <returns>returns true when the uninstalling successfully.</returns>
		void Uninstall(string applicationName);
	}
}

