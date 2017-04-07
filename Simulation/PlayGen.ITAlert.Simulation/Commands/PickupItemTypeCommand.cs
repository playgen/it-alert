using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Components;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	[Deduplicate(DeduplicationPolicy.Replace)]
	public class PickupItemTypeCommand : ICommand
	{
		public int PlayerId { get; set; }

		public Type ItemType { get; set; }
	}

	public class PickupItemTypeCommandHandler : CommandHandler<PickupItemTypeCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Activation, CurrentLocation, Owner> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		public override IEqualityComparer<ICommand> Deduplicator => new PickupItemTypeCommandqualityComparer();

		public PickupItemTypeCommandHandler(IMatcherProvider matcherProvider)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Activation, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		protected override bool TryProcessCommand(PickupItemTypeCommand command)
		{
			if (command.ItemType == null
				|| typeof(IItemType).IsAssignableFrom(command.ItemType) == false)
			{
				return false;
			}

			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out var playerTuple)
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out var subsystemTuple))
			{
				foreach (var ic in subsystemTuple.Component2.Items.Where(ic => ic?.Item != null))
				{
					if (_itemMatcherGroup.TryGetMatchingEntity(ic.Item.Value, out var itemTuple)
						&& itemTuple.Entity.TryGetComponent(command.ItemType, out var itemComponent) // item is of correct type
						&& itemTuple.Component2.ActivationState == ActivationState.NotActive // item is not active
						&& (itemTuple.Component4.AllowAll || itemTuple.Component4.Value == null || itemTuple.Component4.Value == command.PlayerId) // player can activate item
						&& _itemMatcherGroup.MatchingEntities.Any(it => it.Component4.Value == command.PlayerId
							&& it.Component2.ActivationState != ActivationState.NotActive) == false) // player has no other active items
					{
						var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
						if (inventory != null 
							&& inventory.Item.HasValue == false
							&& ic.CanRelease)
						{
							inventory.Item = itemTuple.Entity.Id;
							itemTuple.Component4.Value = playerTuple.Entity.Id;
							itemTuple.Component3.Value = null;
							ic.Item = null;
							return true;
						}
					}
				}

			}
			return false;
		}
	}

	public class PickupItemTypeCommandqualityComparer : CommandEqualityComparer<PickupItemTypeCommand>
	{
		#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

		protected override bool IsDuplicate(PickupItemTypeCommand x, PickupItemTypeCommand other)
		{
			return x.PlayerId == other.PlayerId;
		}

		#endregion
	}
}
