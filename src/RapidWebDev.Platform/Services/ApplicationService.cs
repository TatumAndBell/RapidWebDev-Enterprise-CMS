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
using System.Globalization;
using RapidWebDev.Platform.Properties;
using System.ServiceModel.Activation;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// external service for populate Application Object
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ApplicationService : IApplicationService
    {
        private IApplicationApi applicationApi = SpringContext.Current.GetObject<IApplicationApi>();



        #region IApplicationService Members
        /// <summary>
        /// Save application. Empty application id of application object means that the specified application object is new created. 
        /// And after it's inserted into database, the database generated id will be set back to property id.
        /// Non-empty application id means to update an existed application. 
        /// If the non-empty application id is not available in database, it throws exception ValidationException.
        /// </summary>
        /// <param name="applicationObject"></param>
        /// <exception cref="RapidWebDev.Common.Validation.ValidationException">If the non-empty application id is not available in database, it throws exception ValidationException.</exception>

        public string SaveJson(ApplicationObject applicationObject)
        {
            if (applicationObject == null) throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidApplicationID + "{0}", "applicationObject cannot be null"));
            try 
            {
                applicationApi.Save(applicationObject);
                return applicationObject.Id.ToString();
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
        /// Get application biz object by id. Returns null if the application id is invalid.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns>returns null if the application id is invalid.</returns>

        public ApplicationObject GetJson(string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId)) throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidApplicationID + "{0}", "applicationId cannot be null"));
            try
            {
                return applicationApi.Get(new Guid(applicationId));
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
        /// Get application biz object by name with ignoring character case. Returns null if the application id is invalid.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns>Returns null if the application name is invalid.</returns>

        public ApplicationObject GetByNameJson(string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName)) throw new BadRequestException(string.Format(CultureInfo.InvariantCulture,  "{0}", "applicationName cannot be null"));
            try
            {
                return applicationApi.Get(applicationName);
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
        /// Returns true if specified application name exists with ignoring character case.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns>Returns true if specified application name exists.</returns>

        public bool ExistsJson(string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName)) throw new BadRequestException(string.Format(CultureInfo.InvariantCulture,  "{0}", "applicationName cannot be null"));
            try
            {
                return applicationApi.Exists(applicationName);
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
    }
}
