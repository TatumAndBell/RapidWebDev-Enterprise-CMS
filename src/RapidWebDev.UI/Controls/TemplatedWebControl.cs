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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// The base class for templated controls that must load their UI either through an external skin or via an in-page template.
	/// </summary>
	[ParseChildren(true), PersistChildren(false),]
	public abstract class TemplatedWebControl : WebControl, INamingContainer
	{
		private ITemplate skinTemplate;

		/// <exclude/>
		public override ControlCollection Controls
		{
			get
			{
				this.EnsureChildControls();
				return base.Controls;
			}
		}

		/// <exclude/>
		public override void DataBind()
		{
			this.EnsureChildControls();
		}

		/// <summary>
		/// Set/get path of ascx template file. The path will be parsed by Server.MapPath("").
		/// </summary>
		public virtual string SkinPath { get; set; }

		/// <summary>
		/// The template used to override the default UI of the control.
		/// </summary>
		/// <remarks>
		/// All serverside controls that are in the default UI must exist and have the same ID's.
		/// </remarks>
		[Browsable(false), DefaultValue(null), Description("Skin Template"), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate SkinTemplate
		{
			get
			{
				return this.skinTemplate;
			}
			set
			{
				this.skinTemplate = value;
				base.ChildControlsCreated = false;
			}
		}

		/// <exclude/>
		public override Control FindControl(string controlId)
		{
			Control ctrl = base.FindControl(controlId);
			if (ctrl == null && this.Controls.Count == 1)
			{
				ctrl = this.Controls[0].FindControl(controlId);
			}
			return ctrl;
		}

		/// <summary>
		/// Loads the skin file from the users current theme
		/// </summary>
		/// <returns></returns>
		protected virtual bool LoadThemedControl()
		{
			if (File.Exists(HttpContext.Current.Server.MapPath(this.SkinPath)))
			{
				Control skin = this.Page.LoadControl(this.SkinPath);
				skin.ID = "_";
				this.Controls.Add(skin);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Load the skin as an inline template. By default, this will be the second option
		/// </summary>
		/// <returns></returns>
		protected virtual bool LoadSkinTemplate()
		{
			if (SkinTemplate != null)
			{
				SkinTemplate.InstantiateIn(this);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.Init event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			this.Controls.Clear();

			// 1) Inline Template
			bool _skinLoaded = this.LoadSkinTemplate();

			// 2) Themed Control
			if (!_skinLoaded)
			{
				_skinLoaded = this.LoadThemedControl();
			}

			// 3) If none of the skin locations were successful, throw.
			if (!_skinLoaded)
			{
				throw new TemplateNotFoundException(this.GetType().ToString());
			}
			else
			{
				this.AttachChildControls();
			}

			base.OnInit(e);
		}

		/// <summary>
		/// Override this method to attach templated or external skin controls to local references.
		/// </summary>
		protected abstract void AttachChildControls();
	}
}
