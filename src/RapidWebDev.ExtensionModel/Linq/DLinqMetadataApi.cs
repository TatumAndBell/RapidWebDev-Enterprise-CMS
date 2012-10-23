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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel.Properties;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.ExtensionModel.Linq
{
	/// <summary>
	/// Based on DLinq's metadata api
	/// </summary>
	public class DLinqMetadataApi : CachableApi, IMetadataApi
	{
		private static object syncObject = new object();
		private IApplicationContext applicationContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="DLinqMetadataApi"/> class.
		/// </summary>
		/// <param name="applicationContext">The application context.</param>
		public DLinqMetadataApi(IApplicationContext applicationContext) : base(applicationContext)
		{
			this.applicationContext = applicationContext;
		}

		/// <summary>
		/// Create current application's extension type metadata.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="category"></param>
		/// <param name="description"></param>
		/// <param name="objectMetadataType"></param>
		/// <param name="isGlobal"></param>
		/// <param name="parentObjectMetadataId"></param>
		/// <returns>created extension type id</returns>
		public Guid AddType(string name, string category, string description, ObjectMetadataTypes objectMetadataType, bool isGlobal, Guid? parentObjectMetadataId)
		{
			return AddType(name, category, description, objectMetadataType, isGlobal, parentObjectMetadataId, 1);
		}

		/// <summary>
		/// Create current application's extension type metadata.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="category">The category.</param>
		/// <param name="description">The description.</param>
		/// <param name="objectMetadataType">Type of the object metadata.</param>
		/// <param name="isGlobal">if set to <c>true</c> [is global].</param>
		/// <param name="parentObjectMetadataId">The parent object metadata id.</param>
		/// <param name="version">The version.</param>
		/// <returns></returns>
		public Guid AddType(string name, string category, string description, ObjectMetadataTypes objectMetadataType, bool isGlobal, Guid? parentObjectMetadataId, int version)
		{
			Kit.NotNull(name, "name");

			lock (syncObject)
			{
				using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
				{
					Guid applicationId = isGlobal ? Guid.Empty : this.applicationContext.ApplicationId;
					bool existedTypeName = ctx.ObjectMetadatas.Where(meta => meta.ApplicationId == applicationId && meta.Name == name).Count() > 0;
					if (existedTypeName)
						throw new ValidationException(string.Format(Resources.ExtensionTypeExist, name));

					if (parentObjectMetadataId.HasValue)
					{
						bool validParentObjectMetadataId = ctx.ObjectMetadatas.Where(meta => new[] { applicationId, Guid.Empty }.Contains(meta.ApplicationId) && meta.ObjectMetadataId == parentObjectMetadataId).Count() > 0;
						if (!validParentObjectMetadataId)
							throw new ValidationException(string.Format(Resources.IDofSpecifiedParentExtensionTypeIsInvalid, parentObjectMetadataId));
					}

					ObjectMetadata metadata = new ObjectMetadata
					{
						ApplicationId = applicationId,
						Name = name,
						Category = category,
						Description = description,
						ParentObjectMetadataId = parentObjectMetadataId,
						LastUpdatedOn = DateTime.UtcNow
					};

					ctx.ObjectMetadatas.InsertOnSubmit(metadata);
					ctx.SubmitChanges();

					this.ClearAllObjectMetadataFromCache();
					return metadata.ObjectMetadataId;
				}
			}
		}

		/// <summary>
		/// Update extension type metadata's description.
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <param name="name"></param>
		/// <param name="category"></param>
		/// <param name="description"></param>
		/// <param name="parentObjectMetadataId"></param>
		public void UpdateType(Guid objectMetadataId, string name, string category, string description, Guid? parentObjectMetadataId)
		{
			Kit.NotNull(name, "name");

			lock (syncObject)
			{
				using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
				{
					ObjectMetadata metadata = ctx.ObjectMetadatas.FirstOrDefault(meta => meta.ObjectMetadataId == objectMetadataId);
					bool existedTypeName = ctx.ObjectMetadatas.Where(meta => meta.ApplicationId == metadata.ApplicationId && meta.ObjectMetadataId != metadata.ObjectMetadataId && meta.Name == name).Count() > 0;
					if (existedTypeName)
						throw new ValidationException(string.Format(Resources.ExtensionTypeExist, name));

					if (metadata != null)
					{
						metadata.Name = name;
						metadata.Category = category;
						metadata.Description = description;
						metadata.ParentObjectMetadataId = parentObjectMetadataId;
						metadata.LastUpdatedOn = DateTime.UtcNow;
						ctx.SubmitChanges();

						this.ClearAllObjectMetadataFromCache();
					}
				}
			}
		}

		/// <summary>
		/// Delete extension type metadata
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <exception cref="ValidationException">Derived type cannot be deleted</exception>
		public void DeleteType(Guid objectMetadataId)
		{
			lock (syncObject)
			{
				using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
				{
					ObjectMetadata metadata = ctx.ObjectMetadatas.FirstOrDefault(meta => meta.ObjectMetadataId == objectMetadataId);
					if (metadata != null)
					{
						int childrenCount = (from meta in ctx.ObjectMetadatas where meta.ParentObjectMetadataId == objectMetadataId select meta).Count();
						if (childrenCount > 0)
							throw new ValidationException(Resources.SpecifiedExtensionTypeHasBeenReferenced);

						ctx.ObjectMetadatas.DeleteOnSubmit(metadata);

						IEnumerable<FieldMetadata> fieldMetadataEnumerable = ctx.FieldMetadatas.Where(f => f.ObjectMetadataId == objectMetadataId);
						foreach (FieldMetadata fieldMetadata in fieldMetadataEnumerable)
							ctx.FieldMetadatas.DeleteOnSubmit(fieldMetadata);

						ctx.SubmitChanges();
						this.ClearAllObjectMetadataFromCache();
					}
				}
			}
		}

		/// <summary>
		/// Get extension type metadata
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <returns></returns>
		public IObjectMetadata GetType(Guid objectMetadataId)
		{
			lock (syncObject)
			{
				return this.GetAllObjectMetadataFromCache().FirstOrDefault(meta => meta.Id == objectMetadataId);
			}
		}

		/// <summary>
		/// Get extension type metadata,first get from current domain, then get from global, if both cannot find, return null.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IObjectMetadata GetType(string name)
		{
			lock (syncObject)
			{
				IEnumerable<IObjectMetadata> allObjectMetadata = this.GetAllObjectMetadataFromCache();
				return allObjectMetadata.FirstOrDefault(t => !t.IsGlobalObjectMetadata && t.Name == name) ?? allObjectMetadata.FirstOrDefault(t => t.IsGlobalObjectMetadata && t.Name == name);
			}
		}

		/// <summary>
		/// Saves the extension type metadata
		/// </summary>
		/// <param name="objectMetadataId">The object metadata id.</param>
		/// <param name="fieldMetadata">The field metadata.</param>
		public void SaveField(Guid objectMetadataId, IFieldMetadata fieldMetadata)
		{
			Kit.NotNull(fieldMetadata, "fieldMetadata");
			Kit.NotNull(fieldMetadata.Name, "fieldMetadata.Name");

			lock (syncObject)
			{
				this.DeleteField(objectMetadataId, fieldMetadata.Name);

				using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
				{
					FieldMetadata entity = new FieldMetadata
					{
						Name = fieldMetadata.Name,
						Priviledge = fieldMetadata.Priviledge,
						FieldGroup = fieldMetadata.FieldGroup,
						Description = fieldMetadata.Description,
						ObjectMetadataId = objectMetadataId,
						Ordinal = fieldMetadata.Ordinal,
						Type = fieldMetadata.Type,
						Definition = SerializeFieldMetadata(fieldMetadata)
					};

					ctx.FieldMetadatas.InsertOnSubmit(entity);
					ctx.SubmitChanges();

					fieldMetadata.Id = entity.FieldMetadataId;
				}
			}
		}

		/// <summary>
		/// Delete a field of specified object.
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <param name="fieldName"></param>
		public void DeleteField(Guid objectMetadataId, string fieldName)
		{
			Kit.NotNull(fieldName, "fieldName");

			lock (syncObject)
			{
				using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
				{
					FieldMetadata fieldMetadata = ctx.FieldMetadatas.FirstOrDefault(f => f.ObjectMetadataId == objectMetadataId && f.Name == fieldName);

					//Because first do deletion  when save field, for saving not existing field, this fieldMetadata will be null, then do following cache clean.
					if (fieldMetadata != null)
					{
						ctx.FieldMetadatas.DeleteOnSubmit(fieldMetadata);
						ctx.SubmitChanges();
					}

					this.ClearAllFieldMetadataFromCache();
				}
			}
		}

		/// <summary>
		/// Get extension type's specified attribute's metadata.
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public IFieldMetadata GetField(Guid objectMetadataId, string fieldName)
		{
			Kit.NotNull(fieldName, "fieldName");

			IEnumerable<IFieldMetadata> fieldMetadataEnumerable = this.GetFields(objectMetadataId);
			return fieldMetadataEnumerable.FirstOrDefault(f => f.Name == fieldName);
		}

		/// <summary>
		/// Get extension type's specified attribute's metadata.
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <param name="fieldId"></param>
		/// <returns></returns>
		public IFieldMetadata GetField(Guid objectMetadataId, Guid fieldId)
		{
			IEnumerable<IFieldMetadata> fieldMetadataEnumerable = this.GetFields(objectMetadataId);
			return fieldMetadataEnumerable.FirstOrDefault(f => f.Id == fieldId);
		}

		/// <summary>
		/// Get all attribute of extension type, ordered by Ordinal property.
		/// </summary>
		/// <param name="objectMetadataId"></param>
		/// <returns></returns>
		public IEnumerable<IFieldMetadata> GetFields(Guid objectMetadataId)
		{
			lock (syncObject)
			{
				var allFieldMetadata = this.GetAllFieldMetadataFromCache();
				if (allFieldMetadata.ContainsKey(objectMetadataId))
					return allFieldMetadata[objectMetadataId];

				return new List<IFieldMetadata>();
			}
		}

		#region Caching On All Object Metadata

		private const string AllObjectMetadataCacheKey = "AllCachedObjectMetadata";

		private IEnumerable<IObjectMetadata> GetAllObjectMetadataFromCache()
		{
			List<IObjectMetadata> results = base.GetCacheObject<List<IObjectMetadata>>(AllObjectMetadataCacheKey);
			if (results != null) return results;

			using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
			{
				results = new List<IObjectMetadata>();
				Guid[] supportedApplicationIds = new[] { Guid.Empty, this.applicationContext.ApplicationId };
				List<ObjectMetadata> allObjectMetadata = ctx.ObjectMetadatas.Where(t => supportedApplicationIds.Contains(t.ApplicationId)).ToList();
				foreach (ObjectMetadata entity in allObjectMetadata)
				{
					results.Add(new ObjectMetadataImpl
					{
						Id = entity.ObjectMetadataId,
						Name = entity.Name,
						Category = entity.Category,
						Description = entity.Description,
						IsGlobalObjectMetadata = entity.ApplicationId == Guid.Empty,
						LastUpdatedOn = LocalizationUtility.ConvertUtcTimeToClientTime(entity.LastUpdatedOn),
						ParentObjectMetadataId = entity.ParentObjectMetadataId
					});
				}

				base.AddCache(AllObjectMetadataCacheKey, results);
			}

			return results;
		}

		private void ClearAllObjectMetadataFromCache()
		{
			base.RemoveCache(AllObjectMetadataCacheKey);

			this.ClearAllFieldMetadataFromCache();
		}

		#endregion

		#region Caching On All Field Metadata

		private const string AllFieldMetadataCacheKey = "AllCachedFieldMetadata";

		private IDictionary<Guid, IEnumerable<IFieldMetadata>> GetAllFieldMetadataFromCache()
		{
			IDictionary<Guid, IEnumerable<IFieldMetadata>> results = base.GetCacheObject<IDictionary<Guid, IEnumerable<IFieldMetadata>>>(AllFieldMetadataCacheKey);
			if (results != null) return results;

			// Check whehter other type derived from it.
			IEnumerable<IObjectMetadata> allObjectMetadata = this.GetAllObjectMetadataFromCache();
			Guid[] allObjectMetadataIds = allObjectMetadata.Select(meta => meta.Id).ToArray();

			// Get current extension type's property
			using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
			{
				Dictionary<Guid, List<FieldMetadata>> fieldMetadataEntityDictionary = (from field in ctx.FieldMetadatas
															   where allObjectMetadataIds.Contains(field.ObjectMetadataId)
															   group field by field.ObjectMetadataId into g
															   select g).ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());

				Dictionary<Guid, IEnumerable<IFieldMetadata>> fieldsByObjectWithoutInherited = new Dictionary<Guid, IEnumerable<IFieldMetadata>>();
				foreach (Guid objectMetadataId in fieldMetadataEntityDictionary.Keys)
				{
					List<IFieldMetadata> fieldMetadataList = (from entity in fieldMetadataEntityDictionary[objectMetadataId]
															  orderby entity.Ordinal
															  select DeserializeFieldMetadata(entity)).ToList();
					fieldsByObjectWithoutInherited[objectMetadataId] = fieldMetadataList;
				}

				results = new Dictionary<Guid, IEnumerable<IFieldMetadata>>();
				foreach (Guid objectMetadataId in allObjectMetadataIds)
					results[objectMetadataId] = this.ResolveFieldsIncludesInherited(fieldsByObjectWithoutInherited, allObjectMetadata, objectMetadataId);
			}

			base.AddCache(AllFieldMetadataCacheKey, results);
			return results;
		}

		private void ClearAllFieldMetadataFromCache()
		{
			base.RemoveCache(AllFieldMetadataCacheKey);
		}

		private IEnumerable<IFieldMetadata> ResolveFieldsIncludesInherited(Dictionary<Guid, IEnumerable<IFieldMetadata>> fieldsByObjectWithoutInherited, IEnumerable<IObjectMetadata> allObjectMetadata, Guid thisObjectMetadataId)
		{
			List<IFieldMetadata> allFieldMetadata = new List<IFieldMetadata>();
			IObjectMetadata thisObjectMetadata = allObjectMetadata.FirstOrDefault(meta => meta.Id == thisObjectMetadataId);
			if (thisObjectMetadata == null) return new List<IFieldMetadata>();

			Dictionary<string, IFieldMetadata> results = new Dictionary<string, IFieldMetadata>();
			if (thisObjectMetadata.ParentObjectMetadataId.HasValue)
			{
				IEnumerable<IFieldMetadata> parentFields = this.ResolveFieldsIncludesInherited(fieldsByObjectWithoutInherited, allObjectMetadata, thisObjectMetadata.ParentObjectMetadataId.Value);
				foreach (IFieldMetadata parentField in parentFields)
				{
					IFieldMetadata parentFieldCopy = parentField.Clone() as IFieldMetadata;
					parentFieldCopy.Inherited = true;
					results[parentField.Name] = parentFieldCopy;
				}
			}

			if (fieldsByObjectWithoutInherited.ContainsKey(thisObjectMetadataId))
			{
				foreach (IFieldMetadata fieldMetadata in fieldsByObjectWithoutInherited[thisObjectMetadataId])
				{
					IFieldMetadata fieldMetadataCopy = fieldMetadata.Clone() as IFieldMetadata;
					fieldMetadataCopy.Inherited = false;
					results[fieldMetadata.Name] = fieldMetadataCopy;
				}
			}

			return results.Values.OrderBy(f => f.Ordinal).ToList();
		}

		#endregion

		#region Serialize / Deserialize Field Metadata

		private static string SerializeFieldMetadata(IFieldMetadata fieldMetadata)
		{
			Kit.NotNull(fieldMetadata, "fieldMetadata");

			XmlSerializer serializer = new XmlSerializer(fieldMetadata.GetType());
			StringBuilder output = new StringBuilder();
			using (StringWriter writer = new StringWriter(output))
			{
				serializer.Serialize(writer, fieldMetadata);
			}

			return output.ToString();
		}

		private static IFieldMetadata DeserializeFieldMetadata(FieldMetadata fieldMetadata)
		{
			if (Kit.IsEmpty(fieldMetadata.Definition)) return null;

			string fieldMetadataTypeName = string.Format("RapidWebDev.ExtensionModel.{0}FieldMetadata, RapidWebDev.ExtensionModel", fieldMetadata.Type);
			Type fieldMetadataType = Type.GetType(fieldMetadataTypeName, true);

			XmlSerializer serializer = new XmlSerializer(fieldMetadataType);
			using (StringReader stringReader = new StringReader(fieldMetadata.Definition))
			{
				IFieldMetadata fieldMetadataImpl = serializer.Deserialize(stringReader) as IFieldMetadata;
				fieldMetadataImpl.Id = fieldMetadata.FieldMetadataId;
				fieldMetadataImpl.FieldGroup = fieldMetadata.FieldGroup;
				fieldMetadataImpl.Priviledge = fieldMetadata.Priviledge;
				fieldMetadataImpl.Ordinal = fieldMetadata.Ordinal;
				fieldMetadataImpl.Description = fieldMetadata.Description;
				fieldMetadataImpl.Inherited = false;
				return fieldMetadataImpl;
			}
		}

		#endregion
	}
}
