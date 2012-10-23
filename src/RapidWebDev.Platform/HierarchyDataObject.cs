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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Hierarchy Data Business Object.
	/// </summary>
	[Serializable]
	[DataContract(Namespace = ServiceNamespaces.Platform)]
	public class HierarchyDataObject : AbstractExtensionBizObject, ICloneable
	{
		/// <summary>
		/// Hierarchy Data Id
		/// </summary>
		[DataMember]
		public Guid HierarchyDataId { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Code
		/// </summary>
		[DataMember]
		public string Code { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Hierarchy Type
		/// </summary>
		[DataMember]
		public string HierarchyType { get; set; }

		/// <summary>
		/// Parent Hierarchy Data Id
		/// </summary>
		[DataMember]
		public Guid? ParentHierarchyDataId { get; set; }

		/// <summary>
		/// Created by
		/// </summary>
		[DataMember]
		public Guid CreatedBy { get;  set; }

		/// <summary>
		/// Created date
		/// </summary>
		[DataMember]
		public DateTime CreatedDate { get;  set; }

		/// <summary>
		/// Last updated by
		/// </summary>
		[DataMember]
		public Guid? LastUpdatedBy { get;  set; }

		/// <summary>
		/// Last updated date
		/// </summary>
		[DataMember]
		public DateTime? LastUpdatedDate { get;  set; }

		/// <summary>
		/// Returns the value of property "Name".
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Name;
		}

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion

		/// <summary>
		/// Get the copy of current object.
		/// </summary>
		/// <returns></returns>
		public HierarchyDataObject Clone()
		{
			HierarchyDataObject copy = new HierarchyDataObject
			{
				HierarchyDataId = this.HierarchyDataId,
				Name = this.Name,
				Description = this.Description,
				ParentHierarchyDataId = this.ParentHierarchyDataId,
				HierarchyType = this.HierarchyType,
				CreatedBy = this.CreatedBy,
				CreatedDate = this.CreatedDate,
				LastUpdatedBy = this.LastUpdatedBy,
				LastUpdatedDate = this.LastUpdatedDate,
				ExtensionDataTypeId = this.ExtensionDataTypeId
			};

			this.ClonePropertiesTo(copy);
			return copy;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (this.HierarchyDataId == default(Guid))
				return base.GetHashCode();
			else
				return this.HierarchyDataId.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			HierarchyDataObject hierarchyDataObject = obj as HierarchyDataObject;
			if (hierarchyDataObject == null) return false;

			return this.GetHashCode() == hierarchyDataObject.GetHashCode();
		}
	}
}

