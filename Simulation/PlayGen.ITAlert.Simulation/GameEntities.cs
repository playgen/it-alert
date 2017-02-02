using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.Flags;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Items.Flags;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Components.Resources;

namespace PlayGen.ITAlert.Simulation
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
			});

		public static readonly Archetype Connection = new Archetype("Connection")
			.Extends(Node)
			.HasComponent(new ComponentBinding<Connection>())
			.HasComponent(new ComponentBinding<EntityTypeProperty>()
			{
				ComponentTemplate = new EntityTypeProperty()
				{
					Value = EntityType.Connection
				}
			})
			.HasComponent(new ComponentBinding<MovementCost>());

		public static readonly Archetype Subsystem = new Archetype("Subsystem")
			.Extends(Node)
			.HasComponent(new ComponentBinding<Subsystem>())
			.HasComponent(new ComponentBinding<EntityTypeProperty>()
			{
				ComponentTemplate = new EntityTypeProperty()
				{
					Value = EntityType.Subsystem
				}
			})
			.HasComponent(new ComponentBinding<Name>())
			.HasComponent(new ComponentBinding<Coordinate2DProperty>())
			.HasComponent(new ComponentBinding<ItemActivator>())
			.HasComponent(new ComponentBinding<ItemStorage>()
			{
				ComponentTemplate = new ItemStorage()
			})
			.HasComponent(new ComponentBinding<MemoryResource>()
			{
				ComponentTemplate = new MemoryResource()
				{
					Value = 0,
					Maximum = SimulationConstants.SubsystemMaxMemory,
				}
			})
			.HasComponent(new ComponentBinding<CPUResource>()
			{
				ComponentTemplate = new CPUResource()
				{
					Value = 4,
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
			.HasComponent(new ComponentBinding<EntityTypeProperty>()
			{
				ComponentTemplate = new EntityTypeProperty()
				{
					Value = EntityType.Player
				}
			})
			.HasComponent(new ComponentBinding<Player>());

		#region viruses

		public static readonly Archetype Virus = new Archetype("Virus")
			.Extends(Actor)
			.HasComponent(new ComponentBinding<EntityTypeProperty>()
			{
				ComponentTemplate = new EntityTypeProperty()
				{
					Value = EntityType.Npc
				}
			})
			.HasComponent(new ComponentBinding<MalwareGenome>())
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


		#endregion

		#endregion

		#region items

		private static readonly Archetype Item = new Archetype("Item")
			.HasComponents(new ComponentBinding[]
			{
				new ComponentBinding<EntityTypeProperty>()
				{
					ComponentTemplate = new EntityTypeProperty()
					{
						Value = EntityType.Item
					}
				},
				new ComponentBinding<CurrentLocation>(),
				new ComponentBinding<Owner>(),
				new ComponentBinding<ConsumeMemory>(),
				new ComponentBinding<Activation>(), 
			});

		public static readonly Archetype Scanner = new Archetype("Scanner")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Scanner>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = 20,
				}
			});

		public static readonly Archetype Repair = new Archetype("Repair")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Repair>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = 20,
				}
			});

		public static readonly Archetype Cleaner = new Archetype("Cleaner")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Cleaner>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = 20,
				}
			});

		public static readonly Archetype Analyser = new Archetype("Analyser")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Analyser>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = 20,
				}
			});

		public static readonly Archetype Tracer = new Archetype("Tracer")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Tracer>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = 20,
				}
			});

		public static readonly Archetype Antivirus = new Archetype("Antivirus")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Antivirus>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = 20,
				}
			});

		public static readonly Archetype Capture = new Archetype("Capture")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Capture>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = 20,
				}
			});

		public static readonly Archetype Data = new Archetype("Data")
			.Extends(Item)
			.HasComponent(new ComponentBinding<Data>());

		#endregion
	}
}
