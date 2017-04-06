using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
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
	internal static class Tutorial1_Introduction
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

		private static readonly Archetype RedTutorialAntivirus = new Archetype(nameof(RedTutorialAntivirus))
			.Extends(AntivirusTool.Archetype)
			.HasComponent(new ComponentBinding<Antivirus>()
			{
				ComponentTemplate = new Antivirus()
				{ 
					TargetGenome = SimulationConstants.MalwareGeneRed,
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

		#endregion

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int playerCount = 1;
			const int singlePlayerId = 0;

			var text = new Dictionary<string, Dictionary<string, string>>();

			#region graph

			var nodeLeft = new NodeConfig()
			{
				Name = "Left",
				X = 0,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			var nodeRight = new NodeConfig()
			{
				Name = "Right",
				X = 1,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			#endregion

			var nodeConfigs = new NodeConfig[]
			{
				nodeLeft,
				nodeRight
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedGridConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, 1);
			var itemConfigs = new ItemConfig[0];

			var archetypes = new List<Archetype>
			{
				SubsystemNode.Archetype,
				ConnectionNode.Archetype,
				Player.Archetype,
				TutorialScanner,
				RedTutorialAntivirus,
				RedTutorialVirus,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs, archetypes);

			#endregion

			var scenario = new SimulationScenario()
			{
				Name = "Tutorial1",
				Description = "Introduction",
				MinPlayers = playerCount,
				MaxPlayers = playerCount,
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new[] { nodeRight.Id }),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
						new ShowText(true, "Tutorial1_Frame1")
					},
					Evaluator = new WaitForTutorialContinue(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new SetCommandEnabled<SetActorDestinationCommand>(true),
					},
				}
			);
			// 2
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, "Tutorial1_Frame2")
					},
					Evaluator = new PlayerIsAtLocation(nodeLeft.Id),
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
						new ShowText(false, "Tutorial1_Frame3"),
					},
					Evaluator = new WaitForSeconds(3),
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
						// TODO: this should disable the cancapture flag of the inventory slot
						new SetCommandEnabled<PickupItemCommand>(false),	
						new SetCommandEnabled<ActivateItemCommand>(false),
						new CreateItem(TutorialScanner, nodeRight.Id),
						new ShowText(true, "Tutorial1_Frame4"),
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
						new ShowText(false, "Tutorial1_Frame5"),
					},
					Evaluator = new PlayerIsAtLocation(nodeRight.Id),
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
						new SetCommandEnabled<ActivateItemCommand>(true),
						new ShowText(false,"Tutorial1_Frame6"),
					},
					Evaluator = new ItemTypeIsActivated<Scanner>(),
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
						new CreateMalware(RedTutorialVirus, nodeLeft.Id),
						new ShowText(true, "Tutorial1_Frame7"),
					},
					Evaluator = new WaitForTutorialContinue(),
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
						new SetCommandEnabled<PickupItemCommand>(true),
						new ShowText(false, "Tutorial1_Frame8")
					},
					Evaluator = new ItemTypeIsInInventory<Scanner>(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
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
						new ShowText(false, "Tutorial1_Frame9"),
					},
					Evaluator = new PlayerIsAtLocation(nodeLeft.Id),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
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
						new ShowText(true, "Tutorial1_Frame10"),
					},
					Evaluator = new WaitForTutorialContinue(),
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
						new SetCommandEnabled<DropItemCommand>(true),
						new ShowText(false, "Tutorial1_Frame11"),
					},
					Evaluator = new ItemTypeIsInStorageAtLocation<Scanner>(nodeLeft.Id)
						.And(new WaitForTutorialContinue()),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(true),
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
						new CreateItem(RedTutorialAntivirus, nodeRight.Id),
						new ShowText(false, "Tutorial1_Frame12"),
					},
					Evaluator = EvaluatorExtensions.Not(new IsInfected(nodeLeft.Id)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new EndGame(EndGameState.Success),
					},
				}
			);

			#endregion

			return scenario;
		}
	}
}
