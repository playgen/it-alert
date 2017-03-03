using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Systems.Enhancements;
using PlayGen.ITAlert.Simulation.Systems.Extensions;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class GarbageDisposalBehaviour : IItemActivationExtension
	{
		public const string AnalysisOutputArchetypeName = "Antivirus";
		
		private readonly ComponentMatcherGroup<GarbageDisposal, CurrentLocation, Owner> _garbageDisposalMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<IItemType> _itemMatcherGroup;


		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public GarbageDisposalBehaviour(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			_garbageDisposalMatcherGroup = matcherProvider.CreateMatcherGroup<GarbageDisposal, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<IItemType>();

			_entityFactoryProvider = entityFactoryProvider;
		}

		public void OnNotActive(int itemId, Activation activation)
		{
			ComponentEntityTuple<GarbageDisposal, CurrentLocation, Owner> itemTuple;
			if (_garbageDisposalMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple)
				&& itemTuple.Component2.Value.HasValue
				&& itemTuple.Component3.Value.HasValue)
			{
				itemTuple.Component3.Value = null;
			}
		}

		public void OnActivating(int itemId, Activation activation)
		{
			ComponentEntityTuple<GarbageDisposal, CurrentLocation, Owner> itemTuple;
			if (_garbageDisposalMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				ComponentEntityTuple<Subsystem, ItemStorage> locationTuple;
				GarbageDisposalTargetItemContainer targetItemContainer;
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out locationTuple)
					&& locationTuple.Component2.TryGetItemContainer(out targetItemContainer)
					&& targetItemContainer.Item.HasValue)
				{
				}
				else
				{
					activation.CancelActivation();
				}
			}

		}

		public void OnActive(int itemId, Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Activation activation)
		{
			ComponentEntityTuple<GarbageDisposal, CurrentLocation, Owner> itemTuple;
			if (_garbageDisposalMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				ComponentEntityTuple<Subsystem, ItemStorage> locationTuple;
				GarbageDisposalTargetItemContainer targetItemContainer;
				ComponentEntityTuple<IItemType> targetItemTuple;
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out locationTuple)
					&& locationTuple.Component2.TryGetItemContainer(out targetItemContainer)
					&& targetItemContainer.Item.HasValue
					&& _itemMatcherGroup.TryGetMatchingEntity(targetItemContainer.Item.Value, out targetItemTuple))
				{
					targetItemTuple.Entity.Dispose();
				}
			}
		}
	}
}
