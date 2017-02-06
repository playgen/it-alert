﻿using System;
using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Photon.Messaging;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.States.Game.Loading;
using PlayGen.Photon.Unity.Client;
using PlayGen.SUGAR.Client;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.States.Game
{
	public class GameState : TickState
	{
		public const string StateName = nameof(GameState);
		private const string GamePlugin = "RoomControllerPlugin";
		private const string GameVersion = "1";

		private readonly GameStateControllerFactory _stateControllerFactory;
		private readonly GameErrorContainer _gameErrorContainer;

		private Client _photonClient;
		private TickStateController _stateController;

		public override string Name => StateName;

		public event Action<Exception> ExceptionEvent;

		public GameState(GameErrorContainer gameErrorContainer)
		{
			_gameErrorContainer = gameErrorContainer;
			_stateControllerFactory = new GameStateControllerFactory();
		}

		public void SetSubstateParentController(StateControllerBase parentStateController)
		{
			_stateControllerFactory.ParentStateController = parentStateController;
		}

		protected override void OnEnter()
		{
			_photonClient = new Client(GamePlugin, GameVersion, new ITAlertMessageSerializationHandler()); ;
			_photonClient.ExceptionEvent += OnClientException;

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
				if (_photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Disconnected)
				{
					_photonClient.Connect();
				}

				_stateController.Tick(deltaTime);
			}
			catch (Exception exception)
			{
				Debug.LogError(exception);
				OnException(exception);
			}
		}

		private void OnClientException(Exception exception)
		{
			OnException(exception);
		}

		private void OnException(Exception exception)
		{
			_gameErrorContainer.Exception = exception;
			ExceptionEvent(exception);
		}

	}
}