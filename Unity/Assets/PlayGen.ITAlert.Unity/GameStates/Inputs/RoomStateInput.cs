using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;
using PlayGen.Photon.Unity.Client.Voice;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.GameStates.Input
{
	public class RoomStateInput : TickStateInput
	{
		private readonly Client _photonClient;
		private GameObject _chatPanel;
		private GameObject _playerChatItemPrefab;
		private Dictionary<int, string> _playerColors;
		private Dictionary<int, Image> _playerVoiceIcons;

		public RoomStateInput(Client photonClient)
		{
			_photonClient = photonClient;
		}

		protected override void OnInitialize()
		{
			_chatPanel = GameObjectUtilities.FindGameObject("Voice/VoicePanelContainer").gameObject;
			_playerChatItemPrefab = Resources.Load("PlayerChatEntry") as GameObject;
		}

		protected override void OnEnter()
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent += InitializePlayers;
		}

		protected override void OnExit()
		{
			_chatPanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			UpdateChatPanel();
		}

		private void InitializePlayers(List<Player> players)
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent -= InitializePlayers;
			SetPlayerColors(players);
			PopulateChatPanel(players);
		}

		private void PopulateChatPanel(ICollection<Player> players)
		{
			foreach (Transform child in _chatPanel.transform)
			{
				GameObject.Destroy(child.gameObject);
			}

			_chatPanel.SetActive(true);

			var offset = 0f;
			var maximumPlayersPossible = 6f;
				// the maximum number of players the game currently supports - Not the max for the room
			var height = _chatPanel.GetComponent<RectTransform>().rect.height / maximumPlayersPossible;

			//sort array into list
			var playersList = players.OrderBy(player => player.PhotonId).ToList();

			_playerVoiceIcons = new Dictionary<int, Image>();

			foreach (var player in playersList)
			{
				var playerItem = UnityEngine.Object.Instantiate(_playerChatItemPrefab).transform;

				var color = new Color();
				ColorUtility.TryParseHtmlString("#" + player.Color, out color);

				var nameText = playerItem.FindChild("Name").GetComponent<Text>();
				nameText.text = player.Name;
				nameText.color = color;

				var soundIcon = playerItem.FindChild("SoundIcon").GetComponent<Image>();
				soundIcon.color = nameText.color;
				_playerVoiceIcons[player.PhotonId] = soundIcon;

				playerItem.SetParent(_chatPanel.transform, false);

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

		private void SetPlayerColors(ICollection<Player> players)
		{
			_playerColors = new Dictionary<int, string>();

			foreach (var player in players)
			{
				_playerColors.Add(player.PhotonId, player.Color);
			}
		}

		private void UpdateChatPanel()
		{
			foreach (var status in VoiceClient.TransmittingStatuses)
			{
				_playerVoiceIcons[status.Key].enabled = status.Value;
			}
		}
	}
}