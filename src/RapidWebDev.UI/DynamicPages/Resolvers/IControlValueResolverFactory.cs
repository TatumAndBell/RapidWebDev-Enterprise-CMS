﻿/****************************************************************************************************
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

namespace RapidWebDev.UI.DynamicPages.Resolvers
{
	using RapidWebDev.UI.DynamicPages.Configurations;

	/// <summary>
	/// Factory to resolve server-side control value from http post parameters.
	/// </summary>
	public interface IControlValueResolverFactory
	{
		/// <summary>
		/// Returns true if the resolver name is included in the factory.
		/// </summary>
		/// <param name="resolverName"></param>
		/// <returns></returns>
		bool Contains(string resolverName);

		/// <summary>
		/// Resolve server side control value from http post value for specified query field configuration.
		/// </summary>
		/// <param name="queryFieldConfiguration"></param>
		/// <param name="httpPostValue"></param>
		/// <returns></returns>
		object Resolve(QueryFieldConfiguration queryFieldConfiguration, string httpPostValue);
	}
}

