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
using System.Globalization;
using System.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;

namespace RapidWebDev.Platform.Linq
{
	/// <summary>
	/// The implementation to manage relationship between 2 entities using linq-to-sql.
	/// </summary>
	public class RelationshipApi : CachableApi, IRelationshipApi
	{
		private IAuthenticationContext authenticationContext;

		/// <summary>
		/// Construct RelationshipApi
		/// </summary>
		/// <param name="authenticationContext"></param>
		public RelationshipApi(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
			this.authenticationContext = authenticationContext;
		}

		/// <summary>
		/// Save relationship b/w 2 entities on special relationship type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relatedObjectId"></param>
		/// <param name="relationshipType"></param>
		public void Save(Guid objectId, Guid relatedObjectId, string relationshipType)
		{
			this.Save(objectId, new RelationshipObject { RelationshipType = relationshipType, ReferenceObjectId = relatedObjectId });
		}

		/// <summary>
		/// Save relationship b/w 2 entities on special relationship type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationship"></param>
		public void Save(Guid objectId, RelationshipObject relationship)
		{
			this.Save(objectId, new[] { relationship });
		}

		/// <summary>
		/// Save a collection of objects related to an object.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationships"></param>
		public void Save(Guid objectId, IEnumerable<RelationshipObject> relationships)
		{
			Kit.NotNull(relationships, "relationships");
			if (relationships.Count() == 0) return;

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				IEnumerable<RelationshipObject> uniqueRelationships = DistinctRelationshipKeyValuePairs(relationships);
				foreach (RelationshipObject relationship in uniqueRelationships)
				{
					Relationship relationshipEntity = this.GetRelationshipFromDb(ctx, objectId, relationship.ReferenceObjectId, relationship.RelationshipType);
					if (relationshipEntity != null) continue;

					Guid smallId = IsXLessThanY(objectId, relationship.ReferenceObjectId) ? objectId : relationship.ReferenceObjectId;
					Guid bigId = smallId != objectId ? objectId : relationship.ReferenceObjectId;
					ctx.Relationships.InsertOnSubmit(new Relationship
					{
						ApplicationId = authenticationContext.ApplicationId,
						ObjectXId = smallId,
						ObjectYId = bigId,
						RelationshipType = relationship.RelationshipType,
						Ordinal = relationship.Ordinal
					});

					base.RemoveCache(objectId);
					base.RemoveCache(relationship.ReferenceObjectId);
				}

				ctx.SubmitChanges();
			}
		}

		/// <summary>
		/// Remove any relationships with specified object.
		/// </summary>
		/// <param name="objectId"></param>
		public void Remove(Guid objectId)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				List<Relationship> relationshipToDelete = (from r in ctx.Relationships
														   where r.ApplicationId == authenticationContext.ApplicationId
															  && (r.ObjectXId == objectId || r.ObjectYId == objectId)
														   select r).ToList();

				ctx.Relationships.DeleteAllOnSubmit(relationshipToDelete);
				ctx.SubmitChanges();

