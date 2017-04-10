//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Engine.Archetypes;
//using Engine.Lifecycle;
//using Engine.Sequencing;
//using PlayGen.ITAlert.Simulation.Scenario.Actions;
//using PlayGen.ITAlert.Simulation.Scenario.Configuration;
//using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
//using PlayGen.ITAlert.Simulation.Sequencing;

//namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios
//{
//	internal static class GraphDemos
//	{
//		private const int MinWidth = 2;
//		private const int MaxWidth = 5;

//		private const int MinHeight = 2;
//		private const int MaxHeight = 3;

//		private static List<SimulationScenario> _scenarios;
//		public static List<SimulationScenario> Scenarios => _scenarios ?? (_scenarios = GenerateScenarios());

//		private static List<SimulationScenario> GenerateScenarios()
//		{
//			var scenarios = new List<SimulationScenario>();
//			for (int w = MinWidth; w <= MaxWidth; w++)
//			{
//				for (int h = MinHeight; h <= MaxHeight; h++)
//				{
//					scenarios.Add(GenerateScenario(w, h));
//				}
//			}
//			return scenarios;
//		}

//		private static SimulationScenario GenerateScenario(int width, int height)
//		{
//			#region configuration

//			const int minPlayerCount = 1;
//			const int maxPlayerCount = 4;

//			var nodeConfigs = ConfigurationHelper.GenerateGraphNodes(width, height);
//			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);
//			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);

//			var archetypes = new List<Archetype>
//			{

//			};

//			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);

//			#endregion

//			var scenario = new SimulationScenario()
//			{
//				Name = $"Test {width}x{height}",
//				Description = "Test",
//				MinPlayers = minPlayerCount,
//				MaxPlayers = maxPlayerCount,
//				Configuration = configuration,

//				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Archetypes.Player.Archetype.Name, nodeConfigs.Select(nc => nc.Id).ToArray()),

//				// TODO: need a config driven specification for these
//				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>()
//			};

//			#region frames

//			// ReSharper disable once UseObjectOrCollectionInitializer
//			#region 1

//			scenario.Sequence.Add(// frame 1 - welcome
//				new SimulationFrame()
//				{
//					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
//					{
//						new ShowText(true, 
//							"Click continue when you are ready to end!")
//					},
//					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
//					{
//						new HideText(),
//						new EndGame(EndGameState.Success),
//					},
//					// TODO: need a more polymorphic way of specifying evaluators
//					// c# 7 pattern match will be nice
//					Evaluator =  new WaitForTutorialContinue(),
//				}
//			);

//			#endregion

//			#endregion

//			return scenario;
//		}
//	}
//}
