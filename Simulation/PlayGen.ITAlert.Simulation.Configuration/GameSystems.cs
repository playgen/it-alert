using System.Collections.Generic;
using Engine.Commands;
using Engine.Configuration;
using Engine.Lifecycle;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Simulation.Systems.Enhancements;
using PlayGen.ITAlert.Simulation.Systems.Items;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using PlayGen.ITAlert.Simulation.Systems.Planning;
using PlayGen.ITAlert.Simulation.Systems.Resources;
using PlayGen.ITAlert.Simulation.Systems.Tutorial;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	/// <summary>
	/// The default system configuration for IT Alert
	/// </summary>
	public static class GameSystems
	{
		public static List<SystemConfiguration> Systems = new List<SystemConfiguration>()
		{
			// TODO: if the systems are tickable the order they are defined here is the order they will be ticked; we probably need to make this more explicit
			new SystemConfiguration<SubsystemResources>()
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
			new SystemConfiguration<IntentSystem>(),
			new SystemConfiguration<ItemStorageSystem>(),
			new SystemConfiguration<PlayerInventorySystem>(),
			new SystemConfiguration<ItemActivationSystem>()
			{
				ExtensionConfiguration = new SystemExtensionConfiguration[]
				{
					new SystemExtensionConfiguration<IItemActivationExtension>()
					{
						Implementations = new SystemExtensionImplementation[]
						{
							new SystemExtensionConfiguration<IItemActivationExtension>.SystemExtensionImplementation<TimedActivationExtension>(),
							new SystemExtensionConfiguration<IItemActivationExtension>.SystemExtensionImplementation<ScannerBehaviour>(),
							new SystemExtensionConfiguration<IItemActivationExtension>.SystemExtensionImplementation<AntivirusBehaviour>(),
							// TODO: need to find a good way to append extensions from the scenario definition
							new SystemExtensionConfiguration<IItemActivationExtension>.SystemExtensionImplementation<ContinueActivationExtension>(),
							new SystemExtensionConfiguration<IItemActivationExtension>.SystemExtensionImplementation<ResetOwnerOnDeactivate>(), 
						}
					}
				}
			},
			new SystemConfiguration<ItemManagement>(),
			// TODO: decide if the enhancement system should be responsible for manipulating the item storage of enhanced systems
			// or if the item storage system should have an extension
			new SystemConfiguration<EnhancementSystem>()
			{
				ExtensionConfiguration = new SystemExtensionConfiguration[]
				{
					// TODO: this kind of pattern might be getting a bit too verbose
					// perhaps there should not be system extensions, or perhaps a subsystem of extension should be first class systems
					// promoted systems could be processed according to interfaces implemented rather than being configured as an extension
					new SystemExtensionConfiguration<IEnhancementSystemExtension>()
					{
						Implementations = new SystemExtensionImplementation[]	
						{
							// this very long winded call is necessary to enforce the extension implementation of the extension interface
							// TODO: can the extension interface be inferred from the parent class?
							new SystemExtensionConfiguration<IEnhancementSystemExtension>.SystemExtensionImplementation<AntivirusWorkstationExtension>(),
						}
					}
				}
			},

			new SystemConfiguration<PathFindingSystem>(),
			// TODO: need to find a good way to append systems from the scenario definition
			new SystemConfiguration<TutorialSystem>(),
			new SystemConfiguration<EndGameSystem>(),
		};
	}
}
