using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Sequencing;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation
{
	public static class GameScenarios
	{
		public static Scenario Introduction = new Scenario()
		{
			Name = "Introduction",
			MinPlayers = 1,
			MaxPlayers = 1,
			Sequence = new SequenceFrame<Simulation>[]
			{
				new SequenceFrame<Simulation>()
				{
					OnEnterActions = new List<ECSAction<Simulation>>()
					{
						new ECSAction<Simulation>()
						{
							Action = ecs =>
							{
								var textCommand = new DisplayTextCommand()
								{
									Text = "Welcome to IT Alert!"
								};
								CommandSystem commandSystem;
								if (ecs.TryGetSystem(out commandSystem)
									&& commandSystem.TryHandleCommand(textCommand) == false)
								{
									throw new SequenceException("Unable to issue tutorial text command");
								}
							}
						}
					}
				}
			}
		};
	}
}
