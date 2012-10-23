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
using System.IO;
using RapidWebDev.Common;

namespace RapidWebDev.FileManagement
{
	/// <summary>
	/// Api to resolve icon of files from assembly manifest resources.
	/// </summary>
    public class FileIconApi : IFileIconApi
    {
        private const string LargeIconManifestPathTemplate = "RapidWebDev.FileManagement.Resources.FileIcons.Large.{0}.gif";
		private const string SmallIconManifestPathTemplate = "RapidWebDev.FileManagement.Resources.FileIcons.Small.{0}.gif";
		private const string LargeMiscellaneousIconManifestPath = "RapidWebDev.FileManagement.Resources.FileIcons.Large.misc.gif";
		private const string SmallMiscellaneousIconManifestPath = "RapidWebDev.FileManagement.Resources.FileIcons.Small.misc.gif";

		/// <summary>
		/// Resolves icon of the file.
		/// </summary>
		/// <param name="fileExtensionName">The file extension name.</param>
		/// <param name="iconSize">The icon size.</param>
		/// <returns></returns>
		public Stream ResolveIcon(string fileExtensionName, IconSize iconSize)
        {
            string iconManifestPath = string.Empty;
			switch (iconSize)
            {
				case IconSize.Pixel16x16:
					iconManifestPath = string.Format(SmallIconManifestPathTemplate, fileExtensionName.ToLower());
                    break;
				case IconSize.Pixel50x50:
					iconManifestPath = string.Format(LargeIconManifestPathTemplate, fileExtensionName.ToLower());
                    break;
            }

			Stream stream = typeof(FileIconApi).Assembly.GetManifestResourceStream(iconManifestPath);
            if (stream == null || stream.Length == 0)
            {
				switch (iconSize)
                {
					case IconSize.Pixel16x16:
						stream = typeof(FileIconApi).Assembly.GetManifestResourceStream(SmallMiscellaneousIconManifestPath);
                        break;
					case IconSize.Pixel50x50:
						stream = typeof(FileIconApi).Assembly.GetManifestResourceStream(LargeMiscellaneousIconManifestPath);
                        break;
                }
            }

            return stream;
        }
    }
}

