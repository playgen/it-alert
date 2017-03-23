using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Components.Resources;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	/// <summary>
	/// Archetype definition for connection entities
	/// </summary>
	public static class Connection
	{
		public static readonly Archetype Archetype = new Archetype("Connection")
			.Extends(Node.Archetype)
			.HasComponent(new ComponentBinding<Components.EntityTypes.Connection>())
			.HasComponent(new ComponentBinding<BandwidthResource>());
	}
}
