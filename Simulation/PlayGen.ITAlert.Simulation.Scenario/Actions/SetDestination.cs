using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class SetDestination : SimulationAction
	{
		private NodeConfig _nodeConfig;

		public SetDestination(NodeConfig nodeConfig, int playerId)
		{
			Action = (ecs, config) =>
			{
				ecs.EnqueueCommand(new SetActorDestinationCommand()
				{
					PlayerId =  playerId,
					DestinationEntityId = nodeConfig.EntityId,
				});
			};
		}
	}
}
