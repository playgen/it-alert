using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class CreatePlayer : SimulationAction
	{
		public CreatePlayer(string playerArchetype, 
			EntityConfig nodeConfig, 
			string name,
			string colourHex = "ffffff"
			)
		{
			Action = (ecs, config) =>
			{
				var createPlayerCommand = new CreatePlayerCommand()
				{
					PlayerConfig = new PlayerConfig()
					{
						Archetype = playerArchetype,
						Colour = colourHex,
						StartingLocation = nodeConfig.Id,
						Name = name,
						Glyph = "star"
					}
				};
				ecs.EnqueueCommand(createPlayerCommand);
			};
		}
	}
}
