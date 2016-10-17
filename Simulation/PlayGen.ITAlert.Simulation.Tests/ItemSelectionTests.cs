using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Intents;
using PlayGen.ITAlert.Simulation.Utilities;

namespace PlayGen.ITAlert.Simulation.Tests
{
	[TestFixture]
	public class ItemSelectionTests
	{
		[Test]
		public void TestSelectItemVerbose()
		{
			var sim = new TestSimulation();
			var subsystem = sim.CreateSubsystem(new NodeConfig(1));
			var otherSubsystem = sim.CreateSubsystem(new NodeConfig(2));
			var connection = sim.CreateConnection(sim.SubsystemsByLogicalId, new EdgeConfig(1, VertexDirection.Left, 2));

			var player = sim.CreatePlayer(new PlayerConfig());
			sim.SubsystemsByLogicalId[1].AddVisitor(player, null, 0);
			Assert.That(subsystem.HasVisitor(player), "Player not added to subsystem");

			var item = sim.CreateItem(ItemType.Cleaner);
			Assert.That(subsystem.CanAddItem(), "Could not add item");
			Assert.That(subsystem.TryAddItem(item), "Item not added");
			Assert.That(subsystem.HasItem(item), "Item not added to subsystem");
			Assert.That(item.IsOnSubsystem, "Item OnEnter CurrentSubsystem not set");
			Assert.That(item.CurrentNode?.Equals(subsystem) ?? false, "Item CurrentNode not set correctly");

			var otherItem = sim.CreateItem(ItemType.Repair);
			Assert.That(otherSubsystem.CanAddItem(), "Could not add item");
			Assert.That(otherSubsystem.TryAddItem(otherItem), "Other Item not added");
			Assert.That(otherSubsystem.HasItem(otherItem), "Other Item not added to subsystem");
			Assert.That(otherItem.IsOnSubsystem, "Other Item OnEnter CurrentSubsystem not set");
			Assert.That(otherItem.CurrentNode?.Equals(otherSubsystem) ?? false, "Other Item CurrentNode not set correctly");

			var playerState = player.GenerateState();
			Assert.That(playerState.InventoryItem.HasValue, Is.False, "PlayerState inventory should be null");

			// create pickup intent
			int itemLocationIndex;
			Assert.That(subsystem.Items.TryGetItemIndex(item, out itemLocationIndex), "Item not found on subsystem [TryGetIndex]");
			player.PickUpItem(item.ItemType, itemLocationIndex, subsystem);

			Assert.That(player.Intents.Peek(), Is.TypeOf(typeof(PickUpItemIntent)));

			var tick = 1;

			// test item before pickup
			Assert.That(item.HasOwner, Is.False, "Item has owner before pickup");
			Assert.That(item.CanBeDropped, Is.False, "Item reports can be dropped on current system");
			var itemState = item.GenerateState();
			Assert.That(itemState, Is.Not.Null, "Item returned null state");
			Assert.That(itemState.CurrentNode.Value, Is.EqualTo(subsystem.Id), "ItemState has wrong subsystem id");
			Assert.That(itemState.Owner.HasValue, Is.False, "ItemState return wrong owner");

			Assert.That(player.HasItem, Is.False, "Player already has item");
			Assert.That(player.IsOnSubsystem, "Player not on subsystem");

			// process pickup intent
			player.Tick(tick);
			Assert.That(player.HasItem, "Player does not have item after pickup");
			Assert.That(item.HasOwner, "Item should have owner");
			Assert.That(item.Owner.Id, Is.EqualTo(player.Id), "Item owner id incorrect");
			Assert.That(item.IsOnSubsystem, "Item should be on subsystem");
			itemState = item.GenerateState();
			Assert.That(itemState.CurrentNode.Value, Is.EqualTo(subsystem.Id), "ItemState has wrong subsystem id");

			Assert.That(itemState.Owner.Value, Is.EqualTo(player.Id), "ItemState owner id incorrect");
			Assert.That(subsystem.HasItem(item), "subsystem no longer has item");

			playerState = player.GenerateState();
			Assert.That(playerState.InventoryItem.Value, Is.EqualTo(item.Id), "PlayerState inventory item id incorrect");

			player.SetDestination(otherSubsystem);
			Assert.That(player.Intents.Peek(), Is.TypeOf(typeof(MoveIntent)));

			// simulate player leaving subsystem
			player.Tick(++tick);
			Assert.That(item.IsOnSubsystem, Is.False, "Item should have left subsystem");
			Assert.That(item.HasOwner, "Item should have owner");
			Assert.That(item.Owner.Id, Is.EqualTo(player.Id), "Item owner id incorrect");
			itemState = item.GenerateState();
			Assert.That(itemState.CurrentNode.HasValue, Is.False, "ItemState should have no subsystem id");
			Assert.That(itemState.Owner.Value, Is.EqualTo(player.Id), "ItemState owner id incorrect");

			playerState = player.GenerateState();
			Assert.That(playerState.InventoryItem.Value, Is.EqualTo(item.Id), "PlayerState inventory item id incorrect");

			// player arrives at destination
			otherSubsystem.AddVisitor(player, connection, 0);
			Assert.That(player.IsOnSubsystem, "Player should be on subsystem");
			Assert.That(player.CurrentSubsystem, Is.EqualTo(otherSubsystem), "Player should be on other subsystem");

			Assert.That(item.IsOnSubsystem, Is.False, "Item should not be on subsystem");
			Assert.That(item.HasOwner, "Item should have owner");
			Assert.That(item.Owner.Id, Is.EqualTo(player.Id), "Item owner id incorrect");
			itemState = item.GenerateState();
			Assert.That(itemState.CurrentNode.HasValue, Is.False, "ItemState should have no subsystem id");
			Assert.That(itemState.Owner.Value, Is.EqualTo(player.Id), "ItemState owner id incorrect");

			// process arrive at destination
			player.Tick(++tick);

			Assert.That(player.HasItem, "Player does not have item after pickup");
			Assert.That(item.HasOwner, "Item should have owner");
			Assert.That(item.Owner.Id, Is.EqualTo(player.Id), "Item owner id incorrect");
			Assert.That(item.IsOnSubsystem, "Item should be on subsystem");
			itemState = item.GenerateState();
			Assert.That(itemState.CurrentNode.Value, Is.EqualTo(otherSubsystem.Id), "ItemState has wrong subsystem id");
			Assert.That(itemState.Owner.Value, Is.EqualTo(player.Id), "ItemState owner id incorrect");
			Assert.That(otherSubsystem.HasItem(item), "subsystem no longer has item");

			playerState = player.GenerateState();
			Assert.That(playerState.InventoryItem.Value, Is.EqualTo(item.Id), "PlayerState inventory item id incorrect");

			// drop on current subsystem
			player.DisownItem();
			Assert.That(player.Intents.Peek(), Is.TypeOf(typeof(DisownItemIntent)));

			//process disown
			player.Tick(++tick);

			Assert.That(player.HasItem, Is.False, "Player has item after disown");
			Assert.That(item.HasOwner, Is.False, "Item should not have owner");
			Assert.That(item.IsOnSubsystem, "Item should be on subsystem");
			itemState = item.GenerateState();
			Assert.That(itemState.CurrentNode.Value, Is.EqualTo(otherSubsystem.Id), "ItemState has wrong subsystem id");
			Assert.That(itemState.Owner.HasValue, Is.False, "ItemState has owner after disown");
			Assert.That(otherSubsystem.HasItem(item), "subsystem no longer has item");

			playerState = player.GenerateState();
			Assert.That(playerState.InventoryItem.HasValue, Is.False, "PlayerState inventory item should be null");

			// test other item before pickup
			Assert.That(otherItem.HasOwner, Is.False, "Item has owner before pickup");
			Assert.That(otherItem.CanBeDropped, Is.False, "Item reports can be dropped on current system");
			var otherItemState = otherItem.GenerateState();
			Assert.That(otherItemState, Is.Not.Null, "Item returned null state");
			Assert.That(otherItemState.CurrentNode.Value, Is.EqualTo(otherSubsystem.Id), "ItemState has wrong subsystem id");
			Assert.That(otherItemState.Owner.HasValue, Is.False, "ItemState return wrong owner");

			Assert.That(player.HasItem, Is.False, "Player already has item");
			Assert.That(player.IsOnSubsystem, "Player not on subsystem");

			// create pickup intent
			Assert.That(otherSubsystem.Items.TryGetItemIndex(otherItem, out itemLocationIndex), "Item not found on subsystem [TryGetIndex]");
			player.PickUpItem(otherItem.ItemType, itemLocationIndex, otherSubsystem);

			Assert.That(player.Intents.Peek(), Is.TypeOf(typeof(PickUpItemIntent)));

			// process pickup intent
			player.Tick(++tick);

			Assert.That(player.HasItem, "Player does not have item after pickup");
			Assert.That(otherItem.HasOwner, "Item should have owner");
			Assert.That(otherItem.Owner.Id, Is.EqualTo(player.Id), "Item owner id incorrect");
			Assert.That(otherItem.IsOnSubsystem, "Item should be on subsystem");
			otherItemState = otherItem.GenerateState();
			Assert.That(otherItemState.CurrentNode.Value, Is.EqualTo(otherSubsystem.Id), "ItemState has wrong subsystem id");
			Assert.That(otherItemState.Owner.Value, Is.EqualTo(player.Id), "ItemState owner id incorrect");
			Assert.That(otherSubsystem.HasItem(otherItem), "subsystem no longer has item");

			playerState = player.GenerateState();
			Assert.That(playerState.InventoryItem.Value, Is.EqualTo(otherItem.Id), "PlayerState inventory item id incorrect");

			Assert.That(item.HasOwner, Is.False, "Item should not have owner");
			Assert.That(item.IsOnSubsystem, "Item should be on subsystem");
			itemState = item.GenerateState();
			Assert.That(itemState.CurrentNode.Value, Is.EqualTo(otherSubsystem.Id), "ItemState has wrong subsystem id");
			Assert.That(itemState.Owner.HasValue, Is.False, "ItemState has owner after disown");
			Assert.That(otherSubsystem.HasItem(item), "subsystem no longer has item");

			// pickup withcurrent item selected

			// create pickup intent
			Assert.That(otherSubsystem.Items.TryGetItemIndex(item, out itemLocationIndex), "Item not found on subsystem [TryGetIndex]");
			player.PickUpItem(item.ItemType, itemLocationIndex, otherSubsystem);

			Assert.That(player.Intents.Peek(), Is.TypeOf(typeof(PickUpItemIntent)));

			// process pickup intent
			player.Tick(++tick);
			
			Assert.That(player.HasItem, "Player does not have item after pickup");
			Assert.That(item.HasOwner, "Item should have owner");
			Assert.That(item.Owner.Id, Is.EqualTo(player.Id), "Item owner id incorrect");
			Assert.That(item.IsOnSubsystem, "Item should be on subsystem");
			itemState = item.GenerateState();
			Assert.That(itemState.CurrentNode.Value, Is.EqualTo(otherSubsystem.Id), "ItemState has wrong subsystem id");
			Assert.That(itemState.Owner.Value, Is.EqualTo(player.Id), "ItemState owner id incorrect");
			Assert.That(otherSubsystem.HasItem(item), "subsystem no longer has item");

			playerState = player.GenerateState();
			Assert.That(playerState.InventoryItem.Value, Is.EqualTo(item.Id), "PlayerState inventory item id incorrect");

			Assert.That(otherItem.HasOwner, Is.False, "Item should not have owner");
			Assert.That(otherItem.IsOnSubsystem, "Item should be on subsystem");
			otherItemState = otherItem.GetState() as ItemState;
			Assert.That(otherItemState.CurrentNode.Value, Is.EqualTo(otherSubsystem.Id), "ItemState has wrong subsystem id");
			Assert.That(otherItemState.Owner.HasValue, Is.False, "ItemState has owner after disown");
			Assert.That(otherSubsystem.HasItem(item), "subsystem no longer has item");
		}
	}
}
