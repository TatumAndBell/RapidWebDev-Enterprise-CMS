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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
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
using System.Globalization;
using RapidWebDev.UI.Properties;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Class to configure gridview field.
	/// </summary>
	public sealed class GridViewFieldConfiguration
	{
		private static readonly HashSet<string> SUPPORT_TRANSFORM_TYPES = new HashSet<string> 
		{ 
			"Transform-Format", 
			"Transform-Callback", 
			"Transform-Switch", 
			"Transform-Inline",
			"Transform-ToString-Parameter"
		};

		private string headerText;
		private string sortingFieldName;

		/// <summary>
		/// Sets/Gets field name
		/// </summary>
		public string FieldName { get; set; }

		/// <summary>
		/// Sets/Gets sorting field name. If property value is not specified, it uses FieldName as backup.
		/// </summary>
		public string SortingFieldName
		{
			set { this.sortingFieldName = value; }
			get
			{
				if (!string.IsNullOrEmpty(this.sortingFieldName))
					return this.sortingFieldName;

				return this.FieldName;
			}
		}

		/// <summary>
		/// Sets/Gets field label
		/// </summary>
		public string HeaderText
		{
			private set { this.headerText = value; }
			get { return WebUtility.ReplaceVariables(this.headerText); }
		}

		/// <summary>
		/// Sets/gets gridview field value transformation instance.
		/// </summary>
		public GridViewFieldValueTransformConfiguration Transformer { get; set; }

		/// <summary>
		/// If not specified, the default renderer uses the raw data value from server. <br/>
		/// The function to use to process the cell's raw data to return HTML markup for the grid view.<br/>
		/// The render function is called with the following parameters:<br/>
		/// # value : Object, the data value for the cell.<br/>
		/// # metadata : Object, an object in which you may set the following attributes:<br/>
		///		* css : String, A CSS class name to add to the cell's TD element.<br/>
		///		* attr : String, An HTML attribute definition string to apply to the data container element within the table cell (e.g. 'style="color:red;"').<br/>
		///	# record : Ext.data.record, The Ext.data.Record from which the data was extracted.<br/>
		///	# rowIndex : Number, Row index<br/>
		///	# colIndex : Number, Column index<br/>
		///	# store : Ext.data.Store, The Ext.data.Store object from which the Record was extracted.<br/>
		///	The more description about the callback JavaScript function refer to ExtJs API Documentation, ColumnModel ->  setRenderer<br/>
		/// </summary>
		/// <example>
		/// The example of property value is, <br/>
		/// function (value, metadata, record, rowIndex, colIndex, store) { alert("The method is being invoked."); }
		/// </example>
		public string ExtJsRenderer { get; set; }

		/// <summary>
		/// Sets/gets true when the field is sortable. The default value is TRUE.
		/// </summary>
		public bool Sortable { get; set; }

		/// <summary>
		/// False to disable column resizing. Defaults to true.
		/// </summary>
		public bool Resizable { get; set; }

		/// <summary>
		/// Sets/gets custom CSS for all table cells in the column (excluding headers). Defaults to undefined.
		/// </summary>
		public string Css { get; set; }

		/// <summary>
		/// Sets/gets the initial width in pixels of the column (defaults to auto).
		/// </summary>
		public int? Width { get; set; }

		/// <summary>
		/// (optional) Set the CSS text-align property of the column. Defaults to NotSet. 
		/// </summary>
		public HorizontalAlign Align { get; set; }

		/// <summary>
		/// (optional) True to hide the column initially. Defaults to false. 
		/// </summary>
		public bool Hidden { get; set; }

		/// <summary>
		/// Construct Basic Data Field instance.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="headerText"></param>
		public GridViewFieldConfiguration(string fieldName, string headerText)
		{
			this.FieldName = fieldName;
			this.HeaderText = headerText;
			this.Sortable = true;
			this.Resizable = true;
		}

		/// <summary>
		/// Construct GridViewFieldConfiguration instance.
		/// </summary>
		/// <param name="gridViewFieldElement"></param>
		/// <param name="xmlParser"></param>
		public GridViewFieldConfiguration(XmlElement gridViewFieldElement, XmlParser xmlParser)
		{
			this.FieldName = xmlParser.ParseString(gridViewFieldElement, "@FieldName");
			this.HeaderText = xmlParser.ParseString(gridViewFieldElement, "@HeaderText");
			this.SortingFieldName = xmlParser.ParseString(gridViewFieldElement, "@SortingFieldName");
			this.Sortable = xmlParser.ParseBoolean(gridViewFieldElement, "@Sortable", true);
			this.Resizable = xmlParser.ParseBoolean(gridViewFieldElement, "@Resizble", true);
			this.Css = xmlParser.ParseString(gridViewFieldElement, "@Css");
			this.Width = xmlParser.ParseInt(gridViewFieldElement, "@Width", null);
			this.ExtJsRenderer = xmlParser.ParseString(gridViewFieldElement, "p:ExtJs-Renderer");
			this.Align = xmlParser.ParseEnum<HorizontalAlign>(gridViewFieldElement, "p:Align");
			this.Hidden = xmlParser.ParseBoolean(gridViewFieldElement, "@Hidden", false);

			foreach (XmlElement transformNode in gridViewFieldElement.ChildNodes)
			{
				XmlElement transformElement = transformNode as XmlElement;
				if (transformElement != null && SUPPORT_TRANSFORM_TYPES.Contains(transformElement.LocalName))
					this.Transformer = new GridViewFieldValueTransformConfiguration(transformElement, xmlParser);
			}
		}
	}
}
