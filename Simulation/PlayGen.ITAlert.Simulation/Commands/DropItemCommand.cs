using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Commands
{
	[Deduplicate(DeduplicationPolicy.Discard)]
	public class DropItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }

		public int ContainerId { get; set; }
	}

	public class DropItemCommandHandler : CommandHandler<DropItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, IItemType> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		private readonly EventSystem _eventSystem;

		#region Overrides of CommandHandler<DropItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new DropItemCommandEqualityComparer();

		#endregion

		public DropItemCommandHandler(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(DropItemCommand command, int currentTick, bool handlerEnabled)
		{
			if (handlerEnabled
				&& _playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out var playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple)
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out var subsystemTuple)
				&& itemTuple.Component2.Value == playerTuple.Entity.Id)
			{
				var @event = new DropItemEvent()
				{
					PlayerEntityId = playerTuple.Entity.Id,
					//ItemId = itemTuple.Entity.Id,
					ItemType = itemTuple.Component4.GetType().Name,
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

						@event.Result = DropItemEvent.ActivationResult.Success;
						_eventSystem.Publish(@event);

						return true;
					}
					else
					{
						@event.Result = DropItemEvent.ActivationResult.Failure_DestinationCannotCapture;
					}
				}
				else
				{
					@event.Result = DropItemEvent.ActivationResult.Failure_CommandDisabled;
				}
				_eventSystem.Publish(@event);
			}
			return false;
		}

		public class DropItemCommandEqualityComparer : CommandEqualityComparer<DropItemCommand>
		{
			#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

			protected override bool IsDuplicate(DropItemCommand x, DropItemCommand other)
			{
				// player can only ever have one destination
				return x.PlayerId == other.PlayerId && x.ItemId == other.ItemId;
			}

			#endregion
		}
	}

	public class DropItemEvent : Event, IPlayerEvent, ISubsystemEvent
	{
		public enum ActivationResult
		{
			Error = 0,
			Success,
			Failure_DestinationCannotCapture,
			Failure_CommandDisabled,
		}

		public ActivationResult Result { get; set; }

		public string ItemType { get; set; }

		public string TargetContainerType { get; set; }
		public int PlayerEntityId { get; set; }
		public int SubsystemEntityId { get; set; }
	}
}
