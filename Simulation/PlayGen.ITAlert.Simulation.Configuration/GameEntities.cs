using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Components;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Components.Resources;
using PlayGen.ITAlert.Simulation.Components.Tutorial;
using PlayGen.ITAlert.Simulation.Systems.Malware;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	/// <summary>
	/// The default game entities for IT Alert
	/// </summary>
	public static class GameEntities
	{
		#region nodes

		private static readonly Archetype Node = new Archetype("Node")
			.HasComponents(new ComponentBinding[]
			{
				new ComponentBinding<Visitors>(), 
				new ComponentBinding<GraphNode>(), 
				new ComponentBinding<ExitRoutes>(),
				new ComponentBinding<MovementCost>(),
			});

		public static readonly Archetype Connection = new Archetype("Connection")
			.Extends(Node)
			.HasComponent(new ComponentBinding<Connection>());

		public static readonly Archetype Subsystem = new Archetype("Subsystem")
			.Extends(Node)
			.HasComponent(new ComponentBinding<Subsystem>())
			.HasComponent(new ComponentBinding<Name>())
			.HasComponent(new ComponentBinding<Coordinate2DProperty>())
			.HasComponent(new ComponentBinding<ItemActivator>())
			.HasComponent(new ComponentBinding<ItemStorage>()
			{
				ComponentTemplate = new ItemStorage()
				{
					Items = new ItemContainer[SimulationConstants.SubsystemMaxItems],
				}
			})
			.HasComponent(new ComponentBinding<MemoryResource>()
			{
				ComponentTemplate = new MemoryResource()
				{
					Value = SimulationConstants.SubsystemInitialMemory,
					Maximum = SimulationConstants.SubsystemMaxMemory,
				}
			})
			.HasComponent(new ComponentBinding<CPUResource>()
			{
				ComponentTemplate = new CPUResource()
				{
					Value = SimulationConstants.SubsystemInitialCPU,
					Maximum = SimulationConstants.SubsystemMaxCPU,
				}
			});

		public static readonly Archetype AnalysisEnhancement = new Archetype("AnalysisEnhancement")
			.Extends(Subsystem)
			.HasComponent(new ComponentBinding<AnalyserEnhancement>());

		public static readonly Archetype AntivirusEnhancement = new Archetype("AntivirusEnhancement")
			.Extends(Subsystem)
			.HasComponent(new ComponentBinding<AntivirusEnhancement>());

		public static readonly Archetype DatabaseEnhacement = new Archetype("DatabaseEnhacement")
			.Extends(Subsystem)
			.HasComponent(new ComponentBinding<DatabaseEnhancement>());

		#endregion

		#region actors

		private static readonly Archetype Actor = new Archetype("Actor")
			.HasComponents(new ComponentBinding[]
			{
				new ComponentBinding<CurrentLocation>(),
				new ComponentBinding<Intents>(),
				new ComponentBinding<VisitorPosition>(), 
				new ComponentBinding<MovementSpeed>()
				{
					ComponentTemplate = new MovementSpeed()
					{
						Value = 1,
					}
				}, 
			});

		public static readonly Archetype Player = new Archetype("Player")
			.Extends(Actor)
			.HasComponent(new ComponentBinding<Player>())
			.HasComponent(new ComponentBinding<ConsumeCPU>()
			{
				ComponentTemplate = new ConsumeCPU()
				{
					Value = SimulationConstants.ActorCPUConsumption,
				}
			})
			.HasComponent(new ComponentBinding<ItemStorage>()
			{
				ComponentTemplate = new ItemStorage()
				{
					ItemLimit = 1,
					MaxItems = 1,
					Items = new ItemContainer[]
					{
						new InventoryItemContainer(), 
					}
				}
			});

		#region viruses

		private static readonly Archetype Virus = new Archetype("Virus")
			.Extends(Actor)
			.HasComponent(new ComponentBinding<Npc>())
			.HasComponent(new ComponentBinding<ConsumeMemory>()
			{
				ComponentTemplate = new ConsumeMemory()
				{
					Value = SimulationConstants.VirusMemoryConsumedInitialValue,
				}
			})
			.HasComponent(new ComponentBinding<ConsumeCPU>()
			{
				ComponentTemplate = new ConsumeCPU()
				{
					Value = SimulationConstants.VirusCPUConsumedInitialValue,
				}
			});

		public static readonly Archetype CPUVirus = new Archetype("CPUVirus")
			.Extends(Virus)
			// TODO: need a better way of overriding existing component binding or component template
			// TODO: need a way of passing configuration to the archetype factory dyanmically
			//		eg. initialize a virus with a specific genome from one archetype rahter than having one archetype per virus class
			.HasComponent(new ComponentBinding<MalwareGenome>()
				{
					ComponentTemplate = new MalwareGenome()
					{
						Values = new List<string>()
						{
							ConsumeCPUMalwareEffect.Key,
						}
					}
				})
			;

		#endregion

		#endregion

		#region items

		private static readonly Archetype Item = new Archetype("Item")
			.HasComponents(new ComponentBinding[]
			{
				new ComponentBinding<CurrentLocation>(),
				new ComponentBinding<Owner>(),
				new ComponentBinding<ConsumeMemory>()
				{
					ComponentTemplate = new ConsumeMemory()
					{
						Value = SimulationConstants.ItemMemoryConsumption,
					}
				},
				new ComponentBinding<Item>(), 
				new ComponentBinding<Activation>(), 
			});

		public static readonly Archetype Scanner = new Archetype("Scanner")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Scanner>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		public static readonly Archetype Repair = new Archetype("Repair")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Repair>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		public static readonly Archetype Cleaner = new Archetype("Cleaner")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Cleaner>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		public static readonly Archetype Analyser = new Archetype("Analyser")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Analyser>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		public static readonly Archetype Tracer = new Archetype("Tracer")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Tracer>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		public static readonly Archetype Antivirus = new Archetype("Antivirus")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Antivirus>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		public static readonly Archetype Capture = new Archetype("Capture")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Capture>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		public static readonly Archetype Data = new Archetype("Data")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Data>());

		#endregion

		#region tutorial system

		public static Archetype TutorialText = new Archetype(SimulationConstants.TutorialTextArchetype)
			.HasComponent(new ComponentBinding<ScenarioText>())
			.HasComponent(new ComponentBinding<Text>());

		#endregion
	}
}
