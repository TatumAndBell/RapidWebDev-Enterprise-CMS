/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi , Email: tim.long.yi@RapidWebDev.org

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
using RapidWebDev.Common;
using System.Collections.ObjectModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Globalization;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// external service implementation
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MembershipService : IMembershipService
    {
        private IMembershipApi membershipApi = SpringContext.Current.GetObject<IMembershipApi>();

        #region IMembershipService Members
        /// <summary>
        /// Add/update user object depends on whether identity of object is empty or not.<br />      
        /// </summary>
        /// <param name="user"></param>   
        public string SaveJson(UserObject user)
        {
            if (user == null) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture,Resources.UserObjectCannotBeNull));
            
            string password = WebOperationContext.Current.IncomingRequest.Headers["password"];
            string passwordAnswer = WebOperationContext.Current.IncomingRequest.Headers["passwordAnswer"];

            try
            {
                membershipApi.Save(user, password, passwordAnswer);
                return user.UserId.ToString();
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
        /// Resolve user objects from enumerable user ids.
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public System.Collections.ObjectModel.Collection<UserObject> BulkGetJson(IdCollection userIds)
        {
            if (userIds.Count() == 0) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidUserID));
            try
            {
                IDictionary<Guid, UserObject> dicts = membershipApi.BulkGet(ServicesHelper.ConvertStringCollectionToGuidEnumerable(userIds));
                if (dicts.Count() == 0)
                    return new Collection<UserObject>();

                Collection<UserObject> results = new Collection<UserObject>();
                foreach (string temp in userIds)
                {
                    results.Add(dicts[new Guid(temp)]);
                }

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
        /// Get user by user name.  
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserObject GetByNameJson(string userName)
        {
            if (string.IsNullOrEmpty(userName)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture,Resources.UserNameCannotBeEmpty));

            try
            {
                UserObject result = membershipApi.Get(userName);
                if (result == null)
                    return null;
                return result;
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
        /// Get user object by user id.
        /// </summary>   
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public UserObject GetByIdJson(string userId)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture,Resources.InvalidUserID));

            try
            {
                UserObject result = membershipApi.Get(new Guid(userId));
                if (result == null)
                    return null;
                return result;
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
        /// Change password of specified user. 
        /// </summary>      
        /// <param name="userId"></param>
        /// <returns>returns true if operation successfully.</returns>
        public bool ChangePasswordJson(string userId)
        {
            string oldPassword = WebOperationContext.Current.IncomingRequest.Headers["oldPassword"];
            string newPassword = WebOperationContext.Current.IncomingRequest.Headers["newPassword"];

            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture,Resources.InvalidUserID));
            if (string.IsNullOrEmpty(oldPassword)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.PasswordCannotBeEmpty ));
            if (string.IsNullOrEmpty(newPassword)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.PasswordCannotBeEmpty ));

            try
            {
                return membershipApi.ChangePassword(new Guid(userId), oldPassword, newPassword);
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
        /// Find user business objects by custom predicates.
        /// </summary>            
        /// <param name="orderby">sorting field and direction</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>        
        /// <param name="predicate">linq predicate. see user properties for predicate at <see cref="RapidWebDev.Platform.Linq.User"/>.</param>
        /// <returns>Returns enumerable user objects</returns>
        public UserQueryResult QueryUsersJson(string orderby, int pageIndex, int pageSize, RapidWebDev.Common.WebServiceQueryPredicate predicate)
        {
            
            try
            {
                int recordCount;
                pageSize = (pageSize == 0) ? 25 : pageSize;
                IEnumerable<UserObject> results = membershipApi.FindUsers(ServicesHelper.ConvertWebPredicateToLinqPredicate(predicate), orderby, pageIndex, pageSize, out recordCount);
                
                UserQueryResult result = new UserQueryResult(results.ToList())
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecordCount = recordCount
                };
                return result;
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
        /// Add/update user object depends on whether identity of object is empty or not.<br />      
        /// </summary>
        /// <param name="user"></param>   
        public string SaveXml(UserObject user)
        {
            return SaveJson(user);
        }
        /// <summary>
        /// Resolve user objects from enumerable user ids.
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public Collection<UserObject> BulkGetXml(IdCollection userIds)
        {
            return BulkGetJson(userIds);
        }
        /// <summary>
        /// Get user by user name.  
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserObject GetByNameXml(string userName)
        {
            return GetByNameJson(userName);
        }
        /// <summary>
        /// Get user object by user id.
        /// </summary>   
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public UserObject GetByIdXml(string userId)
        {
            return GetByIdJson(userId);
        }
        /// <summary>
        /// Change password of specified user. 
        /// </summary>      
        /// <param name="userId"></param>
        /// <returns>returns true if operation successfully.</returns>
        public bool ChangePasswordXml(string userId)
        {
            return ChangePasswordJson(userId);
        }
        /// <summary>
        /// Find user business objects by custom predicates.
        /// </summary>           
        /// <param name="orderby">sorting field and direction</param>
        /// <param name="pageIndex">current paging index</param>
        /// <param name="pageSize">page size</param>         
        /// <param name="predicate">linq predicate. see user properties for predicate at <see cref="RapidWebDev.Platform.Linq.User"/>.</param>
        /// <returns>Returns enumerable user objects</returns>
        public UserQueryResult QueryUsersXml(string orderby, int pageIndex, int pageSize, WebServiceQueryPredicate predicate)
        {
            return QueryUsersJson(orderby, pageIndex, pageSize, predicate);
        }

        #endregion
    }
}
