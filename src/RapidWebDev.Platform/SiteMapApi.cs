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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Properties;

namespace RapidWebDev.Platform
{
    /// <summary>
    /// The implementation is to populate all sitemap configurations for users.
    /// </summary>
    public class SiteMapApi : ISiteMapApi
    {
        private static object syncObj = new object();
		private IPlatformConfiguration platformConfiguration;
        private IAuthenticationContext authenticationContext;
        private IRoleApi roleApi;
        private IOrganizationApi organizationApi;
		private IPermissionApi permissionApi;
        private string siteMapFilePath;
		private volatile Dictionary<CultureInfo, Dictionary<string, IEnumerable<SiteMapItemConfig>>> allSiteMapItemConfigs = new Dictionary<CultureInfo, Dictionary<string, IEnumerable<SiteMapItemConfig>>>();

        /// <summary>
        /// Gets all sitemap item configurations existed in system.
        /// </summary>
        private IEnumerable<SiteMapItemConfig> AllSiteMapItemConfigs
        {
            get
            {
				Dictionary<string, IEnumerable<SiteMapItemConfig>> siteMapItemConfigByDomain = null;
				CultureInfo currentCulture = Resources.Culture ?? CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;
				if (!this.allSiteMapItemConfigs.ContainsKey(currentCulture))
                {
                    lock (syncObj)
                    {
						if (!this.allSiteMapItemConfigs.ContainsKey(currentCulture))
                        {
                            string siteMapFilePath = Kit.ToAbsolutePath(this.siteMapFilePath);
                            XmlDocument xmldoc = new XmlDocument();
                            xmldoc.Load(siteMapFilePath);
                            string schemaXml = Kit.GetManifestFile(typeof(IPermissionApi), "SiteMapConfig.xsd");
                            XmlSchema schema = XmlSchema.Read(new StringReader(schemaXml), null);
                            Kit.ValidateXml(schema, xmldoc);

                            XmlSerializer serializer = new XmlSerializer(typeof(SiteMapConfig));
                            using (FileStream fileStream = new FileStream(siteMapFilePath, FileMode.Open, FileAccess.Read))
                            {
                                SiteMapConfig siteMapConfig = serializer.Deserialize(fileStream) as SiteMapConfig;
								IEnumerable<SiteMapItemConfig> siteMapConfigToProcess = siteMapConfig.Domain.SelectMany(domain => domain.Item);
								ReplaceTextGlobalizationIdentifiers(siteMapConfigToProcess);

								int siteMapItemIndexer = 1;
								CreateSiteMapItemId(siteMapConfigToProcess, ref siteMapItemIndexer);

								siteMapItemConfigByDomain = siteMapConfig.Domain.ToDictionary(d => d.Value, d => d.Item.AsEnumerable());
								this.allSiteMapItemConfigs[currentCulture] = siteMapItemConfigByDomain;
                            }
                        }
                    }
                }

				if (!authenticationContext.Identity.IsAuthenticated)
					throw new UnauthorizedAccessException(Resources.InvalidAuthentication);

				if (authenticationContext.Organization == null)
					throw new UnauthorizedAccessException(Resources.InvalidOrganizationID);

				OrganizationTypeObject orgType = organizationApi.GetOrganizationType(authenticationContext.Organization.OrganizationTypeId);
				if (siteMapItemConfigByDomain == null)
					siteMapItemConfigByDomain = this.allSiteMapItemConfigs[currentCulture];

				if (!siteMapItemConfigByDomain.ContainsKey(orgType.Domain))
					return new List<SiteMapItemConfig>();

				return siteMapItemConfigByDomain[orgType.Domain];
            }
        }

        /// <summary>
		/// Construct SiteMapApi instance
        /// </summary>
        /// <param name="authenticationContext"></param>
        /// <param name="roleApi"></param>
        /// <param name="organizationApi"></param>
		/// <param name="permissionApi"></param>
        /// <param name="platformConfiguration"></param>
        /// <param name="siteMapFilePath"></param>
		public SiteMapApi(IAuthenticationContext authenticationContext, IRoleApi roleApi, IOrganizationApi organizationApi, IPermissionApi permissionApi, IPlatformConfiguration platformConfiguration, string siteMapFilePath)
        {
            this.authenticationContext = authenticationContext;
            this.roleApi = roleApi;
            this.organizationApi = organizationApi;
			this.permissionApi = permissionApi;
            this.platformConfiguration = platformConfiguration;
            this.siteMapFilePath = siteMapFilePath;
        }

        /// <summary>
        /// Returns all sitemap item configurations accessible to specified user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<SiteMapItemConfig> FindSiteMapConfig(Guid userId)
        {
            try
            {
				string sessionKey = "FindSiteMapConfig_" + userId.ToString("N");
				if (authenticationContext.Session[sessionKey] == null)
				{
					IEnumerable<SiteMapItemConfig> siteMapItemConfigs = this.AllSiteMapItemConfigs;

					// if the user is system administrator, he should have all permissions
					if (this.roleApi.IsUserInRole(userId, this.platformConfiguration.Role.RoleId))
						return siteMapItemConfigs;

					authenticationContext.Session[sessionKey] = this.FilterSiteMapItemsByUserPermission(siteMapItemConfigs, userId);
				}

				return authenticationContext.Session[sessionKey] as IEnumerable<SiteMapItemConfig>;
            }
            catch (Exception exp)
            {
                Logger.Instance(this).Error(exp);
                throw;
            }
        }

