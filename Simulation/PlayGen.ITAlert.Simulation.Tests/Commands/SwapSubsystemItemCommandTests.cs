using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Engine.Sequencing;
using Engine.Systems;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Scenario.Actions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Startup;
using Player = PlayGen.ITAlert.Simulation.Archetypes.Player;

namespace PlayGen.ITAlert.Simulation.Tests.Commands
{
	public class SwapSubsystemItemCommandTests
	{
		[Test]
		public void CantSwapUnreleasableWithReleasableItem()
		{
			// Arrange
			const string testName = "CantSwapUnreleasableWithReleasableItem";

			var nodeConfigs = new[]
			{
				new NodeConfig(
					0,
					0,
					AntivirusWorkstation.Archetype),

				new NodeConfig(
					0,
					1,
					SubsystemNode.Archetype),
			};

			SetupScenario(testName, out var lifecycleManager, nodeConfigs);

			CreatePlayer(testName,
				nodeConfigs[0],
				lifecycleManager,
				out var player);

			CreateItem(TutorialScanner.Archetype.Name,
				nodeConfigs[0],
				lifecycleManager,
				out var item2,
				out var item2ContainerIndex,
				out var item2Container);

			GetSubsystemItemStorage(nodeConfigs[0],
				lifecycleManager,
				out var subsystem1ItemStorage);

			var item1ContainerIndex = 0;
			var item1Id = subsystem1ItemStorage.Component2.Items[item1ContainerIndex].Item;

			// Assert items are in correct locatioons
			Assert.NotNull(subsystem1ItemStorage.Component2.Items[0].Item);
			Assert.NotNull(subsystem1ItemStorage.Component2.Items[1].Item);
			Assert.AreEqual(subsystem1ItemStorage.Component2.Items[0].Item.Value, item1Id);
			Assert.AreEqual(subsystem1ItemStorage.Component2.Items[1].Item.Value, item2.Entity.Id);

			// Act
			var swapCommand = new SwapSubsystemItemCommand
			{
				PlayerId = player.Entity.Id,
				SubsystemId = nodeConfigs[0].EntityId,
				FromItemId = item2.Entity.Id,
				FromContainerIndex = item2ContainerIndex,
				ToItemId = item1Id.Value,
				ToContainerIndex = item1ContainerIndex
			};

			lifecycleManager.ECSRoot.ECS.EnqueueCommand(swapCommand);

			lifecycleManager.ECSRoot.ECS.Tick();

			// Assert items are still in original locations
			Assert.AreEqual(subsystem1ItemStorage.Component2.Items[0].Item.Value, item1Id);
			Assert.AreEqual(subsystem1ItemStorage.Component2.Items[1].Item.Value, item2.Entity.Id);
		}

		[Test]
		public void CantSwapReleasableWithUnreleasableItem()
		{
			// Arrange
			const string testName = "CantSwapReleasableWithUnreleasableItem";

			var nodeConfigs = new []
			{
				new NodeConfig(
					0,
					0,
					AntivirusWorkstation.Archetype),

				new NodeConfig(
					0,
					1,
					SubsystemNode.Archetype),
			};

			SetupScenario(testName, out var lifecycleManager, nodeConfigs);

			CreatePlayer(testName,
				nodeConfigs[0],
				lifecycleManager,
				out var player);

			CreateItem(TutorialScanner.Archetype.Name,
				nodeConfigs[0],
				lifecycleManager,
				out var item2,
				out var item2ContainerIndex,
				out var item2Container);

			GetSubsystemItemStorage(nodeConfigs[0],
				lifecycleManager,
				out var subsystem1ItemStorage);

			var item1ContainerIndex = 0;
			var item1Id = subsystem1ItemStorage.Component2.Items[item1ContainerIndex].Item;

			// Assert items are in correct locatioons
			Assert.NotNull(subsystem1ItemStorage.Component2.Items[0].Item);
			Assert.NotNull(subsystem1ItemStorage.Component2.Items[1].Item);
			Assert.AreEqual(subsystem1ItemStorage.Component2.Items[0].Item.Value, item1Id);
			Assert.AreEqual(subsystem1ItemStorage.Component2.Items[1].Item.Value, item2.Entity.Id);

			// Act
			var swapCommand = new SwapSubsystemItemCommand
			{
				PlayerId = player.Entity.Id,
				SubsystemId = nodeConfigs[0].EntityId,
				FromItemId = item1Id.Value,
				FromContainerIndex = item1ContainerIndex,
				ToItemId = item2.Entity.Id,
				ToContainerIndex = item2ContainerIndex
			};

			lifecycleManager.ECSRoot.ECS.EnqueueCommand(swapCommand);

			lifecycleManager.ECSRoot.ECS.Tick();

			// Assert items are still in original locations
			Assert.AreEqual(subsystem1ItemStorage.Component2.Items[0].Item.Value, item1Id);
			Assert.AreEqual(subsystem1ItemStorage.Component2.Items[1].Item.Value, item2.Entity.Id);
		}

