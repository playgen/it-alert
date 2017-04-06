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
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Systems;
using PlayGen.ITAlert.Simulation.Modules.Malware.Systems;
using PlayGen.ITAlert.Simulation.Modules.Resources.Systems;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Systems;
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
			#region initialization

			new SystemConfiguration<GraphSystem>(),
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

			new SystemConfiguration<RNGSystem>(),

			#region resource system

			// TODO: if the systems are tickable the order they are defined here is the order they will be ticked; we probably need to make this more explicit
			new SystemConfiguration<ResourcesSystem>()
			{
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

			#region malware effects

			new SystemConfiguration<AdjacenetMalwareIncreaseMovementCostSystem>(),

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

							// movement
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<SetActorDestinationCommandHandler>(),
							// tutorial
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<DisplayTextCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<ContinueCommandCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<HideTextCommandHandler>(),
							// entity creation - no system mapping yet
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<CreateNpcCommandHandler>(),
							// items
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<CreateItemCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<ActivateItemCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<PickupItemCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<DropItemCommandHandler>(),
							// testing
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<HaltAndCatchFireCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<EndGameCommandHandler>(), 
						}

					}
				}
			},

			#endregion

			//new SystemConfiguration<IntentSystem>(),
			new SystemConfiguration<ItemStorageSystem>(),
			new SystemConfiguration<PlayerInventorySystem>(),

			#region item activation system

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
							// items
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<ScannerBehaviour>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<AntivirusBehaviour>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<CoopMultiColourAntivirusBehaviour>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<CaptureBehaviour>(),
							// enhancement actiovators
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<AnalyserBehaviour>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<GarbageDisposalActivatorBehaviour>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<TransferBehaviour>(), 

							// TODO: need to find a good way to append extensions from the scenario definition
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<ContinueActivationExtension>(),
							new SystemExtensionConfiguration<IActivationExtension>.SystemExtensionImplementation<ResetOwnerOnDeactivate>(), 
						}
					}
				}
			},

			#endregion

			// new SystemConfiguration<ItemManagementSystem>(),

			#region enhancement system

			// TODO: decide if the enhancement system should be responsible for manipulating the item storage of enhanced systems
			// or if the item storage system should have an extension
			new SystemConfiguration<AntivirusEnhancementSystem>(),
			new SystemConfiguration<GarbageDisposalEnhancementSystem>(),
			new SystemConfiguration<TransferEnhancementSystem>(),

			#endregion

			new SystemConfiguration<PathFindingSystem>(),
			// TODO: need to find a good way to append systems from the scenario definition
			new SystemConfiguration<TutorialSystem>(),
			new SystemConfiguration<EndGameSystem>(),

			new SystemConfiguration<MalwarePropogationSystem>(),
		};
	}
}
