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
using RapidWebDev.Common;
using RapidWebDev.UI.DynamicPages.Configurations;
using System.Collections.Generic;
using System.Collections;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The interface of dynamic component in the infrastructure.
	/// </summary>
	public interface IDynamicComponent
	{
		/// <summary>
		/// The delegate will be registered by dynamic page infrastructure. 
		/// It's used for dynamic page developers to show messages onto UI to users through internal infrastructure.
		/// </summary>
		Action<MessageTypes, string> ShowMessage { get; set; }

		/// <summary>
		/// Gets/sets page dynamic page configuration to build up web page.
		/// </summary>
		DynamicPageConfiguration Configuration { get; set; }

		/// <summary>
		/// Setup context temporary variables for formatting configured text-typed properties.
		/// </summary>
		/// <param name="sender">The sender which invokes the method.</param>
		/// <param name="e">Callback event argument.</param>
		void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e);
	}

	/// <summary>
	/// Event argument to setup variables for the request context in dynamic/detail panel/aggregate panel pages.
	/// </summary>
	public class SetupApplicationContextVariablesEventArgs : EventArgs
	{
		/// <summary>
		/// Gets application context.
		/// </summary>
		public IDictionary TempVariables { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public SetupApplicationContextVariablesEventArgs()
		{
			IApplicationContext applicationContext = SpringContext.Current.GetObject<IApplicationContext>();
			this.TempVariables = applicationContext.TempVariables;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tempVariables"></param>
		public SetupApplicationContextVariablesEventArgs(IDictionary tempVariables)
		{
			this.TempVariables = tempVariables;
		}
	}
}

