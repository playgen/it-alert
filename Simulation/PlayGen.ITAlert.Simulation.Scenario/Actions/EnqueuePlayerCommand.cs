using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class EnqueuePlayerCommand : SimulationAction
	{
		public EnqueuePlayerCommand(ICommand command)
		{
			Action = (ecs, config) =>
			{
				ecs.EnqueueCommand(command);
			};
		}
	}
}
