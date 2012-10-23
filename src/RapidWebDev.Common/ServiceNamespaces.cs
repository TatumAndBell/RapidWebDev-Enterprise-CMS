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

namespace RapidWebDev.Common
{
    /// <summary>
    /// Indicate version of all application framework assemblies.
    /// </summary>
    public static class ServiceNamespaces
    {
		/// <summary>
		/// Namespace of Common WCF services
		/// </summary>
		public const string Common = "http://www.rapidwebdev.org/common";

        /// <summary>
		/// Namespace of Platform WCF services
        /// </summary>
		public const string Platform = "http://www.rapidwebdev.org/platform";

		/// <summary>
		/// Namespace of Extension Model DataContract
		/// </summary>
		public const string ExtensionModel = "http://www.rapidwebdev.org/extensionmodel";

		/// <summary>
		/// Namespace of File Management DataContract
		/// </summary>
		public const string FileManagement = "http://www.rapidwebdev.org/filemanagement";

		/// <summary>
		/// Namespace of Statistics DataContract
		/// </summary>
		public const string Statistics = "http://www.rapidwebdev.org/statistics";
    }
}