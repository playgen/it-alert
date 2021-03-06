﻿using System;
using GameWork.Core.Logging.Loggers;
using GameWork.Core.States;
using GameWork.Core.States.Tick;

using PlayGen.ITAlert.Unity.Debug;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Photon.Messaging;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.States.Game.Loading;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.ITAlert.Unity.States.Game
{
	public class GameState : TickState
	{
		public const string StateName = nameof(GameState);
		private const string GamePlugin = "RoomControllerPlugin";
		private const string GameVersion = "1";

		private readonly GameStateControllerFactory _stateControllerFactory;

		private ITAlertPhotonClient _photonClient;
		private TickStateController _stateController;

		public override string Name => StateName;

		public event Action<Exception> Exception;
		public event Action Disconnected;

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
			_photonClient?.Dispose();
			_photonClient = new ITAlertPhotonClient(GamePlugin, GameVersion, new ITAlertMessageSerializationHandler());
			_photonClient.ExceptionEvent += OnClientException;
			_photonClient.ConnectedEvent += OnConnected;

			PlayerCommands.PhotonClient = _photonClient;
			DebugCommands.PhotonClient = _photonClient;

			_stateController = _stateControllerFactory.Create(_photonClient);
			_stateController.Initialize();

			if (!GameExceptionHandler.HasException)
			{
				_stateController.EnterState(LoadingState.StateName);
				_photonClient.Connect();
			}
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
				if (_photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Disconnected)
				{
					_photonClient.Connect();
				}

				_stateController.Tick(deltaTime);
			}
			catch (Exception exception)
			{
				LogProxy.Error(exception.ToString());
			    LogProxy.Debug(exception.StackTrace);
                OnException(exception);
			}
		}

		private void OnConnected()
		{
			_photonClient.DisconnectedEvent += OnDisconnected;
		}

		private void OnDisconnected()
		{
			_photonClient.DisconnectedEvent -= OnDisconnected;
			Disconnected?.Invoke();
		}

		private void OnClientException(Exception exception)
		{
			OnException(exception);
		}

		private void OnException(Exception exception)
		{
			Exception?.Invoke(exception);
		}
	}
}