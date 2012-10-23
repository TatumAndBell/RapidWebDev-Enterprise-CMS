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
using System.Text;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Role management dynamic page handler.
	/// </summary>
	public class RoleManagement : RapidWebDev.UI.DynamicPages.DynamicPage
	{
		private static object syncObject = new object();
		private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		private static IRoleApi roleApi = SpringContext.Current.GetObject<IRoleApi>();

		/// <summary>
		/// Execute query for results binding to dynamic page grid.
		/// </summary>
		/// <param name="parameter">Query parameter.</param>
		/// <returns>Returns query results.</returns>
		public override QueryResults Query(QueryParameter parameter)
		{
			string domain = authenticationContext.TempVariables["Domain.Value"] as string;
			int recordCount;
            LinqPredicate predicate = LinqPredicate.Concat(parameter.Expressions.Compile(), new LinqPredicate("Domain=@0", domain));

			string sortingExpression = null;
			if (parameter.SortExpression != null)
				sortingExpression = parameter.SortExpression.Compile();

			if (Kit.IsEmpty(sortingExpression))
				sortingExpression = "RoleName ASC";

            IEnumerable<RoleObject> roleObjects = roleApi.FindRoles(predicate, sortingExpression, parameter.PageIndex, parameter.PageSize, out recordCount);
			return new QueryResults(recordCount, roleObjects);
		}

		/// <summary>
		/// Delete role by id.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Delete(string entityId)
		{
			RoleObject roleObject = roleApi.Get(new Guid(entityId));
			if (roleObject != null && !roleObject.Predefined)
				roleApi.HardDelete(roleObject.RoleId);
		}

		/// <summary>
		/// Predefined roles cannot be edited and deleted.
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
			SetupContextTempVariablesUtility.SetupOrganizationDomain(sender, e);
		}
	}
}

