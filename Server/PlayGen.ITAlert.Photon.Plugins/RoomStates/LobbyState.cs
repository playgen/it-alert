using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Plugins.Extensions;
using PlayGen.ITAlert.Photon.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugins.RoomStates
{
	public class LobbyState : RoomState
	{
		public const string StateName = "Lobby";     

		public override string Name
		{
			get { return StateName; }
		}

		public LobbyState(PluginBase plugin, PlayerManager playerManager, Controller sugarController)
			: base(plugin, playerManager, sugarController)
		{
		}

		#region Events
		public override void OnRaiseEvent(IRaiseEventCallInfo info)
		{
			switch (info.Request.EvCode)
			{
				case (byte)PlayerEventCode.SetReady:
					ChangePlayerStatus(info.ActorNr, PlayerStatus.Ready);
					break;

				case (byte)PlayerEventCode.SetNotReady:
					ChangePlayerStatus(info.ActorNr, PlayerStatus.NotReady);
					break;

				case (byte)PlayerEventCode.StartGame:
					var data = (bool[])info.Request.Data;
					StartGame(data[0], data[1]);
					break;
			}
		}

		public override void OnCreate(ICreateGameCallInfo info)
		{
		}

		public override void OnJoin(IJoinGameCallInfo info)
		{
		}

		public override void OnLeave(ILeaveGameCallInfo info)
		{
		}

		#endregion

		public override void Enter()
		{
			PlayerManager.ChangeAllStatuses(PlayerStatus.NotReady);
			Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.PlayerList, PlayerManager.Players);
		}

		public override void Exit()
		{
		}

		private void StartGame(bool force, bool close)
		{
			if (force || PlayerManager.CombinedPlayerStatus == PlayerStatus.Ready)
			{
				if (close)
				{
					Plugin.SetRoomOpen(false);
				}

				// TODO: gamework upgrade
				//ChangeState(GameState.StateName);
			}
		}

		private void ChangePlayerStatus(int playerId, PlayerStatus status)
		{
			var didChange = PlayerManager.ChangeStatus(playerId, status);
			if (didChange)
			{
				Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.PlayerList, PlayerManager.Players);
			}
		}
	}
}