		[Test]
		public void CantSwapOnAnotherSystem()
		{
			// Arrange
			const string testName = "CantSwapOnAnotherSystem";

			SetupScenario(testName,
				out var lifecycleManager,
				out var nodeConfigs);

			CreatePlayer(testName,
				nodeConfigs[0],
				lifecycleManager,
				out var player);

			CreateItem(TutorialScanner.Archetype.Name,
				nodeConfigs[1],
				lifecycleManager,
				out var item1,
				out var item1ContainerIndex,
				out var item1Container);

			CreateItem(TutorialScanner.Archetype.Name,
				nodeConfigs[1],
				lifecycleManager,
				out var item2,
				out var item2ContainerIndex,
				out var item2Container);

			GetSubsystemItemStorage(nodeConfigs[0],
				lifecycleManager,
				out var subsystem1ItemStorage);

			GetSubsystemItemStorage(nodeConfigs[1],
				lifecycleManager,
				out var subsystem2ItemStorage);

			// Assert setup has 2 items in other system
			Assert.IsNull(subsystem1ItemStorage.Component2.Items[0].Item);
			Assert.IsNull(subsystem1ItemStorage.Component2.Items[1].Item);
			Assert.IsNull(subsystem1ItemStorage.Component2.Items[2].Item);
			Assert.IsNull(subsystem1ItemStorage.Component2.Items[3].Item);

			Assert.AreEqual(subsystem2ItemStorage.Component2.Items[0].Item, item1.Entity.Id);
			Assert.AreEqual(subsystem2ItemStorage.Component2.Items[1].Item, item2.Entity.Id);
			Assert.IsNull(subsystem2ItemStorage.Component2.Items[2].Item);
			Assert.IsNull(subsystem2ItemStorage.Component2.Items[3].Item);

			// Act
			var swapCommand = new SwapSubsystemItemCommand
			{
				PlayerId = player.Entity.Id,
				SubsystemId = nodeConfigs[0].EntityId,
				FromItemId = item1.Entity.Id,
				FromContainerIndex = item1ContainerIndex,
				ToItemId = item2.Entity.Id,
				ToContainerIndex = item2ContainerIndex
			};

			lifecycleManager.ECSRoot.ECS.EnqueueCommand(swapCommand);

			lifecycleManager.ECSRoot.ECS.Tick();

			// Assert still has 2 items in other system
			Assert.IsNull(subsystem1ItemStorage.Component2.Items[0].Item);
			Assert.IsNull(subsystem1ItemStorage.Component2.Items[1].Item);
			Assert.IsNull(subsystem1ItemStorage.Component2.Items[2].Item);
			Assert.IsNull(subsystem1ItemStorage.Component2.Items[3].Item);

			Assert.AreEqual(subsystem2ItemStorage.Component2.Items[0].Item, item1.Entity.Id);
			Assert.AreEqual(subsystem2ItemStorage.Component2.Items[1].Item, item2.Entity.Id);
			Assert.IsNull(subsystem2ItemStorage.Component2.Items[2].Item);
			Assert.IsNull(subsystem2ItemStorage.Component2.Items[3].Item);
		}
		
