﻿using System;
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
	internal static class Tutorial2_Analysis
	{
		private static SimulationScenario _scenario;
		public static SimulationScenario Scenario => _scenario ?? (_scenario = GenerateScenario());

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
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
				GreenTutorialVirus.Archetype,
				TutorialText.Archetype,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);
			configuration.RNGSeed = 897891658;

			#endregion

			var scenario = new SimulationScenario()
			{
				Key = "Tutorial2",
				Name = "Tutorial2_Name",
				Description = "Tutorial2_Description",
				MinPlayers = 1,
				MaxPlayers = 1,
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new [] { nodeRight.Id }),

				// TODO: need a config driven specification for these
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};
			scenario.LocalizationDictionary = LocalizationHelper.GetLocalizationFromEmbeddedResource(scenario.Key);

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateItem(ScannerTool.Archetype, nodeRight),
						new ShowText(true, $"{scenario.Key}_Frame1"),
					},
					ExitCondition = new WaitForTutorialContinue(),
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
					ExitCondition = new WaitForTutorialContinue(),
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
						new CreateMalware(RedTutorialVirus.Archetype, nodeLeft),
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
					ExitCondition = new WaitForTutorialContinue(),
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
					ExitCondition = new PlayerIsAtLocation(nodeAntivirus.Id),
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
					ExitCondition = EvaluatorExtensions.Not(new ItemTypeIsInInventory<Scanner>()),
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
					ExitCondition = new PlayerIsAtLocation(nodeLeft.Id),
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
					ExitCondition = new PlayerIsAtLocation(nodeAntivirus.Id).And(new ItemTypeIsInInventory<Capture>()),
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
						new CreateMalware(GreenTutorialVirus.Archetype, nodeMiddle),
						new ShowText(false, $"{scenario.Key}_Frame13"),
					},
					ExitCondition = new GenomeRevealedAtLocation(nodeMiddle, SimulationConstants.MalwareGeneGreen),
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
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(nodeMiddle)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
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
