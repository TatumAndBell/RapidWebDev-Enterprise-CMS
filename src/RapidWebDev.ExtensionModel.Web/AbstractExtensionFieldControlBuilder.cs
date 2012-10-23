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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using RapidWebDev.UI.Controls;
using System.Globalization;
using WebControls = System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Web.Compilation;
using RapidWebDev.Common;
using System.IO;
using RapidWebDev.UI;

namespace RapidWebDev.ExtensionModel.Web
{
	/// <summary>
	/// The abstract implementation class to build control for supported typed extension field.
	/// </summary>
	public abstract class AbstractExtensionFieldControlBuilder : IExtensionFieldControlBuilder
	{
		private static ExtensionModelConfiguration configuration = SpringContext.Current.GetObject<ExtensionModelConfiguration>();

		/// <summary>
		/// Sets/gets true if the data input control is readonly.
		/// </summary>
		public abstract bool ReadOnly { get; set; }

		/// <summary>
		/// Sets/gets editor control value.
		/// </summary>
		public abstract object Value { get; set; }

		/// <summary>
		/// Sets/gets field metadata in metadata management UI.
		/// </summary>
		public abstract IFieldMetadata Metadata { get; set; }

		/// <summary>
		/// Build data input control for specified field metadata.
		/// </summary>
		/// <param name="fieldMetadata"></param>
		/// <returns></returns>
		public abstract ExtensionDataInputControl BuildDataInputControl(IFieldMetadata fieldMetadata);

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		public abstract Control BuildMetadataControl();

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		protected Control CreateFieldMetadataTemplateControl(FieldType fieldType)
		{
			string fieldMetadataControlTemplateFileName = string.Format(CultureInfo.InvariantCulture, "{0}MetadataControl.ascx", fieldType);
			string fieldMetadataControlTemplatePath = Path.Combine(configuration.FieldMetadataControlTemplateDirectory, fieldMetadataControlTemplateFileName);

			Page webpage = HttpContext.Current.Handler as Page;
			Control control = webpage.LoadControl(fieldMetadataControlTemplatePath);
			WebUtility.SetControlsByBindingAttribute(control, this);

			return control;
		}
	}
}
