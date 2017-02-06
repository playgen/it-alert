using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Tutorial;
using PlayGen.ITAlert.Simulation.Systems.Tutorial;

namespace PlayGen.ITAlert.Simulation.Commands.Tutorial
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

		protected override bool TryProcessCommand(ContinueCommand command)
		{
			_tutorialSystem.SetContinue();
			return true;
		}
	}
}
