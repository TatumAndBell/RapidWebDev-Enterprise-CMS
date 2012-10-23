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
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.Common.Validation;

namespace RapidWebDev.ExtensionModel.Linq
{
	/// <summary>
	/// Extended Linq DataContext which intercepts changes on submitted entities which implement the interface <see cref="IExtensionObject"/>. 
	/// The interception serializes dynamic properties of entities into xml string and saves to the property ExtensionData of the entities.
	/// </summary>
	public class ExtensionDataContext : DataContext
	{
		/// <summary>
		/// Initializes a new instance of the System.Data.Linq.DataContext class by referencing the connection used by the .NET Framework.
		/// </summary>
		/// <param name="connection">The connection used by the .NET Framework.</param>
		public ExtensionDataContext(IDbConnection connection) : base(connection) { }

		/// <summary>
		/// Initializes a new instance of the System.Data.Linq.DataContext class by referencing a file source.
		/// </summary>
		/// <param name="fileOrServerOrConnection">
		/// This argument can be any one of the following: The name of a file where a SQL Server Express database resides.
		/// The name of a server where a database is present.
		/// In this case the provider uses the default database for a user.
		/// A complete connection string. LINQ to SQL just passes the string to the provider without modification.
		/// </param>
		public ExtensionDataContext(string fileOrServerOrConnection) : base(fileOrServerOrConnection) { }

		/// <summary>
		/// Initializes a new instance of the System.Data.Linq.DataContext class by referencing a connection and a mapping source.
		/// </summary>
		/// <param name="connection">The connection used by the .NET Framework.</param>
		/// <param name="mapping">The System.Data.Linq.Mapping.MappingSource.</param>
		public ExtensionDataContext(IDbConnection connection, MappingSource mapping) : base(connection, mapping) { }

		/// <summary>
		/// Initializes a new instance of the System.Data.Linq.DataContext class by referencing a file source and a mapping source.
		/// </summary>
		/// <param name="fileOrServerOrConnection">
		/// This argument can be any one of the following: The name of a file where a SQL Server Express database resides.
		/// The name of a server where a database is present.
		/// In this case the provider uses the default database for a user.
		/// A complete connection string. LINQ to SQL just passes the string to the provider without modification.
		/// </param>
		/// <param name="mapping">The System.Data.Linq.Mapping.MappingSource.</param>
		public ExtensionDataContext(string fileOrServerOrConnection, MappingSource mapping) : base(fileOrServerOrConnection, mapping) { }

		/// <summary>
		/// Sends changes that were made to retrieved objects to the underlying database, and specifies the action to be taken if the submission fails.
		/// </summary>
		/// <param name="failureMode">The action to be taken if the submission fails.</param>
		public override void SubmitChanges(ConflictMode failureMode)
		{
			ChangeSet changeSet = this.GetChangeSet();
			IEnumerable<object> entities = changeSet.Inserts.Union(changeSet.Updates);

			using (ValidationScope validationScope = new ValidationScope())
			{
				foreach (object entity in entities)
				{
					IExtensionObject extensionObject = entity as IExtensionObject;
					if (extensionObject != null && (extensionObject.HasChanged || extensionObject.ExtensionData == null))
					{
						IExtensionObjectSerializer serializer = SpringContext.Current.GetObject<IExtensionObjectSerializer>();
						serializer.Serialize(extensionObject);
					}
				}
			}

			base.SubmitChanges(failureMode);
		}
	}
}