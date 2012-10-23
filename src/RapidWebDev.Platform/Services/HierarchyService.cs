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
using System.Globalization;
using System.Linq;
using System.ServiceModel.Activation;
using RapidWebDev.Common;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// The service to operate areas
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class HierarchyService : IHierarchyService
    {
        private static IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

        /// <summary>
        /// Save a hierarchy data object. 
        /// </summary>
        /// <param name="hierarchyDataObject"></param>
        public string SaveJson(HierarchyDataObject hierarchyDataObject)
        {
            if (hierarchyDataObject == null) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.HierarchyObjectCannotBeNull));
            try
            {
                hierarchyApi.Save(hierarchyDataObject);
                return hierarchyDataObject.HierarchyDataId.ToString();
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }

        }

        /// <summary>
        /// Save a hierarchy data object. 
        /// </summary>
        /// <param name="hierarchyDataObject"></param>
        public string SaveXml(HierarchyDataObject hierarchyDataObject)
        {
            return SaveJson(hierarchyDataObject);
        }

        /// <summary>
        /// Get a hierarchy data object by id. 
        /// </summary>
        /// <param name="hierarchyDataId"></param>
        /// <returns></returns>
        public HierarchyDataObject GetHierarchyDataByIdJson(string hierarchyDataId)
        {
            if (string.IsNullOrEmpty(hierarchyDataId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyDataId));
            Guid id = Guid.Empty;
            try
            {
                id = new Guid(hierarchyDataId);
                HierarchyDataObject hierarchyDataObject = hierarchyApi.GetHierarchyData(id);
                if (hierarchyDataObject == null)
                    return null;
                return hierarchyDataObject;
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }

        }

        /// <summary>
        /// Get a hierarchy data object by id.
        /// </summary>
        /// <param name="hierarchyDataId"></param>
        /// <returns></returns>
        public HierarchyDataObject GetHierarchyDataByIdXml(string hierarchyDataId)
        {
            return GetHierarchyDataByIdJson(hierarchyDataId);
        }

        /// <summary>
        /// Get a hierarchy data object by name.
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <param name="hierarchyDataName"></param>
        /// <returns></returns>
        public HierarchyDataObject GetHierarchyDataByNameJson(string hierarchyType, string hierarchyDataName)
        {
            if (string.IsNullOrEmpty(hierarchyType)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyType));
            if (string.IsNullOrEmpty(hierarchyDataName)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyDataName));
            try
            {
                HierarchyDataObject hierarchyDataObject = hierarchyApi.GetHierarchyData(hierarchyType, hierarchyDataName);
                if (hierarchyDataObject == null)
                    return null;
                return hierarchyDataObject;
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Get a hierarchy data object by name.
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <param name="hierarchyDataName"></param>
        /// <returns></returns>
        public HierarchyDataObject GetHierarchyDataByNameXml(string hierarchyType, string hierarchyDataName)
        {
            return GetHierarchyDataByNameJson(hierarchyType, hierarchyDataName);
        }

        /// <summary>
        /// Get all children of the specified hierarchy data by id.<br/>
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <param name="parentHierarchyDataId"></param>
        /// <returns></returns>
        public Collection<HierarchyDataObject> GetImmediateChildrenJson(string hierarchyType, string parentHierarchyDataId)
        {
            if (string.IsNullOrEmpty(hierarchyType)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyType ));
            if (string.IsNullOrEmpty(parentHierarchyDataId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyDataId));
            Guid parentId = Guid.Empty;
            try
            {
                parentId = new Guid(parentHierarchyDataId);
                Collection<HierarchyDataObject> results = new Collection<HierarchyDataObject>(hierarchyApi.GetImmediateChildren(hierarchyType, parentId).ToList());
                if (results.Count() == 0)
                    return new Collection<HierarchyDataObject>();
                return results;
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }

        }

        /// <summary>
        /// Get all children of the specified hierarchy data by id.<br/>
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <param name="parentHierarchyDataId"></param>
        /// <returns></returns>
        public Collection<HierarchyDataObject> GetImmediateChildrenXml(string hierarchyType, string parentHierarchyDataId)
        {
            return GetImmediateChildrenJson(hierarchyType, parentHierarchyDataId);
        }

        /// <summary>
        /// Get all children of the specified hierarchy data includes not immediately.<br/>
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <param name="parentHierarchyDataId"></param>
        /// <returns></returns>
        public Collection<HierarchyDataObject> GetAllChildrenJson(string hierarchyType, string parentHierarchyDataId)
        {
            if (string.IsNullOrEmpty(hierarchyType))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyType));
            if (string.IsNullOrEmpty(parentHierarchyDataId))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyDataId));
            Guid parentId = Guid.Empty;
            try
            {
                parentId = new Guid(parentHierarchyDataId);
                Collection<HierarchyDataObject> results = new Collection<HierarchyDataObject>(hierarchyApi.GetAllChildren(hierarchyType, parentId).ToList());
                if (results.Count() == 0)
                    return new Collection<HierarchyDataObject>();
                return results;
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }

        }

        /// <summary>
        /// Get all children of the specified hierarchy data includes not immediately.<br/>
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <param name="parentHierarchyDataId"></param>
        /// <returns></returns>
        public Collection<HierarchyDataObject> GetAllChildrenXml(string hierarchyType, string parentHierarchyDataId)
        {
            return GetAllChildrenJson(hierarchyType, parentHierarchyDataId);
        }

        /// <summary>
        /// Get all hierarchy data in specified hierarchy type.<br/>
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <returns></returns>
        public Collection<HierarchyDataObject> GetAllHierarchyDataJson(string hierarchyType)
        {
            if (string.IsNullOrEmpty(hierarchyType)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyType));
            try
            {
                Collection<HierarchyDataObject> results =  new Collection<HierarchyDataObject>(hierarchyApi.GetAllHierarchyData(hierarchyType).ToList());
                if (results.Count() == 0)
                    return new Collection<HierarchyDataObject>();
                return results;
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Get all hierarchy data in specified hierarchy type.<br/>
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <returns></returns>
        public Collection<HierarchyDataObject> GetAllHierarchyDataXml(string hierarchyType)
        {
            return GetAllHierarchyDataJson(hierarchyType);
        }

        /// <summary>
        /// Hard delete a hierarchy data with all its children by id. <br/>
        /// </summary>
        /// <param name="hierarchyDataId"></param>
        public void HardDeleteHierarchyDataJson(string hierarchyDataId)
        {
            if (string.IsNullOrEmpty(hierarchyDataId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyDataId));
            Guid id = Guid.Empty;
            try
            {
                id = new Guid(hierarchyDataId);
                hierarchyApi.HardDeleteHierarchyData(id);
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }

        }

        /// <summary>
        /// Hard delete a hierarchy data with all its children by id. <br/>
        /// </summary>
        /// <param name="hierarchyDataId"></param>
        public void HardDeleteHierarchyDataXml(string hierarchyDataId)
        {
            HardDeleteHierarchyDataJson(hierarchyDataId);
        }

        /// <summary>
        /// Find all hierarchies which include the query keyword in code or name.
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <param name="query"></param>
        /// <param name="maxReturnedCount"></param>
        /// <returns></returns>
        public Collection<HierarchyDataObject> FindByKeywordJson(string hierarchyType, string query, int maxReturnedCount)
        {
            if (string.IsNullOrEmpty(hierarchyType))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidHierarchyType, hierarchyType));

            try
            {
                int recordCount;
                LinqPredicate predicate = new LinqPredicate("HierarchyType=@0", hierarchyType);
                if (!string.IsNullOrEmpty(query))
                    predicate = predicate.Add("(Code!=null AND Code.StartsWith(@0)) OR (Name!=null AND Name.StartsWith(@0))", query.Replace("-", "").Trim());

                IEnumerable<HierarchyDataObject> hierarchyDataObjects = hierarchyApi.FindHierarchyData(predicate, "Name ASC", 0, maxReturnedCount, out recordCount);

                Collection<HierarchyDataObject> results = new Collection<HierarchyDataObject>();
                IEnumerable<HierarchyDataObject> dummyRootHierarchyDataObjects = FindHierarchyDataObjectWithParentNotExist(hierarchyDataObjects);
                foreach (HierarchyDataObject dummyRootHierarchyDataObject in dummyRootHierarchyDataObjects)
                    HierarchizeHierarchyData(hierarchyDataObjects, dummyRootHierarchyDataObject, results, 0);

                if (results.Count() == 0)
                    return new Collection<HierarchyDataObject>();
                return results;
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Find all hierarchies which include the query keyword in code or name.
        /// </summary>
        /// <param name="hierarchyType"></param>
        /// <param name="query"></param>
        /// <param name="maxReturnedCount"></param>
        /// <returns></returns>
        public Collection<HierarchyDataObject> FindByKeywordXml(string hierarchyType, string query, int maxReturnedCount)
        {
            return this.FindByKeywordJson(hierarchyType, query, maxReturnedCount);
        }

        /// <summary>
        /// Query hierarchy data in all types by custom predicates.<br/>
        /// </summary>
        /// <param name="orderby">dynamic orderby command</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="predicate">linq predicate which supports properties of <see cref="RapidWebDev.Platform.HierarchyDataObject"/> for query expression.</param>
        /// <returns></returns>
        public HierarchyDataQueryResult QueryHierarchyDataJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            int recordCount;
            try
            {
                pageSize = (pageSize == 0) ? 25 : pageSize;
                LinqPredicate linqPredicate = ServicesHelper.ConvertWebPredicateToLinqPredicate(predicate);
                IEnumerable<HierarchyDataObject> rets = hierarchyApi.FindHierarchyData(linqPredicate, orderby, pageIndex, pageSize, out recordCount);
             
                HierarchyDataQueryResult results = new HierarchyDataQueryResult(rets.ToList())
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecordCount = recordCount
                };
                return results;
            }
            catch (ArgumentException ex)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, ex.Message));
            }
            catch (BadRequestException bad)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, bad.Message));
            }
            catch (FormatException formatEx)
            {
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, formatEx.Message));
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw new InternalServerErrorException();
            }
        }

        /// <summary>
        /// Query hierarchy data in all types by custom predicates.<br/>
        /// </summary>
        /// <param name="orderby">dynamic orderby command</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="predicate">linq predicate which supports properties of <see cref="RapidWebDev.Platform.HierarchyDataObject"/> for query expression.</param>
        /// <returns></returns>
        public HierarchyDataQueryResult QueryHierarchyDataXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            return QueryHierarchyDataJson(orderby, pageIndex, pageSize, predicate);
        }

        #region Private Methods

        private static void HierarchizeHierarchyData(IEnumerable<HierarchyDataObject> allHierarchyDataObjects, HierarchyDataObject parentHierarchyData, IList<HierarchyDataObject> results, int hierarchyLevel)
        {
            results.Add(ResolveHierarchyDisplayName(parentHierarchyData, hierarchyLevel));
            IEnumerable<HierarchyDataObject> childHierarchyDataObjects = allHierarchyDataObjects.Where(h => h.ParentHierarchyDataId == parentHierarchyData.HierarchyDataId);
            foreach (HierarchyDataObject childHierarchyDataObject in childHierarchyDataObjects)
                HierarchizeHierarchyData(allHierarchyDataObjects, childHierarchyDataObject, results, hierarchyLevel + 1);
        }

        private static IEnumerable<HierarchyDataObject> FindHierarchyDataObjectWithParentNotExist(IEnumerable<HierarchyDataObject> allHierarchyDataObjects)
        {
            List<HierarchyDataObject> results = new List<HierarchyDataObject>();
            foreach (HierarchyDataObject hierarchyDataObject in allHierarchyDataObjects)
            {
                if (!hierarchyDataObject.ParentHierarchyDataId.HasValue
                    || allHierarchyDataObjects.FirstOrDefault(h => h.HierarchyDataId == hierarchyDataObject.ParentHierarchyDataId.Value) == null)
                    results.Add(hierarchyDataObject);
            }

            return results;
        }

        private static HierarchyDataObject ResolveHierarchyDisplayName(HierarchyDataObject hierarchyDataObject, int hierarchyLevel)
        {
            if (hierarchyLevel <= 0) return hierarchyDataObject;

            string prefix = "";
            for (int i = 0; i < hierarchyLevel; i++)
                prefix += "--";

            HierarchyDataObject returnValue = hierarchyDataObject.Clone();
            returnValue.Name = string.Format(CultureInfo.InvariantCulture, "{0} {1}", prefix, hierarchyDataObject.Name);
            return returnValue;
        }

        #endregion
    }
}
