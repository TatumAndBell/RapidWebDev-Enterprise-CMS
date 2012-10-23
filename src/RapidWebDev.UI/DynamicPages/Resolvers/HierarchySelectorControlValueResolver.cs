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

namespace RapidWebDev.UI.DynamicPages.Resolvers
{
	using System;
	using System.Linq;
	using System.Collections;
	using System.Web.Script.Serialization;
	using RapidWebDev.Common;
	using RapidWebDev.UI.Controls;
	using RapidWebDev.UI.DynamicPages.Configurations;
	using System.Collections.Generic;

	/// <summary>
	/// The class to resolve server-side control value for HierarchySelector query field.
	/// </summary>
	public class HierarchySelectorControlValueResolver : IControlValueResolver
	{
		private readonly static JavaScriptSerializer serializer = new JavaScriptSerializer();

		object IControlValueResolver.Resolve(QueryFieldConfiguration queryFieldConfiguration, string httpPostValue)
		{
			if(string.IsNullOrEmpty(httpPostValue))
				return new ArrayList();

			List<HierarchyItem> hierarchyItems = serializer.Deserialize<List<HierarchyItem>>(httpPostValue);
			IEnumerable<string> postedValueItems = hierarchyItems.Select(item => item.Id.ToString());

			Type fieldValueType = queryFieldConfiguration.FieldValueType;
			ArrayList parameters = new ArrayList();
			foreach (string postedValueItem in postedValueItems)
			{
				object parameterValue = postedValueItem;
				if (fieldValueType != null)
					parameterValue = Kit.ConvertType(postedValueItem.Trim(), fieldValueType);

				parameters.Add(parameterValue);
			}

			return parameters;
		}		
	}
}