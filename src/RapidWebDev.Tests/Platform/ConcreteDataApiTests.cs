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
using System.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using RapidWebDev.ExtensionModel;

namespace RapidWebDev.Tests.Platform
{
	[TestFixture]
	public class ConcreteDataApiTests
	{
		private static IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
		private static IConcreteDataApi concreteDataApi = SpringContext.Current.GetObject<IConcreteDataApi>();

		private Guid metadataTypeId;

		[TearDown]
		public void TearDown()
		{
			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
				ctx.ConcreteDatas.Delete(s => s.ApplicationId == authenticationContext.ApplicationId);
				ctx.SubmitChanges();
			}

			metadataApi.DeleteType(metadataTypeId);
		}

		[Test, Description("Simple test concrete data create and query function.")]
		public void ConcreteDataSimpleTest()
		{
			const string DEGREE = "Degree";
			ConcreteDataObject master = new ConcreteDataObject { Type = DEGREE, Name = "Master" };
			ConcreteDataObject regularCollege = new ConcreteDataObject { Type = DEGREE, Name = "Regular College" };
			ConcreteDataObject juniorCollege = new ConcreteDataObject { Type = DEGREE, Name = "Junior College" };

			concreteDataApi.Save(master);
			concreteDataApi.Save(regularCollege);
			concreteDataApi.Save(juniorCollege);

			IEnumerable<ConcreteDataObject> degrees = concreteDataApi.FindAllByType(DEGREE);
			Assert.AreEqual(3, degrees.Count());

			int recordCount;
			LinqPredicate predicate = new LinqPredicate("Name.EndsWith(@0) AND Type=@1", "College", DEGREE);
			degrees = concreteDataApi.FindConcreteData(predicate, null, 0, 10, out recordCount);
			Assert.AreEqual(2, recordCount);
		}

		[Test, Description("Create and query concrete data in two types and ensure they're not conflicting.")]
		public void ConcreteDataWithinDifferentTypesTest()
		{
			const string DEGREE = "Degree";
			const string CITY = "City";

			ConcreteDataObject master = new ConcreteDataObject { Type = DEGREE, Name = "Master" };
			ConcreteDataObject regularCollege = new ConcreteDataObject { Type = DEGREE, Name = "Regular College" };
			ConcreteDataObject juniorCollege = new ConcreteDataObject { Type = DEGREE, Name = "Junior College" };

			ConcreteDataObject shanghai = new ConcreteDataObject { Type = CITY, Name = "Shanghai" };
			ConcreteDataObject beijing = new ConcreteDataObject { Type = CITY, Name = "Beijing" };

			concreteDataApi.Save(master);
			concreteDataApi.Save(regularCollege);
			concreteDataApi.Save(juniorCollege);

			concreteDataApi.Save(shanghai);
			concreteDataApi.Save(beijing);

			IEnumerable<ConcreteDataObject> degrees = concreteDataApi.FindAllByType(DEGREE);
			Assert.AreEqual(3, degrees.Count());

			IEnumerable<ConcreteDataObject> cities = concreteDataApi.FindAllByType(CITY);
			Assert.AreEqual(2, cities.Count());

			int recordCount;
			LinqPredicate predicate = new LinqPredicate("Name.EndsWith(@0) AND Type=@1", "College", DEGREE);
			degrees = concreteDataApi.FindConcreteData(predicate, null, 0, 10, out recordCount);
			Assert.AreEqual(2, recordCount);

			predicate = new LinqPredicate("Name=@0 AND Type=@1", "Shanghai", CITY);
			degrees = concreteDataApi.FindConcreteData(predicate, null, 0, 10, out recordCount);
			Assert.AreEqual(1, recordCount);
		}

		[Test, Description("Create complex concrete data with Value and custom properties.")]
		public void ComplexConcreteDataTest()
		{
			metadataTypeId = metadataApi.AddType("OpenSourceLicense", "ConcreteData", null, ObjectMetadataTypes.Custom, false, null);
			metadataApi.SaveField(metadataTypeId, new StringFieldMetadata { Name = "Copyright" });
			metadataApi.SaveField(metadataTypeId, new DateTimeFieldMetadata { Name = "CreatedOn" });

			const string LICENSE = "License";
			string GPLv2Description = "The licenses for most software are designed to take away your freedom to share and change it. By contrast, the GNU General Public License is intended to guarantee your freedom to share and change free software--to make sure the software is free for all its users. This General Public License applies to most of the Free Software Foundation's software and to any other program whose authors commit to using it. (Some other Free Software Foundation software is covered by the GNU Library General Public License instead.) You can apply it to your programs, too.";
			ConcreteDataObject GPLv2 = new ConcreteDataObject 
			{ 
				Type = LICENSE, 
				Name = "GNU General Public License version 2", 
				Value = "GPLv2", 
				ExtensionDataTypeId = metadataTypeId,
				Description = GPLv2Description
			};

			GPLv2["Copyright"] = "Free Software Foundation, Inc";
			GPLv2["CreatedOn"] = new DateTime(1989, 1, 1);
			concreteDataApi.Save(GPLv2);

			ConcreteDataObject Apache = new ConcreteDataObject
			{
				Type = LICENSE,
				Name = "Apache License",
				Value = "Apache",
				ExtensionDataTypeId = metadataTypeId,
				Description = "Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable copyright license to reproduce, prepare Derivative Works of, publicly display, publicly perform, sublicense, and distribute the Work and such Derivative Works in Source or Object form."
			};

			Apache["Copyright"] = "Apache License";
			Apache["CreatedOn"] = new DateTime(2004, 1, 1);
			concreteDataApi.Save(Apache);

			IEnumerable<ConcreteDataObject> licenses = concreteDataApi.FindAllByType(LICENSE);
			Assert.AreEqual(2, licenses.Count());

			int recordCount;
			LinqPredicate predicate = new LinqPredicate("Value=@0 AND TYPE=@1", "GPLv2", LICENSE);
			licenses = concreteDataApi.FindConcreteData(predicate, null, 0, 10, out recordCount);

			Assert.AreEqual(1, recordCount);
			GPLv2 = licenses.First();
			Assert.AreEqual("GNU General Public License version 2", GPLv2.Name);
			Assert.AreEqual("GPLv2", GPLv2.Value);
			Assert.AreEqual(GPLv2Description, GPLv2.Description);
			Assert.AreEqual("Free Software Foundation, Inc", GPLv2["Copyright"]);
			Assert.AreEqual(new DateTime(1989, 1, 1), GPLv2["CreatedOn"]);
		}
	}
}