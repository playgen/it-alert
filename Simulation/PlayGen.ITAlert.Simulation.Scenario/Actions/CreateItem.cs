using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class CreateItem : SimulationAction
	{
		public CreateItem(string itemType, int systemLogicalId)
		{
			Action = (ecs, config) =>
			{
				var createItemCommand = new CreateItemCommand()
				{
					Archetype = itemType,
					IdentifierType = IdentifierType.Logical,
					SystemId = systemLogicalId
				};
				ecs.EnqueueCommand(createItemCommand);
			};
		}
	}
}
