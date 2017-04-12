using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes
{
	public static class VisibleRedTutorialVirus
	{
		public static readonly Archetype Archetype = new Archetype(nameof(VisibleRedTutorialVirus))
			.Extends(RedTutorialVirus.Archetype)
			.HasComponent(new ComponentBinding<MalwareVisibility>()
			{
				ComponentTemplate = new MalwareVisibility()
				{
					VisibleTo = MalwareVisibility.All,
				}
			});
	}
}
