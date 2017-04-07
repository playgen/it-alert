using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Archetypes
{
	public static class TutorialText
	{
		public static Archetype Archetype = new Archetype(nameof(TutorialText))
			.HasComponent<ScenarioText>()
			.HasComponent<Text>();
	}
}
