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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using RapidWebDev.Common.CodeDom;
using RapidWebDev.UI;
using NUnit.Framework;
using Spring.Objects;

namespace RapidWebDev.Tests.Common
{
	[TestFixture(Description = "Test cases for code dom functionalities in Common assembly.")]
	public class CodeDomTests
	{
		[Test(Description = "Test cases on creating decorate type dynamically for anonymous class instances.")]
		public void CreateDecorateTypeTest()
		{
			var anonymousObject = new { Name = "Eunge Liu", Career = "Software Development", Birthday = new DateTime(1982, 2, 7) };
			Type anonymousType = anonymousObject.GetType();

			IEnumerable<PropertyDefinition> propertyDecorateConfigs = new PropertyDefinition[]
			{
				new PropertyDefinition("Name") { NewPropertyName = "Name" },
				new PropertyDefinition("Career") { NewPropertyName = "Career" },
				new PropertyDefinition("Birthday") { NewPropertyName = "Birthday" },
			};

			Type decorateType = ClassDecorator.CreateDecorateType(anonymousType.Name, propertyDecorateConfigs);
			Assert.IsNotNull(decorateType);

			PropertyInfo[] propertyInfoArray = decorateType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			Assert.AreEqual(3, propertyInfoArray.Length, "There should be three properties generated in decorate class.");
		}

		[Test(Description = "Test cases on creating decorate objects dynamically for anonymous class instances without converting property types.")]
		public void CreateDecorateObjectsWithoutConvertPropertyTypesTest()
		{
			var anonymousObjects = new[] 
			{
				new { Name = "Eunge Liu", Career = "Software Development", Birthday = new DateTime(1982, 2, 7) },
				new { Name = "Lucy Liu", Career = "Financial", Birthday = new DateTime(1982, 5, 1) }
			};

			IEnumerable<PropertyDefinition> propertyDecorateConfigs = new PropertyDefinition[]
			{
				new PropertyDefinition(typeof(string)) { PropertyName = "Name", NewPropertyName = "Name" },
				new PropertyDefinition(typeof(string)) { PropertyName = "Career", NewPropertyName = "Career" },
				new PropertyDefinition(typeof(DateTime)) { PropertyName = "Birthday", NewPropertyName = "Birthday" },
			};

			IEnumerable<object> returnObjects = ClassDecorator.CreateDecorateObjects(anonymousObjects, propertyDecorateConfigs).Cast<object>();
			Assert.IsNotNull(returnObjects);
			Assert.AreEqual(2, returnObjects.Count());

			IObjectWrapper objectWrapper = new ObjectWrapper(returnObjects.First());
			Assert.AreEqual("Eunge Liu", objectWrapper.GetPropertyValue("Name"));
			Assert.AreEqual("Software Development", objectWrapper.GetPropertyValue("Career"));
			Assert.AreEqual(new DateTime(1982, 2, 7), objectWrapper.GetPropertyValue("Birthday"));

			objectWrapper = new ObjectWrapper(returnObjects.Last());
			Assert.AreEqual("Lucy Liu", objectWrapper.GetPropertyValue("Name"));
			Assert.AreEqual("Financial", objectWrapper.GetPropertyValue("Career"));
			Assert.AreEqual(new DateTime(1982, 5, 1), objectWrapper.GetPropertyValue("Birthday"));
		}

		[Test(Description = "Test cases on creating decorate objects dynamically for anonymous class instances with converting property types.")]
		public void CreateDecorateObjectsWithConvertPropertyTypesTest()
		{
			var anonymousObjects = new[] 
			{
				new { Name = "Eunge Liu", Career = "Software Development", Birthday = new DateTime(1982, 2, 7), Salary = 13000 },
				new { Name = "Lucy Liu", Career = "Financial", Birthday = new DateTime(1982, 5, 1), Salary = 3000 }
			};

			IEnumerable results = ClassDecorator.CreateDecorateObjects(anonymousObjects, new[]
				{
					new PropertyDefinition(typeof(string)) { PropertyName = "Name" },
					new PropertyDefinition(typeof(string)) { PropertyName = "Career" },
					new PropertyDefinition(typeof(string)) { PropertyName = "Birthday", PropertyValueConvertCallback = sender => { return ((DateTime)sender).ToString("yyyy-MM-dd") ;}},
					new PropertyDefinition(typeof(string)) { PropertyName = "Salary" },
				});
			IEnumerable<object> returnObjects = results.Cast<object>();
			Assert.IsNotNull(returnObjects);
			Assert.AreEqual(2, returnObjects.Count());

			IObjectWrapper objectWrapper = new ObjectWrapper(returnObjects.First());
			Assert.AreEqual("Eunge Liu", objectWrapper.GetPropertyValue("Name"));
			Assert.AreEqual("Software Development", objectWrapper.GetPropertyValue("Career"));
			Assert.AreEqual("1982-02-07", objectWrapper.GetPropertyValue("Birthday"));
			Assert.AreEqual("13000", objectWrapper.GetPropertyValue("Salary"));

			objectWrapper = new ObjectWrapper(returnObjects.Last());
			Assert.AreEqual("Lucy Liu", objectWrapper.GetPropertyValue("Name"));
			Assert.AreEqual("Financial", objectWrapper.GetPropertyValue("Career"));
			Assert.AreEqual("1982-05-01", objectWrapper.GetPropertyValue("Birthday"));
			Assert.AreEqual("3000", objectWrapper.GetPropertyValue("Salary"));
		}

