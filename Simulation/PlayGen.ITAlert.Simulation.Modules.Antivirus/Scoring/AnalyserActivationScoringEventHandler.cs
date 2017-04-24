using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Events;
using Engine.Systems.Scoring;
using PlayGen.ITAlert.Scoring.Components;
using PlayGen.ITAlert.Scoring.EventHandlers;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Scoring
{
	public class AnalyserActivationScoringEventHandler : PlayerScoringEventHandler<AnalyserActivationEvent>
	{

		private readonly ComponentMatcherGroup<MalwareGenome, CurrentLocation> _malwareMatcherGroup;

		private readonly ComponentMatcherGroup<Components.Antivirus> _antivirusMatcherGroup;

		public AnalyserActivationScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider)
			: base (eventSystem, matcherProvider)
		{
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome, CurrentLocation>();
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<Components.Antivirus>();
		}

		protected override void HandleEvent(AnalyserActivationEvent @event)
		{
			if (@event.PlayerEnttityId.HasValue
				&& PlayerScoreMatcherGroup.TryGetMatchingEntity(@event.PlayerEnttityId.Value, out var playerTuple))
			{
				var avWorkstationInfected = _malwareMatcherGroup.MatchingEntities.Any(mw => mw.Component2.Value == @event.LocationEntityId);
				var malwareByGenome = _malwareMatcherGroup.MatchingEntities.GroupBy(mw => mw.Component1.Value);
				var antivirusThisGenome = _antivirusMatcherGroup.MatchingEntities.Count(av => av.Component1.TargetGenome == @event.GenomeProduced);

				var malwareThisGenome = 0;

				foreach (var malwareGenomeGroup in malwareByGenome)
				{
					if ((malwareGenomeGroup.Key & @event.GenomeProduced) == @event.GenomeProduced)
					{
						malwareThisGenome += malwareGenomeGroup.Count();
					}
				}

				switch (@event.ActivationResult)
				{
					case AnalyserActivationEvent.AnalyserActivationResult.Error:
						// TODO: log
						break;

					case AnalyserActivationEvent.AnalyserActivationResult.NoSamplePresent:
						playerTuple.Component2.ResourceManagement -= 1;
						break;
					case AnalyserActivationEvent.AnalyserActivationResult.OutputContainerFull:
						playerTuple.Component2.ResourceManagement -= 1;
						break;
					case AnalyserActivationEvent.AnalyserActivationResult.AnalysisComplete:
						// +/- depending whether there are more av than virus for this gene
						playerTuple.Component2.ResourceManagement += malwareThisGenome > antivirusThisGenome
							? -1
							: 1;
						// one if the workstation is infected
						playerTuple.Component2.Systematicity += avWorkstationInfected
							? 1
							: 0;
						// one for each virus of this colour
						playerTuple.Component2.Systematicity += (Math.Max(1, malwareThisGenome) - 1);
						break;
				}
			}
		}
	}
}
