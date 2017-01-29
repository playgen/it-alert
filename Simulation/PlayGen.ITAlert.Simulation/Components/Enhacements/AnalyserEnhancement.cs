using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Components.Enhacements
{
	[ComponentDependency(typeof(ItemStorage))]
	public sealed class AnalyserEnhancement : IFlagComponent
	{
	}
}
