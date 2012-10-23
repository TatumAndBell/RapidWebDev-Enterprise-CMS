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
using System.Globalization;
using System.Linq;
using System.Text;
using RapidWebDev.Common;

namespace RapidWebDev.UI.DynamicPages
{
	/// <summary>
	/// Query sorting expression
	/// </summary>
	public class SortExpression
	{
		/// <summary>
		/// Gets query sorting expression field name.
		/// </summary>
		public string FieldName { get; private set; }

		/// <summary>
		/// Gets query sorting expression direction.
		/// </summary>
		public bool Ascending { get; private set; }

		/// <summary>
		/// Construct SortExpression instance
		/// </summary>
		/// <param name="fieldName">Query sorting expression field name.</param>
		/// <param name="ascending">Query sorting expression direction.</param>
		public SortExpression(string fieldName, bool ascending)
		{
			this.FieldName = fieldName;
			this.Ascending = ascending;
		}

		/// <summary>
		/// Construct SortExpression instance
		/// </summary>
		/// <param name="fieldName">Query sorting expression field name.</param>
		public SortExpression(string fieldName)
		{
			this.FieldName = fieldName;
			this.Ascending = true;
		}

		/// <summary>
		/// Compile sort expression collection to predicate for database query.
		/// </summary>
		/// <returns></returns>
		public string Compile()
		{
			IPredicateCompiler compiler = SpringContext.Current.GetObject<IPredicateCompiler>();
			return compiler.CompileSortExpression(this);
		}

		/// <summary>
		/// Returns a string that represents the current expression.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.FieldName, this.Ascending ? "ASC" : "DESC");
		}
	}
}

