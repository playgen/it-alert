using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	public static class Actor
	{
		public static readonly Archetype Archetype = new Archetype(nameof(Actor))
			.HasComponent<CurrentLocation>()
			.HasComponent<Destination>()
			.HasComponent<VisitorPosition>()
			.HasComponent<MovementSpeed>();
	}
}
