using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Room.Feedback
{
	public class FeedbackState : InputTickState
	{
		public const string StateName = "Feedback";

		private readonly Client _photonClient;

		public override string Name
		{
			get { return StateName; }
		}

		public FeedbackState(FeedbackStateInput input, Client photonClient) : base(input)
		{
			input.PlayerRankingsCompleteEvent += OnPlayerRankingsComplete;
			_photonClient = photonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);
		}

		private void OnPlayerRankingsComplete(Dictionary<string, int[]> rankedPlayerPhotonIdBySection)
		{
			_photonClient.CurrentRoom.Messenger.SendMessage(new PlayerFeedbackMessage()
			{
				PlayerPhotonId = _photonClient.CurrentRoom.Player.PhotonId,
				RankedPlayerPhotonIdBySection = rankedPlayerPhotonIdBySection,
			});
		}
	}
}