﻿using System;
using NUnit.Framework;

namespace PlayGen.ITAlert.Simulation.Entities.Tests
{
//	[TestFixture]
//	public class EntityTests
//	{
//		private class TestEntity : ITAlertEntity<ITAlertEntityState>
//		{
//			public TestEntity(ISimulation simulation)
//				: base(simulation, EntityType.Undefined)
//			{
//			}
//
//			protected override void OnTick()
//			{
//				throw new NotImplementedException();
//			}
//
//			public override ITAlertEntityState GenerateState()
//			{
//				throw new NotImplementedException();
//			}
//		}
//
//		//[Test]
//		//public void TestEntityCreatedEvent()
//		//{
//		//	Entity lastEntityCreated = null;
//		//	EntityBase.EntityCreated += (sender, args) => lastEntityCreated = sender as Entity;
//
//		//	Assert.Null(lastEntityCreated);
//
//		//	var testOne = new TestEntity();
//		//	Assert.NotNull(lastEntityCreated);
//		//	Assert.AreEqual(testOne.Id, lastEntityCreated.Id);
//		//	Assert.AreEqual(testOne, lastEntityCreated);
//
//		//	var testTwo = new TestEntity();
//		//	Assert.NotNull(lastEntityCreated);
//		//	Assert.AreEqual(testTwo.Id, lastEntityCreated.Id);
//		//	Assert.AreEqual(testTwo, lastEntityCreated);
//		//}
//
//		[Test]
//		public void TestEntityDisposalAndDestroyedEvent()
//		{
//			var sim = new TestSimulation();
//
//			Entity lastEntityDestroyed = null;
//			Entity tempEntity;
//
//			using (var testOne = new TestEntity(sim))
//			{
//				testOne.EntityDelegate += (sender, args) => lastEntityDestroyed = sender as Entity;
//
//				Assert.Null(lastEntityDestroyed);
//				tempEntity = testOne;
//			}
//
//			Assert.NotNull(tempEntity);
//			Assert.NotNull(lastEntityDestroyed);
//			Assert.AreEqual(tempEntity.Id, lastEntityDestroyed.Id);
//			Assert.AreEqual(tempEntity, lastEntityDestroyed);
//
//			var testTwo = new TestEntity(sim);
//			testTwo.EntityDelegate += (sender, args) => lastEntityDestroyed = sender as Entity;
//			testTwo.Dispose();
//
//			Assert.NotNull(lastEntityDestroyed);
//			Assert.AreEqual(testTwo.Id, lastEntityDestroyed.Id);
//			Assert.AreEqual(testTwo, lastEntityDestroyed);
//
//			// test that the destroyed event is not called again after the entity has been disposed
//			lastEntityDestroyed = null;
//			testTwo.Dispose();
//
//			Assert.Null(lastEntityDestroyed);
//		}
//	}
}
