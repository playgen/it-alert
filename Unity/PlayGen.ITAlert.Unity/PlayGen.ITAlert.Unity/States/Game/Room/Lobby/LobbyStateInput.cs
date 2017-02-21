using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.States.Game.Room.Playing;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;
using PlayGen.Photon.Unity.Client.Voice;
using UnityEngine;
using UnityEngine.UI;
using Logger = PlayGen.Photon.Unity.Logger;
using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Lobby
{
	public class LobbyStateInput : TickStateInput
	{
		private readonly Client _photonClient;
		private readonly List<Color> _playerColors = new List<Color>();

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
		private Button _backButton;

		public event Action LeaveLobbyClickedEvent;

		public LobbyStateInput(Client photonClient)
		{
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

		}

		protected override void OnEnter()
		{
			_ready = false;

			_photonClient.CurrentRoom.PlayerListUpdatedEvent += OnPlayersChanged;

			SetRoomMax(Convert.ToInt32(_photonClient.CurrentRoom.RoomInfo.maxPlayers));
			SetRoomName(_photonClient.CurrentRoom.RoomInfo.name);

			_readyButton.gameObject.GetComponentInChildren<Text>().text = "READY";
			_lobbyPanel.SetActive(true);
			_buttons.BestFit();

			_backButton.onClick.AddListener(OnBackButtonClick);
			_readyButton.onClick.AddListener(OnReadyButtonClick);
			_changeColorButton.onClick.AddListener(OnColorChangeButtonClick);

			LoadingUtility.HideSpinner();
		}

		protected override void OnExit()
		{
			if (_photonClient.CurrentRoom != null)
			{
				_photonClient.CurrentRoom.PlayerListUpdatedEvent -= OnPlayersChanged;
			}

			_backButton.onClick.RemoveListener(OnBackButtonClick);
			_readyButton.onClick.RemoveListener(OnReadyButtonClick);
			_changeColorButton.onClick.RemoveListener(OnColorChangeButtonClick);
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
			var currentlyReady = _photonClient.CurrentRoom.Player.State ==
								(int) ITAlert.Photon.Players.ClientState.Ready;
			CommandQueue.AddCommand(new ReadyPlayerCommand(!currentlyReady));
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
		
		private void UpdatePlayerList(List<Player> players)
		{
			_readyButton.gameObject.GetComponentInChildren<Text>().text = 
				_photonClient.CurrentRoom.Player.State == (int) ITAlert.Photon.Players.ClientState.Ready
				? "Waiting"
				: "Ready";

			// todo give this a condition list that gets subtracted from as conditions are fulfilled
			// so the spinner will remain active in a case where a player had it show for ready state change
			// and some other state change, where at this point in code the ready state has returned so that
			// condition is fulfilled but it should still remain active as we are still waiting on the other
			// state to return.
			LoadingUtility.HideSpinner();

			foreach (Transform child in _playerListObject.transform)
			{
				Object.Destroy(child.gameObject);
			}

			var offset = 0f;
			var maximumPlayersPossible = 6f;
				// the maximum number of players the game currently supports - Not the max for the room
			var height = _playerListObject.GetComponent<RectTransform>().rect.height / maximumPlayersPossible;
			_playerVoiceIcons = new Dictionary<int, Image>();

			foreach (var player in players)
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
				readyText.text = player.State == (int)ITAlert.Photon.Players.ClientState.Ready ? "Ready" : "Waiting";
				readyText.color = color;

				var soundIcon = playerItem.FindChild("SoundIcon").GetComponent<Image>();
				soundIcon.color = color;
				_playerVoiceIcons[player.PhotonId] = soundIcon;

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

			for (var i = players.Count; i < _lobbyPlayerMax; i++)
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

		private void SetPlayerColors(List<Player> players)
		{
			_playerColors.Clear();

			Color color;
			foreach (var colorString in players.Select(p => p.Color))
			{
				if (ColorUtility.TryParseHtmlString(colorString, out color))
				{
					_playerColors.Add(color);
				}
				else
				{
					Logger.LogError($"Couldn't parse {colorString} to {typeof(Color)}");
				}
			}
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

		private void OnPlayersChanged(List<Player> players)
		{
			UpdatePlayerList(players);
			SetPlayerColors(players);
		}
	}
}