using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Components.Tutorial;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios
{
	internal static class Introduction
	{
		private static SimulationScenario _scenario;
		public static SimulationScenario Scenario => _scenario ?? (_scenario = GenerateScenario());

		public static readonly Archetype TutorialScanner = new Archetype("TutorialScanner")
			.Extends(GameEntities.Scanner)
			.HasComponent(new ComponentBinding<ActivationContinue>()
			{
				ComponentTemplate = new ActivationContinue()
				{
					ContinueOn = ActivationContinue.ActivationPhase.Activating,
				}
			});
		
		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int playerCount = 1;

			var nodeLeft = new NodeConfig(0)
			{
				Name = "Left",
				X = 0,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeRight = new NodeConfig(1)
			{
				Name = "Right",
				X = 1,
				Y = 0,
				ArchetypeName = nameof(GameEntities.Subsystem),
			};

			var nodeConfigs = new NodeConfig[] { nodeLeft, nodeRight };
			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, 1);
			var itemConfigs = new ItemConfig[0];
			var playerConfigFactory = new Func<int, PlayerConfig>(i => new PlayerConfig()
			{
				StartingLocation = nodeRight.Id,
				ArchetypeName = nameof(GameEntities.Player)
			});
			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, itemConfigs);
			configuration.Archetypes.Add(TutorialScanner);


			#endregion

			#region frames

			// ReSharper disable once UseObjectOrCollectionInitializer
			var frames = new List<SequenceFrame<Simulation, SimulationConfiguration>>();

			#region 1

			frames.Add(// frame 1 - welcome
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.SetCommandEnabled<SetActorDestinationCommand>(false),
						ScenarioHelpers.GenerateTextAction(true,
							"Welcome to IT Alert!", 
							"You are a system administrator tasked with maintaing the network.",
							"Let's get started..")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					// TODO: need a more polymorphic way of specifying evaluators
					// c# 7 pattern match will be nice
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			#endregion

			#region 2 - movement

			frames.Add(// frame 2a - movement
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.SetCommandEnabled<SetActorDestinationCommand>(true),
						ScenarioHelpers.GenerateTextAction(false, 
							"Try navigating to another system by clicking on it..")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.OnlyPlayerIsAtLocation(nodeLeft),
				}
			);

			frames.Add(// frame 2b - patronise
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(false, 
							"Well done. You can follow simple instructions!")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForSeconds(3),
				}
			);

			#endregion

			#region 3 - resources

			frames.Add(// frame 3a - resources
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true,
							"Systems have resources, the red bar represents available CPU and the blue bar available memory.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 3b - resources
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true, 
							"Items consume system memory, and players consume CPU. When either run out the system won't be able to contain any more of that type of object.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);


			frames.Add(// frame 3c - resources
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true, 
							"As you move between nodes you will notice the CPU consumption vary.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			#endregion

			#region 4 - items

			frames.Add(// frame 4a - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.CreateItemCommand(nameof(TutorialScanner), nodeRight.Id),
						ScenarioHelpers.SetCommandEnabled<ActivateItemCommand>(false),
						ScenarioHelpers.GenerateTextAction(true, 
							"The item that has just been spawned on the right hand node is a scanner.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					// TODO: wait for tutorial currently must be the last evaluator in an AND check because the waithandle is reset after a successful read
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 4a - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(false, 
							"To use items you must be on the same node...")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					// TODO: wait for tutorial currently must be the last evaluator in an AND check because the waithandle is reset after a successful read
					Evaluator = ScenarioHelpers.OnlyPlayerIsAtLocation(nodeRight),
				}
			);



			frames.Add(// frame 4b - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true, 
							"When you are on the same node as available items they will appear in the tray at the bottom of your screen.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 4c - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.SetCommandEnabled<ActivateItemCommand>(true),
						ScenarioHelpers.GenerateTextAction(false, 
							"Clicking the item on your tray will activate the item.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 4e - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true,
							"While the item is active the colour changes to indicate the player that is performing the action."),
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 4e - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true,
							"The scanner reveals malware on a system when it is present and hidden.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			#endregion

			#region 5 - malware

			frames.Add(// frame 5a - malware
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.CreateNpcCommand(nameof(GameEntities.CPUVirus), nodeLeft.Id),
						ScenarioHelpers.GenerateTextAction(true,
							"Malware has infected the Left system! You should investigate")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.OnlyPlayerIsAtLocation(nodeLeft).And(ScenarioHelpers.WaitForTutorialContinue),
				}
			);

			frames.Add(// frame 5a - malware
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.GenerateTextAction(true,
							"It appears that this infections is consuming CPU cycles on the system. As CPU is consumed the performance of the system decreases and so does movement speed.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						ScenarioHelpers.HideTextAction,
					},
					Evaluator = ScenarioHelpers.WaitForTutorialContinue,
				}
			);

			#endregion

			#endregion

			return new SimulationScenario()
			{
				Name = "Introduction",
				Description = "Introduction",
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
