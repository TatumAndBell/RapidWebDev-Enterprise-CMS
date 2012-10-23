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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.UI;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.Controls
{
    /// <summary>
    /// The image button for popup a selectable IE dialog.
    /// </summary>
    public class PopupSelectorControl : WebControl
    {
        /// <summary>
        /// Callback control id to receive the selected value in popup window.
        /// </summary>
        public string CallbackControlId { get; set; }

        /// <summary>
        /// Set/get true to append selected value to callback control by a separator ";".
        /// </summary>
        public bool AppendSelectedValue { get; set; }

        /// <summary>
        /// Set/get advisor name of selector web page.
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Sets/gets extra parameters will be formatted to query string.
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            string pageUrl = this.GetPageUrl();
            string javascript = WebUtility.GetJavaScriptToOpenWindow(pageUrl, this.Width.Value.ToString(), this.Height.Value.ToString(), false);
            string html = "<span style=\"cursor:pointer\" onclick=\"javascript:{0}\"><img src=\"{1}\" alt=\"Search\" /></span>";
			writer.Write(html, javascript, Kit.ResolveAbsoluteUrl("~/resources/images/query.png"));
        }

        /// <summary>
        /// Get page url to popup.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetPageUrl()
        {
            if (string.IsNullOrEmpty(this.CallbackControlId))
				throw new InvalidProgramException(Resources.Ctrl_PropertyCallbackControlIdCannotBeNullOrEmpty);

            if (string.IsNullOrEmpty(this.ObjectId))
				throw new InvalidProgramException(Resources.Ctrl_PropertyObjectIdCannotBeNullOrEmpty);

            var callbackControl = this.Parent.FindControl(this.CallbackControlId);
            if (callbackControl == null)
				throw new InvalidProgramException(Resources.Ctrl_NoControlFoundByCallbackControlId);

            StringBuilder q = new StringBuilder();
            q.AppendFormat("?OpenerCallbackControlId={0}", callbackControl.ClientID);
            q.AppendFormat("&AppendSelectedValue={0}", this.AppendSelectedValue);
            q.AppendFormat("&ObjectId={0}", this.ObjectId);

            if (this.Parameters != null && this.Parameters.Count > 0)
            {
                foreach (string parameterName in this.Parameters.Keys)
                {
                    object parameterValue = this.Parameters[parameterName];
                    if (parameterValue == null) continue;
                    q.AppendFormat("&{0}={1}", parameterName, parameterValue);
                }
            }

            return string.Format("/Workshop.aspx{0}", q.ToString());
        }
    }
}

