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
	public class SwapInventoryItemAndActivateCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int SubsystemId { get; set; }

		public int? SubsystemItemId { get; set; }

		public int InventoryItemId { get; set; }

		public int ContainerId { get; set; }
	}

	public class SwapInventoryItemAndActivateCommandHandler : CommandHandler<SwapInventoryItemAndActivateCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, IItemType> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Activation, CurrentLocation, Owner, IItemType> _activationMatcherGroup;

		private readonly EventSystem _eventSystem;

		#region Overrides of CommandHandler<SwapInventoryItemAndActivateCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new SwapInventoryItemAndActivateCommandEqualityComparer();

		#endregion

		public SwapInventoryItemAndActivateCommandHandler(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_activationMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Activation, CurrentLocation, Owner, IItemType>();
			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(SwapInventoryItemAndActivateCommand command, int currentTick, bool handlerEnabled)
		{
			ComponentEntityTuple<Item, Owner, CurrentLocation, IItemType> subsystemItemTuple = null;
			ComponentEntityTuple<Item, Owner, CurrentLocation, IItemType> inventoryItemTuple = null;

			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId,
					out var playerTuple)
				&& (!command.SubsystemItemId.HasValue
					|| _itemMatcherGroup.TryGetMatchingEntity(command.SubsystemItemId.Value,
						out subsystemItemTuple))
				&& _itemMatcherGroup.TryGetMatchingEntity(command.InventoryItemId,
						out inventoryItemTuple)
				// Player must be on a subsystem
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value,
					out var subsystemTuple)
				// Must be same subsystem as when command was issued
				&& subsystemTuple.Entity.Id == command.SubsystemId)
			{
				// No one must own the subsystem item, but the player must own the inventory item
				if (subsystemItemTuple?.Component2.Value == null && (inventoryItemTuple?.Component2.Value == playerTuple.Entity.Id || inventoryItemTuple?.Component2.Value == null))
				{
					var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
					var subsystemContainer = subsystemTuple.Component2.Items[command.ContainerId];

					// Items must still be in same locations as when the command was issued
					if (subsystemContainer.Item == command.SubsystemItemId
						&& inventory.Item == command.InventoryItemId
						// Must be enabled
						&& subsystemContainer.Enabled
						&& inventory.Enabled
						// Must be able to release items
						&& (subsystemContainer.CanRelease || subsystemContainer.Item == null)
						&& (inventory.CanRelease || inventory.Item == null)
						// Containers must be able to accept items
						&& (!command.SubsystemItemId.HasValue || inventory.CanContain(command.SubsystemItemId.Value)))
					{
						inventory.Item = command.SubsystemItemId;
						if (inventoryItemTuple != null)
						{
							inventoryItemTuple.Component3.Value = subsystemTuple.Entity.Id;
							inventoryItemTuple.Component2.Value = null;
						}
						
						subsystemContainer.Item = command.InventoryItemId;
						if (subsystemItemTuple != null)
						{
							subsystemItemTuple.Component2.Value = playerTuple.Entity.Id;
							subsystemItemTuple.Component3.Value = null;
						}

						if (_activationMatcherGroup.TryGetMatchingEntity(command.InventoryItemId, out var itemTuple))
						{
							var @event = new SwapInventoryItemAndActivateEvent()
							{
								InventoryItemId = itemTuple.Entity.Id,
								InventoryItemType = itemTuple.Component5.GetType().Name,
								SubsystemItemId = subsystemItemTuple.Entity.Id,
								SubsystemItemType = subsystemItemTuple.Component4.GetType().Name,
								PlayerEntityId = command.PlayerId,
								SubsystemEntityId = itemTuple.Component3.Value ?? -1
							};

							var itemNotActive = itemTuple.Component2.ActivationState == ActivationState.NotActive;  // item is not active
							var playerCanActivate = (itemTuple.Component4.AllowAll || itemTuple.Component4.Value == null || itemTuple.Component4.Value == command.PlayerId); // player can activate item
							// TODO: should an item have to have a location to be activated?
							var playerHasActive = _activationMatcherGroup.MatchingEntities.Any(it => it.Component4.Value == command.PlayerId && it.Component2.ActivationState != ActivationState.NotActive);    // player has no other active items
							var subsystemCanContain = subsystemContainer.CanContain(command.InventoryItemId);
							var target = subsystemTuple.Component2.Items[command.ContainerId];

							@event.TargetContainerType = target.GetType().Name;

							if (handlerEnabled
								&& itemNotActive
								&& playerCanActivate
								&& subsystemCanContain
								&& playerHasActive == false
								&& itemTuple.Component3.Value.HasValue // item is on a subsystem
								&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out var _))
							{
								itemTuple.Component2.SetState(ActivationState.Activating,
									currentTick);
								itemTuple.Component4.Value = command.PlayerId;

								@event.Result = SwapInventoryItemAndActivateEvent.ActivationResult.Success;
								_eventSystem.Publish(@event);
								return true;
							}

							inventory.Item = command.InventoryItemId;
							if (inventoryItemTuple != null)
							{
								inventoryItemTuple.Component2.Value = playerTuple.Entity.Id;
								inventoryItemTuple.Component3.Value = null;
							}

							subsystemContainer.Item = command.SubsystemItemId;
							if (subsystemItemTuple != null)
							{
								subsystemItemTuple.Component3.Value = subsystemTuple.Entity.Id;
								subsystemItemTuple.Component2.Value = null;
							}

							@event.Result = handlerEnabled
								? itemNotActive
									? playerCanActivate
										? subsystemCanContain
											? playerHasActive
												? SwapInventoryItemAndActivateEvent.ActivationResult.Failure_PlayerHasActive
												: SwapInventoryItemAndActivateEvent.ActivationResult.Error
											: SwapInventoryItemAndActivateEvent.ActivationResult.Failure_DestinationCannotCapture
										: SwapInventoryItemAndActivateEvent.ActivationResult.Failure_PlayerCannotActivate
									: SwapInventoryItemAndActivateEvent.ActivationResult.Failure_ItemAlreadyActive
								: SwapInventoryItemAndActivateEvent.ActivationResult.Failure_CommandDisabled;
							_eventSystem.Publish(@event);
						}
						return false;
					}
				}
			}

			return false;
		}

		public class SwapInventoryItemAndActivateCommandEqualityComparer : CommandEqualityComparer<SwapInventoryItemAndActivateCommand>
		{
			#region Overrides of CommandEqualityComparer<SwapInventoryItemAndActivateCopmmand>

			protected override bool IsDuplicate(SwapInventoryItemAndActivateCommand a, SwapInventoryItemAndActivateCommand b)
			{
				// player can only ever have one destination
				return a.PlayerId == b.PlayerId
						&& (a.SubsystemItemId == b.SubsystemItemId
							|| a.SubsystemItemId == b.InventoryItemId
							|| a.InventoryItemId == b.SubsystemItemId
							|| a.InventoryItemId == b.InventoryItemId);
			}

			#endregion
		}
	}

	public class SwapInventoryItemAndActivateEvent : Event, IPlayerEvent, ISubsystemEvent
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

		public int InventoryItemId { get; set; }
		public string InventoryItemType { get; set; }

		public int SubsystemItemId { get; set; }
		public string SubsystemItemType { get; set; }

		public string TargetContainerType { get; set; }
		public int PlayerEntityId { get; set; }
		public int SubsystemEntityId { get; set; }
	}
}
