using System;
using GameWork.Core.States;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using PlayGen.Photon.Messaging;
using PlayGen.ITAlert.Photon.Messages.Game;
using PlayGen.ITAlert.Photon.Messages.Simulation.PlayerState;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class FinalizingState : TickableSequenceState
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

		public override void Enter()
		{
			Logger.LogDebug("Entered " + StateName);

			_networkClient.CurrentRoom.Messenger.Subscribe((int)Photon.Messages.Channels.Feedback, ProcessFeedbackMessage);

			_networkClient.CurrentRoom.Messenger.SendMessage(new FinalizedMessage()
			{
				PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId
			});
		}

		public override void Exit()
		{
			_networkClient.CurrentRoom.Messenger.Unsubscribe((int)Photon.Messages.Channels.Feedback, ProcessFeedbackMessage);
		}

		//todo change these to be called game state messages as they're used for state synch
		private void ProcessFeedbackMessage(Message message)
		{
			var feedbackStartedMessage = message as FeedbackStartedMessage;
			if (feedbackStartedMessage != null)
			{
				ChangeState(FeedbackState.StateName);
				return;
			}

			throw new Exception("Unhandled Feedback Message: " + message);
		}

		public override void NextState()
		{
		}

		public override void PreviousState()
		{
		}

		public override void Tick(float deltaTime)
		{
		}
	}
}