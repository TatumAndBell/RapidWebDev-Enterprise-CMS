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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using RapidWebDev.Common;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Class to management organizations.
	/// </summary>
	public class OrganizationManagement : RapidWebDev.UI.DynamicPages.DynamicPage
	{
		private static object syncObject = new object();
		/// <summary>
		/// Protected authentication context.
		/// </summary>
		protected static readonly IAuthenticationContext authenticationContext  = SpringContext.Current.GetObject<IAuthenticationContext>();
		/// <summary>
		/// Protected organization Api.
		/// </summary>
		protected static readonly IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
		/// <summary>
		/// Protected permission Api.
		/// </summary>
		protected static readonly IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();

		/// <summary>
		/// Execute query for organizations binding to dynamic page grid.
		/// </summary>
		/// <param name="parameter">Query parameter.</param>
		/// <returns>Returns query results.</returns>
		public override QueryResults Query(QueryParameter parameter)
		{
            LinqPredicate predicate = LinqPredicate.Concat(parameter.Expressions.Compile(), this.CreateCustomQuery(parameter));

			string sortExpression = null;
			if (parameter.SortExpression != null)
				sortExpression = parameter.SortExpression.Compile();

			if (Kit.IsEmpty(sortExpression))
				sortExpression = "LastUpdatedDate DESC";

			int recordCount;
			IEnumerable<OrganizationObject> organizationObjects = organizationApi.FindOrganizations(predicate, sortExpression, parameter.PageIndex, parameter.PageSize, out recordCount);
			var dataSource = organizationObjects.Select(c => new
			{
				OrganizationId = c.OrganizationId,
				OrganizationCode = c.OrganizationCode,
				OrganizationName = c.OrganizationName,
				c.Description,
				c.Status,
				ParentOrganizationId = c.ParentOrganizationId,
				c.Hierarchies,
				c.LastUpdatedBy,
				c.LastUpdatedDate,
				c.CreatedBy,
				c.CreatedDate,
				OrganizationTypeName = organizationApi.GetOrganizationType(c.OrganizationTypeId).ToString(),
				c.Properties
			}).ToList();

			return new QueryResults(recordCount, dataSource);
		}

		/// <summary>
		/// Delete specified organization by id.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Delete(string entityId)
		{
			OrganizationObject organizationObject = organizationApi.GetOrganization(new Guid(entityId));
			if (organizationObject != null)
			{
				organizationObject.Status = OrganizationStatus.Disabled;
				organizationApi.Save(organizationObject);
			}
		}

		/// <summary>
		/// Disabled organizations cannot be edited and deleted.
		/// </summary>
		/// <param name="e"></param>
		public override void OnGridRowControlsBind(GridRowControlBindEventArgs e)
		{
			OrganizationStatus status = (OrganizationStatus)DataBinder.Eval(e.DataItem, "Status");
			e.ShowDeleteButton = status != OrganizationStatus.Disabled;
		}

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// Set domain into http context when web page is initializing.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e) 
		{
			SetupContextTempVariablesUtility.SetupOrganizationDomain(sender, e);
		}

		/// <summary>
		/// Create custom query expression which will be used with input query parameter to execute query by the method Query.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		protected virtual LinqPredicate CreateCustomQuery(QueryParameter parameter)
		{
			string domain = authenticationContext.TempVariables["Domain.Value"] as string;
			return new LinqPredicate("OrganizationType.Domain=@0", new object[] { domain });
		}
	}
}

