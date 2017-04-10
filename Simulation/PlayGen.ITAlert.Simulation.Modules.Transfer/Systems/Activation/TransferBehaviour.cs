using System.Linq;
using Engine.Components;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Transfer.Systems.Activation
{
	public class TransferBehaviour : IActivationExtension
	{
		public const string AnalysisOutputArchetypeName = "Antivirus";
		
		private readonly ComponentMatcherGroup<TransferActivator, CurrentLocation, Owner, Engine.Systems.Activation.Components.Activation, TimedActivation> _transferActivatorMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, TransferEnhancement, ItemStorage> _transferSystemMatcherGroup;
		private readonly ComponentMatcherGroup<IItemType, CurrentLocation, Owner> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<IItemType, Engine.Systems.Activation.Components.Activation> _itemActivationMatcherGroup;

		public TransferBehaviour(IMatcherProvider matcherProvider)
		{
			_transferActivatorMatcherGroup = matcherProvider.CreateMatcherGroup<TransferActivator, CurrentLocation, Owner, Engine.Systems.Activation.Components.Activation, TimedActivation>();
			_transferSystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, TransferEnhancement, ItemStorage>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<IItemType, CurrentLocation, Owner>();
			_itemActivationMatcherGroup = matcherProvider.CreateMatcherGroup<IItemType, Engine.Systems.Activation.Components.Activation>();
		}

		public void OnNotActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_transferActivatorMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple)
				&& itemTuple.Component2.Value.HasValue
				&& itemTuple.Component3.Value.HasValue)
			{
				itemTuple.Component3.Value = null;
			} 
		}

		public void OnActivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_transferActivatorMatcherGroup.TryGetMatchingEntity(itemId, out var localTransferActivatorTuple))
			{
				if (localTransferActivatorTuple.Component2.Value.HasValue // TODO: this should never fail so might be able to remove it
					&& _transferSystemMatcherGroup.TryGetMatchingEntity(localTransferActivatorTuple.Component2.Value.Value, out var localTransferSystemTuple)
					&& localTransferActivatorTuple.Component3.Value.HasValue)
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
						remoteTransferActivatorTuple.Component4.SetState(ActivationState.Active);
						// TODO: find a better way of doing this
						remoteTransferActivatorTuple.Component5.ActivationTicksRemaining = remoteTransferActivatorTuple.Component5.ActivationDuration;
					}
					else
					{
						activation.CancelActivation();
					}
				}
				else
				{
					activation.CancelActivation();
				}
			}

		}

		public void OnActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_transferActivatorMatcherGroup.TryGetMatchingEntity(itemId, out var localTransferActivatorTuple))
			{

				if (localTransferActivatorTuple.Component2.Value.HasValue
					&& _transferSystemMatcherGroup.TryGetMatchingEntity(localTransferActivatorTuple.Component2.Value.Value, out var localTransferSystemTuple)
					&& localTransferActivatorTuple.Component3.Value.HasValue)
				{
					var otherTransferSystemTuple = _transferSystemMatcherGroup.MatchingEntities.SingleOrDefault(t => t.Entity.Id != localTransferSystemTuple.Entity.Id);

					if (otherTransferSystemTuple != null
						&& localTransferSystemTuple.Component3.TryGetItemContainer<TransferItemContainer>(out var localTransferItemContainer)
						&& otherTransferSystemTuple.Component3.TryGetItemContainer<TransferItemContainer>(out var remoteTransferItemContainer)
						&& otherTransferSystemTuple.Component3.TryGetItemContainer<TransferActivatorTargetItemContainer>(out var remoteTransferActivatorContainer)
						&& remoteTransferActivatorContainer.Item.HasValue
						&& _transferActivatorMatcherGroup.TryGetMatchingEntity(remoteTransferActivatorContainer.Item.Value, out var remoteTransferActivatorTuple))
					{
						localTransferItemContainer.Locked = false;
						remoteTransferItemContainer.Locked = false;
						remoteTransferActivatorTuple.Component4.SetState(ActivationState.NotActive);

						ComponentEntityTuple<IItemType, CurrentLocation, Owner> localTransferItemTuple = null;
						ComponentEntityTuple<IItemType, CurrentLocation, Owner> remoteTransferItemTuple = null;

						if (localTransferItemContainer.Item.HasValue
							&& _itemMatcherGroup.TryGetMatchingEntity(localTransferItemContainer.Item.Value, out localTransferItemTuple))
						{
							if (_itemActivationMatcherGroup.TryGetMatchingEntity(localTransferItemTuple.Entity.Id, out var localTransferItemActivationTuple))
							{
								localTransferItemActivationTuple.Component2.CancelActivation();
							}
							localTransferItemTuple.Component2.Value = remoteTransferActivatorTuple.Component2.Value;
							localTransferItemTuple.Component3.Value = null;
						}

						if (remoteTransferItemContainer.Item.HasValue
							&& _itemMatcherGroup.TryGetMatchingEntity(remoteTransferItemContainer.Item.Value, out remoteTransferItemTuple))
						{
							if (_itemActivationMatcherGroup.TryGetMatchingEntity(remoteTransferItemTuple.Entity.Id, out var remoteTransferItemActivationTuple))
							{
								remoteTransferItemActivationTuple.Component2.CancelActivation();
							}
							remoteTransferItemTuple.Component2.Value = localTransferActivatorTuple.Component2.Value;
							remoteTransferItemTuple.Component3.Value = null;
						}

						localTransferItemContainer.Item = remoteTransferItemTuple?.Entity.Id;
						remoteTransferItemContainer.Item = localTransferItemTuple?.Entity.Id;
					}
				}
			}
		}
	}
}
