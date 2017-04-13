//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Engine.Archetypes;
//using Engine.Lifecycle;
//using Engine.Sequencing;
//using Engine.Systems;
//using PlayGen.ITAlert.Simulation.Archetypes;
//using PlayGen.ITAlert.Simulation.Common;
//using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
//using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
//using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
//using PlayGen.ITAlert.Simulation.Modules.Transfer.Archetypes;
//using PlayGen.ITAlert.Simulation.Scenario.Actions;
//using PlayGen.ITAlert.Simulation.Scenario.Configuration;
//using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
//using PlayGen.ITAlert.Simulation.Sequencing;

//namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios
//{
//	internal static class BigGraphTest
//	{
//		private static SimulationScenario _scenario;
//		public static SimulationScenario Scenario => _scenario ?? (_scenario = GenerateScenario());

//		private static readonly Archetype RedVirus = new Archetype(nameof(RedVirus))
//			.Extends(Virus.Archetype)
//			.HasComponent(new ComponentBinding<MalwareGenome>()
//			{
//				ComponentTemplate = new MalwareGenome()
//				{
//					Value = SimulationConstants.MalwareGeneRed,
//				}
//			});

//		private static readonly Archetype GreenVirus = new Archetype(nameof(GreenVirus))
//			.Extends(Virus.Archetype)
//			.HasComponent(new ComponentBinding<MalwareGenome>()
//			{
//				ComponentTemplate = new MalwareGenome()
//				{
//					Value = SimulationConstants.MalwareGeneGreen,
//				}
//			});

//		private static SimulationScenario GenerateScenario()
//		{
//			#region configuration

//			const int minPlayerCount = 1;
//			const int maxPlayerCount = 4;

//			/*
//			 * T - r       # - #
//			 * |   |       |   |
//			 * # - # - A - g - T
//			 *         | 
//			 *         # 
//			 */

//			var node00 = new NodeConfig()
//			{
//				Name = "0 0",
//				X = 0,
//				Y = 0,
//				Archetype = TransferWorkstation.Archetype.Name,
//			};

//			var node10 = new NodeConfig()
//			{
//				Name = "1 0",
//				X = 1,
//				Y = 0,
//				Archetype = SubsystemNode.Archetype.Name,
//			};

//			var node01 = new NodeConfig()
//			{
//				Name = "0 1",
//				X = 0,
//				Y = 1,
//				Archetype = nameof(SubsystemNode.Archetype.Name),
//			};

//			var node11 = new NodeConfig()
//			{
//				Name = "1 1",
//				X = 1,
//				Y = 1,
//				Archetype = nameof(SubsystemNode.Archetype.Name),
//			};

//			var node21 = new NodeConfig()
//			{
//				Name = "2 1",
//				X = 2,
//				Y = 1,
//				Archetype = nameof(AntivirusWorkstation.Archetype.Name),
//			};

//			var node22 = new NodeConfig()
//			{
//				Name = "2 2",
//				X = 2,
//				Y = 2,
//				Archetype = nameof(SubsystemNode.Archetype.Name),
//			};

//			var node30 = new NodeConfig()
//			{
//				Name = "3 0",
//				X = 3,
//				Y = 0,
//				Archetype = nameof(SubsystemNode.Archetype.Name),
//			};

//			var node31 = new NodeConfig()
//			{
//				Name = "3 1",
//				X = 3,
//				Y = 1,
//				Archetype = nameof(SubsystemNode.Archetype.Name),
//			};

//			var node40 = new NodeConfig()
//			{
//				Name = "4 0",
//				X = 4,
//				Y = 0,
//				Archetype = nameof(SubsystemNode.Archetype.Name),
//			};

//			var node41 = new NodeConfig()
//			{
//				Name = "4 1",
//				X = 4,
//				Y = 1,
//				Archetype = nameof(TransferWorkstation.Archetype.Name),
//			};

//			var nodeConfigs = new NodeConfig[]
//			{
//				node00,
//				node01,
//				node10,
//				node11,
//				node21,
//				node22,
//				node30,
//				node31,
//				node40,
//				node41,
//			};
//			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);
//			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);

//			var archetypes = new List<Archetype>
//			{
//				RedVirus,
//				GreenVirus,
//			};

//			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);

//			configuration.RNGSeed = 456980495;


//			#endregion

//			var scenario = new SimulationScenario()
//			{
//				Name = "BigGraphTest",
//				Description = "BigGraphTest",
//				MinPlayers = minPlayerCount,
//				MaxPlayers = maxPlayerCount,
//				Configuration = configuration,

//				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Archetypes.Player.Archetype.Name, nodeConfigs.Select(nc => nc.Id).ToArray()),

//				// TODO: need a config driven specification for these
//				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
//			};

//			#region frames

//			// ReSharper disable once UseObjectOrCollectionInitializer
//			var random = new Engine.Util.Random(894765213);

//			#region 1
			
//			//T SelectCyclical<T>(T[] options, int i) => options[i % options.Length];
//			//int virusIndex = 0;
			
//			scenario.Sequence.Add(// frame 1 - welcome
//				new SimulationFrame()
//				{
//					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
//					{
//						new CreateMalware(RedVirus.Name, node10),
//						new CreateMalware(GreenVirus.Name, node31),

//						new CreateItem(ScannerTool.Archetype.Name, node00),
//						new CreateItem(CaptureTool.Archetype.Name, node41),
//						//ScenarioHelpers.GenerateTextAction(true, 
//						//	"Click continue when you are ready to end!")
//					},
//					OnTickActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
//					{
//						//new ConditionalAction<Simulation, SimulationConfiguration>(new CreateNpcAtRandomLocationCommand(new [] { RedVirus, GreenVirus }[random.Next(2)].Name, random), new WaitForTicks(300))
//					},
//					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
//					{
//						new HideText(),
//						new EndGame(EndGameState.Success),
//					},
//					// TODO: need a more polymorphic way of specifying evaluators
//					// c# 7 pattern match will be nice
//					ExitCondition = new WaitForTutorialContinue(),
//				}
//			);

//			#endregion


//			#endregion

//			return scenario;
//		}
//	}
//}
