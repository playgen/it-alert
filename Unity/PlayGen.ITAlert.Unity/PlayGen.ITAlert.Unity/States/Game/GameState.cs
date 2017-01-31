using System;
using System.Collections;
using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Behaviours;
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
		private PlayGen.Photon.Unity.Client.ClientState _lastState;
		private GameObject _gameStateGameObject;

		public override string Name => StateName;

		public event Action<Exception> ExceptionEvent;

		private GameObject GameStateGameObject => _gameStateGameObject ?? (_gameStateGameObject = new GameObject("GameState", typeof(DontDestroyOnLoad)));
	
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
			_photonClient = CreateClient();
			_sugarClient = new SUGARClient("http://api.sugarengine.org/");
			PlayerCommands.PhotonClient = _photonClient;

			_stateController = _stateControllerFactory.Create(_photonClient);
			_stateController.Initialize(LoadingState.StateName);
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

		private Client CreateClient()
		{
			var clientBase = GameStateGameObject.AddComponent<PhotonClientWrapper>();
			clientBase.Initialize(GameVersion, GamePlugin);

			var client = new Client(clientBase, new ITAlertMessageSerializationHandler());
			client.ExceptionEvent += OnClientException;

			clientBase.StartCoroutine(ClientLoop(client));

			return client;
		}

		private void OnClientException(Exception exception)
		{
			ExceptionEvent(exception);
		}

		private IEnumerator ClientLoop(Client photonClient)
		{
			while (true)
			{
				if (photonClient.ClientState != _lastState)
				{
					Debug.Log("Photon State Changed to: " + photonClient.ClientState);
					_lastState = photonClient.ClientState;
				}

				switch (photonClient.ClientState)
				{
					case PlayGen.Photon.Unity.Client.ClientState.Disconnected:
						photonClient.Connect();
						yield return new WaitForSeconds(0.1f);
						break;

					case PlayGen.Photon.Unity.Client.ClientState.Connecting:
						yield return new WaitForSeconds(0.5f);
						break;

					case PlayGen.Photon.Unity.Client.ClientState.Connected:
						yield return new WaitForSeconds(1.0f);
						break;
				}

				yield return new WaitForSeconds(1.0f);
			}
		}
	}
}
