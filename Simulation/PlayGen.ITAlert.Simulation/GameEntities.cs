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
			.HasComponent(() => new ItemStorageProperty());

		public static readonly Archetype Connection = new Archetype("Connection")
			.Extends(Node)
			.HasComponent(() => new ConnectionMovement()
			{
				Positions = SimulationConstants.ConnectionPositions,
			})
			.HasComponent(() => new MovementCost(4));

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
	};

}
