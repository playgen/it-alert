using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Plugin.RoomStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.Analytics;
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
		public RoomStateController Create(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, AnalyticsServiceManager analytics)
		{
			var lobbyState = new LobbyState(photonPlugin, messenger, playerManager, analytics);
			var gameStartedTransition = new EventTransition(GameState.StateName);
			lobbyState.GameStartedEvent += gameStartedTransition.ChangeState;
			lobbyState.AddTransitions(gameStartedTransition);

			var gameState = new GameState(photonPlugin, messenger, playerManager, analytics);
			var controller = new RoomStateController(lobbyState, gameState);

			gameState.ParentStateController = controller;
			
			return controller;
		}
	}
}
