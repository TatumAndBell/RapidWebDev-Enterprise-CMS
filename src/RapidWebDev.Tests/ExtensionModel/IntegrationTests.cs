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
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.ExtensionModel;
using RapidWebDev.ExtensionModel.Linq;
using RapidWebDev.Platform;
using NUnit.Framework;

namespace RapidWebDev.Tests.ExtensionModel
{
	/// <summary>
	/// ExtensionModel的整合测试
	/// </summary>
	[TestFixture]
	public class IntegrationTests
	{
		private Guid objectMetadataTypeId;
		private Guid ObjectMetadataTypeId
		{
			get
			{
				if (objectMetadataTypeId == Guid.Empty)
				{
					IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
					objectMetadataTypeId = metadataApi.AddType("User", "Membership", "User Extension Description", ObjectMetadataTypes.Custom, false, null);
					metadataApi.SaveField(objectMetadataTypeId, new StringFieldMetadata() { Name = "Name", IsRequired = true, MaxLengthSpecified = true, MaxLength = 32, Ordinal = 1 });
					metadataApi.SaveField(objectMetadataTypeId, new DateTimeFieldMetadata() { Name = "Birthday", DefaultValue = new DateTimeValue { DateTimeValueType = DateTimeValueTypes.Now }, Ordinal = 2 });
					metadataApi.SaveField(objectMetadataTypeId, new IntegerFieldMetadata() { Name = "Level", IsRequired = true, DefaultSpecified = true, Default = 10, Ordinal = 3 });
					metadataApi.SaveField(objectMetadataTypeId, new DecimalFieldMetadata() { Name = "Salary", IsRequired = true, MinValueSpecified = true, MinValue = 800m, DefaultSpecified = true, Default = 2500m, Ordinal = 4 });

					HierarchyNode hierarchyNodeQA = new HierarchyNode { Name = "QA", Value = "QA" };
					HierarchyNode hierarchyNodeDev = new HierarchyNode { Name = "Dev", Value = "Dev" };
					HierarchyNode hierarchyNodeLeader = new HierarchyNode { Name = "Leader", Value = "Leader", Node = new HierarchyNode[] { hierarchyNodeQA, hierarchyNodeDev } };
					HierarchyNode hierarchyNodeManager = new HierarchyNode { Name = "Manager", Value = "Manager", Node = new HierarchyNode[] { hierarchyNodeLeader } };
					HierarchyNode hierarchyNodeDirector = new HierarchyNode { Name = "Director", Value = "Director", Node = new HierarchyNode[] { hierarchyNodeManager } };
					HierarchyFieldMetadata hierarchyFieldMetadata = new HierarchyFieldMetadata { Name = "Position", Description = "Position in hierarchy", IsRequired = true, Ordinal = 5, SelectionMode = SelectionModes.Single, Node = new HierarchyNode[] { hierarchyNodeDirector } };
					metadataApi.SaveField(objectMetadataTypeId, hierarchyFieldMetadata);

					SelectionItem selectionItemShangHai = new SelectionItem { Name = "ShangHai", Value = "ShangHai" };
					SelectionItem selectionItemJiangSu = new SelectionItem { Name = "JiangSu", Value = "JiangSu" };
					SelectionItem selectionItemSiChuan = new SelectionItem { Name = "SiChuan", Value = "SiChuan" };
					SelectionItem selectionItemBlank = new SelectionItem { Name = "", Value = "" };
					EnumerationFieldMetadata selectionFieldMetadata = new EnumerationFieldMetadata { Name = "HuKou", Ordinal = 6, SelectionMode = SelectionModes.Single, Items = new SelectionItem[] { selectionItemBlank, selectionItemJiangSu, selectionItemShangHai, selectionItemSiChuan } };
					metadataApi.SaveField(objectMetadataTypeId, selectionFieldMetadata);
				}

				return objectMetadataTypeId;
			}
		}

		[TearDown]
		public void TearDown()
		{
			using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
			{
				IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
				List<ObjectMetadata> objectMetadataToDelete = ctx.ObjectMetadatas.Where(o => o.ApplicationId == authenticationContext.ApplicationId).ToList();
				List<FieldMetadata> fieldMetadataToDelete = ctx.FieldMetadatas.Where(f => objectMetadataToDelete.Select(o => o.ObjectMetadataId).ToArray().Contains(f.ObjectMetadataId)).ToList();
				ctx.FieldMetadatas.DeleteAllOnSubmit(fieldMetadataToDelete);
				ctx.ObjectMetadatas.DeleteAllOnSubmit(objectMetadataToDelete);
				ctx.SubmitChanges();
			}

			using (SampleObjectDataContext ctx = CreateSampleObjectDataContext())
			{
				List<SampleObject> sampleObjectsToDelete = ctx.SampleObjects.ToList();
				ctx.SampleObjects.DeleteAllOnSubmit(sampleObjectsToDelete);
				ctx.SubmitChanges();
			}
		}

