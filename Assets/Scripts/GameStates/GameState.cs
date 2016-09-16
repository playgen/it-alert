using System;
using GameWork.States;
using PlayGen.ITAlert.GameStates.GameSubStates;
using PlayGen.ITAlert.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.ITAlert.GameStates
{
    public class GameState : TickableSequenceState
    {
        public const string StateName = "GameState";

        private readonly TickableStateController _stateController;
        private readonly ITAlertClient _networkClient;
        private readonly VoiceController _voiceController;

        public override string Name
        {
            get { return StateName; }
        }

        public GameState(ITAlertClient networkClient, VoiceController voiceController)
        {
            _networkClient = networkClient;

            _stateController = new TickableStateController(new InitializingState(_networkClient),
                new PlayingState(_networkClient),
                new FinalizingState(_networkClient));

            _voiceController = voiceController;
        }

        public override void Enter()
        {
            _stateController.Initialize();
            _stateController.SetState(InitializingState.StateName);

            Director.Client = _networkClient;
            SceneManager.LoadScene("Network");
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

            switch (_networkClient.GameState)
            {
                case Network.GameStates.Initializing:
                    if (_stateController.CurrentStateName != InitializingState.StateName)
                    {
                        _stateController.ChangeState(InitializingState.StateName);
                    }
                    else
                    {
                        _stateController.Tick(deltaTime);
                    }
                    break;

                case Network.GameStates.Playing:
                    if (_stateController.CurrentStateName != PlayingState.StateName)
                    {
                        _stateController.ChangeState(PlayingState.StateName);
                    }
                    else
                    {
                        _stateController.Tick(deltaTime);
                    }
                    break;

                case Network.GameStates.Finalizing:


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