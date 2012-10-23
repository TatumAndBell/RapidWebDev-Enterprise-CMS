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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel.Web.Properties;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.ExtensionModel.Web.Controls;
using RapidWebDev.Common.Web;

namespace RapidWebDev.ExtensionModel.Web.DynamicPage
{
	/// <summary>
	/// Extension field metadata aggregate panel page handler to preview editable form for the extension type.
	/// </summary>
	public class ExtensionFieldMetadataAggregatePanel : AggregatePanelPage
	{
		private static IApplicationContext applicationContext = SpringContext.Current.GetObject<IApplicationContext>();
		private static IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();

		/// <summary />
		[Binding]
		protected ExtensionDataForm ExtensionDataFormPreview;

		/// <summary>
		/// Set MetadataTypeId to the form.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnLoad(IRequestHandler sender, AggregatePanelPageEventArgs e)
		{
			base.OnLoad(sender, e);
			this.ExtensionDataFormPreview.CreateDataInputForm((Guid)applicationContext.TempVariables["MetadataDataTypeId"]);
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

