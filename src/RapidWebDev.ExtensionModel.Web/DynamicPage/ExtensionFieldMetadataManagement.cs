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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.ExtensionModel.Web.Properties;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.ExtensionModel.Web.DynamicPage
{
	/// <summary>
	/// Extension field metadata management dynamic page handler.
	/// </summary>
	public class ExtensionFieldMetadataManagement : RapidWebDev.UI.DynamicPages.DynamicPage
	{
		private static object syncObject = new object();
		private static IApplicationContext applicationContext = SpringContext.Current.GetObject<IApplicationContext>();
		private static IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();

		/// <summary>
		/// Execute query for results binding to dynamic page grid.
		/// </summary>
		/// <param name="parameter">Query parameter.</param>
		/// <returns>Returns query results.</returns>
		public override QueryResults Query(QueryParameter parameter)
		{
			IEnumerable<IFieldMetadata> fieldMetadataEnumerable = metadataApi.GetFields((Guid)applicationContext.TempVariables["MetadataDataTypeId"]);
			if (parameter.SortExpression != null)
				fieldMetadataEnumerable = fieldMetadataEnumerable.AsQueryable().OrderBy(parameter.SortExpression.Compile());

			return new QueryResults(fieldMetadataEnumerable.Count(), fieldMetadataEnumerable);
		}

		/// <summary>
		/// Delete field metadata by id.
		/// </summary>
		/// <param name="entityId"></param>
		public override void Delete(string entityId)
		{
			string fieldName = entityId;
			metadataApi.DeleteField((Guid)applicationContext.TempVariables["MetadataDataTypeId"], fieldName);
		}

		/// <summary>
		/// Cannot modify and delete the inherited fields from parents.
		/// </summary>
		/// <param name="e"></param>
		public override void OnGridRowControlsBind(GridRowControlBindEventArgs e)
		{
			bool inherited = (bool)DataBinder.Eval(e.DataItem, "Inherited");
			e.ShowDeleteButton = !inherited;
			e.ShowEditButton = !inherited;
		} 

		/// <summary>
		/// Validate query string "MetadataDataTypeName" and setup metadata data type name to IAuthenticationContext.TempVariables.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			string metadataDataTypeName = QueryStringUtility.MetadataDataTypeName(sender);
			IObjectMetadata metadata = metadataApi.GetType(metadataDataTypeName);
			if (metadata == null)
				throw new ResourceNotFoundException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidMetadataTypeName, metadataDataTypeName));

			e.TempVariables["MetadataDataTypeId"] = metadata.Id;
			e.TempVariables["MetadataDataTypeName"] = metadata.Name;
		}
	}
}
