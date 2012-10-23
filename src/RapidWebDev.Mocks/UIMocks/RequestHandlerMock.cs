/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.long.yi@RapidWebDev.org

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
using System.Collections.Specialized;
using System.Web;

using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.Mocks.UIMocks
{
    /// <summary>
    /// Mock the IRequestHandler
    /// </summary>
    public class RequestHandlerMock : IRequestHandler
    {
        #region inherit from IRequestHandler
        /// <summary>
        /// 
        /// </summary>
        public bool isPostBack;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPostBack
        {
            get { return isPostBack; }
            set { isPostBack = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool isAsync;
        /// <summary>
        /// 
        /// </summary>
        public bool IsAsynchronous
        {
            get { return isAsync; }
            set { isAsync = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public NameValueCollection Parameters
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.Params;
                else
                    throw new Exception("No HttpContext");
            }
        }
        #endregion
    }
}
