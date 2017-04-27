using System;
using System.Collections.Generic;
using System.Text;
using Engine.Archetypes;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	/// <summary>
	/// Archetype definition for connection entities
	/// </summary>
	public static class ConnectionNode
	{
		public static readonly Archetype Archetype = new Archetype(nameof(ConnectionNode))
			.Extends(Node.Archetype)
			.HasComponent<Components.EntityTypes.Connection>();
	}
}
