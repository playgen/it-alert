using Engine.Commands;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Systems;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands
{
	public class ContinueCommand : ICommand
	{
		public string Text { get; set; }
	}

	public class ContinueCommandCommandHandler : CommandHandler<ContinueCommand>
	{
		private readonly ITutorialSystem _tutorialSystem;

		public ContinueCommandCommandHandler(ITutorialSystem tutorialSystem)
		{
			_tutorialSystem = tutorialSystem;
		}

		protected override bool TryProcessCommand(ContinueCommand command, int currentTick)
		{
			_tutorialSystem.SetContinue();
			return true;
		}
	}
}