		[Test]
		public void CanSwapItemToNothing()
		{
			// Arrange
			const string testName = "CanSwapItemToNothing";
			
			SetupScenario(testName,
				out var lifecycleManager,
				out var nodeConfigs);

			CreatePlayer(testName,
				nodeConfigs[0],
				lifecycleManager,
				out var player);

			CreateItem(TutorialScanner.Archetype.Name,
				nodeConfigs[0],
				lifecycleManager,
				out var item1,
				out var item1ContainerIndex,
				out var item1Container);

			GetSubsystemItemStorage(nodeConfigs[0],
				lifecycleManager,
				out var subsystemItemStorage);

			// Assert there is only one item in thge correct location
			var item2Container = subsystemItemStorage.Component2.Items[1];

			Assert.AreEqual(subsystemItemStorage.Component2.Items[0].Item.Value,
				item1.Entity.Id);
			Assert.IsNull(item2Container.Item);
			Assert.IsNull(subsystemItemStorage.Component2.Items[2].Item);
			Assert.IsNull(subsystemItemStorage.Component2.Items[3].Item);
			
			// Act
			var swapCommand = new SwapSubsystemItemCommand
			{
				PlayerId = player.Entity.Id,
				SubsystemId = nodeConfigs[0].EntityId,
				FromItemId = item1.Entity.Id,
				FromContainerIndex = item1ContainerIndex,
				ToItemId = null,
				ToContainerIndex = 1
			};

			lifecycleManager.ECSRoot.ECS.EnqueueCommand(swapCommand);

			lifecycleManager.ECSRoot.ECS.Tick();

			// Assert Swapped
			Assert.AreEqual(item1Container.Item, null);
			Assert.AreEqual(item2Container.Item, item1.Entity.Id);
		}

		[Test]
		public void CanSwapItemToItem()
		{
			// Arrange
			const string testName = "CanSwapItemToItem";

			SetupScenario(testName,
				out var lifecycleManager,
				out var nodeConfigs);

			CreatePlayer(testName,
				nodeConfigs[0],
				lifecycleManager,
				out var player);

			CreateItem(TutorialScanner.Archetype.Name,
				nodeConfigs[0],
				lifecycleManager,
				out var item1,
				out var item1ContainerIndex,
				out var item1Container);

			CreateItem(TutorialScanner.Archetype.Name,
				nodeConfigs[0],
				lifecycleManager,
				out var item2,
				out var item2ContainerIndex,
				out var item2Container);

			// Assert Not Swapped
			Assert.AreEqual(item1Container.Item,
				item1.Entity.Id);
			Assert.AreEqual(item2Container.Item,
				item2.Entity.Id);

			// Act
			var swapCommand = new SwapSubsystemItemCommand
			{
				PlayerId = player.Entity.Id,
				SubsystemId = nodeConfigs[0].EntityId,
				FromItemId = item1.Entity.Id,
				FromContainerIndex = item1ContainerIndex,
				ToItemId = item2.Entity.Id,
				ToContainerIndex = item2ContainerIndex
			};

			lifecycleManager.ECSRoot.ECS.EnqueueCommand(swapCommand);

			lifecycleManager.ECSRoot.ECS.Tick();

			// Assert Swapped
			Assert.AreEqual(item1Container.Item, item2.Entity.Id);
			Assert.AreEqual(item2Container.Item, item1.Entity.Id);
		}

		#region Helpers

		public void SetupScenario(string name, out SimulationLifecycleManager lifecycleManager, NodeConfig[] nodeConfigs)
		{
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);
			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);

