using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Common
{
	public class CurrentLocation : IComponent
	{
		// TODO: this probably needs to be nullable
		public int Value { get; set; }
	}
}
