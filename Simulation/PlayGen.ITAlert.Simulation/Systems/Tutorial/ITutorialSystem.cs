using Engine.Systems;

namespace PlayGen.ITAlert.Simulation.Systems.Tutorial
{
	public interface ITutorialSystem : IInitializingSystem
	{
		bool Continue { get; }
		void SetContinue();
	}
}