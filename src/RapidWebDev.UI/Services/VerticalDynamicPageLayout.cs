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
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// The implementation to render panels of dynamic page as vertical layout.
	/// </summary>
	public class VerticalDynamicPageLayout : IDynamicPageLayout
	{
		/// <summary>
		/// Create the control contains all components for the dynamic page configuration.
		/// </summary>
		/// <param name="dynamicPageConfiguration"></param>
		/// <returns></returns>
		public Control Create(DynamicPageConfiguration dynamicPageConfiguration)
		{
			PlaceHolder placeHolder = new PlaceHolder();

			bool isTopPanel = true;
			foreach (BasePanelConfiguration basePanelConfiguration in dynamicPageConfiguration.Panels)
			{
				WebControl createdControl = null;
				switch (basePanelConfiguration.PanelType)
				{
					case DynamicPagePanelTypes.ButtonPanel:
						createdControl = CreateButtonPanel(basePanelConfiguration);
						break;

					case DynamicPagePanelTypes.GridViewPanel:
						createdControl = CreateGridViewPanel(basePanelConfiguration, dynamicPageConfiguration);
						createdControl.Style["margin-top"] = isTopPanel ? "2px" : "4px";
						break;

					case DynamicPagePanelTypes.QueryPanel:
						createdControl = CreateQueryPanel(basePanelConfiguration);
						createdControl.Style["margin-top"] = isTopPanel ? "2px" : "4px";
						break;
				}

				if (createdControl != null)
				{
					isTopPanel = false;
					placeHolder.Controls.Add(createdControl);
				}
			}

			return placeHolder;
		}

		private static WebControl CreateButtonPanel(BasePanelConfiguration panelConfiguration)
		{
			Panel buttonPanelContainer = null;
			ButtonPanelConfiguration configuration = panelConfiguration as ButtonPanelConfiguration;
			if (configuration != null)
			{
				buttonPanelContainer = new Panel { CssClass = "buttonpanel" };
				buttonPanelContainer.Style["margin-top"] = "1px";

				ButtonPanel buttonPanel = new ButtonPanel
				{
					ID = string.Format(CultureInfo.InvariantCulture, "ButtonPanel_{0}_{1}", QueryStringUtility.ObjectId, configuration.Id),
					Configuration = configuration
				};

				buttonPanelContainer.Controls.Add(buttonPanel);
			}

			return buttonPanelContainer;
		}

		private static WebControl CreateGridViewPanel(BasePanelConfiguration panelConfiguration, DynamicPageConfiguration dynamicPageConfiguration)
		{
			Panel gridPanelContainer = null;
			GridViewPanelConfiguration configuration = panelConfiguration as GridViewPanelConfiguration;

			if (configuration != null)
			{
				gridPanelContainer = new Panel { CssClass = "gridviewpanel" };

				GridViewPanel gridViewPanel = new GridViewPanel
				{
					ID = string.Format(CultureInfo.InvariantCulture, "GridViewPanel_{0}_{1}", QueryStringUtility.ObjectId, configuration.Id),
					Configuration = configuration,
					AsUniqueGridView = true,
					DetailPanelPlugin = ResolveDetailPanelConfiguration(dynamicPageConfiguration),
					AggregatePanelPlugin = ResolveAggregatePanelConfiguration(dynamicPageConfiguration)
				};

				gridPanelContainer.Controls.Add(gridViewPanel);
			}

			return gridPanelContainer;
		}

		private static WebControl CreateQueryPanel(BasePanelConfiguration panelConfiguration)
		{
			Panel queryPanelContainer = null;
			QueryPanelConfiguration configuration = panelConfiguration as QueryPanelConfiguration;
			if (configuration != null)
			{
				queryPanelContainer = new Panel { CssClass = "querypanel" };

				QueryPanel queryPanel = new QueryPanel
				{
					ID = string.Format(CultureInfo.InvariantCulture, "QueryPanel_{0}_{1}", QueryStringUtility.ObjectId, configuration.Id),
					Configuration = configuration
				};

				queryPanelContainer.Controls.Add(queryPanel);
			}

			return queryPanelContainer;
		}

		private static GridViewPanelPluginConfiguration4DetailPanel ResolveDetailPanelConfiguration(DynamicPageConfiguration dynamicPageConfiguration)
		{
			AggregatePanelConfiguration configuration = dynamicPageConfiguration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.DetailPanel) as AggregatePanelConfiguration;
			if (configuration == null)
				return new GridViewPanelPluginConfiguration4DetailPanel();

			return new GridViewPanelPluginConfiguration4DetailPanel
			{
				Draggable = configuration.Draggable,
				Resizable = configuration.Resizable,
				Height = configuration.Height,
				Width = configuration.Width,
				EditableHeaderText = configuration.HeaderText,
				ViewableHeaderText = configuration.HeaderText
			};
		}

		private static GridViewPanelPluginConfiguration4AggregatePanel ResolveAggregatePanelConfiguration(DynamicPageConfiguration dynamicPageConfiguration)
		{
			AggregatePanelConfiguration configuration = dynamicPageConfiguration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.AggregatePanel) as AggregatePanelConfiguration;
			if (configuration == null)
				return new GridViewPanelPluginConfiguration4AggregatePanel();

			return new GridViewPanelPluginConfiguration4AggregatePanel
			{
				Draggable = configuration.Draggable,
				Resizable = configuration.Resizable,
				Height = configuration.Height,
				Width = configuration.Width,
				HeaderText = configuration.HeaderText
			};
		}
	}
}