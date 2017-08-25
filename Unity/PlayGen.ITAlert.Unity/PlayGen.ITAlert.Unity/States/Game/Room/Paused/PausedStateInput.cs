using System;
using System.Collections.Generic;

using GameWork.Core.States.Tick.Input;

using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;
using Engine.Lifecycle;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Paused
{
	public class PausedStateInput : TickStateInput
	{
		public event Action ContinueClickedEvent;
		public event Action SettingsClickedEvent;
		public event Action QuitClickedEvent;

		private GameObject _menuPanel;
		private ButtonList _buttons;
		private Button _quitButton;
		private Button _continueButton;
		private Button _settingsButton;

		private GameObject _gameContainer;
		private CanvasGroup _canvasGroup;
		private Dictionary<BlinkBehaviour, bool> _blinkPauseToggle = new Dictionary<BlinkBehaviour, bool>();

		private readonly Director _director;
		private bool _endGame;

		public PausedStateInput(Director director)
		{
			_director = director;
		}

		protected override void OnInitialize()
		{
			// Main Menu
			_menuPanel = GameObject.Find("PauseContainer").transform.GetChild(0).gameObject;
			_buttons = new ButtonList("PauseContainer/PausePanelContainer/PauseContainer");

			_continueButton = _buttons.GetButton("ContinueButtonContainer");
			_settingsButton = _buttons.GetButton("SettingsButtonContainer");
			_quitButton = _buttons.GetButton("QuitButtonContainer");

			_gameContainer = GameObjectUtilities.FindGameObject("Game/Canvas");
			_canvasGroup = _gameContainer.GetComponent<CanvasGroup>();
			_director.GameEnded += OnEndGame;
		}

		private void OnContinueClick()
		{
			if (!_endGame)
			{
				GameVisible();
			}
			_endGame = false;
			ContinueClickedEvent?.Invoke();
		}

		private void OnSettingsClick()
		{
			SettingsClickedEvent?.Invoke();
		}

		private void OnQuitClick()
		{
			GameVisible();
			_endGame = false;
			QuitClickedEvent?.Invoke();
		}

		private void OnEndGame(EndGameState obj)
		{
			_endGame = true;
		}

		protected override void OnEnter()
		{
			_continueButton.onClick.AddListener(OnContinueClick);
			_settingsButton.onClick.AddListener(OnSettingsClick);
			_quitButton.onClick.AddListener(OnQuitClick);
			GameObjectUtilities.Find("Game/Canvas").GetComponent<PlayerInputHandler>().ClearClicks();

			_menuPanel.SetActive(true);
			_buttons.Buttons.BestFit();

			_gameContainer.SetActive(true);
			_canvasGroup.alpha = 0.1f;
			_canvasGroup.blocksRaycasts = false;
			foreach (var trail in _gameContainer.GetComponentsInChildren<TrailRenderer>())
			{
				trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.25f);
				trail.endColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.125f);
			}
			foreach (var blink in _gameContainer.GetComponentsInChildren<BlinkBehaviour>())
			{
				var image = blink.GetComponent<Image>();
				if (image != null)
				{
                    if (!_blinkPauseToggle.ContainsKey(blink))
                    {
                        _blinkPauseToggle.Add(blink, blink.enabled);
                        blink.enabled = false;
                        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.625f);
                    }
				}
			}
		}

		protected override void OnExit()
		{
			_continueButton.onClick.RemoveListener(OnContinueClick);
			_settingsButton.onClick.RemoveListener(OnSettingsClick);
			_quitButton.onClick.RemoveListener(OnQuitClick);
			_menuPanel.SetActive(false);
		}

		private void GameVisible()
		{
			_gameContainer.SetActive(false);
			_canvasGroup.alpha = 1;
			_canvasGroup.blocksRaycasts = true;
			foreach (var trail in _gameContainer.GetComponentsInChildren<TrailRenderer>())
			{
				trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 1);
				trail.endColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.875f);
			}
			foreach (var blink in _blinkPauseToggle.Keys)
			{
				blink.enabled = _blinkPauseToggle[blink];
			}
		    _blinkPauseToggle.Clear();
        }

		protected override void OnTick(float deltaTime)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				OnContinueClick();
			}
		}
	}
}