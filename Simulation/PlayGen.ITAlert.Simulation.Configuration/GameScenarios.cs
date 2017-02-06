using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Archetypes;
using Engine.Commands;
using Engine.Entities;
using Engine.Evaluators;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Tutorial;
using PlayGen.ITAlert.Simulation.Evaluators;
using PlayGen.ITAlert.Simulation.Extensions;
using PlayGen.ITAlert.Simulation.Sequencing;
using PlayGen.ITAlert.Simulation.Systems.Tutorial;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	// ReSharper disable once InconsistentNaming
	public static class ECSSequenceExtensions
	{
		public static void HandleCommand(this IECS ecs, ICommand command)
		{
			if (ecs.TryHandleCommand(command) == false)
			{
				throw new SequenceException($"Unable to handle sequenced command '{command.GetType().Name}");
			}
		}
	}

	public static class GameScenarios
	{
		public static readonly Archetype TutorialScanner = new Archetype("TutorialScanner")
			.Extends(GameEntities.Scanner)
			.HasComponent(new ComponentBinding<ActivationContinue>()
			{
				ComponentTemplate = new ActivationContinue()
				{
					ContinueOn = ActivationContinue.ActivationPhase.Activating,
				}
			});


		// TODO: extract helpers to factory or the command class that is applicable
		#region Action helpers

		#region text

		private static SimulationAction HideTextAction => new SimulationAction((ecs, config) =>
		{
			ecs.HandleCommand(new HideTextCommand());
		}, "Hide Text");

		private static SimulationAction GenerateTextAction(string text, bool @continue = true)
		{
			return new SimulationAction((ecs, config) =>
			{
				var textCommand = new DisplayTextCommand()
				{
					Text = text,
					Continue = @continue,
				};
				ecs.HandleCommand(textCommand);
			}, "Welcome Message");
		}

		#endregion

		#region commands

		private static SimulationAction SetCommandEnabled<TCommand>(bool enabled)
			where TCommand : ICommand
		{
			return new SimulationAction((ecs, config) =>
			{
				ICommandSystem commandSystem;
				ICommandHandler commandHandler;
				if (ecs.TryGetSystem(out commandSystem)
					&& commandSystem.TryGetHandler<TCommand>(out commandHandler))
				{
					commandHandler.SetEnabled(enabled);
				}
			}, $"Set command enabled ({nameof(TCommand)}): {enabled}");
		}

		#endregion

		#region evaluator helpers

		private static SimulationEvaluator WaitForTutorialContinue => new SimulationEvaluator((ecs, config) =>
		{
			ITutorialSystem tutorialSystem;
			if (ecs.TryGetSystem(out tutorialSystem))
			{
				return tutorialSystem.Continue;
			}
			return false;
		});

		private static TimeEvaluator<Simulation, SimulationConfiguration> WaitForSeconds(int seconds)
		{
			return new TimeEvaluator<Simulation, SimulationConfiguration>()
			{
				Threshold = 1000 * seconds,
			};
		}

		private static TickEvaluator<Simulation, SimulationConfiguration> WaitForTicks(int ticks)
		{
			return new TickEvaluator<Simulation, SimulationConfiguration>()
			{
				Threshold = ticks,
			};
		}

		private static SimulationEvaluator OnlyPlayerIsAtLocation(NodeConfig node)
		{
			return new SimulationEvaluator((ecs, config) =>
			{
				var playerId = config.PlayerConfiguration.Single().EntityId;
				Entity playerEntity;
				CurrentLocation location;
				return ecs.Entities.TryGetValue(playerId, out playerEntity)
						&& playerEntity.TryGetComponent(out location)
						&& location.Value == node.EntityId;
			});
		}

		#endregion

		#region entity creation

		private static SimulationAction CreateItemCommand(string itemType, int systemLogicalId)
		{
			return new SimulationAction((ecs, config) =>
			{
				var createItemCommand = new CreateItemCommand()
				{ 
					Archetype = itemType,
					IdentifierType = IdentifierType.Logical,
					SystemId = systemLogicalId
				};
				ecs.HandleCommand(createItemCommand);
			}, "Create item");
		}

		private static SimulationAction CreateNpcCommand(string archetype, int systemLogicalId)
		{
			return new SimulationAction((ecs, config) =>
			{
				var createNpcCommand = new CreateNpcCommand()
				{
					Archetype = archetype,
					IdentifierType = IdentifierType.Logical,
					SystemId = systemLogicalId
				};
				ecs.HandleCommand(createNpcCommand);
			}, "Create NPC");
		}

		#endregion

		#endregion

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateIntroductionScenario()
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
			var frames = new List<SimulationFrame>();

			#region 1

			frames.Add(// frame 1 - welcome
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						SetCommandEnabled<SetActorDestinationCommand>(false),
						GenerateTextAction($"Welcome to IT Alert!{Environment.NewLine}You are a system administrator tasked with maintaing the network.{Environment.NewLine}Let's get started..")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					// TODO: need a more polymorphic way of specifying evaluators
					// c# 7 pattern match will be nice
					Evaluator = WaitForTutorialContinue,
				}
			);

			#endregion

			#region 2 - movement

			frames.Add(// frame 2a - movement
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						SetCommandEnabled<SetActorDestinationCommand>(true),
						GenerateTextAction("Try navigating to another system by clicking on it..", false)
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = OnlyPlayerIsAtLocation(nodeLeft),
				}
			);

			frames.Add(// frame 2b - patronise
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						GenerateTextAction("Well done. You can follow simple instructions!", false)
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForSeconds(3),
				}
			);

			#endregion

			#region 3 - resources

			frames.Add(// frame 3a - resources
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						GenerateTextAction("Systems have resources, the red bar represents available CPU and the blue bar available memory.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 3b - resources
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						GenerateTextAction("Items consume system memory, and players consume CPU. When either run out the system won't be able to contain any more of that type of object.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForTutorialContinue,
				}
			);


			frames.Add(// frame 3c - resources
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						GenerateTextAction("As you move between nodes you will notice the CPU consumption vary.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForTutorialContinue,
				}
			);

			#endregion

			#region 4 - items

			frames.Add(// frame 4a - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						CreateItemCommand(nameof(TutorialScanner), nodeRight.Id),
						SetCommandEnabled<ActivateItemCommand>(false),
						GenerateTextAction($"The item that has just been spawned on the right hand node is a scanner.{Environment.NewLine}To use items you must be on the same node.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					// TODO: wait for tutorial currently must be the last evaluator in an AND check because the waithandle is reset after a successful read
					Evaluator = OnlyPlayerIsAtLocation(nodeRight).And(WaitForTutorialContinue),
				}
			);

			frames.Add(// frame 4b - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						GenerateTextAction("When you are on the same node as available items they will appear in the tray at the bottom of your screen.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 4c - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						SetCommandEnabled<ActivateItemCommand>(true),
						GenerateTextAction("Clicking the item on your tray will activate the item.", false)
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 4e - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						GenerateTextAction($"While the item is active the colouyr changes to indicate the player that is performing the action."),
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForTutorialContinue,
				}
			);

			frames.Add(// frame 4e - item
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						GenerateTextAction($"Unfortunately the scanner had no affect this time.{Environment.NewLine}The scanner reveals malware on a system when it is present and hidden.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForTutorialContinue,
				}
			);

			#endregion

			#region 5 - malware

			frames.Add(// frame 5a - malware
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						CreateNpcCommand(nameof(GameEntities.CPUVirus), nodeLeft.Id),
						GenerateTextAction($"Malware has infected the Left system! You should investigate")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = OnlyPlayerIsAtLocation(nodeLeft).And(WaitForTutorialContinue),
				}
			);

			frames.Add(// frame 5a - malware
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						CreateNpcCommand(nameof(GameEntities.CPUVirus), nodeLeft.Id),
						GenerateTextAction($"It appears that this infections is consuming CPU cycles on the system. As CPU is consumed the performance of the system decreases and so does movement speed.")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					Evaluator = WaitForTutorialContinue,
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

		private static SimulationScenario GenerateMultiplayerIntroductionScenario()
		{
			#region configuration

			const int minPlayerCount = 4;
			const int maxPlayerCount = 4;

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
			var frames = new List<SimulationFrame>();

			#region 1

			frames.Add(// frame 1 - welcome
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						SetCommandEnabled<SetActorDestinationCommand>(false),
						GenerateTextAction($"Hellooo humans!{Environment.NewLine} Seeing as you're biological brains aren't anywhere close to the computational speed of my CPU, I" +
											$"figured: the more of you, the better.{Environment.NewLine}Let's get started...also this is a test level so you're now stuck here until you escape! Mwuhahaha!")
					},
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						HideTextAction,
					},
					// TODO: need a more polymorphic way of specifying evaluators
					// c# 7 pattern match will be nice
					Evaluator = WaitForTutorialContinue,
				}
			);

			#endregion
			

			#endregion

			return new SimulationScenario()
			{
				Name = "MultiplayerIntroduction",
				Description = "MultiplayerIntroduction",
				MinPlayers = minPlayerCount,
				MaxPlayers = maxPlayerCount,
				Configuration = configuration,

				CreatePlayerConfig = playerConfigFactory,

				// TODO: need a config driven specification for these
				Sequence = frames.ToArray(),
			};
		}

		private static SimulationScenario _introduction = null;
		public static SimulationScenario Introduction => _introduction ?? (_introduction = GenerateIntroductionScenario());

		private static SimulationScenario _multiplayerIntroduction = null;
		public static SimulationScenario MultiplayerIntroduction => _multiplayerIntroduction ?? (_multiplayerIntroduction = GenerateMultiplayerIntroductionScenario());
	}
}
