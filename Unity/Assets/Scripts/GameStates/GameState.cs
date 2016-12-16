using System;
using GameWork.Core.States;
using GameWork.Core.States.Controllers;
using PlayGen.ITAlert.GameStates.GameSubStates;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PlayGen.ITAlert.GameStates
{
	using System.Linq;

	public class GameState : TickableSequenceState
	{
		public const string StateName = "GameState";

		private readonly TickableStateController _stateController;
		private readonly Client _client;
		private readonly VoiceController _voiceController;
		private readonly GameStateInterface _interface;
		private readonly LobbyController _lobbyController;

		public override string Name
		{
			get { return StateName; }
		}

		public GameState(Client client, GameStateInterface @interface, LobbyController lobbyController, VoiceController voiceController)
		{
			_client = client;
			_interface = @interface;
			_stateController = new TickableStateController(new InitializingState(_client),
				new PlayingState(_client),
				new FinalizingState(_client));

			_lobbyController = lobbyController;
			_voiceController = voiceController;
		}

		public override void Enter()
		{
			_stateController.Initialize();
			_stateController.ChangeState(InitializingState.StateName);

			
			
			SceneManager.LoadScene("Network", LoadSceneMode.Additive);
			SceneManager.activeSceneChanged += SceneLoaded;
			Debug.Log(SceneManager.GetActiveScene().name);
			
		}

		private void SceneLoaded(Scene arg0, Scene scene)
		{
			Debug.Log(SceneManager.GetActiveScene().name);

			Director.Client = _client;

			_interface.Initialize();
			if (_client != null && _client.CurrentRoom != null)
			{
				_interface.SetPlayerColors(_client.CurrentRoom.Players);
				_interface.PopulateChatPanel(_client.CurrentRoom.ListCurrentRoomPlayers);
			}
		}

		public override void Exit()
		{
			_stateController.Terminate();

			SceneManager.UnloadScene("Network");
		}

		public override void Tick(float deltaTime)
		{
			_voiceController.HandleVoiceInput();

			if (_client != null && _client.CurrentRoom != null)
			{
				switch (_client.CurrentRoom.CurrentGame.State)
				{
					case Assets.Scripts.Network.Client.GameStates.Initializing:
						if (_stateController.ActiveState != InitializingState.StateName)
						{
							_stateController.ChangeState(InitializingState.StateName);
						}
						else
						{
							_stateController.Tick(deltaTime);
						}
						break;

					case Assets.Scripts.Network.Client.GameStates.Playing:
						if (_stateController.ActiveState != PlayingState.StateName)
						{
							_stateController.ChangeState(PlayingState.StateName);
						}
						else
						{
							_stateController.Tick(deltaTime);
						}
						break;

					case Assets.Scripts.Network.Client.GameStates.Finalizing:


						if (_stateController.ActiveState != FinalizingState.StateName)
						{
							_stateController.ChangeState(FinalizingState.StateName);
						}
						else
						{
							_stateController.Tick(deltaTime);
						}
						break;
				}

			}
			_interface.UpdateChatPanel();
		}

		public override void NextState()
		{
			ChangeState(LobbyState.StateName);
		}

		public override void PreviousState()
		{
			ChangeState(LobbyState.StateName);
		}
	}
}