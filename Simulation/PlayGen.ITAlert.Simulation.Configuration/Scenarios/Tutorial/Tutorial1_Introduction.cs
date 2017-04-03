using System;
using System.Collections.Generic;
using System.Linq;
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

		#endregion

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int playerCount = 1;
			const int singlePlayerId = 0;

			#region graph

			var nodeLeft = new NodeConfig()
			{
				Name = "Left",
				X = 0,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeRight = new NodeConfig()
			{
				Name = "Right",
				X = 1,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
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

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs);

			configuration.Archetypes.Add(TutorialScanner);
			configuration.Archetypes.Add(RedTutorialVirus);

			#endregion

			var scenario = new SimulationScenario()
			{
				Name = "Introduction",
				Description = "Introduction",
				MinPlayers = playerCount,
				MaxPlayers = playerCount,
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(nameof(GameEntities.Player), new[] { nodeRight.Id }),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			#region frames

			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
						new ShowText(true,
							"Welcome to IT Alert!", 
							"You are a system administrator tasked with maintaining the network.",
							"Let's get started...")
					},
					Evaluator = new WaitForTutorialContinue(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new SetCommandEnabled<SetActorDestinationCommand>(true),
					},
				}
			);

			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false, 
							"Try navigating to another system by clicking on it...")
					},
					Evaluator = new PlayerIsAtLocation(nodeLeft.Id),
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
						new ShowText(false, 
							"Well done. Now you know how to move around the network!")
					},
					Evaluator = new WaitForSeconds(3),
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
						// TODO: this should disable the cancapture flag of the inventory slot
						new SetCommandEnabled<PickupItemCommand>(false),	
						new SetCommandEnabled<ActivateItemCommand>(false),
						new CreateItem(nameof(TutorialScanner), nodeRight.Id),
						new ShowText(true, 
							"The item that has just been spawned on the right is a scanner.",
							"",
							"The scanner reveals malware on a system when it is activated.")
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
						new ShowText(false, 
							"To use items you must be located on the same system...")
					},
					Evaluator = new PlayerIsAtLocation(nodeRight.Id),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						//ScenarioHelpers.SetCommandEnabled<SetActorDestinationCommand>(false),
					},
					// TODO: wait for tutorial currently must be the last evaluator in an AND check because the waithandle is reset after a successful read
				}
			);

			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<ActivateItemCommand>(true),
						new ShowText(false,
							"When you are on the same system as available items they will appear in the tray at the bottom of your screen.",
							"",
							"Clicking the item on your tray will activate the item...")
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
						new CreateMalware(nameof(RedTutorialVirus), nodeLeft.Id),
						new ShowText(true,
							"Malware has infected the Left system! You should investigate...",
							"",
							"You'll need to bring the scanner with you...")
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
						new SetCommandEnabled<PickupItemCommand>(true),
						new ShowText(false,
							"To pick up an item you must click on your inventory slot, and then on the item you want to pick up.",
							"",
							"The inventory slot is on the right, indicated by the case.")
					},
					Evaluator = new ItemTypeIsInInventory<Scanner>(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new SetCommandEnabled<DropItemCommand>(false),
					},
				}
			);

			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(false,
							"Now it is time to investigate.",
							"",
							"Make your way to the left system..")
					},
					Evaluator = new PlayerIsAtLocation(nodeLeft.Id),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
						new HideText(),
					},
				}
			);

			// frame 11
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ShowText(true,
							"You need to use the scanner tool to reveal the source of the problem."),
					},
					Evaluator = new WaitForTutorialContinue(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);

			// frame 12
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<DropItemCommand>(true),
						new ShowText(false,
							"First you will have to install the item on the current system",
							"",
							"Select your inventory and then click on one of the item slots in the panel at the bottom of the screen.")
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

			// frame 13
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateItem(nameof(GameEntities.RedAntivirus), nodeRight.Id),
						new ShowText(false,
							"You have now revealed the source of the infection.",
							"",
							"We've provided you with the necessary antivirus, you'll have to figure out the rest on your own...")
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
