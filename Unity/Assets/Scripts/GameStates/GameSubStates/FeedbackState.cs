using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class FeedbackState : TickState
	{
		public const string StateName = "Feedback";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public FeedbackState(Client networkClient)
		{
			_networkClient = networkClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			// todo gather feedback from user
			_networkClient.CurrentRoom.Messenger.SendMessage(new PlayerFeedbackMessage()
			{
				PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId
			});
		}
	}
}