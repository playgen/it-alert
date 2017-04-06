using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class HideText : SimulationAction
	{
		public HideText()
		{
			Action = (ecs, configuration) => {
				ecs.EnqueueCommand(new HideTextCommand());
			};
		}
	}
}
