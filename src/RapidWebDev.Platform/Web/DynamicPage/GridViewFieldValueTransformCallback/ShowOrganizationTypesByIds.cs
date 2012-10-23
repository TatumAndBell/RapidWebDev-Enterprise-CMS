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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Web.UI;
using RapidWebDev.Common;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.Platform.Web.DynamicPage.GridViewFieldValueTransformCallback
{
	/// <summary>
	/// The callback to show organization types by enumerable organization type ids.
	/// </summary>
	public class ShowOrganizationTypesByIds : IGridViewFieldValueTransformCallback
	{
		#region IGridViewFieldValueTransformCallback Members

		/// <summary>
		/// Format input field value. 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		public string Format(string fieldName, object fieldValue)
		{
			IEnumerable<Guid> organizationTypeIds = fieldValue as IEnumerable<Guid>;
			if (organizationTypeIds == null) return "";

			StringBuilder displayTextBuilder = new StringBuilder();
			IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			foreach (Guid organizationTypeId in organizationTypeIds)
			{
				if (displayTextBuilder.Length > 0) displayTextBuilder.Append(", ");

				OrganizationTypeObject organizationTypeObject = organizationApi.GetOrganizationType(organizationTypeId);
				displayTextBuilder.Append(organizationTypeObject.Name);
			}

			return displayTextBuilder.ToString();
		}

		#endregion
	}
}

