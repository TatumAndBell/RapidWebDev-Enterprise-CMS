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
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel.Linq;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;

namespace RapidWebDev.Tests.Platform
{
	[TestFixture]
	public class ApplicationApiTests
	{
		[Test, Description("Basic application test")]
		public void BasicTest()
		{
			string applicationName = string.Format("application-{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
			string description = applicationName + "-desc";
			ApplicationObject applicationObject = new ApplicationObject
			{
				Name = applicationName,
				Description = description
			};

			IApplicationApi applicationApi = SpringContext.Current.GetObject<IApplicationApi>();
			applicationApi.Save(applicationObject);

			Assert.IsTrue(applicationApi.Exists(applicationName), "The application should exist");

			applicationObject = applicationApi.Get(applicationName);
			Assert.AreEqual(applicationName, applicationObject.Name, "The application name should be the same as created.");
			Assert.AreEqual(description, applicationObject.Description, "The application description should be the same as created.");

			applicationObject = applicationApi.Get(applicationObject.Id);
			Assert.AreEqual(applicationName, applicationObject.Name, "The application name should be the same as created.");
			Assert.AreEqual(description, applicationObject.Description, "The application description should be the same as created.");

			// update application description
			description = "modified description";
			applicationObject.Description = description;
			applicationApi.Save(applicationObject);

			// re-check application properties
			applicationObject = applicationApi.Get(applicationName);
			Assert.AreEqual(applicationName, applicationObject.Name);

			applicationObject = applicationApi.Get(applicationObject.Id);
			Assert.AreEqual(applicationName, applicationObject.Name);

			// update application name
			string modifiedApplicationName = string.Format("App-{0}", Guid.NewGuid());
			applicationObject.Name = modifiedApplicationName;
			applicationApi.Save(applicationObject);

			Assert.IsFalse(applicationApi.Exists(applicationName), "The original application name should not exist.");
			Assert.IsNull(applicationApi.Get(applicationName), "The original application name should not exist.");
			Assert.IsTrue(applicationApi.Exists(modifiedApplicationName), "The modified application name should exist.");
		}
	}
}

