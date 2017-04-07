using System;
using Engine.Components;
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
	public class CPUConsumptionIncreasesTimedActivationDurationExtension : IActivationExtension
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

		private readonly ComponentMatcherGroup<TimedActivation, CurrentLocation> _timedActivationMatcherGroup;

		public CPUConsumptionIncreasesTimedActivationDurationExtension(IMatcherProvider matcherProvider)
		{
			_timedActivationMatcherGroup = matcherProvider.CreateMatcherGroup<TimedActivation, CurrentLocation>();
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, CPUResource, MovementCost>();
		}

		public void OnNotActive(int itemId, Activation activation)
		{
			if (_timedActivationMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple))
			{
				itemTuple.Component1.ActivationTickModifier = SimulationConstants.TimedActivationTickModifier;
			}
		}

		public void OnActivating(int itemId, Activation activation)
		{
		}

		public void OnActive(int itemId, Activation activation)
		{
			if (_timedActivationMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple)
				&& itemTuple.Component2.Value.HasValue
				&& _subsystemMatcher.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out var subsystemTuple))
			{
				var activationTickModifier = (1 - (subsystemTuple.Component2.Value / (decimal) subsystemTuple.Component2.Maximum) * CPURatioScalingFactor);
				itemTuple.Component1.ActivationTickModifier = RangeHelper.AssignWithinBounds(activationTickModifier, 0, Int32.MaxValue);
			}
		}

		public void OnDeactivating(int entityId, Activation activation)
		{

		}
	}
}
