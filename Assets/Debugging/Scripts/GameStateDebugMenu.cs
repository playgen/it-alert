using System;
using System.Reflection;
using GameWork.States;
using UnityEngine;
using PlayGen.ITAlert.GameStates;

namespace Debugging
{
    public class GameStateDebugMenu : MonoBehaviour
    {
        private ControllerBehaviour _controllerBehaviour;
        private TickableStateController<TickableSequenceState> _stateController;

        void Start()
        {
            _controllerBehaviour = GameObject.FindObjectOfType<ControllerBehaviour>();

            _stateController = (TickableStateController<TickableSequenceState>) GetField(_controllerBehaviour.GetType(),
                _controllerBehaviour,
                "_stateController");
        }

        private void OnGUI()
        {
            GameStateButton(LoginState.StateName);
            GameStateButton(LoadingState.StateName);
            GameStateButton(MenuState.StateName);
            GameStateButton(LobbyState.stateName);
            GameStateButton(GamesListState.StateName);
            GameStateButton(CreateGameState.StateName);
            GameStateButton(SettingsState.StateName);
            GameStateButton(GameState.StateName);
        }

        private object GetField(Type type, object instance, string fieldName)
        {
            var field = type.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var value = field.GetValue(instance);

            return value;
        }

        private void GameStateButton(string stateName)
        {
            if (GUILayout.Button(stateName))
            {
                _stateController.ChangeState(stateName);
            }
        }
    }
}