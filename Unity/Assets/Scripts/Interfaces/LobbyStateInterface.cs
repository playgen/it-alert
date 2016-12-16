using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using UnityEngine;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Network.Client.Voice;
using PlayGen.ITAlert.Photon.Players;

using UnityEngine.UI;
using Object = UnityEngine.Object;

public class LobbyStateInterface : StateInterface
{
	private GameObject _lobbyPanel;
	private ButtonList _buttons;
	private Button _readyButton;
	private Button _changeColorButton;
	private GameObject _playerListObject;
	private GameObject _playerItemPrefab;
	private GameObject _playerSpacePrefab;
	private GameObject _roomNameObject;
	private Dictionary<int, Image> _playerVoiceIcons = new Dictionary<int, Image>();

	private bool _ready;
	private int _lobbyPlayerMax;
	private List<Color> _playerColors;

	public override void Initialize()
	{
		_lobbyPanel = GameObject.Find("LobbyContainer").transform.GetChild(0).gameObject;
		_buttons = new ButtonList("LobbyContainer/LobbyPanelContainer/ButtonPanel");

		var backButton = _buttons.GetButton("BackButtonContainer");
		backButton.onClick.AddListener(OnBackButtonClick);

		_readyButton = _buttons.GetButton("ReadyButtonContainer");
		_readyButton.onClick.AddListener(OnReadyButtonClick);

		_changeColorButton = _buttons.GetButton("ChangeColourButtonContainer");
		_changeColorButton.onClick.AddListener(OnColorChangeButtonClick);

		_roomNameObject = GameObjectUtilities.FindGameObject("LobbyContainer/LobbyPanelContainer/LobbyPanel/RoomNameContainer/RoomName");

		_playerListObject = GameObjectUtilities.FindGameObject("LobbyContainer/LobbyPanelContainer/LobbyPanel/PlayerListContainer");
		_playerItemPrefab = Resources.Load("PlayerItem") as GameObject;
		_playerSpacePrefab = Resources.Load("PlayerSpace") as GameObject;
	}

	private void OnReadyButtonClick()
	{
		EnqueueCommand(new ReadyPlayerCommand(!_ready));
	}

	private void OnBackButtonClick()
	{
		EnqueueCommand(new LeaveRoomCommand());
	}

	private void OnColorChangeButtonClick()
	{
		// Get the selected colors list from the client

		PopupUtility.ShowColorPicker(ColorPicked, _playerColors);
	}

	private void ColorPicked(Color pickedColor)
	{
		EnqueueCommand(new ChangePlayerColorCommand(ColorUtility.ToHtmlStringRGB(pickedColor)));
	}

	public void OnLeaveSuccess()
	{
		EnqueueCommand(new PreviousStateCommand());
	}

	public override void Enter()
	{
		_lobbyPanel.SetActive(true);
		_buttons.BestFit();
	}

	public override void Exit()
	{
		_lobbyPanel.SetActive(false);
	}

	public void SetRoomName(string name)
	{
		_roomNameObject.GetComponent<Text>().text = name;
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
		_readyButton.gameObject.GetComponentInChildren<Text>().text = text;
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
		var maximumPlayersPossible = 6f; // the maximum number of players the game currently supports - Not the max for the room
		var height = _playerListObject.GetComponent<RectTransform>().rect.height / maximumPlayersPossible;

		//sort array into list
		var playersList = players.OrderBy(player => player.Id).ToList();

		var index = 1;

		_playerVoiceIcons = new Dictionary<int, Image>();

		foreach (var player in playersList)
		{
			var playerItem = Object.Instantiate(_playerItemPrefab).transform;

			var color = new Color();
			ColorUtility.TryParseHtmlString("#" + player.Color, out color);

			var nameText = playerItem.FindChild("Name").GetComponent<Text>();
			nameText.text = player.Name;
			nameText.color = color;
			index++;

			var readyText = playerItem.FindChild("Ready").GetComponent<Text>();
			readyText.text = player.IsReady ? "Ready" : "Waiting";
			readyText.color = color;

			var soundIcon = playerItem.FindChild("SoundIcon").GetComponent<Image>();
			soundIcon.color = color;    
			_playerVoiceIcons[player.Id] = soundIcon;

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

	public void SetPlayerColors(Dictionary<int, string> colors)
	{
		// Convert Dictionary to string list
		var colorStrings = colors.Select(item => item.Value).ToList();
		var colorList = new List<Color>();
		var c = new Color();
		foreach (var color in colorStrings)
		{
			ColorUtility.TryParseHtmlString("#" + color, out c);
			colorList.Add(c);
		}
		_playerColors = colorList;

		EnqueueCommand(new RefreshPlayerListCommand());
	}

	public void UpdateVoiceStatuses()
	{
		foreach (var status in VoiceClient.TransmittingStatuses)
		{
			if (_playerVoiceIcons.ContainsKey(status.Key))
			{
				_playerVoiceIcons[status.Key].enabled = status.Value;
			}
		}
	}

	public void OnJoinedRoom(ClientRoom room)
	{
		room.OtherPlayerJoinedEvent += (otherPlayer) => RefreshPlayerList();
		room.OtherPlayerLeftEvent += (otherPlayer) => RefreshPlayerList();

		RefreshPlayerList();
	}

	public void OnPlayersChanged(Player[] players)
	{
		RefreshPlayerList();
		SetPlayerColors(players.ToDictionary(
			p => p.Id,
			p => p.Color));
	}
}

