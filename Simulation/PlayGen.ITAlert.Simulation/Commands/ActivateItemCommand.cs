using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Engine.Commands;
using Engine.Components;
using Engine.Events;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Events;

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
		private readonly ComponentMatcherGroup<Item, Activation, CurrentLocation, Owner, IItemType> _activationMatcherGroup;
		// TODO: match subsystems on presence of activationcontainer once theyr are refactored into independent entities
		private readonly ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup;

		#region Overrides of CommandHandler<ActivateItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new ActivateItemCommandqualityComparer();

		#endregion

		private readonly EventSystem _eventSystem;

		public ActivateItemCommandHandler(IMatcherProvider matcherProvider, EventSystem eventSystem)
		{
			_activationMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Activation, CurrentLocation, Owner, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem>();
			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(ActivateItemCommand command, int currentTick, bool handlerEnabled)
		{
			if (_activationMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple))
			{
				var @event = new ActivateItemEvent() {
					ItemId = itemTuple.Entity.Id,
					ItemType = itemTuple.Component5.GetType().Name,
					PlayerEntityId = command.PlayerId,
				};

				var itemNotActive = itemTuple.Component2.ActivationState == ActivationState.NotActive;	// item is not active
				var playerCanActivate = (itemTuple.Component4.AllowAll || itemTuple.Component4.Value == null || itemTuple.Component4.Value == command.PlayerId); // player can activate item
				// TODO: should an item have to have a location to be activated?
				var playerHasActive = _activationMatcherGroup.MatchingEntities.Any(it => it.Component4.Value == command.PlayerId && it.Component2.ActivationState != ActivationState.NotActive);    // player has no other active items

				if (handlerEnabled
					&& itemNotActive
					&& playerCanActivate
					&& playerHasActive == false
					&& itemTuple.Component3.Value.HasValue // item is on a subsystem
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out var subsystemTuple))
				{
					itemTuple.Component2.SetState(ActivationState.Activating,
						currentTick);
					itemTuple.Component4.Value = command.PlayerId;

					@event.Result = ActivateItemEvent.ActivationResult.Success;
					_eventSystem.Publish(@event);
					return true;
				}
				@event.Result = handlerEnabled
					? itemNotActive
						? playerCanActivate
							? playerHasActive
								? ActivateItemEvent.ActivationResult.Failure_PlayerHasActive
								: ActivateItemEvent.ActivationResult.Error
							: ActivateItemEvent.ActivationResult.Failure_PlayerCannotActivate
						: ActivateItemEvent.ActivationResult.Failure_ItemAlreadyActive
					: ActivateItemEvent.ActivationResult.Failure_CommandDisabled;
				_eventSystem.Publish(@event);
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

	public class ActivateItemEvent : Event, IPlayerEvent, ISubsystemEvent
	{
		public enum ActivationResult
		{
			Error = 0,
			Success,
			Failure_ItemAlreadyActive,
			Failure_PlayerCannotActivate,
			Failure_PlayerHasActive,
			Failure_CommandDisabled,
		}

		public ActivationResult Result { get; set; }

		public int ItemId { get; set; }

		public string ItemType { get; set; }

		public int PlayerEntityId { get; set; }
		public int SubsystemEntityId { get; set; }
	}
}
