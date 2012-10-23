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
using System.Web.UI.WebControls;

using RapidWebDev.ExtensionModel.Web.DynamicPage;
using RapidWebDev.Mocks.UIMocks;
using RapidWebDev.Mocks;
using RapidWebDev.Platform.Web.DynamicPage;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;

using NUnit.Framework;
namespace RapidWebDev.Tests.DynamicPage
{
    [TestFixture]
    public class ConcreteDataDetailPanelTest
    {
        List<Guid> ids = new List<Guid>();
        [TearDown]
        public void CleanUp()
        {
            IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();
            foreach (var id in ids)
            {
                var obj = concreteDataApi.GetById(id);
                obj.DeleteStatus = DeleteStatus.Deleted;
                concreteDataApi.Save(obj);
            }
        }
        [Test, Description("Test the Create Method in ConcreteDataDetailPanel")]
        public void TestCreate()
        {
            ConcreteDataDetailPanel page = new ConcreteDataDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();
            using (var httpEnv = new HttpEnvironment())
            {
                httpEnv.SetRequestUrl(@"/ConcreteDataDetailPanel/DynamicPage.svc?ConcreteDataType=Department");
                #region Bind Control
                Guid guid = Guid.NewGuid();

                string surfix = guid.ToString().Substring(0, 5);
                TextBox TextBoxName = new TextBox();
                TextBoxName.Text = "concrete" + surfix;
                proxy.Set("TextBoxName", TextBoxName);

                TextBox TextBoxValue = new TextBox();
                TextBoxValue.Text = "concrete" + surfix;
                proxy.Set("TextBoxValue", TextBoxValue);

                TextBox TextBoxDescription = new TextBox();
                TextBoxDescription.Text = "concrete" + surfix;
                proxy.Set("TextBoxDescription", TextBoxDescription);

                Array statusData = new string[] { "NotDeleted", "Deleted" };
                RadioButtonList RadioButtonListStatus = new RadioButtonList();
                RadioButtonListStatus.DataSource = statusData;
                RadioButtonListStatus.DataBind();
                RadioButtonListStatus.SelectedIndex = 0;
                proxy.Set("RadioButtonListStatus", RadioButtonListStatus);

                proxy.Set("ExtensionDataForm", null);
                #endregion

                string entityId = proxy.Create();
                ids.Add(new Guid(entityId));

                ConcreteDataObject obj = concreteDataApi.GetById(new Guid(entityId));
                Assert.AreEqual(obj.Name, "concrete" + surfix);

            }
        }

        [Test,Description("")]
        public void TestUpdate()
        {
            ConcreteDataDetailPanel page = new ConcreteDataDetailPanel();

            DetailPanelPageProxy proxy = new DetailPanelPageProxy(page);

            IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();
            using (var httpEnv = new HttpEnvironment())
            {
                Guid guid = Guid.NewGuid();

                string surfix = guid.ToString().Substring(0, 5);

                httpEnv.SetRequestUrl(@"/ConcreteDataDetailPanel/DynamicPage.svc?ConcreteDataType=Department");

                ConcreteDataObject obj = new ConcreteDataObject() 
                {
                    Name = "concrete" + surfix,
                    Value = "concrete" + surfix,
                    Type = "Department",
                    DeleteStatus = DeleteStatus.NotDeleted
                };

                concreteDataApi.Save(obj);

                #region Bind Control
                TextBox TextBoxName = new TextBox();
                TextBoxName.Text = "concrete" + surfix;
                proxy.Set("TextBoxName", TextBoxName);

                TextBox TextBoxValue = new TextBox();
                TextBoxValue.Text = "concreteUpdate" + surfix;
                proxy.Set("TextBoxValue", TextBoxValue);

                Array statusData = new string[] { "NotDeleted", "Deleted" };
                RadioButtonList RadioButtonListStatus = new RadioButtonList();
                RadioButtonListStatus.DataSource = statusData;
                RadioButtonListStatus.DataBind();
                RadioButtonListStatus.SelectedIndex = 0;
                proxy.Set("RadioButtonListStatus", RadioButtonListStatus);

                proxy.Set("ExtensionDataForm", null);
                #endregion

                proxy.Update(obj.ConcreteDataId.ToString());

                Assert.AreEqual(obj.Value, "concrete" + surfix);

                obj = concreteDataApi.GetById(obj.ConcreteDataId);

                Assert.AreEqual(obj.Value, "concreteUpdate" + surfix);

            }
        }
    }
}
