using System;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.Commands;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Plugin.Extensions;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.Photon.Plugin.Analytics;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class LobbyState : RoomState
	{
		public const string StateName = "Lobby";     

		public override string Name => StateName;

		public event Action GameStartedEvent;

		public LobbyState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, 
			RoomController roomController, AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager, roomController, analytics)
		{
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)ITAlertChannel.GameCommands, ProcessGameCommandMessage);
			
			Messenger.SendAllMessage(new LobbyMessage
			{
				PlayerPhotonId = RoomControllerPlugin.ServerPlayerId,
			});

			PlayerManager.ChangeAllState((int)ClientState.NotReady);
			PlayerManager.PlayersUpdated += TryStartGame;
			RoomController.MinPlayersChangedEvent += TryStartGame;
		}

		protected override void OnExit()
		{
			RoomController.MinPlayersChangedEvent -= TryStartGame;
			PlayerManager.PlayersUpdated -= TryStartGame;
			Messenger.Unsubscribe((int)ITAlertChannel.GameCommands, ProcessGameCommandMessage);
		}

		private void ProcessGameCommandMessage(Message message)
		{
			var startGameMessage = message as StartGameMessage;
			if (startGameMessage != null)
			{
				TryStartGame(startGameMessage.Force, startGameMessage.Close);
				return;
			}

			throw new Exception($"Unhandled Room Message: ${message}");
		}

		private void TryStartGame()
		{
			TryStartGame(false, true);
		}

		private void TryStartGame(bool force, bool close)
		{
			if (AreStartGameConditionsMet() || force)
			{
				if (close)
				{
					RoomController.TrySetOpen(!close);
				}

				GameStartedEvent?.Invoke();
			}
		}

		private bool AreStartGameConditionsMet()
		{
			return PlayerManager.Players.Count >= RoomController.MinPlayers &&
					PlayerManager.Players.GetCombinedStates() == ClientState.Ready;
		}
	}
}
