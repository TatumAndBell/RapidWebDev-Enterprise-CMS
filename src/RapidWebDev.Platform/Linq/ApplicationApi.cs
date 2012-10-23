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
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using RapidWebDev.Platform.Properties;
using RapidWebDev.Common.Data;

namespace RapidWebDev.Platform.Linq
{
    /// <summary>
    /// The implementation of operating application
    /// </summary>
    public class ApplicationApi : CachableApi, IApplicationApi
    {
        /// <summary>
        /// Save application. Empty application id of application object means that the specified application object is new created. 
        /// And after it's inserted into database, the database generated id will be set back to property id.
        /// Non-empty application id means to update an existed application. 
		/// If the non-empty application id is not available in database, it throws exception ValidationException.
        /// </summary>
        /// <param name="applicationObject"></param>
		/// <exception cref="ValidationException">If the non-empty application id is not available in database, it throws exception ValidationException.</exception>
        public void Save(ApplicationObject applicationObject)
        {
            Kit.NotNull(applicationObject, "applicationObject");

            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                Application application = null;
                if (applicationObject.Id != Guid.Empty)
                {
                    application = ctx.Applications.FirstOrDefault(app => app.ApplicationId == applicationObject.Id);
                    if (application == null)
						throw new ValidationException(string.Format(Resources.InvalidApplicationID, applicationObject.Id));
                }
                else
                {
					application = CreateApplication(applicationObject);
                    ctx.Applications.InsertOnSubmit(application);
                }

                string originalApplicationName = application.ApplicationName;
                application.ApplicationName = applicationObject.Name;
                application.LoweredApplicationName = applicationObject.Name.ToLowerInvariant();
                application.Description = applicationObject.Description;
				application.ParseExtensionPropertiesFrom(applicationObject);
                ctx.SubmitChanges();

                applicationObject.Id = application.ApplicationId;
                base.RemoveCache(originalApplicationName);
				base.RemoveCache(applicationObject.Id);
            }
        }

        /// <summary>
        /// Get application biz object by id. Returns null if the application id is invalid.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns>returns null if the application id is invalid.</returns>
        public ApplicationObject Get(Guid applicationId)
        {
            ApplicationObject applicationObject = base.GetCacheObject<ApplicationObject>(applicationId);
            if (applicationObject != null) return applicationObject.Clone();

            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                Application application = ctx.Applications.FirstOrDefault(app => app.ApplicationId == applicationId);
                if (application == null) return null;

                applicationObject = new ApplicationObject
                {
                    Id = application.ApplicationId,
                    Name = application.ApplicationName,
                    Description = application.Description
                };

				applicationObject.ParseExtensionPropertiesFrom(application);
                base.AddCache(applicationObject.Id, applicationObject);
                return applicationObject;
            }
        }

        /// <summary>
        /// Get application biz object by name with ignoring character case. Returns null if the application id is invalid.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns>Returns null if the application name is invalid.</returns>
        public ApplicationObject Get(string applicationName)
        {
            string loweredApplicationName = applicationName.ToLowerInvariant();
            Guid applicationId = base.GetCacheObject<Guid>(loweredApplicationName);
			if (applicationId != Guid.Empty) return this.Get(applicationId);

            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
				applicationId = ctx.Applications
					.Where(app => app.LoweredApplicationName == loweredApplicationName)
					.Select(app => app.ApplicationId)
					.FirstOrDefault();

				if (applicationId == default(Guid)) return null;

                base.AddCache(loweredApplicationName, applicationId);
                return this.Get(applicationId);
            }
        }

        /// <summary>
        /// Returns true if specified application name exists with ignoring character case.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns>Returns true if specified application name exists.</returns>
        public bool Exists(string applicationName)
        {
			return this.Get(applicationName) != null;
        }

		private static Application CreateApplication(ApplicationObject applicationObject)
		{
			Application application = new Application { ExtensionDataTypeId = applicationObject.ExtensionDataTypeId };
			IEnumerator<KeyValuePair<string, object>> properties = applicationObject.GetFieldEnumerator();
			while (properties.MoveNext())
				application[properties.Current.Key] = properties.Current.Value;

			return application;
		}
    }
}
