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
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using RapidWebDev.Common;

namespace RapidWebDev.UI.Controls
{
    /// <summary>
	/// BooleanIcon Icon is used to display boolean value by Icon instead of text.
    /// </summary>
	public class BooleanIcon : Image
    {
		/// <summary>
		/// True/False value for the control to display relative icon.
		/// </summary>
		public bool Value { get; set; }

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			this.ImageUrl = GetIconUrl(this.Value);
			base.OnPreRender(e);
		}

        private static string GetIconUrl(bool indicator)
        {
			string iconUrl = string.Format(CultureInfo.InvariantCulture, "~/resources/images/{0}.gif", indicator);
			return Kit.ResolveAbsoluteUrl(iconUrl);
        }
    }
}

