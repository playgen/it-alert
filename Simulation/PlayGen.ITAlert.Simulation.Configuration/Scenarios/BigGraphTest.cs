﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Lifecycle;
using Engine.Sequencing;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Scenario.Actions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
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

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs);

			configuration.RNGSeed = 456980495;

			configuration.Archetypes.AddRange(new[]
			{
				RedVirus,
				GreenVirus,
			});

			#endregion

			var scenario = new SimulationScenario()
			{
				Name = "BigGraphTest",
				Description = "BigGraphTest",
				MinPlayers = minPlayerCount,
				MaxPlayers = maxPlayerCount,
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Archetypes.Player.Archetype.Name, nodeConfigs.Select(nc => nc.Id).ToArray()),

				// TODO: need a config driven specification for these
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			#region frames

			// ReSharper disable once UseObjectOrCollectionInitializer
			var random = new Engine.Util.Random(894765213);

			#region 1
			
			//T SelectCyclical<T>(T[] options, int i) => options[i % options.Length];
			//int virusIndex = 0;
			
			scenario.Sequence.Add(// frame 1 - welcome
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(RedVirus.Name, node10.Id),
						new CreateMalware(GreenVirus.Name, node31.Id),

						new CreateItem(GameEntities.Scanner.Name, node00.Id),
						new CreateItem(GameEntities.Capture.Name, node41.Id),
						//ScenarioHelpers.GenerateTextAction(true, 
						//	"Click continue when you are ready to end!")
					},
					OnTickActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						//new ConditionalAction<Simulation, SimulationConfiguration>(new CreateNpcAtRandomLocationCommand(new [] { RedVirus, GreenVirus }[random.Next(2)].Name, random), new WaitForTicks(300))
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new EndGame(EndGameState.Success),
					},
					// TODO: need a more polymorphic way of specifying evaluators
					// c# 7 pattern match will be nice
					Evaluator = new WaitForTutorialContinue(),
				}
			);

			#endregion


			#endregion

			return scenario;
		}
	}
}
