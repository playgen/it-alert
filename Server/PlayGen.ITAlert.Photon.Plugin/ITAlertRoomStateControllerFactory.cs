using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Plugin.RoomStates;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.Interfaces;
using PlayGen.Photon.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugin
{
	public class ITAlertRoomStateControllerFactory : IRoomStateControllerFactory
	{
		public RoomStateController Create(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController)
		{
			return new RoomStateController(new LobbyState(photonPlugin, messenger, playerManager, sugarController), 
				new GameState(photonPlugin, messenger, playerManager, sugarController));
		}
	}
}
