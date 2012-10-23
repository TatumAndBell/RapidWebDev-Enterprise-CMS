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
using System.Web.UI;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The interface mapping to aggregate panel of dynamic page.
	/// </summary>
	public interface IAggregatePanelPage : IDynamicComponent
	{
		/// <summary>
		/// The method will be invoked when the user clicks Save button in aggregate panel.
		/// </summary>
		/// <param name="commandArgument">Configured button command argument.</param>
		/// <param name="entityIdEnumerable">
		/// The id collection of entities which need to be processed by specified command argument.
		/// </param>
		void Save(string commandArgument, IEnumerable<string> entityIdEnumerable);

		/// <summary>
		/// The method will be invoked when the detail panel is initialized.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web application.</param>
		/// <param name="e">Callback event argument</param>
		void OnInit(IRequestHandler sender, AggregatePanelPageEventArgs e);

		/// <summary>
		///  The method will be invoked when detail panel is loaded.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web application.</param>
		/// <param name="e">Callback event argument</param>
		void OnLoad(IRequestHandler sender, AggregatePanelPageEventArgs e);

		/// <summary>
		/// The method will be invoked when the detail panel is prerendering.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web application.</param>
		/// <param name="e">Callback event argument</param>
		void OnPreRender(IRequestHandler sender, AggregatePanelPageEventArgs e);
	}

	/// <summary>
	/// The event argument of aggregate panel page.
	/// </summary>
	public class AggregatePanelPageEventArgs : EventArgs
	{
		/// <summary>
		/// Gets command argument for the aggregate panel.
		/// </summary>
		public string CommandArgument { get; private set; }

		/// <summary>
		/// Gets multiple selected entity ids for processing.
		/// </summary>
		public IEnumerable<string> EntityIdEnumerable { get; private set; }

		/// <summary>
		/// Construct event argument 
		/// </summary>
		/// <param name="commandArgument"></param>
		/// <param name="entityIdEnumerable"></param>
		public AggregatePanelPageEventArgs(string commandArgument, IEnumerable<string> entityIdEnumerable)
		{
			this.CommandArgument = commandArgument;
			this.EntityIdEnumerable = entityIdEnumerable;
		}
	}
}
