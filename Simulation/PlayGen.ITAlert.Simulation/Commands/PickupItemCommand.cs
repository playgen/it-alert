﻿using System.Linq;
using Engine.Commands;
using Engine.Components;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class PickupItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }
	}

	public class PickupItemCommandHandler : CommandHandler<PickupItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, Activation> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		public PickupItemCommandHandler(IMatcherProvider matcherProvider)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, Activation>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		protected override bool TryProcessCommand(PickupItemCommand command, int currentTick)
		{
			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out var playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple)
				&& itemTuple.Component2.Value.HasValue == false
				&& itemTuple.Component4.ActivationState == ActivationState.NotActive
				&& itemTuple.Component3.Value.HasValue
				&& itemTuple.Component3.Value == playerTuple.Component3.Value
				&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out var subsystemTuple))
			{
				var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
				var source = subsystemTuple.Component2.Items.SingleOrDefault(ic => ic.Item == itemTuple.Entity.Id);
				if (inventory != null && inventory.Item.HasValue == false
					&& source != null && source.CanRelease)
				{
					inventory.Item = itemTuple.Entity.Id;
					itemTuple.Component2.Value = playerTuple.Entity.Id;
					itemTuple.Component3.Value = null;
					source.Item = null;
					return true;
				}

			}
			return false;
		}
	}
}
