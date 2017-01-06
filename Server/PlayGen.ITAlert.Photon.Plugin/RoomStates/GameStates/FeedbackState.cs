using System;
using System.Collections.Generic;
using GameWork.Core.States;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using State = PlayGen.ITAlert.Photon.Players.State;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class FeedbackState : RoomState
	{
		public const string StateName = "Feedback";

		public override string Name => StateName;

		public event Action<List<Player>> PlayerFeedbackSentEvent;

		public FeedbackState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController, params EventStateTransition[] stateTransitions) 
			: base(photonPlugin, messenger, playerManager, sugarController, stateTransitions)
		{
		}

		public override void Enter()
		{
			Messenger.Subscribe((int)Channels.Feedback, ProcessFeedbackMessage);

			Messenger.SendAllMessage(new FeedbackStartedMessage());
		}

		public override void Exit()
		{
			Messenger.Unsubscribe((int)Channels.Feedback, ProcessFeedbackMessage);
		}

		private void ProcessFeedbackMessage(Message message)
		{
			var feedbackMessage = message as PlayerFeedbackMessage;
			if (feedbackMessage != null)
			{
				var player = PlayerManager.Get(feedbackMessage.PlayerPhotonId);
				player.State = (int)State.FeedbackSent;
				PlayerManager.UpdatePlayer(player);

				// todo write feedback fields to sugar using SugarController and adding it as match data
				// note: the player's sugar Id is stored in the PlayerManager's players as ExternalId
				
				PlayerFeedbackSentEvent(PlayerManager.Players);
				return;
			}

			throw new Exception($"Unhandled Simulation State Message: ${message}");
		}
	}
}