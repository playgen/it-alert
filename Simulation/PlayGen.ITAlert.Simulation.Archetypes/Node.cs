using System;
using System.Collections.Generic;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	/// <summary>
	/// Archetype base for graph node entities
	/// </summary>
	public static class Node
	{
		public static readonly Archetype Archetype = new Archetype(nameof(Node))
			.HasComponent<Visitors>()
			.HasComponent<GraphNode>()
			.HasComponent<ExitRoutes>()
			.HasComponent<MovementCost>();
	}
}
