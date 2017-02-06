using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Entities;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Resources;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Simulation.Systems.Items;
using PlayGen.ITAlert.Simulation.Systems.Resources;

namespace PlayGen.ITAlert.Simulation.Tests.Components
{
	[TestFixture]
	public class MemoryResourceTests
	{
		[Test]
		public void TestMemoryResourceTransmission()
		{
			var nodes = new[] { new NodeConfig(0)
				{
					X = 0,
					Y = 0,
					Name = "Node 0",
					ArchetypeName = nameof(GameEntities.Subsystem)
				}
			};
			var items = new[]
			{
				new ItemConfig()
				{
					StartingLocation = 0,
					ArchetypeName = "Scanner",
				}
			};

			var systems = new List<SystemConfiguration>()
			{
				new SystemConfiguration<ItemStorageSystem>(),
				new SystemConfiguration<SubsystemResources>()
				{
					ExtensionConfiguration = new SystemExtensionConfiguration[]
					{
						new SystemExtensionConfiguration<ISubsystemResourceEffect>()
						{
							Implementations = new SystemExtensionImplementation[]
							{
								new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<ResetMemoryEachTick>(),
								new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<ItemsConsumeMemoryEffect>(),
							}
						}
					}
				},
			};

			var archetypes = new List<Archetype>()
			{
				GameEntities.Subsystem,
				GameEntities.Scanner,
			};

			var lifecycleConfig = new LifeCycleConfiguration();
			var configuration = new SimulationConfiguration(nodes, null, null, items, archetypes, systems, lifecycleConfig);
			var rootA = SimulationInstaller.CreateSimulationRoot(configuration);
			var ecsA = rootA.ECS;

			MemoryResource memoryResourceA;

			TestEntityState(ecsA, SimulationConstants.SubsystemInitialMemory, out memoryResourceA);

			ecsA.Tick();

			var rootB = SimulationInstaller.CreateSimulationRoot(configuration);
			var ecsB = rootB.ECS;

			MemoryResource memoryResourceB;
			TestEntityState(ecsB, SimulationConstants.SubsystemInitialMemory, out memoryResourceB);

			Assert.That(memoryResourceB, Is.Not.EqualTo(memoryResourceA));

			var json = rootA.GetEntityState();
			rootB.UpdateEntityState(json);

			MemoryResource memoryResourceC;
			TestEntityState(ecsB, SimulationConstants.ItemMemoryConsumption, out memoryResourceC);
			
			Assert.That(memoryResourceC, Is.EqualTo(memoryResourceB), "Memory Resource was recreated");
		}

		private void TestEntityState(Simulation ecs, int expectedMemoryConsumption, out MemoryResource memoryResource)
		{
			Assert.That(ecs.Entities.Count, Is.EqualTo(2), "Entity count incorrect at initialization");

			var system = ecs.Entities[1];
			Assert.That(system.HasComponent<Subsystem>(), "First entity does not have subsystem component");

			var item = ecs.Entities[2];
			Assert.That(item.HasComponent<IItemType>(), "Second entity does not have item component");

			ItemStorage itemStorage;
			Assert.That(system.TryGetComponent(out itemStorage), "System does not have ItemSTorage component");
			Assert.That(itemStorage.Items[0], Is.Not.Null, "Item storage not initialized");
			Assert.That(itemStorage.Items[0].Item.HasValue, "Item storage location 0 has not item");
			Assert.That(itemStorage.Items[0].Item.Value, Is.EqualTo(item.Id), "Item storage item id mismatch");

			ConsumeMemory consumeMemory;
			Assert.That(item.TryGetComponent(out consumeMemory), "Item does not have ConsumeMemory component");
			Assert.That(consumeMemory.Value, Is.EqualTo(SimulationConstants.ItemMemoryConsumption), "ConsumeMemory initial value incorrect");

			Assert.That(system.TryGetComponent(out memoryResource), "System does not have MemoryResource component");
			Assert.That(memoryResource.Maximum, Is.EqualTo(SimulationConstants.SubsystemMaxMemory), "System memory resource");

			Assert.That(memoryResource.Value, Is.EqualTo(expectedMemoryConsumption), "System memory resource initial value incorrect");
		}

	}
}
