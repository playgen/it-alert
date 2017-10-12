using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Feedback
{
	public class FeedbackState : InputTickState
	{
		public const string StateName = "Feedback";

		private readonly ITAlertPhotonClient _photonClient;

		public override string Name => StateName;

		public FeedbackState(FeedbackStateInput input, ITAlertPhotonClient photonClient) : base(input)
		{
			input.PlayerRankingsCompleteEvent += OnPlayerRankingsComplete;
			_photonClient = photonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);
		}

		private void OnPlayerRankingsComplete(Dictionary<int, int[]> rankedPlayerPhotonIdBySection)
		{
			_photonClient.CurrentRoom.Messenger.SendMessage(new PlayerFeedbackMessage()
			{
				PlayerPhotonId = _photonClient.CurrentRoom.Player.PhotonId,
				RankedPlayerPhotonIdBySection = rankedPlayerPhotonIdBySection,
			});
		}
	}
}