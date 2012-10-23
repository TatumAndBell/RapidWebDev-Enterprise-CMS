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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.Common.Validation;
using RapidWebDev.ExtensionModel;
using RapidWebDev.ExtensionModel.Linq;
using RapidWebDev.Platform;
using RapidWebDev.Platform.Linq;
using NUnit.Framework;

namespace RapidWebDev.Tests.ExtensionModel
{
	/// <summary>
	/// MetadataAPI的测试
	/// </summary>
	[TestFixture]
	public class MetadataApiTests
	{
		private Collection<Guid> addedGlobalObjectMetadataTypeIds = new Collection<Guid>();

		[TearDown]
		public void TearDown()
		{
			using (ExtensionModelDataContext ctx = DataContextFactory.Create<ExtensionModelDataContext>())
			{
				IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
				List<ObjectMetadata> objectMetadataToDelete = ctx.ObjectMetadatas.Where(o => o.ApplicationId == authenticationContext.ApplicationId || addedGlobalObjectMetadataTypeIds.ToArray().Contains(o.ObjectMetadataId)).ToList();
				List<FieldMetadata> fieldMetadataToDelete = ctx.FieldMetadatas.Where(f => objectMetadataToDelete.Select(o => o.ObjectMetadataId).ToArray().Contains(f.ObjectMetadataId)).ToList();
				ctx.FieldMetadatas.DeleteAllOnSubmit(fieldMetadataToDelete);
				ctx.ObjectMetadatas.DeleteAllOnSubmit(objectMetadataToDelete);
				ctx.SubmitChanges();
			}
		}

		[Test(Description = "测试扩展类型的CRUD")]
		public void CRUDObjectMetadataTypeTest()
		{
			IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
			Guid baseTypeId = metadataApi.AddType("BaseType", "Organization", "BaseType.Desc", ObjectMetadataTypes.Custom, true, null);
			addedGlobalObjectMetadataTypeIds.Add(baseTypeId);

			Guid localTypeId = metadataApi.AddType("LocalType", "Organization", "LocalType.Desc", ObjectMetadataTypes.Custom, false, baseTypeId);

			IObjectMetadata objectMetadata = metadataApi.GetType(baseTypeId);
			Assert.AreEqual("BaseType", objectMetadata.Name);
			Assert.AreEqual("Organization", objectMetadata.Category);
			Assert.AreEqual("BaseType.Desc", objectMetadata.Description);
			Assert.IsFalse(objectMetadata.ParentObjectMetadataId.HasValue);

			objectMetadata = metadataApi.GetType(localTypeId);
			Assert.AreEqual("LocalType", objectMetadata.Name);
			Assert.AreEqual("Organization", objectMetadata.Category);
			Assert.AreEqual("LocalType.Desc", objectMetadata.Description);
			Assert.AreEqual(baseTypeId, objectMetadata.ParentObjectMetadataId.Value);

			metadataApi.UpdateType(baseTypeId, "BaseType", "Common", "Common Extension Type", null);
			objectMetadata = metadataApi.GetType(baseTypeId);
			Assert.AreEqual("BaseType", objectMetadata.Name);
			Assert.AreEqual("Common", objectMetadata.Category);
			Assert.AreEqual("Common Extension Type", objectMetadata.Description);

			metadataApi.UpdateType(localTypeId, "User", "Membership", "User Extension Type", baseTypeId);
			objectMetadata = metadataApi.GetType(localTypeId);
			Assert.AreEqual("User", objectMetadata.Name);
			Assert.AreEqual("Membership", objectMetadata.Category);
			Assert.AreEqual("User Extension Type", objectMetadata.Description);

			metadataApi.DeleteType(localTypeId);
			metadataApi.DeleteType(baseTypeId);

			Assert.IsNull(metadataApi.GetType(baseTypeId));
			Assert.IsNull(metadataApi.GetType(localTypeId));
		}

