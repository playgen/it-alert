using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Resources.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Resources.Systems
{
	/// <summary>
	/// This subsystem resource effect causes the value of the MovementSpeed component to be modulated accoring to the utilisation of the CPU Resource on that node
	/// </summary>
	//// ReSharper disable once InconsistentNaming
	public class CPUReducesMovementSpeed : ISubsystemResourceEffect
	{
		private readonly ComponentMatcherGroup<Subsystem, CPUResource, MovementCost> _subsystemMatcher;

		public CPUReducesMovementSpeed(IMatcherProvider matcherProvider)
		{
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, CPUResource, MovementCost>();
		}

		public void Tick()
		{
			foreach (var subsystemTuple in _subsystemMatcher.MatchingEntities)
			{
				subsystemTuple.Component3.Value = subsystemTuple.Component2.Value * SimulationConstants.CPUMovementSpeedReduction;
			}
		}
	}
}
