using System;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.Commands;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.Photon.Analytics;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class LobbyState : ITAlertRoomState
	{
		public const string StateName = "Lobby";     

		public override string Name => StateName;

		public event Action GameStartedEvent;

		public LobbyState(PluginBase photonPlugin, 
			Messenger messenger, 
			ITAlertPlayerManager playerManager, 
			RoomSettings roomSettings,
			AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager, roomSettings, analytics)
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
			PlayerManager.PlayerLeft += PlayerManagerOnPlayerLeft;
			PlayerManager.PlayersUpdated += TryStartGame;
			RoomSettings.MinPlayersChangedEvent += TryStartGame;
		}

		private void PlayerManagerOnPlayerLeft(int i)
		{
			PlayerManager.ChangeAllState((int)ClientState.NotReady);
		}

		protected override void OnExit()
		{
			RoomSettings.MinPlayersChangedEvent -= TryStartGame;
			PlayerManager.PlayersUpdated -= TryStartGame;
			Messenger.Unsubscribe((int)ITAlertChannel.GameCommands, ProcessGameCommandMessage);
		}

		private void ProcessGameCommandMessage(Message message)
		{
			var startGameMessage = message as StartGameMessage;
			if (startGameMessage != null)
			{
				TryStartGame(startGameMessage.Force);
				return;
			}

			throw new Exception($"Unhandled Room Message: ${message}");
		}

		private void TryStartGame()
		{
			TryStartGame(false);
		}

		private void TryStartGame(bool force)
		{
			if (force || (PlayerManager.Players.Count >= RoomSettings.MinPlayers &&
					PlayerManager.Players.GetCombinedStates() == ClientState.Ready))
			{
				if (RoomSettings.CloseOnStarted)
				{
					RoomSettings.IsOpen = false;
					RoomSettings.IsVisible = false;
				}

				GameStartedEvent?.Invoke();
			}
		}
	}
}
