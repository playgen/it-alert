using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Events;
using Engine.Systems.Scoring;
using PlayGen.ITAlert.Scoring.EventHandlers;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Scoring
{
	public class AntivirusActivationScoringEventHandler : PlayerScoringEventHandler<AntivirusActivationEvent>
	{
		public AntivirusActivationScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider) 
			: base(eventSystem, matcherProvider)
		{
		}

		protected override void HandleEvent(AntivirusActivationEvent @event)
		{
			if (PlayerScoreMatcherGroup.TryGetMatchingEntity(@event.PlayerEntityId,
					out var playerTuple))
			{
				switch (@event.ActivationResult)
				{
					case AntivirusActivationEvent.AntivirusActivationResult.Error:
						// TODO: log
						break;

					case AntivirusActivationEvent.AntivirusActivationResult.NoVirusPresent:
					case AntivirusActivationEvent.AntivirusActivationResult.IncorrectGenome:
						playerTuple.Component2.ResourceManagement -= 1;
						playerTuple.Component2.Systematicity -= 1;
						break;

					case AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination:
						playerTuple.Component2.ResourceManagement += 1;
						playerTuple.Component2.Systematicity += 1;
						break;

					case AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination:
						playerTuple.Component2.ResourceManagement += 1;
						playerTuple.Component2.Systematicity += 1;

						break;
				}
			}
		}
	}
}
