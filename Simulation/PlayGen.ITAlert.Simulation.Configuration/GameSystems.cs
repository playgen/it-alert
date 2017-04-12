using System.Collections.Generic;
using Engine.Commands;
using Engine.Configuration;
using Engine.Lifecycle;
using Engine.Systems;
using Engine.Systems.Activation;
using Engine.Systems.RNG;
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

							// testing
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<HaltAndCatchFireCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<EndGameCommandHandler>(), 
						}

					}
				}
			},

			#endregion

			#region item storage

			//new SystemConfiguration<IntentSystem>(),
			new SystemConfiguration<ItemStorageSystem>(),
			new SystemConfiguration<PlayerInventorySystem>(),

			#endregion

			#region transfer system

			new SystemConfiguration<TransferEnhancementSystem>(),

			#endregion

			#region antivirus system

			new SystemConfiguration<AntivirusEnhancementSystem>(),

			#endregion

			#region garbage disposal

			new SystemConfiguration<GarbageDisposalEnhancementSystem>(),

			#endregion

			#region activation systems

			new SystemConfiguration<ActivationSystem>()
			{
				ExtensionConfiguration = new SystemExtensionConfiguration[]
				{
					new SystemExtensionConfiguration<IActivationExtension>()
					{
						Implementations = new SystemExtensionImplementation[]
						{
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<CPUConsumptionIncreasesTimedActivationDurationExtension>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<TimedActivationExtension>(),
							
							// malware
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<ScannerBehaviour>(),
							// antivirus
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<AnalyserBehaviour>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<AntivirusBehaviour>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<CaptureBehaviour>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<CoopMultiColourAntivirusBehaviour>(),
							// garbage disposal
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<GarbageDisposalActivatorBehaviour>(),
							// transfer
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<TransferBehaviour>(), 

							// tutorial
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<ContinueActivationExtension>(),

							// reset owner - this must come last or anything that depends on knowing who the owner is in OnDeactivating will break
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<ResetOwnerOnDeactivate>(),

							// do consumable behaviour last
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<ConsumableActivationExtension>(), 
						}
					}
				}
			},

			#endregion

			// TODO: need to find a good way to append systems from the scenario definition
			new SystemConfiguration<TutorialSystem>(),
		};
	}
}
