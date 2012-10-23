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
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using RapidWebDev.Common;
using System.Collections.ObjectModel;
using System.Globalization;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// external service for CRUD relationships
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RelationshipService : IRelationshipService
    {
        private IRelationshipApi relationshipApi = SpringContext.Current.GetObject<IRelationshipApi>();

        #region IRelationshipService Members
        /// <summary>
        /// Save relationship b/w 2 entities on special relationship type.
        /// </summary>
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>      
        /// <param name="relationshipType"></param>
        /// <param name="Ordinal"></param>
        public void SaveJson(string objectXId, string objectYId, string relationshipType, string Ordinal)
        {
            if (string.IsNullOrEmpty(objectXId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "objectId cannot be empty"));
            if (string.IsNullOrEmpty(objectYId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "referenceobject's Id cannot be empty"));
            if (string.IsNullOrEmpty(relationshipType)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "relationshipType cannot be empty"));

            try
            {
                RelationshipObject relationship = new RelationshipObject() { ReferenceObjectId = new Guid(objectYId), RelationshipType = relationshipType, Ordinal = string.IsNullOrEmpty(Ordinal) ? 0 : Convert.ToInt32(Ordinal) };
                relationshipApi.Save(new Guid(objectXId), relationship);
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
        /// Remove any relationships with specified object.
        /// </summary>
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>
        /// <param name="relationshipType"></param>
        public void RemoveJson(string objectXId, string objectYId, string relationshipType)
        {
            if (string.IsNullOrEmpty(objectXId) && string.IsNullOrEmpty(objectYId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "two objectId cannot be empty together"));

            try
            {
                if (string.IsNullOrEmpty(objectXId) && !string.IsNullOrEmpty(objectYId))
                    if (string.IsNullOrEmpty(relationshipType))
                        relationshipApi.Remove(new Guid(objectYId));
                    else
                        relationshipApi.Remove(new Guid(objectYId), relationshipType);
                else if (!string.IsNullOrEmpty(objectXId) && !string.IsNullOrEmpty(objectYId))
                    if (string.IsNullOrEmpty(relationshipType))
                        relationshipApi.Remove(new Guid(objectXId), new Guid(objectYId));
                    else
                        relationshipApi.Remove(new Guid(objectXId), new Guid(objectYId), relationshipType);
                else
                    if (string.IsNullOrEmpty(relationshipType))
                        relationshipApi.Remove(new Guid(objectXId));
                    else
                        relationshipApi.Remove(new Guid(objectXId), relationshipType);


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
        /// Get the object related to the target object in special type.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        public RelationshipObject GetOneToOneJson(string objectId, string relationshipType)
        {
            if (string.IsNullOrEmpty(objectId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "objectId cannot be empty"));
            if (string.IsNullOrEmpty(relationshipType)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "relationshipType cannot be empty"));

            try
            {
                RelationshipObject relationshipObject = relationshipApi.GetOneToOne(new Guid(objectId), relationshipType);
                if (relationshipObject == null)
                    return null;
                return relationshipObject;
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
        /// Get the object related to the target object in special type.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        public RelationshipObject GetManyToOneJson(string objectId, string relationshipType)
        {
            if (string.IsNullOrEmpty(objectId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "objectId cannot be empty"));
            if (string.IsNullOrEmpty(relationshipType))
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "relationshipType cannot be empty"));

            try
            {
                RelationshipObject relationshipObject = relationshipApi.GetManyToOne(new Guid(objectId), relationshipType);
                if (relationshipObject == null)
                    return null;

                return relationshipObject;
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
        /// Get the object related to the target object in special type.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        public Collection<RelationshipObject> GetOneToManyJson(string objectId, string relationshipType)
        {
            if (string.IsNullOrEmpty(objectId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "objectId cannot be empty"));
            if (string.IsNullOrEmpty(relationshipType)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "relationshipType cannot be empty"));

            try
            {
                Collection<RelationshipObject> relationshipObjects = new Collection<RelationshipObject>(relationshipApi.GetOneToMany(new Guid(objectId), relationshipType).ToList());
                if (relationshipObjects.Count() == 0)
                    return new Collection<RelationshipObject>();
                return relationshipObjects;
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
        /// Find all objects related to the target object in any special relationship type.
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public System.Collections.ObjectModel.Collection<RelationshipObject> FindAllRelationshipJson(string objectId)
        {
            if (string.IsNullOrEmpty(objectId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "objectId cannot be empty"));
            try
            {
                Collection<RelationshipObject> results = new Collection<RelationshipObject>(relationshipApi.FindAllRelationship(new Guid(objectId)).ToList());
                if (results.Count() == 0)
                    return new Collection<RelationshipObject>();
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

        #endregion

        #region IRelationshipService Members

        /// <summary>
        /// Save relationship b/w 2 entities on special relationship type.
        /// </summary>
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>      
        /// <param name="relationshipType"></param>
        /// <param name="Ordinal"></param>
        public void SaveXml(string objectXId, string objectYId, string relationshipType, string Ordinal)
        {
            SaveJson(objectXId, objectYId, relationshipType, Ordinal);
        }
        /// <summary>
        /// Remove any relationships with specified object.
        /// </summary>
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>
        /// <param name="relationshipType"></param>
        public void RemoveXml(string objectXId, string objectYId, string relationshipType)
        {
            RemoveJson(objectXId, objectYId, relationshipType);
        }
        /// <summary>
        /// Get the object related to the target object in special type.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        public RelationshipObject GetOneToOneXml(string objectId, string relationshipType)
        {
            return GetOneToOneJson(objectId, relationshipType);
        }

        /// <summary>
        /// Get the object related to the target object in special type.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        public RelationshipObject GetManyToOneXml(string objectId, string relationshipType)
        {
            return GetManyToOneJson(objectId,relationshipType);
        }


        /// <summary>
        /// Get the object related to the target object in special type.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="relationshipType"></param>
        /// <returns></returns>
        public Collection<RelationshipObject> GetOneToManyXml(string objectId, string relationshipType)
        {
            return GetOneToManyJson(objectId, relationshipType);
        }


        /// <summary>
        /// Find all objects related to the target object in any special relationship type.
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public Collection<RelationshipObject> FindAllRelationshipXml(string objectId)
        {
            return FindAllRelationshipJson(objectId);
        }

        #endregion
    }
}
