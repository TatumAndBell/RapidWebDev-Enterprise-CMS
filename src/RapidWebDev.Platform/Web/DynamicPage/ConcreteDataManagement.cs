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
using System.Text;
using System.Threading;
using System.Web.UI;
using RapidWebDev.Common;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.Platform.Web.DynamicPage
{
	/// <summary>
	/// Concrete data management dynamic page handler.
	/// </summary>
	public class ConcreteDataManagement : RapidWebDev.UI.DynamicPages.DynamicPage
	{
		private static object syncObject = new object();
		/// <summary>
		/// Protected authentication context.
		/// </summary>
		protected static readonly IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
		/// <summary>
		/// Protected concrete data Api.
		/// </summary>
		protected static readonly IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();

		/// <summary>
		/// Execute query for results binding to dynamic page grid.
		/// </summary>
		/// <param name="parameter">Query parameter.</param>
		/// <returns>Returns query results.</returns>
		public override QueryResults Query(QueryParameter parameter)
		{
            LinqPredicate predicate = LinqPredicate.Concat(parameter.Expressions.Compile(), this.CreateCustomQuery(parameter));

			int recordCount;
			string sortingExpression = null;
			if (parameter.SortExpression != null)
				sortingExpression = parameter.SortExpression.Compile();
			if (Kit.IsEmpty(sortingExpression))
				sortingExpression = "LastUpdatedDate ASC";

			string concreteDataType = authenticationContext.TempVariables["ConcreteDataType"] as string;
			predicate.Add("Type=@0", concreteDataType);
			IEnumerable<ConcreteDataObject> concreteDataObjects = concreteDataApi.FindConcreteData(predicate, sortingExpression, parameter.PageIndex, parameter.PageSize, out recordCount);
			return new QueryResults(recordCount, concreteDataObjects);
		}

		/// <summary>
		/// Delete concrete data by id.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Delete(string entityId)
		{
			ConcreteDataObject concreteDataObject = concreteDataApi.GetById(new Guid(entityId));
			if (concreteDataObject != null)
			{
				concreteDataObject.DeleteStatus = DeleteStatus.Deleted;
				concreteDataApi.Save(concreteDataObject);
			}
		}

		/// <summary>
		/// Not approved concrete datas cannot be edited and deleted.
		/// </summary>
		/// <param name="e"></param>
		public override void OnGridRowControlsBind(GridRowControlBindEventArgs e)
		{
			DeleteStatus deleteStatus = (DeleteStatus)DataBinder.Eval(e.DataItem, "DeleteStatus");
			e.ShowDeleteButton = deleteStatus != DeleteStatus.Deleted;
		}

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// Set domain into http context when web page is initializing.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			SetupContextTempVariablesUtility.SetupConcreteDataType(sender, e, true);
		}

		/// <summary>
		/// Create custom query expression which will be used with input query parameter to execute query by the method Query.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		protected virtual LinqPredicate CreateCustomQuery(QueryParameter parameter)
		{
			return null;
		}
	}
}

