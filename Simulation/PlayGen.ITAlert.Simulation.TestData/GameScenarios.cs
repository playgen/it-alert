using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Entities;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items.Flags;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Startup;

namespace PlayGen.ITAlert.Simulation
{
	public static class GameScenarios
	{
		// TODO: this should be parameterized further and read from config
		private static Scenario GenerateIntroductionScenario()
		{
			const int width = 2;
			const int height = 1;
			const int playerCount = 1;

			var nodeConfigs = SimulationHelper.GenerateGraphNodes(width, height);
			var edgeConfigs = SimulationHelper.GenerateFullyConnectedConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, 1);

			var player = new PlayerConfig()
			{
				Colour = "#ff00ff",
				Name = "Player",
				StartingLocation = 1,
			};
			var playerConfigs = new PlayerConfig[]
			{
				player,
			};

			var itemConfigs = new ItemConfig[]
			{
				new ItemConfig()
				{
					StartingLocation = 2,
					TypeName = nameof(Scanner),
				}
			};

			var configuration = SimulationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);

			return new Scenario()
			{
				Name = "Introduction",
				MinPlayers = playerCount,
				MaxPlayers = playerCount,
				Configuration = configuration,

				// TODO: need a config driven specification for these
				Sequence = new SequenceFrame<Simulation>[]
				{
					// frame 1 - welcome
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
								},
								Name = "Welcome Message"
							}
						},
						OnExitActions = new List<ECSAction<Simulation>>(),
						// TODO: need a more polymorphic way of specifying evaluators
						// c# 7 pattern match will be nice
						Evaluator = new TickEvaluator<Simulation>()
						{
							Threshold = 20,
						}
					},
					// frame 2 - movement
					// then stop
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
										Text = "Try navigating to another system by clicking on it"
									};
									CommandSystem commandSystem;
									if (ecs.TryGetSystem(out commandSystem)
										&& commandSystem.TryHandleCommand(textCommand) == false)
									{
										throw new SequenceException("Unable to issue tutorial text command");
									}
								},
								Name = "Navigation Message"
							}
						},
						OnExitActions = new List<ECSAction<Simulation>>()
						{
							new ECSAction<Simulation>()
							{
								Action = ecs =>
								{
									LifecycleSystem lifecycleSystem;
									if (ecs.TryGetSystem(out lifecycleSystem))
									{
										lifecycleSystem.TryStop();
									}
								}
							}
						},
						Evaluator = new EvaluatorProxy<Simulation>(sim =>
						{
							Entity playerEntity;
							CurrentLocation location;
							return sim.Entities.TryGetValue(player.EntityId, out playerEntity)
									&& playerEntity.TryGetComponent(out location)
									&& location.Value == nodeConfigs.Last().EntityId;
						})
					}
				}
			};
		}

		private static Scenario _introduction = null;
		public static Scenario Introduction => _introduction ?? (_introduction = GenerateIntroductionScenario());
	}
}
