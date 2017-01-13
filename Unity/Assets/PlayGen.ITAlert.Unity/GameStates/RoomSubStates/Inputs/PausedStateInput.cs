using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.GameStates.RoomSubStates.Input
{
	public class PausedStateInput : TickStateInput
	{
		public event Action ContinueClickedEvent;
		public event Action SettingsClickedEvent;
		public event Action QuitClickedEvent;

		private GameObject _menuPanel;
		private ButtonList _buttons;

		protected override void OnInitialize()
		{
			// Main Menu
			_menuPanel = GameObject.Find("PauseContainer").transform.GetChild(0).gameObject;
			_buttons = new ButtonList("PauseContainer/PausePanelContainer/PauseContainer");

			var continueButton = _buttons.GetButton("ContinueButtonContainer");
			continueButton.onClick.AddListener(OnContinueClick);

			var settingsButton = _buttons.GetButton("SettingsButtonContainer");
			settingsButton.onClick.AddListener(OnSettingsClick);

			var quitButton = _buttons.GetButton("QuitButtonContainer");
			quitButton.onClick.AddListener(OnQuitClick);
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