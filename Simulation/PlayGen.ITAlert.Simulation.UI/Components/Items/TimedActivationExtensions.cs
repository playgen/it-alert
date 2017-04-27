using System;
using System.Collections.Generic;
using System.Text;
using Engine.Systems.Activation.Components;

namespace PlayGen.ITAlert.Simulation.UI.Components.Items
{
	public static class TimedActivationExtensions
	{
		public static float GetActivationProportion(this TimedActivation timedActivation)
		{
			return (float) timedActivation.ActivationTicksRemaining / timedActivation.ActivationDuration;
		}
	}
}
