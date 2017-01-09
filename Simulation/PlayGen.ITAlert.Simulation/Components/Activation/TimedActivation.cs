using System;
using Engine.Components;
using Engine.Entities;
using Engine.Serialization;

namespace PlayGen.ITAlert.Simulation.Components.Activation
{
	[ComponentDependency(typeof(Activation))]
	public abstract class TimedActivation : IComponent
	{
		public int ActivationTicksRemaining { get; set; }

		public int ActivationDuration { get; set; }
	}
}
