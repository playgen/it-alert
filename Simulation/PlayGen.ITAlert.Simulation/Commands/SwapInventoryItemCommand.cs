using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Engine.Commands;
using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Commands
{
	[Deduplicate(DeduplicationPolicy.Replace)]
	public class SwapInventoryItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int SubsystemId { get; set; }

		public int? SubsystemItemId { get; set; }

		public int? InventoryItemId { get; set; }

		public int ContainerId { get; set; }
	}

	public class SwapInventoryItemCommandHandler : CommandHandler<SwapInventoryItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, IItemType> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		private readonly EventSystem _eventSystem;

		#region Overrides of CommandHandler<SwapInventoryItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new SwapInventoryItemCommandEqualityComparer();
		#endregion

		public SwapInventoryItemCommandHandler(IMatcherProvider matcherProvider, EventSystem eventSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(SwapInventoryItemCommand command, int currentTick, bool handlerEnabled)
		{
			ComponentEntityTuple<Item, Owner, CurrentLocation, IItemType> subsystemItemTuple = null;
			ComponentEntityTuple<Item, Owner, CurrentLocation, IItemType> inventoryItemTuple = null;

			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId,
					out var playerTuple)
				&& (!command.SubsystemItemId.HasValue
					|| _itemMatcherGroup.TryGetMatchingEntity(command.SubsystemItemId.Value,
						out subsystemItemTuple))
				&& (!command.InventoryItemId.HasValue
					|| _itemMatcherGroup.TryGetMatchingEntity(command.InventoryItemId.Value,
						out inventoryItemTuple))
				// Player must be on a subsystem
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value,
					out var subsystemTuple)
				// Must be same subsystem as when command was issued
				&& subsystemTuple.Entity.Id == command.SubsystemId)
			{
				var @event = new SwapInventoryItemCommandEvent()
				{
					InventoryItemId = inventoryItemTuple.Entity.Id,
					InventoryItemType = inventoryItemTuple.Component4.GetType().Name,
					SubsystemItemId = subsystemItemTuple.Entity.Id,
					SubsystemItemType = subsystemItemTuple.Component4.GetType().Name,
					PlayerEntityId = command.PlayerId,
					SubsystemEntityId = subsystemItemTuple.Component3.Value ?? -1
				};

				var target = subsystemTuple.Component2.Items[command.ContainerId];

				@event.TargetContainerType = target.GetType().Name;

				// No one must own the subsystem item, but the player must own the inventory item
				if (subsystemItemTuple?.Component2.Value == null)
				{
					if (inventoryItemTuple?.Component2.Value == playerTuple.Entity.Id || inventoryItemTuple?.Component2.Value == null)
					{
						var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
						var subsystemContainer = subsystemTuple.Component2.Items[command.ContainerId];

						// Items must still be in same locations as when the command was issued
						if (handlerEnabled
							&& subsystemContainer.Item == command.SubsystemItemId
							&& inventory.Item == command.InventoryItemId
							// Must be enabled
							&& subsystemContainer.Enabled
							&& inventory.Enabled
							// Must be able to release items
							&& (subsystemContainer.CanRelease || subsystemContainer.Item == null)
							&& (inventory.CanRelease || inventory.Item == null)
							// Containers must be able to accept items
							&& (!command.SubsystemItemId.HasValue || inventory.CanContain(command.SubsystemItemId.Value))
							&& (!command.InventoryItemId.HasValue || subsystemContainer.CanContain(command.InventoryItemId.Value)))
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

							@event.Result = SwapInventoryItemCommandEvent.ActivationResult.Success;
							_eventSystem.Publish(@event);
							return true;
						}
						else
						{
							@event.Result = handlerEnabled
										? !command.InventoryItemId.HasValue || subsystemContainer.CanContain(command.InventoryItemId.Value)
									? SwapInventoryItemCommandEvent.ActivationResult.Error
								: SwapInventoryItemCommandEvent.ActivationResult.Failure_DestinationCannotCapture 
							: SwapInventoryItemCommandEvent.ActivationResult.Failure_CommandDisabled;
							_eventSystem.Publish(@event);
						}
					}
					else
					{
						@event.Result = SwapInventoryItemCommandEvent.ActivationResult.Error;
						_eventSystem.Publish(@event);
					}
				}
				else
				{
					@event.Result = SwapInventoryItemCommandEvent.ActivationResult.Failure_ItemAlreadyActive;
					_eventSystem.Publish(@event);
				}
			}

			return false;
		}
	}

	public class SwapInventoryItemCommandEqualityComparer : CommandEqualityComparer<SwapInventoryItemCommand>
	{
		#region Overrides of CommandEqualityComparer<SwapInventoryItemCommand>

		protected override bool IsDuplicate(SwapInventoryItemCommand a, SwapInventoryItemCommand b)
		{
			return a.PlayerId == b.PlayerId
					&& (a.SubsystemItemId == b.SubsystemItemId
						|| a.SubsystemItemId == b.InventoryItemId
						|| a.InventoryItemId == b.SubsystemItemId
						|| a.InventoryItemId == b.InventoryItemId);
		}

		#endregion
	}

	public class SwapInventoryItemCommandEvent : Event, IPlayerEvent, ISubsystemEvent
	{
		public enum ActivationResult
		{
			Error = 0,
			Success,
			Failure_ItemAlreadyActive,
			Failure_DestinationCannotCapture,
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
