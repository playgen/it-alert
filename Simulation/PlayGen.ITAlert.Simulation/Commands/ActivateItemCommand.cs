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
	[Deduplicate(DeduplicationPolicy.Discard)]
	public class ActivateItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }
	}

	public class ActivateItemCommandHandler : CommandHandler<ActivateItemCommand>
	{
		private readonly ComponentMatcherGroup<Item, Activation, CurrentLocation, Owner> _activationMatcherGroup;
		// TODO: match subsystems on presence of activationcontainer once theyr are refactored into independent entities
		private readonly ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup;

		#region Overrides of CommandHandler<ActivateItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new ActivateItemCommandqualityComparer();

		#endregion

		public ActivateItemCommandHandler(IMatcherProvider matcherProvider)
		{
			_activationMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Activation, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem>();
		}

		protected override bool TryProcessCommand(ActivateItemCommand command, int currentTick)
		{
			if (_activationMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple)
				&& itemTuple.Component2.ActivationState == ActivationState.NotActive	// item is not active
				&& (itemTuple.Component4.AllowAll || itemTuple.Component4.Value == null || itemTuple.Component4.Value == command.PlayerId) // player can activate item
				// TODO: should an item have to have a location to be activated?
				&& _activationMatcherGroup.MatchingEntities.Any(it => it.Component4.Value == command.PlayerId && it.Component2.ActivationState != ActivationState.NotActive) == false	// player has no other active items
				&& itemTuple.Component3.Value.HasValue	// item is on a subsystem
				&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out var subsystemTuple))
			{
				itemTuple.Component2.SetState(ActivationState.Activating, currentTick);
				itemTuple.Component4.Value = command.PlayerId;
				return true;
			}
			return false;
		}
	}

	public class ActivateItemCommandqualityComparer : CommandEqualityComparer<ActivateItemCommand>
	{
		#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

		protected override bool IsDuplicate(ActivateItemCommand x, ActivateItemCommand other)
		{
			return x.PlayerId == other.PlayerId 
				&& x.ItemId == other.ItemId;
		}

		#endregion
	}
}
