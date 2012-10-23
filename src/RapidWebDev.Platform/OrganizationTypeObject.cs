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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using RapidWebDev.Common;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Organization type business object.
	/// </summary>
	[Serializable]
	[DataContract(Namespace = ServiceNamespaces.Platform)]
	public class OrganizationTypeObject : ICloneable
	{
		/// <summary>
		/// Set/get organization type id.
		/// </summary>
		[DataMember]
		public Guid OrganizationTypeId { get; set; }

		/// <summary>
		/// Set/get organization type domain.
		/// </summary>
		[DataMember]
		public string Domain { get; set; }

		/// <summary>
		/// Set/get organization type name.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Set/get organization type description
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Sets/gets organization type last updated date.
		/// </summary>
		[DataMember]
		public DateTime LastUpdatedDate { get;  set; }

		/// <summary>
		/// Sets/gets if the organization type is predefined
		/// </summary>
		[DataMember]
		public bool Predefined { get; set; }

		/// <summary>
		/// Sets/gets delete status
		/// </summary>
		[DataMember]
		public DeleteStatus DeleteStatus { get; set; }

		/// <summary>
		/// Display this instance as organization type name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (this.OrganizationTypeId == default(Guid))
				return base.GetHashCode();
			else
				return this.OrganizationTypeId.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			OrganizationTypeObject organizationTypeObject = obj as OrganizationTypeObject;
			if (organizationTypeObject == null) return false;

			return this.GetHashCode() == organizationTypeObject.GetHashCode();
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
		public OrganizationTypeObject Clone()
		{
			return new OrganizationTypeObject
			{
				OrganizationTypeId = this.OrganizationTypeId,
				Domain = this.Domain,
				Predefined = this.Predefined,
				Name = this.Name,
				Description = this.Description,
				LastUpdatedDate = this.LastUpdatedDate
			};
		}
	}
}
