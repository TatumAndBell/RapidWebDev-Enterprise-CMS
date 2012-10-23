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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel;

namespace RapidWebDev.Platform.Services
{
	/// <summary>
	/// Collection of identities.
	/// </summary>
	[CollectionDataContract(Name = "IdCollection", ItemName = "Id", Namespace = ServiceNamespaces.Platform)]
	public class IdCollection : Collection<string>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public IdCollection()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ids"></param>
		public IdCollection(IList<string> ids) : base(ids)
		{
		}
	}
}
