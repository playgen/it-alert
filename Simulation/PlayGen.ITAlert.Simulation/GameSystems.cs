using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Configuration;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Simulation.Systems.Enhancements;
using PlayGen.ITAlert.Simulation.Systems.Items;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using PlayGen.ITAlert.Simulation.Systems.Planning;
using PlayGen.ITAlert.Simulation.Systems.Resources;

namespace PlayGen.ITAlert.Simulation
{
	/// <summary>
	/// The default system configuration for IT Alert
	/// </summary>
	public static class GameSystems
	{
		public static List<SystemConfiguration> Systems = new List<SystemConfiguration>()
		{
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
			new SystemConfiguration<CommandSystem>()
			{
				ExtensionConfiguration = new SystemExtensionConfiguration[]
				{
					new SystemExtensionConfiguration<ICommandHandler>()
					{
						Implementations = new SystemExtensionImplementation[]
						{
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<SetActorDestinationCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<DisplayTextCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<HideTextCommandHandler>(),
							new SystemExtensionConfiguration<ICommandHandler>.SystemExtensionImplementation<SpawnNpcCommandHandler>(),
						}

					}
				}
			},
			new SystemConfiguration<IntentSystem>(),
			new SystemConfiguration<ItemStorageSystem>(),
			new SystemConfiguration<ItemActivationSystem>(),
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
							new SystemExtensionConfiguration<IEnhancementSystemExtension>.SystemExtensionImplementation<AnalyserEnhancementExtension>(),
						}
					}
				}
			},
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
							new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<ItemsConsumeMemoryEffect>(),
							new SystemExtensionConfiguration<ISubsystemResourceEffect>.SystemExtensionImplementation<VisitorsConsumeCPUEffect>(),
						}
					}
				}
			},
			new SystemConfiguration<PathFindingSystem>()
		};
	}
}
