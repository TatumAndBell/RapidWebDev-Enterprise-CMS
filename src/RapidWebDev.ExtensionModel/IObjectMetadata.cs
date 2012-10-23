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
using System.Linq;
using System.Text;

namespace RapidWebDev.ExtensionModel
{
	/// <summary>
    /// Extension type's metadata
	/// </summary>
	public interface IObjectMetadata
	{
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
		string Name
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
		string Category
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
		string Description
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the last updated time.
        /// </summary>
        /// <value>The last updated on.</value>
		DateTime LastUpdatedOn
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the parent object metadata id.
        /// </summary>
        /// <value>The parent object metadata id.</value>
		Guid? ParentObjectMetadataId
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
		Guid Id
		{
			get;
			set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is global object metadata.
        /// A global object metadata means the definition doesn't belong to any special applications in SAAS architect but all applications can share it.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is global object metadata; otherwise, <c>false</c>.
        /// </value>
		bool IsGlobalObjectMetadata
		{
			get;
			set;
		}
	}
}