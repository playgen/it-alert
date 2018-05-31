using System;
using System.Linq;
using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Scoring.Player.Antivirus
{
	public class AnalyserActivationScoringEventHandler : PlayerScoringEventHandler<AnalyserActivationEvent>
	{
		private readonly ComponentMatcherGroup<MalwareGenome, CurrentLocation> _malwareMatcherGroup;

		private readonly ComponentMatcherGroup<ITAlert.Simulation.Modules.Antivirus.Components.Antivirus> _antivirusMatcherGroup;

		public AnalyserActivationScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider)
			: base (eventSystem, matcherProvider)
		{
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome, CurrentLocation>();
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<ITAlert.Simulation.Modules.Antivirus.Components.Antivirus>();
		}

		protected override void HandleEvent(AnalyserActivationEvent @event)
		{
			if (PlayerScoreMatcherGroup.TryGetMatchingEntity(@event.PlayerEntityId, out var playerTuple))
			{
				var avWorkstationInfected = _malwareMatcherGroup.MatchingEntities.Any(mw => mw.Component2.Value == @event.LocationEntityId);
				switch (@event.ActivationResult)
				{
					case AnalyserActivationEvent.AnalyserActivationResult.Error:
						// TODO: log
						break;
					case AnalyserActivationEvent.AnalyserActivationResult.NoSamplePresent:
					case AnalyserActivationEvent.AnalyserActivationResult.OutputContainerFull:
						playerTuple.Component2.ResourceManagement -= 1;
						break;
					case AnalyserActivationEvent.AnalyserActivationResult.AnalysisComplete:
						playerTuple.Component2.ResourceManagement += 1;
						// one extra if the workstation is infected
						playerTuple.Component2.Systematicity += 1 + (avWorkstationInfected ? 1 : 0);
						break;
				}
				playerTuple.Component2.ActionCompleted(ActivationEventScoring.GetMultiplier(@event.ActivationResult));
			}
		}

		public override void Dispose()
		{
			_malwareMatcherGroup?.Dispose();
			_antivirusMatcherGroup?.Dispose();
			base.Dispose();
		}
	}
}
