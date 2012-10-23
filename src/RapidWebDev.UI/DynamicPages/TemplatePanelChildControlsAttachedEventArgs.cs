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
using System.Linq;
using System.Text;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The event argument for the event after child controls of template panel loaded.
	/// </summary>
	public class TemplatePanelChildControlsAttachedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets template panel which has loaded the child controls.
		/// </summary>
		public TemplatePanel TemplatePanel { get; private set; }

		/// <summary>
		/// Construct event argument for specified template panel.
		/// </summary>
		/// <param name="templatePanel"></param>
		public TemplatePanelChildControlsAttachedEventArgs(TemplatePanel templatePanel)
		{
			this.TemplatePanel = templatePanel;
		}
	}
}

