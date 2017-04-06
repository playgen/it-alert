using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Components
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
