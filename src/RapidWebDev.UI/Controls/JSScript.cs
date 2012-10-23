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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using RapidWebDev.Common.Web;

namespace RapidWebDev.UI.Controls
{
	/// <summary>
	/// The control holds a block of javascript code and render them into page by ScriptManager.
	/// </summary>
	[DefaultProperty("Value"), ParseChildren(true, "Value"), ControlValueProperty("Value"), ValidationProperty("Value")]
	public class JSScript : Control, ITextControl, INamingContainer
	{
		/// <summary>
		/// Gets/gets script value. 
		/// If JSScript equals Block, the Value should be a block of immediate JavaScript code.
		/// If JSScript equals File, the Value should an external JavaScript File URL.
		/// If JSScript equals EmbeddedResource, the Value should URI of embedded resource.
		/// </summary>
		[Bindable(true, BindingDirection.TwoWay), DefaultValue(""), Localizable(true)]
		public string Value
		{
			get
			{
				if (base.ViewState["Value"] == null) return string.Empty;
				return base.ViewState["Value"] as string;
			}
			set { base.ViewState["Value"] = value; }
		}

		/// <summary>
		/// Gets/gets JavaScript types. The default value is Block.
		/// </summary>
		public JSScriptTypes Type
		{
			get
			{
				if (base.ViewState["JSScriptTypes"] == null) return JSScriptTypes.Block;
				return (JSScriptTypes)base.ViewState["JSScriptTypes"];
			}
			set { base.ViewState["JSScriptTypes"] = value; }
		}

		string ITextControl.Text
		{
			get { return this.Value; }
			set { this.Value = value; }
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.Init event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			switch (this.Type)
			{
				case JSScriptTypes.EmbeddedResource:
					string referenceUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), this.Value);
					ClientScripts.RegisterHeaderScriptInclude(referenceUrl);
					break;
				case JSScriptTypes.File:
					ClientScripts.RegisterHeaderScriptInclude(VirtualPathUtility.ToAbsolute(this.Value));
					break;
			}
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			switch (this.Type)
			{
				case JSScriptTypes.Block:
					ClientScripts.RegisterScriptBlock(this.Value);
					break;
			}
		}
	}

	/// <summary>
	/// JavaScript Types
	/// </summary>
	public enum JSScriptTypes 
	{ 
		/// <summary>
		/// Javascript code block
		/// </summary>
		Block = 0, 
		
		/// <summary>
		/// External javascript file
		/// </summary>
		File = 1, 
		
		/// <summary>
		/// Assembly embedded resource
		/// </summary>
		EmbeddedResource = 2 
	}
}
