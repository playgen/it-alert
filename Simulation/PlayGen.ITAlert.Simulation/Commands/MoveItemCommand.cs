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
	public class MoveItemCommand : ICommand
	{
		public int SystemEntityId { get; set; }

		public int PlayerId { get; set; }

		public int ItemId { get; set; }

		public int SourceContainerId { get; set; }

		public int DestinationContainerId { get; set; }

	}

	public class MoveItemCommandHandler : CommandHandler<MoveItemCommand>
	{

		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, IItemType> _itemMatcherGroup;

		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		private readonly EventSystem _eventSystem;

		#region Overrides of CommandHandler<DropItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new MoveItemCommandEqualityComparer();

		#endregion

		public MoveItemCommandHandler(IMatcherProvider matcherProvider, EventSystem eventSystem)
		{
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(MoveItemCommand command, int currentTick, bool handlerEnabled)
		{
			if (_itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple)
				&& _subsystemMatcherGroup.TryGetMatchingEntity(command.SystemEntityId, out var subsystemTuple))
			{
				var @event = new MoveItemCommandEvent()
				{
					ItemId = itemTuple.Entity.Id,
					ItemType = itemTuple.Component4.GetType().Name,
					PlayerEntityId = command.PlayerId,
					SubsystemEntityId = itemTuple.Component3.Value ?? -1
				};

				var source = subsystemTuple.Component2.Items[command.SourceContainerId];
				var target = subsystemTuple.Component2.Items[command.DestinationContainerId];

				@event.TargetContainerType = target.GetType().Name;

				if (handlerEnabled
					&& itemTuple.Component2.Value == null
					&& source != null 
					&& source.Item == itemTuple.Entity.Id
					&& source.CanRelease
					&& target != null
					&& target.CanCapture(itemTuple.Entity.Id))
				{
					target.Item = itemTuple.Entity.Id;
					source.Item = null;

					@event.Result = MoveItemCommandEvent.ActivationResult.Success;
					_eventSystem.Publish(@event);
					return true;
				}
				@event.Result = handlerEnabled
								? itemTuple.Component2.Value == null
									? target.CanCapture(itemTuple.Entity.Id)
										? MoveItemCommandEvent.ActivationResult.Error
											: MoveItemCommandEvent.ActivationResult.Failure_DestinationCannotCapture
										: MoveItemCommandEvent.ActivationResult.Failure_ItemAlreadyActive
									: MoveItemCommandEvent.ActivationResult.Failure_CommandDisabled;
				_eventSystem.Publish(@event);
			}
			return false;
		}
	}

	public class MoveItemCommandEqualityComparer : CommandEqualityComparer<MoveItemCommand>
	{
		#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

		protected override bool IsDuplicate(MoveItemCommand x, MoveItemCommand other)
		{
			// player can only ever have one destination
			return x.PlayerId == other.PlayerId && x.ItemId == other.ItemId;
		}

		#endregion
	}

	public class MoveItemCommandEvent : Event, IPlayerEvent, ISubsystemEvent
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

		public int ItemId { get; set; }
		public string ItemType { get; set; }

		public string TargetContainerType { get; set; }
		public int PlayerEntityId { get; set; }
		public int SubsystemEntityId { get; set; }
	}
}
