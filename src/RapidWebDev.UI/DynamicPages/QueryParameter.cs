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
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.CodeDom;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Query parameter.
	/// </summary>
	public class QueryParameter
	{
		/// <summary>
		/// Sets/gets  collection of query parameter expressions, defaults to Empty if there is no query expression.
		/// </summary>
		public QueryParameterExpressionCollection Expressions { get; private set; }

		/// <summary>
		/// Sets/gets query paging index.
		/// </summary>
		public int PageIndex { get; set; }

		/// <summary>
		/// Sets/gets page size. int.MaxValue indicates to query all records by matching query parameters.
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// Sets/gets sorting expression. E.g "Name" | "Name ASC" | "Name DESC". 
		/// Defautls to NULL if there is no sorting expression specified.
		/// </summary>
		public SortExpression SortExpression { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
		public QueryParameter()
		{
            this.Expressions = new QueryParameterExpressionCollection();
		}

		/// <summary>
		/// Create <see cref="QueryParameter"/> instance from a series of parameters.
		/// </summary>
		/// <param name="dynamicPageService"></param>
		/// <param name="queryStrings"></param>
		/// <param name="start"></param>
		/// <param name="limit"></param>
		/// <param name="sortField"></param>
		/// <param name="sortDirection"></param>
		/// <returns></returns>
		internal static QueryParameter CreateQueryParameter(IDynamicPage dynamicPageService, NameValueCollection queryStrings, int start, int limit, string sortField, string sortDirection)
		{
			// Collect query parameters from HTTP requests
			Collection<KeyValuePair<int, string>> queryStringVariables = new Collection<KeyValuePair<int, string>>();
			foreach (string queryStringName in queryStrings.AllKeys)
			{
				if (queryStringName.StartsWith(WebUtility.QUERY_FIELD_CONTROL_POST_PREFRIX_NAME))
				{
					int queryFieldControlIndex;
					if (int.TryParse(queryStringName.Substring(WebUtility.QUERY_FIELD_CONTROL_POST_PREFRIX_NAME.Length), out queryFieldControlIndex))
					{
						string queryStringValue = queryStrings[queryStringName];
						if (!Kit.IsEmpty(queryStringValue))
							queryStringVariables.Add(new KeyValuePair<int, string>(queryFieldControlIndex, queryStringValue));
					}
				}
			}

			// Build query parameters expression
			QueryParameterExpressionCollection queryParameterExpressionCollection = null;
			QueryPanelConfiguration queryPanelConfiguration = dynamicPageService.Configuration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.QueryPanel) as QueryPanelConfiguration;
			if (queryPanelConfiguration != null)
			{
				IEnumerable<QueryFieldConfiguration> queryFieldConfigurations = queryPanelConfiguration.Controls;
				queryParameterExpressionCollection = QueryFieldControlFactory.BuildQueryParameterExpressions(queryFieldConfigurations, queryStringVariables);
			}

			// The client posting sortField is definitely the data binding FieldName
			// Here should transform sort field name if SortingFieldName is configured for the field.
			SortExpression sortExpression = null;
			if (!Kit.IsEmpty(sortField))
			{
				GridViewPanelConfiguration gridViewPanel = dynamicPageService.Configuration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.GridViewPanel) as GridViewPanelConfiguration;
				if (gridViewPanel != null)
				{
					string resolvedSortField = gridViewPanel.ResolveOriginalFieldName(sortField);
					sortExpression = new SortExpression(resolvedSortField, sortDirection == null || sortDirection.ToUpperInvariant() != "DESC");
				}
			}

			QueryParameter queryParameter = new QueryParameter
			{
				PageIndex = start / limit,
				PageSize = limit,
				SortExpression = sortExpression,
				Expressions = queryParameterExpressionCollection ?? new QueryParameterExpressionCollection()
			};

			return queryParameter;
		}
	}
}

