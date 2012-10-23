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

using System.Collections.Generic;
using System.Runtime.Serialization;
using RapidWebDev.Common;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.Platform.Initialization
{
	/// <summary>
	/// The implementation to the interface IInitializationConfiguration which contains the system initialization information.
	/// </summary>
	public class PlatformConfiguration : IPlatformConfiguration
	{
		/// <summary>
		/// Sets/gets hierarchy type value of area.
		/// </summary>
		public string AreaHierarchyTypeValue { get; set; }

		/// <summary>
		/// Sets/gets default organization type
		/// </summary>
		public OrganizationTypeObject OrganizationType { get; set; }

		/// <summary>
		/// Sets/gets default organization
		/// </summary>
		public OrganizationObject Organization { get; set; }

		/// <summary>
		/// Sets/gets default role.
		/// </summary>
		public RoleObject Role { get; set; }

		/// <summary>
		/// Sets/gets default user
		/// </summary>
		public UserObject User { get; set; }

		/// <summary>
		/// Sets/gets password of default user.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Sets/gets answer to retrieve password for default user.
		/// </summary>
		public string PasswordAnswer { get; set; }

		/// <summary>
		/// Sets/gets all available organization domains.
		/// </summary>
		public IEnumerable<OrganizationDomain> Domains { get; set; }
	}
}

