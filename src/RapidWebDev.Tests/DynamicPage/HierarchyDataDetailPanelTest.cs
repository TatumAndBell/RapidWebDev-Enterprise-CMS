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
using W = System.Web.UI.WebControls;

using RapidWebDev.ExtensionModel.Web.DynamicPage;
using RapidWebDev.Mocks.UIMocks;
using RapidWebDev.Mocks;
using RapidWebDev.Platform.Web.DynamicPage;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;

using NUnit.Framework;
using RapidWebDev.UI.Controls;

namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class HierarchyDataDetailPanelTest
    {
        List<Guid> ids = new List<Guid>();

        [TearDown]
        public void CleanUp()
        {
            IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

            foreach (var id in ids)
            {
                hierarchyApi.HardDeleteHierarchyData(id);
            }
        }

        [Test,Description("")]
        public void TestCreate()
        {
            HierarchyDataDetailPanel page = new HierarchyDataDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                #region Create Data
                httpEnv.SetRequestUrl(@"/HierarchyDataDetailPanel/DynamicPage.svc?HierarchyType=Department");

                Guid guid = Guid.NewGuid();

                string surfix = guid.ToString().Substring(0, 5);
                W.TextBox TextBoxName = new W.TextBox();
                TextBoxName.Text = "HierarchyType" + surfix;
                proxy.Set("TextBoxName", TextBoxName);

                //ComboBox ComboBoxParentHierarchyData = new ComboBox();


                W.TextBox TextBoxDescription = new W.TextBox();
                TextBoxDescription.Text = "HierarchyType" + surfix;
                proxy.Set("TextBoxDescription", TextBoxDescription);

                proxy.Set("ComboBoxParentHierarchyData", null);

                proxy.Set("ExtensionDataForm",null);
                #endregion

                string entityId = proxy.Create();

                ids.Add(new Guid(entityId));
            }

        }
        [Test, Description("")]
        public void TestUpdate()
        {
            HierarchyDataDetailPanel page = new HierarchyDataDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

            using (var httpEnv = new HttpEnvironment())
            {
                #region Create Data
                httpEnv.SetRequestUrl(@"/HierarchyDataDetailPanel/DynamicPage.svc?HierarchyType=Department");
                Guid guid = Guid.NewGuid();

                string surfix = guid.ToString().Substring(0, 5);

                HierarchyDataObject obj = new HierarchyDataObject() 
                {
                    HierarchyType = "Department",
                    Name = "HierarchyType" + surfix,
                    Description = "HierarchyType" + surfix
                    
                };

                hierarchyApi.Save(obj);
                #endregion

                #region Bind Web Control
                W.TextBox TextBoxName = new W.TextBox();
                TextBoxName.Text = "HierarchyType" + surfix;
                proxy.Set("TextBoxName", TextBoxName);

                W.TextBox TextBoxDescription = new W.TextBox();
                TextBoxDescription.Text = "HierarchyTypeUpdate" + surfix;
                proxy.Set("TextBoxDescription", TextBoxDescription);

                proxy.Set("ComboBoxParentHierarchyData", null);

                proxy.Set("ExtensionDataForm", null);
                #endregion

                proxy.Update(obj.HierarchyDataId.ToString());

                Assert.AreEqual(obj.Description, "HierarchyType" + surfix);

                obj = hierarchyApi.GetHierarchyData(obj.HierarchyDataId);

                Assert.AreEqual(obj.Description, "HierarchyTypeUpdate" + surfix);

            } 
        }

    }
}
