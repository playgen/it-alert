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
	public class SwapSubsystemItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int SubsystemId { get; set; }

		public int FromItemId { get; set; }

		public int? ToItemId { get; set; }

		public int FromContainerIndex { get; set; }

		public int ToContainerIndex { get; set; }
	}

	public class SwapSubsystemItemCommandHandler : CommandHandler<SwapSubsystemItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, IItemType> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		private readonly EventSystem _eventSystem;

		#region Overrides of CommandHandler<SwapSubsystemItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new SwapSubsystemItemCommandEqualityComparer();
		#endregion

		public SwapSubsystemItemCommandHandler(IMatcherProvider matcherProvider, EventSystem eventSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(SwapSubsystemItemCommand command, int currentTick, bool handlerEnabled)
		{
			ComponentEntityTuple<Item, Owner, CurrentLocation, IItemType> toItemTuple = null;

			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId,
					out var playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.FromItemId,
					out var fromItemTuple)
				&& (!command.ToItemId.HasValue
					|| _itemMatcherGroup.TryGetMatchingEntity(command.ToItemId.Value,
						out toItemTuple))
				// Player must be on a subsystem
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value,
					out var subsystemTuple)
				// Must be same subsystem as when command was issued
				&& subsystemTuple.Entity.Id == command.SubsystemId)
			{

				var @event = new SwapSubsystemItemCommandEvent()
				{
					FromItemId = fromItemTuple.Entity.Id,
					FromItemType = fromItemTuple.Component4.GetType().Name,
					ToItemId = toItemTuple?.Entity.Id ?? -1,
					ToItemType = toItemTuple?.Component4.GetType().Name,
					PlayerEntityId = command.PlayerId,
					SubsystemEntityId = fromItemTuple.Component3.Value ?? -1
				};

				// No one must own either item
				if (handlerEnabled && fromItemTuple.Component2.Value == null && toItemTuple?.Component2.Value == null)
				{
					var toContainer = subsystemTuple.Component2.Items[command.ToContainerIndex];
					var fromContainer = subsystemTuple.Component2.Items[command.FromContainerIndex];

					@event.FromContainerType = fromContainer.GetType().Name;
					@event.ToContainerType = toContainer.GetType().Name;

					// Items must still be in same locations as when the command was issued
					if (fromContainer.Item == command.FromItemId
						&& toContainer.Item == command.ToItemId
						// Must be enabled
						&& fromContainer.Enabled
						&& toContainer.Enabled
						// Must be able to release items
						&& (fromContainer.CanRelease || fromContainer.Item == null)
						&& (toContainer.CanRelease || toContainer.Item == null)
						// Containers must be able to accept items
						&& toContainer.CanContain(command.FromItemId) 
						&& (!command.ToItemId.HasValue || fromContainer.CanContain(command.ToItemId.Value)))
					{
						toContainer.Item = command.FromItemId;
						fromContainer.Item = command.ToItemId;

						@event.Result = SwapSubsystemItemCommandEvent.ActivationResult.Success;
						_eventSystem.Publish(@event);
						return true;
					}
					else
					{
						if (toContainer.CanContain(command.FromItemId)
							&& (!command.ToItemId.HasValue || fromContainer.CanContain(command.ToItemId.Value)))
						{
							@event.Result = SwapSubsystemItemCommandEvent.ActivationResult.Error;
							_eventSystem.Publish(@event);
						}
						else
						{
							@event.Result = SwapSubsystemItemCommandEvent.ActivationResult.Failure_DestinationCannotCapture;
							_eventSystem.Publish(@event);
						}
					}
				}
				else
				{
					if (handlerEnabled)
					{
						@event.Result = SwapSubsystemItemCommandEvent.ActivationResult.Failure_ItemAlreadyActive;
						_eventSystem.Publish(@event);
					}
					else
					{
						@event.Result = SwapSubsystemItemCommandEvent.ActivationResult.Failure_CommandDisabled;
						_eventSystem.Publish(@event);
					}
				}
			}

			return false;
		}
	}

	public class SwapSubsystemItemCommandEqualityComparer : CommandEqualityComparer<SwapSubsystemItemCommand>
	{
		#region Overrides of CommandEqualityComparer<SwapSubsystemItemCommand>

		protected override bool IsDuplicate(SwapSubsystemItemCommand a, SwapSubsystemItemCommand b)
		{
			return a.PlayerId == b.PlayerId
					&& (a.ToItemId == b.ToItemId
						|| a.ToItemId == b.FromItemId
						|| a.FromItemId == b.ToItemId
						|| a.FromItemId == b.FromItemId);
		}

		#endregion
	}

	public class SwapSubsystemItemCommandEvent : Event, IPlayerEvent, ISubsystemEvent
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

		public int FromItemId { get; set; }
		public string FromItemType { get; set; }
		public string FromContainerType { get; set; }

		public int ToItemId { get; set; }
		public string ToItemType { get; set; }
		public string ToContainerType { get; set; }

		public int PlayerEntityId { get; set; }
		public int SubsystemEntityId { get; set; }
	}
}
