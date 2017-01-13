using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.GameStates.Input
{
	public class MenuStateInput : TickStateInput
	{
		public event Action QuitClickedEvent;

		private readonly Client _photonClient;

		private GameObject _mainMenuPanel;
		private GameObject _createGamePopup;
		private ButtonList _buttons;

		public event Action JoinGameEvent;
		public event Action CreateGameClickedEvent;
		public event Action SettingsClickedEvent;
		public event Action JoinGameSuccessEvent;

		public MenuStateInput(Client photonClient)
		{
			_photonClient = photonClient;
		}

		protected override void OnInitialize()
		{
			// Main Menu
			_mainMenuPanel = GameObject.Find("MainMenuContainer").transform.GetChild(0).gameObject;
			_buttons = new ButtonList("MainMenuContainer/MenuPanelContainer/MenuContainer");

			var quitButton = _buttons.GetButton("QuitButtonContainer");
			quitButton.onClick.AddListener(OnQuitClick);

			var createGameButton = _buttons.GetButton("CreateGameButtonContainer");
			createGameButton.onClick.AddListener(OnCreateGameClick);

			var joinGameButton = _buttons.GetButton("JoinGameButtonContainer");
			joinGameButton.onClick.AddListener(OnJoinGameClick);

			var quickMatchButton = _buttons.GetButton("QuickMatchButtonContainer");
			quickMatchButton.onClick.AddListener(OnQuickMatchClick);

			var settingsButton = _buttons.GetButton("SettingsButtonContainer");
			settingsButton.onClick.AddListener(OnSettingsClick);
		}

		private void OnJoinGameSuccess(ClientRoom clientRoom)
		{
			JoinGameSuccessEvent();
		}

		private void OnJoinGameClick()
		{
			JoinGameEvent();
		}

		private void OnCreateGameClick()
		{
			CreateGameClickedEvent();
		}

		private void OnQuickMatchClick()
		{
			CommandQueue.AddCommand(new QuickGameCommand());
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
			_photonClient.JoinedRoomEvent += OnJoinGameSuccess;
			_mainMenuPanel.SetActive(true);
			_buttons.BestFit();
		}

		protected override void OnExit()
		{
			_photonClient.JoinedRoomEvent -= OnJoinGameSuccess;
			_mainMenuPanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				OnQuitClick();
			}
		}
	}
}