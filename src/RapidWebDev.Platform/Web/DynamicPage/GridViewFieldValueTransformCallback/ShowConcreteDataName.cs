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
	/// Show concrete data name by concrete data id.
	/// </summary>
	public class ShowConcreteDataName : IGridViewFieldValueTransformCallback
	{
		private static IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();

		#region IGridViewFieldValueTransformCallback Members

		/// <summary>
		/// Format input field value. 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		public string Format(string fieldName, object fieldValue)
		{
			Guid concreteDataId = Guid.Empty;
			try
			{
				concreteDataId = new Guid(fieldValue.ToString());
			}
			catch
			{
				return "";
			}

			ConcreteDataObject concreteDataObject = concreteDataApi.GetById(concreteDataId);
			if (concreteDataObject == null) return "";

			return concreteDataObject.ToString();
		}

		#endregion
	}
}

