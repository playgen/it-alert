using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Engine.Configuration;
using Engine.Testing;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Tests.Components
{
	[TestFixture]
	public class ItemOwnerTests
	{
		[Test]
		public void TestItemOwnerTransmission()
		{
			var testSystem = new Archetype("TestSystem")
				.HasComponent(new ComponentBinding<Subsystem>())
				.HasComponent(new ComponentBinding<Coordinate2DProperty>())
				.HasComponent(new ComponentBinding<Name>())
				.HasComponent(new ComponentBinding<ItemStorage>()
				{
					ComponentTemplate = new ItemStorage()
					{
						Items = new ItemContainer[1],
					}
				});

			var testItem = new Archetype("TestItem")
				.HasComponent(new ComponentBinding<Scanner>())
				.HasComponent(new ComponentBinding<Owner>());

			var nodeConfigs = new NodeConfig[] { new NodeConfig()
				{
					X = 0,
					Y = 0,
					Name = "Node 0",
					Archetype = testSystem.Name
				},
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var systems = new List<SystemConfiguration>()
			{
				new SystemConfiguration<ItemStorageSystem>(),
			};

			var archetypes = new List<Archetype>()
			{
				testSystem,
				testItem,
			};

			var lifecycleConfig = new LifeCycleConfiguration();
			var configuration = new SimulationConfiguration(nodeConfigs, null, null, archetypes, systems, lifecycleConfig);
			var scenario = new SimulationScenario() { Configuration = configuration };

			var rootA = SimulationInstaller.CreateSimulationRoot(scenario);
			var ecsA = rootA.ECS;

			Owner ownerA;
			TestEntityState(ecsA, null, out ownerA);

			ecsA.Tick();

			var rootB = SimulationInstaller.CreateSimulationRoot(scenario);
			var ecsB = rootB.ECS;

			Owner ownerB;
			TestEntityState(ecsB, null, out ownerB);

			Assert.That(ownerB, Is.Not.EqualTo(ownerA));

			const int owner = 3;
			ownerA.Value = 3;

			var json = rootA.GetEntityState();
			rootB.UpdateEntityState(json);

			Owner ownerC;
			TestEntityState(ecsB, owner, out ownerC);
			
			Assert.That(ownerC, Is.EqualTo(ownerB), "Owner was recreated");

			ownerA.Value = null;
			json = rootA.GetEntityState();
			rootB.UpdateEntityState(json);

			Owner ownerD;
			TestEntityState(ecsB, null, out ownerD);

			Assert.That(ownerD, Is.EqualTo(ownerB), "Owner was recreated");
		}

		private void TestEntityState(Simulation ecs, int? expectedOwnerId, out Owner owner)
		{
			Assert.That(ecs.Entities.Count, Is.EqualTo(2), "Entity count incorrect at initialization");

			var system = ecs.Entities[1];
			Assert.That(system.HasComponent<Subsystem>(), "First entity does not have subsystem component");

			var item = ecs.Entities[2];
			Assert.That(item.HasComponent<IItemType>(), "Second entity does not have item component");

			Assert.That(item.TryGetComponent(out owner), "Item does not have Owner component");
			Assert.That(owner.Value, Is.EqualTo(expectedOwnerId), "Owner id not equal");
		}

	}
}
