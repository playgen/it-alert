using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Tutorial
{
	public class ActivationContinue : IFlagComponent
	{
		public enum ActivationPhase
		{
			Activating,
			Active,
			Deactivating,
		}

		public ActivationPhase ContinueOn { get; set; }
	}
}