		[Test(Description = "测试各个类型的扩展属性的CRUD，该扩展类型没有继承于其它的类型")]
		public void AllFieldMetadataOfIndividualObjectMetadataTest()
		{
			IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
			Guid userTypeId = metadataApi.AddType("User", "Membership", "User Extension Description", ObjectMetadataTypes.Custom, false, null);
			metadataApi.SaveField(userTypeId, new StringFieldMetadata() { Name = "Name", IsRequired = true, MaxLengthSpecified = true, MaxLength = 32, Ordinal = 1 });
			metadataApi.SaveField(userTypeId, new DateTimeFieldMetadata() { Name = "Birthday", DefaultValue = new DateTimeValue { DateTimeValueType = DateTimeValueTypes.Now }, Ordinal = 2 });
			metadataApi.SaveField(userTypeId, new IntegerFieldMetadata() { Name = "Level", IsRequired = true, DefaultSpecified = true, Default = 10, Ordinal = 3 });
			metadataApi.SaveField(userTypeId, new DecimalFieldMetadata() { Name = "Salary", IsRequired = true, MinValueSpecified = true, MinValue = 800m, DefaultSpecified = true, Default = 2500m, Ordinal = 4 });

			HierarchyNode hierarchyNodeQA = new HierarchyNode { Name = "QA", Value = "QA" };
			HierarchyNode hierarchyNodeDev = new HierarchyNode { Name = "Dev", Value = "Dev" };
			HierarchyNode hierarchyNodeLeader = new HierarchyNode { Name = "Leader", Value = "Leader", Node = new HierarchyNode[] { hierarchyNodeQA, hierarchyNodeDev } };
			HierarchyNode hierarchyNodeManager = new HierarchyNode { Name = "Manager", Value = "Manager", Node = new HierarchyNode[] { hierarchyNodeLeader } };
			HierarchyNode hierarchyNodeDirector = new HierarchyNode { Name = "Director", Value = "Director", Node = new HierarchyNode[] { hierarchyNodeManager } };
			HierarchyFieldMetadata hierarchyFieldMetadata = new HierarchyFieldMetadata { Name = "Position", Description = "Position in hierarchy", IsRequired = true, Ordinal = 5, SelectionMode = SelectionModes.Single, Node = new HierarchyNode[] { hierarchyNodeDirector } };
			metadataApi.SaveField(userTypeId, hierarchyFieldMetadata);

			SelectionItem selectionItemShangHai = new SelectionItem { Name = "ShangHai", Value = "ShangHai" };
			SelectionItem selectionItemJiangSu = new SelectionItem { Name = "JiangSu", Value = "JiangSu" };
			SelectionItem selectionItemSiChuan = new SelectionItem { Name = "SiChuan", Value = "SiChuan" };
			SelectionItem selectionItemBlank = new SelectionItem { Name = "", Value = "" };
			EnumerationFieldMetadata enumerationFieldMetadata = new EnumerationFieldMetadata { Name = "HuKou", Ordinal = 6, SelectionMode = SelectionModes.Single, Items = new SelectionItem[] { selectionItemBlank, selectionItemJiangSu, selectionItemShangHai, selectionItemSiChuan } };
			metadataApi.SaveField(userTypeId, enumerationFieldMetadata);

			// 测试获取各个属性
			AssertFieldMetadata(metadataApi, userTypeId);

			// 测试删除一个属性
			metadataApi.DeleteField(userTypeId, "HuKou");
			Assert.AreEqual(5, metadataApi.GetFields(userTypeId).Count());

			// 测试更新一个属性的默认值
			IntegerFieldMetadata fieldMetadataLevel = metadataApi.GetField(userTypeId, "Level") as IntegerFieldMetadata;
			fieldMetadataLevel.Default = 9;
			metadataApi.SaveField(userTypeId, fieldMetadataLevel);
			fieldMetadataLevel = metadataApi.GetField(userTypeId, "Level") as IntegerFieldMetadata;
			Assert.AreEqual(9, fieldMetadataLevel.Default);

			// 删除扩展类型，会自动删除其所有的属性定义
			metadataApi.DeleteType(userTypeId);
			Assert.AreEqual(0, metadataApi.GetFields(userTypeId).Count());
		}

		[Test(Description = "测试扩展类型的继承关系，以及关系变化对各扩展类型的影响")]
		public void ObjectInheritenaceTest()
		{
			IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
			Guid commonTypeId = metadataApi.AddType("MembershipBase", "Membership", "Membership Base Extension Description", ObjectMetadataTypes.Custom, true, null);
			addedGlobalObjectMetadataTypeIds.Add(commonTypeId);
			metadataApi.SaveField(commonTypeId, new StringFieldMetadata() { Name = "Name", IsRequired = true, MaxLengthSpecified = true, MaxLength = 32, Ordinal = 1 });
			metadataApi.SaveField(commonTypeId, new StringFieldMetadata() { Name = "Description", MaxLengthSpecified = true, MaxLength = 256, Ordinal = 2 });

			Guid userTypeId = metadataApi.AddType("Users", "Membership", "User Extension Type Description", ObjectMetadataTypes.Custom, false, commonTypeId);
			IEnumerable<IFieldMetadata> userFields = metadataApi.GetFields(userTypeId);
			Assert.AreEqual(2, userFields.Count());
			Assert.IsNotNull(userFields.FirstOrDefault(f => f.Name == "Name"));
			Assert.IsNotNull(userFields.FirstOrDefault(f => f.Name == "Description"));

			metadataApi.SaveField(userTypeId, new StringFieldMetadata() { Name = "Name", IsRequired = true, MaxLengthSpecified = true, MaxLength = 256, Ordinal = 1 });
			metadataApi.SaveField(userTypeId, new IntegerFieldMetadata() { Name = "Age", Ordinal = 2 });
			userFields = metadataApi.GetFields(userTypeId);
			Assert.AreEqual(3, userFields.Count());

			StringFieldMetadata nameField = userFields.FirstOrDefault(f => f.Name == "Name") as StringFieldMetadata;
			Assert.IsTrue(nameField.MaxLengthSpecified);
			Assert.AreEqual(256, nameField.MaxLength);

			metadataApi.SaveField(commonTypeId, new StringFieldMetadata() { Name = "EmployeeNo", MaxLengthSpecified = true, MaxLength = 8, Ordinal = 3 });
			userFields = metadataApi.GetFields(userTypeId);
			Assert.AreEqual(4, userFields.Count());

			metadataApi.DeleteField(commonTypeId, "Description");
			userFields = metadataApi.GetFields(userTypeId);
			Assert.AreEqual(3, userFields.Count());
		}

