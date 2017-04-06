using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Resources.Components
{
	public interface IResource : IComponent
	{
		int Value { get; set; }

		int Maximum { get; }
	}
}
