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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using RapidWebDev.Common;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using System.Threading;
using System.Web.UI;
using RapidWebDev.Common.Data;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Organization type management dynamic page handler.
	/// </summary>
	public class OrganizationTypeManagement : RapidWebDev.UI.DynamicPages.DynamicPage
	{
		private static object syncObject = new object();
		/// <summary>
		/// Protected authentication context.
		/// </summary>
		protected static readonly IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		/// <summary>
		/// Protected organization Api.
		/// </summary>
		protected static readonly IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();

		/// <summary>
		/// Execute query for results binding to dynamic page grid.
		/// </summary>
		/// <param name="parameter">Query parameter.</param>
		/// <returns>Returns query results.</returns>
		public override QueryResults Query(QueryParameter parameter)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				Guid applicationId = SpringContext.Current.GetObject<IAuthenticationContext>().ApplicationId;
				var q = from orgType in ctx.OrganizationTypes where orgType.ApplicationId == applicationId select orgType;

				if (authenticationContext.TempVariables["Domain.Value"] != null)
					q = q.Where(orgType => orgType.Domain == authenticationContext.TempVariables["Domain.Value"] as string);

				LinqPredicate predicate = parameter.Expressions.Compile();
				if (predicate != null)
					q = q.Where(predicate.Expression, predicate.Parameters);

				int recordCount = q.Count();

				string sortingExpression = null;
				if (parameter.SortExpression != null)
					sortingExpression = parameter.SortExpression.Compile();

				if (Kit.IsEmpty(sortingExpression))
					sortingExpression = "LastUpdatedDate DESC";

				var dataSource = q.OrderBy(sortingExpression).Skip(parameter.PageIndex * parameter.PageSize).Take(parameter.PageSize).ToList();
				return new QueryResults(recordCount, dataSource);
			}
		}

		/// <summary>
		/// Delete orgianization type by id.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Delete(string entityId)
		{
			OrganizationTypeObject organizationTypeObject = organizationApi.GetOrganizationType(new Guid(entityId));
			if (organizationTypeObject != null && !organizationTypeObject.Predefined)
			{
				organizationTypeObject.DeleteStatus = DeleteStatus.Deleted;
				organizationApi.Save(organizationTypeObject);
			}
		}

		/// <summary>
		/// Predefined organization type cannot be edited and deleted.
		/// </summary>
		/// <param name="e"></param>
		public override void OnGridRowControlsBind(GridRowControlBindEventArgs e)
		{
			bool predefined = (bool)DataBinder.Eval(e.DataItem, "Predefined");
			e.ShowDeleteButton = !predefined;
			e.ShowEditButton = !predefined;
		}

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// Set domain into http context when web page is initializing.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			SetupContextTempVariablesUtility.SetupOrganizationDomain(sender, e, false);
		}
	}
}

