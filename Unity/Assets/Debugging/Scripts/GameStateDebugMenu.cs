using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Behaviours;
using UnityEngine;
using PlayGen.ITAlert.Unity.GameStates;
using PlayGen.ITAlert.Unity.GameStates.Loading;
using PlayGen.ITAlert.Unity.GameStates.Menu;
using PlayGen.ITAlert.Unity.GameStates.Menu.CreateGame;
using PlayGen.ITAlert.Unity.GameStates.Menu.GamesList;
using PlayGen.ITAlert.Unity.GameStates.Room;
using PlayGen.ITAlert.Unity.GameStates.Room.Lobby;

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

			_stateController = (TickStateController<TickState>)GetField(_controllerBehaviour.GetType(), _controllerBehaviour, "_stateController");
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
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var value = field.GetValue(instance);
			return value;
		}

		#region Temporary workaround

		private void CallChangeStateInternal(object stateController, string stateName)
		{
			var changeState = stateController.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).SingleOrDefault(mi => mi.Name.Equals("ChangeState"));
			if (changeState != null)
			{
				changeState.Invoke(stateController, new object[] { stateName });
			}
		}

		#endregion

		private void GameStateButton(string stateName, string parentState = null)
		{
			if (GUILayout.Button(stateName))
			{
				if (parentState != null)
				{
					CallChangeStateInternal(_stateController, parentState);
					var states = (Dictionary<string, TickState>)GetField(_stateController.GetType(),
					_stateController,
					"States");
					var currentState = states[_stateController.ActiveStateName];
					var subStateController = (TickStateController)GetField(currentState.GetType(),
					currentState,
					"_stateController");
					CallChangeStateInternal(subStateController, stateName);
				}
				else
				{
					CallChangeStateInternal(_stateController, stateName);
				}
			}
		}
	}
}
