using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ResetOwnerOnDeactivationSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Activation, Owner> _ownerActivationMatcherGroup;

		public ResetOwnerOnDeactivationSystem(IMatcherProvider matcherProvider)
		{
			_ownerActivationMatcherGroup = matcherProvider.CreateMatcherGroup<Activation, Owner>();
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _ownerActivationMatcherGroup.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.Deactivating:
						OnDeactivating(match);
						break;
				}
			}
		}

		public void OnDeactivating(ComponentEntityTuple<Activation, Owner> entityTuple)
		{
			entityTuple.Component2.Value = null;
		}

		public void Dispose()
		{
			_ownerActivationMatcherGroup?.Dispose();
		}
	}
}
