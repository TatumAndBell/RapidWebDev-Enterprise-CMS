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
using System.Linq.Expressions;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using RapidWebDev.Platform.Properties;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.Platform.Linq
{
	/// <summary>
	/// The concrete data API is used for CRUD constant values implemented in linq-2-sql.
	/// The method executed add/update depends on whether identity of object is empty or not.
	/// Take an example to explain the business value of concrete data API.
	/// In a product-order system, the product may have many properties which have candidate property values for users to select.
	/// The product may have Size property with three candidate values "Big", "Medium" and "Small" and Color property with 2 candidate values "Light" and "Dark".
	/// Then we can categorize that there are 3 concrete data objects "Big", "Medium" and "Small" in concrete data type "Size" as 2 concrete data objects "Light" and "Dark" in concrete data type "Color" in concrete model provided in RapidWebDev.
	/// Generally, with Concrete Data Model, we have avoided to design database schema and implement data access API to CRUD constant values.
	/// And With extension model integrated into concrete data model, we can define the dynamic properties for concrete data based on the needs of business. 
	/// </summary>
	public class ConcreteDataApi : CachableApi, IConcreteDataApi
	{
		private IAuthenticationContext authenticationContext;

		/// <summary>
		/// Construct ConcreteDataApi
		/// </summary>
		/// <param name="authenticationContext"></param>
		public ConcreteDataApi(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
			this.authenticationContext = authenticationContext;
		}

		/// <summary>
		/// The method executed add/update depends on whether identity of object is empty or not.
		/// </summary>
		/// <param name="concreteDataObject">The name of concrete data should be unique in a concrete data type.</param>
		public void Save(ConcreteDataObject concreteDataObject)
		{
			Kit.NotNull(concreteDataObject, "concreteDataObject");
			Kit.NotNull(concreteDataObject.Name, "concreteDataObject.Name");
			Kit.NotNull(concreteDataObject.Type, "concreteDataObject.Type");

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				ConcreteData concreteData = ctx.ConcreteDatas.FirstOrDefault(c => c.ApplicationId == authenticationContext.ApplicationId && c.Name == concreteDataObject.Name && c.Type == concreteDataObject.Type);


				// validate whether Name is unique in a concrete type.
				using (ValidationScope validationScope = new ValidationScope())
				{
					if (concreteData != null && concreteData.ConcreteDataId != concreteDataObject.ConcreteDataId)
					{
						validationScope.Error(Resources.DuplicateConcreteDataName, concreteData.Name);
						return;
					}
				}

				if (concreteDataObject.ConcreteDataId == Guid.Empty)
				{
					concreteData = ExtensionObjectFactory.Create<ConcreteData>(concreteDataObject);
					concreteData.CreatedBy = authenticationContext.User.UserId;
					concreteData.CreatedDate = DateTime.UtcNow;
					concreteData.DeleteStatus = DeleteStatus.NotDeleted;

					ctx.ConcreteDatas.InsertOnSubmit(concreteData);
				}
				else
				{
					concreteData = ctx.ConcreteDatas.FirstOrDefault(c => c.ApplicationId == authenticationContext.ApplicationId && c.ConcreteDataId == concreteDataObject.ConcreteDataId);
				}

				concreteData.ApplicationId = authenticationContext.ApplicationId;
				concreteData.Type = concreteDataObject.Type;
				concreteData.DeleteStatus = concreteDataObject.DeleteStatus;
				concreteData.Description = concreteDataObject.Description;
				concreteData.Name = concreteDataObject.Name;
				concreteData.Value = concreteDataObject.Value;
				concreteData.LastUpdatedBy = authenticationContext.User.UserId;
				concreteData.LastUpdatedDate = DateTime.UtcNow;
				concreteData.ParseExtensionPropertiesFrom(concreteDataObject);

				ctx.SubmitChanges();

				// set server generated information back to the business object to return.
				concreteDataObject.ConcreteDataId = concreteData.ConcreteDataId;
				concreteDataObject.CreatedBy = concreteData.CreatedBy;
				concreteDataObject.CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(concreteData.CreatedDate);
				concreteDataObject.LastUpdatedBy = concreteData.LastUpdatedBy;
				concreteDataObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(concreteData.LastUpdatedDate);

				// remove the object from cache.
				base.RemoveCache(concreteDataObject.ConcreteDataId);
			}
		}

		/// <summary>
		/// Get concrete data by id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ConcreteDataObject GetById(Guid id)
		{
			ConcreteDataObject concreteDataObject = base.GetCacheObject<ConcreteDataObject>(id);
			
			// cannot load concrete data from cache, then try to load it from database.
			if (concreteDataObject == null)
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					ConcreteData entity = ctx.ConcreteDatas.FirstOrDefault(c => c.ApplicationId == authenticationContext.ApplicationId && c.ConcreteDataId == id);
					if (entity == null) return null;

					concreteDataObject = new ConcreteDataObject
					{
						ConcreteDataId = entity.ConcreteDataId,
						Type = entity.Type,
						CreatedBy = entity.CreatedBy,
						CreatedDate = entity.CreatedDate,
						DeleteStatus = entity.DeleteStatus,
						Description = entity.Description,
						ExtensionDataTypeId = entity.ExtensionDataTypeId,
						LastUpdatedBy = entity.LastUpdatedBy,
						LastUpdatedDate = entity.LastUpdatedDate,
						Name = entity.Name,
						Value = entity.Value
					};

					// parse dynamic properties.
					concreteDataObject.ParseExtensionPropertiesFrom(entity);

					// cache the business object with UTC datetime.
					base.AddCache(concreteDataObject.ConcreteDataId, concreteDataObject);
				}
			}

			// get the copy for timezone convertion.
			concreteDataObject = concreteDataObject.Clone();

			// convert UTC datetime into client timezone before return.
			concreteDataObject.CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(concreteDataObject.CreatedDate);
			concreteDataObject.LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(concreteDataObject.LastUpdatedDate);
			return concreteDataObject;
		}

		/// <summary>
		/// Get concrete data by name.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public ConcreteDataObject GetByName(string type, string name)
		{
			Kit.NotNull(type, "type");
			Kit.NotNull(name, "name");

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				ConcreteData entity = (from c in ctx.ConcreteDatas
									   where c.ApplicationId == authenticationContext.ApplicationId
										 && c.Type == type && c.Name == name
									   select c).FirstOrDefault();

				if (entity == null) return null;

				ConcreteDataObject concreteDataObject = new ConcreteDataObject
				{
					ConcreteDataId = entity.ConcreteDataId,
					Type = entity.Type,
					CreatedBy = entity.CreatedBy,
					CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(entity.CreatedDate),
					DeleteStatus = entity.DeleteStatus,
					Description = entity.Description,
					ExtensionDataTypeId = entity.ExtensionDataTypeId,
					LastUpdatedBy = entity.LastUpdatedBy,
					LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(entity.LastUpdatedDate),
					Name = entity.Name,
					Value = entity.Value
				};

				concreteDataObject.ParseExtensionPropertiesFrom(entity);
				return concreteDataObject;
			}
		}

		/// <summary>
		/// Find all concrete data objects includes soft deleted in the special type sorted by Name ascendingly.
		/// </summary>
		/// <param name="type">valid concrete data type</param>
		/// <returns></returns>
		public IEnumerable<ConcreteDataObject> FindAllByType(string type)
		{
			Kit.NotNull(type, "type");

			List<ConcreteDataObject> concreteDataObjects = new List<ConcreteDataObject>();
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				IEnumerable<ConcreteData> entities = ctx.ConcreteDatas.Where(c => c.ApplicationId == authenticationContext.ApplicationId && c.Type == type);
				foreach (ConcreteData entity in entities)
				{
					ConcreteDataObject concreteDataObject = new ConcreteDataObject
					{
						ConcreteDataId = entity.ConcreteDataId,
						Type = entity.Type,
						CreatedBy = entity.CreatedBy,
						CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(entity.CreatedDate),
						DeleteStatus = entity.DeleteStatus,
						Description = entity.Description,
						ExtensionDataTypeId = entity.ExtensionDataTypeId,
						LastUpdatedBy = entity.LastUpdatedBy,
						LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(entity.LastUpdatedDate),
						Name = entity.Name,
						Value = entity.Value
					};

					concreteDataObject.ParseExtensionPropertiesFrom(entity);
					concreteDataObjects.Add(concreteDataObject);
				}

				return concreteDataObjects;
			}
		}

		/// <summary>
		/// Find all available concrete data types.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> FindConcreteDataTypes()
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				return (from c in ctx.ConcreteDatas
										where c.ApplicationId == authenticationContext.ApplicationId
										select c.Type).Distinct().ToList();
			}
		}

		/// <summary>
		/// Find concrete data in all types by custom predicates.
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="orderby"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public IEnumerable<ConcreteDataObject> FindConcreteData(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount)
		{
			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					var q = from c in ctx.ConcreteDatas
							where c.ApplicationId == authenticationContext.ApplicationId
							select c;

					if (predicate != null && !string.IsNullOrEmpty(predicate.Expression))
						q = q.Where(predicate.Expression, predicate.Parameters);

					if (!string.IsNullOrEmpty(orderby))
						q = q.OrderBy(orderby);

					recordCount = q.Count();

					List<ConcreteDataObject> concreteDataObjects = new List<ConcreteDataObject>();
					List<ConcreteData> concreteDataEntities = q.Skip(pageIndex * pageSize).Take(pageSize).ToList();
					foreach (ConcreteData entity in concreteDataEntities)
					{
						ConcreteDataObject concreteDataObject = new ConcreteDataObject
						{
							ConcreteDataId = entity.ConcreteDataId,
							Type = entity.Type,
							CreatedBy = entity.CreatedBy,
							CreatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(entity.CreatedDate),
							DeleteStatus = entity.DeleteStatus,
							Description = entity.Description,
							ExtensionDataTypeId = entity.ExtensionDataTypeId,
							LastUpdatedBy = entity.LastUpdatedBy,
							LastUpdatedDate = LocalizationUtility.ConvertUtcTimeToClientTime(entity.LastUpdatedDate),
							Name = entity.Name,
							Value = entity.Value
						};

						concreteDataObject.ParseExtensionPropertiesFrom(entity);
						concreteDataObjects.Add(concreteDataObject);
					}

					return concreteDataObjects;
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}
	}
}