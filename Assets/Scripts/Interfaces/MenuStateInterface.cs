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

        // Join Game Popup
        _joinGamePopup = GameObjectUtilities.FindGameObject("MainMenuContainer/JoinGamePopup");
        var joinGameCloseButton =
            GameObjectUtilities.FindGameObject("MainMenuContainer/JoinGamePopup/ButtonPanel/CloseButtonContainer/CloseButton")
                .GetComponent<Button>();
        joinGameCloseButton.onClick.AddListener(OnClosePopupClick);

        // Create Game Popup
        _createGamePopup = GameObjectUtilities.FindGameObject("MainMenuContainer/CreateGamePopup");
        var popUpButtons = new ButtonList("MainMenuContainer/CreateGamePopup/ButtonPanel");

        var createGameCloseButton = popUpButtons.GetButton("CloseButtonContainer");
        createGameCloseButton.onClick.AddListener(OnClosePopupClick);

        var createGamePopupButton = popUpButtons.GetButton("CreateButtonContainer");
        // Create Game Listener Goes Here

    }

    private void OnCreateGameClick()
    {
        _createGamePopup.SetActive(true);
    }

    private void OnClosePopupClick()
    {
        _joinGamePopup.SetActive(false);
        _createGamePopup.SetActive(false);
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