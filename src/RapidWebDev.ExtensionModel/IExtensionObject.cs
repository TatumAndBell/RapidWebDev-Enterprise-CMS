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
using System.Xml.Linq;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
    /// Extendable Object Interface.
	/// </summary>
	public interface IExtensionObject
	{
        /// <summary>
        /// Gets or sets the extension data type id.
        /// </summary>
        /// <value>The extension data type id.</value>
		Guid ExtensionDataTypeId { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        /// <value>The extension data.</value>
        string ExtensionData { get; set; }

        /// <summary>
        /// Gets a value indicating whether this extension properties has changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has changed; otherwise, <c>false</c>.
        /// </value>
		bool HasChanged { get; }

        /// <summary>
        /// Gets or sets the property's value with the specified name.
        /// </summary>
        /// <value></value>
		object this[string name] { get; set; }

        /// <summary>
        /// Gets the dynamic field enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<string, object>> GetFieldEnumerator();
		
		/// <summary>
		/// Clear all existed extension fields.
		/// </summary>
		void RemoveAllExtensionFields();
	}
}
