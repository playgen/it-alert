﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Components.Resources;
using PlayGen.ITAlert.Simulation.Systems.Movement;

namespace PlayGen.ITAlert.Simulation
{
	/// <summary>
	/// TEMPORARY archetype defintion format, the lambdas with no intiial parameters wont do in the long term!
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
			.HasComponent(new ComponentBinding<EntityTypeProperty>()) // EntityType.Connection
			.HasComponent(new ComponentBinding<MovementCost>()); // 4 // TODO: read from simulation constants

		public static readonly Archetype Subsystem = new Archetype("Subsystem")
			.Extends(Node)
			.HasComponent(new ComponentBinding<EntityTypeProperty>()) // EntityType.Subsystem
			.HasComponent(new ComponentBinding<Name>())
			.HasComponent(new ComponentBinding<Coordinate2DProperty>())
			.HasComponent(new ComponentBinding<ItemActivator>())
			.HasComponent(new ComponentBinding<ItemStorage>()) // SimulationConstants.SubsystemMaxItems
			.HasComponent(new ComponentBinding<MemoryResource>()) // 0, 4
			.HasComponent(new ComponentBinding<CPUResource>()); // 0, 4

		public static readonly Archetype Analysis = new Archetype("Analysis")
			.Extends(Subsystem)
			.HasComponent(new ComponentBinding<AnalyserEnhancement>());

		public static readonly Archetype Antivirus = new Archetype("Antivirus")
			.Extends(Subsystem)
			.HasComponent(new ComponentBinding<AntivirusEnhancement>());

		public static readonly Archetype Database = new Archetype("Database")
			.Extends(Subsystem)
			.HasComponent(new ComponentBinding<DatabaseEnhancement>());

		#endregion

		#region actors

		private static readonly Archetype Actor = new Archetype("Actor")
			.HasComponents(new ComponentBinding[]
			{
				new ComponentBinding<CurrentLocation>(),
				new ComponentBinding<IntentsProperty>(),
				new ComponentBinding<VisitorPosition>(), 
				new ComponentBinding<MovementSpeed>(), 
			});

		public static readonly Archetype Player = new Archetype("Player")
			.Extends(Actor)
			.HasComponent(new ComponentBinding<EntityTypeProperty>()) //EntityType.Player
			.HasComponent(new ComponentBinding<Name>());	// TODO: name should problably be somewhere else - player system?

		#region viruses

		public static readonly Archetype Virus = new Archetype("Virus")
			.Extends(Actor)
			.HasComponent(new ComponentBinding<EntityTypeProperty>()) // EntityType.Npc
			.HasComponent(new ComponentBinding<MalwareGenome>())
			.HasComponent(new ComponentBinding<ConsumeMemory>()) // 0
			.HasComponent(new ComponentBinding<ConsumeCPU>()); // 0


		#endregion

		#endregion

		#region items

		private static readonly Archetype Item = new Archetype("Item")
			.HasComponents(new ComponentBinding[]
			{
				new ComponentBinding<EntityTypeProperty>(), //EntityType.Item,
				new ComponentBinding<CurrentLocation>(),
				new ComponentBinding<Owner>(),
				new ComponentBinding<ConsumeMemory>(),
			});

		public static readonly Archetype Scanner = new Archetype("Scanner")
			.Extends(Item);

		public static readonly Archetype Repair = new Archetype("Repair")
			.Extends(Item);

		public static readonly Archetype Cleaner = new Archetype("Cleaner")
			.Extends(Item);

		public static readonly Archetype Analyser = new Archetype("Analyser")
			.Extends(Item);

		public static readonly Archetype Tracer = new Archetype("Tracer")
			.Extends(Item);

		public static readonly Archetype Capture = new Archetype("Capture")
			.Extends(Item);

		public static readonly Archetype Data = new Archetype("Data")
			.Extends(Item);

		#endregion
	}
}
