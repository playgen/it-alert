using Engine.Components;
using Engine.Systems.Activation.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Components
{
	public class ActivationContinue : IFlagComponent
	{
		public ActivationState ContinueOn { get; set; }
	}
}