		[Test(Description = "Test cases on creating decorate objects dynamically for anonymous class instances with dictionary property.")]
		public void CreateDecorateObjectsForTypeWithDictionaryPropertyTest()
		{
			CodeDomTestObject obj1 = new CodeDomTestObject { Name = "Eunge" };
			obj1.Properties["AnnualPackage"] = 210000;
			obj1.Properties["Birthday"] = new DateTime(1982, 2, 7);
			obj1.Properties["Position"] = "Engineer";

			IEnumerable results = ClassDecorator.CreateDecorateObjects(new[] { obj1 }, new[]
				{
					new PropertyDefinition(typeof(string)) { PropertyName = "Name" },
					new PropertyDefinition(typeof(int))
					{ 
						PropertyName = @"Properties[""AnnualPackage""]", 
						NewPropertyName = FieldNameTransformUtility.DataBoundFieldName(@"Properties[""AnnualPackage""]", 1),
					},
					new PropertyDefinition(typeof(DateTime))
					{ 
						PropertyName = @"Properties[""Birthday""]", 
						NewPropertyName = FieldNameTransformUtility.DataBoundFieldName(@"Properties[""Birthday""]", 2),
					},
					new PropertyDefinition(typeof(string))
					{ 
						PropertyName = @"Properties[""Position""]", 
						NewPropertyName = FieldNameTransformUtility.DataBoundFieldName(@"Properties[""Position""]", 3),
					}
				});

			IEnumerable<object> returnObjects = results.Cast<object>();
			Assert.IsNotNull(returnObjects);
			Assert.AreEqual(1, returnObjects.Count());

			IObjectWrapper objectWrapper = new ObjectWrapper(returnObjects.First());
			Assert.AreEqual("Eunge", objectWrapper.GetPropertyValue("Name"));

			string positionPropertyName = FieldNameTransformUtility.DataBoundFieldName(@"Properties[""Position""]", 3);
			Assert.AreEqual("Engineer", objectWrapper.GetPropertyValue(positionPropertyName));
		}

		[Test(Description = "")]
		public void CreateComplexDecorateObjectsTest()
		{
			string[] bindingPropertyNames = new string[] { "Name", "SubObject.Category", "Properties[\"Ext\"].Category", "[\"Com\"].Category", "[\"Com\"].Description" };
			CodeDomTestObject fixture = new CodeDomTestObject
			{
				Name = "Eunge",
				SubObject = new SubCodeDomTestObject { Category = "NUnit", Description = "Desc" },
			};

			fixture["Com"] = new SubCodeDomTestObject { Category = "Com.NUnit", Description = "Com.Desc" };
			fixture.Properties["Ext"] = new SubCodeDomTestObject { Category = "Ext.NUnit", Description = "Ext.Desc" };

			Collection<PropertyDefinition> configs = new Collection<PropertyDefinition>();
			// Add wrappers for original properties
			Dictionary<string, string> cachedFieldNames = new Dictionary<string,string>();
			int index = 0;
			foreach (string bindingPropertyName in bindingPropertyNames)
			{
				PropertyDefinition config = new PropertyDefinition(typeof(string))
				{
					PropertyName = bindingPropertyName,
					NewPropertyName = FieldNameTransformUtility.DataBoundFieldName(bindingPropertyName, index++),
				};

				cachedFieldNames[bindingPropertyName]=config.NewPropertyName;
				configs.Add(config);
			}

			IEnumerable outputs = ClassDecorator.CreateDecorateObjects(new[] { fixture }, configs);
			object outputObject = outputs.Cast<object>().First();
			Assert.AreEqual("Eunge", DataBinder.Eval(outputObject, cachedFieldNames["Name"]));
			Assert.AreEqual("NUnit", DataBinder.Eval(outputObject, cachedFieldNames["SubObject.Category"]));
			Assert.AreEqual("Ext.NUnit", DataBinder.Eval(outputObject, cachedFieldNames["Properties[\"Ext\"].Category"]));
			Assert.AreEqual("Com.NUnit", DataBinder.Eval(outputObject, cachedFieldNames["[\"Com\"].Category"]));
			Assert.AreEqual("Com.Desc", DataBinder.Eval(outputObject, cachedFieldNames["[\"Com\"].Description"]));
		}

