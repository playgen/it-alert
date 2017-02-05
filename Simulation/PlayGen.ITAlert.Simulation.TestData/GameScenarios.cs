using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Entities;
using Engine.Evaluators;
using Engine.Lifecycle.Commands;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public static class GameScenarios
	{
		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateIntroductionScenario()
		{
			const int width = 2;
			const int height = 1;
			const int playerCount = 1;

			var nodeConfigs = SimulationHelper.GenerateGraphNodes(width, height);
			var edgeConfigs = SimulationHelper.GenerateFullyConnectedConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, 1);

			var itemConfigs = new ItemConfig[]
			{
				new ItemConfig()
				{
					StartingLocation = 0,
					TypeName = nameof(Scanner),
				}
			};

			var playerConfigFactory = new Func<int, PlayerConfig>(i => new PlayerConfig() {StartingLocation = 1});

			var configuration = SimulationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs);

			return new SimulationScenario()
			{
				Name = "Introduction",
				Description = "Introduction",
				MinPlayers = playerCount,
				MaxPlayers = playerCount,
				Configuration = configuration,

				CreatePlayerConfig = playerConfigFactory,

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
						OnExitActions = new List<ECSAction<Simulation>>()
						{
							new ECSAction<Simulation>()
							{
								Action = ecs =>
								{
									ICommandSystem commandSystem;
									if (ecs.TryGetSystem(out commandSystem))
									{
										commandSystem.TryHandleCommand(new HideTextCommand());
									}
								}
							}
						},
						// TODO: need a more polymorphic way of specifying evaluators
						// c# 7 pattern match will be nice
						Evaluator = new TimeEvaluator<Simulation>()
						{
							Threshold = 3000,
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
									ICommandSystem commandSystem;
									if (ecs.TryGetSystem(out commandSystem))
									{
										commandSystem.TryHandleCommand(new HideTextCommand());
									}
								}
							}
						},
						Evaluator = new EvaluatorProxy<Simulation>(sim =>
						{
							Entity playerEntity;
							CurrentLocation location;
							return sim.Entities.TryGetValue(configuration.PlayerConfiguration.Single().EntityId, out playerEntity)
									&& playerEntity.TryGetComponent(out location)
									&& location.Value == nodeConfigs.First().EntityId;
						})
					}
				}
			};
		}

		private static SimulationScenario _introduction = null;
		public static SimulationScenario Introduction => _introduction ?? (_introduction = GenerateIntroductionScenario());
	}
}
