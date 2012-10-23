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
using System.Text;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Query predicate expressions compiler which used to assembly QueryPredicate instance for different database engine solution, e.g Linq2SQL, ADO.NET Entity Framework...
	/// </summary>
	public interface IPredicateCompiler
	{
		/// <summary>
		/// Compile expression collection to query predicate. Returns NULL if there is no query expression indicated.
		/// </summary>
		/// <param name="expressions"></param>
		/// <returns></returns>
		LinqPredicate CompileQuery(QueryParameterExpressionCollection expressions);

		/// <summary>
		/// Compile sorting expression to sort predicate. Returns NULL if there is no sort expression indicated.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		string CompileSortExpression(SortExpression sortExpression);
	}
}