		[Test(Description = "")]
		public void FieldNameExtractor_GetClientDataBoundNameTest()
		{
			// SubObject.Category
			string newPropertyName = FieldNameTransformUtility.DataBoundFieldName("SubObject.Category", 0);
			Assert.AreEqual("SubObject___pCategory___db0", newPropertyName);

			// Reuse SubObject.Category
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("SubObject.Category", 1);
			Assert.AreEqual("SubObject___pCategory___db1", newPropertyName);
		}

		[Test(Description = "")]
		public void FieldNameExtractor_GetClientDataBoundName_WithIndexerPropertyTest()
		{
			#region Property Indexer 

			// Properties["Ext"]
			string newPropertyName = FieldNameTransformUtility.DataBoundFieldName("Properties[\"Ext\"]", 0);
			Assert.AreEqual("Properties___idxExt___db0", newPropertyName);

			// Properties["Ext"].Category
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("Properties[\"Ext\"].Category", 0);
			Assert.AreEqual("Properties___idxExt___pCategory___db0", newPropertyName);

			// Reuse Properties["Ext"].Category
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("Properties[\"Ext\"].Category", 1);
			Assert.AreEqual("Properties___idxExt___pCategory___db1", newPropertyName);

			// Properties["Ext"].Complex["Category"]
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("Properties[\"Ext\"].Complex[\"Category\"]", 0);
			Assert.AreEqual("Properties___idxExt___pComplex___idxCategory___db0", newPropertyName);

			// Properties["Ext"].Complex["Category"].Name
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("Properties[\"Ext\"].Complex[\"Category\"].Name", 0);
			Assert.AreEqual("Properties___idxExt___pComplex___idxCategory___pName___db0", newPropertyName);

			// Properties["Ext"]["Category"]
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("Properties[\"Ext\"][\"Category\"]", 0);
			Assert.AreEqual("Properties___idxExt___idxCategory___db0", newPropertyName);

			// Reuse Properties["Ext"]["Category"]
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("Properties[\"Ext\"][\"Category\"]", 1);
			Assert.AreEqual("Properties___idxExt___idxCategory___db1", newPropertyName);

			#endregion

			#region this Indexer

			// ["Ext"].Category
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("[\"Ext\"].Category", 0);
			Assert.AreEqual("this___idxExt___pCategory___db0", newPropertyName);

			// Reuse ["Ext"].Category
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("[\"Ext\"].Category", 1);
			Assert.AreEqual("this___idxExt___pCategory___db1", newPropertyName);

			// ["Ext"].Complex["Category"].Name
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("[\"Ext\"].Complex[\"Category\"].Name", 0);
			Assert.AreEqual("this___idxExt___pComplex___idxCategory___pName___db0", newPropertyName);

			#endregion

			#region Indirect Indexer Property 

			// SubObject.Properties["Ext"]["Category"]
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("SubObject.Properties[\"Ext\"][\"Category\"]", 0);
			Assert.AreEqual("SubObject___pProperties___idxExt___idxCategory___db0", newPropertyName);

			// SubObject.Properties["Ext"].Properties["Category"]
			newPropertyName = FieldNameTransformUtility.DataBoundFieldName("SubObject.Properties[\"Ext\"].Properties[\"Category\"]", 0);
			Assert.AreEqual("SubObject___pProperties___idxExt___pProperties___idxCategory___db0", newPropertyName);

			#endregion
		}
	}

	public class CodeDomTestObject
	{
		private IDictionary<string, object> properties = new Dictionary<string, object>();

		public string Name { get; set; }
		public SubCodeDomTestObject SubObject { get; set; }
		public IDictionary<string, object> Properties
		{
			get { return this.properties; }
		}

		public object this[string name]
		{
			get { if (!this.properties.ContainsKey(name)) return null; return this.properties[name]; }
			set { this.properties[name] = value; }
		}
	}

	public class SubCodeDomTestObject
	{
		public string Category { get; set; }
		public string Description { get; set; }
	}
}

