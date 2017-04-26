using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;

namespace PlayGen.ITAlert.Scoring.Transfer
{
	public class TransferActivationScoringEventHandler : PlayerScoringEventHandler<TransferActivationEvent>
	{
		public TransferActivationScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider) 
			: base(eventSystem, matcherProvider)
		{
		}

		protected override void HandleEvent(TransferActivationEvent @event)
		{
			var resourceManagementModifier = 0;
			var systematicityModifier = 0;
			
			switch (@event.ActivationResult)
			{
				case TransferActivationEvent.TransferActivationResult.NoItemsPresent:

					break;
				case TransferActivationEvent.TransferActivationResult.PulledItem:
				case TransferActivationEvent.TransferActivationResult.PushedItem:
				case TransferActivationEvent.TransferActivationResult.SwappedItems:
					resourceManagementModifier += 1;
					break;
			}

			if (PlayerScoreMatcherGroup.TryGetMatchingEntity(@event.PlayerEntityId,
				out var playerTuple))
			{
				playerTuple.Component2.ResourceManagement += resourceManagementModifier;
				playerTuple.Component2.Systematicity += systematicityModifier;
			}
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
