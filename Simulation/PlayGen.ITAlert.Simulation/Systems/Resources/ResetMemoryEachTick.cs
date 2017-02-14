using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Components.Resources;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	/// <summary>
	/// This subsystem resource effect causes the Memory Resource value to be reset to zero on each tick and should be placed before effects that incremenmt the counter
	/// </summary>
	public class ResetMemoryEachTick : ISubsystemResourceEffect
	{
		private readonly ComponentMatcherGroup<Subsystem, MemoryResource> _subsystemMatcher;

		public ResetMemoryEachTick(IMatcherProvider matcherProvider, IEntityRegistry entityRegistry)
		{
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, MemoryResource>();
		}

		public void Tick()
		{
			foreach (var subsystemTuple in _subsystemMatcher.MatchingEntities)
			{
				subsystemTuple.Component2.Value = 0;
			}
		}
	}
}
