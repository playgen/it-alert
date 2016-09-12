using UnityEngine;
using GameWork.Commands.States;
using GameWork.Interfacing;
using UnityEngine.UI;

public class GamesListStateInterface : StateInterface
{
    private GameObject _joinGamePanel;
    private GameObject _gameListObject;
    private GameObject _gameItemPrefab;

    public override void Initialize()
    {
        // Join Game Popup
        _joinGamePanel = GameObjectUtilities.FindGameObject("JoinGameContainer/JoinPanelContainer");
        var panelButtons = new ButtonList("JoinGameContainer/JoinPanelContainer/ButtonPanel");

        var backButton = panelButtons.GetButton("BackButtonContainer");
        backButton.onClick.AddListener(OnBackClick);

        var refreshButton = panelButtons.GetButton("RefreshButtonContainer");
        refreshButton.onClick.AddListener(OnRefreshClick);

        _gameListObject = GameObjectUtilities.FindGameObject("JoinGameContainer/JoinPanelContainer/GameListContainer/Viewport/Content");
        _gameItemPrefab = Resources.Load("Prefabs/GameItem") as GameObject;
    }

    private void OnRefreshClick()
    {
        EnqueueCommand(new RefreshGamesListCommand());
    }

    private void OnBackClick()
    {
        EnqueueCommand(new PreviousStateCommand());
    }

    public override void Enter()
    {
        _joinGamePanel.SetActive(true);
        OnRefreshClick();
    }

    public override void Exit()
    {
        _joinGamePanel.SetActive(false);
    }

    public void OnGamesListSuccess(RoomInfo[] rooms)
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
}
