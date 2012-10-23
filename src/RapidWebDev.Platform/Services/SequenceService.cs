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
using System.Globalization;
using RapidWebDev.Platform.Properties;
using System.Collections.ObjectModel;

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// external service for populate sequence number
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SequenceService : ISequenceService
    {
        private ISequenceNoApi sequenceNoApi = SpringContext.Current.GetObject<ISequenceNoApi>();

        #region ISequenceService Members
      
        /// <summary>
        /// Create sequence number on specified type for special object id.
        /// The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
        /// </summary>
        /// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
        /// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
        /// <returns></returns>
        public long CreateSingleJson(string objectId, string sequenceNoType)
        {
            if (string.IsNullOrEmpty(sequenceNoType)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "{0}", "sequenceNoType cannot be empty"));

             

            try
            {
                if (string.IsNullOrEmpty(objectId))
                    return sequenceNoApi.Create(sequenceNoType);
                else
                    return sequenceNoApi.Create(new Guid(objectId), sequenceNoType);
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
        /// Create sequence numbers on specified type for special object id.
        ///  The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
        /// </summary>
        /// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
        /// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
        /// <param name="sequenceNoCount">Indicates how many sequence number is required.</param>
        /// <returns></returns>
        public Collection<long> CreateMultipleJson(string objectId, string sequenceNoType, string sequenceNoCount)
        {
            if (string.IsNullOrEmpty(sequenceNoType)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "{0}", "sequenceNoType cannot be empty"));

            if (string.IsNullOrEmpty(objectId)) 
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "{0}", "objectId cannot be empty"));

            try 
            {
                if (string.IsNullOrEmpty(objectId))
                    return new Collection<long>(sequenceNoApi.Create(sequenceNoType, ushort.Parse(sequenceNoCount)).ToList()); 
                else
                    return new Collection<long>(sequenceNoApi.Create(new Guid(objectId), sequenceNoType, ushort.Parse(sequenceNoCount)).ToList());
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



        #region ISequenceService Members

        /// <summary>
        /// Create sequence number on specified type for special object id.
        /// The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
        /// </summary>
        /// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
        /// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
        /// <returns></returns>
        public long CreateSingleXml(string objectId, string sequenceNoType)
        {
            return CreateSingleJson(objectId, sequenceNoType);
        }
        /// <summary>
        /// Create sequence numbers on specified type for special object id.
        ///  The method generates the sequence number suppressing transaction scope which means the generated sequence number cannot be rollback.
        /// </summary>
        /// <param name="objectId">E.g. we want each company in the system owns standalone sequence number generation. That means company A can have a sequence number 999 as company B has it meanwhile.</param>
        /// <param name="sequenceNoType">Sequence number type that means a company for above example can has multiple sequence number generation path. E.g. company A has a Order sequence number 999 as it has a Product sequence number 999 meanwhile.</param>
        /// <param name="sequenceNoCount">Indicates how many sequence number is required.</param>
        /// <returns></returns>
        public Collection<long> CreateMultipleXml(string objectId, string sequenceNoType, string sequenceNoCount)
        {
            return CreateMultipleJson(objectId, sequenceNoType, sequenceNoCount);
        }

        #endregion
    }
}
