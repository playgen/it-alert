using Engine.Systems;

namespace PlayGen.ITAlert.Simulation.Modules.Resources.Systems
{
	public interface ISubsystemResourceEffect : ISystemExtension
	{
		void Tick();
	}
}
