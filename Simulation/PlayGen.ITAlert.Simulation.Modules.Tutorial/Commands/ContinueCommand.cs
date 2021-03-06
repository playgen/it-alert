﻿using Engine.Commands;
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

		protected override bool TryHandleCommand(ContinueCommand command, int currentTick, bool handlerEnabled)
		{
			_tutorialSystem.SetContinue();
			return true;
		}
	}


}
