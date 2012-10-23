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
using System.Reflection;
using System.Text;
using System.Web.Security;
using System.Security.Principal;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.ExtensionModel;
using System.Runtime.Serialization;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Business object for application
	/// </summary>
	[Serializable]
    [DataContract]
	public class ApplicationObject : AbstractExtensionBizObject, ICloneable
	{
		/// <summary>
		/// Application identifier
		/// </summary>
		[DataMember]
        public Guid Id { get; set; }

		/// <summary>
		/// Application name
		/// </summary>
        [DataMember]
        public string Name { get; set; }

		/// <summary>
		/// Application description
		/// </summary>
        [DataMember]
        public string Description { get; set; }

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
		public ApplicationObject Clone()
		{
			ApplicationObject copy = new ApplicationObject
			{
				Id = this.Id,
				Name = this.Name,
				Description = this.Description,
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
			if (this.Id == default(Guid))
				return base.GetHashCode();
			else
				return this.Id.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ApplicationObject applicationObject = obj as ApplicationObject;
			if (applicationObject == null) return false;

			return this.GetHashCode() == applicationObject.GetHashCode();
		}
	}
}

