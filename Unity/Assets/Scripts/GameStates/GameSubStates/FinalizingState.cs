using System;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using PlayGen.Photon.Messaging;
using PlayGen.ITAlert.Photon.Messages.Simulation.PlayerState;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class FinalizingState : TickState
	{
		public const string StateName = "Finalizing";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public FinalizingState(Client networkClient)
		{
			_networkClient = networkClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			_networkClient.CurrentRoom.Messenger.Subscribe((int)Photon.Messages.Channels.Feedback, ProcessFeedbackMessage);

			_networkClient.CurrentRoom.Messenger.SendMessage(new FinalizedMessage()
			{
				PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId
			});
		}

		protected override void OnExit()
		{
			_networkClient.CurrentRoom.Messenger.Unsubscribe((int)Photon.Messages.Channels.Feedback, ProcessFeedbackMessage);
		}

		//todo change these to be called game state messages as they're used for state synch
		private void ProcessFeedbackMessage(Message message)
		{
			var feedbackStartedMessage = message as FeedbackStartedMessage;
			if (feedbackStartedMessage != null)
			{
				// todo refactor states
				//ChangeState(FeedbackState.StateName);
				return;
			}

			throw new Exception("Unhandled Feedback Message: " + message);
		}
	}
}