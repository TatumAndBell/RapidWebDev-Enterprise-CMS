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
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using AjaxControlToolkit;
using RapidWebDev.Common;
using RapidWebDev.UI.Properties;
using System.Globalization;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// A specialized panel intended for use as an application window. The body of the window is loaded from specified URL.
	/// </summary>
	public sealed class ModalWindowHandler
	{
		/// <summary>
		/// Format passing arguments to a JavaScript sentence which used for opening a modal window to show specified page url.
		/// </summary>
		/// <param name="headerText"></param>
		/// <param name="pageUrl"></param>
		/// <param name="draggable"></param>
		/// <param name="modal"></param>
		/// <param name="resizable"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="onWindowClosedJsCallback"></param>
		/// <returns></returns>
		public static string FormatVariableDeclaration(string headerText, string pageUrl, bool draggable, bool modal, bool resizable, int width, int height, string onWindowClosedJsCallback)
		{
			return string.Format(CultureInfo.InvariantCulture, @"window.ModalWindowHandler.Show('{0}', '{1}', {{ draggable: {2}, modal: {3}, resizable: {4}, width: {5}, height: {6}, windowCloseCallback: '{7}' }});",
				WebUtility.EncodeJavaScriptString(headerText),
				WebUtility.EncodeJavaScriptString(pageUrl),
				draggable.ToString().ToLowerInvariant(),
				modal.ToString().ToLowerInvariant(),
				resizable.ToString().ToLowerInvariant(),
				width,
				height,
				WebUtility.EncodeJavaScriptString(onWindowClosedJsCallback));
		}
	}
}

