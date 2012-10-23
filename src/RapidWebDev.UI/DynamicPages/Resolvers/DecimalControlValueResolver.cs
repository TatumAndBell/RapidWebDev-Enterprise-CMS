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
	using System.IO;
	using System.Configuration;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls.WebParts;
	using ListItem = System.Web.UI.WebControls.ListItem;
	using RapidWebDev.Common;
	using RapidWebDev.UI.Controls;
	using RapidWebDev.UI.DynamicPages.Configurations;
	using RapidWebDev.UI.Properties;

	/// <summary>
	/// The class to resolve server-side control value for Decimal query field.
	/// </summary>
	public class DecimalControlValueResolver : IControlValueResolver
	{
		object IControlValueResolver.Resolve(QueryFieldConfiguration queryFieldConfiguration, string httpPostValue)
		{
			decimal returnedValue;
			if (!decimal.TryParse(httpPostValue, out returnedValue))
				throw new ControlValueResolveException(string.Format(CultureInfo.InvariantCulture, Resources.DP_ControlValueConvertFailedMessage, httpPostValue, "Decimal"));

			return returnedValue;
		}
	}
}
