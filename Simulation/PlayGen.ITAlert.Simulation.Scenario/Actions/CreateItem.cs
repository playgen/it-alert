using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class CreateItem : SimulationAction
	{
		private readonly EntityConfig _nodeEntityConfig;

		public CreateItem(string itemType, EntityConfig nodeEntityConfig, Type containerType = null)
		{
			_nodeEntityConfig = nodeEntityConfig;

			Action = (ecs, config) =>
			{
				var createItemCommand = new CreateItemCommand()
				{
					Archetype = itemType,
					SystemId = _nodeEntityConfig.EntityId,
					ContainerType = containerType,
				};
				ecs.EnqueueCommand(createItemCommand);
			};
		}
	}
}
