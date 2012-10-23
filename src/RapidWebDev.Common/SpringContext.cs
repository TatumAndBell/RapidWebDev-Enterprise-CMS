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

namespace RapidWebDev.Common
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Web;
	using Spring.Context;
	using Spring.Context.Support;
	using Spring.Core;

	/// <summary>
	/// Sprint.NET application context which helps to resolve objects configured in IoC container easily and it supports to resolve generic objects by specifying generic type.
	/// </summary>
	public static class SpringContext
	{
		private static Spring.Context.IApplicationContext current;
		private static object syncObject = new object();

		/// <summary>
		/// Get current Sprint.NET application context.
		/// </summary>
		public static Spring.Context.IApplicationContext Current
		{
			get
			{
                if (HttpContext.Current != null)
                {
                    try
                    {
                        return WebApplicationContext.Current;
                    }
                    catch
                    {
                    }
                }

				if (current == null)
				{
					lock (syncObject)
					{
						if (current == null)
						{
							try
							{
								current = ContextRegistry.GetContext();
							}
							catch (Exception exp)
							{
								Logger.Instance(typeof(SpringContext)).Error(exp);
								throw;
							}
						}
					}
				}

				return current;
			}
		}

		/// <summary>
		/// Get object in Spring IoC by generic type name. 
		/// If specified generic type is a interface with name starts with "I", "I" will be removed from type name prior to retrieve from IoC.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="applicationContext"></param>
		/// <returns></returns>
		public static T GetObject<T>(this Spring.Context.IApplicationContext applicationContext) where T : class
		{
			IDictionary containedObjects = (IDictionary)applicationContext.GetObjectsOfType(typeof(T));
			if (containedObjects.Count == 0) return null;

			if (containedObjects.Count == 1)
				return containedObjects[containedObjects.Keys.Cast<object>().First()] as T;

			string objectId = string.Empty;
			Type type = typeof(T);
			if (type.IsInterface && type.Name.StartsWith("I") && type.Name.Length > 1)
				objectId = type.Name.Substring(1);
			else
				objectId = type.Name;

			if (containedObjects.Contains(objectId))
				return containedObjects[objectId] as T;

			return null;
		}

		/// <summary>
		/// Get object in Spring IoC by specified object id and return it in generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="applicationContext"></param>
		/// <param name="objectId"></param>
		/// <returns></returns>
		public static T GetObject<T>(this Spring.Context.IApplicationContext applicationContext, string objectId) where T : class
		{
			if (!applicationContext.ContainsObject(objectId)) return null;
			return applicationContext.GetObject(objectId) as T;
		}
	}
}

