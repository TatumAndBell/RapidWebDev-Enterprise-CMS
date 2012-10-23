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
using System.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Validation;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Linq
{
    /// <summary>
    /// Role Api bases on integrating asp.net Role api.
    /// </summary>
    public class RoleApi : CachableApi, IRoleApi
    {
        private IAuthenticationContext authenticationContext;

        /// <summary>
        /// Construct RoleApi by authenticationContext
        /// </summary>
        /// <param name="authenticationContext"></param>
		public RoleApi(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
        {
            this.authenticationContext = authenticationContext;
        }

        /// <summary>
        /// Save role business object. 
        /// It does create/update based on roleObject.Id. If id is empty, the method will create a new role object.
        /// If the specified id is invalid, the method will throw an exception.
        /// </summary>
        /// <param name="roleObject">role object</param>
        /// <exception cref="ValidationException">Role name existed in system</exception>
        /// <exception cref="ArgumentException">Role id is invalid</exception>
        public void Save(RoleObject roleObject)
        {
            Kit.NotNull(roleObject, "roleObject");


            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                try
                {
					Role role = null;
					using (ValidationScope validationScope = new ValidationScope(true))
					{
						var duplicateRoleNameCount = (from r in ctx.Roles
													  where r.ApplicationId == this.authenticationContext.ApplicationId
														 && r.RoleName == roleObject.RoleName
														 && r.Domain == roleObject.Domain
														 && r.RoleId != roleObject.RoleId
													  select r).Count();
						if (duplicateRoleNameCount > 0)
							validationScope.Error(Resources.ExistedRoleName, roleObject.RoleName);

						if (roleObject.RoleId == Guid.Empty)
						{
							role = new Role()
							{
								ApplicationId = this.authenticationContext.ApplicationId,
							};

							ctx.Roles.InsertOnSubmit(role);
						}
						else
						{
							role = ctx.Roles.FirstOrDefault(r => r.RoleId == roleObject.RoleId);
							if (role == null)
								validationScope.Error(Resources.InvalidRoleID, "roleObject");

							base.RemoveCache(FormatRoleNameCacheKey(role.RoleName));
							base.RemoveCache(roleObject.RoleId);
						}
					}

					role.Domain = roleObject.Domain;
                    role.RoleName = roleObject.RoleName;
                    role.LoweredRoleName = roleObject.RoleName.ToLowerInvariant();
                    role.Description = roleObject.Description;
                    role.LastUpdatedDate = DateTime.UtcNow;
                    role.Predefined = roleObject.Predefined;

                    ctx.SubmitChanges();
                    roleObject.RoleId = role.RoleId;
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (ValidationException)
                {
                    throw;
                }
                catch (Exception exp)
                {
                    Logger.Instance(this).Error(exp);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="roleId"></param>
        public void HardDelete(Guid roleId)
        {
            try
            {
                using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
                {
                    Role roleToDelete = null;
                    List<Guid> affectedUserIds = null;

                    try
                    {
                        if (ctx.Roles.Where(r => r.RoleId == roleId).Count() == 0) return;

                        affectedUserIds = ctx.UsersInRoles.Where(usersInRole => usersInRole.RoleId == roleId).Select(usersInRole => usersInRole.UserId).ToList();
                        ctx.UsersInRoles.Delete(usersInRole => usersInRole.RoleId == roleId);
                        ctx.Permissions.Delete(permission => permission.RoleId == roleId);

                        roleToDelete = ctx.Roles.FirstOrDefault(role => role.RoleId == roleId);
                        ctx.Roles.DeleteOnSubmit(roleToDelete);

                        ctx.SubmitChanges();
                    }
                    catch
                    {
                        throw;
                    }

                    if (roleToDelete != null)
                    {
                        if (affectedUserIds != null)
                            affectedUserIds.ForEach(userId => base.RemoveCache(FormatAllRolesOfUserCacheKey(userId)));

                        base.RemoveCache(FormatRoleNameCacheKey(roleToDelete.RoleName));
                        base.RemoveCache(roleId);
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw;
            }
        }

        /// <summary>
        /// Set user into roles which overwrites all existed roles of user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        public void SetUserToRoles(Guid userId, IEnumerable<Guid> roleIds)
        {
            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                try
                {
					User user = ctx.Users.FirstOrDefault(u => u.ApplicationId == this.authenticationContext.ApplicationId && u.UserId == userId);
					if (user == null)
                        throw new ArgumentException(Resources.InvalidUserID, "userId");

					string userDomain = user.Organization.OrganizationType.Domain;

					int roleCount = ctx.Roles.Count(r => roleIds.ToArray().Contains(r.RoleId) && r.Domain == userDomain);
                    if (roleCount < roleIds.Count())
                        throw new ArgumentException(Resources.InvalidRoleID, "roleIds");

                    ctx.UsersInRoles.Delete(userInRole => userInRole.UserId == userId);
					foreach (Guid roleId in roleIds)
						ctx.UsersInRoles.InsertOnSubmit(new UsersInRole() { RoleId = roleId, UserId = userId });

                    ctx.SubmitChanges();
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception exp)
                {
                    Logger.Instance(this).Error(exp);
                    throw;
                }

                base.RemoveCache(FormatAllRolesOfUserCacheKey(userId));
            }
        }

        /// <summary>
        /// Get role id by role name.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public RoleObject Get(string roleName)
        {
            Kit.NotNull(roleName, "roleName");

            Guid roleId = base.GetCacheObject<Guid>(FormatRoleNameCacheKey(roleName));

            if (roleId == Guid.Empty)
            {
                using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
                {
					roleId = (from r in ctx.Roles
							  where r.RoleName == roleName && r.ApplicationId == this.authenticationContext.ApplicationId
							  select r.RoleId).FirstOrDefault();

                    if (roleId == Guid.Empty) return null;
                    base.AddCache(FormatRoleNameCacheKey(roleName), roleId);
                }
            }

            return this.Get(roleId);
        }

        /// <summary>
        /// Get role id by role id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RoleObject Get(Guid roleId)
        {
            RoleObject roleObject = base.GetCacheObject<RoleObject>(roleId);
            if (roleObject != null) return roleObject;

            try
            {
                using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
                {
					Role role = ctx.Roles.FirstOrDefault(r => r.RoleId == roleId);
                    if (role == null) return null;

                    roleObject = new RoleObject()
                    {
                        RoleId = role.RoleId,
                        RoleName = role.RoleName,
                        Description = role.Description,
                        Predefined = role.Predefined,
						Domain = role.Domain
                    };

                    base.AddCache(roleId, roleObject);
                    return roleObject;
                }
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw;
            }
        }

		/// <summary>
		/// Bulkget role objects by a collection of role ids.
		/// </summary>
		/// <param name="roleIds"></param>
		/// <returns></returns>
		public IDictionary<Guid, RoleObject> BulkGet(IEnumerable<Guid> roleIds)
		{
			Dictionary<Guid, RoleObject> roleDictionary = new Dictionary<Guid, RoleObject>();
			List<Guid> uncachedRoleIds = new List<Guid>();
			foreach (Guid roleId in roleIds.Distinct())
			{
				RoleObject cachedRoleObject = base.GetCacheObject<RoleObject>(roleId);
				if (cachedRoleObject != null)
				{
					roleDictionary.Add(roleId, cachedRoleObject);
					base.AddCache(roleId, cachedRoleObject);
				}
				else
					uncachedRoleIds.Add(roleId);
			}

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				var roleObjects = ctx.Roles.Where(r => r.ApplicationId == this.authenticationContext.ApplicationId && uncachedRoleIds.ToArray().Contains(r.RoleId)).Select(r => new RoleObject()
				{
					RoleId = r.RoleId,
					RoleName = r.RoleName,
					Description = r.Description,
					Predefined = r.Predefined,
					Domain = r.Domain
				});

				foreach (RoleObject roleObject in roleObjects)
				{
					base.AddCache(roleObject.RoleId, roleObject);
					base.AddCache(FormatRoleNameCacheKey(roleObject.RoleName), roleObject.RoleId);
					roleDictionary.Add(roleObject.RoleId, roleObject);
				}
			}

			return roleDictionary;
		}

        /// <summary>
        /// Find all available roles.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleObject> FindAll()
        {
            try
            {
                using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
                {
                    var roleIds = from role in ctx.Roles
                                  where role.ApplicationId == this.authenticationContext.ApplicationId
                                  select role.RoleId;

                    return this.BulkGet(roleIds).Values;
                }
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw;
            }
        }

		/// <summary>
		/// Find roles by domain. returns all if specified argument is null.
		/// </summary>
		/// <param name="domain">organization domain</param>
		/// <returns></returns>
		public IEnumerable<RoleObject> FindByDomain(string domain)
        {
			Kit.NotNull(domain, "domain");

            try
            {
                using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
                {
					var q = (from r in ctx.Roles
							 where r.ApplicationId == this.authenticationContext.ApplicationId
								&& r.Domain == domain
							 select r.RoleId).Distinct().ToList();

					return this.BulkGet(q).Values.OrderBy(r => r.RoleName).ToList();
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw;
            }
        }

        /// <summary>
        /// Find all roles of specified user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<RoleObject> FindByUserId(Guid userId)
        {
            List<Guid> roleIds = FindRoleIdsOfUser(userId);
            return this.BulkGet(roleIds).Values;
        }

        /// <summary>
        /// Gets true if specified user is in role.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsUserInRole(Guid userId, Guid roleId)
        {
            List<Guid> roleIds = this.FindRoleIdsOfUser(userId);
            return roleIds.Contains(roleId);
        }

        /// <summary>
        /// Gets true if specified user is in role.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool IsUserInRole(Guid userId, string roleName)
        {
            RoleObject roleObject = this.Get(roleName);
            if (roleObject == null) return false;

            return this.IsUserInRole(userId, roleObject.RoleId);
        }

		/// <summary>
		/// Find role objects by custom predicates.
		/// </summary>
		/// <param name="predicate">linq predicate. see role properties for predicate at <see cref="RapidWebDev.Platform.Linq.Role"/>.</param>
		/// <param name="orderby">dynamic orderby command</param>
		/// <param name="pageIndex">current paging index</param>
		/// <param name="pageSize">page size</param>
		/// <param name="recordCount">total hit records count</param>
		/// <returns>Returns enumerable role objects</returns>
		public IEnumerable<RoleObject> FindRoles(LinqPredicate predicate, string orderby, int pageIndex, int pageSize, out int recordCount)
		{
			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
					var q = from r in ctx.Roles where r.ApplicationId == authenticationContext.ApplicationId select r;

					if (predicate != null && !Kit.IsEmpty(predicate.Expression))
						q = q.Where(predicate.Expression, predicate.Parameters);

					if (!Kit.IsEmpty(orderby))
						q = q.OrderBy(orderby);

					recordCount = q.Count();
					List<Guid> roleIds = q.Skip(pageIndex * pageSize).Take(pageSize)
						.Select(r => r.RoleId).ToList();

					return this.BulkGet(roleIds).Values;
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

        private List<Guid> FindRoleIdsOfUser(Guid userId)
        {
            string cachedKey = FormatAllRolesOfUserCacheKey(userId);
            List<Guid> roleIds = base.GetCacheObject<List<Guid>>(cachedKey);
            if (roleIds == null)
            {
                using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
                {
                    roleIds = ctx.UsersInRoles.Where(usersInRole => usersInRole.UserId == userId).Select(usersInRole => usersInRole.RoleId).ToList();
                    base.AddCache(cachedKey, roleIds);
                }
            }

            return roleIds;
        }

        private string FormatAllRolesOfUserCacheKey(Guid userId)
        {
			return "FormatAllRolesOfUserCacheKey@" + userId.ToString();
        }

        private string FormatRoleNameCacheKey(string roleName)
        {
			return "FormatRoleNameCacheKey@" + roleName;
        }
    }
}

