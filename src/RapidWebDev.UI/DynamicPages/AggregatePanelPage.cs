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
using RapidWebDev.Common;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The abstract implementation to interface IAggregatePanelPage. 
	/// The class has declared all interface methods as virtual so that the business implementation class can implement signatures by requirement. 
	/// </summary>
	public abstract class AggregatePanelPage : IAggregatePanelPage
	{
		/// <summary>
		/// The delegate will be registered by dynamic page infrastructure. 
		/// It's used for dynamic page developers to show messages onto UI to users through internal infrastructure.
		/// </summary>
		public Action<MessageTypes, string> ShowMessage { get; set; }

		/// <summary>
		/// Gets/sets page dynamic page configuration to build up web page.
		/// </summary>
		public DynamicPageConfiguration Configuration { get; set; }

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		public virtual void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e) { }

		/// <summary>
		/// The method will be invoked when the user clicks Save button in aggregate panel.
		/// </summary>
		/// <param name="commandArgument">Configured button command argument.</param>
		/// <param name="entityIdEnumerable">
		/// The id collection of entities which need to be processed by specified command argument.
		/// </param>
		public virtual void Save(string commandArgument, IEnumerable<string> entityIdEnumerable) { }

		/// <summary>
		/// The method will be invoked when the detail panel is initialized.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web application.</param>
		/// <param name="e">Callback event argument</param>
		public virtual void OnInit(IRequestHandler sender, AggregatePanelPageEventArgs e) { }

		/// <summary>
		///  The method will be invoked when detail panel is loaded.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web application.</param>
		/// <param name="e">Callback event argument</param>
		public virtual void OnLoad(IRequestHandler sender, AggregatePanelPageEventArgs e) { }

		/// <summary>
		/// The method will be invoked when the detail panel is prerendering.
		/// </summary>
		/// <param name="sender">The sender which invokes the callback. It's a web page in web application.</param>
		/// <param name="e">Callback event argument</param>
		public virtual void OnPreRender(IRequestHandler sender, AggregatePanelPageEventArgs e) { }
	}
}
