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
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The hierarchy API is used for CRUD generic data within hierarchy.
	/// The common business scenario likes Geography that each area in geography potentially has a parent and multiple children.
	/// The example likes the managed geography can be selected as shipping destination city when the user creates a order for shipping in product-order system.
	/// In this case we don't need to design the database schema and create API to CRUD areas in geography any more with Hierarchy model provided in the RapidWebDev. 
	/// We can use IHierarchyApi to CRUD any hierarchy data generally. 
	/// The hierarchy data object focused in the IHierarchyApi includes basic information likes Code, Name, Description, HierarchyType and it integrates the extension model provided in RapidWebDev. 
	/// The property HierarchyType is used to categorize the hierarchy data which provides the capacity to manage multiple hierarchy objects in a system at a time, likes Geography, Functional Zone.
	/// With extension model integrated into Hierarchy, we can define the dynamic properties for hierarchy data based on the needs of business. 
	/// </summary>
	public interface IHierarchyApi
	{
		/// <summary>
		/// Save a hierarchy data object.
		/// </summary>
		/// <param name="hierarchyDataObject"></param>
		void Save(HierarchyDataObject hierarchyDataObject);

		/// <summary>
		/// Get a hierarchy data object by id.
		/// </summary>
		/// <param name="hierarchyDataId"></param>
		/// <returns></returns>
		HierarchyDataObject GetHierarchyData(Guid hierarchyDataId);

		/// <summary>
		/// Get a hierarchy data object by name.
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="hierarchyDataName"></param>
		/// <returns></returns>
		HierarchyDataObject GetHierarchyData(string hierarchyType, string hierarchyDataName);

		/// <summary>
		/// Get all children of the specified hierarchy data.
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="parentHierarchyDataId"></param>
		/// <returns></returns>
		IEnumerable<HierarchyDataObject> GetImmediateChildren(string hierarchyType, Guid? parentHierarchyDataId);

		/// <summary>
		/// Get all children of the specified hierarchy data includes not immediately.
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <param name="parentHierarchyDataId"></param>
		/// <returns></returns>
		IEnumerable<HierarchyDataObject> GetAllChildren(string hierarchyType, Guid? parentHierarchyDataId);

		/// <summary>
		/// Get all hierarchy data in specified hierarchy type.
		/// </summary>
		/// <param name="hierarchyType"></param>
		/// <returns></returns>
		IEnumerable<HierarchyDataObject> GetAllHierarchyData(string hierarchyType);

		/// <summary>
		/// Hard delete a hierarchy data with all its children by id. 
		/// </summary>
		/// <param name="hierarchyDataId"></param>
		void HardDeleteHierarchyData(Guid hierarchyDataId);

		/// <summary>
		/// Find hierarchy data in all types by custom predicates.
		/// </summary>
		/// <param name="predicate">linq predicate which supports properties of <see cref="RapidWebDev.Platform.HierarchyDataObject"/> for query expression.</param>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns></returns>
		IEnumerable<HierarchyDataObject> FindHierarchyData(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount);
	}
}
