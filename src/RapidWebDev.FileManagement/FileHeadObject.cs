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
using System.ServiceModel;
using System.IO;
using System.Runtime.Serialization;
using RapidWebDev.Common;

namespace RapidWebDev.FileManagement
{
	/// <summary>
	/// File head object which includes basic information of a file but not the stream.
	/// </summary>
	[DataContract(Name = "FileHeadObject", Namespace = ServiceNamespaces.FileManagement)]
	public class FileHeadObject
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[DataMember]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the file name only without directory information.
		/// </summary>
		/// <value>The file name without directory information.</value>
		[DataMember]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets extension name of the file.
		/// </summary>
		/// <value>Extension name of the file.</value>
		[DataMember]
		public string FileExtensionName { get; set; }

		/// <summary>
		/// Gets or sets the category of file.
		/// </summary>
		/// <value>The category of file.</value>
		[DataMember]
		public string Category { get; set; }

		/// <summary>
		/// Gets or sets the file description.
		/// </summary>
		/// <value>The file description.</value>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets size of the file in byte.
		/// </summary>
		/// <value>The bytes count.</value>
		[DataMember]
		public long BytesCount { get; set; }

		/// <summary>
		/// Gets/sets file version.
		/// </summary>
		[DataMember]
		public int Version { get; set; }

		/// <summary>
		/// Gets or sets the time when the file is created.
		/// </summary>
		/// <value>The created on.</value>
		[DataMember]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the time when the file is last updated.
		/// </summary>
		/// <value>The updated on.</value>
		[DataMember]
		public DateTime? UpdatedOn { get; set; }
	}
}
