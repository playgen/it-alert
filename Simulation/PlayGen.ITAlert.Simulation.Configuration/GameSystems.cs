﻿using System.Collections.Generic;
using Engine.Commands;
using Engine.Configuration;
using Engine.Events;
using Engine.Lifecycle;
using Engine.Systems;
using Engine.Systems.Activation;
using Engine.Systems.RNG;
using Engine.Systems.Timing;
using Engine.Systems.Timing.Commands;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems.Activation;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Components;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Systems;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Systems.Activation;
using PlayGen.ITAlert.Simulation.Modules.Malware.Systems;
using PlayGen.ITAlert.Simulation.Modules.Malware.Systems.Activation;
using PlayGen.ITAlert.Simulation.Modules.Malware.Systems.Movement;
using PlayGen.ITAlert.Simulation.Modules.Resources.Systems;
using PlayGen.ITAlert.Simulation.Modules.Resources.Systems.Activation;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Systems;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Systems.Activation;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Systems;
using PlayGen.ITAlert.Simulation.Systems.Initialization;
using PlayGen.ITAlert.Simulation.Systems.Items;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using PlayGen.ITAlert.Simulation.Systems.Players;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	/// <summary>
	/// The default system configuration for IT Alert
	/// </summary>
	public static class GameSystems
	{
		public static List<SystemConfiguration> Systems = new List<SystemConfiguration>()
		{
			#region common

			// graph systems
			new SystemConfiguration<GraphSystem>(),
			new SystemConfiguration<PathFindingSystem>(),
			// game end
			new SystemConfiguration<EndGameSystem>(),
			// rng system
			new SystemConfiguration<RNGSystem>(),
			// events
			new SystemConfiguration<EventSystem>(),

			#endregion

			#region players

			// player system
			new SystemConfiguration<PlayerSystem>()
			{
				ExtensionConfiguration = new SystemExtensionConfiguration[]
				{
					new SystemExtensionConfiguration<IPlayerSystemBehaviour>()
					{
						Implementations = new SystemExtensionImplementation[]
						{
							new SystemExtensionConfiguration<IPlayerSystemBehaviour>.SystemExtensionImplementation<DropInventoryOnDisconnect>(),
						}
					}
				}
			},

			#endregion

			#region resource system

			// TODO: if the systems are tickable the order they are defined here is the order they will be ticked; we probably need to make this more explicit
			new SystemConfiguration<ResourcesSystem>()
			{
				// TODO: could the fact the archetypes are static cause problems?
				BindingInitialize = ResourcesSystem.InitializeArchetypes,
				ExtensionConfiguration = new SystemExtensionConfiguration[]
				{
					new SystemExtensionConfiguration<ISubsystemResourceEffect>()
					{
						Implementations = new SystemExtensionImplementation[]
						{
							new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<ResetCPUEachTick>(),
							new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<ResetMemoryEachTick>(),
							new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<ItemStorageConsumesMemoryEffect>(),
							new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<VisitorsConsumeCPUEffect>(),
							new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<CPUReducesMovementSpeed>(), 
						}
					}
				}
			},

			#endregion

			#region malware systems

			new SystemConfiguration<AdjacenetMalwareIncreaseMovementCostSystem>(),
			new SystemConfiguration<MalwarePropogationSystem>(),

			#endregion

			#region movement system

			new SystemConfiguration<MovementSystem>()
			{
				ExtensionConfiguration = new SystemExtensionConfiguration[]
				{
					new SystemExtensionConfiguration<IMovementSystemExtension>()
					{
						AllOfType = true,
					}
				}
			},

			#endregion

			#region timer

			new SystemConfiguration<TimerSystem>(),

			#endregion

			#region command system

			// TODO: this has now become mandatory so I need a better way of specifying the configuration such that I can find the CommandSystem config entry and append to it
			// this pattern would also come in handy for adding scenario specific system bindings
			new SystemConfiguration<CommandSystem>()
			{
				ExtensionConfiguration = new SystemExtensionConfiguration[]
				{
					new SystemExtensionConfiguration<ICommandHandler>()
					{
						Implementations = new SystemExtensionImplementation[]
						{
							// TODO: some of these handlers should be encapsulated into the relevant system
							// probably create a zenject installer

							// tutorial
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<DisplayTextCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<ContinueCommandCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<HideTextCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<SetHighlightCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<ClearHighlightCommandHandler>(), 
							
							// entity creation
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<CreateMalwareCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<CreateItemCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<CreatePlayerCommandHandler>(),

							// item management
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<ActivateItemCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<ActivateItemTypeCommandHandler>(),

							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<DropItemCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<DropItemTypeCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<PickupItemCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<PickupItemTypeCommandHandler>(),
						    new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<MoveItemCommandHandler>(),
							
							// movement
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<SetActorDestinationCommandHandler>(),

							// timer
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<SetTimerCommandHandler>(), 

							// testing
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<HaltAndCatchFireCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<EndGameCommandHandler>(), 
						}

					}
				}
			},

			#endregion

			#region item systems

			//new SystemConfiguration<IntentSystem>(),
			new SystemConfiguration<ItemStorageSystem>(),
			new SystemConfiguration<PlayerInventorySystem>(),

			#region activation systems
			// cpu timed activatiojn before timed
			new SystemConfiguration<CPUConsumptionIncreasesTimedActivationDurationSystem>(),
			// timed activation needs to come before activation
			new SystemConfiguration<TimedActivationsystem>(),
			new SystemConfiguration<ActivationSystem>(),


			#endregion

			#region transfer system

			new SystemConfiguration<TransferEnhancementSystem>(),

			new SystemConfiguration<TransferSystem>(),

			#endregion

			#region antivirus systems

			new SystemConfiguration<AntivirusEnhancementSystem>(),

			new SystemConfiguration<CoopMultiColourAntivirusSystem>(),
			new SystemConfiguration<AntivirusSystem>(),
			new SystemConfiguration<AnalyserSystem>(),
			new SystemConfiguration<CaptureSystem>(),

			#endregion

			#region malware systems

			new SystemConfiguration<ScannerSystem>(),

			#endregion

			#region garbage disposal

			new SystemConfiguration<GarbageDisposalEnhancementSystem>(),

			new SystemConfiguration<GarbageDisposalSystem>(),

			#endregion

			#region tutorial systems

			new SystemConfiguration<ContinueSystem>(),

			#endregion

			#region activation systems

			new SystemConfiguration<ResetOwnerOnDeactivationSystem>(),

			// consumables needs to be last
			new SystemConfiguration<ConsumableActivationSystem>(),

			#endregion

			#endregion

			// TODO: need to find a good way to append systems from the scenario definition
			new SystemConfiguration<TutorialSystem>(),
		};
	}
}
