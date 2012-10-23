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
	using System.Collections;
	using RapidWebDev.Common;
	using RapidWebDev.UI.Controls;
	using RapidWebDev.UI.DynamicPages.Configurations;

	/// <summary>
	/// The class to resolve server-side control value for CheckBoxGroup query field.
	/// </summary>
	public class CheckBoxGroupControlValueResolver : IControlValueResolver
	{
		object IControlValueResolver.Resolve(QueryFieldConfiguration queryFieldConfiguration, string httpPostValue)
		{
			CheckBoxGroupConfiguration checkBoxGroupConfiguration = queryFieldConfiguration.Control as CheckBoxGroupConfiguration;
			string[] postedValueItems = httpPostValue.Split(new[] { WebUtility.ARRAY_ITEM_SEPARATOR }, StringSplitOptions.None);

			Type fieldValueType = queryFieldConfiguration.FieldValueType;

			ArrayList parameters = new ArrayList();
			foreach (string postedValueItem in postedValueItems)
			{
				int checkBoxItemIndex;
				if (int.TryParse(postedValueItem, out checkBoxItemIndex) && checkBoxItemIndex < checkBoxGroupConfiguration.Items.Count)
				{
					string checkBoxValue = checkBoxGroupConfiguration.Items[checkBoxItemIndex].Value;
					object parameterValue = checkBoxValue;
					if (fieldValueType != null)
						parameterValue = Kit.ConvertType(checkBoxValue, fieldValueType);

					parameters.Add(parameterValue);
				}
			}

			return parameters;
		}		
	}
}

