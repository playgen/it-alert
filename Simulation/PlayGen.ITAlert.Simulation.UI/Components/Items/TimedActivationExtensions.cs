using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Components.Activation;

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
