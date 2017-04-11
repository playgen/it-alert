using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes
{
	public static class TutorialSubsystem
	{
		public static readonly Archetype Archetype = new Archetype(nameof(TutorialSubsystem))
			.Extends(SubsystemNode.Archetype)
			.HasComponent(new ComponentBinding<TutorialHighlight>()
			{
				ComponentTemplate = new TutorialHighlight()
				{
					Enabled = false,
				}
			});
	}
}
