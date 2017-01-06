using System;
using GameWork.Core.States;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.SUGAR;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.Messages.Players;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Plugin.Extensions;
using PlayGen.ITAlert.Photon.Players.Extensions;
using State = PlayGen.ITAlert.Photon.Players.State;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class LobbyState : RoomState
	{
		public const string StateName = "Lobby";     

		public override string Name => StateName;

		public event Action GameStartedEvent;

		public LobbyState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController, params EventStateTransition[] stateTransitions)
			: base(photonPlugin, messenger, playerManager, sugarController, stateTransitions)
		{
		}

		public override void Enter()
		{
			Messenger.Subscribe((int)Channels.Game, ProcessGameMessage);

			ResetAllPlayerStatuses();
		}

		public override void Exit()
		{
			Messenger.Unsubscribe((int)Channels.Game, ProcessGameMessage);
		}

		private void ProcessGameMessage(Message message)
		{
			var startGameMessage = message as StartGameMessage;
			if (startGameMessage != null)
			{
				StartGame(startGameMessage.Force, startGameMessage.Close);
				return;
			}

			throw new Exception($"Unhandled Room Message: ${message}");
		}

		private void ResetAllPlayerStatuses()
		{
			PlayerManager.ChangeAllState((int)State.NotReady);
			Messenger.SendAllMessage(new ListedPlayersMessage
			{
				Players = PlayerManager.Players,
			});
		}

		private void StartGame(bool force, bool close)
		{
			if (force || PlayerManager.Players.GetCombinedStates() == State.Ready)
			{
				if (close)
				{
					PhotonPlugin.SetRoomOpen(false);
				}

				GameStartedEvent?.Invoke();
			}
		}
	}
}
