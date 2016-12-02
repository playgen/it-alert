using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Engine.Components.Common;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation
{
	/// <summary>
	/// TEMPORARY archetype defintion format, the lambdas with no intiial parameters wont do in the long term!
	/// </summary>
	public static class GameEntities
	{
		#region nodes

		private static readonly Archetype Node = new Archetype("Node")
			.HasComponents(new ComponentFactoryDelegate[]
			{
				() => new Visitors(),
				() => new GraphNode(),
				() => new ExitRoutes(), 
			});

		public static readonly Archetype Subsystem = new Archetype("Subsystem")
			.Extends(Node)
			.HasComponent(() => new Name())
			.HasComponent(() => new Coordinate2DProperty())
			.HasComponent(() => new SubsystemMovement()
			{
				Positions = SimulationConstants.SubsystemPositions,
			})			
			.HasComponent(() => new ItemStorage());

		public static readonly Archetype Connection = new Archetype("Connection")
			.Extends(Node)
			.HasComponent(() => new ConnectionMovement()
			{
				Positions = SimulationConstants.ConnectionPositions,
			})
			.HasComponent(() => new MovementCost(4));

		#endregion

		#region actors

		private static readonly Archetype Actor = new Archetype("Actor")
			.HasComponents(new ComponentFactoryDelegate[]
			{
				() => new CurrentLocationProperty(),
				() => new IntentsProperty(),
				() => new VisitorPosition(), 
			});

		public static readonly Archetype Player = new Archetype("Player")
			.Extends(Actor)
			.HasComponent(() => new Name());

		public static readonly Archetype Virus = new Archetype("Virus")
			.Extends(Actor);

		#endregion

		#region items

		private static readonly Archetype Item = new Archetype("Item")
			.HasComponents(new ComponentFactoryDelegate[]
			{
				() => new OwnerProperty(),
				() => new ConsumeMemory(),
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

		#endregion

		#region subsystem enhancements

		private static readonly Archetype Enhancement = new Archetype("Enhancement")
			.HasComponents(new ComponentFactoryDelegate[]
			{

			});

		public static readonly Archetype Analysis = new Archetype("Analysis")
			.Extends(Enhancement);

		#endregion
	}
}
