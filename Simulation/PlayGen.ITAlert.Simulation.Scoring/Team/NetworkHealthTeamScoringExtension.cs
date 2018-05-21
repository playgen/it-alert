using System;
using System.Linq;
using Engine;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Scoring.Team
{
	public class NetworkHealthTeamScoringExtension : ITeamScoringExtension, ITickable
	{
		public event Action<int> Score;

		private readonly ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup;

		private readonly ComponentMatcherGroup<MalwarePropogation, CurrentLocation> _malwareMatcherGroup;

		private const int PointsPerTick = 100;

		public NetworkHealthTeamScoringExtension(IMatcherProvider matcherProvider)
		{
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwarePropogation, CurrentLocation>();
		}

		public void Tick(int currentTick)
		{
			var infections = _malwareMatcherGroup.MatchingEntities.Join(_subsystemMatcherGroup.MatchingEntities,
					mw => mw.Component2.Value,
					ss => ss.Entity.Id,
					(mw, ss) => mw)
				.ToArray();

			var infectionRatio = (decimal)infections.Length / _subsystemMatcherGroup.MatchingEntities.Length;

			//var ageOfInfections = infections.Sum(i => i.Component2.TicksAtLocation);

			//var ageRatio = (decimal) ageOfInfections / (infections.Length * currentTick);

            var inverseInfectionRatio = 1 - infectionRatio;
            OnScore((int)(inverseInfectionRatio * PointsPerTick));
		}

		public void Dispose()
		{
			_subsystemMatcherGroup?.Dispose();
			_malwareMatcherGroup?.Dispose();
		}

		protected virtual void OnScore(int obj)
		{
			Score?.Invoke(obj);
		}
	}
}
