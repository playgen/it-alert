using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Playing
{
	public class PlayingStateInput : TickStateInput
	{
		public event Action PauseClickedEvent;

		private GameObject _gamePanel;
		private GameObject _gameContainer;

		protected override void OnInitialize()
		{
			_gamePanel = GameObjectUtilities.FindGameObject("Game/GameCanvas/GameContainer");
			_gameContainer = GameObjectUtilities.FindGameObject("Game/Graph");
		}

		private void OnPauseClicked()
		{
			PauseClickedEvent();
		}

		protected override void OnEnter()
		{
			_gamePanel.SetActive(true);
			_gameContainer.SetActive(true);
			LoadingUtility.HideSpinner();
		}

		protected override void OnExit()
		{
			_gamePanel.SetActive(false);
			_gameContainer.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				OnPauseClicked();
			}
		}
	}
}