			var archetypes = new List<Archetype>
			{
				SubsystemNode.Archetype,
				ConnectionNode.Archetype,
				Player.Archetype,
				TutorialScanner.Archetype,
				AntivirusWorkstation.Archetype,
				AnalyserActivator.Archetype,
				AntivirusTool.Archetype,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs,
				edgeConfigs,
				null,
				archetypes);

			var scenarioInfo = new ScenarioInfo
			{
				Description = name,
				Key = name,
				LocalizationDictionary = null,
				MaxPlayerCount = 1,
				MinPlayerCount = 1,
				Name = name
			};

			var scenario = new SimulationScenario(scenarioInfo)
			{
				Configuration = configuration,
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			lifecycleManager = SimulationLifecycleManager.Initialize(scenario);
		}

		public void SetupScenario(string name, out SimulationLifecycleManager lifecycleManager, out NodeConfig[] nodeConfigs)
		{
			// Arrange
			var nodeLeft = new NodeConfig
			{
				Name = "00",
				X = 0,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			var nodeRight = new NodeConfig()
			{
				Name = "10",
				X = 1,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			nodeConfigs = new []
			{
				nodeLeft,
				nodeRight
			};

			SetupScenario(name, out lifecycleManager, nodeConfigs);
		}

		private bool GetSubsystemItemStorage(EntityConfig node, SimulationLifecycleManager lifecycleManager, out ComponentEntityTuple<Subsystem, ItemStorage> subsystemTuple)
		{
			var systemWithStorageMatcher = lifecycleManager.ECSRoot.ECS.MatcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			return systemWithStorageMatcher.TryGetMatchingEntity(node.EntityId, out subsystemTuple);
		}

		private void CreatePlayer(string name, EntityConfig node, SimulationLifecycleManager lifecycleManager, out ComponentEntityTuple<ITAlert.Simulation.Components.EntityTypes.Player> player)
		{
			var playerMatcher = lifecycleManager.ECSRoot.ECS.MatcherProvider.CreateMatcherGroup<ITAlert.Simulation.Components.EntityTypes.Player>();
			var priorMatchingEntitesTotal = playerMatcher.MatchingEntities.Length;
			
			// Create a NPC to initiate the swap action
			var createNpc = new CreatePlayer(Player.Archetype,
				node,
				name);
			createNpc.Execute(lifecycleManager.ECSRoot.ECS,
				lifecycleManager.ECSRoot.Configuration);

			// Tick so commands get processed
			lifecycleManager.ECSRoot.ECS.Tick();
			
			player = playerMatcher.MatchingEntities[priorMatchingEntitesTotal];
		}

		private void CreateItem(string itemType, EntityConfig node, SimulationLifecycleManager lifecycleManager, out ComponentEntityTuple<IItemType, CurrentLocation> item, out int itemContainerIndex, out ItemContainer itemContainer)
		{
			// store current match
			var itemLocationMatcher = lifecycleManager.ECSRoot.ECS.MatcherProvider.CreateMatcherGroup<IItemType, CurrentLocation>();
			var priorMatchignEntitiesTotal = itemLocationMatcher.MatchingEntities.Length;

			// Add Items to swap
			var createItem = new CreateItem(TutorialScanner.Archetype.Name,
				node);

			createItem.Execute(lifecycleManager.ECSRoot.ECS,
				lifecycleManager.ECSRoot.Configuration);

			// Tick so commands get processed
			lifecycleManager.ECSRoot.ECS.Tick();

			item = itemLocationMatcher.MatchingEntities[priorMatchignEntitiesTotal];

			// Get the subsystem the item's current location is set to
			var systemWithStorageMatcher = lifecycleManager.ECSRoot.ECS.MatcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			systemWithStorageMatcher.TryGetMatchingEntity(item.Component2.Value.Value,
				out var subsystemTuple);

			var itemLocal = item;
			itemContainerIndex = Array.FindIndex(subsystemTuple.Component2.Items,
				ic => ic.Item == itemLocal.Entity.Id);

			itemContainer = subsystemTuple.Component2.Items[itemContainerIndex];
		}
		#endregion
	}
}
