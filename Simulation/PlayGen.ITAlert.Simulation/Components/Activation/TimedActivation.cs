using System;
using Engine.Components;
using Engine.Entities;
using Engine.Serialization;
using PlayGen.ITAlert.Simulation.Components.Behaviours;

namespace PlayGen.ITAlert.Simulation.Components.Activation
{
	[ComponentDependency(typeof(Activation))]
	public abstract class TimedActivation : Component, IActivatable
	{
		private int _activationTicksRemaining;

		private int _activationDuration;

		private Activation _activation;

		public void SetDuration(int duration)
		{
			_activationDuration = duration;
		}

		public override void Initialize(Entity entity)
		{
			base.Initialize(entity);

			_activation = Entity.GetComponent<Activation>();
		}

		public void OnActivating()
		{
			_activationTicksRemaining = _activationDuration;
		}

		public void OnActive()
		{
			if (_activationTicksRemaining-- <= 0)
			{
				// TODO: find a better way of signalling this: deactivation intent?
				_activation.Deactivate();
			}
		}

		public void OnDeactivating()
		{
			// nothing to do
		}
	}
}
