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
using System.Web.UI;

using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.Mocks.UIMocks
{
    /// <summary>
    /// The proxy let user can test business logic which class inherit from IDetailPanelPage
    /// </summary>
    public class DetailPanelPageProxy : DynamicComponentUtil,IDetailPanelPage
    {
        
        /// <summary>
        ///  The event argument of detail panel page
        /// </summary>
        DetailPanelPageEventArgs detailPanelArgs;
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
        public DetailPanelPageProxy(object actual) : base(actual)
        {
            this.actual = actual;
            //Construct the fields of the DetailPanelPage class
            FieldInfo[] fields = actual.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Count(); i++)
            {
                FieldInfo field = fields[i];
                if (field.GetCustomAttributes(typeof(BindingAttribute), false).Count() > 0 )
                    fields[i].SetValue(actual, Activator.CreateInstance(field.FieldType));
            }  
        }


        #region inherit from IDetailPanelPageProxy
        /// <summary>
        /// When User call Create method in IDetailPanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <returns></returns>
        public string Create()
        {
            Validate();
            setupArgs = new SetupApplicationContextVariablesEventArgs();
            detailPanelArgs = new DetailPanelPageEventArgs("", DetailPanelPageRenderModes.New);
            sender = new RequestHandlerMock();
            sender.isPostBack = true;
            IDetailPanelPage actualPanel = actual as IDetailPanelPage;
            actualPanel.SetupContextTempVariables(sender, setupArgs);
            actualPanel.OnInit(sender, detailPanelArgs);
            actualPanel.OnLoad(sender, detailPanelArgs);

            return actualPanel.Create();


        }
        /// <summary>
        /// When User call Update method in IDetailPanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <param name="entityId"></param>
        public void Update(string entityId)
        {
            Validate();
            setupArgs = new SetupApplicationContextVariablesEventArgs();
            detailPanelArgs = new DetailPanelPageEventArgs(entityId, DetailPanelPageRenderModes.Update);
            sender = new RequestHandlerMock();
            sender.isPostBack = true;
            IDetailPanelPage actualPanel = actual as IDetailPanelPage;

            actualPanel.SetupContextTempVariables(sender, setupArgs);
            actualPanel.OnInit(sender, detailPanelArgs);
            actualPanel.OnLoad(sender, detailPanelArgs);

            actualPanel.Update(entityId);

        }
        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
        }
        /// <summary>
        /// When User call LoadWritableEntity method in IDetailPanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <param name="entityId"></param>
        public void LoadWritableEntity(string entityId)
        {
            Validate();
            setupArgs = new SetupApplicationContextVariablesEventArgs();
            detailPanelArgs = new DetailPanelPageEventArgs(entityId, DetailPanelPageRenderModes.Update);
            sender = new RequestHandlerMock();
            sender.isPostBack = true;
            IDetailPanelPage actualPanel = actual as IDetailPanelPage;
            actualPanel.SetupContextTempVariables(sender, setupArgs);
            actualPanel.OnInit(sender, detailPanelArgs);
            actualPanel.OnLoad(sender, detailPanelArgs);

            actualPanel.LoadWritableEntity(entityId);
        }
        /// <summary>
        /// When User call LoadReadOnlyEntity method in IDetailPanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <param name="entityId"></param>
        public void LoadReadOnlyEntity(string entityId) 
        {
            Validate();
            setupArgs = new SetupApplicationContextVariablesEventArgs();
            detailPanelArgs = new DetailPanelPageEventArgs(entityId, DetailPanelPageRenderModes.Update);
            sender = new RequestHandlerMock();
            sender.isPostBack = true;
            IDetailPanelPage actualPanel = actual as IDetailPanelPage;
            actualPanel.SetupContextTempVariables(sender, setupArgs);
            actualPanel.OnInit(sender, detailPanelArgs);
            actualPanel.OnLoad(sender, detailPanelArgs);

            actualPanel.LoadReadOnlyEntity(entityId);
        }


        /// <summary>
        /// When User call OnPreRender method in IDetailPanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnPreRender(IRequestHandler sender, DetailPanelPageEventArgs e)
        {

        }
        /// <summary>
        /// When User call SetupContextTempVariables method in IDetailPanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e) { }
        /// <summary>
        /// When User call OnInit method in IDetailPanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnInit(IRequestHandler sender, DetailPanelPageEventArgs e) { }
        /// <summary>
        /// When User call OnLoad method in IDetailPanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLoad(IRequestHandler sender, DetailPanelPageEventArgs e) { }

        #endregion

       
    }
}
