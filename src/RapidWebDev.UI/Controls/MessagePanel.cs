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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.Controls
{
    /// <summary>
    /// Message Panel bases on ExtJs
    /// </summary>
    public partial class MessagePanel : WebControl, INamingContainer
    {
        /// <summary>
        /// Show error message
        /// </summary>
        /// <param name="message"></param>
        public void ShowError(string message)
        {
            this.ShowMessage(MessageTypes.Error, message);
        }

        /// <summary>
        /// Show error message
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="args"></param>
        public void ShowError(string messageFormat, params object[] args)
        {
            this.ShowMessage(MessageTypes.Error, string.Format(messageFormat, args));
        }

        /// <summary>
        /// Show error message
        /// </summary>
        /// <param name="exception"></param>
        public void ShowError(Exception exception)
        {
			this.ShowMessage(MessageTypes.Error, Resources.DP_UnknownErrorDetail);
        }

        /// <summary>
        /// Show warning message
        /// </summary>
        /// <param name="message"></param>
        public void ShowWarning(string message)
        {
            this.ShowMessage(MessageTypes.Warn, message);
        }

        /// <summary>
        /// Show warning message
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="args"></param>
        public void ShowWarning(string messageFormat, params object[] args)
        {
            this.ShowMessage(MessageTypes.Warn, string.Format(messageFormat, args));
        }

        /// <summary>
        /// Show confirm message
        /// </summary>
        /// <param name="message"></param>
        public void ShowConfirm(string message)
        {
            this.ShowMessage(MessageTypes.Info, message);
        }

        /// <summary>
        /// Show warning message
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="args"></param>
        public void ShowConfirm(string messageFormat, params object[] args)
        {
            this.ShowMessage(MessageTypes.Info, string.Format(messageFormat, args));
        }

        /// <summary>
        /// Show typed message on web panel.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        private void ShowMessage(MessageTypes messageType, string message)
        {
			string invokedMethod = string.Format(CultureInfo.InvariantCulture, "window.RWD.MessageBox.{0}('{0}', '{1}');", messageType, message);
			ClientScripts.RegisterScriptBlock(invokedMethod);
        }
    }
}
