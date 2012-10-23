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
using System.Runtime.Serialization;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Concrete data business object
	/// </summary>
	[Serializable]
	[DataContract(Namespace = ServiceNamespaces.Platform)]
	public class ConcreteDataObject : AbstractExtensionBizObject, ICloneable
	{
		/// <summary>
		/// Id
		/// </summary>
		[DataMember]
		public Guid ConcreteDataId { get; set; }

		/// <summary>
		/// Type
		/// </summary>
		[DataMember]
		public string Type { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Value
		/// </summary>
		[DataMember]
		public string Value { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Created by
		/// </summary>
		[DataMember]
		public Guid CreatedBy { get;  set; }

		/// <summary>
		/// Created date
		/// </summary>
		[DataMember]
		public DateTime CreatedDate { get; set; }

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
		/// Delete status
		/// </summary>
		[DataMember]
		public DeleteStatus DeleteStatus { get; set; }

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
		public ConcreteDataObject Clone()
		{
			ConcreteDataObject copy = new ConcreteDataObject
			{
				ConcreteDataId = this.ConcreteDataId,
				Type = this.Type,
				Name = this.Name,
				Value = this.Value,
				DeleteStatus = this.DeleteStatus,
				Description = this.Description,
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
			if (this.ConcreteDataId == default(Guid))
				return base.GetHashCode();
			else
				return this.ConcreteDataId.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ConcreteDataObject concreteDataObject = obj as ConcreteDataObject;
			if (concreteDataObject == null) return false;

			return this.GetHashCode() == concreteDataObject.GetHashCode();
		}
	}
}

