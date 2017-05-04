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
	public class PickupItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }
	}

	public class PickupItemCommandHandler : CommandHandler<PickupItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, Activation, IItemType> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		private readonly EventSystem _eventSystem;

		public PickupItemCommandHandler(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, Activation, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();

			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(PickupItemCommand command, int currentTick, bool handlerEnabled)
		{
			if (handlerEnabled
				&& _playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out var playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple)
				&& itemTuple.Component2.Value.HasValue == false
				&& itemTuple.Component4.ActivationState == ActivationState.NotActive
				&& itemTuple.Component3.Value.HasValue
				&& itemTuple.Component3.Value == playerTuple.Component3.Value
				&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out var subsystemTuple))
			{
				var @event = new PickupItemEvent()
				{
					PlayerEntityId = command.PlayerId,
					ItemId = itemTuple.Entity.Id,
					ItemType = itemTuple.Component5.GetType().Name,
				};

				var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
				var source = subsystemTuple.Component2.Items.SingleOrDefault(ic => ic.Item == itemTuple.Entity.Id);
				if (inventory != null && inventory.Item.HasValue == false
					&& source != null && source.CanRelease)
				{
					inventory.Item = itemTuple.Entity.Id;
					itemTuple.Component2.Value = playerTuple.Entity.Id;
					itemTuple.Component3.Value = null;
					source.Item = null;

					@event.Result = PickupItemEvent.ActivationResult.Success;
					_eventSystem.Publish(@event);
					return true;
				}

			}
			return false;
		}
	}

	public class PickupItemEvent : Event, IPlayerEvent, ISubsystemEvent
	{
		public enum ActivationResult
		{
			Error = 0,
			Success,
			Failure_NotOwner,
			Failure_ItemActive,
			Failure_PlayerHasInventory,
			Failure_CommandDisabled,
		}

		public ActivationResult Result { get; set; }

		public int ItemId { get; set; }

		public string ItemType { get; set; }
		public int PlayerEntityId { get; set; }
		public int SubsystemEntityId { get; set; }
	}
}
