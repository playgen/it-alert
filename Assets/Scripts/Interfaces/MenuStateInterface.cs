using System;
using System.Linq;
using GameWork.Commands.States;
using GameWork.Interfacing;
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

        var logoutButton = menu.GetButton("LogoutButtonContainer");
        logoutButton.onClick.AddListener(OnLogoutClick);

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

    private void OnLogoutClick()
    {
        //EnqueueCommand(new LogoutCommand());
        EnqueueCommand(new PreviousStateCommand());
    }


    public override void Enter()
    {
        _mainMenuPanel.SetActive(true);
    }

    public override void Exit()
    {
        _mainMenuPanel.SetActive(false);
    }

    public void OnJoinGameSuccess()
    {
        EnqueueCommand(new NextStateCommand());
    }
}