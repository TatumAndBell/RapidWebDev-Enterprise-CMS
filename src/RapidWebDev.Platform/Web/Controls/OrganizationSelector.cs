/****************************************************************************************************
    Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
    Author: Eunge, Legal Name: Jian Liu, Email: eunge.liu@RapidWebDev.org

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ****************************************************************************************************/

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages.Configurations;
using MyControls = RapidWebDev.UI.Controls;

namespace RapidWebDev.Platform.Web.Controls
{
	/// <summary>
	/// Organization selector web control.
	/// </summary>
	[ToolboxData("<{0}:OrganizationSelector runat=\"server\"/>")]
	public class OrganizationSelector : System.Web.UI.WebControls.WebControl, INamingContainer, IPostBackDataHandler, ITextControl
	{
		private static IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();
		private static IOrganizationApi organizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
		private MyControls.ComboBox comboBox;
		private OrganizationObject selectedOrganizationObject;

		/// <summary>
		/// Sets/gets max items rendered as selection candidate, defaults to 25.
		/// </summary>
		public int MaxItemCount
		{
			get { if (base.ViewState["MaxItemCount"] == null) return 25; return (int)base.ViewState["MaxItemCount"]; }
			set { base.ViewState["MaxItemCount"] = value; }
		}

		/// <summary>
		/// Sets/gets domain which the selectable organizations belong to.
		/// </summary>
		public string Domain
		{
			get { return base.ViewState["Domain"] as string; }
			set { base.ViewState["Domain"] = value; }
		}

		/// <summary>
		/// Sets/gets organization type which the selectable organizations should belong to.
		/// </summary>
		public Guid OrganizationTypeId
		{
			get
			{
				if (base.ViewState["OrganizationTypeId"] == null) return Guid.Empty;
				return (Guid)base.ViewState["OrganizationTypeId"]; ;
			}
			set { base.ViewState["OrganizationTypeId"] = value; }
		}

		/// <summary>
		/// Sets/gets client event when the selected organization changed. 
		/// The signature of javascript callback method should be as: "function MethodName(string newValue, string oldValue) { }"
		/// </summary>
		public string SelectedOrganizationChangedCallback
		{
			get
			{
				if (Kit.IsEmpty(base.ViewState["SelectedOrganizationChangedCallback"]))
					return null;

				return base.ViewState["SelectedOrganizationChangedCallback"] as string;
			}
			set { base.ViewState["SelectedOrganizationChangedCallback"] = value; }
		}

		/// <summary>
		/// Sets/gets selected organization.
		/// </summary>
		public OrganizationObject SelectedOrganization
		{
			get { return this.selectedOrganizationObject; }
			set { this.selectedOrganizationObject = value; }
		}

		/// <summary>
		/// Text of the control.
		/// </summary>
		public string Text
		{
			get
			{
				if (this.selectedOrganizationObject != null) return this.selectedOrganizationObject.OrganizationName;
				return null;
			}
			set { }
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.Load event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.Page.RegisterRequiresPostBack(this);
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based 
		/// implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			if (!platformConfiguration.Domains.Select(d => d.Value).Contains(this.Domain))
				throw new InvalidProgramException(string.Format("The specified property Domain [{0}] is invalid.", this.Domain));

			this.comboBox = new MyControls.ComboBox
			{
				ID = "ComboBox",
				TextField = "OrganizationName",
				ValueField = "OrganizationId",
				MinChars = 1,
				Mode = MyControls.ComboBoxDataSourceModes.Remote,
				Root = "",
				ItemSelector = ".search-item",
				QueryParam = "q",
				Proxy = DataProxyTypes.HttpProxy,
				Url = Kit.ResolveAbsoluteUrl("~/Services/OrganizationService.svc/json/search"),
				XTemplate = @"<tpl for="".""><div class=""search-item""><div class=""subject"">({OrganizationCode}) {OrganizationName}</div>{Description}</div></tpl>",
				ExtraFields = new string[] { "OrganizationCode", "Description" },
				Enabled = this.Enabled
			};

			this.comboBox.Params = new Collection<ComboBoxDynamicDataSourceParamConfiguration> 
            {
                new ComboBoxDynamicDataSourceParamConfiguration("start") { Value = "0" },
                new ComboBoxDynamicDataSourceParamConfiguration("limit") { Value = this.MaxItemCount.ToString() },
                new ComboBoxDynamicDataSourceParamConfiguration("sortfield") { Value = "OrganizationName" },
                new ComboBoxDynamicDataSourceParamConfiguration("sortDirection") { Value = "ASC" },
                new ComboBoxDynamicDataSourceParamConfiguration("domain") { Value = this.Domain },
            };

			if (this.OrganizationTypeId != Guid.Empty)
				this.comboBox.Params.Add(new ComboBoxDynamicDataSourceParamConfiguration("orgtypeid") { Value = this.OrganizationTypeId.ToString() });

			this.Controls.Add(this.comboBox);
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			this.comboBox.SelectedItemChangedCallback = this.SelectedOrganizationChangedCallback;

			if (this.selectedOrganizationObject != null)
			{
				this.comboBox.Items.Clear();
				this.comboBox.Items.Add(new ListItem(this.selectedOrganizationObject.OrganizationName, this.selectedOrganizationObject.OrganizationId.ToString()));
				this.comboBox.SelectedValue = this.selectedOrganizationObject.OrganizationId.ToString();
				this.comboBox.SelectedText = this.selectedOrganizationObject.OrganizationName;
			}
			else
			{
				this.comboBox.SelectedValue = null;
				this.comboBox.SelectedText = null;
			}
		}

		#region IPostBackDataHandler Members

		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			string comboboxKey = string.Format(CultureInfo.InvariantCulture, "{0}$ComboBox", postDataKey);
			string selectedItemValue = postCollection[comboboxKey];
			if (!string.IsNullOrEmpty(selectedItemValue))
			{
				Guid organizationId = new Guid(selectedItemValue);
				this.selectedOrganizationObject = organizationApi.GetOrganization(organizationId);
			}

			return true;
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{

		}

		#endregion
	}
}

