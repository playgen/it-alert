﻿using System;
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
			.Extends(GameEntities.CPUVirus)
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneRed,
				}
			});

		private static readonly Archetype GreenTutorialVirus = new Archetype("GreenTutorialVirus")
			.Extends(GameEntities.CPUVirus)
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

			var nodeLeft = new NodeConfig()
			{
				Name = "Left",
				X = 0,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeMiddle = new NodeConfig()
			{
				Name = "Middle",
				X = 1,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeRight = new NodeConfig()
			{
				Name = "Right",
				X = 2,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeBottom = new NodeConfig()
			{
				Name = "Bottom",
				X = 1,
				Y = 1,
				ArchetypeName = nameof(GameEntities.AntivirusWorkstation),
			};

			var nodeConfigs = new NodeConfig[] { nodeLeft, nodeRight, nodeMiddle, nodeBottom };
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);
			var itemConfigs = new ItemConfig[0];
			var playerConfigFactory = new Func<int, PlayerConfig>(i => new PlayerConfig()
			{
				StartingLocation = nodeRight.Id,
				ArchetypeName = nameof(GameEntities.Player)
			});
			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs);

			configuration.Archetypes.Add(TutorialScanner);
			configuration.Archetypes.Add(RedTutorialVirus);

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
						ScenarioHelpers.SetCommandEnabled<SetActorDestinationCommand>(false),
						ScenarioHelpers.GenerateTextAction(true,
							"Welcome to IT Alert!", 
							"You are a system administrator tasked with maintaing the network.",
							"Let's get started...")
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
						ScenarioHelpers.SetCommandEnabled<SetActorDestinationCommand>(true),
					},
				}
			);

			// frame 2
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(false, 
							"Try navigating to another system by clicking on it...")
					},
					Evaluator = ScenarioHelpers.OnlyPlayerIsAtLocation(nodeLeft),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
				}
			);

			// frame 3
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(false, 
							"Well done. Now you know how to move around the network!")
					},
					Evaluator = ScenarioHelpers.WaitForSeconds(3),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
				}
			);

			// frame 4
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						// TODO: this should disable the cancapture flag of the inventory slot
						ScenarioHelpers.SetCommandEnabled<PickupItemCommand>(false),	
						ScenarioHelpers.SetCommandEnabled<ActivateItemCommand>(false),
						ScenarioHelpers.CreateItemCommand(nameof(TutorialScanner), nodeRight.Id),
						ScenarioHelpers.GenerateTextAction(true, 
							"The item that has just been spawned on the right is a scanner.",
							"",
							"The scanner reveals malware on a system when it is activated.")
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
				}
			);

			// frame 5
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(false, 
							"To use items you must be located on the same system...")
					},
					Evaluator = ScenarioHelpers.OnlyPlayerIsAtLocation(nodeRight),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
						//ScenarioHelpers.SetCommandEnabled<SetActorDestinationCommand>(false),
					},
					// TODO: wait for tutorial currently must be the last evaluator in an AND check because the waithandle is reset after a successful read
				}
			);

			// frame 6
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.SetCommandEnabled<ActivateItemCommand>(true),
						ScenarioHelpers.GenerateTextAction(false,
							"When you are on the same system as available items they will appear in the tray at the bottom of your screen.",
							"",
							"Clicking the item on your tray will activate the item...")
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
				}
			);

			//// frame 7
			//frames.Add(
			//	new SimulationFrame()
			//	{
			//		OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
			//		{
			//			ScenarioHelpers.GenerateTextAction(false,
			//				"While the item is active the colour changes to indicate the player that is performing the action."),
			//		},
			//		Evaluator = ScenarioHelpers.WaitForSeconds(3),
			//		OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
			//		{
			//			ScenarioHelpers.HideTextAction,
			//		},
			//	}
			//);

			// frame 8
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.CreateNpcCommand(nameof(RedTutorialVirus), nodeLeft.Id),
						ScenarioHelpers.GenerateTextAction(true,
							"Malware has infected the Left system! You should investigate...",
							"",
							"You'll need to bring the scanner with you...")
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
				}
			);

			// frame 9
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.SetCommandEnabled<PickupItemCommand>(true),
						ScenarioHelpers.GenerateTextAction(false,
							"To pick up an item you must click on your inventory slot, and then on the item you want to pick up.",
							"",
							"The inventory slot is on the right, indicated by the case.")
					},
					Evaluator = ScenarioHelpers.ItemTypeIsInInventory<Scanner>(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
						ScenarioHelpers.SetCommandEnabled<DropItemCommand>(false),
					},
				}
			);

			// frame 10
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(false,
							"Now it is time to investigate.",
							"",
							"Make your way to the left system..")
					},
					Evaluator = ScenarioHelpers.OnlyPlayerIsAtLocation(nodeLeft),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.SetCommandEnabled<SetActorDestinationCommand>(false),
						ScenarioHelpers.HideTextAction,
					},
				}
			);

			// frame 11
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true,
							"You need to use the scanner tool to reveal the source of the problem."),
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
				}
			);

			// frame 12
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.SetCommandEnabled<DropItemCommand>(true),
						ScenarioHelpers.GenerateTextAction(false,
							"First you will have to install the item on the current system",
							"",
							"Select your inventory and then click on one of the item slots in the panel at the bottom of the screen.")
					},
					Evaluator = ScenarioHelpers.ItemTypeIsAtLocation<Scanner>(nodeLeft)
						.And(ScenarioHelpers.WaitForTutorialContinue),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.SetCommandEnabled<SetActorDestinationCommand>(true),
						ScenarioHelpers.HideTextAction,
					},
				}
			);

			// frame 13
			frames.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.CreateItemCommand(nameof(GameEntities.RedAntivirus), nodeRight.Id),
						ScenarioHelpers.GenerateTextAction(false,
							"You have now revealed the source of the infection.",
							"",
							"We've provided you with the necessary antivirus, you'll have to figure out the rest on your own...")
					},
					Evaluator = EvaluatorExtensions.Not(ScenarioHelpers.SystemIsInfected(nodeLeft)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
						ScenarioHelpers.EndGame(EndGameState.Success),
					},
				}
			);

			#endregion

			return new SimulationScenario()
			{
				Name = "Analysis",
				Description = "Analysis",
				MinPlayers = playerCount,
				MaxPlayers = playerCount,
				Configuration = configuration,

				CreatePlayerConfig = playerConfigFactory,

				// TODO: need a config driven specification for these
				Sequence = frames.ToArray(),
			};
		}
	}
}