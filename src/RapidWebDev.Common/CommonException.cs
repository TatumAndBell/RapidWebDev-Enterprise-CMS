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
using System.Runtime.Serialization;
using RapidWebDev.Common.Properties;

namespace RapidWebDev.Common
{
    /// <summary>
    /// The bad request exception.
    /// </summary>
    [Serializable]
    public abstract class CommonException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CommonException()
            : this(Resources.BadRequest)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msg">Message to include in the exception.</param>
        public CommonException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msg">Message to include in the exception.</param>
        /// <param name="exception">Inner exception.</param>
        public CommonException(string msg, Exception exception)
            : base(msg, exception)
        {
        }


        /// <summary>
        /// Constructor used for serialization
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="context">StreamingContext object</param>
		protected CommonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

