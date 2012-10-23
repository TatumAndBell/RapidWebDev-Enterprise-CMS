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

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Interface to print queried data in the dynamic page to any format depends on the implementation.
	/// </summary>
	public interface IDynamicPagePrinter
	{
		/// <summary>
		/// Print queried data in the dynamic page to any format depends on the implementation.
		/// </summary>
		/// <param name="dynamicPageService">The dynamic page handler.</param>
		/// <param name="queryStringParameters">The query string parameters.</param>
		/// <returns></returns>
		DynamicPagePrintResult Print(IDynamicPage dynamicPageService, NameValueCollection queryStringParameters);
	}

	/// <summary>
	/// Dynamic page print result.
	/// </summary>
	public class DynamicPagePrintResult
	{
		/// <summary>
		/// Printing result.
		/// </summary>
		public string Result { get; set; }

		/// <summary>
		/// Printing result type.
		/// </summary>
		public DynamicPagePrintResultType ResultType { get; set; }
	}

	/// <summary>
	/// Dynamic page print result type.
	/// </summary>
	public enum DynamicPagePrintResultType
	{
		/// <summary>
		/// The result should be an Uri to a temporary file in the server.
		/// </summary>
		TemporaryFile,

		/// <summary>
		/// The result should be the printing result content.
		/// </summary>
		Content
	}
}