        #region Private Methods

		private static void ReplaceTextGlobalizationIdentifiers(IEnumerable<SiteMapItemConfig> siteMapItemConfigEnumerable)
		{
			if (siteMapItemConfigEnumerable == null || siteMapItemConfigEnumerable.Count() == 0) return;

			foreach (SiteMapItemConfig siteMapItemConfig in siteMapItemConfigEnumerable)
			{
				siteMapItemConfig.Text = GlobalizationUtility.ReplaceGlobalizationVariables(siteMapItemConfig.Text);
				ReplaceTextGlobalizationIdentifiers(siteMapItemConfig.Item);
			}
		}

		private static void CreateSiteMapItemId(IEnumerable<SiteMapItemConfig> siteMapItemConfigEnumerable, ref int counter)
		{
			if (siteMapItemConfigEnumerable == null || siteMapItemConfigEnumerable.Count() == 0) return;

			foreach (SiteMapItemConfig siteMapItemConfig in siteMapItemConfigEnumerable)
			{
				siteMapItemConfig.Id = "SiteMapItemAutoAssignedId" + counter++;
				CreateSiteMapItemId(siteMapItemConfig.Item, ref counter);
			}
		}

        /// <summary>
        /// Filter all sitemap item configs not included in specified user permissions.
        /// </summary>
        /// <param name="siteMapItemConfigEnumerable"></param>
		/// <param name="userId"></param>
        /// <returns></returns>
        private IEnumerable<SiteMapItemConfig> FilterSiteMapItemsByUserPermission(IEnumerable<SiteMapItemConfig> siteMapItemConfigEnumerable, Guid userId)
        {
            List<SiteMapItemConfig> results = new List<SiteMapItemConfig>();

            int totalCount = siteMapItemConfigEnumerable.Count();
            int authenticatedSiteMapItemCount = 0;
            SiteMapItemConfig previousSiteMapItemConfig = null;
            for (int i = 0; i < totalCount; i++)
            {
                SiteMapItemConfig siteMapItemConfig = siteMapItemConfigEnumerable.ElementAt(i);
                bool emptySiteMapItem = Kit.IsEmpty(siteMapItemConfig.Value);
				if (!emptySiteMapItem)
				{
					// remove an unauthenticated sitemap item with non-empty value.
					bool authenticatedSiteMapItem = this.HasPermission(userId, siteMapItemConfig);
					if (!authenticatedSiteMapItem) continue;
				}

                IEnumerable<SiteMapItemConfig> includedSiteMapItemConfigEnumerable = null;
                if (siteMapItemConfig.Item != null && siteMapItemConfig.Item.Length > 0)
                    includedSiteMapItemConfigEnumerable = FilterSiteMapItemsByUserPermission(siteMapItemConfig.Item, userId);

                // remove the "ancestor" sitemap item (SiteMapItem.Value is null or empty) without child
                if (emptySiteMapItem
					&& siteMapItemConfig.Type != SiteMapItemTypes.Separator
                    && (includedSiteMapItemConfigEnumerable == null || includedSiteMapItemConfigEnumerable.Count() == 0))
                {
                    continue;
                }

                SiteMapItemConfig newSiteMapItemConfig = new SiteMapItemConfig
                {
					Id = siteMapItemConfig.Id,
                    ClientSideCommand = siteMapItemConfig.ClientSideCommand,
					PageUrl = siteMapItemConfig.PageUrl, 
					IconClassName = siteMapItemConfig.IconClassName,
                    Item = siteMapItemConfig.Item,
                    Text = siteMapItemConfig.Text,
                    Type = siteMapItemConfig.Type,
                    Value = siteMapItemConfig.Value
                };

                // set processed children back to siteMapItemConfig
                newSiteMapItemConfig.Item = (includedSiteMapItemConfigEnumerable != null && includedSiteMapItemConfigEnumerable.Count() > 0) ? includedSiteMapItemConfigEnumerable.ToArray() : null;

                if (newSiteMapItemConfig.Type == SiteMapItemTypes.Separator)
                {
                    // "Separator" never be first
                    if (authenticatedSiteMapItemCount == 0) continue;

                    // no continuous 2 "Separator"
                    if (previousSiteMapItemConfig != null && previousSiteMapItemConfig.Type == SiteMapItemTypes.Separator) continue;

                    // "Separator" never be last
                    if (i == totalCount - 1) continue;
                }

                previousSiteMapItemConfig = newSiteMapItemConfig;
                results.Add(newSiteMapItemConfig);
                authenticatedSiteMapItemCount++;
            }

            return results;
        }

		/// <summary>
		/// Check the user whether has the permission to the sitemap item.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="siteMapItem"></param>
		/// <returns></returns>
		private bool HasPermission(Guid userId, SiteMapItemConfig siteMapItem)
		{
			if (siteMapItem == null || string.IsNullOrEmpty(siteMapItem.Value)) return true;
			return this.permissionApi.HasPermission(userId, siteMapItem.Value);
		}

        #endregion
    }
}
