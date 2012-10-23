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

using System.Collections.Specialized;
using AopAlliance.Aop;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// The request handler which invokes the callback.
	/// </summary>
	public interface IRequestHandler : IAdvice
	{
		/// <summary>
		/// Whether the request is a postback.
		/// </summary>
		bool IsPostBack { get; }

		/// <summary>
		/// Whether the request is in asynchronous way.
		/// </summary>
		bool IsAsynchronous { get; }

		/// <summary>
		/// Gets the parameters to the request.
		/// </summary>
		NameValueCollection Parameters { get; }
	}
}