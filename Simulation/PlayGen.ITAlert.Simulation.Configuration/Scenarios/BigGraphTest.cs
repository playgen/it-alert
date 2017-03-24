using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Lifecycle;
using Engine.Sequencing;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios
{
	internal static class BigGraphTest
	{
		private static SimulationScenario _scenario;
		public static SimulationScenario Scenario => _scenario ?? (_scenario = GenerateScenario());

		private static readonly Archetype RedVirus = new Archetype("RedVirus")
			.Extends(GameEntities.Malware)
			//.RemoveComponent<MalwarePropogation>()
			//.HasComponent(new ComponentBinding<MalwareVisibility>()
			//{
			//	ComponentTemplate = new MalwareVisibility()
			//	{
			//		VisibleTo = MalwareVisibility.All,
			//	}
			//})
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneRed,
				}
			});

		private static readonly Archetype GreenVirus = new Archetype("GreenVirus")
			.Extends(GameEntities.Malware)
			//.RemoveComponent<MalwarePropogation>()
			//.HasComponent(new ComponentBinding<MalwareVisibility>()
			//{
			//	ComponentTemplate = new MalwareVisibility()
			//	{
			//		VisibleTo = MalwareVisibility.All,
			//	}
			//})
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneGreen,
				}
			});

		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int minPlayerCount = 1;
			const int maxPlayerCount = 4;

			/*
			 * T - r       # - #
			 * |   |       |   |
			 * # - # - A - g - T
			 *         | 
			 *         # 
			 */

			var node00 = new NodeConfig()
			{
				Name = "0 0",
				X = 0,
				Y = 0,
				ArchetypeName = nameof(GameEntities.TransferStation),
			};

			var node10 = new NodeConfig()
			{
				Name = "1 0",
				X = 1,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var node01 = new NodeConfig()
			{
				Name = "0 1",
				X = 0,
				Y = 1,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var node11 = new NodeConfig()
			{
				Name = "1 1",
				X = 1,
				Y = 1,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var node21 = new NodeConfig()
			{
				Name = "2 1",
				X = 2,
				Y = 1,
				ArchetypeName = nameof(GameEntities.AntivirusWorkstation),
			};

			var node22 = new NodeConfig()
			{
				Name = "2 2",
				X = 2,
				Y = 2,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var node30 = new NodeConfig()
			{
				Name = "3 0",
				X = 3,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var node31 = new NodeConfig()
			{
				Name = "3 1",
				X = 3,
				Y = 1,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var node40 = new NodeConfig()
			{
				Name = "4 0",
				X = 4,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var node41 = new NodeConfig()
			{
				Name = "4 1",
				X = 4,
				Y = 1,
				ArchetypeName = nameof(GameEntities.TransferStation),
			};

			var nodeConfigs = new NodeConfig[]
			{
				node00,
				node01,
				node10,
				node11,
				node21,
				node22,
				node30,
				node31,
				node40,
				node41,
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);
			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);
			var itemConfigs = new ItemConfig[0];
			var playerConfigFactory = new Func<int, PlayerConfig>(i => new PlayerConfig()
			{
				StartingLocation = i,
				ArchetypeName = nameof(GameEntities.Player)
			});
			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs);

			configuration.RNGSeed = 456980495;

			configuration.Archetypes.AddRange(new[]
			{
				RedVirus,
				GreenVirus,
			});

			#endregion

			#region frames

			// ReSharper disable once UseObjectOrCollectionInitializer
			var frames = new List<SimulationFrame>();

			var random = new Engine.Util.Random(894765213);

			#region 1
			
			//T SelectCyclical<T>(T[] options, int i) => options[i % options.Length];
			//int virusIndex = 0;
			
			frames.Add(// frame 1 - welcome
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.CreateNpcCommand(RedVirus.Name, node10.Id),
						ScenarioHelpers.CreateNpcCommand(GreenVirus.Name, node31.Id),

						ScenarioHelpers.CreateItemCommand(GameEntities.Scanner.Name, node00.Id),
						ScenarioHelpers.CreateItemCommand(GameEntities.Capture.Name, node41.Id),
						//ScenarioHelpers.GenerateTextAction(true, 
						//	"Click continue when you are ready to end!")
					},
					OnTickActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ConditionalAction<Simulation, SimulationConfiguration>(ScenarioHelpers.CreateNpcAtRandomLocationCommand(new [] { RedVirus, GreenVirus }[random.Next(2)].Name, random), ScenarioHelpers.WaitForTicks(300))
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
						ScenarioHelpers.EndGame(EndGameState.Success),
					},
					// TODO: need a more polymorphic way of specifying evaluators
					// c# 7 pattern match will be nice
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			#endregion


			#endregion

			return new SimulationScenario()
			{
				Name = "BigGraphTest",
				Description = "BigGraphTest",
				MinPlayers = minPlayerCount,
				MaxPlayers = maxPlayerCount,
				Configuration = configuration,

				CreatePlayerConfig = playerConfigFactory,

				// TODO: need a config driven specification for these
				Sequence = frames.ToArray(),
			};
		}
	}
}
