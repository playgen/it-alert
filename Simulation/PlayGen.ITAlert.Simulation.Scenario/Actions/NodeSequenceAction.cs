using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class NodeSequenceAction : SimulationAction
	{
		private readonly NodeSequence _nodeSequence;

		public NodeSequenceAction(NodeSequence nodeSequence, Func<EntityConfig, SimulationAction> generator)
		{
			_nodeSequence = nodeSequence;

			Action = (ecs, config) =>
			{
				if (_nodeSequence.TryGetNext(out var next))
				{
					var action = generator(next);
					action.Execute(ecs, config);
				}
			};
		}
	}
}