		[Test, Description("测试新增扩展模型的对象")]
		public void AddExtensionObjectTest()
		{
			using (SampleObjectDataContext ctx = CreateSampleObjectDataContext())
			{
				SampleObject sampleObject = new SampleObject { Name = "RapidWebDev", ExtensionDataTypeId = this.ObjectMetadataTypeId };
				sampleObject["Name"] = "Eunge";
				sampleObject["Birthday"] = new DateTime(1982, 2, 7);
				sampleObject["Level"] = 10;
				sampleObject["Salary"] = 5000m;
				sampleObject["Position"] = new HierarchyNodeValueCollection { "Dev" };
				sampleObject["HuKou"] = new EnumerationValueCollection { "SiChuan" };

				ctx.SampleObjects.InsertOnSubmit(sampleObject);
				ctx.SubmitChanges();

				sampleObject = ctx.SampleObjects.FirstOrDefault(so => so.ObjectId == sampleObject.ObjectId);
				Assert.AreEqual("Eunge", sampleObject["Name"]);
				Assert.AreEqual(new DateTime(1982, 2, 7), sampleObject["Birthday"]);
				Assert.AreEqual(10, sampleObject["Level"]);
				Assert.AreEqual(5000m, sampleObject["Salary"]);
				Assert.AreEqual("Dev", (sampleObject["Position"] as HierarchyNodeValueCollection)[0]);
				Assert.AreEqual("SiChuan", (sampleObject["HuKou"] as EnumerationValueCollection)[0]);
			}
		}

		[Test, Description("测试更新扩展对象的属性")]
		public void UpdateExtensionObjectTest()
		{
			using (SampleObjectDataContext ctx = CreateSampleObjectDataContext())
			{
				SampleObject sampleObject = new SampleObject { Name = "RapidWebDev", ExtensionDataTypeId = this.ObjectMetadataTypeId };
				sampleObject["Name"] = "Eunge";
				sampleObject["Birthday"] = new DateTime(1982, 2, 7);
				sampleObject["Level"] = 10;
				sampleObject["Salary"] = 5000m;
				sampleObject["Position"] = new HierarchyNodeValueCollection { "Dev" };
				sampleObject["HuKou"] = new EnumerationValueCollection { "SiChuan" };

				ctx.SampleObjects.InsertOnSubmit(sampleObject);
				ctx.SubmitChanges();

				sampleObject["Name"] = "Eunge Liu";
				sampleObject["Level"] = 11;
				ctx.SubmitChanges();

				sampleObject = ctx.SampleObjects.FirstOrDefault(so => so.ObjectId == sampleObject.ObjectId);
				Assert.AreEqual("Eunge Liu", sampleObject["Name"]);
				Assert.AreEqual(11, sampleObject["Level"]);
			}
		}

		[Test, Description("测试删除扩展对象的属性值")]
		public void DeleteExtensionObjectFieldValueTest()
		{
			using (SampleObjectDataContext ctx = CreateSampleObjectDataContext())
			{
				SampleObject sampleObject = new SampleObject { Name = "RapidWebDev", ExtensionDataTypeId = this.ObjectMetadataTypeId };
				sampleObject["Name"] = "Eunge";
				sampleObject["Birthday"] = new DateTime(1982, 2, 7);
				sampleObject["Level"] = 10;
				sampleObject["Salary"] = 5000m;
				sampleObject["Position"] = new HierarchyNodeValueCollection { "Dev" };
				sampleObject["HuKou"] = new EnumerationValueCollection { "SiChuan" };

				ctx.SampleObjects.InsertOnSubmit(sampleObject);
				ctx.SubmitChanges();

				sampleObject["HuKou"] = null;
				ctx.SubmitChanges();

				sampleObject = ctx.SampleObjects.FirstOrDefault(so => so.ObjectId == sampleObject.ObjectId);
				Assert.IsNull(sampleObject["HuKou"]);
			}
		}

		private static SampleObjectDataContext CreateSampleObjectDataContext()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["Global"].ConnectionString;
			return new SampleObjectDataContext(connectionString);
		}
	}
}

