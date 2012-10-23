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
	/// The interface contains the system initialization information.
	/// </summary>
	public interface IPlatformConfiguration
	{
		/// <summary>
		/// Hierarchy type value of area.
		/// </summary>
		string AreaHierarchyTypeValue { get; }

		/// <summary>
		/// Gets default organization type
		/// </summary>
		OrganizationTypeObject OrganizationType { get; }

		/// <summary>
		/// Gets default organization
		/// </summary>
		OrganizationObject Organization { get; }

		/// <summary>
		/// Gets default role.
		/// </summary>
		RoleObject Role { get; }

		/// <summary>
		/// Gets default user
		/// </summary>
		UserObject User { get; }

		/// <summary>
		/// Gets password of default user.
		/// </summary>
		string Password { get; }

		/// <summary>
		/// Gets answer to retrieve password for default user.
		/// </summary>
		string PasswordAnswer { get; }

		/// <summary>
		/// Gets all available organization domains.
		/// </summary>
		IEnumerable<OrganizationDomain> Domains { get; }
	}
}

