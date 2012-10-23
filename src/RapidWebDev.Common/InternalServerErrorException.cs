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
    /// The internal server error exception.
    /// </summary>
    [Serializable]
	public class InternalServerErrorException : CommonException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public InternalServerErrorException()
            : this(Resources.InternalServerErrorMessage)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msg">Message to include in the exception.</param>
        public InternalServerErrorException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">Inner exception.</param>
        public InternalServerErrorException(Exception exception)
            : base(exception.Message, exception)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msg">Message to include in the exception.</param>
        /// <param name="exception">Inner exception.</param>
        public InternalServerErrorException(string msg, Exception exception)
            : base(msg, exception)
        {
        }

        /// <summary>
        /// Constructor used for serialization
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="context">StreamingContext object</param>
        protected InternalServerErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

