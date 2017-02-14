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
	/// This subsystem resource effect causes the CPU Resource value to be reset to zero on each tick and should be placed before effects that incremenmt the counter
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class ResetCPUEachTick : ISubsystemResourceEffect
	{
		private readonly ComponentMatcherGroup<Subsystem, CPUResource> _subsystemMatcher;

		public ResetCPUEachTick(IMatcherProvider matcherProvider, IEntityRegistry entityRegistry)
		{
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, CPUResource>();
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
