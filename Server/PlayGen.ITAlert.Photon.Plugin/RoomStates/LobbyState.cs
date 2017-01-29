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

		public LobbyState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager, analytics)
		{
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)Channel.GameCommands, ProcessGameCommandMessage);
			
			Messenger.SendAllMessage(new LobbyMessage
			{
				PlayerPhotonId = RoomControllerPlugin.ServerPlayerId,
			});

			PlayerManager.ChangeAllState((int)ClientState.NotReady);
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)Channel.GameCommands, ProcessGameCommandMessage);
		}

		private void ProcessGameCommandMessage(Message message)
		{
			var startGameMessage = message as StartGameMessage;
			if (startGameMessage != null)
			{
				StartGame(startGameMessage.Force, startGameMessage.Close);
				return;
			}

			throw new Exception($"Unhandled Room Message: ${message}");
		}
		
		private void StartGame(bool force, bool close)
		{
			if (force || PlayerManager.Players.GetCombinedStates() == ClientState.Ready)
			{
				if (close)
				{
					PhotonPlugin.SetRoomOpen(false);
				}

				GameStartedEvent();
			}
		}
	}
}
