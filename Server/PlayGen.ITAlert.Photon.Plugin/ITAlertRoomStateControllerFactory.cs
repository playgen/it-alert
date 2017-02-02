using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Plugin.RoomStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.Interfaces;

namespace PlayGen.ITAlert.Photon.Plugin
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertRoomStateControllerFactory : IRoomStateControllerFactory
	{
		/// <summary>
		/// This is where it all begins!
		/// </summary>
		/// <param name="photonPlugin"></param>
		/// <param name="messenger"></param>
		/// <param name="playerManager"></param>
		/// <param name="analytics"></param>
		/// <returns></returns>
		public RoomStateController Create(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager,RoomSettings roomSettings, AnalyticsServiceManager analytics)
		{
			var lobbyState = new LobbyState(photonPlugin, messenger, playerManager, roomSettings, analytics);
			var gameStartedTransition = new EventTransition(GameState.StateName);
			lobbyState.GameStartedEvent += gameStartedTransition.ChangeState;
			lobbyState.AddTransitions(gameStartedTransition);

			var gameState = new GameState(photonPlugin, messenger, playerManager, roomSettings, analytics);
			var controller = new RoomStateController(lobbyState, gameState);

			gameState.ParentStateController = controller;
			
			return controller;
		}
	}
}
