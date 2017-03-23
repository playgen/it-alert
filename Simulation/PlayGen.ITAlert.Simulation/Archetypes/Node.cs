using System;
using System.Collections.Generic;
using System.Linq;
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
		public static readonly Archetype Archetype = new Archetype("Node")
			.HasComponents(new ComponentBinding[]
			{
				new ComponentBinding<Visitors>(),
				new ComponentBinding<GraphNode>(),
				new ComponentBinding<ExitRoutes>(),
				new ComponentBinding<MovementCost>(),
			});
	}
}
