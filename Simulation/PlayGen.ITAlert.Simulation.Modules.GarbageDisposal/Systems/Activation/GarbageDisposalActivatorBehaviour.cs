﻿using Engine.Components;
using Engine.Entities;
using Engine.Systems.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Components;

namespace PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Systems.Activation
{
	public class GarbageDisposalActivatorBehaviour : IActivationExtension
	{
		private readonly ComponentMatcherGroup<GarbageDisposalActivator, CurrentLocation, Owner> _garbageDisposalMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<IItemType> _itemMatcherGroup;



		public GarbageDisposalActivatorBehaviour(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			_garbageDisposalMatcherGroup = matcherProvider.CreateMatcherGroup<GarbageDisposalActivator, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<IItemType>();

		}

		public void OnNotActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_garbageDisposalMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple)
				&& itemTuple.Component2.Value.HasValue
				&& itemTuple.Component3.Value.HasValue)
			{
				itemTuple.Component3.Value = null;
			}
		}

		public void OnActivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_garbageDisposalMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple))
			{
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out var locationTuple)
					&& locationTuple.Component2.TryGetItemContainer<GarbageDisposalTargetItemContainer>(out var targetItemContainer)
					&& targetItemContainer.Item.HasValue)
				{
					targetItemContainer.Locked = true;
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
			if (_garbageDisposalMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple))
			{
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out var locationTuple)
					&& locationTuple.Component2.TryGetItemContainer<GarbageDisposalTargetItemContainer>(out var targetItemContainer)
					&& targetItemContainer.Item.HasValue
					&& _itemMatcherGroup.TryGetMatchingEntity(targetItemContainer.Item.Value, out var targetItemTuple))
				{
					targetItemContainer.Locked = false;
					targetItemTuple.Entity.Dispose();
				}
			}
		}
	}
}