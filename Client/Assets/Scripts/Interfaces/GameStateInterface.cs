using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameWork.Interfacing;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client.Voice;
using UnityEngine.UI;

public class GameStateInterface : StateInterface
{
    private GameObject _chatPanel;
    private GameObject _playerChatItemPrefab;
    private Dictionary<int, string> _playerColors;
    private Dictionary<int, Image> _playerVoiceIcons;

    public override void Initialize()
    {
        _chatPanel = GameObject.Find("Chat").gameObject;
        _playerChatItemPrefab = Resources.Load("PlayerChatEntry") as GameObject;

    }

    public override void Enter()
    {
    }

    public override void Exit()
    {

    }

    public void PopulateChatPanel(PhotonPlayer[] players)
    {
        foreach (Transform child in _chatPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var offset = 0f;
        var maximumPlayersPossible = 6f; // the maximum number of players the game currently supports - Not the max for the room
        var height = _chatPanel.GetComponent<RectTransform>().rect.height / maximumPlayersPossible;

        //sort array into list
        var playersList = players.OrderBy(player => player.ID).ToList();

        _playerVoiceIcons = new Dictionary<int, Image>();

        foreach (var player in playersList)
        {
            var playerItem = Object.Instantiate(_playerChatItemPrefab).transform;

            var nameText = playerItem.FindChild("Name").GetComponent<Text>();
            nameText.text = player.name;
            nameText.color = GetColorForPlayerNumber(player.ID);

            var soundIcon = playerItem.FindChild("SoundIcon").GetComponent<Image>();
            soundIcon.color = nameText.color;
            _playerVoiceIcons[player.ID] = soundIcon;

            playerItem.SetParent(_chatPanel.transform);

            // set anchors
            var playerItemRect = playerItem.GetComponent<RectTransform>();

            playerItemRect.localScale = Vector3.one;

            playerItemRect.pivot = new Vector2(0.5f, 1f);
            playerItemRect.anchorMax = Vector2.one;
            playerItemRect.anchorMin = new Vector2(0f, 1f);

            playerItemRect.offsetMin = new Vector2(0f, offset - height);
            playerItemRect.offsetMax = new Vector2(0f, offset);

            // increment the offset
            offset -= height;
        }
    }


    public Color GetColorForPlayerNumber(int playerNum)
    {
        //get the color of the player from the client
        var playerColorString = _playerColors[playerNum];

        var playerColor = new Color();
        ColorUtility.TryParseHtmlString("#" + playerColorString, out playerColor);

        return playerColor;
    }

    public void SetPlayerColors(Dictionary<int, string> colors)
    {
        _playerColors = colors;
    }

    public void UpdateChatPanel()
    {
        foreach (var status in VoiceClient.TransmittingStatuses)
        {
            _playerVoiceIcons[status.Key].enabled = status.Value;
        }
    }
}
