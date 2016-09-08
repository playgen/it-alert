using System.Linq;
using GameWork.Commands;
using GameWork.Commands.Interfaces;
using GameWork.Interfacing;
using UnityEngine;
using UnityEngine.UI;

public class MenuStateInterface : StateInterface
{
    private GameObject _mainMenuPanel;
    private GameObject _joinGamePopup;

    public override void Initialize()
    {
        // Main Menu
        _mainMenuPanel = GameObject.Find("MainMenuContainer").transform.GetChild(0).gameObject;
        var menu = new ButtonList("MainMenuContainer/MenuPanelContainer/MenuContainer");

        var logoutButton = menu.GetButton("LogoutButtonContainer");
        logoutButton.onClick.AddListener(OnLogoutClick);

        var joinGameButton = menu.GetButton("JoinGameButtonContainer");
        joinGameButton.onClick.AddListener(OnJoinGameClick);

        var quickMatchButton = menu.GetButton("QuickMatchButtonContainer");
        quickMatchButton.onClick.AddListener(OnQuickMatchClick);

        // Join Game Popup
        _joinGamePopup =
            GameObjectUtilities.FindGameObject("MainMenuContainer/JoinGamePopup");
        var joinGameCloseButton =
            GameObjectUtilities.FindGameObject("MainMenuContainer/JoinGamePopup/ButtonPanel/CloseButtonContainer/CloseButton")
                .GetComponent<Button>();
        joinGameCloseButton.onClick.AddListener(OnCloseJoinGamePopupClick);

    }

    private void OnCloseJoinGamePopupClick()
    {
        _joinGamePopup.SetActive(false);
    }

    private void OnJoinGameClick()
    {
        _joinGamePopup.SetActive(true);
        EnqueueCommand(new RefreshGamesListCommand());
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
        _joinGamePopup.SetActive(false);
        _mainMenuPanel.SetActive(false);
    }

    public void OnGameListSuccess(object[] objects)
    {
        // Populate Game list UI
        foreach (var @object in objects)
        {
            //_joinGamePopup.listofgames.add(@object);
        }
    }
}