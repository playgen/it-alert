using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class FeedbackState : InputTickState
	{
		public const string StateName = "Feedback";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public FeedbackState(FeedbackStateInput input, Client networkClient) : base(input)
		{
			input.PlayerRankingsCompleteEvent += OnPlayerRankingsComplete;
			_networkClient = networkClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);
		}

		private void OnPlayerRankingsComplete(Dictionary<string, int[]> rankedPlayerPhotonIdBySection)
		{
			_networkClient.CurrentRoom.Messenger.SendMessage(new PlayerFeedbackMessage()
			{
				PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId,
				RankedPlayerPhotonIdBySection = rankedPlayerPhotonIdBySection,
			});
		}
	}
}