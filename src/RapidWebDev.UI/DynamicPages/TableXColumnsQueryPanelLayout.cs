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
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Factory to create table layout with 6 columns. 
	/// All the query field controls will be rendered into table cells in pipeline. 
	/// If the cells of a table row is fully occupied, the following control will be rendered into next row. 
	/// </summary>
	public class TableXColumnsQueryPanelLayout : IQueryPanelLayout
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TableXColumnsQueryPanelLayout()
		{
			this.ColumnsNumber = 6;
		}

		/// <summary>
		/// Sets/gets column number of layout table. Defaults to 6.
		/// </summary>
		public int ColumnsNumber { get; set; }

		/// <summary>
		/// Create layout control which contains all query field controls from configurations.
		/// </summary>
		/// <param name="queryFieldControlsByConfigurations"></param>
		/// <returns></returns>
		public Control Create(IEnumerable<KeyValuePair<QueryFieldConfiguration, IEnumerable<IQueryFieldControl>>> queryFieldControlsByConfigurations)
		{
			Panel layoutPanel = new Panel();

			HtmlTable table = new HtmlTable { Width = "100%", CellPadding = 0, CellSpacing = 0 };
			table.Attributes["class"] = "layout";
			layoutPanel.Controls.Add(table);

			HtmlTableRow tableRow = new HtmlTableRow();
			table.Rows.Add(tableRow);

			HtmlTableCell lastControlCell = null;
			int occupiedCellsOfThisLine = 0;
			foreach (KeyValuePair<QueryFieldConfiguration, IEnumerable<IQueryFieldControl>> queryFieldControlsByConfiguration in queryFieldControlsByConfigurations)
			{
				QueryFieldConfiguration queryFieldConfiguration = queryFieldControlsByConfiguration.Key;
				IEnumerable<IQueryFieldControl> queryFieldControls = queryFieldControlsByConfiguration.Value;

				int queryControlOccupiedCells = queryFieldConfiguration.Occupation * 2;
				if (queryControlOccupiedCells > this.ColumnsNumber)
				{
					if (occupiedCellsOfThisLine > 0)
					{
						if (lastControlCell != null && this.ColumnsNumber - occupiedCellsOfThisLine > 0)
							lastControlCell.ColSpan += this.ColumnsNumber - occupiedCellsOfThisLine;

						tableRow = new HtmlTableRow();
						table.Rows.Add(tableRow);
					}

					occupiedCellsOfThisLine = this.ColumnsNumber;
					queryControlOccupiedCells = Math.Min(queryControlOccupiedCells, this.ColumnsNumber);
				}
				else if (queryControlOccupiedCells + occupiedCellsOfThisLine > this.ColumnsNumber)
				{
					if (lastControlCell != null && this.ColumnsNumber - occupiedCellsOfThisLine > 0)
						lastControlCell.ColSpan += this.ColumnsNumber - occupiedCellsOfThisLine;

					occupiedCellsOfThisLine = queryControlOccupiedCells;
					tableRow = new HtmlTableRow();
					table.Rows.Add(tableRow);
				}
				else
					occupiedCellsOfThisLine += queryControlOccupiedCells;

				lastControlCell = this.CreateControlCells(layoutPanel, tableRow, queryFieldConfiguration, queryFieldControls, queryControlOccupiedCells);
			}

			AddBlankTableCells(tableRow, occupiedCellsOfThisLine);
			return layoutPanel;
		}

		private HtmlTableCell CreateControlCells(Panel layoutPanel, HtmlTableRow row, QueryFieldConfiguration queryFieldConfiguration, IEnumerable<IQueryFieldControl> controls, int queryControlOccupiedCells)
		{
			HtmlTableCell labelCell = new HtmlTableCell("td");
			labelCell.Attributes["class"] = "c1";
			labelCell.NoWrap = true;
			labelCell.InnerText = queryFieldConfiguration.Control.Label;
			row.Cells.Add(labelCell);

			HtmlTableCell controlCell = new HtmlTableCell("td");
			HtmlTableCell lastControlCell = controlCell;
			controlCell.Attributes["class"] = "c2";
			controlCell.NoWrap = true;
			if (queryControlOccupiedCells > 1)
				controlCell.ColSpan = queryControlOccupiedCells - 1;

			row.Cells.Add(controlCell);

			if (controls.Count() == 1)
			{
				controlCell.Controls.Add(controls.FirstOrDefault() as Control);
			}
			else
			{
				HtmlTable controlContainerTable = new HtmlTable { CellPadding = 0, CellSpacing = 0 }; ;
				controlCell.Controls.Add(controlContainerTable);

				HtmlTableRow controlContainerRow = new HtmlTableRow();
				controlContainerTable.Rows.Add(controlContainerRow);

				bool addFirstQueryFieldControl = true;
				foreach (IQueryFieldControl queryFieldControl in controls)
				{
					if (!addFirstQueryFieldControl)
					{
						HtmlTableCell controlSeparatorCell = new HtmlTableCell { InnerHtml = string.Format(CultureInfo.InvariantCulture, " {0} ", Resources.DPCtrl_QueryFieldControl_Between_Separator), Width = "16px", Align = "center" };
						controlContainerRow.Cells.Add(controlSeparatorCell);
					}

					HtmlTableCell controlContainerCell = new HtmlTableCell();
					controlContainerRow.Cells.Add(controlContainerCell);
					controlContainerCell.Controls.Add(queryFieldControl as Control);
					addFirstQueryFieldControl = false;
				}
			}

			return lastControlCell;
		}

		private void AddBlankTableCells(HtmlTableRow tableRow, int occupiedCellsOfThisLine)
		{
			int cellPairNumber = (this.ColumnsNumber - occupiedCellsOfThisLine) / 2;
			for (int i = 0; i < cellPairNumber; i++)
			{
				string spacerImageUrl = Kit.ResolveAbsoluteUrl("~/Resources/Images/spacer.gif");

				HtmlTableCell labelCell = new HtmlTableCell("td");
				labelCell.Attributes["class"] = "c1";
				labelCell.NoWrap = true;
				labelCell.Controls.Add(new LiteralControl("<img src='" + spacerImageUrl + "' alt='' />"));
				tableRow.Cells.Add(labelCell);

				HtmlTableCell controlCell = new HtmlTableCell("td");
				controlCell.Attributes["class"] = "c2";
				controlCell.NoWrap = true;
				controlCell.Controls.Add(new LiteralControl("<img src='" + spacerImageUrl + "' alt='' />"));
				tableRow.Cells.Add(controlCell);
			}
		}
	}
}

