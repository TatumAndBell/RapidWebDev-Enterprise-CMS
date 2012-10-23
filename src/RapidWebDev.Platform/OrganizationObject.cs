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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Organization business object.
	/// </summary>
	[DataContract(Namespace = ServiceNamespaces.Platform)]
	[Serializable]
	public class OrganizationObject : AbstractExtensionBizObject, ICloneable
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OrganizationObject()
		{
			this.Hierarchies = new Dictionary<string, Guid>();
		}

		/// <summary>
		/// Gets/sets organization id.
		/// </summary>
		[DataMember]
		public Guid OrganizationId { get; set; }

		/// <summary>
		/// Gets/sets organization name.
		/// </summary>
		[DataMember]
		public string OrganizationName { get; set; }

		/// <summary>
		/// Gets/sets organization code.
		/// </summary>
		[DataMember]
		public string OrganizationCode { get; set; }

		/// <summary>
		/// Gets/sets organization type id.
		/// </summary>
		[DataMember]
		public Guid OrganizationTypeId { get; set; }

		/// <summary>
		/// Gets/sets organization description.
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// The hierarchies which the organization associated with.
		/// </summary>
		[DataMember]
		public Dictionary<string, Guid> Hierarchies { get; private set; }

		/// <summary>
		/// Gets/sets organization status.
		/// </summary>
		[DataMember]
		public OrganizationStatus Status { get; set; }

		/// <summary>
		/// Gets/sets id of user who created the organization.
		/// </summary>
		[DataMember]
		public Guid CreatedBy { get; internal set; }

		/// <summary>
		/// Gets/sets organization created date.
		/// </summary>
		[DataMember]
		public DateTime CreatedDate { get;  set; }

		/// <summary>
		/// Gets/sets id of user who updated the organization at last.
		/// </summary>
		[DataMember]
		public Guid LastUpdatedBy { get;  set; }

		/// <summary>
		/// Gets/sets organization last updated date.
		/// </summary>
		[DataMember]
		public DateTime LastUpdatedDate { get;  set; }

		/// <summary>
		/// Gets/sets parent organization id.
		/// </summary>
		[DataMember]
		public Guid? ParentOrganizationId { get; set; }

		/// <summary>
		/// Returns current organization as a string for displaying as "[organization name]".
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.OrganizationName;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (this.OrganizationId == default(Guid))
				return base.GetHashCode();
			else
				return this.OrganizationId.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			OrganizationObject organizationObject = obj as OrganizationObject;
			if (organizationObject == null) return false;

			return this.GetHashCode() == organizationObject.GetHashCode();
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
		public OrganizationObject Clone()
		{
			OrganizationObject copy = new OrganizationObject
			{
				OrganizationId = this.OrganizationId,
				Description = this.Description,
				ParentOrganizationId = this.ParentOrganizationId,
				Hierarchies = this.Hierarchies,
				Status = this.Status,
				OrganizationCode = this.OrganizationCode,
				OrganizationName = this.OrganizationName,
				OrganizationTypeId = this.OrganizationTypeId,
				CreatedBy = this.CreatedBy,
				CreatedDate = this.CreatedDate,
				LastUpdatedBy = this.LastUpdatedBy,
				LastUpdatedDate = this.LastUpdatedDate,
				ExtensionDataTypeId = this.ExtensionDataTypeId
			};

			this.ClonePropertiesTo(copy);
			return copy;
		}
	}

	
}