using System;
using Engine.Components;
using Engine.Systems;
using Engine.Systems.Activation.Components;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Resources.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Resources.Systems
{
	/// <summary>
	/// This subsystem resource effect causes the value of the MovementSpeed component to be modulated accoring to the utilisation of the CPU Resource on that node
	/// </summary>
	//// ReSharper disable once InconsistentNaming
	public class CPUConsumptionIncreasesTimedActivationDurationSystem : ITickableSystem
	{
		/*
		 * a constant of 0.7 gives the following timed activation duration multiplier for each elvel of memory consumption
		 * 0: 1x
		 * 1: 1.21x
		 * 2: 1.54x
		 * 3: 2.11x
		 * 4: 3.33x
		 */
		private const decimal CPURatioScalingFactor = 0.7m;

		private readonly ComponentMatcherGroup<Subsystem, CPUResource, MovementCost> _subsystemMatcher;

		private readonly ComponentMatcherGroup<Engine.Systems.Activation.Components.Activation, TimedActivation, CurrentLocation> _timedActivationMatcherGroup;

		public CPUConsumptionIncreasesTimedActivationDurationSystem(IMatcherProvider matcherProvider)
		{
			_timedActivationMatcherGroup = matcherProvider.CreateMatcherGroup<Engine.Systems.Activation.Components.Activation, TimedActivation, CurrentLocation>();
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, CPUResource, MovementCost>();
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _timedActivationMatcherGroup.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.NotActive:
						OnNotActive(match);
						break;
					case ActivationState.Active:
						OnActive(match);
						break;
				}
			}
		}

		private void OnNotActive(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, TimedActivation, CurrentLocation> entityTuple)
		{
			entityTuple.Component2.ActivationTickModifier = SimulationConstants.TimedActivationTickModifier;
		}

		private void OnActive(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, TimedActivation, CurrentLocation> entityTuple)
		{
			if (entityTuple.Component3.Value.HasValue
				&& _subsystemMatcher.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var subsystemTuple))
			{
				var activationTickModifier = (1 - (subsystemTuple.Component2.Value / (decimal) subsystemTuple.Component2.Maximum) * CPURatioScalingFactor);
				entityTuple.Component2.ActivationTickModifier = RangeHelper.AssignWithinBounds(activationTickModifier, 0, Int32.MaxValue);
			}
		}

		public void Dispose()
		{
			_subsystemMatcher?.Dispose();
			_timedActivationMatcherGroup?.Dispose();
		}
	}
}
