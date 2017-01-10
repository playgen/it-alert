using System;
using System.Collections.Generic;
using System.Reflection;
using GameWork.Core.States.Tick;
using UnityEngine;
using PlayGen.ITAlert.GameStates;

namespace Debugging
{
	public class GameStateDebugMenu : MonoBehaviour
	{
		private ControllerBehaviour _controllerBehaviour;
		private TickStateController<TickState> _stateController;

		private bool _isVisible = false;

		void Start()
		{
			_controllerBehaviour = GameObject.FindObjectOfType<ControllerBehaviour>();

			_stateController = (TickStateController<TickState>) GetField(_controllerBehaviour.GetType(),
				_controllerBehaviour,
				"_stateController");
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F2))
			{
				_isVisible = !_isVisible;
			}
		}

		private void OnGUI()
		{
			if (!_isVisible) return;

			GameStateButton(LoginState.StateName);
			GameStateButton(LoadingState.StateName);
			GameStateButton(MenuState.StateName);
			GameStateButton(LobbyState.StateName, MenuState.StateName);
			GameStateButton(GamesListState.StateName, MenuState.StateName);
			GameStateButton(CreateGameState.StateName, MenuState.StateName);
			GameStateButton(SettingsState.StateName, MenuState.StateName);
			GameStateButton(RoomState.StateName);
		}

		private object GetField(Type type, object instance, string fieldName)
		{
			var field = type.GetField(fieldName,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var value = field.GetValue(instance);

			return value;
		}

		private void GameStateButton(string stateName, string parentState = null)
		{
			if (GUILayout.Button(stateName))
			{
				if (parentState != null)
				{
					_stateController.ChangeState(parentState);
					var states = (Dictionary<string, TickState>)GetField(_stateController.GetType(),
					_stateController,
					"States");
					var currentState = states[_stateController.ActiveStateName];
					var subStateController = (TickStateController)GetField(currentState.GetType(),
					currentState,
					"_stateController");
					subStateController.ChangeState(stateName);
				}
				else
				{
					_stateController.ChangeState(stateName);
				}
			}
		}
	}
}