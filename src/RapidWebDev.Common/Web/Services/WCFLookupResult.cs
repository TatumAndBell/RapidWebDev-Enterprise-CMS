/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.long.yi@RapidWebDev.org

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

namespace RapidWebDev.Common.Web.Services
{
	/// <summary>
	/// WCF lookup result which includes the accessing service contract and operation contract.
	/// </summary>
	public sealed class WCFLookupResult
	{
		/// <summary>
		/// Service contract name.
		/// </summary>
		public string ServiceContractName { get; private set; }

		/// <summary>
		/// Operation contract name.
		/// </summary>
		public string OperationContractName { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serviceContractName"></param>
		/// <param name="operationContractName"></param>
		public WCFLookupResult(string serviceContractName, string operationContractName)
		{
			this.ServiceContractName = serviceContractName;
			this.OperationContractName = operationContractName;
		}
	}
}
