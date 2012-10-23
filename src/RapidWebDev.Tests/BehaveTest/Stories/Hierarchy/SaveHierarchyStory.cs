/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.yi@RapidWebDev.org

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
using System.Linq;
using BaoJianSoft.Common;
using BaoJianSoft.Common.Data;
using BaoJianSoft.Platform;
using BaoJianSoft.Platform.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace BaoJianSoft.Tests.BehaveTest.Stories.Hierarchy
{
	[TestFixture, Theme]
	public class SaveHierarchyStory : SetupFixture
	{
		private Story _story;

		private HierarchyDataObject _HierarchyDataObject;
		private HierarchyDataObject _HierarchyDataObject1;
		private HierarchyDataObject _HierarchyDataObject2;

		private OrganizationObject _OrganizationObject1;
		private OrganizationObject _OrganizationObject2;

		private OrganizationTypeObject _OrganizationTypeObject1;

		private RelationshipObject _RelationshipObject1;
		private RelationshipObject _RelationshipObject2;
		private RelationshipObject _RelationshipObject3;

		private IOrganizationApi _OrganizationApi;
		private IHierarchyApi _HierarchyApi;
		private IRelationshipApi _RelationShipApi;

		private List<Guid> createdOrganizationIds;
		private List<Guid> createdOrganizationTypeIds;
		private List<Guid> createHierarchyIds;
		private BehaveOrganizationUtils _Organutils;

		[SetUp]
		public void Setup()
		{
			base.GlobalSetup();
			createdOrganizationIds = new List<Guid>();
			createdOrganizationTypeIds = new List<Guid>();
			_Organutils = new BehaveOrganizationUtils();
			createHierarchyIds = new List<Guid>();
			_HierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();
			_OrganizationApi = SpringContext.Current.GetObject<IOrganizationApi>();
			_RelationShipApi = SpringContext.Current.GetObject<IRelationshipApi>();
		}

		public SaveHierarchyStory()
		{
			Console.WriteLine("=================Setup===================");


			base.GlobalSetup();

			createdOrganizationIds = new List<Guid>();

			createdOrganizationTypeIds = new List<Guid>();

			_Organutils = new BehaveOrganizationUtils();

			//createdRelationshipIds = new List<Guid>();

			createHierarchyIds = new List<Guid>();

			_HierarchyApi = SpringContext.Current.GetObject<IHierarchyApi>();

			_OrganizationApi = SpringContext.Current.GetObject<IOrganizationApi>();


			_RelationShipApi = SpringContext.Current.GetObject<IRelationshipApi>();

			Console.WriteLine("============Ending Setup===================");
		}
		[TearDown]
		public void CleanUp()
		{
			Console.WriteLine("============Clean Up====================");

			using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
			{
				var organsTodel = ctx.Organizations.Where(x => createdOrganizationIds.ToArray().Contains(x.OrganizationId));

				var organtypeTodel = ctx.OrganizationTypes.Where(x => createdOrganizationTypeIds.ToArray().Contains(x.OrganizationTypeId));

				var relationTodel = ctx.Relationships.Where(x => 1 == 1);

				var hierarchyTodel = ctx.HierarchyDatas.Where(x => createHierarchyIds.ToArray().Contains(x.HierarchyDataId));

				ctx.OrganizationTypes.DeleteAllOnSubmit(organtypeTodel);

				ctx.Organizations.DeleteAllOnSubmit(organsTodel);

				ctx.Relationships.DeleteAllOnSubmit(relationTodel);

				ctx.HierarchyDatas.DeleteAllOnSubmit(hierarchyTodel);

				ctx.SubmitChanges();
			}


			Console.WriteLine("========Ending Clean Up====================");
		}

		[Story, Test]
		public void HierarchyTreeStory()
		{
			_story = new Story("Save the Hierarchy data And Bind to Organization Type");

			_story.AsA("User")
			.IWant("Create Hierarchy Data")
			.SoThat("I can bind the Hierarchy to the object that have cascade relationship");

			_story.WithScenario("Save Hierarchy Data")
			.Given("a Hierarchy data which is new ", () =>
			{
				_HierarchyDataObject = new HierarchyDataObject()
				{
					Code = "1111",
					Description = "Sample",
					HierarchyType = "Tree",
					Name = "Root"
				};

				_HierarchyApi.Save(_HierarchyDataObject);
				createHierarchyIds.Add(_HierarchyDataObject.Id);
			})
			.And("create More than one Organization ", () =>
			{
				_OrganizationTypeObject1 = _Organutils.CreateOrganizationTypeOject("Root", "Inc");

				_OrganizationApi.Save(_OrganizationTypeObject1);
				createdOrganizationTypeIds.Add(_OrganizationTypeObject1.OrganizationTypeId);

				_OrganizationObject1 = _Organutils.CreateOrganizationObject(_OrganizationTypeObject1.OrganizationTypeId, "Tim", "sh");
				_OrganizationObject2 = _Organutils.CreateOrganizationObject(_OrganizationTypeObject1.OrganizationTypeId, "Euge", "sc");

				_OrganizationApi.Save(_OrganizationObject1);
				_OrganizationApi.Save(_OrganizationObject2);

				createdOrganizationIds.Add(_OrganizationObject1.OrganizationId);
				createdOrganizationIds.Add(_OrganizationObject2.OrganizationId);
			})
			.When("Have the Hierarchy Data", () =>
			{
				_HierarchyDataObject1 = _HierarchyApi.GetHierarchyData(_HierarchyDataObject.Id);
				_HierarchyDataObject2 = _HierarchyApi.GetHierarchyData("Tree", "Root");
				_HierarchyDataObject1.Id.ShouldEqual(_HierarchyDataObject.Id);
				_HierarchyDataObject2.Id.ShouldEqual(_HierarchyDataObject.Id);
			})

			.Then("I can use the Hierarchy Data to bind to Organization object", () =>
			{

				_RelationshipObject1 = new RelationshipObject()
				{
					ReferenceObjectId = _HierarchyDataObject.Id,
					RelationshipType = "Tree"
				};

				_RelationShipApi.Save(_OrganizationObject1.OrganizationId, _RelationshipObject1);
				_RelationShipApi.Save(_OrganizationObject2.OrganizationId, _RelationshipObject1);

				_RelationshipObject2 = _RelationShipApi.GetOneToOne(_OrganizationObject1.OrganizationId, _RelationshipObject1.RelationshipType);
				_RelationshipObject3 = _RelationShipApi.GetOneToOne(_OrganizationObject2.OrganizationId, _RelationshipObject1.RelationshipType);

				_RelationshipObject2.ReferenceObjectId.ShouldEqual(_RelationshipObject3.ReferenceObjectId);
				_RelationshipObject2.Ordinal.ShouldEqual(_RelationshipObject3.Ordinal);
			})
			.WithScenario("How to create Hierarchy in Organization")
			.Given("Remove the relationship between Organ1 and Organ2", () =>
			{
				_RelationShipApi.Remove(_OrganizationObject1.OrganizationId);
				_RelationShipApi.Remove(_OrganizationObject2.OrganizationId, "Tree");
			})
			.And("Create a child Hierarchy data ", () =>
			{
				_HierarchyDataObject2 = new HierarchyDataObject()
				{
					Code = "1111",
					Description = "Sample",
					HierarchyType = "Tree",
					Name = "Leaf",
					ParentHierarchyDataId = _HierarchyDataObject.Id
				};
				_HierarchyApi.Save(_HierarchyDataObject2);
			})
			.When("I add the hierarchy data to Organization", () =>
			{
				_OrganizationObject1.Hierarchies.Add("Tree", _HierarchyDataObject.Id);

				_OrganizationObject2.Hierarchies.Add("Tree", _HierarchyDataObject2.Id);

				_OrganizationApi.Save(_OrganizationObject1);
				_OrganizationApi.Save(_OrganizationObject2);

			})
			.Then("I can get children HierarchyData by Hierarchy", () =>
				 {
					 _HierarchyDataObject1 = null;
					 _HierarchyDataObject1 = _HierarchyApi.GetImmediateChildren("Tree", _HierarchyDataObject.Id).FirstOrDefault();
					 _HierarchyDataObject1.Id.ShouldEqual(_HierarchyDataObject2.Id);
				 })
			.WithScenario("Delete the children Hierarchy Data")
			.Given("an Existing parent Hierarchy Data and an existing child", () => { })
			.When("I delete the parent hierarchy Data", () =>
				 {
					 _HierarchyApi.DeleteHierarchyData(_HierarchyDataObject.Id);
				 })
			 .Then("I cannot get the parent data, and Organization should not have this Hierarchy data", () =>
				  {

				  });

			this.CleanUp();
		}
	}
}
