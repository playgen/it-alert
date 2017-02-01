using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;

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

		protected override void OnInitialize()
		{
			// Main Menu
			_menuPanel = GameObject.Find("PauseContainer").transform.GetChild(0).gameObject;
			_buttons = new ButtonList("PauseContainer/PausePanelContainer/PauseContainer");

			_continueButton = _buttons.GetButton("ContinueButtonContainer");
			_settingsButton = _buttons.GetButton("SettingsButtonContainer");
			_quitButton = _buttons.GetButton("QuitButtonContainer");

			_continueButton.onClick.AddListener(OnContinueClick);
			_settingsButton.onClick.AddListener(OnSettingsClick);
			_quitButton.onClick.AddListener(OnQuitClick);
		}

		protected override void OnTerminate()
		{
			_continueButton.onClick.RemoveListener(OnContinueClick);
			_settingsButton.onClick.RemoveListener(OnSettingsClick);
			_quitButton.onClick.RemoveListener(OnQuitClick);
		}

		private void OnContinueClick()
		{
			ContinueClickedEvent();
		}

		private void OnSettingsClick()
		{
			SettingsClickedEvent();
		}

		private void OnQuitClick()
		{
			QuitClickedEvent();
		}

		protected override void OnEnter()
		{
			_menuPanel.SetActive(true);
			_buttons.BestFit();
		}

		protected override void OnExit()
		{
			_menuPanel.SetActive(false);
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