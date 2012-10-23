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
using System.Collections;
using System.Linq;
using System.Text;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Query results
	/// </summary>
	public class QueryResults
	{
		/// <summary>
		/// Sets/gets query total record count.
		/// </summary>
		public int RecordCount { get; private set; }

		/// <summary>
		/// Sets/gets returned query results.
		/// </summary>
		public IEnumerable Results { get; private set; }

		/// <summary>
		/// Construct QueryResults
		/// </summary>
		/// <param name="results"></param>
		/// <param name="recordCount"></param>
		public QueryResults(int recordCount, IEnumerable results)
		{
			Kit.NotNull(results, "results");

			this.RecordCount = recordCount;
			this.Results = results;
		}
	}
}

