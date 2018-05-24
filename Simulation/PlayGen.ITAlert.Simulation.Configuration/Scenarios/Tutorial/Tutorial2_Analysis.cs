using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Actions;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;
using PlayGen.ITAlert.Simulation.Scenario.Actions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
using PlayGen.ITAlert.Simulation.Scenario.Localization;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial
{
	// ReSharper disable once InconsistentNaming
	internal class Tutorial2_Analysis : ScenarioFactory
	{
		public Tutorial2_Analysis()
			: base(key: "Tutorial2",
				nameToken: "Tutorial2_Name",
				descriptionToken: "Tutorial2_Description",
				minPlayers: 1,
				maxPlayers: 1,
                timeLimitSeconds: null)
		{

		}
		
		// TODO: this should be parameterized further and read from config
		public override SimulationScenario GenerateScenario()
		{
			#region configuration

			#region graph

			var nodeLeft = new NodeConfig()
			{
				Name = "00",
				X = 0,
				Y = 0,
				Archetype = TutorialSubsystem.Archetype,
			};

			var nodeMiddle = new NodeConfig()
			{
				Name = "10",
				X = 1,
				Y = 0,
				Archetype = TutorialSubsystem.Archetype,
			};

			var nodeRight = new NodeConfig()
			{
				Name = "20",
				X = 2,
				Y = 0,
				Archetype = TutorialSubsystem.Archetype,
			};

			var nodeAntivirus = new NodeConfig()
			{
				Name = "Antivirus",
				X = 1,
				Y = 1,
				Archetype = TutorialAntivirusWorkstation.Archetype,
			};

			var nodeConfigs = new NodeConfig[] {
				nodeLeft,
				nodeRight,
				nodeMiddle,
				nodeAntivirus
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);

			#endregion

			var archetypes = new List<Archetype>
			{
				TutorialSubsystem.Archetype,
				ConnectionNode.Archetype,
				Player.Archetype,
				ScannerTool.Archetype,
				TutorialScanner.Archetype,
				TutorialCapture.Archetype,
				TutorialAntivirusWorkstation.Archetype,
				AnalyserActivator.Archetype,
				AntivirusTool.Archetype,
				RedTutorialVirus.Archetype,
				VisibleGreenTutorialVirus.Archetype,
				TutorialText.Archetype,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);
			configuration.RNGSeed = 897891658;

			#endregion

			var scenario = new SimulationScenario(ScenarioInfo)
			{
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new [] { nodeRight.Id }),

				// TODO: need a config driven specification for these
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateItem(ScannerTool.Archetype, nodeRight),
						new ShowText(true, $"{scenario.Key}_Frame1"),
						new CreateMalware(RedTutorialVirus.Archetype, nodeLeft),
					},
					ExitCondition = new WaitForTutorialContinue().Or(new GenomeRevealedAtLocation(nodeLeft, SimulationConstants.MalwareGeneRed)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 2
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetHighlight(nodeAntivirus),
						new ShowText(true, $"{scenario.Key}_Frame2"),
					},
					ExitCondition = new WaitForTutorialContinue().Or(new GenomeRevealedAtLocation(nodeLeft, SimulationConstants.MalwareGeneRed)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new ClearHighlight(),
					},
				}
			);
			// 3
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetHighlight(nodeLeft),
						new ShowText(false, $"{scenario.Key}_Frame3"),
					},
					ExitCondition = new GenomeRevealedAtLocation(nodeLeft, SimulationConstants.MalwareGeneRed),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new ClearHighlight(),
					},
				}
			);
			// 4
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateItem(TutorialCapture.Archetype, nodeAntivirus),
						new ShowText(true, $"{scenario.Key}_Frame4"),
					},
					ExitCondition = new ItemTypeIsInInventory<Capture>().Or(new WaitForTutorialContinue()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 5
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame5"),
					},
					ExitCondition = new PlayerIsAtLocation(nodeAntivirus).Or(new ItemTypeIsInInventory<Capture>()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 6
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame6"),
					},
					ExitCondition = EvaluatorExtensions.Not(new ItemTypeIsInInventory<Scanner>()).Or(new ItemTypeIsInInventory<Capture>()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 7
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame7"),
					},
					ExitCondition = new ItemTypeIsInInventory<Capture>(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new ClearHighlight(),
					},
				}
			);
			// 8
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame8"),
					},
					ExitCondition = new PlayerIsAtLocation(nodeLeft).Or(new GenomeIsCaptured(SimulationConstants.MalwareGeneRed)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 9
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame9"),
					},
					ExitCondition = new GenomeIsCaptured(SimulationConstants.MalwareGeneRed),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 10
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame10"),
					},
					ExitCondition = new PlayerIsAtLocation(nodeAntivirus).And(new ItemTypeIsInInventory<Capture>()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 11
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame11"),
					},
					ExitCondition = new ItemTypeIsInStorageAtLocation<Antivirus>(nodeAntivirus),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 12
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame12"),
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(nodeLeft)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);

			// 13
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(VisibleGreenTutorialVirus.Archetype, nodeLeft),
						new CreateMalware(VisibleGreenTutorialVirus.Archetype, nodeMiddle),
						new CreateMalware(VisibleGreenTutorialVirus.Archetype, nodeRight),
						new ShowText(true, $"{scenario.Key}_Frame13"),
					},
					ExitCondition = new WaitForTutorialContinue().Or(EvaluatorExtensions.Not(new IsInfected())),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 14
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame14"),
					},
					ExitCondition = new EntityDisposed<Malware>().Or(EvaluatorExtensions.Not(new IsInfected())),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 15
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, $"{scenario.Key}_Frame15"),
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 16
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					ExitCondition = new WaitForTicks(5),
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
