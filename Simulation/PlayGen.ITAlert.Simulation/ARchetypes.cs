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
	public static class Archetypes
	{
		private static readonly Archetype Node = new Archetype("Node")
			.HasComponents(new ComponentFactoryDelegate[]
			{
				entity => new Visitors(entity),
				entity => new GraphNode(entity), 
				entity => new ExitRoutes(entity), 
			});


		public static readonly Archetype Subsystem = new Archetype("Subsystem")
			.Extends(Node)
			.HasComponent(entity => new Name(entity))
			.HasComponent(entity => new Coordinate2DProperty(entity))
			.HasComponent(entity => new SubsystemMovement(entity)
			{
				Positions = SimulationConstants.SubsystemPositions,
			})			
			.HasComponent(entity => new ItemStorageProperty(entity));

		public static readonly Archetype Connection = new Archetype("Connection")
			.Extends(Node)
			.HasComponent(entity => new ConnectionMovement(entity)
			{
				Positions = SimulationConstants.ConnectionPositions,
			});

		private static readonly Archetype Actor = new Archetype("Actor")
			.HasComponents(new ComponentFactoryDelegate[]
			{
				entity => new CurrentLocationProperty(entity),
				entity => new IntentsProperty(entity),
				entity => new VisitorPosition(entity), 
			});

		public static readonly Archetype Player = new Archetype("Player")
			.Extends(Actor)
			.HasComponent(entity => new Name(entity));

		public static readonly Archetype Virus = new Archetype("Virus")
			.Extends(Actor);
	};

}
