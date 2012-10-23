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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using RapidWebDev.Common.CodeDom;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Class to configure gridview control.
	/// </summary>
	public sealed class GridViewPanelConfiguration : BasePanelConfiguration
	{
		private string entityName;

		/// <summary>
		/// Set/get entity name.
		/// </summary>
		public string EntityName
		{
			set { this.entityName = value; }
			get { return WebUtility.ReplaceVariables(this.entityName); }
		}

		/// <summary>
		/// Set/get primary key field name.
		/// </summary>
		public string PrimaryKeyFieldName { get; set; }

		/// <summary>
		/// Set/get gridview binding fields.
		/// </summary>
		public Collection<GridViewFieldConfiguration> Fields { get; set; }

		/// <summary>
		/// A config that will be used to create the grid's UI row view.
		/// </summary>
		public GridViewFieldRowViewConfiguration RowView { get; set; }

		/// <summary>
		/// Set/get true to enable checkbox field as the first column of gridview.
		/// </summary>
		public bool EnabledCheckBoxField { get; set; }

		/// <summary>
		/// Set/get True to execute query when the web page is loaded. The default value is true.
		/// </summary>
		public bool ExecuteQueryWhenLoaded { get; set; }

		/// <summary>
		/// True to enable drag and drop reorder of columns (defaults to false).
		/// </summary>
		public bool EnabledColumnMove { get; set; }

		/// <summary>
		/// False to turn off column resizing for the whole grid (defaults to false).
		/// </summary>
		public bool EnabledColumnResize { get; set; }

		/// <summary>
		/// Set/get gridview paging size. The default size is 25.
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// Sets/gets configuration instance of view button.
		/// </summary>
		public GridViewRowButtonConfiguration ViewButton { get; set; }

		/// <summary>
		/// Sets/gets configuration instance of edit button.
		/// </summary>
		public GridViewRowButtonConfiguration EditButton { get; set; }

		/// <summary>
		/// Sets/gets configuration instance of delete button.
		/// </summary>
		public GridViewRowButtonConfiguration DeleteButton { get; set; }

		/// <summary>
		/// Sets/gets height of gridview panel. (defaults to 500)
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Sets/gets the default sort field name which should be included in Fields.
		/// </summary>
		public string DefaultSortField { get; set; }

		/// <summary>
		/// Sets/gets  the candidate values are ASC | DESC (defaults to ASC).
		/// </summary>
		public GridViewFieldSortDirection DefaultSortDirection { get; set; }

		/// <summary>
		/// Gets panel type
		/// </summary>
		public override DynamicPagePanelTypes PanelType { get { return DynamicPagePanelTypes.GridViewPanel; } }

		/// <summary>
		/// Construct GridView Configuration
		/// </summary>
		public GridViewPanelConfiguration() : base()
		{
			this.ExecuteQueryWhenLoaded = true;
			this.Fields = new Collection<GridViewFieldConfiguration>();
		}

		/// <summary>
		/// Construct GridView Configuration
		/// </summary>
		/// <param name="gridViewElement"></param>
		/// <param name="xmlParser"></param>
		public GridViewPanelConfiguration(XmlElement gridViewElement, XmlParser xmlParser) : base(gridViewElement, xmlParser)
		{
			this.EntityName = xmlParser.ParseString(gridViewElement, "@EntityName");
			this.PrimaryKeyFieldName = xmlParser.ParseString(gridViewElement, "@PrimaryKeyFieldName");
			this.PageSize = xmlParser.ParseInt(gridViewElement, "@PageSize", 25);
			this.EnabledCheckBoxField = xmlParser.ParseBoolean(gridViewElement, "@EnabledCheckBoxField", false);
			this.ExecuteQueryWhenLoaded = xmlParser.ParseBoolean(gridViewElement, "@ExecuteQueryWhenLoaded", true);
			this.EnabledColumnMove = xmlParser.ParseBoolean(gridViewElement, "@EnabledColumnMove", false);
			this.EnabledColumnResize = xmlParser.ParseBoolean(gridViewElement, "@EnabledColumnResize", false);
			this.Height = xmlParser.ParseInt(gridViewElement, "@Height", 500);
			this.DefaultSortField = xmlParser.ParseString(gridViewElement, "@DefaultSortField");
			this.DefaultSortDirection = xmlParser.ParseEnum<GridViewFieldSortDirection>(gridViewElement, "@DefaultSortDirection");

			XmlElement viewButtomElement = gridViewElement.SelectSingleNode("p:ViewButton", xmlParser.NamespaceManager) as XmlElement;
			if (viewButtomElement != null)
				this.ViewButton = new GridViewRowButtonConfiguration(viewButtomElement, xmlParser);

			XmlElement editButtomElement = gridViewElement.SelectSingleNode("p:EditButton", xmlParser.NamespaceManager) as XmlElement;
			if (editButtomElement != null)
				this.EditButton = new GridViewRowButtonConfiguration(editButtomElement, xmlParser);

			XmlElement deleteButtomElement = gridViewElement.SelectSingleNode("p:DeleteButton", xmlParser.NamespaceManager) as XmlElement;
			if (deleteButtomElement != null)
				this.DeleteButton = new GridViewRowButtonConfiguration(deleteButtomElement, xmlParser);

			this.Fields = new Collection<GridViewFieldConfiguration>();
			XmlNodeList fieldNodes = gridViewElement.SelectNodes("p:Fields/p:Field", xmlParser.NamespaceManager);
			foreach (XmlElement fieldElement in fieldNodes.Cast<XmlElement>())
				this.Fields.Add(new GridViewFieldConfiguration(fieldElement, xmlParser));

			XmlElement rowViewElement = gridViewElement.SelectSingleNode("p:Fields/p:RowView", xmlParser.NamespaceManager) as XmlElement;
			if (rowViewElement != null)
				this.RowView = new GridViewFieldRowViewConfiguration(rowViewElement, xmlParser);
		}
	}

	/// <summary>
	/// Sort direction of GridView field.
	/// </summary>
	public enum GridViewFieldSortDirection
	{
		/// <summary>
		/// Ascending
		/// </summary>
		ASC = 0,
		
		/// <summary>
		/// Descending
		/// </summary>
		DESC = 1
	}
}
