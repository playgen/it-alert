using System.Linq;
using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;

namespace PlayGen.ITAlert.Scoring.Antivirus
{
	public class AntivirusActivationScoringEventHandler : PlayerScoringEventHandler<AntivirusActivationEvent>
	{
		private readonly ComponentMatcherGroup<Subsystem, AntivirusEnhancement> _antivirusWorkstationMatcherGroup;

		public AntivirusActivationScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider) 
			: base(eventSystem, matcherProvider)
		{
			_antivirusWorkstationMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, AntivirusEnhancement>();
		}

		protected override void HandleEvent(AntivirusActivationEvent @event)
		{
			var onAvWorkstation = _antivirusWorkstationMatcherGroup.MatchingEntityKeys.Contains(@event.LocationEntityId);

			var resourceManagementModifier = 0;
			var systematicityModifier = 0;

			switch (@event.ActivationResult)
			{
				case AntivirusActivationEvent.AntivirusActivationResult.Error:
					// TODO: log
					break;

				case AntivirusActivationEvent.AntivirusActivationResult.NoVirusPresent:
				case AntivirusActivationEvent.AntivirusActivationResult.IncorrectGenome:
					resourceManagementModifier -= 1;
					systematicityModifier -= 1;
					break;

				case AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination:
					resourceManagementModifier += 1;
					systematicityModifier += onAvWorkstation 
						? 2
						: 1;

					break;

				case AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination:
					resourceManagementModifier += 2;
					systematicityModifier += onAvWorkstation
						? 2
						: 1;

					break;
			}
			if (PlayerScoreMatcherGroup.TryGetMatchingEntity(@event.PlayerEntityId, out var playerTuple))
			{
				playerTuple.Component2.ResourceManagement += resourceManagementModifier;
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
