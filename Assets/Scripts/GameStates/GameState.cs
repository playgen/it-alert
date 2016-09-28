using System;
using GameWork.States;
using PlayGen.ITAlert.GameStates.GameSubStates;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PlayGen.ITAlert.GameStates
{
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
            _stateController.SetState(InitializingState.StateName);

            
            
            SceneManager.LoadScene("Network");
            SceneManager.activeSceneChanged += SceneLoaded;
            Debug.Log(SceneManager.GetActiveScene().name);
            
        }

        private void SceneLoaded(Scene arg0, Scene scene)
        {
            Debug.Log(SceneManager.GetActiveScene().name);

            Director.Client = _client;

            _interface.Initialize();
            _interface.SetPlayerColors(_client.CurrentRoom.PlayerColors);
            _interface.PopulateChatPanel(_client.CurrentRoom.ListCurrentRoomPlayers);
        }

        public override void Exit()
        {
            _stateController.ExitState();
            _stateController.Terminate();

	        SceneManager.UnloadScene("Network");
        }

        public override void Tick(float deltaTime)
        {
            _voiceController.HandleVoiceInput();

            switch (_client.CurrentRoom.CurrentGame.State)
            {
                case Network.Client.States.GameStates.Initializing:
                    if (_stateController.CurrentStateName != InitializingState.StateName)
                    {
                        _stateController.ChangeState(InitializingState.StateName);
                    }
                    else
                    {
                        _stateController.Tick(deltaTime);
                    }
                    break;

                case Network.Client.States.GameStates.Playing:
                    if (_stateController.CurrentStateName != PlayingState.StateName)
                    {
                        _stateController.ChangeState(PlayingState.StateName);
                    }
                    else
                    {
                        _stateController.Tick(deltaTime);
                    }
                    break;

                case Network.Client.States.GameStates.Finalizing:


                    if (_stateController.CurrentStateName != FinalizingState.StateName)
                    {
                        _stateController.ChangeState(FinalizingState.StateName);
                    }
                    else
                    {
                        _stateController.Tick(deltaTime);
                    }
                    break;
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