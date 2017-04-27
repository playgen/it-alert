using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Modules.Resources.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Resources.Systems
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

		public void Dispose()
		{
			_subsystemMatcher?.Dispose();
		}
	}
}
