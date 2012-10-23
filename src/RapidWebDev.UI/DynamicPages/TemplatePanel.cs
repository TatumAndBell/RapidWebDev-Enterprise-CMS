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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using RapidWebDev.Common;
using BJSC = RapidWebDev.UI.Controls;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages
{
    /// <summary>
    /// Detail panel control.
    /// </summary>
    [ParseChildren(true), PersistChildren(false)]
    public sealed class TemplatePanel : WebControl, INamingContainer
    {
        /// <summary>
        /// Sets/gets saving event handler.
        /// </summary>
        public event EventHandler Saving;

        /// <summary>
        /// Sets/gets cancelling event handler.
        /// </summary>
        public event EventHandler Cancelling;

		/// <summary>
		/// Sets/gets event handler fired after child controls of template panel loaded.
		/// </summary>
		public event EventHandler<TemplatePanelChildControlsAttachedEventArgs> ChildControlLoaded;

        /// <summary>
        /// Sets/gets header text of detail panel.
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Sets/gets true when the background color of panel is transparent.
        /// </summary>
        public bool Transparent { get; set; }

        /// <exclude/>
        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }

        /// <summary>
        /// Sets/gets path of ascx template file. The path will be parsed by Server.MapPath("").
        /// </summary>
        public string SkinPath { get; set; }

        /// <summary>
        /// Sets/gets saving button.
        /// </summary>
		public BJSC.Button ButtonSave { get; private set; }

        /// <summary>
        /// Sets/gets cancelling button.
        /// </summary>
		public BJSC.Button ButtonCancel { get; private set; }

        /// <summary>
        /// Construct detail panel control.
        /// </summary>
        public TemplatePanel()
        {
        }

        /// <summary>
        /// Raises the System.Web.UI.Control.Init event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!File.Exists(HttpContext.Current.Server.MapPath(this.SkinPath)))
				throw new FileNotFoundException(Resources.DPCtrl_TemplateSkinFileNotExists, this.SkinPath);

            this.Controls.Clear();
			BJSC.Container container = new BJSC.Container() { Frame = this.Transparent };
            this.Controls.Add(container);

            if (!Kit.IsEmpty(this.HeaderText))
            {
                HtmlGenericControl header = new HtmlGenericControl("h4") { InnerText = this.HeaderText };
                container.Controls.Add(header);
            }

            container.Controls.Add(this.LoadThemedControl());


            HtmlTable table = new HtmlTable() { CellPadding = 0, CellSpacing = 0 };
            table.Attributes["class"] = "table6col";
            container.Controls.Add(table);

            HtmlTableRow tr = new HtmlTableRow();
            table.Controls.Add(tr);

            HtmlTableCell cell = new HtmlTableCell("td") { ColSpan = 6, Align = "center" };
            cell.Style["PADDING-TOP"] = "8px";
            tr.Cells.Add(cell);

			this.ButtonSave = new BJSC.Button() { ID = "ButtonSave", Text = Resources.DPCtrl_SaveText };
			this.ButtonCancel = new BJSC.Button() { ID = "ButtonCancel", Text = Resources.DPCtrl_CancelText };
            this.ButtonSave.Click += new EventHandler(this.OnSaving);
            this.ButtonCancel.Click += new EventHandler(this.OnCancelling);
            cell.Controls.Add(this.ButtonSave);
            cell.Controls.Add(new HtmlGenericControl("span") { InnerText = " " });
            cell.Controls.Add(this.ButtonCancel);

			if (this.ChildControlLoaded != null)
				this.ChildControlLoaded(this, new TemplatePanelChildControlsAttachedEventArgs(this));
        }

		private void OnSaving(object sender, EventArgs e)
        {
            if (this.Saving != null) this.Saving(sender, e);
        }

		private void OnCancelling(object sender, EventArgs e)
        {
            if (this.Cancelling != null) this.Cancelling(sender, e);
        }

        /// <summary>
		/// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
			base.DataBind();
            this.EnsureChildControls();
        }

        /// <summary>
		/// Searches for a server control by id included in this control.
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        public override Control FindControl(string controlId)
        {
            Control ctrl = base.FindControl(controlId);
            if (ctrl == null && this.Controls.Count == 1)
                ctrl = this.Controls[0].FindControl(controlId);

            if (ctrl == null)
                ctrl = this.Controls[0].Controls[1].Controls[0].FindControl(controlId);

            if (ctrl == null)
                ctrl = this.Controls[0].Controls[0].Controls[0].FindControl(controlId);

			if (ctrl == null)
				return FindControl(this, controlId);

            return ctrl;
        }

        /// <summary>
        /// Loads the skin file from the users current theme
        /// </summary>
        /// <returns></returns>
        private Control LoadThemedControl()
        {
            Control skinTemplate = this.Page.LoadControl(this.SkinPath);
            skinTemplate.ID = Path.GetFileNameWithoutExtension(this.SkinPath);

            Panel panel = new Panel();
            panel.Controls.Add(skinTemplate);

            this.Controls.Add(panel);
            return panel;
        }

		/// <summary>
		/// Find child controls contained in parentControl by going through all children.
		/// </summary>
		/// <param name="parentControl"></param>
		/// <param name="controlId"></param>
		/// <returns></returns>
		private static Control FindControl(Control parentControl, string controlId)
		{
			if (parentControl == null || parentControl.Controls == null || parentControl.Controls.Count == 0)
				return null;

			foreach (Control control in parentControl.Controls)
				if (control.ID == controlId)
					return control;

			foreach (Control control in parentControl.Controls)
			{
				Control subControl = FindControl(control, controlId);
				if (subControl != null) return subControl;
			}

			return null;
		}
    }
}

