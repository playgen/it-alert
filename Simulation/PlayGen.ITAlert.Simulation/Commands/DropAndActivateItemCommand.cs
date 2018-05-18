using System.Collections.Generic;
using System.Linq;
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
	public class DropAndActivateItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }

		public int ContainerId { get; set; }
	}

	public class DropAndActivateItemCommandHandler : CommandHandler<DropAndActivateItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, IItemType> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Activation, CurrentLocation, Owner, IItemType> _activationMatcherGroup;

		private readonly EventSystem _eventSystem;

		#region Overrides of CommandHandler<DropAndActivateItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new DropAndActivateItemCommandEqualityComparer();

		#endregion

		public DropAndActivateItemCommandHandler(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_activationMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Activation, CurrentLocation, Owner, IItemType>();
			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(DropAndActivateItemCommand command, int currentTick, bool handlerEnabled)
		{
			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out var playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple)
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out var subsystemTuple)
				&& itemTuple.Component2.Value == playerTuple.Entity.Id)
			{
				var @event = new DropAndActivateItemEvent()
				{
					PlayerEntityId = playerTuple.Entity.Id,
					ItemId = itemTuple.Entity.Id,
					ItemType = itemTuple.Component4.GetType().Name,
					SubsystemEntityId = itemTuple.Component3.Value ?? -1
				};

				var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
				var target = subsystemTuple.Component2.Items[command.ContainerId];

				@event.TargetContainerType = target.GetType().Name;
				if (handlerEnabled)
				{
					if (inventory != null
						&& inventory.Item == itemTuple.Entity.Id
						&& target != null
						&& target.CanCapture(itemTuple.Entity.Id))
					{
						target.Item = itemTuple.Entity.Id;
						itemTuple.Component3.Value = subsystemTuple.Entity.Id;
						inventory.Item = null;
						itemTuple.Component2.Value = null;

						if (_activationMatcherGroup.TryGetMatchingEntity(command.ItemId, out var activeItemTuple))
						{
							var itemNotActive = activeItemTuple.Component2.ActivationState == ActivationState.NotActive;  // item is not active
							var playerCanActivate = (activeItemTuple.Component4.AllowAll || activeItemTuple.Component4.Value == null || activeItemTuple.Component4.Value == command.PlayerId); // player can activate item
																																															   // TODO: should an item have to have a location to be activated?
							var playerHasActive = _activationMatcherGroup.MatchingEntities.Any(it => it.Component4.Value == command.PlayerId && it.Component2.ActivationState != ActivationState.NotActive);    // player has no other active items

							if (itemNotActive
								&& playerCanActivate
								&& playerHasActive == false
								&& activeItemTuple.Component3.Value.HasValue // item is on a subsystem
								&& _subsystemMatcherGroup.TryGetMatchingEntity(activeItemTuple.Component3.Value.Value, out var _))
							{
								activeItemTuple.Component2.SetState(ActivationState.Activating,
									currentTick);
								activeItemTuple.Component4.Value = command.PlayerId;

								@event.Result = DropAndActivateItemEvent.ActivationResult.Success;
								_eventSystem.Publish(@event);
								return true;
							}

							inventory.Item = itemTuple.Entity.Id;
							itemTuple.Component2.Value = playerTuple.Entity.Id;
							itemTuple.Component3.Value = null;
							target.Item = null;

							@event.Result = itemNotActive
									? playerCanActivate
										? playerHasActive
											? DropAndActivateItemEvent.ActivationResult.Failure_PlayerHasActive
											: DropAndActivateItemEvent.ActivationResult.Error
										: DropAndActivateItemEvent.ActivationResult.Failure_PlayerCannotActivate
									: DropAndActivateItemEvent.ActivationResult.Failure_ItemAlreadyActive;
							_eventSystem.Publish(@event);
						}
					}
					else
					{
						@event.Result = DropAndActivateItemEvent.ActivationResult.Failure_DestinationCannotCapture;
						_eventSystem.Publish(@event);
					}
				}
				else
				{
					@event.Result = DropAndActivateItemEvent.ActivationResult.Failure_CommandDisabled;
					_eventSystem.Publish(@event);
				}
			}
			return false;
		}

		public class DropAndActivateItemCommandEqualityComparer : CommandEqualityComparer<DropAndActivateItemCommand>
		{
			#region Overrides of CommandEqualityComparer<DropAndActivateItemCopmmand>

			protected override bool IsDuplicate(DropAndActivateItemCommand x, DropAndActivateItemCommand other)
			{
				// player can only ever have one destination
				return x.PlayerId == other.PlayerId && x.ItemId == other.ItemId;
			}

			#endregion
		}
	}

	public class DropAndActivateItemEvent : Event, IPlayerEvent, ISubsystemEvent
	{
		public enum ActivationResult
		{
			Error = 0,
			Success,
			Failure_DestinationCannotCapture,
			Failure_ItemAlreadyActive,
			Failure_PlayerCannotActivate,
			Failure_PlayerHasActive,
			Failure_CommandDisabled,
		}

		public ActivationResult Result { get; set; }

		public int ItemId { get; set; }

		public string ItemType { get; set; }

		public string TargetContainerType { get; set; }
		public int PlayerEntityId { get; set; }
		public int SubsystemEntityId { get; set; }
	}
}