		[Test(Description = "被子类型继承的类型不能被删除")]
		[ExpectedException(typeof(ValidationException))]
		public void ParentTypeCannotBeDeletedTest()
		{
			IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
			Guid commonTypeId = metadataApi.AddType("MembershipBase", "Membership", "Membership Base Extension Description", ObjectMetadataTypes.Custom, true, null);
			addedGlobalObjectMetadataTypeIds.Add(commonTypeId);
			metadataApi.SaveField(commonTypeId, new StringFieldMetadata() { Name = "Name", IsRequired = true, MaxLengthSpecified = true, MaxLength = 32, Ordinal = 1 });

			Guid userTypeId = metadataApi.AddType("Users", "Membership", "User Extension Type Description", ObjectMetadataTypes.Custom, false, commonTypeId);

			metadataApi.DeleteType(commonTypeId);
		}

		private static void AssertFieldMetadata(IMetadataApi metadataApi, Guid objectMetadataId)
		{
			IFieldMetadata fieldMetadata = metadataApi.GetField(objectMetadataId, "Name");
			Assert.AreEqual("Name", fieldMetadata.Name);
			Assert.AreEqual(FieldType.String, fieldMetadata.Type);
			Assert.AreEqual(1, fieldMetadata.Ordinal);
			Assert.IsTrue(fieldMetadata.IsRequired);

			fieldMetadata = metadataApi.GetField(objectMetadataId, "Birthday");
			Assert.AreEqual("Birthday", fieldMetadata.Name);
			Assert.AreEqual(FieldType.DateTime, fieldMetadata.Type);
			Assert.AreEqual(2, fieldMetadata.Ordinal);
			Assert.IsFalse(fieldMetadata.IsRequired);

			fieldMetadata = metadataApi.GetField(objectMetadataId, "Level");
			Assert.AreEqual("Level", fieldMetadata.Name);
			Assert.AreEqual(FieldType.Integer, fieldMetadata.Type);
			Assert.AreEqual(3, fieldMetadata.Ordinal);
			Assert.IsTrue(fieldMetadata.IsRequired);

			fieldMetadata = metadataApi.GetField(objectMetadataId, "Salary");
			Assert.AreEqual("Salary", fieldMetadata.Name);
			Assert.AreEqual(FieldType.Decimal, fieldMetadata.Type);
			Assert.AreEqual(4, fieldMetadata.Ordinal);
			Assert.IsTrue(fieldMetadata.IsRequired);

			fieldMetadata = metadataApi.GetField(objectMetadataId, "Position");
			Assert.AreEqual("Position", fieldMetadata.Name);
			Assert.AreEqual(FieldType.Hierarchy, fieldMetadata.Type);
			Assert.AreEqual(5, fieldMetadata.Ordinal);
			Assert.IsTrue(fieldMetadata.IsRequired);

			HierarchyFieldMetadata hierarchyFieldMetadata = fieldMetadata as HierarchyFieldMetadata;
			Assert.AreEqual(1, hierarchyFieldMetadata.Node.Length);
			Assert.AreEqual("Director", hierarchyFieldMetadata.Node[0].Name);

			fieldMetadata = metadataApi.GetField(objectMetadataId, "HuKou");
			Assert.AreEqual("HuKou", fieldMetadata.Name);
			Assert.AreEqual(FieldType.Enumeration, fieldMetadata.Type);
			Assert.AreEqual(6, fieldMetadata.Ordinal);
			Assert.IsFalse(fieldMetadata.IsRequired);

			IEnumerable<IFieldMetadata> allFieldMetadata = metadataApi.GetFields(objectMetadataId);
			Assert.AreEqual(6, allFieldMetadata.Count());
		}
	}
}
