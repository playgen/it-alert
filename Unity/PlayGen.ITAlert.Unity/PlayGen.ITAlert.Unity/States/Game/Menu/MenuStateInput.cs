using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.States.Game.Menu
{
	public class MenuStateInput : TickStateInput
	{
		public event Action QuitClickedEvent;

		private readonly Client _photonClient;

		private GameObject _mainMenuPanel;
		private ButtonList _buttons;
		private Button _quitButton;
		private Button _settingsButton;
		private Button _quickMatchButton;
		private Button _joinGameButton;
		private Button _createGameButton;
		private Text _serverConnectionText;
		private Text _sugarConnectionText;

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

			_quitButton = _buttons.GetButton("QuitButtonContainer");
			_createGameButton = _buttons.GetButton("CreateGameButtonContainer");
			_joinGameButton = _buttons.GetButton("JoinGameButtonContainer");
			_quickMatchButton = _buttons.GetButton("QuickMatchButtonContainer");
			_settingsButton = _buttons.GetButton("SettingsButtonContainer");
			_serverConnectionText = _mainMenuPanel.transform.Find("MenuStatusContainer/ServerConnection").GetComponent<Text>();
			_sugarConnectionText = _mainMenuPanel.transform.Find("MenuStatusContainer/SUGARConnection").GetComponent<Text>();

			_quitButton.onClick.AddListener(OnQuitClick);
			_createGameButton.onClick.AddListener(OnCreateGameClick);
			_joinGameButton.onClick.AddListener(OnJoinGameClick);
			_quickMatchButton.onClick.AddListener(OnQuickMatchClick);
			_settingsButton.onClick.AddListener(OnSettingsClick);
		}

		protected override void OnTerminate()
		{
			_quitButton.onClick.RemoveListener(OnQuitClick);
			_createGameButton.onClick.RemoveListener(OnCreateGameClick);
			_joinGameButton.onClick.RemoveListener(OnJoinGameClick);
			_quickMatchButton.onClick.RemoveListener(OnQuickMatchClick);
			_settingsButton.onClick.RemoveListener(OnSettingsClick);
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
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnQuitClick();
			}
			_quickMatchButton.interactable = _photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected;
			_joinGameButton.interactable = _photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected;
			_createGameButton.interactable = _photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected;
			_serverConnectionText.text = "Game Server: " + (_photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected ? "Connected" : "Not Connected");
			_serverConnectionText.color = _photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected ? Color.green : Color.red;
			if (SUGARManager.CurrentUser != null)
			{
				_sugarConnectionText.text = "SUGAR User: " + SUGARManager.CurrentUser.Name;
				_sugarConnectionText.color = Color.green;
			}
			else
			{
				_sugarConnectionText.text = "SUGAR: Not Signed In";
				_sugarConnectionText.color = Color.red;
			}
		}
	}
}