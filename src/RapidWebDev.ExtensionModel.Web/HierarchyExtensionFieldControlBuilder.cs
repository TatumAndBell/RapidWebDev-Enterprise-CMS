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

namespace RapidWebDev.ExtensionModel.Web
{
	/// <summary>
	/// The implementation class to build control for hierarchy typed extension field.
	/// </summary>
	public class HierarchyExtensionFieldControlBuilder : AbstractExtensionFieldControlBuilder
	{
		/// <summary>
		/// Sets/gets true if the data input control is readonly.
		/// </summary>
		public override bool ReadOnly 
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Sets/gets editor control value.
		/// </summary>
		public override object Value
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Sets/gets field metadata in metadata management UI.
		/// </summary>
		public override IFieldMetadata Metadata { get; set; }

		/// <summary>
		/// Build data input control for specified field metadata.
		/// </summary>
		/// <param name="fieldMetadata"></param>
		/// <returns></returns>
		public override ExtensionDataInputControl BuildDataInputControl(IFieldMetadata fieldMetadata)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		public override Control BuildMetadataControl()
		{
			throw new NotImplementedException();
		}
	}
}
