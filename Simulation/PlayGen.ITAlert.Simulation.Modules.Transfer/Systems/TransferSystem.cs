using System.Linq;
using Engine.Components;
using Engine.Events;
using Engine.Systems;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Components;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Transfer.Systems
{
	public class TransferSystem : ITickableSystem
	{
		public const string AnalysisOutputArchetypeName = "Antivirus";
		
		private readonly ComponentMatcherGroup<Engine.Systems.Activation.Components.Activation, TransferActivator, CurrentLocation, Owner, TimedActivation> _transferActivatorMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, TransferEnhancement, ItemStorage> _transferSystemMatcherGroup;
		private readonly ComponentMatcherGroup<IItemType, CurrentLocation, Owner> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<IItemType, Engine.Systems.Activation.Components.Activation> _itemActivationMatcherGroup;

		private readonly EventSystem _eventSystem;

		public TransferSystem(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_transferActivatorMatcherGroup = matcherProvider.CreateMatcherGroup<Engine.Systems.Activation.Components.Activation, TransferActivator, CurrentLocation, Owner, TimedActivation>();
			_transferSystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, TransferEnhancement, ItemStorage>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<IItemType, CurrentLocation, Owner>();
			_itemActivationMatcherGroup = matcherProvider.CreateMatcherGroup<IItemType, Engine.Systems.Activation.Components.Activation>();

			_eventSystem = eventSystem;
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _transferActivatorMatcherGroup.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.NotActive:
						OnNotActive(match, currentTick);
						break;
					case ActivationState.Activating:
						OnActivating(match, currentTick);
						break;
					case ActivationState.Active:
						OnActive(match, currentTick);
						break;
					case ActivationState.Deactivating:
						OnDeactivating(match, currentTick);
						break;
				}
			}
		}

		public void OnNotActive(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, TransferActivator, CurrentLocation, Owner, TimedActivation> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& entityTuple.Component4.Value.HasValue)
			{
				entityTuple.Component4.Value = null;
			} 
		}

		public void OnActivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, TransferActivator, CurrentLocation, Owner, TimedActivation> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue // TODO: this should never fail so might be able to remove it
				&& _transferSystemMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var localTransferSystemTuple)
				&& entityTuple.Component4.Value.HasValue)
			{
				var otherTransferSystemTuple = _transferSystemMatcherGroup.MatchingEntities.SingleOrDefault(t => t.Entity.Id != localTransferSystemTuple.Entity.Id);

				if (otherTransferSystemTuple != null
					&& localTransferSystemTuple.Component3.TryGetItemContainer<TransferItemContainer>(out var localTransferItemContainer)
					&& otherTransferSystemTuple.Component3.TryGetItemContainer<TransferItemContainer>(out var remoterTransferItemContainer)
					&& otherTransferSystemTuple.Component3.TryGetItemContainer<TransferActivatorTargetItemContainer>(out var remoteTransferActivatorContainer)
					&& remoteTransferActivatorContainer.Item.HasValue
					&& _transferActivatorMatcherGroup.TryGetMatchingEntity(remoteTransferActivatorContainer.Item.Value, out var remoteTransferActivatorTuple))
				{
					localTransferItemContainer.Locked = true;
					remoterTransferItemContainer.Locked = true;
					remoteTransferActivatorTuple.Component1.SetState(ActivationState.Active, currentTick);
					// TODO: find a better way of doing this
					remoteTransferActivatorTuple.Component5.Synchronized = true;
					remoteTransferActivatorTuple.Component5.ActivationTicksRemaining = remoteTransferActivatorTuple.Component5.ActivationDuration;
				}
				else
				{
					entityTuple.Component1.SetState(ActivationState.NotActive, currentTick);
				}
			}
			else
			{
				entityTuple.Component1.SetState(ActivationState.NotActive, currentTick);
			}
		}

		public void OnActive(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, TransferActivator, CurrentLocation, Owner, TimedActivation> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue // TODO: this should never fail so might be able to remove it
				&& _transferSystemMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value,
					out var localTransferSystemTuple)
				&& entityTuple.Component4.Value.HasValue)
			{
				var otherTransferSystemTuple =
					_transferSystemMatcherGroup.MatchingEntities.SingleOrDefault(
						t => t.Entity.Id != localTransferSystemTuple.Entity.Id);

				if (otherTransferSystemTuple != null
					&& otherTransferSystemTuple.Component3.TryGetItemContainer<TransferActivatorTargetItemContainer>(out var remoteTransferActivatorContainer)
					&& remoteTransferActivatorContainer.Item.HasValue
					&& _transferActivatorMatcherGroup.TryGetMatchingEntity(remoteTransferActivatorContainer.Item.Value, out var remoteTransferActivatorTuple))
				{
					remoteTransferActivatorTuple.Component5.ActivationTicksRemaining = entityTuple.Component5.ActivationTicksRemaining;
				}
			}
		}


		public void OnDeactivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, TransferActivator, CurrentLocation, Owner, TimedActivation> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& _transferSystemMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var localTransferSystemTuple) // get the subsystem with the transfer activator
				&& entityTuple.Component4.Value.HasValue) // transfer activator is owned
			{
				var otherTransferSystemTuple = _transferSystemMatcherGroup.MatchingEntities.SingleOrDefault(t => t.Entity.Id != localTransferSystemTuple.Entity.Id);

				if (otherTransferSystemTuple != null
					&& localTransferSystemTuple.Component3.TryGetItemContainer<TransferItemContainer>(out var localTransferItemContainer)
					&& otherTransferSystemTuple.Component3.TryGetItemContainer<TransferItemContainer>(out var remoteTransferItemContainer)
					&& otherTransferSystemTuple.Component3.TryGetItemContainer<TransferActivatorTargetItemContainer>(out var remoteTransferActivatorContainer)
					&& remoteTransferActivatorContainer.Item.HasValue
					&& _transferActivatorMatcherGroup.TryGetMatchingEntity(remoteTransferActivatorContainer.Item.Value, out var remoteTransferActivatorTuple))
				{
					var @event = new TransferActivationEvent()
					{
						PlayerEntityId = entityTuple.Component4.Value.Value,
						SubsystemEntityId = entityTuple.Component3.Value.Value,
					};

					localTransferItemContainer.Locked = false;
					remoteTransferItemContainer.Locked = false;
					remoteTransferActivatorTuple.Component1.SetState(ActivationState.NotActive, currentTick);
					remoteTransferActivatorTuple.Component5.Synchronized = false;

					// local transfer push
					ComponentEntityTuple<IItemType, CurrentLocation, Owner> localTransferItemTuple = null;
					var push = false;

					if (localTransferItemContainer.Item.HasValue
						&& _itemMatcherGroup.TryGetMatchingEntity(localTransferItemContainer.Item.Value, out localTransferItemTuple))	// get local item
					{
						@event.LocalItemEntityId = localTransferItemContainer.Item;
						push = true;

						if (_itemActivationMatcherGroup.TryGetMatchingEntity(localTransferItemTuple.Entity.Id, out var localTransferItemActivationTuple))
						{
							localTransferItemActivationTuple.Component2.SetState(ActivationState.NotActive, currentTick);
						}
						localTransferItemTuple.Component2.Value = remoteTransferActivatorTuple.Component3.Value;
						localTransferItemTuple.Component3.Value = null;
					}

					// remote transfer pull
					ComponentEntityTuple<IItemType, CurrentLocation, Owner> remoteTransferItemTuple = null;
					var pull = false;

					if (remoteTransferItemContainer.Item.HasValue
						&& _itemMatcherGroup.TryGetMatchingEntity(remoteTransferItemContainer.Item.Value, out remoteTransferItemTuple))
					{
						@event.RemoteItemEntityId = remoteTransferItemContainer.Item;
						pull = true;

						if (_itemActivationMatcherGroup.TryGetMatchingEntity(remoteTransferItemTuple.Entity.Id, out var remoteTransferItemActivationTuple))
						{
							remoteTransferItemActivationTuple.Component2.SetState(ActivationState.NotActive, currentTick);
						}
						remoteTransferItemTuple.Component2.Value = entityTuple.Component3.Value;
						remoteTransferItemTuple.Component3.Value = null;
					}

					// event handling
					@event.ActivationResult = (push && pull)
						? TransferActivationEvent.TransferActivationResult.SwappedItems
						: push
							? TransferActivationEvent.TransferActivationResult.PushedItem
							: pull
								? TransferActivationEvent.TransferActivationResult.PulledItem
								: TransferActivationEvent.TransferActivationResult.NoItemsPresent;
					_eventSystem.Publish(@event);

					localTransferItemContainer.Item = remoteTransferItemTuple?.Entity.Id;
					remoteTransferItemContainer.Item = localTransferItemTuple?.Entity.Id;
				}
			}
		}

		public void Dispose()
		{
			_transferActivatorMatcherGroup?.Dispose();
			_transferSystemMatcherGroup?.Dispose();
			_itemMatcherGroup?.Dispose();
			_itemActivationMatcherGroup?.Dispose();
		}
	}
}
