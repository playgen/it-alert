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

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int playerCountMin = 1;
			const int playerCountMax = 1;

			#region graph

			var nodeLeft = new NodeConfig()
			{
				Name = "Left",
				X = 0,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			var nodeTop = new NodeConfig()
			{
				Name = "Top",
				X = 1,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			var nodeRight = new NodeConfig()
			{
				Name = "Right",
				X = 2,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
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
				nodeTop,
				nodeBottom
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);

			#endregion

			var archetypes = new List<Archetype>
			{
				SubsystemNode.Archetype,
				ConnectionNode.Archetype,
				Player.Archetype,
				ScannerTool.Archetype,
				CaptureTool.Archetype,
				AntivirusWorkstation.Archetype,
				AnalyserActivator.Archetype,
				AntivirusTool.Archetype,
				RedTutorialVirus.Archetype,
				GreenTutorialVirus.Archetype,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);

			configuration.RNGSeed = 897891658;

			#endregion

			var scenario = new SimulationScenario()
			{
				Key = "Tutorial2",
				Name = "Tutorial2_Name",
				Description = "Tutorial2_Description",
				MinPlayers = playerCountMin,
				MaxPlayers = playerCountMax,
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
						new CreateItem(ScannerTool.Archetype, nodeRight.Id),
						new ShowText(true, $"{scenario.Key}_Frame1"),
					},
					Evaluator = new WaitForTutorialContinue(),
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
						new ShowText(true, $"{scenario.Key}_Frame2"),
					},
					Evaluator = new WaitForTutorialContinue(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 3
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(RedTutorialVirus.Archetype, nodeLeft.Id),
						new ShowText(false, $"{scenario.Key}_Frame3"),
					},
					Evaluator = new GenomeRevealedAtLocation(nodeLeft.Id, SimulationConstants.MalwareGeneRed),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 4
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateItem(CaptureTool.Archetype, nodeBottom.Id),
						new ShowText(true, $"{scenario.Key}_Frame4"),
					},
					Evaluator = new WaitForTutorialContinue(),
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
					Evaluator = new PlayerIsAtLocation(nodeBottom.Id),
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
					Evaluator = EvaluatorExtensions.Not(new ItemTypeIsInInventory<Scanner>()),
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
					Evaluator = new ItemTypeIsInInventory<Capture>(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
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
					Evaluator = new PlayerIsAtLocation(nodeLeft.Id),
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
					Evaluator = new GenomeIsCaptured(SimulationConstants.MalwareGeneRed),
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
					Evaluator = new PlayerIsAtLocation(nodeBottom.Id).And(new ItemTypeIsInInventory<Capture>()),
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
					Evaluator = new ItemTypeIsInStorageAtLocation<Antivirus>(nodeBottom.Id),
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
					Evaluator = EvaluatorExtensions.Not(new IsInfected(nodeLeft.Id)),
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
						new CreateMalware(GreenTutorialVirus.Archetype, nodeTop.Id),
						new ShowText(false, $"{scenario.Key}_Frame13"),
					},
					Evaluator = new GenomeRevealedAtLocation(nodeTop.Id, SimulationConstants.MalwareGeneGreen),
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
					Evaluator = EvaluatorExtensions.Not(new IsInfected(nodeTop.Id)),
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
