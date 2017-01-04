﻿using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Activation
{
	public class Activation : Component
	{
		public ActivationState ActivationState { get; private set; }

		public void Activate()
		{
			ActivationState = ActivationState.Activating;
		}

		public void SetState(ActivationState state)
		{
			ActivationState = state;
		}

		public void Deactivate()
		{
			ActivationState = ActivationState.Deactivating;
		}
	}
}