using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Events;
using Engine.Lifecycle;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Feedback;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Simulation.UI.Events;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Plugin;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class FeedbackState : RoomState
	{
		public const string StateName = nameof(FeedbackState);

		public override string Name => StateName;

		public event Action<List<Player>> PlayerFeedbackSentEvent;

		private readonly SimulationLifecycleManager _simulationLifecycleManager;

		public FeedbackState(SimulationLifecycleManager simulationLifecycleManager, 
			PluginBase photonPlugin, 
			Messenger messenger, 
			PlayerManager playerManager,
			RoomSettings roomSettings, 
			AnalyticsServiceManager analytics) 
			: base(photonPlugin, 
				  messenger, 
				  playerManager, 
				  roomSettings, 
				  analytics)
		{
			_simulationLifecycleManager = simulationLifecycleManager;
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)ITAlertChannel.Feedback, ProcessFeedbackMessage);

			//Messenger.SendAllMessage(new Messages.Game.States.FeedbackMessage());
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)ITAlertChannel.Feedback, ProcessFeedbackMessage);
			_simulationLifecycleManager.Dispose();
		}

		private void ProcessFeedbackMessage(Message message)
		{
			var feedbackMessage = message as PlayerFeedbackMessage;
			if (feedbackMessage != null)
			{
				var player = PlayerManager.Get(feedbackMessage.PlayerPhotonId);
				player.State = (int) ClientState.FeedbackSent;
				PlayerManager.UpdatePlayer(player);

				if (_simulationLifecycleManager.ECSRoot.ECS.TryGetSystem<EventSystem>(out var evenTsystem))
				{
					foreach (var feedbackPlayer in feedbackMessage.RankedPlayerPhotonIdBySection)
					{
						var feedbackEvent = new PlayerFeedbackEvent() {
							PlayerExternalId = feedbackMessage.PlayerPhotonId,
							RankedPlayerExternalId = feedbackPlayer.Key,
							PlayerRankings = feedbackPlayer.Value,
						};
						evenTsystem.Publish(feedbackEvent);
					}
				}

				PlayerFeedbackSentEvent(PlayerManager.Players);
				return;
			}

			throw new Exception($"Unhandled Simulation ClientState Message: ${message}");
		}
	}
}