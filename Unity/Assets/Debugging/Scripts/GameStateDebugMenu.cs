using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Behaviours;
using UnityEngine;
using PlayGen.ITAlert.Unity.GameStates.Game;
using PlayGen.ITAlert.Unity.GameStates.Game.Loading;
using PlayGen.ITAlert.Unity.GameStates.Game.Menu;
using PlayGen.ITAlert.Unity.GameStates.Game.Menu.CreateGame;
using PlayGen.ITAlert.Unity.GameStates.Game.Menu.GamesList;
using PlayGen.ITAlert.Unity.GameStates.Game.Room;
using PlayGen.ITAlert.Unity.GameStates.Game.Room.Lobby;
using PlayGen.ITAlert.Unity.GameStates.Game.Settings;

namespace Debugging
{
	public class GameStateDebugMenu : MonoBehaviour
	{
		private TickStateController _gameStateController;
		
		private bool _isVisible = false;

		private TickStateController GameStateController
		{
			get { return _gameStateController ?? (_gameStateController = GetGameStateController()); }
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
					CallChangeStateInternal(GameStateController, parentState);
					var states = (Dictionary<string, TickState>)GetField(GameStateController.GetType(),
					GameStateController,
					"States");
					var currentState = states[GameStateController.ActiveStateName];
					var subStateController = (TickStateController)GetField(currentState.GetType(),
					currentState,
					"_stateController");
					CallChangeStateInternal(subStateController, stateName);
				}
				else
				{
					CallChangeStateInternal(GameStateController, stateName);
				}
			}
		}

		private TickStateController GetGameStateController()
		{
			var stateBehaviour = FindObjectOfType<StateBehaviour>();

			var stateController = (TickStateController)GetField(stateBehaviour.GetType(), stateBehaviour, "_stateController");

			var states = (Dictionary<string, TickState>)GetField(stateController.GetType(),
				   stateController,
				   "States");

			var gameState = states[GameState.StateName];

			var gameStateController = (TickStateController)GetField(gameState.GetType(),
				gameState,
				"_stateController");

			return gameStateController;
		}
	}
}
