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
		public static readonly Archetype Archetype = new Archetype("Actor")
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
	}
}
