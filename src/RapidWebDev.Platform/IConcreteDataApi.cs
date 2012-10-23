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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel;
using System.Linq.Expressions;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The concrete data API is used for CRUD constant values.
	/// The method executed add/update depends on whether identity of object is empty or not.
	/// Take an example to explain the business value of concrete data API.
	/// In a product-order system, the product may have many properties which have candidate property values for users to select.
	/// The product may have Size property with three candidate values "Big", "Medium" and "Small" and Color property with 2 candidate values "Light" and "Dark".
	/// Then we can categorize that there are 3 concrete data objects "Big", "Medium" and "Small" in concrete data type "Size" as 2 concrete data objects "Light" and "Dark" in concrete data type "Color" in concrete model provided in RapidWebDev.
	/// Generally, with Concrete Data Model, we have avoided to design database schema and implement data access API to CRUD constant values.
	/// And With extension model integrated into concrete data model, we can define the dynamic properties for concrete data based on the needs of business. 
	/// </summary>
	public interface IConcreteDataApi
	{
		/// <summary>
		/// The method executed add/update depends on whether identity of object is empty or not.
		/// </summary>
		/// <param name="concreteDataObject">The name of concrete data should be unique in a concrete data type.</param>
		void Save(ConcreteDataObject concreteDataObject);

		/// <summary>
		/// Get concrete data by id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		ConcreteDataObject GetById(Guid id);

		/// <summary>
		/// Get concrete data by name.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		ConcreteDataObject GetByName(string type, string name);

		/// <summary>
		/// Find all concrete data objects includes soft deleted in the special type sorted by Name ascendingly.
		/// </summary>
		/// <param name="type">valid concrete data type</param>
		/// <returns></returns>
		IEnumerable<ConcreteDataObject> FindAllByType(string type);

		/// <summary>
		/// Find all available concrete data types.
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> FindConcreteDataTypes();

		/// <summary>
		/// Find concrete data in all types by custom predicates.
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="orderby"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		IEnumerable<ConcreteDataObject> FindConcreteData(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount);
	}
}
