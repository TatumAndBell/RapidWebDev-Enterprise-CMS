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
using System.Security.Principal;
using System.Text;
using System.Web;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;

namespace RapidWebDev.Platform.Initialization
{
	/// <summary>
	/// The implementation class of IInstallerManager manages all installer instances.
	/// </summary>
	public class InstallerManager : IInstallerManager
	{
		private object syncObject = new object();
		private HashSet<string> installedApplicationNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Gets/sets enumerable installers.
		/// </summary>
		public IEnumerable<IInstaller> Installers { get; set; }

		/// <summary>
		/// Install application
		/// </summary>
		/// <param name="applicationName">application name to install</param>
		public void Install(string applicationName)
		{
			if (this.Installers == null) return;

			lock (syncObject)
			{
				try
				{
					if (this.installedApplicationNames.Contains(applicationName)) return;
					//using (TransactionScope transactionScope = new TransactionScope())
					//{
						foreach (IInstaller installer in this.Installers)
							installer.Install(applicationName);

                      //  transactionScope.Complete();
					//}

					this.installedApplicationNames.Add(applicationName);
				}
				catch (Exception exp)
				{
					Logger.Instance(this).Error(exp);
                
                    throw exp;
           

				}
			}
		}

		/// <summary>
		/// Uninstall application
		/// </summary>
		/// <param name="applicationName">application name to uninstall</param>
		public void Uninstall(string applicationName)
		{
			if (this.Installers == null) return;

			lock (syncObject)
			{
				foreach (IInstaller installer in this.Installers)
					installer.Uninstall(applicationName);
			}
		}
	}
}

