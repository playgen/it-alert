using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios
{
	internal static class MultiplayerIntroduction
	{
		private static SimulationScenario _scenario;
		public static SimulationScenario Scenario => _scenario ?? (_scenario = GenerateScenario());

		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int minPlayerCount = 2;
			const int maxPlayerCount = 4;

			var nodeTopLeft = new NodeConfig(0)
			{
				Name = "Top Left",
				X = 0,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeTopRight = new NodeConfig(1)
			{
				Name = "Top Right",
				X = 1,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeBottomLeft = new NodeConfig(2)
			{
				Name = "Bottom Left",
				X = 0,
				Y = 1,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeBottomRight = new NodeConfig(3)
			{
				Name = "Bottom Right",
				X = 1,
				Y = 1,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeConfigs = new NodeConfig[] { nodeTopLeft, nodeTopRight, nodeBottomLeft, nodeBottomRight };
			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, 1);
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
				Name = "MultiplayerIntroduction",
				Description = "MultiplayerIntroduction",
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
