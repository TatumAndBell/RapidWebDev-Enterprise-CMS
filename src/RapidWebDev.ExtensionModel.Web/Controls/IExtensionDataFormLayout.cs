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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.UI;

namespace RapidWebDev.ExtensionModel.Web.Controls
{
	/// <summary>
	/// The interface to generate layout of data input form for the special metadata type.
	/// </summary>
	public interface IExtensionDataFormLayout
	{
		/// <summary>
		/// Create layout control which contains data input form for the special metadata type.
		/// </summary>
		/// <param name="extensionFieldControlBuildersByFieldMetadata">Extension field control builders by field metadata.</param>
		/// <returns></returns>
		Control Create(IEnumerable<KeyValuePair<IFieldMetadata, IExtensionFieldControlBuilder>> extensionFieldControlBuildersByFieldMetadata);
	}
}
