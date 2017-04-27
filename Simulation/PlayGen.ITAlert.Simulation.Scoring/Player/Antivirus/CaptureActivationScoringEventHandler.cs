using System.Linq;
using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;

namespace PlayGen.ITAlert.Simulation.Scoring.Player.Antivirus
{
	public class CaptureActivationScoringEventHandler : PlayerScoringEventHandler<CaptureActivationEvent>
	{
		private readonly ComponentMatcherGroup<Subsystem, AntivirusEnhancement> _antivirusWorkstationMatcherGroup;

		public CaptureActivationScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider)
			: base (eventSystem, matcherProvider)
		{
			_antivirusWorkstationMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, AntivirusEnhancement>();
		}

		protected override void HandleEvent(CaptureActivationEvent @event)
		{
			var onAvWorkstation = _antivirusWorkstationMatcherGroup.MatchingEntityKeys.Contains(@event.LocationEntityId);

			var systematicityModifier = 0;

			switch (@event.ActivationResult)
			{
				case CaptureActivationEvent.CaptureActivationResult.NoVirusPresent:
					systematicityModifier += -1;
					break;
				case CaptureActivationEvent.CaptureActivationResult.SimpleGenomeCaptured:
					systematicityModifier += onAvWorkstation
						? 2
						: 1;
					break;
				case CaptureActivationEvent.CaptureActivationResult.ComplexGenomeCaptured:
					systematicityModifier += onAvWorkstation
						? 3
						: 2;
					break;
			}
			if (PlayerScoreMatcherGroup.TryGetMatchingEntity(@event.PlayerEntityId, out var playerTuple))
			{
				playerTuple.Component2.Systematicity += systematicityModifier;
			}
		}

		public override void Dispose()
		{
			_antivirusWorkstationMatcherGroup?.Dispose();
			base.Dispose();
		}
	}
}
