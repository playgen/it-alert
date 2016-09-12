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

        

        // Create Game Popup
        _createGamePopup = GameObjectUtilities.FindGameObject("MainMenuContainer/CreateGamePopup");
        var popUpButtons = new ButtonList("MainMenuContainer/CreateGamePopup/ButtonPanel");

        var createGameCloseButton = popUpButtons.GetButton("CloseButtonContainer");
        createGameCloseButton.onClick.AddListener(OnClosePopupClick);

        var createGamePopupButton = popUpButtons.GetButton("CreateButtonContainer");
        createGamePopupButton.onClick.AddListener(OnCreateClick);
        // Create Game Listener Goes Here

    }

    private void OnCreateClick()
    {
        var details = _createGamePopup.GetComponent<CreateGamePopupBehaviour>().GetGameDetails();
        EnqueueCommand(new CreateGameCommand(details.GameName, details.MaxPlayers));
        OnClosePopupClick();
    }

    private void OnCreateGameClick()
    {
        _createGamePopup.SetActive(true);
    }

    private void OnClosePopupClick()
    {
        _createGamePopup.SetActive(false);
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

    public void OnGameListSuccess(RoomInfo[] rooms)
    {
        // Populate Game list UI
        foreach (var room in rooms)
        {
            var gameItem = Object.Instantiate(_gameItemPrefab).transform;
            gameItem.FindChild("Name").GetComponent<Text>().text = room.name;
            gameItem.FindChild("Players").GetComponent<Text>().text = room.playerCount.ToString() + "/" + room.maxPlayers.ToString();
            gameItem.SetParent(_gameListObject.transform);
        }
    }

    public void OnCreateGameSuccess()
    {
        Debug.Log("Create Game Success!");
    }
}