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
using System.Xml;
using System.Xml.Linq;
using RapidWebDev.Common;
using RapidWebDev.Common.Data;
using RapidWebDev.ExtensionModel;
using RapidWebDev.ExtensionModel.Linq;
using RapidWebDev.Platform;
using NUnit.Framework;

namespace RapidWebDev.Tests.ExtensionModel
{
	/// <summary>
	/// 测试ExtensionObject的序列化
	/// </summary>
	[TestFixture]
	public class ExtensionObjectSerializerTests
	{
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
		}

		[Test(Description = "测试各个类型的字段的序列化")]
		public void AllFieldTypesSerializationTest()
		{
			MockExtensionObject4Serializer entityToSerialize = new MockExtensionObject4Serializer();
			entityToSerialize["Name"] = "Eunge";
			entityToSerialize["Birthday"] = new DateTime(1982, 2, 7);
			entityToSerialize["Level"] = 10;
			entityToSerialize["Salary"] = 13000m;
			entityToSerialize["Position"] = new HierarchyNodeValueCollection { "Dev" };
			entityToSerialize["HuKou"] = new EnumerationValueCollection { "SiChuan" };
			entityToSerialize["UnknownProperty"] = "Lucy";
			entityToSerialize.ExtensionDataTypeId = this.UserTypeId;

			IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
			ExtensionObjectSerializer serializer = new ExtensionObjectSerializer(metadataApi);	
			serializer.Serialize(entityToSerialize);

			using(StringReader stringReader = new StringReader(entityToSerialize.ExtensionData))
			using (XmlReader xmlreader = XmlReader.Create(stringReader))
			{
				XDocument xmldoc = XDocument.Load(xmlreader);
				IEnumerable<XElement> fieldNameValuePairs = xmldoc.Descendants(XName.Get("FieldNameValuePair", ServiceNamespaces.ExtensionModel));
				Assert.AreEqual(7, fieldNameValuePairs.Count());
			}

			MockExtensionObject4Serializer entityToDeserialize = new MockExtensionObject4Serializer()
			{
				ExtensionData = entityToSerialize.ExtensionData,
				ExtensionDataTypeId = entityToSerialize.ExtensionDataTypeId
			};

			IDictionary<string, object> properties = serializer.Deserialize(entityToDeserialize);
			Assert.AreEqual("Eunge", properties["Name"]);
			Assert.AreEqual(new DateTime(1982, 2, 7), properties["Birthday"]);
			Assert.AreEqual(10, properties["Level"]);
			Assert.AreEqual(13000m, properties["Salary"]);
			Assert.AreEqual("Dev", (properties["Position"] as HierarchyNodeValueCollection)[0]);
			Assert.AreEqual("SiChuan", (properties["HuKou"] as EnumerationValueCollection)[0]);
			Assert.AreEqual("Lucy", properties["UnknownProperty"]);
		}

		private Guid userTypeId;
		private Guid UserTypeId
		{
			get
			{
				if (userTypeId == Guid.Empty)
				{
					IMetadataApi metadataApi = SpringContext.Current.GetObject<IMetadataApi>();
					userTypeId = metadataApi.AddType("User", "Membership", "User Extension Description", ObjectMetadataTypes.Custom, false, null);
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
					EnumerationFieldMetadata selectionFieldMetadata = new EnumerationFieldMetadata { Name = "HuKou", Ordinal = 6, SelectionMode = SelectionModes.Single, Items = new SelectionItem[] { selectionItemBlank, selectionItemJiangSu, selectionItemShangHai, selectionItemSiChuan } };
					metadataApi.SaveField(userTypeId, selectionFieldMetadata);
				}

				return userTypeId;
			}
		}
	}
}
