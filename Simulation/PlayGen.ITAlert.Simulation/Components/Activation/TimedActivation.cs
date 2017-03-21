using System;
using Engine.Components;
using Engine.Entities;
using Engine.Serialization;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Activation
{
	[ComponentDependency(typeof(Activation))]
	public class TimedActivation : IComponent
	{
		public decimal ActivationTickModifier { get; set; } = SimulationConstants.TimedActivationTickModifier;

		public decimal ActivationTicksRemaining { get; set; }

		public int ActivationDuration { get; set; }
	}
}
