using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin.States;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using State = PlayGen.ITAlert.Photon.Players.State;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class FeedbackState : RoomState
	{
		public const string StateName = "Feedback";

		public override string Name => StateName;

		public event Action<List<Player>> PlayerFeedbackSentEvent;

		public FeedbackState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController) : base(photonPlugin, messenger, playerManager, sugarController)
		{
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)Channel.Feedback, ProcessFeedbackMessage);

			Messenger.SendAllMessage(new Messages.Game.States.FeedbackMessage());
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)Channel.Feedback, ProcessFeedbackMessage);
		}

		private void ProcessFeedbackMessage(Message message)
		{
			var feedbackMessage = message as PlayerFeedbackMessage;
			if (feedbackMessage != null)
			{
				var player = PlayerManager.Get(feedbackMessage.PlayerPhotonId);
				player.State = State.FeedbackSent.IntValue();
				PlayerManager.UpdatePlayer(player);
				
				WritePlayerFeedback(feedbackMessage.RankedPlayerPhotonIdBySection);
			
				PlayerFeedbackSentEvent(PlayerManager.Players);
				return;
			}

			throw new Exception($"Unhandled Simulation State Message: ${message}");
		}

		private void WritePlayerFeedback(Dictionary<string, int[]> rankedPlayerPhotonIdBySection)
		{
			foreach (var feedbackKVP in rankedPlayerPhotonIdBySection)
			{
				var category = feedbackKVP.Key;

				for (int i = 0; i < feedbackKVP.Value.Length; i++)
				{
					var rank = i + 1;
					var playerPhotonId = feedbackKVP.Value[i];
					var playerSugarId = PlayerManager.Players.Single(p => p.PhotonId == playerPhotonId).ExternalId.Value;

					SugarController.AddMatchRankingData(category, rank, playerSugarId);
				}
			}
		}
	}
}