				relationshipToDelete.ForEach(r =>
					{
						base.RemoveCache(r.ObjectXId);
						base.RemoveCache(r.ObjectYId);
					});
			}
		}

		/// <summary>
		/// Remove any relationships with specified object in specified relationship type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationshipType"></param>
		public void Remove(Guid objectId, string relationshipType)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				List<Relationship> relationshipToDelete = (from r in ctx.Relationships
														   where r.ApplicationId == authenticationContext.ApplicationId
															  && (r.ObjectXId == objectId || r.ObjectYId == objectId)
															  && r.RelationshipType == relationshipType
														   select r).ToList();

				ctx.Relationships.DeleteAllOnSubmit(relationshipToDelete);
				ctx.SubmitChanges();

				relationshipToDelete.ForEach(r =>
				{
					base.RemoveCache(r.ObjectXId);
					base.RemoveCache(r.ObjectYId);
				});
			}
		}

		/// <summary>
		/// Remove any relationship b/w X and Y in any relationship types.
		/// </summary>
		/// <param name="objectXId"></param>
		/// <param name="objectYId"></param>
		public void Remove(Guid objectXId, Guid objectYId)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				Guid smallId = IsXLessThanY(objectXId, objectYId) ? objectXId : objectYId;
				Guid bigId = smallId != objectXId ? objectXId : objectYId;
				List<Relationship> relationshipToDelete = (from r in ctx.Relationships
																 where r.ApplicationId == authenticationContext.ApplicationId
																	&& r.ObjectXId == smallId
																	&& r.ObjectYId == bigId
																 select r).ToList();

				ctx.Relationships.DeleteAllOnSubmit(relationshipToDelete);
				ctx.SubmitChanges();

				base.RemoveCache(objectXId);
				base.RemoveCache(objectYId);
			}
		}

		/// <summary>
		/// Remove the relationship b/w X and Y in the special relationship type.
		/// </summary>
		/// <param name="objectXId"></param>
		/// <param name="objectYId"></param>
		/// <param name="relationshipType"></param>
		public void Remove(Guid objectXId, Guid objectYId, string relationshipType)
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				Guid smallId = IsXLessThanY(objectXId, objectYId) ? objectXId : objectYId;
				Guid bigId = smallId != objectXId ? objectXId : objectYId;
				List<Relationship> relationshipToDelete = (from r in ctx.Relationships
														   where r.ApplicationId == authenticationContext.ApplicationId
															  && r.ObjectXId == smallId
															  && r.ObjectYId == bigId
															  && r.RelationshipType == relationshipType
														   select r).ToList();

				ctx.Relationships.DeleteAllOnSubmit(relationshipToDelete);
				ctx.SubmitChanges();

				base.RemoveCache(objectXId);
				base.RemoveCache(objectYId);
			}
		}

		/// <summary>
		/// Get the object related to the target object in special type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationshipType"></param>
		/// <returns></returns>
		public RelationshipObject GetOneToOne(Guid objectId, string relationshipType)
		{
			return this.FindAllRelationship(objectId).FirstOrDefault(kvp => kvp.RelationshipType == relationshipType);
		}

		/// <summary>
		/// Get the object related to the target object in special type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationshipType"></param>
		/// <returns></returns>
		public RelationshipObject GetManyToOne(Guid objectId, string relationshipType)
		{
			return this.GetOneToOne(objectId, relationshipType);
		}

		/// <summary>
		/// Get the object related to the target object in special type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <param name="relationshipType"></param>
		/// <returns></returns>
		public IEnumerable<RelationshipObject> GetOneToMany(Guid objectId, string relationshipType)
		{
			return this.FindAllRelationship(objectId).Where(kvp => kvp.RelationshipType == relationshipType).OrderBy(r => r.Ordinal);
		}

		/// <summary>
		/// Find all objects related to the target object in any special relationship type.
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		public IEnumerable<RelationshipObject> FindAllRelationship(Guid objectId)
		{
			CloneableList<RelationshipObject> results = base.GetCacheObject<CloneableList<RelationshipObject>>(objectId);
			if (results != null) return results;

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				List<Relationship> relationships = (from r in ctx.Relationships
													where r.ApplicationId == authenticationContext.ApplicationId
													   && (r.ObjectXId == objectId || r.ObjectYId == objectId)
													orderby r.Ordinal ascending
													select r).ToList();

				HashSet<string> processedKeyValuePair = new HashSet<string>();
				results = new CloneableList<RelationshipObject>();
				foreach (Relationship relationship in relationships)
				{
					Guid relatedObjectId = objectId != relationship.ObjectXId ? relationship.ObjectXId : relationship.ObjectYId;
					string key = string.Format(CultureInfo.InvariantCulture, "{0}::{1}", relationship.RelationshipType, relatedObjectId);
					if (processedKeyValuePair.Contains(key)) continue;

					processedKeyValuePair.Add(key);
					results.Add(new RelationshipObject 
					{ 
						RelationshipType = relationship.RelationshipType,
						ReferenceObjectId = relatedObjectId,
						Ordinal = relationship.Ordinal
					});
				}

				base.AddCache(objectId, results);
			}

			return results;
		}

		private Relationship GetRelationshipFromDb(MembershipDataContext ctx, Guid objectXId, Guid objectYId, string relationshipType)
		{
			Guid smallId = IsXLessThanY(objectXId, objectYId) ? objectXId : objectYId;
			Guid bigId = smallId != objectXId ? objectXId : objectYId;
			var q = from r in ctx.Relationships
					where r.ApplicationId == authenticationContext.ApplicationId
						&& r.ObjectXId == smallId
						&& r.ObjectYId == bigId
						&& r.RelationshipType == relationshipType
					select r;

			return q.FirstOrDefault();
		}

		private static IEnumerable<RelationshipObject>  DistinctRelationshipKeyValuePairs(IEnumerable<RelationshipObject> inputObjects)
		{
			List<RelationshipObject> results = new List<RelationshipObject>();
			HashSet<string> processedKeyValuePair = new HashSet<string>();
			foreach (RelationshipObject relationship in inputObjects)
			{
				string key = string.Format(CultureInfo.InvariantCulture, "{0}::{1}", relationship.RelationshipType, relationship.ReferenceObjectId);
				if (processedKeyValuePair.Contains(key)) continue;

				processedKeyValuePair.Add(key);
				results.Add(relationship);
			}

			return results;
		}

		private static bool IsXLessThanY(Guid x, Guid y)
		{
			string xValue = x.ToString();
			string yValue = y.ToString();
			return string.Compare(xValue, yValue, true) < 0;
		}
	}
}
