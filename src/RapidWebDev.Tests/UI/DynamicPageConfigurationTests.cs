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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using RapidWebDev.Common;
using RapidWebDev.Platform;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.Tests.UI
{
	[TestFixture(Description = "Test cases for parsing dynamic page xml element to configuration instances.")]
	public class DynamicPageConfigurationTests
	{
		private static DynamicPageConfiguration ResolveDynamicPageConfiguration(string dynamicPageFileName)
		{
			string xmlFile = string.Format(CultureInfo.InvariantCulture, "../../UI/Resources/DynamicPage/{0}", dynamicPageFileName);

			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load(xmlFile);

			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			xmlNamespaceManager.AddNamespace("p", "http://www.rapidwebdev.org/schemas/dynamicpage");

			string schemaXml = Kit.GetManifestFile(typeof(DynamicPageConfigurationParser), "DynamicPage.xsd");
			using (StringReader sr = new StringReader(schemaXml))
			{
				XmlParser xmlParser = new XmlParser(xmlNamespaceManager, XmlSchema.Read(sr, null));

				Kit.ValidateXml(xmlParser.Schema, xmldoc);
				XmlElement pageElement = xmldoc.SelectSingleNode("p:Page", xmlNamespaceManager) as XmlElement;
				return new DynamicPageConfiguration(pageElement, xmlParser);
			}
		}

		[Test(Description = "Parse an xml element with all properties to dynamic page configuration.")]
		public void ParseDynamicPageConfigurationWithAllPropertiesTest()
		{
			DynamicPageConfiguration dynamicPageConfiguration = ResolveDynamicPageConfiguration("UserManagement.pw.xml");
			Assert.AreEqual("User Management", dynamicPageConfiguration.Title);
			Assert.AreEqual("UserManagement", dynamicPageConfiguration.PermissionValue);
			Assert.AreEqual("UserManagement", dynamicPageConfiguration.ObjectId);
			Assert.IsNotNull(dynamicPageConfiguration.DynamicPageType);

			Assert.AreEqual(3, dynamicPageConfiguration.Panels.Count, "There should be 3 panels configured.");

			#region Assert Query Panel

			QueryPanelConfiguration queryPanel = dynamicPageConfiguration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.QueryPanel) as QueryPanelConfiguration;
			Assert.IsNotNull(queryPanel, "The query panel should not be null.");
			Assert.IsNull(queryPanel.Id);
			Assert.AreEqual("Query Users", queryPanel.HeaderText);

			Assert.AreEqual(5, queryPanel.Controls.Count, "There should be 5 query controls.");

			// UserName
			QueryFieldConfiguration queryFieldUserName = queryPanel.Controls.FirstOrDefault(ctrl => ctrl.FieldName == "UserName");
			Assert.IsNull(queryFieldUserName.FieldValueType);
			Assert.AreEqual(QueryFieldOperators.Auto, queryFieldUserName.Operator);
			TextBoxConfiguration textBoxUserName = queryFieldUserName.Control as TextBoxConfiguration;
			Assert.AreEqual("Name: ", textBoxUserName.Label);

			// DisplayName
			QueryFieldConfiguration queryFieldDisplayName = queryPanel.Controls.FirstOrDefault(ctrl => ctrl.FieldName == "DisplayName");
			Assert.IsNull(queryFieldDisplayName.FieldValueType);
			Assert.AreEqual(QueryFieldOperators.Auto, queryFieldDisplayName.Operator);
			TextBoxConfiguration textBoxDisplayName = queryFieldDisplayName.Control as TextBoxConfiguration;
			Assert.AreEqual("Display: ", textBoxDisplayName.Label);

			// Organization
			QueryFieldConfiguration queryFieldOrganization = queryPanel.Controls.FirstOrDefault(ctrl => ctrl.FieldName == "Organization.OrganizationName");
			Assert.IsNull(queryFieldOrganization.FieldValueType);
			Assert.AreEqual(QueryFieldOperators.Like, queryFieldOrganization.Operator);
			ComboBoxConfiguration comboBoxOrganization = queryFieldOrganization.Control as ComboBoxConfiguration;
			Assert.AreEqual("Organization: ", comboBoxOrganization.Label);

			ComboBoxDynamicDataSourceConfiguration dynamicDataSource = comboBoxOrganization.DynamicDataSource;
			Assert.IsNotNull(dynamicDataSource);
			Assert.AreEqual("OrganizationName", dynamicDataSource.TextField);
			Assert.AreEqual("OrganizationId", dynamicDataSource.ValueField);
			Assert.AreEqual("OrganizationName", dynamicDataSource.QueryParam);
			Assert.AreEqual("/Services/OrganizationServices.svc/Search", dynamicDataSource.Url);

			// Role
			QueryFieldConfiguration queryFieldRole = queryPanel.Controls.FirstOrDefault(ctrl => ctrl.FieldName == "UsersInRole.Role.RoleName");
			Assert.IsNull(queryFieldRole.FieldValueType);
			Assert.AreEqual(QueryFieldOperators.Like, queryFieldRole.Operator);
			ComboBoxConfiguration comboBoxRole = queryFieldRole.Control as ComboBoxConfiguration;
			Assert.AreEqual("Role: ", comboBoxRole.Label);

			Assert.IsNotNull(comboBoxRole.DynamicDataSource.XTemplate);
			Assert.AreEqual(@"<tpl for=""."" xmlns=""http://www.rapidwebdev.org/schemas/dynamicpage""><div class=""search-item""><h3>{topic_title}</h3></div></tpl>", comboBoxRole.DynamicDataSource.XTemplate);
			Assert.AreEqual("div.search-item", comboBoxRole.DynamicDataSource.ItemSelector);

			// Status
			QueryFieldConfiguration queryFieldStatus = queryPanel.Controls.FirstOrDefault(ctrl => ctrl.FieldName == "IsApproved");
			Assert.IsNull(queryFieldStatus.FieldValueType);
			Assert.AreEqual(QueryFieldOperators.Auto, queryFieldStatus.Operator);
			CheckBoxGroupConfiguration comboBoxStatus = queryFieldStatus.Control as CheckBoxGroupConfiguration;
			Assert.AreEqual("Status: ", comboBoxStatus.Label);

			CheckBoxGroupItemConfiguration comboBoxItemTrue = comboBoxStatus.Items.FirstOrDefault(item => item.Value == "True");
			Assert.AreEqual("<span style='color:green'>Approved</span>", comboBoxItemTrue.Text);
			Assert.IsTrue(comboBoxItemTrue.Checked);

			CheckBoxGroupItemConfiguration comboBoxItemFalse = comboBoxStatus.Items.FirstOrDefault(item => item.Value == "False");
			Assert.AreEqual("<span style='color:red'>Forbidden</span>", comboBoxItemFalse.Text);
			Assert.IsFalse(comboBoxItemFalse.Checked);

			#endregion

			#region Assert GridView Panel

			GridViewPanelConfiguration gridViewPanel = dynamicPageConfiguration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.GridViewPanel) as GridViewPanelConfiguration;
			Assert.IsNotNull(gridViewPanel, "The GridViewPanel should not be null.");
			Assert.AreEqual(true, gridViewPanel.EnabledCheckBoxField);
			Assert.AreEqual("Users Query Results", gridViewPanel.HeaderText);
			Assert.AreEqual(25, gridViewPanel.PageSize);
			Assert.AreEqual("UserId", gridViewPanel.PrimaryKeyFieldName);
			Assert.AreEqual("User", gridViewPanel.EntityName);

			Assert.IsNotNull(gridViewPanel.ViewButton);
			Assert.IsTrue(gridViewPanel.ViewButton.DisplayAsImage);

			Assert.IsNotNull(gridViewPanel.EditButton);
			Assert.IsTrue(gridViewPanel.EditButton.DisplayAsImage);

			Assert.IsNotNull(gridViewPanel.DeleteButton);
			Assert.IsTrue(gridViewPanel.DeleteButton.DisplayAsImage);

			Assert.AreEqual(7, gridViewPanel.Fields.Count, "There should be 7 gridview fields.");

			// UserName
			GridViewFieldConfiguration fieldUserName = gridViewPanel.Fields.FirstOrDefault(f => f.FieldName == "UserName");
			Assert.AreEqual("Name", fieldUserName.HeaderText);
			Assert.IsTrue(fieldUserName.Sortable);
			Assert.IsNull(fieldUserName.Transformer);

			// RoleNames
			GridViewFieldConfiguration fieldRoleNames = gridViewPanel.Fields.FirstOrDefault(f => f.FieldName == "RoleNames");
			Assert.AreEqual("Roles", fieldRoleNames.HeaderText);
			Assert.IsFalse(fieldRoleNames.Sortable);
			Assert.IsNull(fieldRoleNames.Transformer);

			// Organization
			GridViewFieldConfiguration fieldOrganization = gridViewPanel.Fields.FirstOrDefault(f => f.FieldName == "OrganizationId");
			Assert.AreEqual("Organization", fieldOrganization.HeaderText);
			Assert.IsNotNull(fieldOrganization.Transformer);

			// Status
			GridViewFieldConfiguration fieldStatus = gridViewPanel.Fields.FirstOrDefault(f => f.FieldName == "IsApproved");
			Assert.AreEqual("Status", fieldStatus.HeaderText);
			Assert.IsNotNull(fieldStatus.Transformer);
			Assert.AreEqual(GridViewFieldValueTransformTypes.Switch, fieldStatus.Transformer.TransformType);
			Assert.AreEqual(2, fieldStatus.Transformer.SwitchCases.Count);

			GridViewFieldValueTransformSwitchCase switchCaseTrue = fieldStatus.Transformer.SwitchCases.FirstOrDefault(sc => sc.Value == "True");
			Assert.IsNotNull(switchCaseTrue);
			Assert.AreEqual(false, switchCaseTrue.CaseSensitive);
			Assert.AreEqual("<span style=\"color:green\">Approved</span>", switchCaseTrue.Output);

			Assert.AreEqual(70, fieldStatus.Width);

			// Last Updated Date
			GridViewFieldConfiguration fieldLastUpdatedDate = gridViewPanel.Fields.FirstOrDefault(f => f.FieldName == "LastUpdatedDate");
			Assert.AreEqual("Last Updated", fieldLastUpdatedDate.HeaderText);
			Assert.IsNotNull(fieldLastUpdatedDate.Transformer);
			Assert.AreEqual(GridViewFieldValueTransformTypes.ToStringParameter, fieldLastUpdatedDate.Transformer.TransformType);
			Assert.AreEqual("{0:yyyy/MM/dd HH:mm}", fieldLastUpdatedDate.Transformer.ToStringParameter);

			#endregion

			#region Assert Detail Panel

			DetailPanelConfiguration detailPanel = dynamicPageConfiguration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.DetailPanel) as DetailPanelConfiguration;
			Assert.IsNotNull(detailPanel);
			Assert.IsNull(detailPanel.Id);
			Assert.AreEqual("User Profile", detailPanel.HeaderText);
			Assert.AreEqual("~/Membership/Templates/User.ascx", detailPanel.SkinPath);
			Assert.IsNotNull(detailPanel.SaveAndCloseButton);
			Assert.IsNotNull(detailPanel.CancelButton);

			#endregion
		}

		[Test(Description = "Parse dynamic page configuration with variables.")]
		public void ParseDynamicPageConfigurationWithVariables()
		{
			IAuthenticationContext applicationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
			applicationContext.TempVariables.Add("Domain", "Customer");

			DynamicPageConfiguration dynamicPageConfiguration = ResolveDynamicPageConfiguration("UserManagement.WithVariables.pw.xml");
			Assert.AreEqual("Customer User Management", dynamicPageConfiguration.Title);
			Assert.AreEqual("CustomerUserManagement", dynamicPageConfiguration.PermissionValue);

			QueryPanelConfiguration queryPanel = dynamicPageConfiguration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.QueryPanel) as QueryPanelConfiguration;
			Assert.AreEqual("Query Customer Users", queryPanel.HeaderText);

			GridViewPanelConfiguration gridViewPanel = dynamicPageConfiguration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.GridViewPanel) as GridViewPanelConfiguration;
			Assert.AreEqual("Customer Users Query Results", gridViewPanel.HeaderText);

			DetailPanelConfiguration detailPanel = dynamicPageConfiguration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.DetailPanel) as DetailPanelConfiguration;
			Assert.AreEqual("Customer User Profile", detailPanel.HeaderText);

			applicationContext.TempVariables.Remove("Domain");
		}
	}
}
