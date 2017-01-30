using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.GameStates.Room.Error
{
	public class ErrorStateInput : TickStateInput
	{
		public event Action BackClickedEvent;

		private GameObject _menuPanel;
		private ButtonList _buttons;
		private Button _backButton;

		protected override void OnInitialize()
		{
			// Main Menu
			_menuPanel = GameObject.Find("ErrorContainer").transform.GetChild(0).gameObject;
			_buttons = new ButtonList("ErrorContainer/ErrorPanelContainer/ButtonPanel");

			_backButton = _buttons.GetButton("BackButtonContainer");
			_backButton.onClick.AddListener(OnBackButtonClick);

		}

		private void OnBackButtonClick()
		{
			BackClickedEvent?.Invoke();
		}

		protected override void OnTerminate()
		{
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
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnBackButtonClick();
			}
		}
	}
}