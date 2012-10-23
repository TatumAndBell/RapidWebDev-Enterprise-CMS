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

namespace RapidWebDev.UI.DynamicPages.Resolvers
{
	using System.Collections.Generic;
	using System.Configuration;
	using RapidWebDev.Common;
	using RapidWebDev.UI.DynamicPages.Configurations;
	using System.Globalization;
	using RapidWebDev.UI.Properties;

	/// <summary>
	/// Factory to resolve server-side control value from http post parameters.
	/// </summary>
	public class ControlValueResolverFactory : IControlValueResolverFactory
	{
		/// <summary>
		/// Dictionary of resolvers for different query control types.
		/// </summary>
		public IDictionary<string, IControlValueResolver> Resolvers { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="resolvers">Dictionary of resolvers for different query control types.</param>
		public ControlValueResolverFactory(IDictionary<string, IControlValueResolver> resolvers)
		{
			this.Resolvers = resolvers;
		}

		/// <summary>
		/// Returns true if the resolver name is included in the factory.
		/// </summary>
		/// <param name="resolverName"></param>
		/// <returns></returns>
		public bool Contains(string resolverName)
		{
			return this.Resolvers != null && this.Resolvers.ContainsKey(resolverName);
		}

		/// <summary>
		/// Resolve server side control value from http post value for specified query field configuration.
		/// </summary>
		/// <param name="queryFieldConfiguration"></param>
		/// <param name="httpPostValue"></param>
		/// <returns></returns>
		public object Resolve(QueryFieldConfiguration queryFieldConfiguration, string httpPostValue)
		{
			string resolverName = null;
			if (queryFieldConfiguration.Control.ControlType == ControlConfigurationTypes.Custom)
				resolverName = (queryFieldConfiguration.Control as CustomConfiguration).ResolverName;
			else
				resolverName = string.Format(CultureInfo.InvariantCulture, "{0}ControlValueResolver", queryFieldConfiguration.Control.ControlType);

			if (!this.Resolvers.ContainsKey(resolverName))
				throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, Resources.DP_ControlValueResolverUnavailable, resolverName));

			IControlValueResolver resolver = this.Resolvers[resolverName];
			return resolver.Resolve(queryFieldConfiguration, httpPostValue);
		}
	}
}

