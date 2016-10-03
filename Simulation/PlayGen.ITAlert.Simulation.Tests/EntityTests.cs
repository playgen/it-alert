using System;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation.Tests
{
	[TestFixture]
	public class EntityTests
	{
		private class TestEntity : Entity<EntityState>
		{
			public TestEntity(ISimulation simulation) 
				: base(simulation, EntityType.Undefined)
			{
			}

			protected override void OnTick()
			{
				throw new NotImplementedException();
			}

			public override EntityState GenerateState()
			{
				throw new NotImplementedException();
			}
		}

		//[Test]
		//public void TestEntityCreatedEvent()
		//{
		//	IEntity lastEntityCreated = null;
		//	EntityBase.EntityCreated += (sender, args) => lastEntityCreated = sender as IEntity;

		//	Assert.Null(lastEntityCreated);

		//	var testOne = new TestEntity();
		//	Assert.NotNull(lastEntityCreated);
		//	Assert.AreEqual(testOne.Id, lastEntityCreated.Id);
		//	Assert.AreEqual(testOne, lastEntityCreated);

		//	var testTwo = new TestEntity();
		//	Assert.NotNull(lastEntityCreated);
		//	Assert.AreEqual(testTwo.Id, lastEntityCreated.Id);
		//	Assert.AreEqual(testTwo, lastEntityCreated);
		//}

		[Test]
		public void TestEntityDisposalAndDestroyedEvent()
		{
			var sim = new TestSimulation();

			IEntity lastEntityDestroyed = null;
			IEntity tempEntity;

			using (var testOne = new TestEntity(sim))
			{
				testOne.EntityDestroyed += (sender, args) => lastEntityDestroyed = sender as IEntity;

				Assert.Null(lastEntityDestroyed);
				tempEntity = testOne;
			}

			Assert.NotNull(tempEntity);
			Assert.NotNull(lastEntityDestroyed);
			Assert.AreEqual(tempEntity.Id, lastEntityDestroyed.Id);
			Assert.AreEqual(tempEntity, lastEntityDestroyed);

			var testTwo = new TestEntity(sim);
			testTwo.EntityDestroyed += (sender, args) => lastEntityDestroyed = sender as IEntity;
			testTwo.Dispose();

			Assert.NotNull(lastEntityDestroyed);
			Assert.AreEqual(testTwo.Id, lastEntityDestroyed.Id);
			Assert.AreEqual(testTwo, lastEntityDestroyed);

			// test that the destroyed event is not called again after the entity has been disposed
			lastEntityDestroyed = null;
			testTwo.Dispose();

			Assert.Null(lastEntityDestroyed);
		}
	}
}
