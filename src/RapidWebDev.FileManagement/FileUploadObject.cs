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

namespace RapidWebDev.FileManagement
{
	/// <summary>
	/// File uploading object
	/// </summary>
	[MessageContract]
	public class FileUploadObject
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[MessageHeader]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the file name only without directory information.
		/// </summary>
		/// <value>The file name without directory information.</value>
		[MessageHeader]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the category of file.
		/// </summary>
		/// <value>The category of file.</value>
		[MessageHeader]
		public string Category { get; set; }

		/// <summary>
		/// Gets or sets the file description.
		/// </summary>
		/// <value>The file description.</value>
		[MessageHeader]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the file stream.
		/// </summary>
		/// <value>The file stream.</value>
		[MessageBodyMember]
		public Stream @Stream { get; set; }
	}
}
