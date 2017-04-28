using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Playing
{
	public class PlayingStateInput : TickStateInput
	{
		public event Action PauseClickedEvent;
		public event Action EndGameContinueClickedEvent;
		public event Action EndGameOnePlayerContinueClickedEvent;

		private GameObject _gameContainer;
		private Button _continueButton;

		private readonly Client _photonClient;
		private readonly Director _director;

		public PlayingStateInput(Client photonClient, Director director)
		{
			_photonClient = photonClient;
			_director = director;
		}

		protected override void OnInitialize()
		{
			_gameContainer = GameObjectUtilities.FindGameObject("Game/Canvas");
			_continueButton = GameObjectUtilities.FindGameObject("Game/Canvas/End Screen/ContinueButtonContainer").GetComponent<Button>();
		}

		private void OnPauseClicked()
		{
			PauseClickedEvent?.Invoke();
		}

		private void OnContinueClick()
		{
			if (_director.Players.Count > 1)
			{
				EndGameContinueClickedEvent?.Invoke();
			}
			else
			{
				EndGameOnePlayerContinueClickedEvent?.Invoke();
			}
		}

		protected override void OnEnter()
		{
			_gameContainer.SetActive(true);
			_continueButton.onClick.AddListener(OnContinueClick);
			PlayGen.Unity.Utilities.Loading.Loading.Stop();
		}

		protected override void OnExit()
		{
			_gameContainer.SetActive(false);
			_continueButton.onClick.RemoveListener(OnContinueClick);
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