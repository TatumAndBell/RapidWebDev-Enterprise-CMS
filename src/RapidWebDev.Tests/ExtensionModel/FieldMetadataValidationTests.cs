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
using System.IO;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel;
using RapidWebDev.ExtensionModel.Linq;
using RapidWebDev.Platform;

namespace RapidWebDev.Tests.ExtensionModel
{
	/// <summary>
	/// FieldMetadata的验证测试
	/// </summary>
	[TestFixture]
	public class FieldMetadataValidationTests
	{
		[Test(Description = "测试StringFieldMetadata的验证")]
		public void StringFieldMetadataValidationTest()
		{
			IFieldMetadata fieldMetadata = new StringFieldMetadata { Name = "Name", IsRequired = true, MaxLengthSpecified = true, MaxLength = 8, Ordinal = 1 };
			fieldMetadata.Validate(new StringFieldValue { Value = "Eunge" });

			try
			{
				fieldMetadata.Validate(new StringFieldValue { Value = "Eunge Liu" });
				Assert.Fail("'Eunge Liu' is composed of more than 8 characters.");
			}
			catch (InvalidFieldValueException)
			{
			}

			try
			{
				fieldMetadata.Validate(null);
				Assert.Fail("Field value cannot be null");
			}
			catch (InvalidFieldValueException)
			{
			}

			try
			{
				fieldMetadata.Validate(new StringFieldValue());
				Assert.Fail("Field value cannot be null");
			}
			catch (InvalidFieldValueException)
			{
			}
		}

		[Test(Description = "测试DateTimeFieldMetadata的验证")]
		public void DateTimeFieldMetadataValidationTest()
		{
			IFieldMetadata fieldMetadata = new DateTimeFieldMetadata
			{
				Name = "CreatedOn",
				DefaultValue = new DateTimeValue { DateTimeValueType = DateTimeValueTypes.Now },
				IsRequired = true,
				MaxValue = new DateTimeValue { ValueSpecified = true, Value = new DateTime(2049, 12, 31) },
				MinValue = new DateTimeValue { DateTimeValueType = DateTimeValueTypes.Now }
			};

			// 验证默认值
			DateTime defaultValue = (DateTime)fieldMetadata.GetDefaultValue().Value;
			Assert.AreEqual(DateTime.Now.Year, defaultValue.Year);
			Assert.AreEqual(DateTime.Now.Month, defaultValue.Month);
			Assert.AreEqual(DateTime.Now.Day, defaultValue.Day);

			fieldMetadata.Validate(DateTime.Now.AddDays(1).FieldValue());

			try
			{
				fieldMetadata.Validate(DateTime.Now.AddDays(-1).FieldValue());
				Assert.Fail("指定的日期值不能小于当前时间！");
			}
			catch (InvalidFieldValueException) { }

			try
			{
				fieldMetadata.Validate(new DateTime(2050, 1, 1).FieldValue());
				Assert.Fail("指定的日期值大于2050-01-01！");
			}
			catch (InvalidFieldValueException) { }

			try
			{
				fieldMetadata.Validate(null);
				Assert.Fail("日期未指定！");
			}
			catch (InvalidFieldValueException) { }
		}

		[Test(Description = "测试IntegerFieldMetadata的验证")]
		public void IntegerFieldMetadataValidationTest()
		{
			IFieldMetadata fieldMetadata = new IntegerFieldMetadata
			{
				Name = "Score",
				DefaultSpecified = true,
				Default = 10,
				MaxValueSpecified = true,
				MaxValue = 100,
				MinValueSpecified = true,
				MinValue = 0
			};

			// 空值合法
			fieldMetadata.Validate(null);

			fieldMetadata.Validate(60.FieldValue());

			try
			{
				fieldMetadata.Validate((-1).FieldValue());
				Assert.Fail("小于0");
			}
			catch (InvalidFieldValueException) { }

			try
			{
				fieldMetadata.Validate(999.FieldValue());
				Assert.Fail("大于100");
			}
			catch (InvalidFieldValueException) { }
		}

