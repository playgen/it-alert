using System;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Activation;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class TimedActivationExtension : IItemActivationExtension
	{
		private readonly ComponentMatcherGroup<TimedActivation> _timedActivationMatcherGroup;

		protected TimedActivationExtension(IMatcherProvider matcherProvider)
		{
			_timedActivationMatcherGroup = matcherProvider.CreateMatcherGroup<TimedActivation>();
		}

		public void OnActivating(int itemId, Activation activation)
		{
			ComponentEntityTuple<TimedActivation> itemTuple;
			if (_timedActivationMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				itemTuple.Component1.ActivationTicksRemaining = itemTuple.Component1.ActivationDuration;
			}
		}

		public void OnActive(int itemId, Activation activation)
		{
			ComponentEntityTuple<TimedActivation> itemTuple;
			if (_timedActivationMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				if (itemTuple.Component1.ActivationTicksRemaining-- <= 0)
				{
					activation.Deactivate();
				}
			}
		}

		public void OnDeactivating(int entityId, Activation activation)
		{
		}

	}
}
