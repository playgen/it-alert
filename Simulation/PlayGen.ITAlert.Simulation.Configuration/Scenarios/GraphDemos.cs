using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios
{
	internal static class GraphDemos
	{
		private const int MinWidth = 2;
		private const int MaxWidth = 5;

		private const int MinHeight = 2;
		private const int MaxHeight = 3;

		private static List<SimulationScenario> _scenarios;
		public static List<SimulationScenario> Scenarios => _scenarios ?? (_scenarios = GenerateScenarios());

		private static List<SimulationScenario> GenerateScenarios()
		{
			var scenarios = new List<SimulationScenario>();
			for (int w = MinWidth; w <= MaxWidth; w++)
			{
				for (int h = MinHeight; h <= MaxHeight; h++)
				{
					scenarios.Add(GenerateScenario(w, h));
				}
			}
			return scenarios;
		}

		private static SimulationScenario GenerateScenario(int width, int height)
		{
			#region configuration

			const int minPlayerCount = 1;
			const int maxPlayerCount = 4;

			var nodeConfigs = ConfigurationHelper.GenerateGraphNodes(width, height);
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);
			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);
			var itemConfigs = new ItemConfig[0];
			var playerConfigFactory = new Func<int, PlayerConfig>(i => new PlayerConfig()
			{
				StartingLocation = i,
				ArchetypeName = nameof(GameEntities.Player)
			});
			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs);

			#endregion

			#region frames

			// ReSharper disable once UseObjectOrCollectionInitializer
			var frames = new List<SimulationFrame>();

			#region 1

			frames.Add(// frame 1 - welcome
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true, 
							"Click continue when you are ready to end!")
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
				Name = $"Test {width}x{height}",
				Description = "Test",
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
