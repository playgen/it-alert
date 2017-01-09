using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Plugin.RoomStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions;
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
			var lobbyState = new LobbyState(photonPlugin, messenger, playerManager, sugarController);
			var lobbyToGameTransition = new LobbyToGameTransition(lobbyState);
			lobbyState.AddTransitions(lobbyToGameTransition);
			
	        var gameState = new GameState(photonPlugin, messenger, playerManager, sugarController);

			var controller = new RoomStateController(lobbyState, gameState);

	        gameState.ParentStateController = controller;

			return controller;
        }
    }
}
