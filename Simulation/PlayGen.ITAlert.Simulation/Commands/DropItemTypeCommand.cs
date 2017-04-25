using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Components;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Systems.Players;

namespace PlayGen.ITAlert.Simulation.Commands
{
	[Deduplicate(DeduplicationPolicy.Replace)]
	public class DropItemTypeCommand : ICommand
	{
		public int PlayerId { get; set; }

		public Type ItemType { get; set; }
	}

	public class DropItemTypeCommandHandler : CommandHandler<DropItemTypeCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Activation, CurrentLocation, Owner> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly PlayerSystem _playerSystem;

		public override IEqualityComparer<ICommand> Deduplicator => new DropItemTypeCommandqualityComparer();

		public DropItemTypeCommandHandler(IMatcherProvider matcherProvider, PlayerSystem playerSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Activation, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_playerSystem = playerSystem; 
		}

		protected override bool TryProcessCommand(DropItemTypeCommand command, int currentTick)
		{
			if (command.ItemType == null
				|| typeof(IItemType).IsAssignableFrom(command.ItemType) == false)
			{
				return false;
			}

			if (_playerSystem.TryGetPlayerEntityId(command.PlayerId, out var playerEntityId)
				&& _playerMatcherGroup.TryGetMatchingEntity(playerEntityId, out var playerTuple)
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out var subsystemTuple)
				&& subsystemTuple.Component2.TryGetEmptyContainer(out var target, out var containerIndex))
			{
				var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
				if (inventory != null
					&& inventory.Item.HasValue
					&& _itemMatcherGroup.TryGetMatchingEntity(inventory.Item.Value, out var inventoryItemTuple)
					&& inventoryItemTuple.Entity.TryGetComponent(command.ItemType, out var itemTypeComponent)
					&& target != null
					&& target.CanCapture(inventory.Item))
				{
					target.Item = inventoryItemTuple.Entity.Id;
					inventoryItemTuple.Component3.Value = subsystemTuple.Entity.Id;
					inventory.Item = null;
					inventoryItemTuple.Component4.Value = null;
					return true;
				}
			}
			return false;
		}
	}

	public class DropItemTypeCommandqualityComparer : CommandEqualityComparer<DropItemTypeCommand>
	{
		#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

		protected override bool IsDuplicate(DropItemTypeCommand x, DropItemTypeCommand other)
		{
			return x.PlayerId == other.PlayerId;
		}

		#endregion
	}
}
