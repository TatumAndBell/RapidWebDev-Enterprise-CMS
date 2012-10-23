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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using RapidWebDev.Common;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
    /// Extension type's business abstract interface
	/// </summary>
	public interface IExtensionBizObject
	{
		/// <summary>
		/// Gets / sets extension data type id.
		/// </summary>
		Guid ExtensionDataTypeId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified property name.
        /// </summary>
        /// <value></value>
		object this[string propertyName] { get; set; }

        /// <summary>
        /// Gets the field enumerator.
        /// </summary>
        /// <returns></returns>
		IEnumerator<KeyValuePair<string, object>> GetFieldEnumerator();
	}
}
