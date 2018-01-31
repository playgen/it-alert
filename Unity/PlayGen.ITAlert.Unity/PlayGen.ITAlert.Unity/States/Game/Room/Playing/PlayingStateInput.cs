using System;
using System.Collections.Generic;
using System.Linq;

using Engine.Lifecycle;

using GameWork.Core.States.Tick.Input;

using PlayGen.ITAlert.Photon.Common;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.SUGAR.Unity;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Playing
{
	public class PlayingStateInput : TickStateInput
	{
		public event Action PauseClickedEvent;
		public event Action EndGameContinueClickedEvent;
		public event Action EndGameOnePlayerContinueClickedEvent;

		private List<GameObject> _gameContainers;
		private Button _continueButton;

		private readonly ITAlertPhotonClient _photonClient;
		private readonly Director _director;
		private bool _endGame;

		public PlayingStateInput(ITAlertPhotonClient photonClient, Director director)
		{
			_photonClient = photonClient;
			_director = director;
		}

		protected override void OnInitialize()
		{
			_gameContainers = GameObjectUtilities.FindGameObject("Game").GetComponentsInChildren<Canvas>(true).Select(c => c.gameObject).ToList();
			_continueButton = GameObjectUtilities.FindGameObject("Game/End Canvas/End Screen/ContinueButtonContainer").GetComponent<Button>();
			_director.GameEnded += OnEndGame;
		}

		private void OnPauseClicked()
		{
			PauseClickedEvent?.Invoke();
		}

		private void OnContinueClick()
		{
			_endGame = false;
			var gameContainer = GameObjectUtilities.FindGameObject("Game/Canvas");
			gameContainer.SetActive(false);
			var canvasGroup = gameContainer.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 1;
			canvasGroup.blocksRaycasts = true;
			foreach (var trail in gameContainer.GetComponentsInChildren<TrailRenderer>())
			{
				trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 1);
				trail.endColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.875f);
			}
			if (_director.Players.Count > 1)
			{
				EndGameContinueClickedEvent?.Invoke();
			}
			else
			{
				EndGameOnePlayerContinueClickedEvent?.Invoke();
				if (_photonClient.CurrentRoom.RoomInfo.customProperties[CustomRoomSettingKeys.GameScenario].ToString() == "SPL3")
				{
					// The following string contains the key for the google form is used for the cognitive load questionnaire
					var formsKey = "1FAIpQLSctM-kR-1hlmF6Nk-pQNIWYnFGxRAVvyP6o3ZV0kr8K7JD5dQ";

					// Google form ID
					var googleFormsURL = "https://docs.google.com/forms/d/e/"
											+ formsKey
											+ "/viewform?entry.1596836094="
											+ SUGARManager.CurrentUser.Name;
					// Open the default browser and show the form
					Application.OpenURL(googleFormsURL);
					Application.Quit();
				}
			}
		}

		protected override void OnEnter()
		{
			_gameContainers.ForEach(g => g.SetActive(true));
			_continueButton.onClick.AddListener(OnContinueClick);
			PlayGen.Unity.Utilities.Loading.Loading.Stop();
		}

		protected override void OnExit()
		{
			_gameContainers.ForEach(g => g.SetActive(false));
			_continueButton.onClick.RemoveListener(OnContinueClick);
		}

		private void OnEndGame(EndGameState obj)
		{
			_endGame = true;
		}

		protected override void OnTick(float deltaTime)
		{
			if (!_endGame && Input.GetKeyDown(KeyCode.Escape))
			{
				OnPauseClicked();
			}
		}
	}
}