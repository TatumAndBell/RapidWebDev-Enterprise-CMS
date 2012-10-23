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
using RapidWebDev.Common;
using System.Collections.ObjectModel;
using System.ServiceModel.Activation;
using System.Globalization;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PermissionService : IPermissionService
    {
        private IPermissionApi permissionApi = SpringContext.Current.GetObject<IPermissionApi>();



        #region IPermissionService Members
        /// <summary>
        /// Set permissions on specified role. The new permissions will overwrite the existed permissions of the role.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissions"></param>
        public void SetRolePermissionsByJson(string roleId, IdCollection permissions)
        {
            if (string.IsNullOrEmpty(roleId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidRoleID + "{0}", "Role Id cannot be null"));
            if (permissions.Count() == 0) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "{0}", "permission cannot be empty"));

            try
            {
                permissionApi.SetRolePermissions(new Guid(roleId), permissions);
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
        /// Set permissions on specified user. The new permissions will overwrite the existed permissions of the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        public void SetUserPermissionsByJson(string userId, IdCollection permissions)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.UserNameCannotBeEmpty + "{0}", "User Id cannot be null"));
            if (permissions.Count() == 0) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "{0}", "permission cannot be empty"));
            try
            {
                permissionApi.SetUserPermissions(new Guid(userId), permissions);
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
        /// Get permissions which the user has.
        /// </summary>
        /// <param name="userId">specified user</param>
        /// <param name="explicitOnly">true indicates that returns the permissions only directly set on the user</param>
        /// <returns>returns user permissions</returns>
        public Collection<string> FindUserPermissionsByJson(string userId, string explicitOnly)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.UserNameCannotBeEmpty + "{0}", "User Id cannot be null"));
            
            try
            {
                IEnumerable<string> results = permissionApi.FindUserPermissions(new Guid(userId), Convert.ToBoolean(explicitOnly)).Select(x=>x.PermissionValue);
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
        /// Get permissions which the role has.
        /// </summary>
        /// <param name="roleId">specified role</param>
        /// <returns>returns role permissions</returns>
        public System.Collections.ObjectModel.Collection<string> FindRolePermissionsByJson(string roleId)
        {
            if (string.IsNullOrEmpty(roleId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidRoleID + "{0}", "Role Id cannot be null"));
            try
            {
                IEnumerable<string> results = permissionApi.FindRolePermissions(new Guid(roleId)).Select(x=>x.PermissionValue);
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
        /// Returns true if input user has the permission.
        /// The permissions owned by the user includes the ones inherited from the roles of the user implicitly.
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="permissionValue">permission value</param>
        /// <returns>Returns true if input user has the permission.</returns>
        public bool DoesTheUserHasPermissionJson(string userId, string permissionValue)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.UserNameCannotBeEmpty + "{0}", "User Id cannot be null"));
            if (string.IsNullOrEmpty(permissionValue)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "{0}", "permissionValue cannot be null"));
            try
            {
                return permissionApi.HasPermission(new Guid(userId), permissionValue);
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
        /// Returns true if the current authenticated user has any permissions in specified permission.
        /// </summary>
        /// <param name="permissionValue">permission value</param>
        /// <returns>Returns true if the current user has permission to access permission key.</returns>
        public bool DoesTheCurrentUserHavePermissionJson(string permissionValue)
        {
            if (string.IsNullOrEmpty(permissionValue)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "{0}", "permissionValue cannot be null"));
            try
            {
                return permissionApi.HasPermission(permissionValue);
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



        #region IPermissionService Members

        /// <summary>
        /// Set permissions on specified role. The new permissions will overwrite the existed permissions of the role.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissions"></param>
        public void SetRolePermissionsByXml(string roleId, IdCollection permissions)
        {
            SetRolePermissionsByJson(roleId, permissions);

        }
        /// <summary>
        /// Set permissions on specified user. The new permissions will overwrite the existed permissions of the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        public void SetUserPermissionsByXml(string userId, IdCollection permissions)
        {
            SetUserPermissionsByJson(userId, permissions);
        }
        /// <summary>
        /// Get permissions which the user has.
        /// </summary>
        /// <param name="userId">specified user</param>
        /// <param name="explicitOnly">true indicates that returns the permissions only directly set on the user</param>
        /// <returns>returns user permissions</returns>
        public Collection<string> FindUserPermissionsByXml(string userId, string explicitOnly)
        {
            return FindUserPermissionsByJson(userId, explicitOnly);
        }
        /// <summary>
        /// Get permissions which the role has.
        /// </summary>
        /// <param name="roleId">specified role</param>
        /// <returns>returns role permissions</returns>
        public Collection<string> FindRolePermissionsByXml(string roleId)
        {
            return FindRolePermissionsByJson(roleId);
        }
        /// <summary>
        /// Returns true if input user has the permission.
        /// The permissions owned by the user includes the ones inherited from the roles of the user implicitly.
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="permissionValue">permission value</param>
        /// <returns>Returns true if input user has the permission.</returns>
        public bool DoesTheUserHasPermissionXml(string userId, string permissionValue)
        {
            return DoesTheUserHasPermissionJson(userId, permissionValue);
        }
        /// <summary>
        /// Returns true if the current authenticated user has any permissions in specified permission.
        /// </summary>
        /// <param name="permissionValue">permission value</param>
        /// <returns>Returns true if the current user has permission to access permission key.</returns>
        public bool DoesTheCurrentUserHavePermissionXml(string permissionValue)
        {
            return DoesTheCurrentUserHavePermissionJson(permissionValue);
        }

        #endregion
    }
}
