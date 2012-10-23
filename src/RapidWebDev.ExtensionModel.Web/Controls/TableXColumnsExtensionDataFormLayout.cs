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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using RapidWebDev.Common;
using RapidWebDev.UI;
using RapidWebDev.ExtensionModel.Web.Properties;

namespace RapidWebDev.ExtensionModel.Web.Controls
{
	/// <summary>
	/// The interface to generate layout of data input form for the special metadata type.
	/// </summary>
	public class TableXColumnsExtensionDataFormLayout : IExtensionDataFormLayout
	{
		/// <summary>
		/// Append a css class to each table row, defaults to empty.
		/// </summary>
		public string TableRowCssClass { get; set; }

		/// <summary>
		/// True to render &lt;table&gt; html element wraps the controls within table rows, defaults to false.
		/// </summary>
		public bool RenderTableElement { get; set; }

		/// <summary>
		/// Append a css class to the table, defaults to empty.
		/// </summary>
		public string TableCssClass { get; set; }

		/// <summary>
		/// Sets/gets column number of layout table, defaults to 6.
		/// </summary>
		public int ColumnsNumber { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public TableXColumnsExtensionDataFormLayout()
		{
			this.TableRowCssClass = "";
			this.ColumnsNumber = 6;
		}

		/// <summary>
		/// Create layout control which contains data input form for the special metadata type.
		/// </summary>
		/// <param name="extensionFieldControlBuildersByFieldMetadata">Extension field control builders by field metadata.</param>
		/// <returns></returns>
		public Control Create(IEnumerable<KeyValuePair<IFieldMetadata, IExtensionFieldControlBuilder>> extensionFieldControlBuildersByFieldMetadata)
		{
			Control placeHolder = this.CreateTableRowsByGroup(extensionFieldControlBuildersByFieldMetadata);
			if (!this.RenderTableElement) return placeHolder;

			HtmlTable table = new HtmlTable { CellPadding = 0, CellSpacing = 0 };
			table.Attributes["class"] = this.TableCssClass;

			foreach (HtmlTableRow tableRow in placeHolder.Controls)
				table.Rows.Add(tableRow);

			return table;
		}

		private Control CreateTableRowsByGroup(IEnumerable<KeyValuePair<IFieldMetadata, IExtensionFieldControlBuilder>> extensionFieldControlBuildersByFieldMetadata)
		{
			// separate extension fields by field groups
			var extensionFieldsByFieldGroup = extensionFieldControlBuildersByFieldMetadata.GroupBy(g => g.Key.FieldGroup, g => g);

			// sort groups by the minimum field ordinal in the group.
			extensionFieldsByFieldGroup = extensionFieldsByFieldGroup.OrderBy(g => g.Min(kvp => kvp.Key.Ordinal));

			PlaceHolder groupContainer = new PlaceHolder();
			int extensionFieldGroupCount = extensionFieldsByFieldGroup.Count();
			extensionFieldsByFieldGroup.ToList().ForEach(g =>
				{
					Control rowContainerControl = this.CreateTableRows(g.OrderBy(field => field.Key.Ordinal));
					if (string.IsNullOrEmpty(g.Key) && extensionFieldGroupCount == 1)
						groupContainer.Controls.Add(rowContainerControl);

					string fieldGroupHeader = !string.IsNullOrEmpty(g.Key) ? g.Key : Resources.DefaultFieldGroupName;
					HtmlTableRow fieldGroupHeaderRow = new HtmlTableRow();
					HtmlTableCell fieldGroupHeaderCell = new HtmlTableCell { ColSpan = this.ColumnsNumber };
					fieldGroupHeaderRow.Cells.Add(fieldGroupHeaderCell);
					fieldGroupHeaderCell.Attributes["class"] = "span";
					fieldGroupHeaderCell.Style["background-color"] = "#666666";
					fieldGroupHeaderCell.Style["color"] = "white";
					fieldGroupHeaderCell.Style["padding"] = "2px";
					fieldGroupHeaderCell.InnerText = fieldGroupHeader;

					groupContainer.Controls.Add(fieldGroupHeaderRow);
					groupContainer.Controls.Add(rowContainerControl);
				});

			return groupContainer;
		}

		private Control CreateTableRows(IEnumerable<KeyValuePair<IFieldMetadata, IExtensionFieldControlBuilder>> extensionFieldControlBuildersByFieldMetadata)
		{
			PlaceHolder placeHolder = new PlaceHolder();

			HtmlTableRow tableRow = new HtmlTableRow();
			tableRow.Attributes["class"] = this.TableRowCssClass;
			placeHolder.Controls.Add(tableRow);

			HtmlTableCell lastControlCell = null;
			int controlColumnCount = this.ColumnsNumber / 2;
			int occupiedCellsOfThisLine = 0;

			foreach (KeyValuePair<IFieldMetadata, IExtensionFieldControlBuilder> fieldControlBuilderByFieldMetadata in extensionFieldControlBuildersByFieldMetadata)
			{
				IFieldMetadata fieldMetadata = fieldControlBuilderByFieldMetadata.Key;
				IExtensionFieldControlBuilder fieldControlBuilder = fieldControlBuilderByFieldMetadata.Value;
				ExtensionDataInputControl dataInputControl = fieldControlBuilder.BuildDataInputControl(fieldMetadata);

				int occupiedControls = dataInputControl.OccupiedControlCells;
				if (occupiedControls > controlColumnCount)
				{
					if (occupiedCellsOfThisLine > 0)
					{
						if (lastControlCell != null)
							lastControlCell.ColSpan += (controlColumnCount - occupiedCellsOfThisLine) * 2;

						tableRow = new HtmlTableRow();
						tableRow.Attributes["class"] = this.TableRowCssClass;
						placeHolder.Controls.Add(tableRow);
					}

					occupiedCellsOfThisLine = controlColumnCount;
				}
				else if ((occupiedControls + occupiedCellsOfThisLine) > controlColumnCount)
				{
					if (lastControlCell != null)
						lastControlCell.ColSpan += (controlColumnCount - occupiedCellsOfThisLine) * 2;

					occupiedCellsOfThisLine = occupiedControls;
					tableRow = new HtmlTableRow();
					tableRow.Attributes["class"] = this.TableRowCssClass;
					placeHolder.Controls.Add(tableRow);
				}
				else
					occupiedCellsOfThisLine += occupiedControls;

				lastControlCell = CreateControlCell(tableRow, fieldMetadata, dataInputControl);
			}

			AddBlankTableCells(tableRow, occupiedCellsOfThisLine, controlColumnCount);
			return placeHolder;
		}

		private static HtmlTableCell CreateControlCell(HtmlTableRow row, IFieldMetadata fieldMetadata, ExtensionDataInputControl dataInputControl)
		{
			HtmlTableCell labelCell = new HtmlTableCell("td");
			labelCell.Attributes["class"] = "c1";
			labelCell.NoWrap = true;
			labelCell.InnerText = fieldMetadata.Name + ": ";
			row.Cells.Add(labelCell);

			HtmlTableCell controlCell = new HtmlTableCell("td");
			HtmlTableCell lastControlCell = controlCell;
			controlCell.Attributes["class"] = "c2";
			controlCell.NoWrap = true;
			controlCell.ColSpan = dataInputControl.OccupiedControlCells != int.MaxValue ? dataInputControl.OccupiedControlCells * 2 - 1 : 5;
			row.Cells.Add(controlCell);

			controlCell.Controls.Add(dataInputControl.Control);
			return lastControlCell;
		}

		private static void AddBlankTableCells(HtmlTableRow tableRow, int occupiedCellsOfThisLine, int maxControlsOfTheLine)
		{
			for (int i = 0; i < maxControlsOfTheLine - occupiedCellsOfThisLine; i++)
			{
				string spacerImageUrl = Kit.ResolveAbsoluteUrl("~/resources/images/spacer.gif");

				HtmlTableCell labelCell = new HtmlTableCell("td");
				labelCell.Attributes["class"] = "c1";
				labelCell.NoWrap = true;
				labelCell.Controls.Add(new LiteralControl("<img src='" + spacerImageUrl + "' alt='' />"));
				tableRow.Cells.Add(labelCell);

				HtmlTableCell controlCell = new HtmlTableCell("td");
				controlCell.Attributes["class"] = "c2";
				controlCell.Controls.Add(new LiteralControl("<img src='" + spacerImageUrl + "' alt='' />"));
				tableRow.Cells.Add(controlCell);
			}
		}
	}
}
