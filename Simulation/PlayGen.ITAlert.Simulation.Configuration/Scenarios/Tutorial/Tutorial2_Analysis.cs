using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Tutorial;
using PlayGen.ITAlert.Simulation.Scenario.Actions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial
{
	// ReSharper disable once InconsistentNaming
	internal static class Tutorial2_Analysis
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
			.RemoveComponent<MalwarePropogation>()
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneRed,
				}
			});

		private static readonly Archetype GreenTutorialVirus = new Archetype("GreenTutorialVirus")
			.Extends(GameEntities.Malware)
			.RemoveComponent<MalwarePropogation>()
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneGreen,
				}
			});
			//.HasComponent(new ComponentBinding<MalwareVisibility>()
			//{
			//	ComponentTemplate = new MalwareVisibility()
			//	{
			//		VisibleTo = MalwareVisibility.All,
			//	}
			//});

		private static readonly Archetype YellowTutorialVirus = new Archetype("YellowTutorialVirus")
			.Extends(GameEntities.Malware)
			.RemoveComponent<MalwarePropogation>()
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen,
				}
			});

		#endregion

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int playerCountMin = 1;
			const int playerCountMax = 1;

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

			var nodeConfigs = new NodeConfig[] { nodeLeft,
				nodeRight,
				nodeMiddle,
				nodeBottom
			};
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

			var scenario = new SimulationScenario()
			{
				Name = "Analysis",
				Description = "Analysis",
				MinPlayers = playerCountMin,
				MaxPlayers = playerCountMax,
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Archetypes.Player.Archetype.Name, new [] { nodeRight.Id }),

				// TODO: need a config driven specification for these
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			#region frames

			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(RedTutorialVirus.Name, nodeLeft.Id),
						new CreateMalware(GreenTutorialVirus.Name, nodeMiddle.Id),
						new CreateMalware(YellowTutorialVirus.Name, nodeRight.Id),

						new CreateItem(GameEntities.Capture.Name, nodeRight.Id),
						new CreateItem(GameEntities.Scanner.Name, nodeMiddle.Id),

						new ShowText(true,
							"In this scenario you will learn about Viruses and how to analyse them and produce antivirus", 
							"",
							"Let's get started..."),


					},
					Evaluator = new WaitForTutorialContinue(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					Evaluator = EvaluatorExtensions.Not(new IsInfected()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EndGame(EndGameState.Success),
					},
				}
			);
			#endregion

			configuration.LifeCycleConfiguration.TickInterval = 100;

			return scenario;
		}
	}
}
