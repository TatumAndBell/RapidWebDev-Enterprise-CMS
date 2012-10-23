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
using System.Reflection;

using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.Mocks.UIMocks
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicPageProxy : DynamicComponentUtil, IDynamicPage
    {
        /// <summary>
        ///  The event argument of detail panel page
        /// </summary>
        EventArgs dynamicPageArgs;
        /// <summary>
        /// Event argument to setup variables for the request context in dynamic/detail panel/aggregate panel pages.
        /// </summary>
        SetupApplicationContextVariablesEventArgs setupArgs;
        /// <summary>
        /// 
        /// </summary>
        RequestHandlerMock sender;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actual"></param>
        public DynamicPageProxy(IDynamicPage actual)
            : base(actual)
        {
            //Construct the fields of the DetailPanelPage class
            FieldInfo[] fields = actual.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Count(); i++)
            {
                FieldInfo field = fields[i];
                if (field.GetCustomAttributes(typeof(BindingAttribute), false).Count() > 0)
                    fields[i].SetValue(actual, Activator.CreateInstance(field.FieldType));
            }

        }

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public QueryResults Query(QueryParameter parameter) 
        {
            IDynamicPage actualPage = this.actual as IDynamicPage;
            setupArgs = new SetupApplicationContextVariablesEventArgs();
            sender = new RequestHandlerMock();
            dynamicPageArgs = new EventArgs();

            actualPage.SetupContextTempVariables(sender, setupArgs);
            actualPage.OnInit(sender, dynamicPageArgs);
            actualPage.OnLoad(sender, dynamicPageArgs);

            return actualPage.Query(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        public void Delete(string entityId) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void OnGridRowControlsBind(GridRowControlBindEventArgs e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnInit(IRequestHandler sender, EventArgs e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLoad(IRequestHandler sender, EventArgs e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnPreRender(IRequestHandler sender, EventArgs e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e) { }
        #endregion
    }
}
