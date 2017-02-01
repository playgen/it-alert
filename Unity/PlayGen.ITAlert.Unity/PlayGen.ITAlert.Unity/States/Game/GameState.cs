using System;
using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.GameStates.Game.Loading;
using PlayGen.ITAlert.Unity.Network;
using PlayGen.ITAlert.Unity.Photon.Messaging;
using PlayGen.Photon.Unity.Client;
using PlayGen.SUGAR.Client;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.GameStates.Game
{
	public class GameState : TickState
	{
		public const string StateName = nameof(GameState);
		private const string GamePlugin = "RoomControllerPlugin";
		private const string GameVersion = "1";
		
		private readonly GameStateControllerFactory _stateControllerFactory;

		private Client _photonClient;
		private SUGARClient _sugarClient;
		private TickStateController _stateController;
		
		public override string Name => StateName;

		public event Action<Exception> ExceptionEvent;
		
		public GameState()
		{
			_stateControllerFactory = new GameStateControllerFactory();
		}

		public void SetSubstateParentController(StateControllerBase parentStateController)
		{
			_stateControllerFactory.ParentStateController = parentStateController;
		}

		protected override void OnEnter()
		{
			_photonClient = new Client(GamePlugin, GameVersion, new ITAlertMessageSerializationHandler());;
			_photonClient.ExceptionEvent += OnClientException;

			_sugarClient = new SUGARClient("http://api.sugarengine.org/");
			PlayerCommands.PhotonClient = _photonClient;

			_stateController = _stateControllerFactory.Create(_photonClient);
			_stateController.Initialize();
			_stateController.EnterState(LoadingState.StateName);

			_photonClient.Connect();
		}

		protected override void OnExit()
		{
			_stateController.Terminate();
			_photonClient.Dispose();
		}

		protected override void OnTick(float deltaTime)
		{
			try
			{
				_stateController.Tick(deltaTime);
			}
			catch (Exception exception)
			{
				Debug.LogError($"Caught Exception: {exception}");
				ExceptionEvent(exception);
			}
		}
		
		private void OnClientException(Exception exception)
		{
			ExceptionEvent(exception);
		}
	}
}