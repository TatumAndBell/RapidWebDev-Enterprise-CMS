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
    /// The proxy let user can test business logic which class inherit from IAggregatePanelPage
    /// </summary>
    public class AggregatePanelPageProxy : DynamicComponentUtil,IAggregatePanelPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actual"></param>
        public AggregatePanelPageProxy(IAggregatePanelPage actual) : base(actual) 
        {
            this.actual = actual;
            //Construct the fields of the DetailPanelPage class
            FieldInfo[] fields = actual.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Count(); i++)
            {
                FieldInfo field = fields[i];
                if (field.GetCustomAttributes(typeof(BindingAttribute), false).Count() > 0)
                    fields[i].SetValue(actual, Activator.CreateInstance(field.FieldType));
            }  
        }

        SetupApplicationContextVariablesEventArgs setupArgs;

        AggregatePanelPageEventArgs aggregatePanelPageArgs;

        RequestHandlerMock sender;

        /// <summary>
        /// When User call Save method in IAggregatePanelPage,
        /// This method will be invoked, and help user do the pre-required conditions, setup parameters
        /// </summary>
        /// <param name="commandArgument"></param>
        /// <param name="entityIdEnumerable"></param>
        public void Save(string commandArgument, IEnumerable<string> entityIdEnumerable) 
        {
            setupArgs = new SetupApplicationContextVariablesEventArgs();
            aggregatePanelPageArgs = new AggregatePanelPageEventArgs("Save", entityIdEnumerable);
            sender = new RequestHandlerMock();

            IAggregatePanelPage actualPanel = actual as IAggregatePanelPage;

            actualPanel.SetupContextTempVariables(sender, setupArgs);
            actualPanel.OnInit(sender, aggregatePanelPageArgs);
            actualPanel.OnLoad(sender, aggregatePanelPageArgs);

            actualPanel.Save(commandArgument, entityIdEnumerable);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnInit(IRequestHandler sender, AggregatePanelPageEventArgs e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLoad(IRequestHandler sender, AggregatePanelPageEventArgs e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnPreRender(IRequestHandler sender, AggregatePanelPageEventArgs e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e) { }
    }
}
