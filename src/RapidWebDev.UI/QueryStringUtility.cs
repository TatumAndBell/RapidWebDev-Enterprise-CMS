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
using System.Globalization;
using System.Web;
using RapidWebDev.Common;
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.UI
{
    /// <summary>
    /// Query string of request utility class
    /// </summary>
    public static class QueryStringUtility
    {
        /// <summary>
        /// Gets the entity id for update.
        /// </summary>
        public static string EntityId
        {
            get
            {
                string entityId = HttpContext.Current.Request.QueryString["EntityId"];
                if (string.IsNullOrEmpty(entityId) || string.Equals(entityId, "null", StringComparison.InvariantCultureIgnoreCase)) return null;

                return entityId;
            }
        }

        /// <summary>
        /// Gets object id from query string which references to configured dynamic page instance.
        /// </summary>
        public static string ObjectId
        {
            get
            {
				const string contextKey = "RapidWebDev.UI.QueryStringUtility::ObjectId";
				if (HttpContext.Current.Items.Contains(contextKey))
					return HttpContext.Current.Items[contextKey] as string;

				string[] segments = HttpContext.Current.Request.Url.Segments;
				int index = 0;
				while (index < segments.Length)
				{
					string segment = segments[index];
					if (!string.IsNullOrEmpty(segment))
					{
						segment = segment.Replace("/", "");
						if (segment.ToUpperInvariant().Contains(".SVC"))
							break;
					}

					index++;
				}

				if (index == segments.Length)
					throw new BadRequestException(@"Not found service host file ""*.svc"" in request Url.");

				string returnValue = segments[index - 1].Replace("/", "");
				if(string.IsNullOrEmpty(returnValue))
					throw new BadRequestException(@"Not found ObjectId implied in request Url for the dynamic page.");

				HttpContext.Current.Items[contextKey] = returnValue;
				return returnValue;
            }
        }

        /// <summary>
        /// Gets command argument for aggregate panel which used to do special command onto a bulk of entities.
        /// </summary>
        public static string CommandArgument { get { return HttpContext.Current.Request["CommandArgument"]; } }

        /// <summary>
        /// Gets query string "DeleteId" for DynamicPageDataServiceHandler to delete an existed entity before executing posted query.
        /// </summary>
        public static string DeleteId { get { return HttpContext.Current.Request["DeleteId"]; } }

        /// <summary>
		/// Gets query string "MetadataDataTypeName"
        /// </summary>
        public static string MetadataDataTypeName(IRequestHandler requestHandler)
        {
			return requestHandler.Parameters["MetadataDataTypeName"];
        }

        /// <summary>
        /// Gets render mode of detail panel.
        /// </summary>
        public static DetailPanelPageRenderModes DetailPanelPageRenderMode
        {
            get
            {
                if (string.IsNullOrEmpty(EntityId))
                    return DetailPanelPageRenderModes.New;

                string renderMode = HttpContext.Current.Request["RenderMode"] ?? "VIEW";
                switch (renderMode.ToUpperInvariant())
                {
                    case "UPDATE":
                        return DetailPanelPageRenderModes.Update;
                    case "VIEW":
                        return DetailPanelPageRenderModes.View;
                    default:
						throw new ResourceNotFoundException(string.Format(CultureInfo.InvariantCulture, @"The query string parameter RenderMode ""{0}""is not supported.", renderMode));
                }
            }
        }
    }
}
