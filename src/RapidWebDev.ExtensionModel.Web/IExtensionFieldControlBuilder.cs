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

namespace RapidWebDev.ExtensionModel.Web
{
	/// <summary>
	/// The interface to build control for extension field.
	/// </summary>
	public interface IExtensionFieldControlBuilder
	{
		/// <summary>
		/// Sets/gets editor control value.
		/// </summary>
		object Value { get; set; }

		/// <summary>
		/// Sets/gets field metadata in metadata management UI.
		/// </summary>
		IFieldMetadata Metadata { get; set; }

		/// <summary>
		/// Sets/gets true if the data input control is readonly.
		/// </summary>
		bool ReadOnly { get; set; }

		/// <summary>
		/// Build data input control for specified field metadata.
		/// </summary>
		/// <param name="fieldMetadata"></param>
		/// <returns></returns>
		ExtensionDataInputControl BuildDataInputControl(IFieldMetadata fieldMetadata);

		/// <summary>
		/// Build a control to manage the field metadata.
		/// </summary>
		/// <returns></returns>
		Control BuildMetadataControl();
	}

	/// <summary>
	/// Extension Editable control.
	/// </summary>
	public class ExtensionDataInputControl
	{
		/// <summary>
		/// Web control contains the editor for the field.
		/// </summary>
		public Control Control { get; set; }

		/// <summary>
		/// How many table cells occupied which is used to render editor control.
		/// </summary>
		public int OccupiedControlCells { get; set; }
	}
}

