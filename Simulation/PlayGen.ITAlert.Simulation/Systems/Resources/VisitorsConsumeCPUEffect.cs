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
	/// This subsystem resource effect causes the CPU Resource to be consumed by visitors located on Subsystems with the Visitors component
	/// </summary>
	//// ReSharper disable once InconsistentNaming
	public class VisitorsConsumeCPUEffect : ISubsystemResourceEffect
	{
		private readonly ComponentMatcherGroup<Subsystem, Visitors, CPUResource> _subsystemMatcher;
		private readonly ComponentMatcherGroup<VisitorPosition, ConsumeCPU> _visitorMatcher;

		public VisitorsConsumeCPUEffect(IMatcherProvider matcherProvider, IEntityRegistry entityRegistry)
		{
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, Visitors, CPUResource>();
			_visitorMatcher = matcherProvider.CreateMatcherGroup<VisitorPosition, ConsumeCPU>();
		}

		public void Tick()
		{
			foreach (var subsystemTuple in _subsystemMatcher.MatchingEntities)
			{
				var sum = subsystemTuple.Component3.Value;
				foreach (var visitorId in subsystemTuple.Component2.Values)
				{
					ComponentEntityTuple<VisitorPosition, ConsumeCPU> visitorTuple;
					if (_visitorMatcher.TryGetMatchingEntity(visitorId, out visitorTuple))
					{
						sum += visitorTuple.Component2.Value;
					}
				}
				subsystemTuple.Component3.Value = RangeHelper.AssignWithinBounds(sum, 0, subsystemTuple.Component3.Maximum);
			}
		}
	}
}