		[Test(Description = "测试DecimalFieldMetadata的验证")]
		public void DecimalFieldMetadataValidationTest()
		{
			IFieldMetadata fieldMetadata = new DecimalFieldMetadata
			{
				Name = "Deposit",
				DefaultSpecified = true,
				Default = 0m,
				MaxValueSpecified = true,
				MaxValue = 1000000m,
				MinValueSpecified = true,
				MinValue = 0
			};

			// 空值合法
			fieldMetadata.Validate(null);

			// 合法值
			fieldMetadata.Validate(10000m.FieldValue());

			try
			{
				fieldMetadata.Validate((-1m).FieldValue());
				Assert.Fail("小于0");
			}
			catch (InvalidFieldValueException) { }

			try
			{
				fieldMetadata.Validate(10000000m.FieldValue());
				Assert.Fail("大于1000000m");
			}
			catch (InvalidFieldValueException) { }
		}

		[Test(Description = "测试HierarchyFieldMetadata的验证")]
		public void HierarchyFieldMetadataValidationTest()
		{
			HierarchyNode hierarchyNodeChengDu = new HierarchyNode { Name = "ChengDu", Value = "ChengDu" };
			HierarchyNode hierarchyNodeShangHai = new HierarchyNode { Name = "ShangHai", Value = "ShangHai" };
			HierarchyNode hierarchyNodeChina = new HierarchyNode { Name = "China", Value = "China", Node = new HierarchyNode[] { hierarchyNodeChengDu, hierarchyNodeShangHai } };
			HierarchyNode hierarchyNodeAsia = new HierarchyNode { Name = "Asia", Value = "Asia", Node = new HierarchyNode[] { hierarchyNodeChina } };
			HierarchyNode hierarchyNodeEarth = new HierarchyNode { Name = "Earth", Value = "Earth", Node = new HierarchyNode[] { hierarchyNodeAsia } };
			HierarchyFieldMetadata fieldMetadata = new HierarchyFieldMetadata
			{
				Name = "Geoghraph",
				SelectionMode = SelectionModes.Single,
				Node = new HierarchyNode[] { hierarchyNodeEarth }
			};

			// 空值合法
			fieldMetadata.Validate(null);

			// 合法值
			fieldMetadata.Validate(new HierarchyNodeValueCollection { "ChengDu" }.FieldValue());

			try
			{
				fieldMetadata.Validate(new HierarchyNodeValueCollection { "HangZhou" }.FieldValue());
				Assert.Fail("HangZhou没有在Hierarchy中定义");
			}
			catch (InvalidFieldValueException) { }

			try
			{
				fieldMetadata.Validate(new HierarchyNodeValueCollection { "ChengDu", "ShangHai" }.FieldValue());
				Assert.Fail("当前Hierarchy只能设定单值");
			}
			catch (InvalidFieldValueException) { }
		}

		[Test(Description = "测试SelectionFieldMetadata的验证")]
		public void SelectionFieldMetadataValidationTest()
		{
			SelectionItem selectionItemShangHai = new SelectionItem { Name = "ShangHai", Value = "ShangHai" };
			SelectionItem selectionItemChengDu = new SelectionItem { Name = "ChengDu", Value = "ChengDu" };
			SelectionItem selectionItemHangZhou = new SelectionItem { Name = "HangZhou", Value = "HangZhou" };
			SelectionItem selectionItemBlank = new SelectionItem { Name = "", Value = "" };
			IFieldMetadata fieldMetadata = new EnumerationFieldMetadata
			{
				Name = "City",
				SelectionMode = SelectionModes.Single,
				Items = new SelectionItem[] { selectionItemBlank, selectionItemChengDu, selectionItemShangHai, selectionItemHangZhou }
			};

			// 空值合法
			fieldMetadata.Validate(null);

			// 合法值
			fieldMetadata.Validate(new EnumerationValueCollection { "ChengDu" }.FieldValue());

			try
			{
				fieldMetadata.Validate(new EnumerationValueCollection { "SuZhou" }.FieldValue());
				Assert.Fail("SuZhou没有在Hierarchy中定义");
			}
			catch (InvalidFieldValueException) { }

			try
			{
				fieldMetadata.Validate(new EnumerationValueCollection { "ChengDu", "ShangHai" }.FieldValue());
				Assert.Fail("当前Selection只能选择单值");
			}
			catch (InvalidFieldValueException) { }
		}
	}
}
