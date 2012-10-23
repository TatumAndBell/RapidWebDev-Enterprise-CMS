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
using RapidWebDev.Common;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.Platform.Web.DynamicPage.GridViewFieldValueTransformCallback
{
	/// <summary>
	/// Show hierarchy data name by hierarchy data id.
	/// </summary>
	public class ShowHierarchyDataName : IGridViewFieldValueTransformCallback
	{
		private static IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

		#region IGridViewFieldValueTransformCallback Members

		/// <summary>
		/// Format input field value. 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		public string Format(string fieldName, object fieldValue)
		{
			Guid hierarchyDataId = Guid.Empty;
			try
			{
				hierarchyDataId = new Guid(fieldValue.ToString());
			}
			catch
			{
				return "";
			}

			HierarchyDataObject hierarchyData = hierarchyApi.GetHierarchyData(hierarchyDataId);
			if (hierarchyData == null) return "";

			return hierarchyData.ToString();
		}

		#endregion
	}
}

