using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using PlayGen.Photon.Messaging;
using PlayGen.ITAlert.Photon.Messages.Game;
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

			_networkClient.CurrentRoom.Messenger.Subscribe((int)Photon.Messages.Channels.Game, ProcessGameMessage);

			// todo gather feedback from user
			_networkClient.CurrentRoom.Messenger.SendMessage(new PlayerFeedbackMessage()
			{
				PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId
			});
		}

		protected override void OnExit()
		{
			_networkClient.CurrentRoom.Messenger.Unsubscribe((int)Photon.Messages.Channels.Game, ProcessGameMessage);
		}

		private void ProcessGameMessage(Message message)
		{
			var gameEndedMessage = message as GameEndedMessage;
			if (gameEndedMessage != null)
			{
				// todo change back to lobby state
				return;
			}
		}
	}
}