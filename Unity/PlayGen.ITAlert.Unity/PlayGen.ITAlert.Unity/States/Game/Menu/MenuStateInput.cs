using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Extensions;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.ITAlert.Unity.States.Game.Menu
{
	public class MenuStateInput : TickStateInput
	{
		public event Action QuitClickedEvent;
		public event Action JoinGameEvent;
		public event Action CreateGameClickedEvent;
		public event Action SettingsClickedEvent;
		public event Action JoinGameSuccessEvent;

		private readonly ITAlertPhotonClient _photonClient;
		private readonly ScenarioController _controller;

		private GameObject _mainMenuPanel;
		private ButtonList _buttons;
		private Button _quitButton;
		private Button _settingsButton;
		private Button _quickMatchButton;
		private Button _joinGameButton;
		private Button _createGameButton;
		private Text _serverConnectionText;
		private Text _sugarConnectionText;

		private Color _red;
		private Color _green;

		public MenuStateInput(ITAlertPhotonClient photonClient, ScenarioController controller)
		{
			_photonClient = photonClient;
			_controller = controller;
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

			_serverConnectionText = _mainMenuPanel.transform.FindText("MenuStatusContainer/ServerConnection");
			_sugarConnectionText = _mainMenuPanel.transform.FindText("MenuStatusContainer/SUGARConnection");

			_quitButton.onClick.AddListener(OnQuitClick);
			_createGameButton.onClick.AddListener(OnCreateGameClick);
			_joinGameButton.onClick.AddListener(OnJoinGameClick);
			_quickMatchButton.onClick.AddListener(OnQuickMatchClick);
			_settingsButton.onClick.AddListener(OnSettingsClick);

			ColorUtility.TryParseHtmlString("#E32730", out _red);
			ColorUtility.TryParseHtmlString("#89C845", out _green);
		}

		protected override void OnTerminate()
		{
			_quitButton.onClick.RemoveListener(OnQuitClick);
			_createGameButton.onClick.RemoveListener(OnCreateGameClick);
			_joinGameButton.onClick.RemoveListener(OnJoinGameClick);
			_quickMatchButton.onClick.RemoveListener(OnQuickMatchClick);
			_settingsButton.onClick.RemoveListener(OnSettingsClick);
		}

		private void OnJoinGameSuccess(ClientRoom<ITAlertPlayer> clientRoom)
		{
			JoinGameSuccessEvent?.Invoke();
		}

		private void OnJoinGameClick()
		{
			JoinGameEvent?.Invoke();
			PlayGen.Unity.Utilities.Loading.Loading.Start();
		}

		private void OnCreateGameClick()
		{
			_controller.SetQuickMatch(false);
			CreateGameClickedEvent?.Invoke();
		}

		private void OnQuickMatchClick()
		{
			_controller.SetQuickMatch(true);
			CreateGameClickedEvent?.Invoke();
		}

		private void OnSettingsClick()
		{
			SettingsClickedEvent?.Invoke();
		}

		private void OnQuitClick()
		{
			QuitClickedEvent?.Invoke();
		}

		protected override void OnEnter()
		{
		    OnTick(0f);
            _photonClient.JoinedRoomEvent += OnJoinGameSuccess;
			_mainMenuPanel.SetActive(true);
			_buttons.Buttons.BestFit();
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
				//OnQuitClick();
			}
			_quickMatchButton.interactable = _photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected;
			_joinGameButton.interactable = _photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected;
			_createGameButton.interactable = _photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected;
			_serverConnectionText.text = Localization.Get("CONNECTION_LABEL") + " " + Localization.Get((_photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected ? "CONNECTION_LABEL_CONNECTED" : "CONNECTION_LABEL_NOT_CONNECTED"));
			_serverConnectionText.color = _photonClient.ClientState == PlayGen.Photon.Unity.Client.ClientState.Connected ? _green : _red;
			if (SUGARManager.CurrentUser != null)
			{
				_sugarConnectionText.text = SUGARManager.CurrentUser.Name;
				_sugarConnectionText.color = _green;
			}
			else
			{
				_sugarConnectionText.text = Localization.Get("SUGAR_LABEL_NOT_SIGNED_IN");
				_sugarConnectionText.color = _red;
			}
		}
	}
}