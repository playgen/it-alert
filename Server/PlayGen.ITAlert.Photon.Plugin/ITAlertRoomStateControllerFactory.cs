﻿using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Plugin.RoomStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.Interfaces;
using PlayGen.Photon.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugin
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertRoomStateControllerFactory : IRoomStateControllerFactory<ITAlertRoomStateController, ITAlertRoomState, ITAlertPlayerManager, ITAlertPlayer>
	{
		/// <summary>
		/// This is where it all begins!
		/// </summary>
		/// <param name="photonPlugin"></param>
		/// <param name="messenger"></param>
		/// <param name="playerManager"></param>
		/// <returns></returns>
		public ITAlertRoomStateController Create(PluginBase photonPlugin, 
			Messenger messenger, 
			ITAlertPlayerManager playerManager, 
			ExceptionHandler exceptionHandler)
		{
			var sugarAnalytics = new AnalyticsServiceAdapter();
			var analytics = new AnalyticsServiceManager(sugarAnalytics);
			var roomSettings = new RoomSettings(photonPlugin);

			var exceptionTransition = new EventTransition(ErrorState.StateName);
			exceptionHandler.ExceptionEvent += e => exceptionTransition.ChangeState();

			var lobbyState = new LobbyState(photonPlugin, messenger, playerManager, roomSettings, analytics);
			var gameStartedTransition = new EventTransition(GameState.StateName);
			lobbyState.GameStartedEvent += gameStartedTransition.ChangeState;
			lobbyState.AddTransitions(gameStartedTransition);

			var gameState = new GameState(photonPlugin, messenger, playerManager, roomSettings, analytics, exceptionHandler);
			gameState.AddTransitions(exceptionTransition);

			var errorState = new ErrorState(photonPlugin, messenger, playerManager, roomSettings, analytics, exceptionHandler);
			var controller = new ITAlertRoomStateController(lobbyState, gameState, errorState);

			gameState.ParentStateController = controller;
			
			return controller;
		}
	}
}
