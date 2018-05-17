using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Actions;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Scenario.Actions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;
using PlayGen.ITAlert.Simulation.Scenario.Localization;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial
{
	// ReSharper disable once InconsistentNaming
	public class Tutorial4_Mutation : ScenarioFactory
	{
		public Tutorial4_Mutation()
			: base(key: "Tutorial4",
				nameToken: "Tutorial4_Name",
				descriptionToken: "Tutorial4_Description",
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

			var node00 = new NodeConfig()
			{
				Name = "00",
				X = 0,
				Y = 0,
				Archetype = TutorialSubsystem.Archetype,
			};
			var node10 = new NodeConfig()
			{
				Name = "10",
				X = 1,
				Y = 0,
				Archetype = TutorialSubsystem.Archetype,
			};
			var node20 = new NodeConfig()
			{
				Name = "20",
				X = 2,
				Y = 0,
				Archetype = TutorialSubsystem.Archetype,
			};
			var node01 = new NodeConfig()
			{
				Name = "01",
				X = 0,
				Y = 1,
				Archetype = TutorialSubsystem.Archetype,
			};
			var node11 = new NodeConfig()
			{
				Name = "11",
				X = 1,
				Y = 1,
				Archetype = AntivirusWorkstation.Archetype,
			};
			var node21 = new NodeConfig()
			{
				Name = "21",
				X = 2,
				Y = 1,
				Archetype = TutorialSubsystem.Archetype,
			};


			var nodeConfigs = new NodeConfig[]
			{
				node00,
				node10,
				node20,
				node01,
				node11,
				node21,
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var connection0010 = new EdgeConfig(node00.Id, EdgeDirection.East, node10.Id, ConnectionNode.Archetype);
			var connection1000 = connection0010.Reciprocate();

			var connection1020 = new EdgeConfig(node10.Id, EdgeDirection.East, node20.Id, ConnectionNode.Archetype);
			var connection2010 = connection1020.Reciprocate();

			var connection1011 = new EdgeConfig(node10.Id, EdgeDirection.South, node11.Id, ConnectionNode.Archetype);
			var connection1110 = connection1011.Reciprocate();

			var connection0111 = new EdgeConfig(node01.Id, EdgeDirection.East, node11.Id, ConnectionNode.Archetype);
			var connection1101 = connection0111.Reciprocate();

			var connection1121 = new EdgeConfig(node11.Id, EdgeDirection.East, node21.Id, ConnectionNode.Archetype);
			var connection2111 = connection1121.Reciprocate();

			var edgeConfigs = new EdgeConfig[]
			{
				connection0010,
				connection1000,
				connection1020,
				connection2010,
				connection1011,
				connection1110,
				connection0111,
				connection1101,
				connection1121,
				connection2111,
			};

			#endregion

			var archetypes = new List<Archetype>
			{
				TutorialSubsystem.Archetype,
				ConnectionNode.Archetype,
				AntivirusWorkstation.Archetype,
				TutorialCapture.Archetype,
				AnalyserActivator.Archetype,
				Player.Archetype,
				AntivirusTool.Archetype,
				VisibleRedTutorialVirus.Archetype,
				VisibleGreenTutorialVirus.Archetype,
				TutorialText.Archetype,
				TutorialNPC.Archetype,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);
			configuration.RNGSeed = 2007196112;
			
			#endregion

			var scenario = new SimulationScenario(ScenarioInfo)
			{
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new[] {node21.Id}),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(VisibleRedTutorialVirus.Archetype, node00),
						new ShowText(true, $"{scenario.Key}_Frame1")
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
						new SetHighlight(node10),
						new CreateMalware(VisibleRedTutorialVirus.Archetype, connection0010),
						new ShowText(true, $"{scenario.Key}_Frame2")
					},
					ExitCondition = new IsInfected(node10)
						.And(new WaitForTutorialContinue()),
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
						new CreateMalware(VisibleRedTutorialVirus.Archetype, connection0010),
						new ShowText(true, $"{scenario.Key}_Frame3")
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(connection0010))
						.And(new WaitForTutorialContinue()),
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
						new CreateMalware(VisibleGreenTutorialVirus.Archetype, node20),
						new CreateMalware(VisibleGreenTutorialVirus.Archetype, connection2010),
						new ShowText(true, $"{scenario.Key}_Frame4")
					},
					ExitCondition = new IsInfected(node10, new MalwareGenomeFilter(SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen))
						.And(new WaitForTutorialContinue()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ClearHighlight(),
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
						new CreateItem(TutorialCapture.Archetype, node11),
						new ShowText(false, $"{scenario.Key}_Frame5")
					},
					ExitCondition = new PlayerIsAtLocation(node10).And(new ItemTypeIsActivated<Capture>(activationState: ActivationState.Deactivating)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new ClearHighlight(),
					},
				}
			);
			// 6
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
						new SetCommandEnabled<PickupItemCommand>(false),
						new ShowText(true, $"{scenario.Key}_Frame6")
					},
					ExitCondition = new WaitForTutorialContinue(),
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
					ExitCondition = new ItemTypeIsActivated<Capture>(activationState: ActivationState.Deactivating),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new SetCommandEnabled<SetActorDestinationCommand>(true),
						new SetCommandEnabled<PickupItemCommand>(true),
					},
				}
			);
			// 8
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetHighlight(node10),
						new ShowText(false, $"{scenario.Key}_Frame8")
					},
					ExitCondition = new ItemTypeIsInStorageAtLocation<Antivirus>(node10, new AntivirusGenomeFilter(SimulationConstants.MalwareGeneGreen))
						.And(new ItemTypeIsInStorageAtLocation<Antivirus>(node10, new AntivirusGenomeFilter(SimulationConstants.MalwareGeneRed))),
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
						new CreatePlayer(TutorialNPC.Archetype, node10, "Colleague"),
						new ShowText(false, $"{scenario.Key}_Frame9")
					},
					ExitCondition = new ItemTypeIsActivated<Antivirus>(node10),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ClearHighlight(),
					},
				}
			);
			// 10
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EnqueuePlayerCommand(new ActivateItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,
						}),
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(node10)),
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
					},
					ExitCondition = new WaitForTicks(5),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EndGame(EndGameState.Success),
					},
				}
			);

			#endregion

			return scenario;
		}
	}
}
