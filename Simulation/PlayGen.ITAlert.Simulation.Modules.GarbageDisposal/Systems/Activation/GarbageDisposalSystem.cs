using Engine.Components;
using Engine.Entities;
using Engine.Events;
using Engine.Systems;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Components;

namespace PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Systems.Activation
{
	public class GarbageDisposalSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Engine.Systems.Activation.Components.Activation, GarbageDisposalActivator, CurrentLocation, Owner> _garbageDisposalMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<IItemType> _itemMatcherGroup;

		private readonly EventSystem _eventSystem;

		public GarbageDisposalSystem(IMatcherProvider matcherProvider, 
			EventSystem eventSystem)
		{
			_garbageDisposalMatcherGroup = matcherProvider.CreateMatcherGroup<Engine.Systems.Activation.Components.Activation, GarbageDisposalActivator, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<IItemType>();

			_eventSystem = eventSystem;
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _garbageDisposalMatcherGroup.MatchingEntities)
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
					case ActivationState.Deactivating:
						OnDeactivating(match, currentTick);
						break;
				}
			}
		}

		private void OnNotActive(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, GarbageDisposalActivator, CurrentLocation, Owner> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& entityTuple.Component4.Value.HasValue)
			{
				entityTuple.Component4.Value = null;
			}
		}

		private void OnActivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, GarbageDisposalActivator, CurrentLocation, Owner> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var locationTuple)
				&& locationTuple.Component2.TryGetItemContainer<GarbageDisposalTargetItemContainer>(out var targetItemContainer)
				&& targetItemContainer.Item.HasValue)
			{
				targetItemContainer.Locked = true;
			}
			else
			{
				entityTuple.Component1.SetState(ActivationState.NotActive, currentTick);
			}
		}

		private void OnDeactivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, GarbageDisposalActivator, CurrentLocation, Owner> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var locationTuple)
				&& locationTuple.Component2.TryGetItemContainer<GarbageDisposalTargetItemContainer>(out var targetItemContainer)
				&& targetItemContainer.Item.HasValue
				&& _itemMatcherGroup.TryGetMatchingEntity(targetItemContainer.Item.Value, out var targetItemTuple))
			{
				targetItemContainer.Locked = false;
				targetItemTuple.Entity.Dispose();
			}
		}

		public void Dispose()
		{
			_garbageDisposalMatcherGroup?.Dispose();
			_subsystemMatcherGroup?.Dispose();
			_itemMatcherGroup?.Dispose();
			_eventSystem?.Dispose();
		}
	}
}
