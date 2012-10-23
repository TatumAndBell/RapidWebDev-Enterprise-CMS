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

using System.Xml;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Form panel button will be rendered as a common style button in the form.
	/// </summary>
	public class FormPanelButtonConfiguration
	{
		private string text;
		private string tooltip;

		/// <summary>
		/// Sets/gets generic button display text.
		/// </summary>
		public string Text
		{
			set { this.text = value; }
			get { return WebUtility.ReplaceVariables(this.text); }
		}

		/// <summary>
		/// Sets/gets generic button tooltip.
		/// </summary>
		public string ToolTip
		{
			set { this.tooltip = value; }
			get { return WebUtility.ReplaceVariables(this.tooltip); }
		}

		/// <summary>
		/// Whether the button is the default button of the form, defaults to false.
		/// </summary>
		public bool IsFormDefaultButton { get; set; }

		/// <summary>
		/// Construct FormPanelButtonConfiguration instance from xml element.
		/// </summary>
		/// <param name="buttonElement"></param>
		/// <param name="xmlParser"></param>
		public FormPanelButtonConfiguration(XmlElement buttonElement, XmlParser xmlParser)
		{
			this.Text = xmlParser.ParseString(buttonElement, "@Text");
			this.ToolTip = xmlParser.ParseString(buttonElement, "@ToolTip");
			this.IsFormDefaultButton = xmlParser.ParseBoolean(buttonElement, "@IsFormDefaultButton", false);
		}
	}
}
