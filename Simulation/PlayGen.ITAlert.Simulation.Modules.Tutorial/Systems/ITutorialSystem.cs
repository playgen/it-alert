using Engine.Systems;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Systems
{
	public interface ITutorialSystem : IInitializingSystem
	{
		bool Continue { get; }
		void SetContinue();
	}
}