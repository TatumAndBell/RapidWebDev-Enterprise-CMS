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
using System.Runtime.Serialization;
using RapidWebDev.Common;
using System.Globalization;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// Relationship object
	/// </summary>
	[Serializable]
	[DataContract(Namespace = ServiceNamespaces.Platform)]
	public class RelationshipObject
	{
		/// <summary>
		/// Relationship category
		/// </summary>
		[DataMember]
		public string RelationshipType { get; set; }

		/// <summary>
		/// Reference object id.
		/// </summary>
		[DataMember]
		public Guid ReferenceObjectId { get; set; }

		/// <summary>
		/// Reference object ordinal.
		/// </summary>
		[DataMember]
		public int Ordinal { get; set; }

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(this.RelationshipType) && this.ReferenceObjectId != Guid.Empty)
				return string.Format(CultureInfo.InvariantCulture, "{1}@{0}", this.RelationshipType, this.ReferenceObjectId).GetHashCode();
			else
				return base.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			RelationshipObject relationshipObject = obj as RelationshipObject;
			if (relationshipObject == null) return false;

			return this.GetHashCode() == relationshipObject.GetHashCode();
		}
	}
}

