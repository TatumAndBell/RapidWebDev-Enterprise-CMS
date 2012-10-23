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
using System.Web.UI;

namespace RapidWebDev.Mocks.UIMocks
{
    /// <summary>
    /// This interface provides the methods let user can set/get the control of the dynamic page
    /// </summary>
    public interface IDynamicPageUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlKey"></param>
        /// <param name="controlValue"></param>
        void Set(string controlKey, Control controlValue);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlKey"></param>
        /// <returns></returns>
        object Get(string controlKey);
    }
}
