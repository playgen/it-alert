using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;
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

		private static readonly Archetype TutorialScanner = new Archetype(nameof(TutorialScanner))
			.Extends(ScannerTool.Archetype)
			.HasComponent(new ComponentBinding<ActivationContinue>()
			{
				ComponentTemplate = new ActivationContinue()
				{
					ContinueOn = ActivationContinue.ActivationPhase.Deactivating,
				}
			});

		private static readonly Archetype RedTutorialVirus = new Archetype(nameof(RedTutorialVirus))
			.Extends(Virus.Archetype)
			.RemoveComponent<MalwarePropogation>()
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneRed,
				}
			});

		private static readonly Archetype GreenTutorialVirus = new Archetype(nameof(GreenTutorialVirus))
			.Extends(Virus.Archetype)
			.RemoveComponent<MalwarePropogation>()
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneGreen,
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
				Archetype = SubsystemNode.Archetype,
			};

			var nodeMiddle = new NodeConfig()
			{
				Name = "Middle",
				X = 1,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			var nodeRight = new NodeConfig()
			{
				Name = "Right",
				X = 2,
				Y = 0,
				Archetype = GarbageDisposalWorkstation.Archetype,
			};

			var nodeBottom = new NodeConfig()
			{
				Name = "Bottom",
				X = 1,
				Y = 1,
				Archetype = AntivirusWorkstation.Archetype,
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
					Archetype = TutorialScanner,
					StartingLocation = nodeLeft.Id,
				},
				new ItemConfig()
				{
					Archetype = ScannerTool.Archetype,
					StartingLocation = nodeMiddle.Id,
				}
			};

			var archetypes = new List<Archetype>
			{
				SubsystemNode.Archetype,
				ConnectionNode.Archetype,
				Player.Archetype,

				TutorialScanner,
				RedTutorialVirus,
				GreenTutorialVirus,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs, archetypes);

			configuration.RNGSeed = 897891658;

			#endregion

			var scenario = new SimulationScenario()
			{
				Name = "Analysis",
				Description = "Analysis",
				MinPlayers = playerCountMin,
				MaxPlayers = playerCountMax,
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new [] { nodeRight.Id }),

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

						new CreateItem(CaptureTool.Archetype, nodeRight.Id),
						new CreateItem(TutorialScanner, nodeMiddle.Id),

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
