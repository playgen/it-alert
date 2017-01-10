using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Network.Client;
using PlayGen.Photon.Unity;

using UnityEngine;

public class MenuStateInput : TickStateInput
{
	private readonly Client _client;

	private GameObject _mainMenuPanel;
	private GameObject _createGamePopup;
	private ButtonList _buttons;
	
	public event Action JoinGameEvent;
	public event Action CreateGameClickedEvent;
	public event Action SettingsClickedEvent;
	public event Action JoinGameSuccessEvent;

	public MenuStateInput(Client client)
	{
		_client = client;
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
		// todo this should trigger a Quit state that any save/cleanup can be done in before actually quitting the application
		// note: Calll the top most StateController's Terminate OnApplicationQuit
		Application.Quit();
	}
	
	protected override void OnEnter()
	{
		_client.JoinedRoomEvent += OnJoinGameSuccess;
		_mainMenuPanel.SetActive(true);
		_buttons.BestFit();
	}

	protected override void OnExit()
	{
		_client.JoinedRoomEvent -= OnJoinGameSuccess;
		_mainMenuPanel.SetActive(false);
	}

	protected override void OnTick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnQuitClick();
		}
	}
}