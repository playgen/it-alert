using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Lifecycle;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class EndGame : SimulationAction
	{
		public EndGame(EndGameState endGameState)
		{
			Action = (ecs, config) =>
			{
				var endGameCommand = new EndGameCommand()
				{
					EndGameState = endGameState
				};
				ecs.EnqueueCommand(endGameCommand);
			};
		}
	}
}
