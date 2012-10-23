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
using System.Linq;
using System.Text;

namespace RapidWebDev.Common.Data
{
	/// <summary>
	/// DataContext configuration which defines all data context used in platform.
	/// </summary>
	public class DataContextConfiguration : ConfigurationSection
	{
		/// <summary>
		/// Data Context Setting Collection
		/// </summary>
		[ConfigurationProperty("settings")]
		public DataContextSettings DataContextSettings
		{
			get { return this["settings"] as DataContextSettings; }
		}

		/// <summary>
		/// Sets/gets true if MSDTC is started and transactions are allowed to use MSDTC potentially.
		/// </summary>
		[ConfigurationProperty("enabledMSDTC", IsRequired = false, DefaultValue = false)]
		public bool EnabledMSDTC
		{
			get { return (bool)this["enabledMSDTC"]; }
			set { this["enabledMSDTC"] = value; }
		}

		/// <summary>
		/// Sets/gets timeout in seconds of the underlying database command of data context.
		/// </summary>
		[ConfigurationProperty("commandTimeout", IsRequired = false, DefaultValue = 30)]
		public int CommandTimeout
		{
			get { return (int)this["commandTimeout"]; }
			set { this["commandTimeout"] = value; }
		}
	}

	/// <summary>
	/// DataContextSetting configuration collection.
	/// </summary>
	public class DataContextSettings : ConfigurationElementCollection
	{
		/// <summary>
		/// When overridden in a derived class, creates a new System.Configuration.ConfigurationElement.
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new DataContextSetting();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			DataContextSetting dataContextSetting = element as DataContextSetting;
			return dataContextSetting.Name;
		}

		/// <summary>
		/// Gets the type of the System.Configuration.ConfigurationElementCollection.
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		/// <summary>
		/// Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.
		/// </summary>
		protected override string ElementName
		{
			get { return "setting"; }
		}
	}

	/// <summary>
	/// DataContext setting class.
	/// </summary>
	public class DataContextSetting : ConfigurationElement
	{
		/// <summary>
		/// Sets/gets data context name.
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get { return this["name"] as string; }
			set { this["name"] = value; }
		}

		/// <summary>
		/// Sets/gets data context type.
		/// </summary>
		[ConfigurationProperty("connectionType", IsRequired = true)]
		public string ConnectionType
		{
			get { return this["connectionType"] as string; }
			set { this["connectionType"] = value; }
		}

		/// <summary>
		/// Sets/gets database connection string name.
		/// </summary>
		[ConfigurationProperty("connectionStringName", IsRequired = true)]
		public string ConnectionStringName
		{
			get { return this["connectionStringName"] as string; }
			set { this["connectionStringName"] = value; }
		}
	}
}

