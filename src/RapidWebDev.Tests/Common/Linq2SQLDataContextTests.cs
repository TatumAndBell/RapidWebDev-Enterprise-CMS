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
using System.Threading;
using RapidWebDev.Common;
using RapidWebDev.Common.Caching;
using RapidWebDev.Common.Data;
using RapidWebDev.Platform.Linq;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Data;
using RapidWebDev.ExtensionModel.Linq;
using System.Globalization;

namespace RapidWebDev.Tests.Common
{
	[TestFixture]
	[Description(@"The test cases is used for us to better understand how Linq2SQL data context and TransactionScope works.
				Before running these test cases, you should be better to stop the windows service [Distributed Transaction Coordinator].
				BTW, in SQL2008 and .NET 3.5, multiple connections to a same database in .NET transaction scope won't be esculated to DTC 
					seeing the detail investigation on http://stackoverflow.com/questions/1690892/transactionscope-automatically-escalating-to-msdtc-on-some-machines.")]
	public class Linq2SQLDataContextTests
	{
		[Test]
		[Description("Verify whether the different data contexts can share the same db connection.")]
		public void TwoDataContextSharesSingleConnection()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["Global"].ConnectionString;
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();

			// MembershipDataContext creates an application using an opening connection
			Application application = null;
			string applicationName = "TwoDataContextSharesOpeningConnection";
			using (MembershipDataContext ctx = new MembershipDataContext(connection))
			{
				application = new Application
				{
					ApplicationName = applicationName,
					LoweredApplicationName = applicationName.ToLowerInvariant()
				};

				ctx.Applications.InsertOnSubmit(application);
				ctx.SubmitChanges();
			}

			Assert.AreEqual(ConnectionState.Open, connection.State);

			// ExtensionModelDataContext creates an object metadata using an opening connection
			using (ExtensionModelDataContext ctx = new ExtensionModelDataContext(connection))
			{
				ObjectMetadata objectMetadata = new ObjectMetadata
				{
					ApplicationId = application.ApplicationId,
					Category = "Linq2SQLDataContextTests.Category",
					Name = "Linq2SQLDataContextTests.Name",
					LastUpdatedOn = DateTime.UtcNow
				};

				ctx.ObjectMetadatas.InsertOnSubmit(objectMetadata);
				ctx.SubmitChanges();
			}

			Assert.AreEqual(ConnectionState.Open, connection.State);
			connection.Close();

			connection = new SqlConnection(connectionString);
			// MembershipDataContext removes an application using an closed connection
			using (MembershipDataContext ctx = new MembershipDataContext(connection))
			{
				application = ctx.Applications.FirstOrDefault(app => app.ApplicationName == applicationName);
				Assert.IsNotNull(application);
				ctx.Applications.DeleteOnSubmit(application);
				ctx.SubmitChanges();
			}

			Assert.AreEqual(ConnectionState.Closed, connection.State);

			// ExtensionModelDataContext removes an object metadata using an closed connection
			using (ExtensionModelDataContext ctx = new ExtensionModelDataContext(connection))
			{
				ObjectMetadata objectMetadata = ctx.ObjectMetadatas.FirstOrDefault(metadata => metadata.Name == "Linq2SQLDataContextTests.Name");
				Assert.IsNotNull(objectMetadata);
				ctx.ObjectMetadatas.DeleteOnSubmit(objectMetadata);
				ctx.SubmitChanges();
			}

			Assert.AreEqual(ConnectionState.Closed, connection.State);
		}

		[Test]
		[Description("Verify whether the connection inner of data context is closable before the data context disposed.")]
		public void CloseConnectionBeforeDataContextDisposal()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["Global"].ConnectionString;
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();

			// set an opening connection and close it before the data context disposed
			using (MembershipDataContext ctx = new MembershipDataContext(connection))
			{
				Application application = ctx.Applications.FirstOrDefault();
				Assert.IsNotNull(application);
				Assert.AreEqual(ConnectionState.Open, connection.State);
				connection.Close();
			}

			connection = new SqlConnection(connectionString);
			// set an closed connection and close it before the data context disposed
			using (MembershipDataContext ctx = new MembershipDataContext(connection))
			{
				Application application = ctx.Applications.FirstOrDefault();
				Assert.IsNotNull(application);
				Assert.AreEqual(ConnectionState.Closed, connection.State);
				connection.Close();
			}
		}

		[Test]
		[Description("Verify whether the connection can be opened inner of data context.")]
		public void OpenConnectionInnerOfDataContext()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["Global"].ConnectionString;
			SqlConnection connection = new SqlConnection(connectionString);

			// set an opening connection and close it before the data context disposed
			using (MembershipDataContext ctx = new MembershipDataContext(connection))
			{
				connection.Open();
				Application application = ctx.Applications.FirstOrDefault();
				Assert.IsNotNull(application);
			}

			Assert.AreEqual(ConnectionState.Open, connection.State);
			connection.Close();
		}

		[Test]
		[Ignore]
		[Description("TransactionScope works with both Linq2SQL DataContext and nature ADO.NET command without the windows service DTC in SQL2008 and .NET3.5.")]
		public void TransactionScopeWithoutDTCTest()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["Global"].ConnectionString;
			using (TransactionScope ts = new TransactionScope())
			{
				// creates an application using MembershipDataContext
				Application application = null;
				string applicationName = string.Format(CultureInfo.InvariantCulture, "TransactionScopeWithoutDTCTest.{0}", Guid.NewGuid());
				using (MembershipDataContext ctx = new MembershipDataContext(connectionString))
				{
					application = new Application
					{
						ApplicationName = applicationName,
						LoweredApplicationName = applicationName.ToLowerInvariant()
					};

					ctx.Applications.InsertOnSubmit(application);
					ctx.SubmitChanges();

				}

				// creates an object metadata using ExtensionModelDataContext
				using (ExtensionModelDataContext ctx = new ExtensionModelDataContext(connectionString))
				{
					Assert.AreEqual(ConnectionState.Closed, ctx.Connection.State);
					ObjectMetadata objectMetadata = new ObjectMetadata
					{
						ApplicationId = application.ApplicationId,
						Category = "TransactionScopeWithoutDTCTest.Category",
						Name = "TransactionScopeWithoutDTCTest.Name",
						LastUpdatedOn = DateTime.UtcNow
					};

					ctx.ObjectMetadatas.InsertOnSubmit(objectMetadata);
					ctx.SubmitChanges();
				}

				// deletes above created temporary data using managed ado.net command.
				using (MembershipDataContext membershipDataContext = new MembershipDataContext())
				using (ExtensionModelDataContext extensionModelDataContext = new ExtensionModelDataContext())
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					SqlCommand command = connection.CreateCommand();
					command.CommandText = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE ApplicationId='{1}' AND Name='{2}'",
						extensionModelDataContext.Mapping.GetTable(typeof(ObjectMetadata)).TableName,
						application.ApplicationId,
						"TransactionScopeWithoutDTCTest.Name");
					command.CommandType = CommandType.Text;
					command.ExecuteNonQuery();

					command.CommandText = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE ApplicationId='{1}'",
						membershipDataContext.Mapping.GetTable(typeof(Application)).TableName,
						application.ApplicationId);
					command.ExecuteNonQuery();
				}

				// gets the user "admin" by ASP.NET Membership
				Assert.IsNotNull(System.Web.Security.Membership.GetUser("admin"), "The user admin should exist.");

				ts.Complete();
			}
		}
	}
}