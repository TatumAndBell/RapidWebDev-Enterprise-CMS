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
using System.IO;
using RapidWebDev.Common;
using RapidWebDev.FileManagement;

namespace RapidWebDev.Tests.FileManagement
{
	internal class MemoryFileStorageApi : IFileStorageApi, IDisposable
	{
		private Dictionary<Guid, Stream> fileStreamContainer = new Dictionary<Guid, Stream>();

		#region IFileStorageApi Members

		public long Store(Guid applicationId, string category, Guid fileId, string fileExtensionName, Stream fileStream)
		{
			MemoryStream memStream = new MemoryStream();
			byte[] buffer = new byte[4096];
			int byteCount = 1;
			long totalByteCount = 0;

			while (byteCount > 0)
			{
				byteCount = fileStream.Read(buffer, 0, 4096);
				totalByteCount += byteCount;
				memStream.Write(buffer, 0, byteCount);
			}

			this.fileStreamContainer[fileId] = memStream;
			return totalByteCount;
		}

		public Stream Load(FileHeadObject fileHeadObject)
		{
			Kit.NotNull(fileHeadObject, "fileHeadObject");
			if (!this.fileStreamContainer.ContainsKey(fileHeadObject.Id))
				return null;

			MemoryStream memStream = new MemoryStream();
			Stream sourceStream = this.fileStreamContainer[fileHeadObject.Id];
			byte[] buffer = new byte[4096];
			int byteCount = 1;

			while (byteCount > 0)
			{
				byteCount = sourceStream.Read(buffer, 0, 4096);
				memStream.Write(buffer, 0, byteCount);
			}

			memStream.Position = 0;
			return memStream;
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			foreach (Guid fileId in this.fileStreamContainer.Keys)
			{
				Stream stream = this.fileStreamContainer[fileId];
				if (stream.CanRead || stream.CanSeek || stream.CanWrite)
					stream.Dispose();
			}

			this.fileStreamContainer.Clear();
		}

		#endregion
	}
}
