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

namespace RapidWebDev.Platform.Services
{
    /// <summary>
    /// Provide the flexible methods
    /// </summary>
    public class ServicesHelper
    {
        /// <summary>
        /// Convert Web query predicate to LinqPredicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static LinqPredicate ConvertWebPredicateToLinqPredicate(WebServiceQueryPredicate predicate)
        {
            if (predicate == null) return null;

            IList<object> LinqPredicateParameters = new List<object>();

            foreach (var param in predicate.Parameters)
            {
                try
                {
                    switch (param.Type)
                    {
                        case WebServiceQueryPredicateParameterTypes.DateTime:
                            LinqPredicateParameters.Add(Kit.ConvertType(param.Value, typeof(System.DateTime)));
                            break;
                        case WebServiceQueryPredicateParameterTypes.Decimal:
                            LinqPredicateParameters.Add(Kit.ConvertType(param.Value, typeof(System.Decimal)));
                            break;
                        case WebServiceQueryPredicateParameterTypes.Double:
                            LinqPredicateParameters.Add(Kit.ConvertType(param.Value, typeof(System.Double)));
                            break;
                        case WebServiceQueryPredicateParameterTypes.Guid:
                            LinqPredicateParameters.Add(Kit.ConvertType(param.Value, typeof(System.Guid)));
                            break;
                        case WebServiceQueryPredicateParameterTypes.Integer:
                            LinqPredicateParameters.Add(Kit.ConvertType(param.Value, typeof(System.Int32)));
                            break;
                        default:
							LinqPredicateParameters.Add(param.Value);
                            break;

                    }
                }
                catch (Exception exp)
                {
                    throw new BadRequestException(exp.Message);
                }
            }

            LinqPredicate linqPredicate = new LinqPredicate(predicate.Expression, LinqPredicateParameters.ToArray());
            return linqPredicate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Collection<Guid> ConvertStringCollectionToGuidCollection(Collection<string> from) 
        {
            Collection<Guid> to = new Collection<Guid>();
            foreach (var temp in from)
            {
                to.Add(new Guid(temp));
            }
            return to;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static IEnumerable<Guid> ConvertStringCollectionToGuidEnumerable(Collection<string> from)
        {
            IList<Guid> to = new List<Guid>();

            foreach (var temp in from)
            {
                to.Add(new Guid(temp));
            }
            return to;
        }

       
    }
}
