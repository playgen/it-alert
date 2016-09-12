using System.Linq;
using GameWork.Commands.States;
using GameWork.Interfacing;
using UnityEngine;
using UnityEngine.UI;

public class MenuStateInterface : StateInterface
{
    private GameObject _mainMenuPanel;
    private GameObject _createGamePopup;

    public override void Initialize()
    {
        // Main Menu
        _mainMenuPanel = GameObject.Find("MainMenuContainer").transform.GetChild(0).gameObject;
        var menu = new ButtonList("MainMenuContainer/MenuPanelContainer/MenuContainer");

        var logoutButton = menu.GetButton("LogoutButtonContainer");
        logoutButton.onClick.AddListener(OnLogoutClick);

        var createGameButton = menu.GetButton("CreateGameButtonContainer");
        createGameButton.onClick.AddListener(OnCreateGameClick);

        var joinGameButton = menu.GetButton("JoinGameButtonContainer");
        joinGameButton.onClick.AddListener(OnJoinGameClick);

        var quickMatchButton = menu.GetButton("QuickMatchButtonContainer");
        quickMatchButton.onClick.AddListener(OnQuickMatchClick);
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
        EnqueueCommand(new NextStateCommand());
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


}