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
	public class HierarchyApiTests
	{
		private const string GEOGRAPHY = "Geography";
		private static IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
		private static IHierarchyApi hierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();
		private List<Guid> temporaryHierarchyDataIds = new List<Guid>();
		private Guid metadataTypeId;

		[TearDown]
		public void TearDown()
		{
			foreach (Guid temporaryHierarchyDataId in this.temporaryHierarchyDataIds)
				hierarchyApi.HardDeleteHierarchyData(temporaryHierarchyDataId);

			metadataApi.DeleteType(metadataTypeId);
		}

		[Test, Description("Simple test hierarchy data create and query function.")]
		public void SimpleHierarchyDataTest()
		{
			HierarchyDataObject china = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "China" };
			hierarchyApi.Save(china);

			HierarchyDataObject shanghai = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Shanghai", ParentHierarchyDataId = china.HierarchyDataId };
			hierarchyApi.Save(shanghai);

			HierarchyDataObject beijing = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Beijing", ParentHierarchyDataId = china.HierarchyDataId };
			hierarchyApi.Save(beijing);

			this.temporaryHierarchyDataIds.AddRange(new[] { china.HierarchyDataId, shanghai.HierarchyDataId, beijing.HierarchyDataId });

			// get all areas
			IEnumerable<HierarchyDataObject> areas = hierarchyApi.GetAllChildren(GEOGRAPHY, null);
			Assert.AreEqual(3, areas.Count());

			// get both implicit and explicit child areas of China
			areas = hierarchyApi.GetAllChildren(GEOGRAPHY, china.HierarchyDataId);
			Assert.AreEqual(2, areas.Count());

			// get only explicit child areas which is the root node in geography.
			areas = hierarchyApi.GetImmediateChildren(GEOGRAPHY, null);
			Assert.AreEqual(1, areas.Count());
			Assert.AreEqual("China", areas.First().Name);

			// query hierarchy data by name.
			int recordCount;
			LinqPredicate predicate = new LinqPredicate("Name=@0 AND ParentHierarchyDataId=@1 AND HierarchyType=@2", "Beijing", china.HierarchyDataId, GEOGRAPHY);
			hierarchyApi.FindHierarchyData(predicate, null, 0, 10, out recordCount);
			Assert.AreEqual(1, recordCount);
		}

		[Test, Description("Simple test hierarchy data create and query function.")]
		public void ComplexHierarchyDataTest()
		{
			// define the metadata of geography.
			metadataTypeId = metadataApi.AddType("Geography", "HierarchyData", null, ObjectMetadataTypes.Custom, false, null);
			metadataApi.SaveField(metadataTypeId, new StringFieldMetadata { Name = "Chief Executive" });
			metadataApi.SaveField(metadataTypeId, new IntegerFieldMetadata { Name = "Population", MinValue = 0 });

			// create geographic areas with extension properties
			HierarchyDataObject china = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "China", ExtensionDataTypeId = metadataTypeId };
			china["Chief Executive"] = "XXX";
			china["Population"] = 1300000000;
			hierarchyApi.Save(china);

			HierarchyDataObject shanghai = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Shanghai", ParentHierarchyDataId = china.HierarchyDataId, ExtensionDataTypeId = metadataTypeId };
			shanghai["Chief Executive"] = "YYY";
			shanghai["Population"] = 20000000;
			hierarchyApi.Save(shanghai);

			HierarchyDataObject beijing = new HierarchyDataObject { HierarchyType = GEOGRAPHY, Name = "Beijing", ParentHierarchyDataId = china.HierarchyDataId, ExtensionDataTypeId = metadataTypeId };
			beijing["Chief Executive"] = "ZZZ";
			beijing["Population"] = 30000000;
			hierarchyApi.Save(beijing);

			this.temporaryHierarchyDataIds.AddRange(new[] { china.HierarchyDataId, shanghai.HierarchyDataId, beijing.HierarchyDataId });

			// query hierarchy data by name.
			int recordCount;
			LinqPredicate predicate = new LinqPredicate("Name=@0 AND HierarchyType=@1", "Shanghai", GEOGRAPHY);
			IEnumerable<HierarchyDataObject> geographicAreas = hierarchyApi.FindHierarchyData(predicate, null, 0, 10, out recordCount);
			shanghai = geographicAreas.First();
			Assert.AreEqual(1, recordCount);
			Assert.AreEqual("YYY", shanghai["Chief Executive"]);
			Assert.AreEqual(20000000, shanghai["Population"]);
		}
	}
}