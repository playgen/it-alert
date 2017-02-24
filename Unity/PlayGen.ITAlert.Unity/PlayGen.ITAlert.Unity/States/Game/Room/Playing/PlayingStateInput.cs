using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Playing
{
	public class PlayingStateInput : TickStateInput
	{
		public event Action PauseClickedEvent;

		private GameObject _gameContainer;

		protected override void OnInitialize()
		{
			_gameContainer = GameObjectUtilities.FindGameObject("Game/Graph");
		}

		private void OnPauseClicked()
		{
			PauseClickedEvent?.Invoke();
		}

		protected override void OnEnter()
		{
			_gameContainer.SetActive(true);
			LoadingUtility.HideSpinner();
		}

		protected override void OnExit()
		{
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