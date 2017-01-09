using System;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Activation;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public abstract class TimedActivationExtension : IItemActivationExtension
	{
		public void OnActivating(Entity item, Activation activation)
		{
			TimedActivation timedActivation;
			if (item.TryGetComponent(out timedActivation))
			{
				timedActivation.ActivationTicksRemaining = timedActivation.ActivationDuration;
			}
		}

		public void OnActive(Entity item, Activation activation)
		{
			TimedActivation timedActivation;
			if (item.TryGetComponent(out timedActivation))
			{
				if (timedActivation.ActivationTicksRemaining-- <= 0)
				{
					activation.Deactivate();
				}
			}
		}

		public void OnDeactivating(Entity item, Activation activation)
		{
		}
	}
}
