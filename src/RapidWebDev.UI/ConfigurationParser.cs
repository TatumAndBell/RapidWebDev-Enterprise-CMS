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
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI
{
    /// <summary>
    /// Configuration parser for workshop controls.
    /// </summary>
    public abstract class ConfigurationParser : IConfigurationParser, IDisposable
    {
        private object syncObj = new object();
		private bool configurationsLoaded;
		private List<string> directories = new List<string>();
		private List<string> files = new List<string>();
        private Dictionary<string, object> objects = new Dictionary<string, object>();
		private List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();

		private IDictionary<string, object> Objects
		{
			get
			{
				if (!this.configurationsLoaded)
				{
					lock (this.syncObj)
					{
						if (!this.configurationsLoaded)
						{
							List<string> allFiles = new List<string>();
							if (this.directories != null)
							{
								foreach (string directory in this.directories)
								{
									if (!Directory.Exists(directory)) continue;
									string[] includedFiles = Directory.GetFiles(directory, "*.dp.xml", SearchOption.AllDirectories);
									foreach (string includedFile in includedFiles)
									{
										string loweredIncludedFile = includedFile.ToLowerInvariant();
										allFiles.Add(loweredIncludedFile);
									}
								}
							}

							if (this.files != null)
							{
								foreach (string file in this.files)
									allFiles.Add(file);
							}

							this.objects.Clear();
							foreach (string xmlFile in allFiles.Distinct())
							{
								try
								{
									XmlDocument xmldoc = this.ValidateInputFile(xmlFile);
									var createdObject = this.CreateConfigurationObject(xmldoc);
									if (this.objects.ContainsKey(createdObject.Key))
										throw new ConfigurationErrorsException(string.Format(Resources.DP_DuplicateConfigurationObject, createdObject.Key));

									this.objects[createdObject.Key] = createdObject.Value;
								}
								catch (Exception exp)
								{
									throw new ConfigurationErrorsException(string.Format(Resources.DP_ConfigurationFileParsedIncorrect, xmlFile), exp);
								}
							}

							this.configurationsLoaded = true;
						}
					}
				}

				return this.objects;
			}
		}

        /// <summary>
		/// Get xml parser instance.
		/// </summary>
		protected abstract XmlParser XmlParser { get; }

        /// <summary>
        /// Construct ConfigurationParser instance.
        /// </summary>
        /// <param name="directories">configuration xml directories contained xml files, which will be parsed into workshop objects.</param>
        /// <param name="files">configuration xml files to be parsed into workshop objects.</param>
        public ConfigurationParser(IEnumerable<string> directories, IEnumerable<string> files)
        {
			if (directories != null)
			{
				foreach (string directory in directories)
				{
					string path = Kit.ToAbsolutePath(directory).ToLowerInvariant();
					if (!this.directories.Contains(path))
					{
						this.directories.Add(path);
						this.CreateFileSystemWatcher(path);
					}
				}
			}

			if (files != null)
			{
				foreach (string file in files)
				{
					string path = Kit.ToAbsolutePath(file).ToLowerInvariant();
					if (!this.files.Contains(path))
					{
						this.files.Add(path);
						this.CreateFileSystemWatcher(path);
					}
				}
			}
        }

        /// <summary>
        /// Get IPageWorkshop object by objectId.
        /// </summary>
        /// <param name="objectId">Specified object id.</param>
        /// <returns></returns>
        public T GetObject<T>(string objectId)
        {
			if (!this.Objects.ContainsKey(objectId))
				throw new NotImplementedException(string.Format(Resources.DP_ObjectIdNotConfigured, objectId));

			return (T)this.Objects[objectId];
        }

        /// <summary>
        /// Returns true if ConfigurationParser contains specified object id.
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public bool ContainsObject(string objectId)
        {
			return this.Objects.ContainsKey(objectId);
        }

        /// <summary>
		/// Create object by specified xml doc and returns created pair (object id and object).
        /// </summary>
		/// <param name="xmldoc"></param>
		protected abstract KeyValuePair<string, object> CreateConfigurationObject(XmlDocument xmldoc);

        private XmlDocument ValidateInputFile(string xmlFile)
        {
            if (!File.Exists(xmlFile))
				throw new FileNotFoundException(Resources.DP_ConfigurationFileNotExists, xmlFile);

            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(xmlFile);
                Kit.ValidateXml(this.XmlParser.Schema, xmldoc);
                return xmldoc;
            }
            catch (XmlSchemaException)
            {
                throw;
            }
            catch (Exception exp)
            {
				throw new XmlSchemaException(string.Format(Resources.DP_InvalidConfigurationFileSchema, xmlFile), exp);
            }
        }

		private void CreateFileSystemWatcher(string path)
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(path);
			fileSystemWatcher.IncludeSubdirectories = true;
			fileSystemWatcher.InternalBufferSize = 2048;
			fileSystemWatcher.Filter = "*.dp.xml";
			fileSystemWatcher.Changed += (sender, e) => { lock (this.syncObj) { this.configurationsLoaded = false; } };
			fileSystemWatcher.Created += (sender, e) => { lock (this.syncObj) { this.configurationsLoaded = false; } };
			fileSystemWatcher.Deleted += (sender, e) => { lock (this.syncObj) { this.configurationsLoaded = false; } };
			fileSystemWatchers.Add(fileSystemWatcher);

			fileSystemWatcher.EnableRaisingEvents = true;
		}

		#region IDisposable Members

		/// <summary>
		/// The dispose method
		/// </summary>
		/// <param name="disposing">called from Dispose?</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fileSystemWatchers.ForEach(watcher => watcher.Dispose());
				this.fileSystemWatchers.Clear();
			}
		}

		/// <summary>
		/// The dispose method
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}

