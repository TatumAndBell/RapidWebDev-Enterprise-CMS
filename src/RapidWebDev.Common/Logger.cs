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
using System.IO;
using System.Reflection;
using System.Text;
using Common.Logging;

namespace RapidWebDev.Common
{
    /// <summary>
	/// The Common Logging wrapper class which helps to resolve a instance for logging easily. 
    /// </summary>
    public static class Logger
    {
        private static object syncObject = new object();
		
        /// <summary>
		/// Get Common Logging instance by logger name.
        /// </summary>
		/// <param name="loggerName">logger name.</param>
		/// <returns>Returns logger instance.</returns>
        public static ILog Instance(string loggerName)
        {
			return LogManager.GetLogger(loggerName);
        }

		/// <summary>
		/// Get Common Logging instance by calling application type.
		/// </summary>
		/// <param name="type">caller type.</param>
		/// <returns>Returns logger instance.</returns>
		public static ILog Instance(Type type)
		{
			return LogManager.GetLogger(type);
		}

		/// <summary>
		/// Get Common Logging instance by calling instance.
		/// </summary>
		/// <param name="instance">calling instance.</param>
		/// <returns>Returns logger instance.</returns>
		public static ILog Instance(object instance)
		{
			return Instance(instance.GetType());
		}

		/// <summary>
		/// Logging info with formatting message by parameters.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		public static void InfoFormat(this ILog log, string messageFormat, params object[] args)
		{
			log.Info(string.Format(messageFormat, args));
		}

		/// <summary>
		/// Logging debug with formatting message by parameters.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		public static void DebugFormat(this ILog log, string messageFormat, params object[] args)
		{
			log.Debug(string.Format(messageFormat, args));
		}

		/// <summary>
		/// Logging warning with formatting message by parameters.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		public static void WarnFormat(this ILog log, string messageFormat, params object[] args)
		{
			log.Warn(string.Format(messageFormat, args));
		}

		/// <summary>
		/// Logging error with formatting message by parameters.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		public static void ErrorFormat(this ILog log, string messageFormat, params object[] args)
		{
			log.Error(string.Format(messageFormat, args));
		}

		/// <summary>
		/// Logging fatal with formatting message by parameters.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		public static void FatalFormat(this ILog log, string messageFormat, params object[] args)
		{
			log.Fatal(string.Format(messageFormat, args));
		}
    }
}
