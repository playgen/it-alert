using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Tutorial;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios
{
	internal static class Analysis
	{
		private static SimulationScenario _scenario;
		public static SimulationScenario Scenario => _scenario ?? (_scenario = GenerateScenario());

		#region scenario specific archetypes

		private static readonly Archetype TutorialScanner = new Archetype("TutorialScanner")
			.Extends(GameEntities.Scanner)
			.HasComponent(new ComponentBinding<ActivationContinue>()
			{
				ComponentTemplate = new ActivationContinue()
				{
					ContinueOn = ActivationContinue.ActivationPhase.Deactivating,
				}
			});

		private static readonly Archetype RedTutorialVirus = new Archetype("RedTutorialVirus")
			.Extends(GameEntities.Malware)
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneRed,
				}
			})
			.HasComponent(new ComponentBinding<MalwareVisibility>()
			{
				ComponentTemplate = new MalwareVisibility()
				{
					VisibleTo = MalwareVisibility.All,
				}
			});

		private static readonly Archetype GreenTutorialVirus = new Archetype("GreenTutorialVirus")
			.Extends(GameEntities.Malware)
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneGreen,
				}
			})
			.HasComponent(new ComponentBinding<MalwareVisibility>()
			{
				ComponentTemplate = new MalwareVisibility()
				{
					VisibleTo = MalwareVisibility.All,
				}
			});

		private static readonly Archetype YellowTutorialVirus = new Archetype("YellowTutorialVirus")
			.Extends(GameEntities.Malware)
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen,
				}
			})
			.HasComponent(new ComponentBinding<MalwareVisibility>()
			{
				ComponentTemplate = new MalwareVisibility()
				{
					VisibleTo = MalwareVisibility.All,
				}
			});

		#endregion

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int PlayerCountMin = 1;
			const int PlayerCountMax = 2;

			var nodeLeft = new NodeConfig()
			{
				Name = "Left",
				X = 0,
				Y = 0,
				ArchetypeName = GameEntities.Subsystem.Name,
			};

			var nodeMiddle = new NodeConfig()
			{
				Name = "Middle",
				X = 1,
				Y = 0,
				ArchetypeName = GameEntities.Subsystem.Name,
			};

			var nodeRight = new NodeConfig()
			{
				Name = "Right",
				X = 2,
				Y = 0,
				ArchetypeName = GameEntities.GarbageDisposalWorkstation.Name,
			};

			var nodeBottom = new NodeConfig()
			{
				Name = "Bottom",
				X = 1,
				Y = 1,
				ArchetypeName = GameEntities.AntivirusWorkstation.Name,
			};

			var nodeConfigs = new NodeConfig[] { nodeLeft, nodeRight, nodeMiddle, nodeBottom };
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);
			var itemConfigs = new ItemConfig[]
			{
				new ItemConfig()
				{
					ArchetypeName = GameEntities.Capture.Name,
					StartingLocation = nodeLeft.Id,
				},
				new ItemConfig()
				{
					ArchetypeName = GameEntities.Scanner.Name,
					StartingLocation = nodeMiddle.Id,
				}

			};
			var playerConfigFactory = new Func<int, PlayerConfig>(i => new PlayerConfig()
			{
				StartingLocation = nodeRight.Id,
				ArchetypeName = GameEntities.Player.Name
			});
			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs);

			configuration.RNGSeed = 897891658;

			configuration.Archetypes.AddRange(new []
			{
				TutorialScanner,
				RedTutorialVirus,
				GreenTutorialVirus,
				YellowTutorialVirus,
			});

			#endregion

			#region frames

			// ReSharper disable once UseObjectOrCollectionInitializer
			var frames = new List<SequenceFrame<Simulation, SimulationConfiguration>>();

			// frame 1
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.CreateNpcCommand(RedTutorialVirus.Name, nodeLeft.Id),
						ScenarioHelpers.CreateNpcCommand(GreenTutorialVirus.Name, nodeMiddle.Id),
						ScenarioHelpers.CreateNpcCommand(YellowTutorialVirus.Name, nodeRight.Id),

						ScenarioHelpers.CreateItemCommand(GameEntities.Capture.Name, nodeRight.Id),
						ScenarioHelpers.CreateItemCommand(GameEntities.Scanner.Name, nodeMiddle.Id),

						ScenarioHelpers.GenerateTextAction(true,
							"Welcome to IT Alert!", 
							"You are a system administrator tasked with maintaining the network.",
							"Let's get started..."),


					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
				}
			);
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.EndGame(EndGameState.Success),
					},
				}
			);
			#endregion

			configuration.LifeCycleConfiguration.TickInterval = 100;

			return new SimulationScenario()
			{
				Name = "Analysis",
				Description = "Analysis",
				MinPlayers = PlayerCountMin,
				MaxPlayers = PlayerCountMax,
				Configuration = configuration,

				CreatePlayerConfig = playerConfigFactory,

				// TODO: need a config driven specification for these
				Sequence = frames.ToArray(),
			};
		}
	}
}
