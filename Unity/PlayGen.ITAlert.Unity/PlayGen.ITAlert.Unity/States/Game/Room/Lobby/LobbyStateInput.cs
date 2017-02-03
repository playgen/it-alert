using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;
using PlayGen.Photon.Unity.Client.Voice;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Lobby
{
	public class LobbyStateInput : TickStateInput
	{
		private readonly LobbyController _controller;
		private readonly Client _photonClient;

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
		private Button _backButton;

		public event Action LeaveLobbyClickedEvent;

		public LobbyStateInput(LobbyController controller, Client photonClient)
		{
			_controller = controller;
			_photonClient = photonClient;
		}

		protected override void OnInitialize()
		{
			_lobbyPanel = GameObject.Find("LobbyContainer").transform.GetChild(0).gameObject;
			_buttons = new ButtonList("LobbyContainer/LobbyPanelContainer/ButtonPanel");

			_backButton = _buttons.GetButton("BackButtonContainer");
			_readyButton = _buttons.GetButton("ReadyButtonContainer");
			_changeColorButton = _buttons.GetButton("ChangeColourButtonContainer");
			
			_roomNameObject = GameObjectUtilities.FindGameObject("LobbyContainer/LobbyPanelContainer/LobbyPanel/RoomNameContainer/RoomName");
			_playerListObject = GameObjectUtilities.FindGameObject("LobbyContainer/LobbyPanelContainer/LobbyPanel/PlayerListContainer");

			_playerItemPrefab = Resources.Load("PlayerItem") as GameObject;
			_playerSpacePrefab = Resources.Load("PlayerSpace") as GameObject;

			_backButton.onClick.AddListener(OnBackButtonClick);
			_readyButton.onClick.AddListener(OnReadyButtonClick);
			_changeColorButton.onClick.AddListener(OnColorChangeButtonClick);
		}

		protected override void OnTerminate()
		{
			_backButton.onClick.RemoveListener(OnBackButtonClick);
			_readyButton.onClick.RemoveListener(OnReadyButtonClick);
			_changeColorButton.onClick.RemoveListener(OnColorChangeButtonClick);
		}

		protected override void OnEnter()
		{
			_controller.ReadySuccessEvent += OnReadySucceeded;
			_controller.RefreshSuccessEvent += UpdatePlayerList;

			_photonClient.JoinedRoomEvent += OnJoinedRoom;
			_photonClient.CurrentRoom.PlayerListUpdatedEvent += OnPlayersChanged;

			SetRoomMax(Convert.ToInt32(_photonClient.CurrentRoom.RoomInfo.maxPlayers));
			SetRoomName(_photonClient.CurrentRoom.RoomInfo.name);

			_lobbyPanel.SetActive(true);
			_buttons.BestFit();
			LoadingUtility.HideSpinner();
		}

		protected override void OnExit()
		{
			_controller.ReadySuccessEvent -= OnReadySucceeded;
			_controller.RefreshSuccessEvent -= UpdatePlayerList;

			_photonClient.JoinedRoomEvent -= OnJoinedRoom;

			if (_photonClient.CurrentRoom != null)
			{
				_photonClient.CurrentRoom.PlayerListUpdatedEvent -= OnPlayersChanged;
			}

			_lobbyPanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			UpdateVoiceStatuses();
			if (_photonClient.ClientState != PlayGen.Photon.Unity.Client.ClientState.Connected)
			{
				OnBackButtonClick();
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnBackButtonClick();
			}
		}

		private void OnReadyButtonClick()
		{
			CommandQueue.AddCommand(new ReadyPlayerCommand(!_ready));
			LoadingUtility.ShowSpinner();
		}

		private void OnBackButtonClick()
		{
			LeaveLobbyClickedEvent?.Invoke();
		}

		private void OnColorChangeButtonClick()
		{
			// Get the selected colors list from the client
			PopupUtility.ShowColorPicker(ColorPicked, _playerColors);
		}

		private void ColorPicked(Color pickedColor)
		{
			CommandQueue.AddCommand(new ChangePlayerColorCommand(ColorUtility.ToHtmlStringRGB(pickedColor)));
		}
		
		private void SetRoomName(string name)
		{
			_roomNameObject.GetComponent<Text>().text = name;
		}

		private void OnReadySucceeded()
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
			LoadingUtility.HideSpinner();
			_readyButton.gameObject.GetComponentInChildren<Text>().text = text;
		}

		private void RefreshPlayerList()
		{
			CommandQueue.AddCommand(new RefreshPlayerListCommand());
		}

		private void UpdatePlayerList(LobbyController.LobbyPlayer[] players)
		{
			foreach (Transform child in _playerListObject.transform)
			{
				GameObject.Destroy(child.gameObject);
			}

			var offset = 0f;
			var maximumPlayersPossible = 6f;
				// the maximum number of players the game currently supports - Not the max for the room
			var height = _playerListObject.GetComponent<RectTransform>().rect.height / maximumPlayersPossible;

			//sort array into list
			var playersList = players.OrderBy(player => player.Id).ToList();

			_playerVoiceIcons = new Dictionary<int, Image>();

			foreach (var player in playersList)
			{
				var playerItem = Object.Instantiate(_playerItemPrefab).transform;

				Color color;
				ColorUtility.TryParseHtmlString(player.Color, out color);
				if (color == Color.white)
				{
					ColorUtility.TryParseHtmlString("#" + player.Color, out color);
				}

				var nameText = playerItem.FindChild("Name").GetComponent<Text>();
				nameText.text = player.Name;
				nameText.color = color;

				var readyText = playerItem.FindChild("Ready").GetComponent<Text>();
				readyText.text = player.IsReady ? "Ready" : "Waiting";
				readyText.color = color;

				var soundIcon = playerItem.FindChild("SoundIcon").GetComponent<Image>();
				soundIcon.color = color;
				_playerVoiceIcons[player.Id] = soundIcon;

				playerItem.SetParent(_playerListObject.transform, false);

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
				playerSpace.SetParent(_playerListObject.transform, false);

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

		private void SetRoomMax(int currentRoomMaxPlayers)
		{
			_lobbyPlayerMax = currentRoomMaxPlayers;
		}

		private void SetPlayerColors(Dictionary<int, string> colors)
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

			CommandQueue.AddCommand(new RefreshPlayerListCommand());
		}

		private void UpdateVoiceStatuses()
		{
			foreach (var status in VoiceClient.TransmittingStatuses)
			{
				if (_playerVoiceIcons.ContainsKey(status.Key))
				{
					_playerVoiceIcons[status.Key].enabled = status.Value;
				}
			}
		}

		private void OnJoinedRoom(ClientRoom room)
		{
			room.OtherPlayerJoinedEvent += (otherPlayer) => RefreshPlayerList();
			room.OtherPlayerLeftEvent += (otherPlayer) => RefreshPlayerList();

			RefreshPlayerList();
		}

		private void OnPlayersChanged(ICollection<Player> players)
		{
			RefreshPlayerList();
			SetPlayerColors(players.ToDictionary(
				p => p.PhotonId,
				p => p.Color));
		}
	}
}