using System;
using UnityEngine;
using GameWork.Commands.States;
using GameWork.Interfacing;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class LobbyStateInterface : StateInterface
{
    private GameObject _lobbyPanel;
    private Button _readyButton;
    private GameObject _playerListObject;
    private GameObject _playerItemPrefab;
    private GameObject _playerSpacePrefab;


    private bool _ready;
    private int _lobbyPlayerMax;

    public override void Initialize()
    {
        _lobbyPanel = GameObject.Find("LobbyContainer").transform.GetChild(0).gameObject;
        var buttons = new ButtonList("LobbyContainer/LobbyPanelContainer/ButtonPanel");

        var backButton = buttons.GetButton("BackButtonContainer");
        backButton.onClick.AddListener(OnBackButtonClick);

        _readyButton = buttons.GetButton("ReadyButtonContainer");
        _readyButton.onClick.AddListener(OnReadyButtonClick);

        _playerListObject = GameObjectUtilities.FindGameObject("LobbyContainer/LobbyPanelContainer/LobbyPanel/PlayerListContainer");
        _playerItemPrefab = Resources.Load("Prefabs/PlayerItem") as GameObject;
        _playerSpacePrefab = Resources.Load("Prefabs/PlayerSpace") as GameObject;
    }

    private void OnReadyButtonClick()
    {
        EnqueueCommand(new ReadyPlayerCommand(!_ready));
    }

    private void OnBackButtonClick()
    {
        EnqueueCommand(new PreviousStateCommand());
    }

    public override void Enter()
    {
        _lobbyPanel.SetActive(true);
    }

    public override void Exit()
    {
        _lobbyPanel.SetActive(false);
    }

    public void OnReadySucceeded()
    {
        var text = "";
        if (_ready)
        {
            text = "READY";
            _ready = false;
        }
        else
        {
            text = "WAITING";
            _ready = true;
        }
        _readyButton.gameObject.GetComponent<Text>().text = text;
    }

    public void RefreshPlayerList()
    {
        EnqueueCommand(new RefreshPlayerListCommand());
    }

    public void UpdatePlayerList(LobbyController.LobbyPlayer[] players)
    {
        foreach (Transform child in _playerListObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var offset = 0f;
        var height = _playerListObject.GetComponent<RectTransform>().rect.height / 6f;

        foreach (var player in players)
        {
            var playerItem = Object.Instantiate(_playerItemPrefab).transform;
            playerItem.FindChild("Name").GetComponent<Text>().text = player.Name;
            playerItem.FindChild("Ready").GetComponent<Text>().text = player.IsReady ? "Ready" : "Waiting";
            playerItem.SetParent(_playerListObject.transform);

            // set anchors
            var playerItemRect = playerItem.GetComponent<RectTransform>();

            playerItemRect.pivot = new Vector2(0.5f, 1f);
            playerItemRect.anchorMax = Vector2.one;
            playerItemRect.anchorMin = new Vector2(0f, 1f);

            playerItemRect.offsetMin = new Vector2(0f, offset - height);
            playerItemRect.offsetMax = new Vector2(0f, offset);

            // increment the offset
            offset -= height;


        }

        for (var i = players.Length; i < _lobbyPlayerMax; i++)
        {
            var playerSpace = Object.Instantiate(_playerSpacePrefab).transform;
            playerSpace.SetParent(_playerListObject.transform);

            // set anchors
            var playerSpaceRect = playerSpace.GetComponent<RectTransform>();

            playerSpaceRect.pivot = new Vector2(0.5f, 1f);
            playerSpaceRect.anchorMax = Vector2.one;
            playerSpaceRect.anchorMin = new Vector2(0f, 1f);

            playerSpaceRect.offsetMin = new Vector2(0f, offset - height);
            playerSpaceRect.offsetMax = new Vector2(0f, offset);

            // increment the offset
            offset -= height;
        }
    }

    public void SetRoomMax(int currentRoomMaxPlayers)
    {
        _lobbyPlayerMax = currentRoomMaxPlayers;
    }
}

