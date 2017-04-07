using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Components;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	[Deduplicate(DeduplicationPolicy.Discard)]
	public class ActivateItemTypeCommand : ICommand
	{
		public int? PlayerId { get; set; }

		public Type ItemType { get; set; }

		public int? LocationEntityId { get; set; }
	}

	public class ActivateItemTypeCommandHandler : CommandHandler<ActivateItemTypeCommand>
	{
		private readonly ComponentMatcherGroup<Item, Activation, CurrentLocation, Owner> _activationMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<Player, CurrentLocation> _playerMatcherGroup;

		public override IEqualityComparer<ICommand> Deduplicator => new ActivateItemTypeCommandqualityComparer();

		public ActivateItemTypeCommandHandler(IMatcherProvider matcherProvider)
		{
			_activationMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Activation, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, CurrentLocation>();
		}

		protected override bool TryProcessCommand(ActivateItemTypeCommand command)
		{
			if (command.ItemType == null 
				|| typeof(IItemType).IsAssignableFrom(command.ItemType) == false
				|| (command.PlayerId.HasValue == false && command.LocationEntityId.HasValue == false))
			{
				return false;
			}
			if (command.PlayerId.HasValue && command.LocationEntityId.HasValue == false
				&& _playerMatcherGroup.TryGetMatchingEntity(command.PlayerId.Value, out var playerTuple)
				&& playerTuple.Component2.Value.HasValue)
			{
				command.LocationEntityId = playerTuple.Component2.Value;
			}

			if (command.LocationEntityId.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(command.LocationEntityId.Value, out var subsystemTuple))
			{
				foreach(var ic in subsystemTuple.Component2.Items.Where(ic => ic?.Item != null))
				{
					if (_activationMatcherGroup.TryGetMatchingEntity(ic.Item.Value, out var itemTuple)
						&& itemTuple.Entity.TryGetComponent(command.ItemType, out var itemComponent) // item is of correct type
						&& itemTuple.Component2.ActivationState == ActivationState.NotActive // item is not active
						&& (itemTuple.Component4.AllowAll || itemTuple.Component4.Value == null || itemTuple.Component4.Value == command.PlayerId) // player can activate item
						&& _activationMatcherGroup.MatchingEntities.Any(it => it.Component4.Value == command.PlayerId
							&& it.Component2.ActivationState != ActivationState.NotActive) == false) // player has no other active items
					{
						itemTuple.Component2.Activate();
						itemTuple.Component4.Value = command.PlayerId;
						return true;
					}
				}
			}
			return false;
		}
	}

	public class ActivateItemTypeCommandqualityComparer : CommandEqualityComparer<ActivateItemTypeCommand>
	{
		#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

		protected override bool IsDuplicate(ActivateItemTypeCommand x, ActivateItemTypeCommand other)
		{
			return x.PlayerId == other.PlayerId 
				&& x.ItemType == other.ItemType
				&& x.LocationEntityId == other.LocationEntityId;
		}

		#endregion
	}
}
