using System;
using System.Linq;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

using PlayGen.ITAlert.Network.Client;
using PlayGen.SUGAR.Unity;

using UnityEngine;

public class MenuStateInterface : StateInterface
{
	private GameObject _mainMenuPanel;
	private GameObject _createGamePopup;

	public override void Initialize()
	{
		// Main Menu
		_mainMenuPanel = GameObject.Find("MainMenuContainer").transform.GetChild(0).gameObject;
		var menu = new ButtonList("MainMenuContainer/MenuPanelContainer/MenuContainer", true);

		var quitButton = menu.GetButton("QuitButtonContainer");
		quitButton.onClick.AddListener(OnQuitClick);

		var createGameButton = menu.GetButton("CreateGameButtonContainer");
		createGameButton.onClick.AddListener(OnCreateGameClick);

		var joinGameButton = menu.GetButton("JoinGameButtonContainer");
		joinGameButton.onClick.AddListener(OnJoinGameClick);

		var quickMatchButton = menu.GetButton("QuickMatchButtonContainer");
		quickMatchButton.onClick.AddListener(OnQuickMatchClick);

		var settingsButton = menu.GetButton("SettingsButtonContainer");
		settingsButton.onClick.AddListener(OnSettingsClick);
	}

	private void OnJoinGameClick()
	{
		EnqueueCommand(new ChangeStateCommand(GamesListState.StateName));
	}

	private void OnCreateGameClick()
	{
		EnqueueCommand(new ChangeStateCommand(CreateGameState.StateName));
	}

	private void OnQuickMatchClick()
	{
		EnqueueCommand(new QuickGameCommand());
	}

	private void OnSettingsClick()
	{
		EnqueueCommand(new ChangeStateCommand(SettingsState.StateName));
	}

	private void OnQuitClick()
	{
		Application.Quit();
	}


	public override void Enter()
	{
		_mainMenuPanel.SetActive(true);
	}

	public override void Exit()
	{
		_mainMenuPanel.SetActive(false);
	}

	public void OnJoinGameSuccess(ClientRoom clientRoom)
	{
		EnqueueCommand(new NextStateCommand());
	}
}