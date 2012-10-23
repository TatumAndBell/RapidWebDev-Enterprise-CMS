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
	/// Role element.
	/// </summary>
	[DataContract(Namespace = ServiceNamespaces.Platform)]
	[Serializable]
	public class RoleObject : ICloneable
	{
		/// <summary>
		/// Sets/gets role id.
		/// </summary>
		[DataMember]
		public Guid RoleId { get; set; }

		/// <summary>
		/// Sets/gets role name.
		/// </summary>
		[DataMember]
		public string RoleName { get; set; }

		/// <summary>
		/// Sets/gets role description
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Sets/gets domain which the role belongs to.
		/// </summary>
		[DataMember]
		public string Domain { get; set; }

		/// <summary>
		/// Sets/gets true if role is predefined.
		/// </summary>
		[DataMember]
		public bool Predefined { get; set; }

		/// <summary>
		/// Display roleElement as role name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.RoleName;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (this.RoleId == default(Guid))
				return base.GetHashCode();
			else
				return this.RoleId.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			RoleObject roleObject = obj as RoleObject;
			if (roleObject == null) return false;

			return this.GetHashCode() == roleObject.GetHashCode();
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
		public RoleObject Clone()
		{
			return new RoleObject
			{
				RoleId = this.RoleId,
				RoleName = this.RoleName,
				Domain = this.Domain,
				Predefined = this.Predefined,
				Description = this.Description,
			};
		}
	}
}
