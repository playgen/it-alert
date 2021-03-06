﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Modules.Transfer;
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
	public class Tutorial3_Bandwidth : ScenarioFactory
	{
		public Tutorial3_Bandwidth()
			: base(key: "Tutorial3",
				nameToken: "Tutorial3_Name",
				descriptionToken: "Tutorial3_Description",
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

			var nodeT1 = new NodeConfig()
			{
				Name = "00",
				X = 0,
				Y = 0,
				Archetype = TransferWorkstation.Archetype,
			};
			var node10 = new NodeConfig()
			{
				Name = "10",
				X = 1,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};
			var node01 = new NodeConfig()
			{
				Name = "01",
				X = 0,
				Y = 1,
				Archetype = SubsystemNode.Archetype,
			};
			var node11 = new NodeConfig()
			{
				Name = "11",
				X = 1,
				Y = 1,
				Archetype = SubsystemNode.Archetype,
			};
			var node21 = new NodeConfig()
			{
				Name = "21",
				X = 2,
				Y = 1,
				Archetype = SubsystemNode.Archetype,
			};
			var node30 = new NodeConfig()
			{
				Name = "30",
				X = 3,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};
			var node31 = new NodeConfig()
			{
				Name = "31",
				X = 3,
				Y = 1,
				Archetype = SubsystemNode.Archetype,
			};
			var node40 = new NodeConfig()
			{
				Name = "40",
				X = 4,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};
			var nodeT2 = new NodeConfig()
			{
				Name = "41",
				X = 4,
				Y = 1,
				Archetype = TransferWorkstation.Archetype,
			};

			var nodeConfigs = new NodeConfig[]
			{
				nodeT1,
				node10,
				node01,
				node11,
				node21,
				node30,
				node31,
				node40,
				nodeT2,
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);
			edgeConfigs = edgeConfigs.Where(e => !(e.Source == nodeT1.Id && e.Destination == node10.Id) && !(e.Source == node10.Id && e.Destination == nodeT1.Id)).ToList();

			#endregion

			var archetypes = new List<Archetype>
			{
				SubsystemNode.Archetype,
				ConnectionNode.Archetype,
				TransferWorkstation.Archetype,
				TransferActivator.Archetype,
				Player.Archetype,
				TutorialNPC.Archetype,
				ScannerTool.Archetype,
				RedTutorialAntivirus.Archetype,
				VisibleRedTutorialVirus.Archetype,
				GreenTutorialAntivirus.Archetype,
				VisibleGreenTutorialVirus.Archetype,
				TutorialText.Archetype,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);
			configuration.RNGSeed = 477359007;

			#endregion

			var scenario = new SimulationScenario(ScenarioInfo)
			{
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new[] { node40.Id }),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};
			
			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(VisibleRedTutorialVirus.Archetype, node10),
						new CreateMalware(VisibleRedTutorialVirus.Archetype, node01),
						new CreateMalware(VisibleRedTutorialVirus.Archetype, node11),
						new CreateMalware(VisibleGreenTutorialVirus.Archetype, node21),
						new CreateMalware(VisibleGreenTutorialVirus.Archetype, node30),
						new CreateMalware(VisibleGreenTutorialVirus.Archetype, node31),
						new ShowText(true, $"{scenario.Key}_Frame1")
					},
					ExitCondition = new PlayerIsAtLocation(nodeT1).Or(new WaitForTutorialContinue()),
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
						new ShowText(true, $"{scenario.Key}_Frame2")
					},
					ExitCondition = new PlayerIsAtLocation(nodeT1).Or(new WaitForTutorialContinue()),
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
						new CreateItem(GreenTutorialAntivirus.Archetype, nodeT1),
						new SetCommandEnabled<ActivateItemCommand>(false),
						new ShowText(false, $"{scenario.Key}_Frame3")
					},
					ExitCondition = new PlayerIsAtLocation(nodeT1)
						.And(new ItemTypeIsInInventory<Antivirus>(filter: new AntivirusGenomeFilter(SimulationConstants.MalwareGeneGreen)).Or(new ItemTypeIsInStorageAtLocation<Antivirus, TransferItemContainer>(nodeT1))),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new ClearHighlight(),
						new SetCommandEnabled<DropItemCommand>(false),
						new SetCommandEnabled<DropAndActivateItemCommand>(false),
						new SetCommandEnabled<SetActorDestinationCommand>(false),
					},
				}
			);
			// 4
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateItem(RedTutorialAntivirus.Archetype, nodeT2, typeof(TransferItemContainer)),
						new SetCommandEnabled<MoveItemCommand>(false),
						new ShowText(true, $"{scenario.Key}_Frame4"),
					},
					ExitCondition = new ItemTypeIsInStorageAtLocation<Antivirus, TransferItemContainer>(nodeT1).Or(new WaitForTutorialContinue()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new ClearHighlight(),
					},
				}
			);
			// 5
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<DropItemCommand>(true),
						new ShowText(false, $"{scenario.Key}_Frame5"),
					},
					ExitCondition = new ItemTypeIsInStorageAtLocation<Antivirus, TransferItemContainer>(nodeT1),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<PickupItemCommand>(false),
						new SetCommandEnabled<ActivateItemCommand>(true),
					},
				}
			);
			// 6
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					ExitCondition = new ItemTypeIsInStorageAtLocation<Antivirus, TransferItemContainer>(nodeT2, new AntivirusGenomeFilter(SimulationConstants.MalwareGeneGreen))
						.And(new ItemTypeIsInStorageAtLocation<Antivirus, TransferItemContainer>(nodeT1, new AntivirusGenomeFilter(SimulationConstants.MalwareGeneRed))),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new SetCommandEnabled<PickupItemCommand>(true),
						new SetCommandEnabled<MoveItemCommand>(true),
						new SetCommandEnabled<DropItemCommand>(false),
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
					ExitCondition = new WaitForTicks(1),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreatePlayer(TutorialNPC.Archetype, nodeT2, "Colleague"),
						new SetCommandEnabled<ActivateItemCommand>(false),
					},
				}
			);

			#region NPC behaviour
			// 8
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(true),
					},
					ExitCondition = new WaitForTicks(10),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EnqueuePlayerCommand(new PickupItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
						new SetDestination(node30, 1),
					},
				}
			);

			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					ExitCondition = new ItemTypeIsInInventory<Antivirus>().And(new PlayerIsAtLocation(node01)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
						new SetCommandEnabled<ActivateItemCommand>(true),
						new SetCommandEnabled<DropAndActivateItemCommand>(true),
						new SetCommandEnabled<DropItemCommand>(true),
					},
				}
			);
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(node01)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(true),
						new SetCommandEnabled<ActivateItemCommand>(false),
						new SetCommandEnabled<DropAndActivateItemCommand>(false),
						new SetCommandEnabled<DropItemCommand>(false),
					},
				}
			);

			// 9
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<DropItemCommand>(true),
					},
					ExitCondition = new PlayerIsAtLocation(node30, 1),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EnqueuePlayerCommand(new DropItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
						new SetCommandEnabled<DropItemCommand>(false),
						new SetCommandEnabled<ActivateItemCommand>(true),
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
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
						new SetCommandEnabled<SetActorDestinationCommand>(true),
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(node30)).And(new WaitForTicks(1)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<ActivateItemCommand>(false),
						new EnqueuePlayerCommand(new PickupItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
						new SetDestination(node31, 1),
					},
				}
			);

			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					ExitCondition = new ItemTypeIsInInventory<Antivirus>().And(new PlayerIsAtLocation(node11)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
						new SetCommandEnabled<ActivateItemCommand>(true),
						new SetCommandEnabled<DropAndActivateItemCommand>(true),
						new SetCommandEnabled<DropItemCommand>(true),
					},
				}
			);
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(node11)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(true),
						new SetCommandEnabled<ActivateItemCommand>(false),
						new SetCommandEnabled<DropAndActivateItemCommand>(false),
						new SetCommandEnabled<DropItemCommand>(false),
					},
				}
			);

			// 11
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<DropItemCommand>(true),
					},
					ExitCondition = new PlayerIsAtLocation(node31, 1),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EnqueuePlayerCommand(new DropItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
						new SetCommandEnabled<DropItemCommand>(false),
						new SetCommandEnabled<ActivateItemCommand>(true),
					},
				}
			);
			// 12
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EnqueuePlayerCommand(new ActivateItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
						new SetCommandEnabled<SetActorDestinationCommand>(true),
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(node31)).And(new WaitForTicks(1)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<ActivateItemCommand>(false),
						new EnqueuePlayerCommand(new PickupItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
						new SetDestination(node21, 1),
					},
				}
			);

			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					ExitCondition = new ItemTypeIsInInventory<Antivirus>().And(new PlayerIsAtLocation(node10)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
						new SetCommandEnabled<ActivateItemCommand>(true),
						new SetCommandEnabled<DropAndActivateItemCommand>(true),
						new SetCommandEnabled<DropItemCommand>(true),
					},
				}
			);
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected(node10)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(true),
						new SetCommandEnabled<ActivateItemCommand>(false),
						new SetCommandEnabled<DropAndActivateItemCommand>(false),
						new SetCommandEnabled<DropItemCommand>(false),
					},
				}
			);

			// 13
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<DropItemCommand>(true),
					},
					ExitCondition = new PlayerIsAtLocation(node21, 1),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EnqueuePlayerCommand(new DropItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
						new SetCommandEnabled<DropItemCommand>(false),
						new SetCommandEnabled<ActivateItemCommand>(true),
					},
				}
			);
			// 14
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EnqueuePlayerCommand(new ActivateItemTypeCommand()
						{
							ItemType = typeof(Antivirus),
							PlayerId = 1,	// TODO: find the npc player id automatically
						}),
					},
					ExitCondition = EvaluatorExtensions.Not(new IsInfected()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<ActivateItemCommand>(false),
						new HideText(),
					},
				}
			);
			#endregion
			// 15
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
