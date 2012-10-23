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

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The interface to manage relationship between 2 entities.
	/// </summary>
	public interface IRelationshipApi
	{
		/// <summary>
		/// Save relationship b/w 2 entities on special relationship type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relatedObjectId"></param>
		/// <param name="relationshipType"></param>
		void Save(Guid objectId, Guid relatedObjectId, string relationshipType);

		/// <summary>
		/// Save relationship b/w 2 entities on special relationship type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationship"></param>
		void Save(Guid objectId, RelationshipObject relationship);

		/// <summary>
		/// Save a collection of objects related to an object.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationships"></param>
		void Save(Guid objectId, IEnumerable<RelationshipObject> relationships);

		/// <summary>
		/// Remove any relationships with specified object.
		/// </summary>
		/// <param name="objectId"></param>
		void Remove(Guid objectId);

		/// <summary>
		/// Remove any relationships with specified object in specified relationship type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationshipType"></param>
		void Remove(Guid objectId, string relationshipType);

		/// <summary>
		/// Remove any relationship b/w X and Y in any relationship types.
		/// </summary>
		/// <param name="objectXId"></param>
		/// <param name="objectYId"></param>
		void Remove(Guid objectXId, Guid objectYId);

		/// <summary>
		/// Remove the relationship b/w X and Y in the special relationship type.
		/// </summary>
		/// <param name="objectXId"></param>
		/// <param name="objectYId"></param>
		/// <param name="relationshipType"></param>
		void Remove(Guid objectXId, Guid objectYId, string relationshipType);

		/// <summary>
		/// Get the object related to the target object in special type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationshipType"></param>
		/// <returns></returns>
		RelationshipObject GetOneToOne(Guid objectId, string relationshipType);

		/// <summary>
		/// Get the object related to the target object in special type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationshipType"></param>
		/// <returns></returns>
		RelationshipObject GetManyToOne(Guid objectId, string relationshipType);

		/// <summary>
		/// Get the object related to the target object in special type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationshipType"></param>
		/// <returns></returns>
		IEnumerable<RelationshipObject> GetOneToMany(Guid objectId, string relationshipType);

		/// <summary>
		/// Find all objects related to the target object in any special relationship type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		IEnumerable<RelationshipObject> FindAllRelationship(Guid objectId);
	}
}
