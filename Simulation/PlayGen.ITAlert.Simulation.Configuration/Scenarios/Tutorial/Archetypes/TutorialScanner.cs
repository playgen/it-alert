using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes
{
	public static class TutorialScanner
	{
		public static readonly Archetype Archetype = new Archetype(nameof(TutorialScanner))
			.Extends(ScannerTool.Archetype)
			.HasComponent(new ComponentBinding<TutorialHighlight>()
			{
				ComponentTemplate = new TutorialHighlight()
				{
					Enabled = true,
				}
			});
	}
}
