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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web.UI.WebControls;
using RapidWebDev.UI.Properties;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Query panel configuration.
	/// </summary>
	public class QueryPanelConfiguration : BasePanelConfiguration
	{
		/// <summary>
		/// Collection of query field controls in the panel. The default value is a empty collection.
		/// </summary>
		public Collection<QueryFieldConfiguration> Controls { get; set; }

		/// <summary>
		/// Gets panel type
		/// </summary>
		public override DynamicPagePanelTypes PanelType { get { return DynamicPagePanelTypes.QueryPanel; } }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public QueryPanelConfiguration() : base()
		{
			this.Controls = new Collection<QueryFieldConfiguration>();
		}

		/// <summary>
		/// Construct QueryPanelConfiguration instance from xml element.
		/// </summary>
		/// <param name="queryPanelElement"></param>
		/// <param name="xmlParser"></param>
		public QueryPanelConfiguration(XmlElement queryPanelElement, XmlParser xmlParser) : base(queryPanelElement, xmlParser)
		{
			this.Controls = new Collection<QueryFieldConfiguration>();
			foreach (XmlNode controlNode in queryPanelElement.ChildNodes)
			{
				XmlElement controlElement = controlNode as XmlElement;
				if (controlElement != null)
					this.Controls.Add(new QueryFieldConfiguration(controlElement, xmlParser));
			}			
		}
	}
}

