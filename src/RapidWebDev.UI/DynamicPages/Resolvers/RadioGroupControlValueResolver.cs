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
	using System.Globalization;
	using System.Linq;
	using RapidWebDev.Common;
	using RapidWebDev.Common.Validation;
	using RapidWebDev.UI.Controls;
	using RapidWebDev.UI.DynamicPages.Configurations;

	/// <summary>
	/// The class to resolve server-side control value for RadioGroup query field.
	/// </summary>
	public class RadioGroupControlValueResolver : IControlValueResolver
	{
		object IControlValueResolver.Resolve(QueryFieldConfiguration queryFieldConfiguration, string httpPostValue)
		{
			int selectedIndex;
			if (int.TryParse(httpPostValue, out selectedIndex))
			{
				RadioGroupConfiguration radioGroupConfiguration = queryFieldConfiguration.Control as RadioGroupConfiguration;
				if(selectedIndex>= radioGroupConfiguration.Items.Count)
					throw new IndexOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "The http post value for the query field \"{0}\" mapping to RadioGroup is out of radio group items.", httpPostValue));

				var fieldValue = radioGroupConfiguration.Items[selectedIndex].Value;
				if (queryFieldConfiguration.FieldValueType != null)
					return Kit.ConvertType(fieldValue, queryFieldConfiguration.FieldValueType);

				return fieldValue;
			}

			string errorMsg = string.Format(CultureInfo.InvariantCulture, "The http post value for the query field \"{0}\" mapping to RadioGroup is invalid.", httpPostValue);
			throw new ValidationException(errorMsg);
		}
	}
}

