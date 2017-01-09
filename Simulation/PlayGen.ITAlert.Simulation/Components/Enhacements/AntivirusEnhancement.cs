using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Components.Enhacements
{
	[ComponentDependency(typeof(ItemStorage))]
	public sealed class AntivirusEnhancement : IComponent, IFlagComponent
	{
	}
}
