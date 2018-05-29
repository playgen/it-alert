using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client.Voice;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;
using PlayGen.ITAlert.Photon.Common;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Components;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.Unity.Utilities.Extensions;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Lobby
{
	public class LobbyStateInput : TickStateInput
	{
		private readonly ITAlertPhotonClient _photonClient;
		private List<ITAlertPlayer> _players;

		private GameObject _lobbyPanel;
		private ButtonList _buttons;
		private Button _readyButton;
		private Button _changeColorButton;
		private GameObject _playerListObject;
		private GameObject _playerItemPrefab;
		private GameObject _playerSpacePrefab;
		private GameObject _roomNameObject;
		private Dictionary<int, Image> _playerVoiceIcons = new Dictionary<int, Image>();

		private int _lobbyPlayerMax;
		private Button _backButton;
		private bool _readyPulseDown;

		public event Action LeaveLobbyClickedEvent;

		private readonly PopupController _popupController;

		public LobbyStateInput(ITAlertPhotonClient photonClient)
		{
			_photonClient = photonClient;
			_popupController = new PopupController(_photonClient);
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
			_photonClient.CurrentRoom.PlayerListUpdatedEvent += OnPlayersChanged;

			SetRoomMax(Convert.ToInt32(_photonClient.CurrentRoom.RoomInfo.maxPlayers));
			SetRoomName(_photonClient.CurrentRoom.RoomInfo.name + " - " + _photonClient.CurrentRoom.RoomInfo.customProperties[CustomRoomSettingKeys.GameScenario]);

			_readyButton.gameObject.GetComponentInChildren<Text>().text = Localization.Get("LOBBY_BUTTON_READY");
			_readyButton.GetComponent<Image>().color = Color.white;
			_lobbyPanel.SetActive(true);
			_buttons.Buttons.BestFit();

			_backButton.onClick.AddListener(OnBackButtonClick);
			_readyButton.onClick.AddListener(OnReadyButtonClick);
			_changeColorButton.onClick.AddListener(OnColorChangeButtonClick);

			PlayGen.Unity.Utilities.Loading.Loading.Stop();
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
            if (_popupController.Active)
            {
                _popupController.PopupClosed();
            }
            _lobbyPanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (_photonClient.CurrentRoom.Player.State == (int)ITAlert.Photon.Players.ClientState.Ready)
			{
				_readyButton.GetComponent<Image>().color += new Color(0, 0, 0, _readyPulseDown ? -Time.deltaTime : Time.deltaTime);
				if (_readyButton.GetComponent<Image>().color.a < 0 && _readyPulseDown)
				{
					_readyPulseDown = false;
				}
				if (_readyButton.GetComponent<Image>().color.a > 1 && !_readyPulseDown)
				{
					_readyPulseDown = true;
				}
			}
			else
			{
				_readyButton.GetComponent<Image>().color = Color.white;
			}
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
			var currentlyReady = _photonClient.CurrentRoom.Player.State == (int) ITAlert.Photon.Players.ClientState.Ready;
			PlayGen.Unity.Utilities.Loading.Loading.Start();
			CommandQueue.AddCommand(new ReadyPlayerCommand(!currentlyReady));
		}

		private void OnBackButtonClick()
		{
			LeaveLobbyClickedEvent?.Invoke();
		}

		private void OnColorChangeButtonClick()
		{
			// Get the selected colors list from the client
			_popupController.ShowColorPickerPopup(PlayerColourChanged, _players, _players.Single(p => p.PhotonId == _photonClient.CurrentRoom.Player.PhotonId));
		}

		private void PlayerColourChanged(PlayerColour playerColour)
		{
			CommandQueue.AddCommand(new ChangePlayerColorCommand(playerColour));
		}
		
		private void SetRoomName(string name)
		{
			_roomNameObject.GetComponent<Text>().text = name;
		}
		
		private void UpdatePlayerList(List<ITAlertPlayer> players)
		{
			_players = players;

			_readyButton.gameObject.GetComponentInChildren<Text>().text =
				Localization.Get(_photonClient.CurrentRoom.Player.State == (int) ITAlert.Photon.Players.ClientState.Ready
				? "LOBBY_LABEL_WAITING"
				: "LOBBY_BUTTON_READY");

			// todo give this a condition list that gets subtracted from as conditions are fulfilled
			// so the spinner will remain active in a case where a player had it show for ready state change
			// and some other state change, where at this point in code the ready state has returned so that
			// condition is fulfilled but it should still remain active as we are still waiting on the other
			// state to return.
			PlayGen.Unity.Utilities.Loading.Loading.Stop();

			foreach (Transform child in _playerListObject.transform)
			{
				UnityEngine.Object.Destroy(child.gameObject);
			}

			var offset = 0f;
			var maximumPlayersPossible = 6f;
				// the maximum number of players the game currently supports - Not the max for the room
			var height = _playerListObject.RectTransform().rect.height / maximumPlayersPossible;
			_playerVoiceIcons = new Dictionary<int, Image>();

			foreach (var player in players)
			{
				var playerItem = UnityEngine.Object.Instantiate(_playerItemPrefab).transform;

				ColorUtility.TryParseHtmlString(player.Colour, out var color);
				if (color == Color.white)
				{
					ColorUtility.TryParseHtmlString("#" + player.Colour, out color);
				}

				var glyphImage = playerItem.Find("Glyph").GetComponent<Image>();
				glyphImage.sprite = Resources.Load<Sprite>($"playerglyph_{player.Glyph}");

				var colorImage = playerItem.Find("Color").GetComponent<Image>();
				colorImage.color = color;

				var nameText = playerItem.Find("Name").GetComponent<Text>();
				nameText.GetComponent<TextCutoff>().text = player.Name;
				nameText.color = color;

				var readyText = playerItem.Find("Ready").GetComponent<Text>();
				readyText.text = Localization.Get(player.State == (int)ITAlert.Photon.Players.ClientState.Ready ? "LOBBY_BUTTON_READY" : "LOBBY_LABEL_WAITING");
				readyText.color = color;

				var soundIcon = playerItem.Find("SoundIcon").GetComponent<Image>();
				soundIcon.color = color;
				_playerVoiceIcons[player.PhotonId] = soundIcon;

				playerItem.SetParent(_playerListObject.transform, false);

				// set anchors
				var playerItemRect = playerItem.RectTransform();

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
				var playerSpace = UnityEngine.Object.Instantiate(_playerSpacePrefab).transform;
				playerSpace.SetParent(_playerListObject.transform, false);

				// set anchors
				var playerSpaceRect = playerSpace.RectTransform();

				playerSpaceRect.pivot = new Vector2(0.5f, 1f);
				playerSpaceRect.anchorMax = Vector2.one;
				playerSpaceRect.anchorMin = new Vector2(0f, 1f);

				playerSpaceRect.offsetMin = new Vector2(0f, offset - height);
				playerSpaceRect.offsetMax = new Vector2(0f, offset);

				// increment the offset
				offset -= height;
			}
			_playerListObject.BestFit();
		}

		private void SetRoomMax(int currentRoomMaxPlayers)
		{
			_lobbyPlayerMax = currentRoomMaxPlayers;
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

		private void OnPlayersChanged(List<ITAlertPlayer> players)
		{
			UpdatePlayerList(players);

			if (players.All(player => player.State == (int) ITAlert.Photon.Players.ClientState.Ready))
			{
				PlayGen.Unity.Utilities.Loading.Loading.Start();
			}
			else
			{
				PlayGen.Unity.Utilities.Loading.Loading.Stop();
			}
		}
	}
}