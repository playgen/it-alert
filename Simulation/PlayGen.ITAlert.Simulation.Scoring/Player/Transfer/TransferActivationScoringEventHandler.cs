using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;

namespace PlayGen.ITAlert.Simulation.Scoring.Player.Transfer
{
	public class TransferActivationScoringEventHandler : PlayerScoringEventHandler<TransferActivationEvent>
	{
		public TransferActivationScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider) 
			: base(eventSystem, matcherProvider)
		{
		}

		protected override void HandleEvent(TransferActivationEvent @event)
		{
			if (PlayerScoreMatcherGroup.TryGetMatchingEntity(@event.PlayerEntityId,out var playerTuple))
			{
				switch (@event.ActivationResult)
				{
					case TransferActivationEvent.TransferActivationResult.NoItemsPresent:
						break;
					case TransferActivationEvent.TransferActivationResult.PulledItem:
					case TransferActivationEvent.TransferActivationResult.PushedItem:
					case TransferActivationEvent.TransferActivationResult.SwappedItems:
						playerTuple.Component2.ResourceManagement += 1;
						break;
				}
				playerTuple.Component2.ActionCompleted(ActivationEventScoring.GetMultiplier(@event.ActivationResult));
			}
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
