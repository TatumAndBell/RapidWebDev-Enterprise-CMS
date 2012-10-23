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

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
    /// Support extension field's value and extension field convert operations.
	/// </summary>
	public interface IExtensionObjectSerializer
	{
		/// <summary>
        /// Parse out extension field collection from extension object's extension field (ExtensionData).
		/// </summary>
		/// <param name="extensionObject"></param>
		/// <returns></returns>
		IDictionary<string, object> Deserialize(IExtensionObject extensionObject);

		/// <summary>
        /// Save extension field serilized into extension field (ExtensionData)
		/// </summary>
		/// <param name="extensionObject"></param>
		void Serialize(IExtensionObject extensionObject);
	}
}
