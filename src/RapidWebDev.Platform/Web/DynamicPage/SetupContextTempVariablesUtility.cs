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
using System.Linq;
using System.Web;
using RapidWebDev.Common;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.Common.Web;
using System.Globalization;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Utility to setup dynamic page temporary variables.
	/// </summary>
	public static class SetupContextTempVariablesUtility
	{
		

		/// <summary>
		/// Get domain from query string of current http request and save "Domain.Text" and "Domain.Value" into TempVariables of IApplicationContext.
		/// It redirects the request to PageNotFound Url if query string "domain" is invalid.
		/// </summary>
		/// <param name="requestHandler">Request handler.</param>
		/// <param name="e">Callback event argument.</param>
		public static void SetupOrganizationDomain(IRequestHandler requestHandler, SetupApplicationContextVariablesEventArgs e)
		{
			SetupOrganizationDomain(requestHandler, e, true);
		}

		/// <summary>
		/// Get domain from query string of current http request and save "Domain.Text" and "Domain.Value" into TempVariables of IApplicationContext.
		/// </summary>
		/// <param name="requestHandler">Request handler.</param>
		/// <param name="e">Callback event argument.</param>
		/// <param name="rediectToPageNotFoundUrlIfDomainNotFound">Whether to redirect the request to PageNotFound Url if query string "domain" is invalid</param>
		public static void SetupOrganizationDomain(IRequestHandler requestHandler, SetupApplicationContextVariablesEventArgs e, bool rediectToPageNotFoundUrlIfDomainNotFound)
		{
			IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
			string domain = requestHandler.Parameters["Domain"];
			OrganizationDomain organizationDomain = platformConfiguration.Domains.FirstOrDefault(d => d.Value == domain);
			if (organizationDomain == null && rediectToPageNotFoundUrlIfDomainNotFound)
				throw new ResourceNotFoundException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidDomain, domain));

			if (organizationDomain != null)
			{
				e.TempVariables["Domain.Text"] = organizationDomain.Text;
				e.TempVariables["Domain.Value"] = organizationDomain.Value;
			}
		}

		/// <summary>
		/// Get concrete data type from query string of current http request and save "ConcreteDataCategory" into TempVariables of IApplicationContext.
		/// </summary>
		/// <param name="requestHandler">Request handler.</param>
		/// <param name="e"></param>
		/// <param name="rediectToPageNotFoundUrlIfCategoryNotFound"></param>
		public static void SetupConcreteDataType(IRequestHandler requestHandler, SetupApplicationContextVariablesEventArgs e, bool rediectToPageNotFoundUrlIfCategoryNotFound)
		{
			string concreteDataType = requestHandler.Parameters["ConcreteDataType"];
			if (string.IsNullOrEmpty(concreteDataType) && rediectToPageNotFoundUrlIfCategoryNotFound)
				throw new ResourceNotFoundException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidConcreteDataType, concreteDataType));

			if (!string.IsNullOrEmpty(concreteDataType))
				e.TempVariables["ConcreteDataType"] = concreteDataType;
		}

		/// <summary>
		/// Get hierarchy type from query string of current http request and save "HierarchyType" into TempVariables of IApplicationContext.
		/// </summary>
		/// <param name="requestHandler">Request handler.</param>
		/// <param name="e"></param>
		/// <param name="rediectToPageNotFoundUrlIfHierarchyTypeNotFound"></param>
		public static void SetupHierarchyType(IRequestHandler requestHandler, SetupApplicationContextVariablesEventArgs e, bool rediectToPageNotFoundUrlIfHierarchyTypeNotFound)
		{
			IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
			string hierarchyType = requestHandler.Parameters["HierarchyType"];
			if (string.IsNullOrEmpty(hierarchyType) && rediectToPageNotFoundUrlIfHierarchyTypeNotFound)
				throw new ResourceNotFoundException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyType, hierarchyType));

			e.TempVariables["HierarchyType"] = hierarchyType;
		}
	}
}
