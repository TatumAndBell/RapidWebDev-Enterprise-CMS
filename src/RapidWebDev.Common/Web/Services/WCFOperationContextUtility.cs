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
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Web.UI;
using System.Reflection;
using System.Globalization;

namespace RapidWebDev.Common.Web.Services
{
	/// <summary>
	/// Utility for WCF operation context to get the accessing service and operation contract.
	/// </summary>
	public static class WCFOperationContextUtility
	{
		/// <summary>
		/// Resolve the accessing service and operation contract.
		/// </summary>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public static WCFLookupResult ResolvePermissionAttribute(OperationContext operationContext)
		{
			Type serviceImplType = operationContext.EndpointDispatcher.DispatchRuntime.Type;
			Type[] interfaceTypes = serviceImplType.GetInterfaces();

			if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch == null)
				return null;

			object WCFLookupResult = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.Data;
			//string operationContractName = DataBinder.Eval(WCFLookupResult, "OperationName") as string;
            string operationContractName = WCFLookupResult.ToString() ;

			foreach (Type interfaceType in interfaceTypes)
			{
				object[] serviceContractAttributes = interfaceType.GetCustomAttributes(typeof(ServiceContractAttribute), true);
				if (serviceContractAttributes.Length == 0) continue;

				string serviceContractName = ((ServiceContractAttribute)serviceContractAttributes[0]).Name;
				MethodInfo operationMethod = GetOperationMethodByOperationContractName(interfaceType, operationContractName);
				if (operationMethod == null) continue;

				object[] permissionAttributes = operationMethod.GetCustomAttributes(typeof(PermissionAttribute), true);
				if (permissionAttributes.Length == 0) continue;

				return new WCFLookupResult(serviceContractName, operationContractName);
			}

			return null;
		}

		private static MethodInfo GetOperationMethodByOperationContractName(Type interfaceType, string operationContractName)
		{
			MethodInfo matchedByMethodName = null;
			MethodInfo[] operationMethods = interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			foreach (MethodInfo operationMethod in operationMethods)
			{
				object[] operationContractAttributes = operationMethod.GetCustomAttributes(typeof(OperationContractAttribute), true);
				if (operationContractAttributes.Length == 0)
					continue;

				OperationContractAttribute operationContract = operationContractAttributes[0] as OperationContractAttribute;
				if (operationContract.Name == operationContractName)
					return operationMethod;

				if (operationMethod.Name == operationContractName)
					matchedByMethodName = operationMethod;
			}

			return matchedByMethodName;
		}
	}
}
