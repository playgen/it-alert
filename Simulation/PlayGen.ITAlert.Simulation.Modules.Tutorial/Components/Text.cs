using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Components
{
	public class Text : IComponent
	{
		public string Value { get; set; }

		public bool ShowContinue { get; set; }
	}
}
