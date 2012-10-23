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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Common.Validation;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform.Linq
{
	/// <summary>
	/// The implementation class of IPermissionApi interface
	/// </summary>
	public class PermissionApi : CachableApi, IPermissionApi
	{
		private static object syncObj = new object();
		private IPlatformConfiguration platformConfiguration;
		private IAuthenticationContext authenticationContext;
		private IMembershipApi membershipApi;
		private IRoleApi roleApi;
		private IOrganizationApi organizationApi;
		private IPermissionConfigurationReader permissionConfigurationReader;
		private volatile Dictionary<CultureInfo, Dictionary<string, IEnumerable<PermissionConfig>>> allPermissionConfigs = new Dictionary<CultureInfo, Dictionary<string, IEnumerable<PermissionConfig>>>();
		private volatile Dictionary<string, Dictionary<string, List<string>>> implicitPermission;

		/// <summary>
		/// Gets all permission configurations existed in system.
		/// </summary>
		private IEnumerable<PermissionConfig> AllPermissionConfigs
		{
			get
			{
				Dictionary<string, IEnumerable<PermissionConfig>> permissionConfigByDomain = null;
				CultureInfo currentCulture = Resources.Culture ?? CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;
				if (!this.allPermissionConfigs.ContainsKey(currentCulture))
				{
					lock (syncObj)
					{
						if (!this.allPermissionConfigs.ContainsKey(currentCulture))
						{
							XmlDocument xmldoc = this.permissionConfigurationReader.Read();
							XmlSerializer serializer = new XmlSerializer(typeof(PermissionConfigs));
							using (StringReader reader = new StringReader(xmldoc.OuterXml))
							{
								PermissionConfigs permissionConfigs = serializer.Deserialize(reader) as PermissionConfigs;
								IEnumerable<PermissionConfig> permissionConfigToGlobalize = permissionConfigs.Domain.SelectMany(domain => domain.Permission);
								ReplaceTextGlobalizationIdentifiers(permissionConfigToGlobalize);

								permissionConfigByDomain = permissionConfigs.Domain.ToDictionary(d => d.Value, d => d.Permission.AsEnumerable());
								this.allPermissionConfigs[currentCulture] = permissionConfigByDomain;
							}
						}
					}
				}

				if (!authenticationContext.Identity.IsAuthenticated)
					throw new UnauthorizedAccessException(Resources.InvalidAuthentication);

				if (authenticationContext.Organization == null)
					throw new UnauthorizedAccessException(Resources.InvalidOrganizationID);

				OrganizationTypeObject orgType = organizationApi.GetOrganizationType(authenticationContext.Organization.OrganizationTypeId);
				if (permissionConfigByDomain == null)
					permissionConfigByDomain = this.allPermissionConfigs[currentCulture];

				if (!permissionConfigByDomain.ContainsKey(orgType.Domain))
					return new List<PermissionConfig>();

				return permissionConfigByDomain[orgType.Domain];
			}
		}

		private Dictionary<string, Dictionary<string, List<string>>> ImplicitPermission
		{
			get
			{
				if (this.implicitPermission == null)
				{
					lock (syncObj)
					{
						if (this.implicitPermission == null)
						{
							XmlDocument xmldoc = this.permissionConfigurationReader.Read();
							XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
							xmlNamespaceManager.AddNamespace("p", "http://www.rapidwebdev.org/schemas/permissions");

							this.implicitPermission = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);
							XmlNodeList domainNodeList = xmldoc.SelectNodes("//p:Domain", xmlNamespaceManager);
							foreach (XmlElement domainElement in domainNodeList)
							{
								string domainValue = domainElement.Attributes["Value"].Value;
								if (!this.implicitPermission.ContainsKey(domainValue))
									this.implicitPermission[domainValue] = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

								XmlNodeList implicitPermissionNodeList = domainElement.SelectNodes("//p:ImplicitPermission", xmlNamespaceManager);
								foreach (XmlElement implicitPermissionElement in implicitPermissionNodeList)
								{
									string implicitPermissionValue = implicitPermissionElement.Attributes["Value"].Value;
									XmlElement parentPermissionElement = implicitPermissionElement.ParentNode as XmlElement;
									if (parentPermissionElement.Attributes["Value"] == null) continue;
									string parentPermissionValue = parentPermissionElement.Attributes["Value"].Value;

									if (!this.implicitPermission[domainValue].ContainsKey(parentPermissionValue))
										this.implicitPermission[domainValue][parentPermissionValue] = new List<string>();

									if (!this.implicitPermission[domainValue][parentPermissionValue].Contains(implicitPermissionValue))
										this.implicitPermission[domainValue][parentPermissionValue].Add(implicitPermissionValue);
								}
							}
						}
					}
				}

				return this.implicitPermission;
			}
		}

		/// <summary>
		/// Construct PermissionApi instance
		/// </summary>
		/// <param name="authenticationContext"></param>
		/// <param name="membershipApi"></param>
		/// <param name="roleApi"></param>
		/// <param name="organizationApi"></param>
		/// <param name="platformConfiguration"></param>
		/// <param name="permissionConfigurationReader"></param>
		public PermissionApi(IAuthenticationContext authenticationContext, 
			IMembershipApi membershipApi, 
			IRoleApi roleApi, 
			IOrganizationApi organizationApi, 
			IPlatformConfiguration platformConfiguration, 
			IPermissionConfigurationReader permissionConfigurationReader) : base(authenticationContext)
		{
			this.authenticationContext = authenticationContext;
			this.membershipApi = membershipApi;
			this.roleApi = roleApi;
			this.organizationApi = organizationApi;
			this.platformConfiguration = platformConfiguration;
			this.permissionConfigurationReader = permissionConfigurationReader;
		}

		/// <summary>
		/// Set permissions on specified role.
		/// </summary>
		/// <param name="roleId"></param>
		/// <param name="permissions"></param>
		public void SetRolePermissions(Guid roleId, IEnumerable<string> permissions)
		{
			Kit.NotNull(permissions, "permissions");

			try
			{
				using (TransactionScope ts = new TransactionScope())
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					ctx.Permissions.Delete(p => p.RoleId == roleId);
					foreach (string permission in permissions.Distinct())
					{
						ctx.Permissions.InsertOnSubmit(new Permission() { RoleId = roleId, PermissionKey = permission });
					}

					ctx.SubmitChanges();
					base.RemoveCache(roleId);
					ts.Complete();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Set permissions on specified user.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="permissions"></param>
		public void SetUserPermissions(Guid userId, IEnumerable<string> permissions)
		{
			Kit.NotNull(permissions, "permissions");

			try
			{
				using (TransactionScope ts = new TransactionScope())
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					ctx.Permissions.Delete(p => p.UserId == userId);
					foreach (string permission in permissions.Distinct())
					{
						ctx.Permissions.InsertOnSubmit(new Permission() { UserId = userId, PermissionKey = permission });
					}

					ctx.SubmitChanges();
					base.RemoveCache(userId);
					ts.Complete();
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Get user owned permissions.
		/// </summary>
		/// <param name="userId">specified user</param>
		/// <param name="explicitOnly">true indicates that returns the permissions only directly set on the user</param>
		/// <returns>returns user permissions</returns>
		public IEnumerable<PermissionObject> FindUserPermissions(Guid userId, bool explicitOnly)
		{
			IEnumerable<PermissionObject> permissions = new List<PermissionObject> { PermissionConst.EVERYONE };

			try
			{
				var userObject = this.membershipApi.Get(userId);
				if (userObject == null)
					throw new ValidationException(string.Format(Resources.InvalidUserID, userId));

				OrganizationObject organization = organizationApi.GetOrganization(userObject.OrganizationId);
				OrganizationTypeObject organizationType = organizationApi.GetOrganizationType(organization.OrganizationTypeId);
				string domain = organizationType.Domain;

				if (!explicitOnly)
				{
					// returns all system permissions always if user is system administrator.
					if (this.roleApi.IsUserInRole(userId, this.platformConfiguration.Role.RoleId))
					{
						IEnumerable<PermissionObject> allExplicitPermissionObjects = this.AllPermissionConfigs.Select(config => (PermissionObject)config.Value);
						return this.AttachImplicitPermissions(domain, allExplicitPermissionObjects);
					}

					foreach (RoleObject roleObject in this.roleApi.FindByUserId(userObject.UserId))
						permissions = permissions.Union(this.FindRolePermissions(roleObject.RoleId));

					permissions = permissions.FilterDeniedPemissions();
				}

				IEnumerable<PermissionObject> userExplicitPermissions = base.GetCacheObject<IEnumerable<PermissionObject>>(userId);
				if (userExplicitPermissions == null)
				{
					using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
					{
						List<PermissionObject> permissionsExplicitResolved = ctx.Permissions.Where(p => p.UserId == userId && p.User.ApplicationId == authenticationContext.ApplicationId)
							.Select(x => (PermissionObject)x.PermissionKey).Distinct().ToList();

						permissionsExplicitResolved.Add((PermissionObject)(string.Format(CultureInfo.InvariantCulture, "MyOrganizationProfile.{0}", userObject.OrganizationId)));
						permissionsExplicitResolved.Add((PermissionObject)(string.Format(CultureInfo.InvariantCulture, "MyProfile.{0}", userId)));

						userExplicitPermissions = permissionsExplicitResolved.FilterDeniedPemissions();
						base.AddCache(userId, userExplicitPermissions);
					}
				}

				permissions = userExplicitPermissions.MergeUserAndRolePermissions(permissions);
				return this.AttachImplicitPermissions(domain, permissions);
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
		/// Get role permissions.
		/// </summary>
		/// <param name="roleId">specified role</param>
		/// <returns>returns role permissions</returns>
		public IEnumerable<PermissionObject> FindRolePermissions(Guid roleId)
		{
			RoleObject roleObject = roleApi.Get(roleId);
			if (roleObject == null)
				throw new ArgumentException(Resources.InvalidRoleID);

			IEnumerable<PermissionObject> permissions = base.GetCacheObject<IEnumerable<PermissionObject>>(roleId);
			if (permissions != null) return permissions;

			try
			{
				using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
				{
					permissions = ctx.Permissions.Where(p => p.RoleId.Equals(roleId) && p.Role.ApplicationId == authenticationContext.ApplicationId)
						.Select(p => (PermissionObject)p.PermissionKey)
						.ToList().FilterDeniedPemissions();

					base.AddCache(roleId, permissions);
					return this.AttachImplicitPermissions(roleObject.Domain, permissions);
				}
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Returns true if specified user owns permission.
		/// </summary>
		/// <param name="userId">Specified user id</param>
		/// <param name="permissionValue">permission value</param>
		/// <returns></returns>
		public bool HasPermission(Guid userId, string permissionValue)
		{
			return this.HasPermissionInternal(userId, permissionValue);
		}

		/// <summary>
		/// Returns true if the current authenticated user has any permissions in specified permission.
		/// </summary>
		/// <param name="permissionValue">permission value</param>
		/// <returns>Returns true if the current user has permission to access permission key.</returns>
		public bool HasPermission(string permissionValue)
		{
			if (string.IsNullOrEmpty(permissionValue)) return true;
			if (string.Equals(permissionValue, PermissionConst.ANONYMOUS, StringComparison.OrdinalIgnoreCase)) return true;

			if (this.authenticationContext.Identity == null || !this.authenticationContext.Identity.IsAuthenticated) return false;
			return this.HasPermissionInternal(this.authenticationContext.User.UserId, permissionValue);
		}

		/// <summary>
		/// Returns all permission configurations owned by specified user.
		/// </summary>
		/// <param name="operateUserId">operate user id</param>
		/// <returns>Returns all permission configurations owned by specified user.</returns>
		public IEnumerable<PermissionConfig> FindPermissionConfig(Guid operateUserId)
		{
			try
			{
				string sessionKey = "FindPermissionConfig_" + operateUserId.ToString("N");
				if (authenticationContext.Session[sessionKey] == null)
				{
					IEnumerable<PermissionConfig> permissionConfigs = this.AllPermissionConfigs;

					// if the user is system administrator, he should have all permissions
					if (this.roleApi.IsUserInRole(operateUserId, this.platformConfiguration.Role.RoleId))
						return permissionConfigs;

					List<PermissionObject> permissions = this.FindUserPermissions(operateUserId, false).ToList();
					authenticationContext.Session[sessionKey] = this.FilterPermissionConfigByUserPermission(permissionConfigs, permissions);
				}

				return authenticationContext.Session[sessionKey] as IEnumerable<PermissionConfig>;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		#region Private Methods

		private bool HasPermissionInternal(Guid userId, string permissionValue)
		{
			try
			{
				// returns true always if user is system administrator.
				if (this.roleApi.IsUserInRole(userId, this.platformConfiguration.Role.RoleId))
					return true;

				PermissionObject targetPermissionObject = new PermissionObject(permissionValue);
				IEnumerable<PermissionObject> userPermissions = this.FindUserPermissions(userId, false);
				foreach (PermissionObject permissionObject in userPermissions)
					if (permissionObject.Contains(targetPermissionObject))
						return true;

				return false;
			}
			catch (Exception exp)
			{
				Logger.Instance(this).Error(exp);
				throw;
			}
		}

		private static void ReplaceTextGlobalizationIdentifiers(IEnumerable<PermissionConfig> permissionConfigEnumerable)
		{
			if (permissionConfigEnumerable == null || permissionConfigEnumerable.Count() == 0) return;

			foreach (PermissionConfig permissionConfig in permissionConfigEnumerable)
			{
				permissionConfig.Text = GlobalizationUtility.ReplaceGlobalizationVariables(permissionConfig.Text);
				ReplaceTextGlobalizationIdentifiers(permissionConfig.Permission);
			}
		}

		/// <summary>
		/// Filter all permission configs not included in specified user permissions.
		/// </summary>
		/// <param name="permissionConfigEnumerable"></param>
		/// <param name="userPermissions"></param>
		/// <returns></returns>
		private IEnumerable<PermissionConfig> FilterPermissionConfigByUserPermission(IEnumerable<PermissionConfig> permissionConfigEnumerable, IEnumerable<PermissionObject> userPermissions)
		{
			List<PermissionConfig> results = new List<PermissionConfig>();
			foreach (PermissionConfig permissionConfig in permissionConfigEnumerable)
			{
				bool isEmptyPermissionValue = Kit.IsEmpty(permissionConfig.Value);
				if (!isEmptyPermissionValue && !ContainsPermissionValue(userPermissions, permissionConfig.Value))
					continue;

				PermissionConfig newPermissionConfig = new PermissionConfig
				{
					Permission = permissionConfig.Permission,
					Text = permissionConfig.Text,
					Value = permissionConfig.Value,
				};

				if (newPermissionConfig.Permission != null && newPermissionConfig.Permission.Length > 0)
				{
					IEnumerable<PermissionConfig> subPermissionConfigs = FilterPermissionConfigByUserPermission(permissionConfig.Permission, userPermissions);
					newPermissionConfig.Permission = subPermissionConfigs.ToArray();
				}

				if (isEmptyPermissionValue && (newPermissionConfig.Permission == null || newPermissionConfig.Permission.Length == 0))
					continue;

				results.Add(newPermissionConfig);
			}

			return results;
		}

		private bool ContainsPermissionValue(IEnumerable<PermissionObject> permissions, string targetPermissionValue)
		{
			foreach (PermissionObject permissionObject in permissions)
				if (permissionObject.Contains(targetPermissionValue)) return true;

			return false;
		}

		private IEnumerable<PermissionObject> AttachImplicitPermissions(string domain, IEnumerable<PermissionObject> explicitPermissions)
		{
			if (!this.ImplicitPermission.ContainsKey(domain)) 
				return new List<PermissionObject>();

			List<string> implicitPermissionValues = new List<string>();
			Dictionary<string, List<string>> implicitPermissionContainer = ImplicitPermission.ContainsKey(domain) ? ImplicitPermission[domain] : new Dictionary<string, List<string>>();
			foreach (PermissionObject explicitPermission in explicitPermissions)
			{
				if (implicitPermissionContainer.ContainsKey(explicitPermission.PermissionValue))
					implicitPermissionValues.AddRange(implicitPermissionContainer[explicitPermission.PermissionValue]);
			}

			IEnumerable<PermissionObject> implicitPermissions = implicitPermissionValues.Select(p => (PermissionObject)p);
			return explicitPermissions.Concat(implicitPermissions).Distinct();
		}

		#endregion
	}
}
