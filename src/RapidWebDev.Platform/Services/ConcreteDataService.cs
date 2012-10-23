/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.long.yi@RapidWebDev.org

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
    /// The services for concrete data.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ConcreteDataService : IConcreteDataService
    {
        private static IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();

        /// <summary>
        /// Add/update concrete data object depends on whether identity of object is empty or not.
        /// </summary>
        /// <param name="concreteDataObject">The name of concrete data should be unique in a concrete data type.</param>
        public string SaveJson(ConcreteDataObject concreteDataObject)
        {
            if (concreteDataObject == null)
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.ConcreteDataObjectCannotBeNull));
            try
            {
                concreteDataApi.Save(concreteDataObject);
                return concreteDataObject.ConcreteDataId.ToString();
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
        /// Add/update concrete data object depends on whether identity of object is empty or not.
        /// </summary>
        /// <param name="concreteDataObject">The name of concrete data should be unique in a concrete data type.</param>
        public string SaveXml(ConcreteDataObject concreteDataObject)
        {
            return SaveJson(concreteDataObject);
        }

        /// <summary>
        /// Get an non-deleted concrete data by id. 
        /// </summary>
        /// <param name="concreteDataId"></param>
        /// <returns></returns>
        public ConcreteDataObject GetByIdJson(string concreteDataId)
        {
            if (string.IsNullOrEmpty(concreteDataId))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidConcreteDataID));

            Guid id = Guid.Empty;
            try
            {
                id = new Guid(concreteDataId);
                return concreteDataApi.GetById(id);
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
        /// Get an non-deleted concrete data by id. 
        /// </summary>
        /// <param name="concreteDataId"></param>
        /// <returns></returns>
        public ConcreteDataObject GetByIdXml(string concreteDataId)
        {
            return this.GetByIdJson(concreteDataId);
        }

        /// <summary>
        /// Get an non-deleted concrete data by id.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConcreteDataObject GetByNameJson(string type, string name)
        {
            if (string.IsNullOrEmpty(type))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidConcreteDataType));
            if (string.IsNullOrEmpty(name))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidDisplayName));
            try
            {
                ConcreteDataObject concreteDataObject = concreteDataApi.GetByName(type, name);
                if (concreteDataObject == null)
                    return null;
                return concreteDataObject;
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
        /// Get an non-deleted concrete data by id.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConcreteDataObject GetByNameXml(string type, string name)
        {
            return this.GetByNameJson(type, name);
        }

        /// <summary>
        /// Find non-deleted concrete data by a keyword which may be included in concrete data name or value.
        /// </summary>
        /// <param name="concreteDataType">Concrete data type.</param>
        /// <param name="query">Keyword included in Name or Value. Null or empty value indicates to query all records in the concrete type.</param>
        /// <param name="limit">Maximum number of returned records.</param>
        /// <returns></returns>
        public Collection<ConcreteDataObject> FindByKeywordJson(string concreteDataType, string query, int limit)
        {
            LinqPredicate predicate = new LinqPredicate("Type=@0 AND DeleteStatus=@1", concreteDataType, DeleteStatus.NotDeleted);

            if (!string.IsNullOrEmpty(query))
                predicate.Add("Name.Contains(@0) OR Value.Contains(@0)", query);

            int recordCount;
            try
            {
                Collection<ConcreteDataObject> results = new Collection<ConcreteDataObject>(concreteDataApi.FindConcreteData(predicate, "Name Asc", 0, limit, out recordCount).ToList());
                
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
        /// Find non-deleted concrete data by a keyword which may be included in concrete data name or value.
        /// </summary>
        /// <param name="concreteDataType">Concrete data type.</param>
        /// <param name="query">Keyword included in Name or Value. Null or empty value indicates to query all records in the concrete type.</param>
        /// <param name="limit">Maximum number of returned records.</param>
        /// <returns></returns>
        public Collection<ConcreteDataObject> FindByKeywordXml(string concreteDataType, string query, int limit)
        {
            return this.FindByKeywordJson(concreteDataType, query, limit);
        }

        /// <summary>
        /// Find all available concrete data types.
        /// </summary>
        /// <returns></returns>
        public Collection<string> FindConcreteDataTypesJson()
        {
            try
            {
                IEnumerable<string> results = concreteDataApi.FindConcreteDataTypes();
                if (results.Count() == 0)
                    return new Collection<string>();

                return new Collection<string>(results.ToList());
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
        /// Find all available concrete data types.
        /// </summary>
        /// <returns></returns>
        public Collection<string> FindConcreteDataTypesXml()
        {
            return this.FindConcreteDataTypesJson();
        }

        /// <summary>
        /// Find concrete data in all types by custom predicates.<br />
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="predicate"></param>
        public ConcreteDataQueryResult QueryConcreteDataJson(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            int recordCount;
            try
            {
                pageSize = (pageSize == 0) ? 25 : pageSize;

                LinqPredicate linqPredicate = ServicesHelper.ConvertWebPredicateToLinqPredicate(predicate);
                IEnumerable<ConcreteDataObject> rets = concreteDataApi.FindConcreteData(linqPredicate, orderby, pageIndex, pageSize, out recordCount);
           
                ConcreteDataQueryResult results = new ConcreteDataQueryResult(rets.ToList())
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
        /// Find concrete data in all types by custom predicates.
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="predicate"></param>
        public ConcreteDataQueryResult QueryConcreteDataXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            return this.QueryConcreteDataJson(orderby, pageIndex, pageSize, predicate);
        }


    }
}
