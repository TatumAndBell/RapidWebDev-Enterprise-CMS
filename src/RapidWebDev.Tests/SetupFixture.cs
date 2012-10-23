/// Copyright (c) 2008 by eunge.liu@gmail.com. All Rights Reserved.
// Information Contained Herein is Proprietary	and Confidential.
// $Id: SetupFixture.cs 385 2009-11-11 10:39:23Z eungeliu $

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Initialization;
using RapidWebDev.Platform.Linq;
using RapidWebDev.UI;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages;
using NUnit.Framework;
using AspNetMembership = System.Web.Security.Membership;

namespace RapidWebDev.Tests
{
	[SetUpFixture]
	public class SetupFixture
	{
		[SetUp]
		public virtual void GlobalSetup()
		{
			Console.Out.WriteLine("************Global Setup Started! ************\r\n");

			SpringContext.Current.GetObject<IInstallerManager>().Install(AspNetMembership.ApplicationName);

			Console.Out.WriteLine("\r\n************Global Setup Done! ************\r\n");
		}

		[TearDown]
		public virtual void GlobalTearDown()
		{
			// TODO:
		}
	}
}